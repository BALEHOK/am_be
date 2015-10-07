 <%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" CodeBehind="EditTask.aspx.cs" Inherits="AssetSite.admin.Tasks.EditTask" %>

<%@ Register Src="~/Controls/TaskCommonParams.ascx" TagPrefix="uc1" TagName="CommonParams" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">

<style type="text/css">
    .funcColumn
    {
        width:20px;
    }
    .labels
    {
        width:150px !important;
    }
</style>

<script src="../../javascript/Tasks.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <asp:ScriptManager ID="mgrFroTasks" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <div id="main-container">
        <div class="panel">
            <div class="panelheader">
                <asp:Label ID="Label2" runat="server" Text="General Task Information"></asp:Label>            
            </div>
        
            <div class="panelcontent">
                <table border="0" cellpadding="0" cellspacing="0" class="w100p">
                    <tr>
                        <td class="labels">
                            <asp:Label ID="lblName" runat="server" Text="<% $Resources:Global, NameText %>"></asp:Label>                                
                        </td>
                        <td class="controls">
                            <asp:TextBox CssClass="SelectControl name" ID="txtName" runat="server" MaxLength="60"
                                    meta:resourcekey="txtNameResource1"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
                                runat="server"
                                ControlToValidate="txtName"
                                ErrorMessage="Please enter attribute name."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="labels">
                            <asp:Label ID="lblDesc" runat="server" Text="<% $Resources:Global, DescText %>"></asp:Label>                                
                        </td>
                        <td class="controls">
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="description" MaxLength="1000"
                                    meta:resourcekey="txtNameResource1"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
                                runat="server"
                                ControlToValidate="txtName"
                                ErrorMessage="Please enter attribute name."></asp:RequiredFieldValidator>
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
                            <asp:Label ID="Label1" runat="server" Text="Function Type"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:DropDownList ID="ddlFunctionType" runat="server" CssClass="SelectControl" 
                                AutoPostBack="true" 
                                onselectedindexchanged="ddlFunctionType_SelectedIndexChanged">
                                <asp:ListItem Text="Select..." Value="-1"></asp:ListItem>
                                <asp:ListItem Text="Save Search" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Launch Batch Task" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Import File" Value="2"></asp:ListItem>
                                <asp:ListItem Text="Export File" Value="3"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="metaCreateAssetListItem" Value="4"></asp:ListItem>
                                <%--<asp:ListItem Text="Print Report" Value="5"></asp:ListItem>--%>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="panel">
            <div class="panelheader">
                <asp:Label ID="Label4" runat="server" Text="Task Configuration"></asp:Label>            
            </div>
        
            <div class="panelcontent">
                <asp:MultiView ID="mvFunctions" runat="server" ActiveViewIndex="-1">
                    <asp:View ID="viewSearch" runat="server">
                        <iframe src="../../Search/Search.aspx" width="100%" height="550px" style="border:0;"></iframe>
                    </asp:View>
                    <asp:View ID="viewBatch" runat="server">
                        <uc1:CommonParams runat="server" ID="BatchCommonParams"></uc1:CommonParams>
                        <br />
                    </asp:View>
                    <asp:View ID="viewExport" runat="server">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td class="labels">
                                    <asp:Label ID="Label5" runat="server" Text="File Type"></asp:Label>
                                </td>
                                <td class="controls">
                                    <asp:DropDownList ID="ddlOutputFileType" runat="server" CssClass="SelectControl">
                                        <asp:ListItem Text="Microsoft Excel" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="XML" Value="1"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                        <uc1:CommonParams runat="server" ID="exportParams"></uc1:CommonParams>
                        <br />
                    </asp:View>
                    <asp:View ID="viewImport" runat="server">
                    </asp:View>
                    <asp:View ID="viewNewAsset" runat="server">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td class="labels">
                                    <asp:Label ID="Label6" runat="server" Text="Select Screen"></asp:Label>
                                </td>
                                <td class="controls">
                                    <asp:DropDownList ID="ddlView" runat="server" CssClass="SelectControl" DataTextField="Name"
                                     DataValueField="ScreenId" DataSourceID="edsScreens">
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsScreens" runat="server" 
                                        ConnectionString="name=DataEntities" DefaultContainerName="DataEntities" 
                                        EnableFlattening="False" EntitySetName="AssetTypeScreen" 
                                        Select="it.[ScreenId], it.[DynEntityConfigUid], it.[Name]" 
                                        EntityTypeFilter="" Where="it.[DynEntityConfigUid]==@dynEntityConfigUid">
                                    </asp:EntityDataSource>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="viewPrintReport" runat="server">
                    </asp:View>
                </asp:MultiView>
                <br />
                <asp:Button ID="btnSave" runat="server" Text="Finished" OnClick="btnSaveTaskConfig_Click" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
</asp:Content>

