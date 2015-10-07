<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageSearchResult.master" AutoEventWireup="true" CodeBehind="SimpleSearchKeywords.aspx.cs" Inherits="AssetSite.Search.NewSimpleSearchKeywords" %>
<asp:Content ContentPlaceHolderID="SearchTopBar" runat="server">
		<div class="panel">
			<div class="panelSearch">
				<table width="100%">
					<tr>
						<td width="90%">
							<asp:TextBox ID="tbSearch" runat="server" Width="100%" 
								meta:resourcekey="tbSearchResource1" />
                            <input type="text" name="hidden" style="visibility:hidden;display:none;" />
						</td>
						<td>
                           <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
                                runat="server" ControlToValidate="tbSearch">*</asp:RequiredFieldValidator>
							<asp:Button ID="ButtonSearch" runat="server" Text="Search" 
								onclick="ButtonSearch_Click" meta:resourcekey="ButtonSearchResource1" />
						</td>
					</tr>
                    <tr>
                        <td colspan="3" class="period">
                            <asp:RadioButtonList runat="server" ID="rbActive" RepeatLayout="Flow" RepeatDirection="Horizontal">
                                <asp:ListItem Text="Active" meta:resourcekey="cbActiveResource1" Selected="True" Value="1" />
                                <asp:ListItem Text="History" meta:resourcekey="cbHistoryResource1" Value="0" />
                            </asp:RadioButtonList>
                        </td>
                    </tr>
				</table>
			</div>
			</div>
</asp:Content>