<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskCommonParams.ascx.cs" Inherits="AssetSite.Controls.TaskCommonParams" %>

<table border="0" cellpadding="0" cellspacing="0" width="100%">
    <tr>
        <td class="labels">
            <asp:Label ID="Label5" runat="server" Text="Executable Type"></asp:Label>
        </td>
        <td class="controls">
            <asp:DropDownList ID="ddlExecType" AutoPostBack="True" runat="server" CssClass="SelectControl" >
                <asp:ListItem Text="SQL Server Integartion Services(SSIS)" Value="2"></asp:ListItem>
                <asp:ListItem Text="Exe File" Value="1"></asp:ListItem>
                <asp:ListItem Text="Predefined task" Value="3"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <asp:Panel ID="RegularParams" runat="server">
        <tr>
            <td class="labels">
                <asp:Label ID="Label6" runat="server" Text="Upload Executable"></asp:Label>
            </td>
            <td class="controls">
                <asp:FileUpload ID="fuExec" runat="server" />
                <asp:RequiredFieldValidator runat="server" ID="fuExecValidator" ControlToValidate="fuExec" Display="Dynamic" Text="Please select a file"  />
                <asp:CustomValidator runat="server" ID="fuExecFileExtValidator" ControlToValidate="fuExec" OnServerValidate="OnFileUploadValidation" Display="Dynamic" Text="" />
                <br />
                <asp:Label ID="lblExecutablePath" runat="server" Text="<%#this.ExecutablePath %>"></asp:Label>
            </td>
        </tr>
    </asp:Panel>
    <asp:Panel ID="PredefinedTaskParams" Visible="False" runat="server">
         <tr>
            <td class="labels">
                <asp:Label ID="Label1" runat="server" Text="Pick a task"></asp:Label>
            </td>
            <td class="controls">
            <asp:DropDownList ID="DropDownListTasks" runat="server" CssClass="SelectControl" >
                <asp:ListItem Text="SobImportPayments" Value="SobImportPayments"></asp:ListItem>
            </asp:DropDownList>
            </td>
        </tr>
    </asp:Panel>
    <tr>
        <td colspan="2">
            <br />
            <hr />
            <table border="0" id="paramsGrid" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="controls">
                        Name
                    </td>
                    <td class="controls">
                        Value
                    </td>
                    <td class="funcColumn">
                                                
                    </td>
                </tr>
                <tr>
                    <td class="controls">
                        <a onclick="return AddParam();" style="cursor:pointer;">Add New Param</a>
                        <asp:HiddenField ID="hfldAddedParams" runat="server" />
                    </td>
                    <td class="controls">
                    </td>
                    <td class="funcColumn">
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Literal ID="litScript" runat="server"></asp:Literal>