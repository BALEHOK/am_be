<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportControl.ascx.cs" Inherits="AssetSite.Controls.ExportControl" %>
<div class="export">
    <h2>
        <asp:Label ID="Label6" runat="server" meta:resourcekey="lnkExport">Export</asp:Label>
    </h2>
    <div>
        <asp:LinkButton ID="lbtnExportToTxt" runat="server" OnClick="OnlbtnExportToTxt_Click" meta:resourcekey="lnkExportToTxt">Export to txt</asp:LinkButton><br />
        <asp:LinkButton ID="lbtnExportToXml" runat="server" OnClick="OnlbtnExportToXml_Click" meta:resourcekey="lnkExportToXml">Export to xml</asp:LinkButton><br />
        <asp:LinkButton ID="lbtnExportToHtml" runat="server" OnClick="lbtnExportToHtml_Click" meta:resourcekey="lnkExportToHtml">Export to html</asp:LinkButton><br />        
    </div>     
</div>
