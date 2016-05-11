using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using AppFramework.DataProxy;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using System.Transactions;

namespace AppFramework.Tasks
{
    public class SobImportPayments
    {
        private readonly string _sobPaymentImportHistoryFolder;
        private readonly string _sobPaymentImportFolder;
        private readonly long _sobPaymentConfigUid;
        private readonly long _sobCertificateConfigUid;
        readonly List<PaymentRecord> _paymentsToPutIntoLog = new List<PaymentRecord>();
        private readonly IUnitOfWork _unitOfWork;

        public string HistoryFolder
        {
            get;
            set;
        }

        public SobImportPayments(
            IUnitOfWork unitOfWork,
            long sobCertificateConfigUid,
            long sobPaymentConfigUid,
            string sobPaymentImportFolder,
            string sobPaymentImportHistoryFolder)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            _sobCertificateConfigUid = sobCertificateConfigUid;
            _sobPaymentConfigUid = sobPaymentConfigUid;
            _sobPaymentImportFolder = sobPaymentImportFolder;
            _sobPaymentImportHistoryFolder = sobPaymentImportHistoryFolder;
        }

        public void DoImport()
        {
            var files = FindTargetFilesToImport().ToList();
            if (files.Any())
            {
                CreateHistoryFolder();
                foreach (var file in files)
                {
                    using (var scope = new TransactionScope())
                    {
                        var payments = GetRecords(file.FullName);
                        var maxDateTrimester = payments.Max(x => x.DateTrimester);
                        ScriptComponent.Execute(maxDateTrimester, _unitOfWork);
                        payments = payments.OrderBy(x => x.RRN).ThenBy(x => x.DateTrimester).ThenBy(x => x.PaymentDate).ThenBy(x => x.NettoAmount).ThenBy(x => x.DaysPaid).ToList();
                        var dossiers = LookupDossier.Execute(_unitOfWork);
                        var paymentsHavingDossier = payments.Where(x => dossiers.ContainsKey(x.RRN)).ToList();
                        ProcessPaymentsFromFileHavingDossier(paymentsHavingDossier, dossiers);
                        ProcessPaymentsFromFileNoMatchingDossier(payments.Where(x => !dossiers.ContainsKey(x.RRN)).ToList());
                        GenerateLogFile();
                        scope.Complete();
                    }
                    CaclulateRemaningAmount();
                    ReIndexPaymentsAndCertificates();
                    MoveFileToHistoryFolder(file);
                }
            }
        }

        void LinkNewPaymentsToCert(IList<PaymentRecord> payments)
        {
            //var payments = LookupImportedPayments.GetSOBImportedPayments();
            //var singlePaymentPerTrimester = payments.Where(x => x.cnt == 1);
            var certs = LookupCertificate.GetCertificates(_unitOfWork).GroupBy(x => x.Dossier).ToDictionary(x => x.Key);
            foreach (var payment in payments)
            {
                if (certs.ContainsKey(payment.Dossier.DossierDynEntityId))
                {
                    PaymentRecord tmp = payment;
                    var cert = certs[payment.Dossier.DossierDynEntityId].FirstOrDefault(x => x.Trimester == tmp.DateTrimester && (!x.PaymentDynEntityUid.HasValue || x.PaymentDynEntityUid == 0));
                    if (cert != null)
                    {
                        //certificate exists, link it
                        _unitOfWork.SqlProvider.ExecuteNonQuery(
                            @"UPDATE ADynEntitySOBCertificate SET Payment = @PaymentDynEntityUid WHERE DynEntityUid = @DynEntityUid",
                            new SqlParameter[]
                            {
                                new SqlParameter("@PaymentDynEntityUid", payment.DynEntityUid)
                                {
                                    SqlDbType = SqlDbType.BigInt
                                },
                                new SqlParameter("@DynEntityUid", cert.DynEntityUid) {SqlDbType = SqlDbType.BigInt},
                            });
                    }
                    else
                    {
                        //create new certificate
                        var name = String.Format(@"Cert {0}, {1}, {2}", payment.DateTrimester, payment.Name, payment.RRN);
                        var year = payment.DateTrimester/10;
                        var quarter = payment.DateTrimester - (payment.DateTrimester/10)*10;
                        var dateFrom = new DateTime(year, ((quarter - 1)*3) + 1, 1);
                        var dateUntil = new DateTime(year, quarter*3, 1).AddMonths(1).AddDays(-1);
                        _unitOfWork.SqlProvider.ExecuteNonQuery(@"INSERT INTO [ADynEntitySOBCertificate]
                               ([ActiveVersion]
                               ,[DynEntityConfigUid]
                               ,[Name]
                               ,[Revision]
                               ,[UpdateUserId]
                               ,[UpdateDate]
                               ,[Dossier]
                               ,[Date_from]
                               ,[Date_until]
                               ,[Printed]
                               ,[Comments]
                               ,[Total_days]
                               ,[Payment]
                               ,[Trimester])
                               VALUES
                               (1,
                               @SobCertificateConfigUid,
                               @Name,
                               1,
                               1,
                               @UpdateDate,
                               @Dossier,
                               @Date_from,
                               @Date_until,
                               1,
                               N'Certificate created when importing payments',
                               0,
                               @PaymentDynEntityUid,
                               @Trimester)",
                            new SqlParameter[]
                            {
                                new SqlParameter("@SobCertificateConfigUid", _sobCertificateConfigUid)
                                {
                                    SqlDbType = SqlDbType.BigInt
                                },
                                new SqlParameter("@Name", name) {SqlDbType = SqlDbType.NVarChar},
                                new SqlParameter("@UpdateDate", DateTime.Now) {SqlDbType = SqlDbType.DateTime},
                                new SqlParameter("@Dossier", payment.Dossier.DossierDynEntityId)
                                {
                                    SqlDbType = SqlDbType.BigInt
                                },
                                new SqlParameter("@Date_from", dateFrom) {SqlDbType = SqlDbType.DateTime},
                                new SqlParameter("@Date_until", dateUntil) {SqlDbType = SqlDbType.DateTime},
                                new SqlParameter("@PaymentDynEntityUid", payment.DynEntityUid)
                                {
                                    SqlDbType = SqlDbType.BigInt
                                },
                                new SqlParameter("@Trimester", payment.DateTrimester) {SqlDbType = SqlDbType.Int},
                            });
                    }
                }
            }
        }


        void MoveFileToHistoryFolder(FileInfo file)
        {
            String uniqueName = String.Format("{0}-{1}", DateTime.Now.Ticks, file.Name);
            File.Move(file.FullName, String.Format(@"{0}\{1}", HistoryFolder, uniqueName));
        }

        void GenerateLogFile()
        {
            if (_paymentsToPutIntoLog.Count > 0)
            {
                var now = DateTime.Now;
                var path = String.Format(@"{0}\{1}{2:00}{3:00}_DossiersNotFound.txt", _sobPaymentImportHistoryFolder, now.Year, now.Month, now.Day);
                bool needToAddHeader = !new FileInfo(path).Exists || new FileInfo(path).Length == 0;
                using (StreamWriter writer = new StreamWriter(path, append: true))
                {
                    if (needToAddHeader)
                    {
                        writer.WriteLine(@"ImportStatus,RRN,Name,Address,Zip,City,C,Country,Vacantex,Cat,Bit,PaymentDate,BankAccount,NettoAmount,DaysPaid,OpenSaldoDays,DateTrimester");
                    }
                    foreach (var p in _paymentsToPutIntoLog)
                    {
                        var logEntry = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}",
                            p.ImportStatus, p.RRN, p.Name, p.Address, p.Zip, p.City, p.C, p.Country, p.Vacantex, p.Cat, p.Bit, p.PaymentDate, p.BankAccount, p.NettoAmount, p.DaysPaid, p.OpenSaldoDays, p.DateTrimester);
                        writer.WriteLine(logEntry);
                    }
                }
            }
        }

        void ProcessPaymentsFromFileNoMatchingDossier(IList<PaymentRecord> paymentsNoMatchingDossier)
        {
            foreach (var payment in paymentsNoMatchingDossier)
            {
                payment.ImportStatus = "Dossier werd niet gevonden";
            }
            _paymentsToPutIntoLog.AddRange(paymentsNoMatchingDossier);
        }

        void ProcessPaymentsAlreadyInStorage(IEnumerable<PaymentRecord> payments)
        {
            //check whether payment from file and payment in the DB are 100% match
            foreach (var payment in payments)
            {
                if (payment.ExistingPayments.Count == 1)
                {
                    payment.MatchExistingPayment = (payment.DaysPaid == payment.ExistingPayments.First().lu_Days && payment.NettoAmount == payment.ExistingPayments.First().lu_Amount && payment.datPaymentDate.Date == payment.ExistingPayments.First().lu_Payment_date.Date);
                }
                else if (payment.ExistingPayments.Count > 1)
                {
                    payment.MatchExistingPayment = (payment.ExistingPayments.Sum(x => x.lu_Days) >= payment.DaysPaid);
                }
                if (!payment.MatchExistingPayment && (payment.ExistingPayments.Sum(x => x.lu_Days) < payment.DaysPaid))
                {
                    //new payment arrived having days payed more, than the sum of days payed in the DB for particular (dossier, trimester)
                    //need to add new payment in this case
                    payment.ForceAddPayment = true;
                    payment.DaysPaid -= payment.ExistingPayments.Sum(x => x.lu_Days);
                    payment.NettoAmount -= payment.ExistingPayments.Sum(x => x.lu_Amount);
                }
                else if (!payment.MatchExistingPayment && (payment.ExistingPayments.Sum(x => x.lu_Days) > payment.DaysPaid))
                {
                    //new payment arrived having days payed less, than the sum of days payed in the DB for particular (dossier, trimester)
                    //need to add new payment with negative days paid value
                    payment.ForceAddPayment = true;
                    payment.DaysPaid -= payment.ExistingPayments.Sum(x => x.lu_Days);
                    payment.NettoAmount -= payment.ExistingPayments.Sum(x => x.lu_Amount);
                }

                if (payment.MatchExistingPayment)
                {
                    payment.ImportStatus = "Betaling is reeds geïmporteerd";
                    _paymentsToPutIntoLog.Add(payment);
                }
                else
                {
                    //using (var unitOfWork = new AppFramework.DataProxy.UnitOfWork())
                    //{
                    //    unitOfWork.SqlProvider.ExecuteNonQuery(@"EXEC _cust_UpdateSOBPayment @PayDynEntityId, @Amount, @Days, @PaymentDate",
                    //       new SqlParameter[] 
                    //       { 
                    //           new SqlParameter("@PayDynEntityId", payment.ExistingPayments.First().lu_PayemntEntityId) { SqlDbType = SqlDbType.BigInt },
                    //           new SqlParameter("@Amount", payment.ExistingPayments.First().lu_Amount) { SqlDbType = SqlDbType.Money },
                    //           new SqlParameter("@Days", payment.ExistingPayments.First().lu_Days) { SqlDbType = SqlDbType.Int },
                    //           new SqlParameter("@PaymentDate", payment.ExistingPayments.First().lu_Payment_date) { SqlDbType = SqlDbType.DateTime },
                    //       });
                    //}
                }
            }
        }

        private void ProcessPaymentsFromFileHavingDossier(IList<PaymentRecord> paymentsHavingDossier,
            IDictionary<long, LookupDossier.Dossier> dossiers)
        {
            ProcessNettoAmount(paymentsHavingDossier);
            //get payments from the DB and group them by dossier number
            var paymentsFromStorage = LookupPayments.Execute(_unitOfWork).GroupBy(x => x.Dossier).ToDictionary(x => x.Key);
            //look for payments from file that already have matching payments in the DB
            foreach (var payment in paymentsHavingDossier)
            {
                var existingPaymentGroups =
                    paymentsFromStorage.Where(
                        x =>
                            x.Key == dossiers[payment.RRN].DossierDynEntityId &&
                            paymentsFromStorage[dossiers[payment.RRN].DossierDynEntityId].Any(
                                p => p.Trimester == payment.DateTrimester)).
                        Select(x => x.Value).
                        ToList();
                if (existingPaymentGroups.Any())
                {
                    var existingPayments = existingPaymentGroups.First().Select(x => x).ToList();
                        //take the first element as long as actually there's only one element in there matching dossiers[payment.RRN].DossierDynEntityId
                    payment.ExistingPayments =
                        existingPayments.Where(p => p.Trimester == payment.DateTrimester).ToList();
                }
            }
            ProcessPaymentsAlreadyInStorage(
                paymentsHavingDossier.Where(
                    x =>
                        paymentsFromStorage.ContainsKey(dossiers[x.RRN].DossierDynEntityId) &&
                        paymentsFromStorage[dossiers[x.RRN].DossierDynEntityId].Any(p => p.Trimester == x.DateTrimester))
                    .ToList());
            //look for payments from file that don't have matching payments in the DB yet
            var paymentsToBeAdded = paymentsHavingDossier.
                Where(
                    x =>
                        !paymentsFromStorage.ContainsKey(dossiers[x.RRN].DossierDynEntityId) ||
                        paymentsFromStorage[dossiers[x.RRN].DossierDynEntityId].All(p => p.Trimester != x.DateTrimester))
                .
                Union(paymentsHavingDossier.Where(x => x.ForceAddPayment)).
                ToList();
            foreach (var paymentRecord in paymentsToBeAdded)
            {
                var dossier = dossiers[paymentRecord.RRN];
                paymentRecord.Dossier = dossier;
                var paymentName = String.Format(@"Payment {0}, {1}", paymentRecord.DateTrimester, dossier.DossierName);
                var uid = _unitOfWork.SqlProvider.ExecuteScalar(@"INSERT INTO [ADynEntitySOBPayment]
                       ([ActiveVersion]
                       ,[DynEntityConfigUid]
                       ,[Name]
                       ,[Revision]
                       ,[UpdateUserId]
                       ,[UpdateDate]
                       ,[Dossier]
                       ,[Payment_date]
                       ,[Amount]
                       ,[Comments]
                       ,[Days]
                       ,[Trimester])
                       VALUES 
                       (1,
                       @DynEntityConfigUid,
                       @Name,
                       1,
                       1,
                       @UpdateDate,
                       @DossierDynEntityId,
                       @datPaymentDate,
                       @Amount,
                       @Comments,
                       @Days,
                       @Trimester);
                       SELECT @@IDENTITY;",
                    new SqlParameter[]
                    {
                        new SqlParameter("@DynEntityConfigUid", _sobPaymentConfigUid) {SqlDbType = SqlDbType.Int},
                        new SqlParameter("@Name", paymentName) {SqlDbType = SqlDbType.NVarChar},
                        new SqlParameter("@UpdateDate", DateTime.Now) {SqlDbType = SqlDbType.DateTime},
                        new SqlParameter("@DossierDynEntityId", dossier.DossierDynEntityId)
                        {
                            SqlDbType = SqlDbType.BigInt
                        },
                        new SqlParameter("@datPaymentDate", paymentRecord.datPaymentDate)
                        {
                            SqlDbType = SqlDbType.DateTime
                        },
                        new SqlParameter("@Amount", paymentRecord.NettoAmount) {SqlDbType = SqlDbType.Money},
                        new SqlParameter("@Comments", paymentRecord.OpenSaldoDays) {SqlDbType = SqlDbType.Text},
                        new SqlParameter("@Days", paymentRecord.DaysPaid) {SqlDbType = SqlDbType.Int},
                        new SqlParameter("@Trimester", paymentRecord.DateTrimester) {SqlDbType = SqlDbType.Int},
                    });
                paymentRecord.DynEntityUid = Convert.ToInt64(uid);
                //_logger.DebugFormat("Add ADynEntitySOBPayment: DynEntityUid: {0}, Name: {1}, Comments: {2}", 
                //    paymentRecord.DynEntityUid,
                //    paymentName,
                //    paymentRecord.OpenSaldoDays);
            }
            LinkNewPaymentsToCert(paymentsToBeAdded);
        }
        
        /// <summary>
        /// Creates (if doesn't exist yet) folder inside SOBPaymentImportHistoryFolder with the name Imported_YYYYMMDD
        /// </summary>
        void CreateHistoryFolder()
        {
            var now = DateTime.Now;
            string path = String.Format(@"{0}\Imported_{1}{2:00}{3:00}", _sobPaymentImportHistoryFolder, now.Year, now.Month, now.Day);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            HistoryFolder = path;
        }

        IEnumerable<FileInfo> FindTargetFilesToImport()
        {
            var files = Directory.GetFiles(_sobPaymentImportFolder, "*.txt");
            return files.Select(file => new FileInfo(file));
        }

        IList<PaymentRecord> GetRecords(string path)
        {
            using (FileStream f = new FileStream(path, FileMode.Open))
            {
                using (StreamReader streamReader = new StreamReader(f))
                {
                    CsvConfiguration config = new CsvConfiguration { Delimiter = ";", QuoteAllFields = true, HasHeaderRecord = false };
                    config.RegisterClassMap(new PaymentRecord());
                    using (CsvReader csvReader = new CsvReader(streamReader, config))
                    {
                        var result = new List<PaymentRecord>();
                        while (csvReader.Read())
                        {
                            var firstField = csvReader.GetField<string>(0);
                            if (firstField == "AANTAL REC:")
                            {
                                //it's the last record, it has the form "AANTAL REC:";"####" which stands for "REC NUMBER:"
                                //just skip it
                                continue;
                            }
                            var record = csvReader.GetRecord<PaymentRecord>();
                            result.Add(record);
                        }
                        return result;
                    }
                }
            }
        }

        void ProcessNettoAmount(IEnumerable<PaymentRecord> payments)
        {
            foreach (var paymentRecord in payments)
            {
                paymentRecord.NettoAmount = paymentRecord.NettoAmount / 100;
            }
        }

        private void CaclulateRemaningAmount()
        {
            _unitOfWork.SqlProvider.ExecuteNonQuery(@"
                    WITH lastpayments AS
                    (
	                    SELECT Dossier, Payment_date, MAX(Trimester) AS Trimester
	                      FROM ADynEntitySOBPayment
                         WHERE (CAST(ADynEntitySOBPayment.UpdateDate AS DATE) = CAST(@StartTime AS DATE))
	                     GROUP BY Dossier, Payment_date
                    )
                    UPDATE ADynEntitySOBDossier
                       SET ADynEntitySOBDossier.Remaining_days = CONVERT(int, CONVERT(varchar(10), ADynEntitySOBPayment.Comments))
                      FROM ADynEntitySOBDossier 
		                    INNER JOIN ADynEntitySOBPayment ON ADynEntitySOBDossier.DynEntityId = ADynEntitySOBPayment.Dossier
		                    INNER JOIN lastpayments ON ADynEntitySOBPayment.dossier = lastpayments.dossier AND 
								                       ADynEntitySOBPayment.trimester = lastpayments.trimester
                     WHERE (ADynEntitySOBPayment.ActiveVersion = 1) 
                       AND (ADynEntitySOBDossier.ActiveVersion = 1) 
                       AND (CAST(ADynEntitySOBPayment.UpdateDate AS DATE) = CAST(@StartTime AS DATE))
   
                    /*UPDATE ADynEntitySOBPayment
                       SET Comments = NULL
                     WHERE (CAST(ADynEntitySOBPayment.UpdateDate AS DATE) = CAST(@StartTime AS DATE))*/

                    /*****************************************************************************************************/

                    DECLARE @DynEntityUid bigint;
                    DECLARE @DynEntityId bigint;
                    DECLARE @DynEntityConfigUid bigint;
                    DECLARE @rows int;
                    DECLARE @i int;

                    DECLARE @tmp AS DynEntityIdsTableType
                    DECLARE @entities AS DynEntityIdsTableType

                    SET NOCOUNT ON

                    DECLARE @TEXT_AUTOCLOSED nvarchar(255) = '#Dossier auto closed when import payments';
                    DECLARE @STATE_ATTRIBCONFIGID bigint = 7103;

                    -- Get Id's from SOBDossiers to close
                    INSERT @tmp (DynEntityUid, DynEntityId, DynEntityConfigUid)
                    SELECT DISTINCT ADynEntitySOBDossier.DynEntityUid, ADynEntitySOBDossier.DynEntityId, ADynEntitySOBDossier.DynEntityConfigUid
                      FROM ADynEntitySOBDossier 
			                    INNER JOIN ADynEntitySOBPayment ON ADynEntitySOBDossier.DynEntityId = ADynEntitySOBPayment.Dossier
                     WHERE (ADynEntitySOBPayment.ActiveVersion = 1) 
                       AND (ADynEntitySOBDossier.ActiveVersion = 1) 
                       AND (ADynEntitySOBDossier.Remaining_days = 0)
                       AND (CAST(ADynEntitySOBPayment.UpdateDate AS DATE) = CAST(@StartTime AS DATE))

                    EXEC _cust_CreateNewRevision @entities = @tmp

                    -- Update the new revision and set to close
                    UPDATE DynListValue
                       SET DynListItemUid = (SELECT DynListItemUid FROM DynListItem WHERE DynListUid = 52 AND Value = 'Closed'),
                                    Value = 'Closed'
                     WHERE DynEntityAttribConfigUid = (SELECT DynEntityAttribConfigUid FROM DynEntityAttribConfig WHERE DynEntityAttribConfigId = @STATE_ATTRIBCONFIGID AND ActiveVersion = 1)
                       AND AssetUid IN 
			                    (SELECT ADynEntitySOBDossier.DynEntityUid 
			                       FROM ADynEntitySOBDossier 
			                      WHERE ActiveVersion = 1
			                        AND DynEntityId IN (SELECT DynEntityId FROM @tmp))

                    UPDATE ADynEntitySOBDossier
                       SET Comments = CONVERT(varchar(4000), ISNULL(Comments, '')) + ' ' + @TEXT_AUTOCLOSED,
                           UpdateDate = @StartTime,
                           UpdateUserId = (SELECT DynEntityId FROM ADynEntityUser WHERE Name = 'admin' AND ActiveVersion = 1)
                     WHERE DynEntityId IN (SELECT DynEntityId FROM @tmp)
                       AND ActiveVersion = 1       

                    -- Update index
                    INSERT @entities (DynEntityUid, DynEntityId, DynEntityConfigUid)
                    SELECT DynEntityUid, DynEntityId, DynEntityConfigUid
                      FROM ADynEntitySOBDossier
                     WHERE DynEntityId IN (SELECT DISTINCT DynEntityId FROM @tmp)
                       AND ActiveVersion = 1
                    EXEC _cust_ReIndex NULL, 1, 0, 'nl-BE', @entities = @entities
                    ",
                new SqlParameter[]
                {
                    new SqlParameter("@StartTime", DateTime.Now) {SqlDbType = SqlDbType.DateTime},
                });
            _unitOfWork.Commit();
        }

        private void ReIndexPaymentsAndCertificates()
        {
            _unitOfWork.SqlProvider.ExecuteNonQuery(@"
                    DECLARE @tmp AS DynEntityIdsTableType

                    -- Update imported payments ...
                    INSERT @tmp (DynEntityUid, DynEntityId, DynEntityConfigUid)
                    SELECT p.DynEntityUid, p.DynEntityId, p.DynEntityConfigUid
                      FROM ADynEntitySOBPayment p
                     WHERE CAST(p.UpdateDate AS DATE) = CAST(@PackageStartTime AS DATE)
                       AND p.ActiveVersion = 1
 
                     -- Update DynEntityTaxonomyItem 
                    EXEC _cust_UpdateDynEntityTaxonomyItem 1, @entities = @tmp
  
                    -- Create Fulltext Index
                    EXEC _cust_ReIndex NULL, 1, 1, 'nl-BE', @entities = @tmp

                    -- Update created certificates (created when no certificate for payment)
                    INSERT @tmp (DynEntityUid, DynEntityId, DynEntityConfigUid)
                    SELECT c.DynEntityUid, c.DynEntityId, c.DynEntityConfigUid
                      FROM ADynEntitySOBCertificate c
                     WHERE CAST(c.UpdateDate AS DATE) = CAST(@PackageStartTime AS DATE)
                       AND c.ActiveVersion = 1
 
                     -- Update DynEntityTaxonomyItem 
                    EXEC _cust_UpdateDynEntityTaxonomyItem 1, @entities = @tmp
  
                    -- Create Fulltext Index
                    EXEC _cust_ReIndex NULL, 1, 1, 'nl-BE', @entities = @tmp",
                new SqlParameter[]
                {
                    new SqlParameter("@PackageStartTime", DateTime.Now) {SqlDbType = SqlDbType.DateTime},
                });
        }
    }

    /// <summary>
    /// Payment record in CSV file
    /// </summary>
    public class PaymentRecord : CsvClassMap<PaymentRecord>
    {
        public Int64? DynEntityUid { get; set; }

        public Int64 RRN { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Zip { get; set; }

        public string City { get; set; }

        public string C { get; set; }

        public string Country { get; set; }

        public string Vacantex { get; set; }

        public string Cat { get; set; }

        public string Bit { get; set; }

        public string PaymentDate { get; set; }

        public DateTime datPaymentDate
        {
            get
            {

                string date;
                if (PaymentDate.Length == 7)
                {//assume that a date 2013829 stands for 2013 08 29
                    date = String.Format(@"{0}/0{1}/{2}", PaymentDate.Substring(0, 4), PaymentDate.Substring(4, 1), PaymentDate.Substring(5, 2));
                }
                else
                {//8 digits otherwise
                    //Note: 201311 will result in exception, 2013111 is ambiguous
                    date = String.Format(@"{0}/{1}/{2}", PaymentDate.Substring(0, 4), PaymentDate.Substring(4, 2), PaymentDate.Substring(6, 2));
                }
                return DateTime.ParseExact(date, @"yyyy/MM/dd", new CultureInfo("en-US"));
            }
        }

        public string BankAccount { get; set; }

        public decimal NettoAmount { get; set; }

        public int DaysPaid { get; set; }

        public int OpenSaldoDays { get; set; }

        public int DateTrimester { get; set; }

        public string ImportStatus { get; set; }

        public IList<LookupPayments.Payment> ExistingPayments { get; set; }

        public LookupDossier.Dossier Dossier { get; set; }

        public bool MatchExistingPayment { get; set; }

        public bool ForceAddPayment { get; set; }

        public override void CreateMap()
        {
            int i = 0;
            Map(m => m.RRN).Index(i++);
            Map(m => m.Name).Index(i++);
            Map(m => m.Address).Index(i++);
            Map(m => m.Country).Index(i++);
            Map(m => m.Zip).Index(i++);
            Map(m => m.City).Index(i++);
            Map(m => m.C).Index(i++);
            Map(m => m.Vacantex).Index(i++);
            Map(m => m.Cat).Index(i++);
            Map(m => m.Bit).Index(i++);
            Map(m => m.DateTrimester).Index(i++);
            Map(m => m.PaymentDate).Index(i++);
            Map(m => m.BankAccount).Index(i++);
            Map(m => m.NettoAmount).Index(i++);
            Map(m => m.DaysPaid).Index(i++);
            Map(m => m.OpenSaldoDays).Index(i++);
        }
    }

    public class LookupPayments
    {
        const string query = @"SELECT Dossier, Trimester, DynEntityId AS lu_PayemntEntityId, Payment_date AS lu_Payment_date, Amount AS lu_Amount, Days AS lu_Days
             FROM ADynEntitySOBPayment 
             WHERE (ActiveVersion = 1)";

        public static IList<Payment> Execute(IUnitOfWork unitOfWork)
        {
            var result = unitOfWork.SqlProvider.DbConnection.Query<Payment>(query).ToList();
            return result;
        }

        public class Payment
        {
            public Int64 Dossier { get; set; }
            public int Trimester { get; set; }
            public Int64 lu_PayemntEntityId { get; set; }
            public DateTime lu_Payment_date { get; set; }
            public decimal lu_Amount { get; set; }
            public int lu_Days { get; set; }
        }
    }

    public class LookupDossier
    {
        const string query = @"/** Always select the Dossier with the latest unemployment date (so the most recent dossier) **/
            WITH PersonLastUnemploymentDate AS
            (
	            SELECT p.RRNKD, MAX(d.unemployed_since) AS LastUnemploymentDate, MAX(d.UpdateDate) AS MaxUpdateDate
	              FROM ADynEntitySOBDossier d
				            INNER JOIN ADynEntityPerson p ON d.Person = p.DynEntityId
	             WHERE (d.ActiveVersion = 1) 
	               AND (p.ActiveVersion = 1)
	             GROUP BY p.RRNKD
            )
            SELECT d.DynEntityId AS DossierDynEntityId, p.RRNKD, CONVERT(varchar(100), d.Name) AS DossierName
              FROM ADynEntitySOBDossier d
		            INNER JOIN ADynEntityPerson p ON d.Person = p.DynEntityId
		            INNER JOIN PersonLastUnemploymentDate ld ON (p.RRNKD = ld.rrnkd AND d.unemployed_since = ld.LastUnemploymentDate AND d.UpdateDate = ld.MaxUpdateDate)
             WHERE d.ActiveVersion = 1
               AND p.ActiveVersion = 1";

        public static IDictionary<Int64, Dossier> Execute(IUnitOfWork unitOfWork)
        {
            var result = unitOfWork.SqlProvider.DbConnection.Query<Dossier>(query).ToList();
            return result.ToDictionary(x => x.RRNKD);
        }

        public class Dossier
        {
            public Int64 DossierDynEntityId { get; set; }
            public Int64 RRNKD { get; set; }
            public string DossierName { get; set; }
        }
    }

    public class LookupImportedPayments
    {
        public static IList<ImportedPayment> GetSOBImportedPayments(IUnitOfWork unitOfWork)
        {
            //note: insecure query when launched right after the midnight
            const string query = @"
                WITH temp AS
	            (
	            SELECT dossier, trimester, count(1) AS cnt
	                FROM ADynEntitySOBPayment
	                WHERE CAST(UpdateDate AS DATE) = CAST(@CurrentDate AS DATE)
	                GROUP BY dossier, trimester
	            )
	            
                SELECT p.DynEntityUid AS PaymentUid, p.DynEntityId AS PaymentId, p.Name, p.dossier, p.Payment_date, p.Days, p.Trimester, cnt 
	            FROM ADynEntitySOBPayment p
				        INNER JOIN temp t ON p.dossier = t.dossier AND p.Trimester = t.Trimester
	            WHERE cnt = 1
	            UNION
	            SELECT p.DynEntityUid AS PaymentUid, p.DynEntityId AS PaymentId, p.Name, p.dossier, p.Payment_date, p.Days, p.Trimester, cnt 
	            FROM ADynEntitySOBPayment p
				        INNER JOIN temp t ON p.dossier = t.dossier AND p.Trimester = t.Trimester
	            WHERE cnt > 1";
            using (var connection = new SqlConnection(unitOfWork.SqlProvider.ConnectionString))
            {
                var result = connection.Query<ImportedPayment>(query, new {CurrentDate = DateTime.Now.Date}).ToList();
                return result;
            }
        }

        public class ImportedPayment
        {
            public int cnt { get; set; }

            public Int64 PaymentId { get; set; }

            public Int64 PaymentUid { get; set; }

            public string Name { get; set; }

            public DateTime Payment_date { get; set; }

            public int Days { get; set; }

            public int Trimester { get; set; }

            public Int64 dossier { get; set; }
        }
    }

    public class LookupCertificate
    {
        public static IList<Certificate> GetCertificates(IUnitOfWork unitOfWork)
        {
            const string query =
                @"SELECT DynEntityUid, Trimester, Dossier, Payment As PaymentDynEntityUid FROM ADynEntitySOBCertificate WHERE ISNULL(Payment, 0) = 0 AND ActiveVersion = 1";
            var result = unitOfWork.SqlProvider.DbConnection.Query<Certificate>(query).ToList();
            return result;
        }

        public class Certificate
        {
            public Int64 DynEntityUid { get; set; }

            public int Trimester { get; set; }

            public Int64 Dossier { get; set; }

            public Int64? PaymentDynEntityUid { get; set; }
        }

    }


    /// <summary>
    /// TODO: give a selfdescriptive name
    /// </summary>
    public class ScriptComponent
    {
        const string sql =
                "DECLARE @tmp AS DynEntityIdsTableType " +
                "DECLARE @entities AS DynEntityIdsTableType " +
                "DECLARE @STATE_DYNENTITYCONFIGID bigint = 8060	-- ADynEntitySOB_Certificates_B.State " +
                "DECLARE @BatchStateDynListId bigint = 54		-- Payments received for SOB Batch " +
                "DECLARE @PaymentsReceivedDynListItemUid bigint " +
                "DECLARE @PaymentsReceivedDynListValue VARCHAR(50); " +
                "" +
                "INSERT @tmp (DynEntityUid, DynEntityId, DynEntityConfigUid) " +
                "SELECT b.DynEntityUid, b.DynEntityId, b.DynEntityConfigUid " +
                "  FROM ADynEntitySOB_Certificates_B b " +
                "           INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = b.DynEntityConfigUid " +
                "			       AND ac.DynEntityAttribConfigId = @STATE_DYNENTITYCONFIGID  " +
                "			       AND ac.ActiveVersion = 1 " +
                "			INNER JOIN DynListValue lv ON lv.DynEntityConfigUid = ac.DynEntityConfigUid " +
                "			       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid " +
                "			       AND lv.AssetUid = b.DynEntityUid " +
                "			INNER JOIN DynListItem li ON lv.DynListItemUid = li.DynListItemUid " +
                " WHERE li.Value NOT IN ('Payments received', 'Payments OK - Closed') " +
                "   AND b.trimester = @DynEntityId " +
                "" +
                "EXEC _cust_CreateNewRevision @entities = @tmp " +
                " " +
                "SELECT @PaymentsReceivedDynListItemUid = DynListItemUid, @PaymentsReceivedDynListValue = Value " +
                "  FROM DynListItem li " +
                " WHERE DynListItemUid = @BatchStateDynListId AND ActiveVersion = 1 " +
                "" +
                "UPDATE DynListValue " +
                "   SET DynListItemUid = @PaymentsReceivedDynListItemUid, " +
                "       Value = @PaymentsReceivedDynListValue " +
                "  FROM DynListValue lv " +
                "			INNER JOIN ADynEntitySOB_Certificates_B b ON b.DynEntityUid = lv.AssetUid " +
                "			INNER JOIN DynEntityAttribConfig ac ON ac.DynEntityConfigUid = b.DynEntityConfigUid " +
                "			       AND ac.DynEntityAttribConfigId = @STATE_DYNENTITYCONFIGID " +
                "			       AND ac.ActiveVersion = 1 " +
                "			       AND lv.DynEntityConfigUid = ac.DynEntityConfigUid " +
                "			       AND lv.DynEntityAttribConfigUid = ac.DynEntityAttribConfigUid " +
                " WHERE b.DynEntityId IN (SELECT DynEntityId FROM @tmp) " +
                "   AND b.ActiveVersion = 1 " +
                "" +
                "-- Update index " +
                "INSERT @entities (DynEntityUid, DynEntityId, DynEntityConfigUid) " +
                "SELECT DynEntityUid, DynEntityId, DynEntityConfigUid " +
                "  FROM ADynEntitySOB_Certificates_B " +
                " WHERE DynEntityId IN (SELECT DISTINCT DynEntityId FROM @tmp) " +
                "   AND ActiveVersion = 1 " +
                "EXEC _cust_ReIndex NULL, 1, 0, 'nl-BE', @entities = @entities";

        public static void Execute(int maxDateTrimester, IUnitOfWork unitOfWork)
        {
            unitOfWork.SqlProvider.ExecuteNonQuery(sql,
                new SqlParameter[]
                {new SqlParameter("@DynEntityId", maxDateTrimester) {SqlDbType = System.Data.SqlDbType.Int}});
        }
    }

}