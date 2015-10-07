<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RestoreAssetMessage.ascx.cs" Inherits="AssetSite.Controls.RestoreAssetMessage" %>
<asp:PlaceHolder ID="RestoreSession" runat="server">
    <div class="restore-asset">
        <p>
            <asp:Literal ID="Literal1" runat="server" meta:resourcekey="lblMessage">Previous version of asset was not saved</asp:Literal>
            (<asp:Literal ID="Literal2" runat="server" meta:resourcekey="lblCreatedAt">created at</asp:Literal>&nbsp;
                <asp:Label runat="server" ID="lblCreationTime" meta:resourcekey="CreationTimeResource1" />).
        </p>

        <a href='<%= Request.Url.OriginalString %>&RestoreState=1'><asp:Label ID="Label1" runat="server" meta:resourcekey="lblRestoreAsset">Restore asset</asp:Label></a>
        <a class="delete" href='<%= Request.Url.OriginalString %>&DeleteState=1'><asp:Label ID="Label2" runat="server" meta:resourcekey="lblDeleteRestoreMessage">Hide this message</asp:Label></a>
   </div>
</asp:PlaceHolder>
