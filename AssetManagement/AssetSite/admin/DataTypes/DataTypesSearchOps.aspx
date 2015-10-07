<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataTypesSearchOps.aspx.cs"
    Inherits="AssetSite.admin.DataTypesSearchOps" MasterPageFile="~/MasterPages/MasterPageDefault.Master" %>

<asp:Content ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <div class="panel">
        <div class="panelheader">
            <asp:Label runat="server" ID="panelTitle" Text="Search operators" meta:resourcekey="panelTitleResource1"></asp:Label>
        </div>
        <div class="panelcontent">
            <asp:GridView runat="server" ID="OperatorsList" DataSourceID="SearchOperatorsDataSource"
                AutoGenerateColumns="false" OnRowDataBound="OperatorsList_DataBinding">
                <Columns>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="Selected" />
                            <asp:HiddenField runat="server" ID="UID" Value='<%# Eval("SearchOperatorUid") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Operator" HeaderText="Operator" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="Description" HeaderText="Description" HeaderStyle-HorizontalAlign="Left"  />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div class="wizard-footer-buttons">
        <asp:Button runat="server" ID="SaveBtn" Text="Save" OnClick="SaveBtn_Click" />
    </div>    
      <asp:EntityDataSource runat="server" ID="SearchOperatorsDataSource"
        ConnectionString="name=DataEntities"
        DefaultContainerName="DataEntities"
        EnableFlattening="false"
        EntitySetName="SearchOperators">
    </asp:EntityDataSource>
</asp:Content>
