<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" CodeBehind="AdditionalScreenStep2.aspx.cs" Inherits="AssetSite.admin.AdditionalScreens.AdditionalScreenStep2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <div class="wizard-menu">
        <div class="active">
            <asp:Label ID="Label3" runat="server">Screen Configuration</asp:Label>            
        </div>
        <br />
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep1.aspx">Create/Edit Screen</asp:HyperLink>
            </span>
        </div>
        <div class="active">
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep2.aspx">Screen Properties</asp:HyperLink>
            </span>
        </div>
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep3.aspx">Layout Selection</asp:HyperLink>
            </span>
        </div>
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep4.aspx">Panels Configuration</asp:HyperLink>
            </span>
        </div>
        <div>
            <span style="font-size:smaller;">
                <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/admin/AdditionalScreens/AdditionalScreenStep5.aspx">Assign Attributes</asp:HyperLink>
            </span>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div class="wizard-header">
        <asp:Label runat="server" ID="lblheader" Text="Item Wizard"></asp:Label>&nbsp;&mdash;&nbsp;
        <asp:Label runat="server" ID="lblStepName" Text="Attributen"></asp:Label>
    </div>
    <p>
        <asp:Literal runat="server" ID="stepDesc" 
            Text="Mauris congue consectetuer quam."></asp:Literal>  <%--meta:resourcekey="stepDescResource1"--%>
    </p>        
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblpanelHeader"></asp:Label>                
        </div>
        <div class="panelcontent">
            <table border="0" cellpadding="0" cellspacing="0" class="w100p">
                <tr>
                    <td class="labels">
                        <asp:Label ID="lblName" runat="server" Text="Name"></asp:Label>                                
                    </td>
                    <td class="controls">
                        <asp:TextBox CssClass="SelectControl name" ID="txtName" runat="server" MaxLength="60"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
                            runat="server"
                            ControlToValidate="txtName"
                            ErrorMessage="*" 
                            meta:resourcekey="RequiredFieldValidator1Resource1"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="labels">
                        <asp:Label runat="server" ID="lblTitle" Text="Title"></asp:Label>
                    </td>
                    <td class="controls">
                        <asp:TextBox ID="tbTitle" CssClass="SelectControl" runat="server" MaxLength="100"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbTitle" ErrorMessage="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="labels">
                        <asp:Label ID="lblSubtitle" runat="server" Text="Subtitle"></asp:Label>
                    </td>
                    <td class="controls">
                        <asp:TextBox ID="tbSubTitle" CssClass="SelectControl" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="labels">
                        <asp:Label ID="lblbPageText" runat="server" Text="Page Text"></asp:Label>
                    </td>
                    <td class="controls">
                        <asp:TextBox ID="tbPageText" CssClass="SelectControl" runat="server" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbPageText" ErrorMessage="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="labels">
                        <asp:Label ID="lblDesc" runat="server" Text="Description"></asp:Label>
                    </td>
                    <td class="controls">
                        <asp:TextBox ID="tbDesc" runat="server" CssClass="SelectControl" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbDesc" ErrorMessage="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="labels">
                        <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                    </td>
                    <td class="controls">
                        <asp:DropDownList ID="ddlStatus" CssClass="SelectControl" runat="server">
                            <asp:ListItem Text="Runtime" Value="0" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Design" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label ID="Label1" runat="server" Text="Linked Item(types)"></asp:Label>
        </div>
        <div class="panelcontent">
            <table border="0" cellpadding="0" cellspacing="0">
                <asp:Repeater ID="repLinkedAssets" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td class="controls" style="width:auto">
                                <asp:CheckBox ID="cbLinkedAsset" runat="server" Checked='<%#Eval("IsChecked") %>' />
                                <asp:HiddenField ID="hdfAssetId" runat="server" Value='<%#Eval("Attribute.UID") %>' />
                            </td>
                            <td class="labels" style="width:auto">
                                <asp:Label ID="Label2" runat="server" Text='<%#Eval("Attribute.Name") %>'></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>

    <div class="wizard-footer-buttons">
        <asp:Button CssClass="btnPrev" Text="<%$ Resources:Global, PreviousText %>" 
            ID="btnPrevious" runat="server" onclick="btnPrevious_Click" 
            CausesValidation="False" meta:resourcekey="btnPreviousResource1" /> 
        <asp:Button CssClass="btnNext" Text="<%$ Resources:Global, NextText %>" 
            ID="btnNext" runat="server" onclick="btnNext_Click" 
            meta:resourcekey="btnNextResource1"  />
        <asp:Button ID="btnClose" runat="server" 
            Text="<%$ Resources:Global, CancelText %>" onclick="btnClose_Click" 
            CausesValidation="False" meta:resourcekey="btnCloseResource1"  />          
    </div> 

</asp:Content>

