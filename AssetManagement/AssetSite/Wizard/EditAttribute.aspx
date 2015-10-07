<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditAttribute.aspx.cs"
    Inherits="AssetSite.Wizard.EditAttribute" MasterPageFile="~/MasterPages/MasterPageWizard.master" meta:resourcekey="PageResource1" %>

<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="WizardContent" runat="Server">

    <div class="wizard-header">
        <asp:Label runat="server" ID="pageTitle" meta:resourcekey="pageTitleResource1">Asset Wizard &mdash; Attributen</asp:Label>
    </div>

    <p>
        <asp:Literal runat="server" ID="pageDesc" meta:resourcekey="pageDescResource1"
            Text="Mauris congue consectetuer quam."></asp:Literal>
    </p>

    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle"
                meta:resourcekey="panelTitleResource1">Algemene kenmerken attribuut</asp:Label>
        </div>
        <div class="panelcontent">
            <table cellpadding="0" cellspacing="10" class="w100p">
                <tr>
                    <td class="labels">
                        <asp:Label ID="lblName" runat="server" Text="<% $Resources:Global, NameText %>"></asp:Label>
                    </td>
                    <td class="controls">
                        <asp:TextBox CssClass="SelectControl name" ID="txtName" autofocus="focus" runat="server" MaxLength="60"
                            meta:resourcekey="txtNameResource1"></asp:TextBox>
                        <asp:RequiredFieldValidator
                            runat="server"
                            Display="Dynamic"
                            ControlToValidate="txtName"
                            ErrorMessage="Please enter attribute name.">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator
                            ID="NameFormatValidator"
                            runat="server"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="txtName"
                            meta:resourcekey="NameFormatValidator"
                            ErrorMessage="Only letters, numbers, spaces and underscore allowed in attribute name."
                            ValidationExpression="^[A-Za-z]{1}[A-Za-z0-9\s_]+$">
                        </asp:RegularExpressionValidator>
                        <asp:Label
                            runat="server"
                            ForeColor="Red"
                            ID="lblNameValidationErrors"
                            Visible="false" />
                    </td>
                    <td align="left">
                        <a href="javascript:void(0)" onclick="showTranslations()">Add translation</a>
                    </td>
                </tr>
                <tr>
                    <td class="labels">
                        <asp:Label ID="lblDesc" runat="server" Text="<% $Resources:Global, DescText %>"></asp:Label>
                    </td>
                    <td class="controls">
                        <asp:TextBox ID="txtDescription" TextMode="MultiLine" CssClass="description" MaxLength="1000"
                            runat="server" meta:resourcekey="txtDescriptionResource1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="labels">
                        <label for="<%= chkActive.ClientID %>">
                            <asp:Label ID="Label3" runat="server" Text="<% $Resources:Global, ActiveText %>"></asp:Label>
                        </label>
                    </td>
                    <td class="controls">
                        <asp:CheckBox ID="chkActive" runat="server"
                            meta:resourcekey="chkActiveResource1" />
                    </td>
                </tr>
                <tr>
                    <td class="labels">
                        <label for="<%= chkRequired.ClientID %>">
                            <asp:Label ID="Label10" runat="server" Text="Required"
                                meta:resourcekey="Label10Resource1"></asp:Label></label>
                    </td>
                    <td class="controls">
                        <asp:CheckBox ID="chkRequired" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle2"
                meta:resourcekey="panelTitle2Resource1">Specifieke kenmerken attribuut</asp:Label>
        </div>
        <div class="panelcontent">
            <asp:UpdatePanel runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellpadding="0" cellspacing="10" class="w100p">
                        <tr>
                            <td class="labels">
                                <asp:Label ID="lblDataType" runat="server" Text="Datatype "
                                    meta:resourcekey="lblDataTypeResource1"></asp:Label>
                            </td>
                            <td class="controls">
                                <asp:DropDownList CssClass="SelectControl" ID="comboDataType" runat="server" AutoPostBack="True"
                                    OnSelectedIndexChanged="comboDataType_SelectedIndexChanged"
                                    meta:resourcekey="comboDataTypeResource1">
                                </asp:DropDownList>
                                <asp:PlaceHolder ID="phAssetType" runat="server" Visible="False">
                                    <div>
                                        <asp:Label ID="lblAssetType" runat="server" Text="Asset type: "
                                            CssClass="labels" meta:resourcekey="lblAssetTypeResource1"></asp:Label>
                                        <asp:DropDownList ID="comboAssetTypes" runat="server" Visible="True" AutoPostBack="True"
                                            OnSelectedIndexChanged="comboAssetTypes_SelectedIndexChanged"
                                            CssClass="controls" meta:resourcekey="comboAssetTypesResource1" AppendDataBoundItems="true">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                        <br />
                                        <asp:RequiredFieldValidator
                                            ID="comboAssetTypesValidator" runat="server"
                                            ControlToValidate="comboAssetTypes"
                                            ErrorMessage="Asset type cannot be empty" Visible="false"
                                            meta:resourcekey="comboAssetTypesValidatorResource1">
                                        </asp:RequiredFieldValidator>
                                    </div>
                                    <div>
                                        <asp:CheckBox ID="chkMultiAssets" runat="server" Text="More than one"
                                            Visible="true" meta:resourcekey="chkMultiAssetsResource1" />
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phAssetTypeAttribute" runat="server" Visible="False">
                                    <asp:Label ID="lblDispF" runat="server" Text="Display field: "
                                        CssClass="labels" meta:resourcekey="lblDispFResource1"></asp:Label>
                                    <asp:DropDownList ID="comboAssetTypeAttributes" runat="server" Visible="True"
                                        CssClass="controls" meta:resourcekey="comboAssetTypeAttributesResource1">
                                    </asp:DropDownList>
                                    <br />
                                    <asp:RequiredFieldValidator ID="comboAssetTypeAttributesValidator" ControlToValidate="comboAssetTypeAttributes"
                                        runat="server" ErrorMessage="Displayed field cannot be empty"
                                        Visible="false" meta:resourcekey="comboAssetTypeAttributesValidatorResource1">
                                    </asp:RequiredFieldValidator>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="DynLists" runat="server" Visible="False">
                                    <div>
                                        <asp:Label ID="lblDL" runat="server" Text="Dynamic list: " CssClass="labels"
                                            meta:resourcekey="lblDLResource1"></asp:Label>
                                        <asp:DropDownList ID="DynListDropDown" runat="server" Visible="True"
                                            CssClass="controls" DataTextField="Name" DataValueField="UID"
                                            meta:resourcekey="DynListDropDownResource1">
                                        </asp:DropDownList>
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator ID="DynListValidator" runat="server" ControlToValidate="DynListDropDown"
                                            ErrorMessage="Dynamic list cannot be empty" Visible="false"
                                            meta:resourcekey="DynListValidatorResource1">
                                        </asp:RequiredFieldValidator>
                                    </div>
                                    <div>
                                        <asp:CheckBox ID="DynListMulti" runat="server" Text="More than one"
                                            Visible="true" meta:resourcekey="DynListMultiResource1" />
                                    </div>
                                </asp:PlaceHolder>
                            </td>
                        </tr>
                        <%-- field formula --%>
                        <tr>
                            <td class="labels">
                                <div>
                                    <asp:Label ID="Label1" CssClass="labels" runat="server">Formula</asp:Label>
                                </div>
                            </td>
                            <td class="controls">
                                <div>
                                    <asp:CheckBox ID="chboxIsFormulaAttribute" runat="server"
                                        OnCheckedChanged="chboxIsFormulaAttribute_OnCheckedChanged" AutoPostBack="true" />
                                </div>
                                <div>
                                    <asp:PlaceHolder ID="formulaEditor" runat="server" Visible="False">
                                        <asp:Label ID="lblFormula" runat="server" Text="Formula text:" CssClass="labels" />
                                        <asp:TextBox ID="formulaText" TextMode="MultiLine" CssClass="description" MaxLength="1000"
                                            runat="server" meta:resourcekey="txtDescriptionResource1" />
                                    </asp:PlaceHolder>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="labels">
                                <div>
                                    <asp:Label ID="lblContext" runat="server" Text="Context "
                                        meta:resourcekey="lblContextResource1"></asp:Label>
                                </div>
                            </td>
                            <td class="controls">
                                <div>
                                    <asp:DropDownList CssClass="SelectControl" ID="comboContext" runat="server"
                                        meta:resourcekey="comboContextResource1">
                                    </asp:DropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="labels">
                                <label for="<%= chkFinancial.ClientID %>">
                                    <asp:Label ID="lblFinInfo" runat="server" Text="Financial Info "
                                        meta:resourcekey="lblFinInfoResource1"></asp:Label></label>
                            </td>
                            <td class="controls">
                                <asp:CheckBox ID="chkFinancial" runat="server"
                                    meta:resourcekey="chkFinancialResource1" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <amc:Translations ID="AttrTranslation" ControlSelector=".controls .name" runat="server" />
</asp:Content>
