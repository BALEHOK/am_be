using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Caching;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.Classes.Validation.Expression;
using AppFramework.Core.Classes.Validation.Operators;
using AppFramework.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AppFramework.Entities;
using Microsoft.Practices.Unity;

namespace AssetSite.admin
{
    public partial class DataTypesValidation : BasePage
    {
        [Dependency]
        public IDataTypeService DataTypeService { get; set; }
        [Dependency]
        public IValidationOperatorFactory ValidationOperatorFactory { get; set; }
        [Dependency]
        public IValidationService ValidationService { get; set; }
        [Dependency]
        public IValidationRulesService ValidationRulesService { get; set; }

        private CustomDataType _dataType;
        private ValidationOperatorBase _operator;

        protected void Page_Load(object sender, EventArgs e)
        {
            long uid;
            if (!string.IsNullOrEmpty(Request.QueryString["Id"]) && long.TryParse(Request.QueryString["Id"].ToString(), out uid))
            {
                _dataType = DataTypeService.GetByUid(uid);
            }
            else
            {
                Response.Redirect("~/admin/DataTypes/DataTypes.aspx");
            }

            if (!Page.IsPostBack)
            {
                OperatorsList.DataSource = ValidationOperatorFactory.GetAll();
                OperatorsList.DataBind();


                GridRules.DataSource = ValidationRulesService.GetValidationRulesForDataType(_dataType);                
                GridRules.DataBind();

                ExprText.Text = _dataType.ValidationExpr;
                ValidationMessage.Text = _dataType.ValidationMessage;

                RebindOperands();

                RebindOpList();
            }
            else
            {
                _operator = ValidationOperatorFactory.GetByUid(long.Parse(OperatorsList.SelectedValue));
            }

            OpList.Attributes.Add("ondblclick", string.Format("onMoveOpClick('{0}','{1}')", ExprText.ClientID, OpList.ClientID));
        }

        /// <summary>
        /// Rebinds the list of available operators, validation rules and brackets - for building expression
        /// </summary>
        private void RebindOpList()
        {
            OpList.Items.AddRange(
                ValidationRulesService.GetValidationRulesForDataType(_dataType).
                    Select(r => new ListItem("@" + r.Name, r.UID.ToString())).ToArray()
            );

            OpList.Items.AddRange(
                                LogicalOperator.GetAll().
                                    Select(r => new ListItem(r.Expr, r.TypeInt.ToString())).ToArray()
                            );

            OpList.Items.AddRange(new[] { new ListItem("(", "0"), new ListItem(")", "-1") });
        }

        /// <summary>
        /// Rebings list of operands for selected operator
        /// </summary>
        private void RebindOperands()
        {
            _operator = ValidationOperatorFactory.GetByUid(long.Parse(OperatorsList.SelectedValue));
            OperandsList.DataSource = _operator.Operands;
            OperandsList.DataBind();
        }

        protected void OperatorsListSelectChanged(object sender, EventArgs e)
        {
            RebindOperands();
        }

        /// <summary>
        /// Lists the operands with values comma separated - to display in GridView
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        public string ListOperands(ValidationRuleBase rule)
        {
            if (!object.Equals(null, rule))
                return string.Join(",", rule.ValidationOperator.Operands.Select(o => string.Format("{0} = {1}", o.Alias, o.Value)).ToArray());
            return string.Empty;          
        }

        protected void CheckExprClick(object sender, EventArgs e)
        {
            var rules = ValidationRulesService.GetValidationRulesForDataType(_dataType).ToList();
            var val = new DataTypeExprValidator(ExprText.Text, _dataType, rules);
            var res = val.Validate(CheckValue.Text);

            CheckResult.Text = string.Join(",",
                val.ParseErrors.ToArray());

            CheckResult.Text += "<br />";

            CheckResult.Text += string.Join(",",
                val.ValidationErrors.ToArray());

            if (res.IsValid)
            {
                CheckMessage.Text = "Success";
                CheckMessage.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                CheckMessage.Text = "Failed";
                CheckMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            List<string> vals = new List<string>();
            string value = string.Empty, alias = string.Empty;
            foreach (RepeaterItem item in OperandsList.Items)
            {
                value = (item.FindControl("Val") as TextBox).Text;
                alias = (item.FindControl("Alias") as Label).Text;
                _operator.Operands.Single(o => o.Alias == alias).Value = value;
            }

            var r = new DataTypeValidationRule(
                new ValidationList {Name = RuleName.Text}, _dataType.UID, _operator);
            ValidationService.SaveDataTypeValidationRule(r);
            RebindOpList();

            var datatype = UnitOfWork.DataTypeRepository.Single(entity => entity.DataTypeUid == _dataType.UID);
            datatype.ValidationExpr = ExprText.Text;
            UnitOfWork.DataTypeRepository.Update(datatype);
            UnitOfWork.Commit();
            CacheFactory.GetCache<CustomDataType>("UID").Remove(_dataType.UID);
            Response.Redirect(Request.Url.OriginalString);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            var datatype = UnitOfWork.DataTypeRepository.Single(entity => entity.DataTypeUid == _dataType.UID);
            datatype.ValidationExpr = ExprText.Text;
            datatype.ValidationMessage = ValidationMessage.Text;
            UnitOfWork.DataTypeRepository.Update(datatype);
            UnitOfWork.Commit();
            Cache<AssetType>.Flush();
            CacheFactory.GetCache<CustomDataType>("UID").Remove(_dataType.UID);
            Response.Redirect("~/admin/DataTypes/DataTypes.aspx");
        }

        protected void GridRules_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            long uid = 0; // long.Parse(GridRules.DataKeys[e.RowIndex].Value.ToString());
            if (long.TryParse((GridRules.Rows[e.RowIndex].Cells[0].FindControl("UID") as HiddenField).Value, 
                out uid) && uid != 0)
            {
                var r = ValidationRulesService
                    .GetValidationRulesForDataType(_dataType)
                    .SingleOrDefault(t => t.UID == uid);
                if (r != null)
                {
                    ValidationService.DeleteDataTypeValidationRule(r);                    
                    CacheFactory.GetCache<CustomDataType>("UID").Remove(_dataType.UID);
                    Response.Redirect(Request.Url.ToString());
                }
            }
        }
    }
}
