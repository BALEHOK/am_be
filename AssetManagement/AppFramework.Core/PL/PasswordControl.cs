/*--------------------------------------------------------
* PasswordControl.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 9/4/2009 1:16:53 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

using System.Web.UI;
using AppFramework.Core.Validation;

namespace AppFramework.Core.PL
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using AC.Providers;
    using Classes;
    using Classes.Validation;

    internal class PasswordControl : Control, IAssetAttributeControl
    {
        private readonly bool _mySettingsPage;
        private TextBox _passwordBox, _newpasswordBox, _confirmpasswordBox, _oldpasswordBox;
        private LiteralControl _ltcnewpassword, _ltcconfirmpassword, _ltcoldpassword;
        private LinkButton _linkButton;
        private bool _changePassword;

        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {            
        }

        public PasswordControl(AssetAttribute attribute, bool mySettingsPage)
        {
            this._mySettingsPage = mySettingsPage;
            this.AssetAttribute = attribute;
        }

        #region IAssetAttributeControl Members

        public bool Editable { get; set; }

        public AppFramework.Core.Classes.AssetAttribute GetAttribute()
        {
            PasswordProvider passwordProvider = new PasswordProvider();
            if (this._changePassword)
            {
                if (_oldpasswordBox != null)
                {
                    this.AssetAttribute.ValidationResults = null;
                    if (this.AssetAttribute.Value != passwordProvider.Encrypt(this._oldpasswordBox.Text.Trim()))
                    {
                        this.AssetAttribute.ValidationResults = new List<ValidationResult>
                            {
                                new ValidationResult(),
                                new ValidationResult
                                    {                                        
                                        ResultLines = new List<ValidationResultLine>()
                                            {
                                                new ValidationResultLine(string.Empty)
                                                    {
                                                        IsValid = false,
                                                        Message = "Current password is incorrect"
                                                    }
                                            }
                                    }
                            };

                    }
                    else if (_newpasswordBox.Text != _confirmpasswordBox.Text ||
                             string.IsNullOrEmpty(_newpasswordBox.Text))
                    {
                        this.AssetAttribute.ValidationResults = new List<ValidationResult>
                            {
                                new ValidationResult(),
                                new ValidationResult
                                    {                                        
                                        ResultLines =
                                            new List<ValidationResultLine>()
                                                {
                                                    new ValidationResultLine(string.Empty)
                                                        {
                                                            IsValid = false,
                                                            Message = "Passwords does not match"
                                                        }
                                                }
                                    }
                            };
                    }
                    else
                    {
                        this.AssetAttribute.Value = passwordProvider.Encrypt(this._newpasswordBox.Text.Trim());
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(this._passwordBox.Text))
                    {
                        this.AssetAttribute.Value = string.Empty;
                    }
                    else
                    {
                        passwordProvider = new PasswordProvider();
                        this.AssetAttribute.Value = passwordProvider.Encrypt(this._passwordBox.Text.Trim());
                    }
                }
            }
            return this.AssetAttribute;
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);
            InitControls();
        }

        protected void linkButton_Click(object sender, EventArgs e)
        {
            if (this._mySettingsPage)
            {
                this._ltcconfirmpassword.Visible = this._ltcoldpassword.Visible = this._ltcnewpassword.Visible =
                                                                                  this._confirmpasswordBox.Visible =
                                                                                  this._oldpasswordBox.Visible =
                                                                                  this._newpasswordBox.Visible = true;
                this._linkButton.Visible = false;
                _changePassword = true;
            }
            else if (this._passwordBox != null)
            {
                this._passwordBox.Visible = true;
                this._linkButton.Visible = false;
                _changePassword = true;
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            this.Controls.Clear();
            this.AddHiddenPassword();
        }

        protected override void LoadControlState(object savedState)
        {
            var state = savedState as object[];
            this._changePassword = (bool)state[0];
            base.LoadControlState(state[1]);
        }

        protected override object SaveControlState()
        {
            var state = new object[] { _changePassword, base.SaveControlState() };
            return state;
        }

        private void InitControls()
        {
            this.Controls.Clear();
            UpdatePanel updatePanel = new UpdatePanel();
            updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

            if (this._mySettingsPage)
            {
                _oldpasswordBox = new TextBox()
                    {
                        Text = string.Empty,
                        Visible = this._changePassword,
                        TextMode = TextBoxMode.Password,
                    };
                _ltcoldpassword = new LiteralControl()
                    {
                        Text = "<lable>Old password:</lable>",
                        Visible = this._changePassword
                    };

                _newpasswordBox = new TextBox()
                    {
                        Text = string.Empty,
                        Visible = this._changePassword,
                        TextMode = TextBoxMode.Password,
                    };
                _ltcnewpassword = new LiteralControl()
                    {
                        Text = "<br/><lable>New password:</lable>",
                        Visible = this._changePassword
                    };

                _confirmpasswordBox = new TextBox()
                    {
                        Text = string.Empty,
                        Visible = this._changePassword,
                        TextMode = TextBoxMode.Password,
                    };
                _ltcconfirmpassword = new LiteralControl()
                    {
                        Text = "<br/><lable>Confirm password:</lable>",
                        Visible = this._changePassword
                    };

                _linkButton = new LinkButton()
                    {
                        ID = "lbChangePwd",
                        Text = "Change password",
                        Visible = !this._changePassword
                    };

                _linkButton.Click += new EventHandler(linkButton_Click);
                updatePanel.ContentTemplateContainer.Controls.Add(_ltcoldpassword);
                updatePanel.ContentTemplateContainer.Controls.Add(_oldpasswordBox);
                updatePanel.ContentTemplateContainer.Controls.Add(_ltcnewpassword);
                updatePanel.ContentTemplateContainer.Controls.Add(_newpasswordBox);
                updatePanel.ContentTemplateContainer.Controls.Add(_ltcconfirmpassword);
                updatePanel.ContentTemplateContainer.Controls.Add(_confirmpasswordBox);
                updatePanel.ContentTemplateContainer.Controls.Add(_linkButton);
                updatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = _linkButton.ID, EventName = "Click" });

                this.Controls.Add(updatePanel);
            }
            else if (this.Editable)
            {
                _passwordBox = new TextBox()
                    {
                        Text = string.Empty,
                        Visible = this._changePassword
                    };

                _linkButton = new LinkButton()
                    {
                        ID = "lbChangePwd",
                        Text = "Change password",
                        Visible = !this._changePassword
                    };

                _linkButton.Click += new EventHandler(linkButton_Click);
                updatePanel.ContentTemplateContainer.Controls.Add(_passwordBox);
                updatePanel.ContentTemplateContainer.Controls.Add(_linkButton);

                updatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = _linkButton.ID, EventName = "Click" });

                this.Controls.Add(updatePanel);
            }
            else
            {
                AddHiddenPassword();
            }
        }

        private void AddHiddenPassword()
        {
            Literal literal = new Literal()
                {
                    Text = "Hidden"
                };

            this.Controls.Add(literal);
        }
    }
}