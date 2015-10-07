<%@ Page Title="Places and Zip Codes" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    AutoEventWireup="true" CodeBehind="ZipsAndPlaces.aspx.cs" Inherits="AssetSite.admin.ZipsAndPlaces"
    EnableEventValidation="false" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript" src="../javascript/pnz.js"></script>
    <script type="text/javascript">
        var pORz = 0;
        var poz_ID = 0;
    </script>
    <style type="text/css">
        #middle-column
        {
            margin-top: -5px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div id="DialogContainer" runat="server" style="display: none;">
        <br />
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td style="text-align: center;">
                    <asp:TextBox ID="tbValue" runat="server" Width="340px" meta:resourcekey="tbValueResource1"></asp:TextBox><br />
                    <br />
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <a id="lbtnOK" runat="server" style="cursor: pointer;">OK</a>
                </td>
            </tr>
        </table>
    </div>
    <asp:ScriptManager ID="svcsManager" runat="server">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div class="panel">
        <div class="panelheader">
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td style="text-align: left; width: 49%">
                        <asp:Label runat="server" meta:resourcekey="LabelResource1">Places</asp:Label>
                    </td>
                    <td style="width: 2%;">
                        &nbsp;
                    </td>
                    <td style="text-align: right; width: 49%">
                        <asp:Label runat="server" meta:resourcekey="LabelResource2">Zip Codes</asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div class="panelcontent">
            <asp:LinkButton ID="lbtnRebindZip" runat="server" OnClick="OnRebindZipCodes" Visible="false">Zip</asp:LinkButton>
            <asp:LinkButton ID="lbtnRebindPlace" runat="server" OnClick="OnRebindPlaces" Visible="false">Place</asp:LinkButton>
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td style="width: 49%">
                        <asp:GridView ID="gvPlaces" runat="server" AutoGenerateColumns="False" DataSourceID="placesDataSource"
                            AutoGenerateSelectButton="True" AllowPaging="True" DataKeyNames="Id" PageSize="20"
                            OnSelectedIndexChanged="gvPlaces_SelectedIndexChanged" OnPageIndexChanged="gvPlaces_PageIndexChanged">
                            <Columns>
                                <asp:TemplateField HeaderText="Place" meta:resourcekey="TemplateFieldResource1">
                                    <ItemTemplate>
                                        <asp:Label runat="server"><%#Eval("PlaceName")%></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <a id="btnPlaceEdit" style="cursor: pointer; border: 0px;" onclick='<%# GetPlaceEditScript(Eval("Id")) %>'>
                                            <asp:Image runat="server" ImageUrl="~/images/buttons/edit.png" /></a>&nbsp;&nbsp;
                                        <a id="btnPlaceDelete" style="cursor: pointer; border: 0px;" onclick='<%# GetPlaceDeleteScript(Eval("Id")) %>'>
                                            <asp:Image runat="server" ImageUrl="~/images/buttons/delete.png" />
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:LinkButton ID="btnRebind" runat="server" Visible="False" OnClick="btnRebind_Click">rebind</asp:LinkButton>
                        <asp:ObjectDataSource runat="server" EnablePaging="True" ID="placesDataSource" SelectMethod="GetAllPaged"
                            StartRowIndexParameterName="start" MaximumRowsParameterName="pageLength" SelectCountMethod="GetCount"
                            TypeName="AppFramework.Core.Classes.Place"></asp:ObjectDataSource>
                    </td>
                    <td style="width: 2%;">
                        &nbsp;
                    </td>
                    <td style="width: 49%">
                        <asp:GridView ID="gvZips" runat="server" AutoGenerateColumns="False">
                            <Columns>
                                <asp:TemplateField HeaderText="Zip Code" meta:resourcekey="TemplateFieldResource3">
                                    <ItemTemplate>
                                        <asp:Label runat="server"><%#Eval("Code")%></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <a id="btnZipEdit" style="cursor: pointer; border: 0px;" onclick='<%# GetZipEditScript(Eval("Id")) %>'>
                                            <asp:Image ID="Image1" runat="server" ImageUrl="~/images/buttons/edit.png" /></a>&nbsp;&nbsp;
                                        <a id="btnZipDelete" style="cursor: pointer; border: 0px;" onclick='<%# GetZipDeleteScript(Eval("Id")) %>'>
                                            <asp:Image ID="Image2" runat="server" ImageUrl="~/images/buttons/delete.png" />
                                        </a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.item').hover(
            function () {
                $(this).css('background-color', '#84d859');
            },
            function () {
                $(this).css('background-color', '#999999');
            }
        );
        });

    </script>
    <div class="wizard-menu">
        <div class="active">
            <asp:Label ID="Label3" runat="server" meta:resourcekey="Label3Resource1">Actions</asp:Label>
        </div>
        <div class="item" style="background-color: rgb(153, 153, 153); cursor: pointer;">
            <a id="lbtnAddPlace" runat="server">Add place</a><br />
        </div>
        <div class="item" style="background-color: rgb(153, 153, 153); cursor: pointer;">
            <a id="lbtnAddZip" runat="server">Add Zipcode</a>
        </div>
    </div>
</asp:Content>
