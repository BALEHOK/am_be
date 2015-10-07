namespace AppFramework.Core.PL.Batch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.UI.WebControls;

    public class BatchJobDataSource : ObjectDataSource
    {
        public BatchJobDataSource()
            : base()
        {
            this.SelectCountMethod = "GetTotalCount";
            this.SelectMethod = "GetAll";
            this.StartRowIndexParameterName = "startRow";
            this.MaximumRowsParameterName = "rowsMax";
            this.SortParameterName = "sortBy";
            this.TypeName = "AppFramework.Core.Classes.Batch.BatchJob";            
            this.EnablePaging = true;            
        }
    }
}
