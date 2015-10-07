<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Categories.aspx.cs" Inherits="AssetSite.admin.Taxonomies.Categories"
    MasterPageFile="~/MasterPages/MasterPageDefault.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumb" runat="server">
    Categories list
</asp:Content>
<asp:Content ID="leftMenu" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="Server">
    <div class="wizard-header">
        Categories list
    </div>
    <div class="panel">
        <div class="panelheader">
            Categories
        </div>
        <div class="panelcontent">
            <asp:GridView ID="TaxonomiesGrid" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="UID"
                ForeColor="#333333" GridLines="None" CssClass="w100p" OnRowEditing="OnRowEditing" OnRowCancelingEdit="OnRowCancelingEdit"
                OnRowUpdating="OnRowUpdating" OnRowDeleting="OnRowDeleting">
                <RowStyle BackColor="White" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HiddenField runat="server" ID="UID" Value='<%# Eval("UID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-Width="30%" />                    
                    <asp:BoundField DataField="Description" HeaderText="Description" HeaderStyle-Width="50%" />
                    <asp:HyperLinkField DataNavigateUrlFields="UID" DataNavigateUrlFormatString="~/admin/Taxonomies/TreeEdit.aspx?Id={0}" Text="Tree" />
                    <asp:CommandField ShowEditButton="True" HeaderStyle-Width="5%" ButtonType="Image"
                        EditImageUrl="/images/buttons/edit.png" UpdateImageUrl="/images/buttons/edit.png" CancelImageUrl="/images/buttons/delete.png"/>
                    <asp:CommandField ShowDeleteButton="True" HeaderStyle-Width="5%" ButtonType="Image"
                        DeleteImageUrl="/images/buttons/delete.png" />
                </Columns>
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#DFDFDF" Font-Bold="True" ForeColor="#1E1E1E" HorizontalAlign="Left" />
                <AlternatingRowStyle BackColor="#E0FFC1" />
                <EmptyDataTemplate>
                    No categories
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
        <div class="panel">
        <div class="panelheader">
            New category
        </div>
        <div class="panelcontent">
            <table cellpadding="0" cellspacing="5">
                <tr>
                    <td>
                        Name
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="TaxText" MaxLength="10"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Description
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="TaxDescr"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="Button1" runat="server" Text="Add" OnClick="AddCategory" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </div>
</asp:Content>