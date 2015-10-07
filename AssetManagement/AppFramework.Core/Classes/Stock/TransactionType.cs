namespace AppFramework.Core.Classes.Stock
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AppFramework.Core.Classes;

    public enum TransactionTypeCode
    {
        In = 1,
        Out,
        Move
    }

    public class TransactionType
    {
        public int UID
        {
            get;
            set;
        }

        public string Name 
        { 
            get; 
            set; 
        }

        public bool IsMovement
        {
            get;
            set;
        }

        public static TransactionType GetByCode(TransactionTypeCode code)
        {
            TransactionType t = new TransactionType() { UID = (int)code, IsMovement = false };
            switch (code)
            {
                case TransactionTypeCode.In:
                    t.Name = "In";
                    break;
                case TransactionTypeCode.Out:
                    t.Name = "Out";
                    break;
                case TransactionTypeCode.Move:
                    t.Name = "Move";
                    t.IsMovement = true;
                    break;
                default:
                    break;
            }
            return t;
        }

        public static IEnumerable<TransactionType> GetAll()
        {
            foreach (TransactionTypeCode item in System.Enum.GetValues(typeof(TransactionTypeCode)))
            {
                yield return GetByCode(item);
            }
        }
    }
}
