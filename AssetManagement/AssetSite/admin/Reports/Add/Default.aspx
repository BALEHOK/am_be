<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.Reports.Add.Default"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PlaceHolderMiddleColumn">
    <asp:ScriptManager runat="server">
    </asp:ScriptManager>
    <script type="text/javascript" language="javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        var thebutton;
        var addbutton;

        function BeginRequestHandler(sender, args) {
            thebutton = args.get_postBackElement();
            addbutton = $get('<%= Save.ClientID %>');
            addbutton.disabled = true;
            thebutton.disabled = true;
        }

        function EndRequestHandler(sender, args) {
            thebutton.disabled = false;
            addbutton.disabled = false;
            $('#sel_general').attr('checked', '');
        }

        function select_general() {
            var gridId = '#<%= AttributesList.ClientID %>';
            var val = $('#sel_general').attr('checked');
            $(gridId + ' input[type="checkbox"]:even').attr('checked', val);
        }
    </script>
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" meta:resourcekey="panelTitleResource1">Report info</asp:Label>
        </div>
        <div class="panelcontent">
            <table class="wh100p">
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblbRepName" meta:resourcekey="lblbRepNameResource1">Report info</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="ReportName" meta:resourcekey="ReportNameResource1"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ReportName" Text="Name required"
                            meta:resourcekey="RequiredFieldValidatorResource1"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblIncFin" meta:resourcekey="lblIncFinResource1">Include financial info in report</asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="IncludeFinInfo" runat="server" meta:resourcekey="IncludeFinInfoResource1" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblbAT" meta:resourcekey="lblbATResource1">Asset type</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="AssetType" runat="server" DataTextField="Name" DataValueField="ID"
                            AutoPostBack="True" OnDataBound="AssetTypeDataBound" OnSelectedIndexChanged="AssetTypeSelectedIndexChanged"
                            meta:resourcekey="AssetTypeResource1">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td align="right">
                        <label for="sel_general">
                            <asp:Literal ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1" Text="Select all attributes"></asp:Literal></label>
                        <input type="checkbox" onclick="select_general()" id="sel_general" />
                    </td>
                </tr>
            </table>
            <asp:UpdatePanel ID="AttributesList" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="AssetType" EventName="SelectedIndexChanged" />
                </Triggers>
                <ContentTemplate>
                    <asp:Repeater runat="server" ID="AssetTypes" OnItemDataBound="AssetTypes_ItemDataBound">
                        <ItemTemplate>
                            <asp:HiddenField runat="server" ID="AttributeName" Value='<%# Eval("Name") %>' />
                            <asp:HiddenField runat="server" ID="AttributeUid" Value='<%# Eval("AttributeUid") %>' />
                            <strong>
                                <asp:Label Text='<%# Eval("Name") %>' runat="server" />
                            </strong>
                            <asp:GridView ID="FieldsList" runat="server" AutoGenerateColumns="False" meta:resourcekey="FieldsListResource1">
                                <RowStyle BackColor="White" />
                                <Columns>
                                    <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                                        <ItemTemplate>
                                            <asp:HiddenField runat="server" Value='<%# Eval("UID") %>' ID="UID" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" meta:resourcekey="BoundFieldResource1" />
                                    <asp:TemplateField meta:resourcekey="TemplateFieldResource2">
                                        <HeaderTemplate>
                                            <asp:Literal ID="Literal2" runat="server" meta:resourcekey="LiteralResource1" Text="Visible"></asp:Literal>
                                        </HeaderTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="IsVisible" meta:resourcekey="IsVisibleResource1" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField meta:resourcekey="TemplateFieldResource3">
                                        <HeaderTemplate>
                                            <asp:Literal ID="Literal3" runat="server" meta:resourcekey="LiteralResource2" Text="Filter by"></asp:Literal>
                                        </HeaderTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="IsFilter" meta:resourcekey="IsFilterResource1" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <asp:Literal runat="server" ID="lblNoData" meta:resourcekey="lblNoDataResource1"
                                        Text="No attributes"></asp:Literal>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </ItemTemplate>
                    </asp:Repeater>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div class="wizard-footer-buttons">
        <asp:Button ID="Save" Text="Save report" runat="server" OnClick="SaveReportClick"
            meta:resourcekey="SaveResource1" />
    </div>
</asp:Content>
