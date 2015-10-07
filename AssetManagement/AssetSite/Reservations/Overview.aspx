<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.Master" AutoEventWireup="true" CodeBehind="Overview.aspx.cs" Inherits="AssetSite.Asset.Reservations.Overview" EnableEventValidation="false" meta:resourcekey="PageResource1" %>
<%@ Register Assembly="DayPilot" Namespace="DayPilot.Web.Ui" TagPrefix="DayPilot" %>
<%@ Register Src="~/Controls/UsersList.ascx" TagName="UsersList" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link href="/css/daypilot/scheduler_white.css" rel="stylesheet" type="text/css" />
    <link href="/css/daypilot/areas.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <asp:ScriptManager ID="ServiceMgr" runat="server">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div id="main-container">
        
        <div id="ReleaseDialog" runat="server" style="display:none;">
            <span style="color:Red;" id="releaseError"></span>
            <table border="0" cellpadding="1" cellspacing="1" width="100%">
                <tr>
                    <td>
                        <asp:Label ID="Label1" AssociatedControlID="cbIsDamaged" runat="server" meta:resourcekey="Label1Resource1">Is Damaged:</asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="cbIsDamaged" runat="server" 
                            meta:resourcekey="cbIsDamagedResource1" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" AssociatedControlID="tbRemark">Remarks:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbRemark" runat="server" ></asp:TextBox>
                    </td>
                </tr>
            </table>
            <asp:HiddenField runat="server" ID="ReservationUid" Value="0" />
        </div>

        <div id="ReservationsDialog" runat="server" style="display:none;">
            <asp:Label ID="lblNewBorrowError" runat="server" style="color:Red;" />
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server">Asset to reserve:</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlNewResource" runat="server" DataValueField="UID" DataTextField="Name" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label10" runat="server">For user:</asp:Label>
                    </td>
                    <td>
                        <uc1:UsersList ID="UsersList1" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label7" runat="server">From:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbNewStartDate" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label8" runat="server">To:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbNewEndDate" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label9" runat="server">Reason:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbNewReason" runat="server" />
                    </td>
                </tr>
            </table>
        </div>

        <div class="panels_leftcol_container">
             <div class="panel" id="printAreaDiv">
                <div class="panelheader">
                    <asp:Label ID="lblIsActive" runat="server" Text="Reservations" 
                        meta:resourcekey="lblIsActiveResource1"></asp:Label><br />
                </div>
                <div class="panelcontent">
                    <asp:LinkButton ID="lbtnRebind" runat="server" Text="rebind" Visible="False" 
                        onclick="lbtnRebind_Click" meta:resourcekey="lbtnRebindResource1"></asp:LinkButton>
                    <asp:GridView ID="gvHistory" runat="server" AllowPaging="True" CssClass="w100p" 
                        AutoGenerateColumns="False" 
                        PageSize="25"
                        OnPageIndexChanging="gvHistory_PageIndexChanging" 
                        OnRowDataBound="gvHistory_OnRowDataBound"
                        meta:resourcekey="gvHistoryResource1">
                        <Columns>
                            <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                                <ItemTemplate>
                                    <div style="display:none;" id='templateContainer<%# Eval("ReservationUid") %>'>
                                        <%# Eval("Description") %>
                                    </div>
                                    <a href="#" rel='<%# Eval("ReservationUid") %>' 
                                        class="assetTemplatePopupTrigger" 
                                        style="text-decoration:none; border:0; cursor:pointer;">
                                        <asp:Image runat="server" ImageUrl="~/images/info.png" 
                                        meta:resourcekey="ImageResource1"/>
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:HyperLinkField 
                                DataTextField="Name"
                                DataNavigateUrlFormatString="/Asset/View.aspx?assetTypeUID={0}&assetUID={1}" 
                                DataNavigateUrlFields="AssetTypeUid,AssetUid"/>
                            <asp:BoundField HeaderText="Start Date" DataField="StartDate"
                                HeaderStyle-HorizontalAlign="Left" meta:resourcekey="BoundFieldResource2" >                             
                            </asp:BoundField>
                            <asp:BoundField HeaderText="End Date" DataField="EndDate" 
                                HeaderStyle-HorizontalAlign="Left" meta:resourcekey="BoundFieldResource3">
                            </asp:BoundField>
                            <asp:BoundField HeaderText="User" DataField="Borrower" 
                                HeaderStyle-HorizontalAlign="Left" meta:resourcekey="BoundFieldResource5">
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left" meta:resourcekey="TemplateFieldResource2">
                                <ItemTemplate>                                    
                                        <a style="cursor:pointer; border:0;" runat="server" 
                                            Visible='<%# ((bool)Eval("IsBorrowed")) && CanBorrowOrRelease((string)Eval("StartDate")) %>'
                                            title='Bring back'
                                            onclick='<%# "return ShowReleaseReservationDialog(" + (long)Eval("ReservationUid") + ")" %>'>
                                            <asp:Image runat="server" ImageUrl='<%# GetImgUrl((bool)Eval("IsBorrowed")) %>' />
                                        </a>   
                                        <a style="cursor:pointer; border:0;" runat="server" 
                                            Visible='<%# (!(bool)Eval("IsBorrowed")) && CanBorrowOrRelease((string)Eval("StartDate")) %>'
                                            title='Borrow'
                                            onclick='<%# "return MarkReservationBorrowed(" + (long)Eval("ReservationUid") + ")" %>'>
                                            <asp:Image runat="server" ImageUrl='<%# GetImgUrl((bool)Eval("IsBorrowed")) %>' />
                                        </a>                                   
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:ImageButton 
                                        ID="ibtnCancel" 
                                        runat="server" 
                                        Title="Cancel reservation"
                                        CommandArgument='<%# Eval("ReservationUid") %>' 
                                        Visible='<%# Eval("IsNotBorrowed") %>' 
                                        ImageUrl="~/images/buttons/delete.png" 
                                        oncommand="ibtnCancel_Command" 
                                        meta:resourcekey="ibtnCancelResource1" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>                            
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

            <div class="panel" id="pnlReservDashboard">
                <div class="panelheader">
                    <asp:Label ID="lblReservDashboard" runat="server" Text="Reservation dashboard"></asp:Label><br />
                </div>
                <div class="panelcontent" style="padding-left:0; padding-right:0;">
                    <DayPilot:DayPilotScheduler 
                        ID="scheduler" 
                        runat="server" 
                        DataStartField="StartDate" 
                        DataEndField="EndDate" 
                        DataTextField="Borrower" 
                        DataValueField="BorrowerId" 
                        DataResourceField="AssetUID" 
                        DataTagFields="ReservationUid, AssetUID, Borrower, BorrowerId, Reason, IsBorrowed"
                        HeaderFontSize="8pt" 
                        HeaderHeight="20" 
                        EventHeight="30"
                        EventFontSize="11px" 
                        Width="945" 
                        RowHeaderWidth="120"
                        CellDuration="1440" 
                        CellGroupBy="Month" 
                        CellWidth="60"
                        BusinessBeginsHour="5" 
                        BusinessEndsHour="24"
                        UseEventBoxes="Always" 
                        EnableViewState="false" 
                        ScrollLabelsVisible="false" 
                        ShowToolTip="false"
                        HeightSpec="Max" 
                        Height="500" 
                        TreeEnabled="false"
                        BorderColor="#666666"
                        
                        CssClassPrefix="scheduler_white"
                        CssOnly="true"
                        HourNameBackColor="#F0F0F0" 
        
                        SyncResourceTree="true" 
                        DragOutAllowed="true"
                        MoveBy="Top"
                        DurationBarVisible = "false"
                        TimeRangeSelectedHandling="JavaScript"
                        EventClickHandling="JavaScript"
                        OnBeforeCellRender="scheduler_OnBeforeCellRender"
                        Visible="False">
                    </DayPilot:DayPilotScheduler>
                </div>
            </div>
        </div>
</div>
    <script type="text/javascript">
        $(function () {
            $('.scheduler_white_corner div').eq(1).remove();
        });
    </script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="PlaceHolderLeftColumn" runat="server">
    <div class="wizard-menu">
        <div class="active">
            <asp:Label ID="Label3" runat="server" meta:resourcekey="Label3Resource1">Asset Types</asp:Label>            
        </div>
        <span style="font-size:smaller;">
            <asp:Repeater ID="repAssetTypes" runat="server">
                <ItemTemplate>
                    <asp:LinkButton ID="lbAssetType" runat="server" Text='<%# Eval("Name") %>' 
                        CommandArgument='<%# Eval("UID") %>' OnCommand="lbtnAssetType_Command" 
                        meta:resourcekey="lbAssetTypeResource1"></asp:LinkButton><br />
                </ItemTemplate>
            </asp:Repeater>    
        </span>
    </div>
</asp:Content>

