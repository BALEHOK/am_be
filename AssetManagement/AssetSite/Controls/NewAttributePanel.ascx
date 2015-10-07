<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewAttributePanel.ascx.cs" Inherits="AssetSite.Controls.NewAttributePanel" %>
 <div class="<%= CssClass %>">
     <div class="panel" id="printAreaDiv">
        <div class="panelheader">
            <%= this.TranslatedHeader %><br />
        </div>
        <div class="panelcontent">
            <asp:GridView ID="GridAttributes" 
                runat="server" 
                SkinID="asset_panel"
                EnableViewState="true"                
                AutoGenerateColumns="False" 
                ShowHeader="false"
                Width="100%"
                GridLines="None">
                <EmptyDataTemplate>No data</EmptyDataTemplate> 
                <Columns>
                    <asp:TemplateField ControlStyle-Width="20%" ItemStyle-CssClass="gridlabels"></asp:TemplateField>  
                    <asp:TemplateField ItemStyle-CssClass="gridcontrols"></asp:TemplateField>
                </Columns>             
            </asp:GridView>         
        </div>
    </div>
</div>