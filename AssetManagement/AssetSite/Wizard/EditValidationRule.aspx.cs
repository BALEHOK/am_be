using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.Classes.Validation.Expression;
using AppFramework.Core.Classes.Validation.Operators;
using System.Text.RegularExpressions;
using AppFramework.Core.Validation;
using AppFramework.Entities;
using Microsoft.Practices.Unity;

namespace AssetSite.Wizard
{
    public partial class EditValidationRule : WizardController
    {
        [Dependency]
        public IValidationOperatorFactory ValidationOperatorFactory { get; set; }
        [Dependency]
        public IValidationRulesService ValidationRulesService { get; set; }
        [Dependency]
        public IValidationService ValidationService { get; set; }

        private ValidationOperatorBase _operator;
        private AssetTypeAttribute _assetAttribute;

        protected void Page_Load(object sender, EventArgs e)
        {
            (Master as MasterPageWizard).WizardMenu.CurrentStepIndex = 3;
            if (AssetType == null)
            {
                Response.Redirect("~/Wizard/Step1.aspx");
            }

            if (!string.IsNullOrEmpty(Request.QueryString["AttrUID"]))
            {
                long uid = long.Parse(Request.QueryString["AttrUID"]);
                _assetAttribute = AssetType.Attributes.Single(a => a.UID == uid);
                if (object.Equals(_assetAttribute, null))
                {
                    Response.Redirect("~/Wizard/Step3.aspx");
                }
            }
            else
            {
                Response.Redirect("~/Wizard/Step1.aspx");
            }
            OpList.Attributes.Add("ondblclick", 
                string.Format("onMoveOpClick('{0}','{1}')", 
                ExprText.ClientID, OpList.ClientID));

            if (!Page.IsPostBack)
            {
                OperatorsList.DataSource = ValidationOperatorFactory.GetAll();
                OperatorsList.DataBind();

                var validationRules = ValidationRulesService.GetValidationRulesForAttribute(_assetAttribute);
                GridRules.DataSource = validationRules;
                GridRules.DataBind();

                ExprText.Text = _assetAttribute.ValidationExpr;
                ValidationMessage.Text = _assetAttribute.ValidationMessage;

                RebindOperands();
                RebindOpList(validationRules);
            }
            else
            {
                _operator = ValidationOperatorFactory.GetByUid(long.Parse(OperatorsList.SelectedValue));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            (Master as MasterPageWizard).NextButton.Visible = false;
            (Master as MasterPageWizard).PreviousButton.Visible = false;
            (Master as MasterPageWizard).CancelButton.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            (Master as MasterPageWizard).WizardMenu.CurrentStepIndex = 4;
            (Master as MasterPageWizard).WizardMenu.CurrentSubIndex = 3;
        }

        private void RebindOpList(IEnumerable<AttributeValidationRule> validationRules)
        {
            OpList.Items.AddRange(
                validationRules.
                    Select(r => new ListItem("@" + r.Name, r.UID.ToString())).ToArray()
                );

            OpList.Items.AddRange(
                                LogicalOperator.GetAll().
                                    Select(r => new ListItem(r.Expr, r.TypeInt.ToString())).ToArray()
                            );

            OpList.Items.AddRange(new[] { new ListItem("(", "0"), new ListItem(")", "-1") });
        }

        public string ListOperands(ValidationRuleBase rule)
        {
            if (!object.Equals(null, rule))
                return string.Join(",", rule.ValidationOperator.Operands.Select(o => string.Format("{0} = {1}", o.Alias, o.Value)).ToArray());
            return string.Empty;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            var vals = new List<string>();
            foreach (RepeaterItem item in OperandsList.Items)
            {
                var value = (item.FindControl("Val") as TextBox).Text;
                var alias = (item.FindControl("Alias") as Label).Text;
                _operator.Operands.Single(o => o.Alias == alias).Value = value;
            }

            var validationRule = new AttributeValidationRule(
                new ValidationList {Name = RuleName.Text},_assetAttribute.ID, 0, _operator);

            //throw new NotImplementedException();
            //_assetAttribute.ValidationRules.Add(r);            
            ValidationService.AddAttributeValidationRule(validationRule);

            var validationRules = ValidationRulesService.GetValidationRulesForAttribute(_assetAttribute);
            RebindOpList(validationRules);
            Response.Redirect(Request.Url.OriginalString);
        }

        public ValidationOperatorBase GetOperator()
        {
            return _operator;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.OriginalString);
        }

        protected void OperatorsListDataBound(object sender, EventArgs e)
        {
            OperatorsList.Items.Insert(0, new ListItem("Please select...", "-1"));
        }

        protected void OperatorsListSelectChanged(object sender, EventArgs e)
        {
            RebindOperands();
        }

        private void RebindOperands()
        {
            _operator = ValidationOperatorFactory.GetByUid(long.Parse(OperatorsList.SelectedValue));
            OperandsList.DataSource = _operator.Operands;
            OperandsList.DataBind();
        }


        protected void btnClose_Click(object sender, EventArgs e)
        {
            string expression = ExprText.Text;
            Regex rule = new Regex(@"@(\w+)");
            MatchCollection matches = rule.Matches(expression);

            //foreach (var match in matches)
            //{
            //    if (_assetAttribute.ValidationRules.All(r => r.Name != match.Groups[1].Value))
            //    {
            //        lblConsistenceValidation.Visible = true;
            //        return;
            //    }
            //}

            _assetAttribute.ValidationExpr = ExprText.Text;
            _assetAttribute.ValidationMessage = ValidationMessage.Text;
            Response.Redirect("~/Wizard/Step3.aspx");
        }

        protected void CheckExprClick(object sender, EventArgs e)
        {
            var rules = ValidationRulesService.GetValidationRulesForAttribute(_assetAttribute).ToList();
            ExprValidator val = new AttributeExprValidator(ExprText.Text, null, rules);
            ValidationResult res = val.Validate(CheckValue.Text);

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

        protected void GridRules_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //long uid = long.Parse(GridRules.DataKeys[e.RowIndex].Value.ToString());
            throw new NotImplementedException();
            //var r = _assetAttribute.ValidationRules[e.RowIndex];
            //if (r != null)
            //{
            //    _assetAttribute.ValidationRules.Remove(r);
            //    Response.Redirect(Request.Url.ToString());
            //}
        }
    }
}
