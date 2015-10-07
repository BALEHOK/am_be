<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Barcode.ascx.cs" Inherits="AssetSite.Controls.Barcode" %>
<asp:MultiView runat="server" ID="mv1">
    <asp:View ID="BarcodeView" runat="server">
        <%--<asp:Panel runat="server" ID="pnlBarcode"></asp:Panel>--%>
        <asp:Image ID="imgBarCode" runat="server" />
    </asp:View>
    <asp:View ID="BarcodeGenerator" runat="server">
        <asp:TextBox runat="server" ID="txtBarcode"></asp:TextBox>
        <asp:ImageButton runat="server" 
            ID="btnGenerate"
            ImageUrl="/images/barcode-icon.gif" 
            AlternateText="generate barcode"
            ImageAlign="Middle"
            ToolTip="Generate Barcode"
            CssClass="barcodeButton" />
    </asp:View>
</asp:MultiView>