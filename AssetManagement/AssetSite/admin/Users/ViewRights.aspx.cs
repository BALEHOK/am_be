using AppFramework.Core.Classes;
using AppFramework.Core.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace AssetSite.admin.Users
{
    public partial class Edit : BasePage
    {

        private long _userId
        {
            get
            {
                return usersList.SelectedValue != null ?
                        long.Parse(usersList.SelectedValue) : default(long);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                usersList.Items.Clear();

                List<AssetUser> users = new List<AssetUser>();
                foreach (AssetUser user in Membership.GetAllUsers())
                {
                    users.Add(user);
                }

                usersList.Items.AddRange((from user in users
                                          orderby user.UserName ascending
                                          select new ListItem(user.UserName, user.Asset.ID.ToString()))
                                          .ToArray());

                int selIndex = 0;
                if (Session["ViewRights:selectedUser"] != null
                    && int.TryParse(Session["ViewRights:selectedUser"].ToString(), out selIndex))
                {
                    if (usersList.Items.Count > selIndex)
                    {
                        usersList.SelectedIndex = selIndex;
                    }
                }
                BindData();
            }
        }

        protected void usersList_SelectedIndexChanged(Object sender, EventArgs e)
        {
            BindData();
            Session.Add("ViewRights:selectedUser", usersList.SelectedIndex);
        }

        protected void userName_TextChanged(object sender, EventArgs e)
        {
            UpdateUsersList();
            addLink.Visible = false;
            aRulesHeader.Visible = false;
            dRulesHeader.Visible = false;
        }

        private void UpdateUsersList()
        {
            usersList.Items.Clear();
            var matchName = userName.Text.Trim();
            foreach (var user in Membership.GetAllUsers()
                                           .Cast<AssetUser>()
                                           .Where(u => u.UserName.ToLowerInvariant().Contains(matchName.ToLowerInvariant())))
            {
                usersList.Items.Add(new ListItem(user.UserName, user.Asset.ID.ToString()));
            }
        }

        private void BindData()
        {
            if (usersList.SelectedIndex >= 0)
            {
                addLink.Visible = true;
                aRulesHeader.Visible = true;
                dRulesHeader.Visible = true;

                var user = (Membership.GetUser(usersList.SelectedValue as object, false) as AssetUser);
                var rights = user.Permissions;
                var adapter = new RightsDatasourceAdapter(rights, UnitOfWork, AssetTypeRepository, AssetsService);

                GridView allowRightsGrid = rightsPanel.FindControl("allowRightsGrid") as GridView;
                allowRightsGrid.DataSource = adapter.GetEnumerator().Where(r => !r.IsDeny);
                allowRightsGrid.DataBind();

                GridView denyRightsGrid = rightsPanel.FindControl("denyRightsGrid") as GridView;
                denyRightsGrid.DataSource = adapter.GetEnumerator().Where(r => r.IsDeny);
                denyRightsGrid.DataBind();
                
                addLink.NavigateUrl
                    = String.Format("~/admin/Users/EditRights.aspx?userid={0}", usersList.SelectedValue);

                hlinkAddNewTaskrule.Visible = pnlDenyTasksRights.Visible = pnlAllowTasksRights.Visible = true;
                var taskrights =  UserService.GetUserTaskRightsList(user.Id);
                if (taskrights != null)
                {
                    var tasksAdapter = new TaskRightsDatasourceAdapter(taskrights, UnitOfWork, AssetTypeRepository);
                    gvAllowTasksRights.DataSource = tasksAdapter.GetEnumerator().Where(r => !r.IsDeny);
                    gvAllowTasksRights.DataBind();
                    gvDenyTasksRights.DataSource = tasksAdapter.GetEnumerator().Where(r => r.IsDeny);
                    gvDenyTasksRights.DataBind();
                }
                hlinkAddNewTaskrule.NavigateUrl
                   = String.Format("~/admin/Users/EditRights.aspx?type=task&userid={0}", usersList.SelectedValue);

            }
        }

        protected void allowRule_Deleting(Object sender, GridViewDeleteEventArgs e)
        {
            long viewId;
            if (long.TryParse(allowRightsGrid.DataKeys[e.RowIndex].Value.ToString(), out viewId))
            {
                DeleteRightsByViewId(viewId);
                AppFramework.Core.AC.Authentication.AccessManager.Instance.ForceRightsUpdate(_userId);
            }
            BindData();
        }

        protected void denyRule_Deleting(Object sender, GridViewDeleteEventArgs e)
        {
            long viewId;
            if (long.TryParse(denyRightsGrid.DataKeys[e.RowIndex].Value.ToString(), out viewId))
            {
                DeleteRightsByViewId(viewId);
                AppFramework.Core.AC.Authentication.AccessManager.Instance.ForceRightsUpdate(_userId);

            }
            BindData();
        }


        protected void gvAllowTasksRights_Deleting(Object sender, GridViewDeleteEventArgs e)
        {
            long viewId;
            if (long.TryParse(gvAllowTasksRights.DataKeys[e.RowIndex].Value.ToString(), out viewId))
            {
                TaskRightsList.DeleteByViewID(viewId);
            }
            BindData();
        }

        protected void gvDenyTasksRights_Deleting(Object sender, GridViewDeleteEventArgs e)
        {
            long viewId;
            if (long.TryParse(gvDenyTasksRights.DataKeys[e.RowIndex].Value.ToString(), out viewId))
            {
                TaskRightsList.DeleteByViewID(viewId);
            }
            BindData();
        }

        /// <summary>
        /// Deletes the list of entries by View ID
        /// </summary>
        /// <param name="viewId"></param>
        private void DeleteRightsByViewId(long viewId)
        {
            var entities = UnitOfWork.RightsRepository.Where(r => r.ViewId == viewId);
            entities.ForEach(entity => UnitOfWork.RightsRepository.Delete(entity));
            UnitOfWork.Commit();
        }
    }
}
