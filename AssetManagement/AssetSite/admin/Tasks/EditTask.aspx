<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master"
    AutoEventWireup="true" CodeBehind="EditTask.aspx.cs" Inherits="AssetSite.admin.Tasks.EditTask" %>

<%@ Register Src="~/Controls/TaskCommonParams.ascx" TagPrefix="uc1" TagName="CommonParams" %>
<%@ Register Src="~/Controls/Translations.ascx" TagName="Translations" TagPrefix="amc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        .funcColumn
        {
            width: 20px;
        }
        .labels
        {
            width: 150px !important;
        }
    </style>
    <script src="../../javascript/Tasks.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <asp:ScriptManager ID="mgrFroTasks" runat="server" EnablePageMethods="true">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
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
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName"
                                ErrorMessage="Name is required" Display="Dynamic"></asp:RequiredFieldValidator>
                            <a href="javascript:showTranslations()">
                                <asp:Literal runat="server" ID="Translations" Text="<% $Resources:Translations.Text %>" /></a>
                        </td>
                    </tr>
                    <tr>
                        <td class="labels">
                            <asp:Label ID="lblDesc" runat="server" Text="<% $Resources:Global, DescText %>"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="description"
                                MaxLength="1000" meta:resourcekey="txtNameResource1"></asp:TextBox>
                            <a href="javascript:showTranslationsFor('.description')">
                                <asp:Literal runat="server" ID="Literal1" Text="<% $Resources:Translations.Text %>" /></a>
                        </td>
                    </tr>
                    <tr>
                        <td class="labels">
                            <asp:Label ID="Label1" runat="server" Text="Function Type"></asp:Label>
                        </td>
                        <td class="controls">
                            <asp:DropDownList ID="ddlFunctionType" runat="server" CssClass="SelectControl" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlFunctionType_SelectedIndexChanged">
                                <asp:ListItem Text="Select..." Value="-1"></asp:ListItem>
                                <asp:ListItem Text="Save Search" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Launch Batch Task" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Import File" Value="2"></asp:ListItem>
                                <asp:ListItem Text="Export File (SSIS)" Value="6"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="metaCreateAssetListItem" Value="4"></asp:ListItem>
                                <asp:ListItem Text="Run SQL Server Agent Job" Value="7"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RangeValidator runat="server" ControlToValidate="ddlFunctionType" MinimumValue="0" MaximumValue="7" ErrorMessage="Please select a function type" />
                        </td>
                    </tr>
                    <tr> 
                        <td class="labels">
                            <asp:Label runat="server" Text="Display this task in the asset's sidebar" />
                        </td>
                        <td class="controls">
                            <asp:CheckBox runat="server" ID="chkShowAtSidebar" />
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
                        <p>
                            <asp:Label runat="server" CssClass="labels">Search url</asp:Label>
                            <asp:TextBox runat="server" ID="txtSearchUrl" CssClass="controls" />
                            <asp:RequiredFieldValidator 
                                runat="server" 
                                ID="vldSearchUrl"
                                ControlToValidate="txtSearchUrl" 
                                Enabled="false"
                                ForeColor="Red"
                                Text="*" />
                        </p>
                    </asp:View>
                    <asp:View ID="viewBatch" runat="server">
                        <uc1:CommonParams runat="server" ID="BatchCommonParams"></uc1:CommonParams>
                        <br />
                    </asp:View>
                    <asp:View ID="viewImport" runat="server">
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
                        <uc1:CommonParams runat="server" ID="importParams"></uc1:CommonParams>
                        <br />
                    </asp:View>
                    <asp:View ID="viewExport" runat="server">
                        <iframe src="../../Search/Search.aspx" width="100%" height="550px" style="border: 0;">
                        </iframe>
                    </asp:View>
                    <asp:View ID="viewNewAsset" runat="server">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td class="labels">
                                    <asp:Label ID="lblSelectScreen" runat="server" Text="Select Screen" meta:resourcekey="metaSelectScreen"></asp:Label>
                                </td>
                                <td class="controls">
                                    <asp:DropDownList ID="ddlView" runat="server" CssClass="SelectControl" DataTextField="Name"
                                        AppendDataBoundItems="true" DataValueField="ScreenId" DataSourceID="edsScreens">
                                        <asp:ListItem Value="0" Text="<% $Resources:DefaultScreenListItem.Text %>"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:EntityDataSource ID="edsScreens" runat="server" ConnectionString="name=DataEntities"
                                        DefaultContainerName="DataEntities" EnableFlattening="False" EntitySetName="AssetTypeScreen"
                                        Select="it.[ScreenId], it.[DynEntityConfigUid], it.[Name]" EntityTypeFilter=""
                                        Where="it.[DynEntityConfigUid]==@dynEntityConfigUid">
                                    </asp:EntityDataSource>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="viewExportSSIS" runat="server">
                        <uc1:CommonParams OnlySSIS="true" runat="server" ID="exportParams"></uc1:CommonParams>
                    </asp:View>
                    <asp:View runat="server" ID="viewExecuteSqlServerAgentJob">
                        <asp:Label runat="server" CssClass="labels" AssociatedControlID="dlAgentJobs">Select a job:</asp:Label>
                        <asp:DropDownList runat="server" ID="dlAgentJobs" />
                    </asp:View>
                </asp:MultiView>
                <asp:Label runat="server" ID="searchSessionEmpty" ForeColor="Red" Text="Please do a search in order to create task."
                    Visible="false" />
                <br />
                <br />
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSaveTaskConfig_Click" />
            </div>
        </div>
    </div>
    <amc:Translations ID="Translations1" controlselector=".SelectControl" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
</asp:Content>
