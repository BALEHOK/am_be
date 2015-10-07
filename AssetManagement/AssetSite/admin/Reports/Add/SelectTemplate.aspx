<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectTemplate.aspx.cs"
    Inherits="AssetSite.admin.Reports.Add.SelectTemplate" MasterPageFile="~/MasterPages/MasterPageDefault.Master" %>

<asp:Content ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" 
                meta:resourcekey="panelTitleResource1" Text="Field definition"></asp:Label>            
        </div>
        <div class="panelcontent">
            <asp:HyperLink runat="server" 
                NavigateUrl='<%# String.Format("~/admin/Reports/Add/AssetInfo.aspx?Uid={0}", report.UID) %>'                
                Target=_blank meta:resourcekey="HyperLinkResource1">Asset info field templates</asp:HyperLink>
           
            <%--<asp:HyperLink runat="server" ID="FinTmplLinks" 
                NavigateUrl="~/admin/Reports/Add/FinInfo.ttx" 
                meta:resourcekey="FinTmplLinksResource1" Visible="false" 
                Text="Financial info field templates"></asp:HyperLink>   --%>         
        </div>
    </div>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle2" 
                meta:resourcekey="panelTitle2Resource1" Text="Report template"></asp:Label>            
        </div>
        <div class="panelcontent">
            <p>
                <asp:Label ID="Label1" runat="server" meta:resourcekey="AssetTypeLabel" Text="Asset type:"></asp:Label>
                <asp:Label ID="AssetTypeName" runat="server"></asp:Label>
            </p>
            <p>
                <asp:Label runat="server" meta:resourcekey="LabelResource1" 
                    Text="Current template:"></asp:Label>                
                <asp:Label runat="server" ID="CurrentTemplate" 
                    meta:resourcekey="CurrentTemplateResource1"></asp:Label>
            </p>
            <div>
                <asp:Label runat="server" meta:resourcekey="LabelResource2" 
                    Text="Select template file"></asp:Label>                
                <asp:FileUpload runat="server" ID="TemplateFile" 
                    meta:resourcekey="TemplateFileResource1" />
                <div>
                    <asp:CheckBox runat="server" meta:resourcekey="SyncLabel" ID="SyncWithCurrentItem" Text="Link the report data with the current opened item (one-item-view only)" />
                </div>
            </div>
            <div class="wizard-footer-buttons">
                <asp:Button ID="SetTemplate" runat="server" Text="<%$ Resources:Global, CompleteText %>"
                    OnClick="SetTemplateClick" meta:resourcekey="SetTemplateResource2"  />
            </div>
        </div>
    </div>
</asp:Content>
