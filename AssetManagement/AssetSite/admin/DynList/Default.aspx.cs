using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;


namespace AssetSite.DynList
{
    public partial class Default : BasePage
    {
        [Dependency]
        public IDataTypeService DataTypeService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            DynListAll.DataBind();
            dvDynList.DataBind();
        }

        protected void DynList_Inserting(object sender, EntityDataSourceChangingEventArgs args)
        {
            var dynList = args.Entity as AppFramework.Entities.DynList;
            dynList.Active = true;
            dynList.DataTypeId = DataTypeService.GetByType(
                AppFramework.ConstantsEnumerators.Enumerators.DataType.String).UID;
            UpdateProperties(dynList);
        }

        protected void DynList_Updating(object sender, EntityDataSourceChangingEventArgs args)
        {
            var dynList = args.Entity as AppFramework.Entities.DynList;
            UpdateProperties(dynList);
            insertHeader.Visible = dvDynList.Visible = true;
        }

        protected void DynList_Deleting(object sender, EntityDataSourceChangingEventArgs args)
        {
            var dynList = args.Entity as AppFramework.Entities.DynList;
            var entity = UnitOfWork.DynListRepository.Single(dl => dl.DynListUid == dynList.DynListUid);
            UpdateProperties(entity);
            entity.Active = false;
            UnitOfWork.Commit();
            args.Cancel = true;
        }

        private void UpdateProperties(AppFramework.Entities.DynList dynList)
        {
            dynList.UpdateDate = DateTime.Now;
            dynList.UpdateUserId = AuthenticationService.CurrentUserId;
        }

        protected void DynList_Inserted(object sender, DetailsViewInsertedEventArgs args)
        {
            DynListAll.DataBind();
        }

        protected void DynListAll_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            insertHeader.Visible = dvDynList.Visible = true;
        }

        protected void DynListAll_RowEditing(object sender, GridViewEditEventArgs e)
        {
            insertHeader.Visible = dvDynList.Visible = false;
        }

        protected string GetDynListName(object dataItem)
        {
            return new TranslatableString((string)DataBinder.Eval(dataItem, "Name")).GetTranslation();
        }
    }
}
