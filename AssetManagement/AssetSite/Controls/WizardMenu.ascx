<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WizardMenu.ascx.cs" Inherits="AssetSite.Controls.WizardMenu" %>
<asp:Panel ID="itemsContainer" runat="server" CssClass="wizard-menu" 
    meta:resourcekey="itemsContainerResource1">

    <asp:Panel ID="step_1" runat="server" meta:resourcekey="step_1Resource1">        
        <asp:Panel ID="title_1" runat="server" CssClass="item pointer" onclick="javascript:location:href='/Wizard/Step1.aspx'"
        meta:resourcekey="title_1Resource1">
        
            <asp:LinkButton 
                runat="server"
                CssClass="title"
                PostBackUrl="~/Wizard/Step1.aspx"
                meta:resourcekey="HyperLinkResource1" Text="Asset Name"></asp:LinkButton>
                           
        </asp:Panel>
             
        <asp:Panel ID="desc_1" runat="server" CssClass="subitem" meta:resourcekey="desc_1Resource1">
            <asp:Label runat="server" meta:resourcekey="LabelResource1" 
                Text="Algemene definitie:"></asp:Label><br/>           
            <asp:Label ID="Label1" runat="server" meta:resourcekey="Label1Resource1" 
                Text="- Naam"></asp:Label><br/> 
            <asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1" 
                Text="- Omschrijving"></asp:Label><br/>    
        </asp:Panel>
    </asp:Panel>
    
    <asp:Panel ID="step_2" runat="server" meta:resourcekey="step_2Resource1">        
        <asp:Panel ID="title_2" runat="server" CssClass="item" meta:resourcekey="title_2Resource1">
        
            <asp:LinkButton 
                runat="server" 
                CssClass="title" 
                PostBackUrl="~/Wizard/Step2.aspx" meta:resourcekey="LinkButtonResource1">Specifieke kenmerken</asp:LinkButton>
              
        </asp:Panel>
        
        <asp:Panel ID="desc_2" runat="server" CssClass="subitem" Visible="False" meta:resourcekey="desc_2Resource1">
            <asp:Label runat="server" meta:resourcekey="LabelResource2" 
                Text="- Abstract type"></asp:Label><br />
            <asp:Label ID="Label3" runat="server" meta:resourcekey="Label3Resource1" 
                Text="- Parent (overerven van ...)"></asp:Label><br />
            <asp:Label ID="Label4" runat="server" meta:resourcekey="Label4Resource1" 
                Text="- Verbruiksartikel"></asp:Label><br />          
        </asp:Panel>
    </asp:Panel>
    
    <asp:Panel ID="step_3" runat="server" meta:resourcekey="step_3Resource1">        
        <asp:Panel ID="title_3" runat="server" CssClass="item" meta:resourcekey="title_3Resource1">
            
            <asp:LinkButton 
                runat="server"
                CssClass="title"
                PostBackUrl="~/Wizard/Step3.aspx"
                meta:resourcekey="HyperLinkResource3" Text="Attributen"></asp:LinkButton>
                        
        </asp:Panel>
        
        <asp:Panel ID="desc_3" runat="server" CssClass="subitem" Visible="False" meta:resourcekey="desc_3Resource1">
            <asp:Label runat="server" meta:resourcekey="LabelResource3" 
                Text="Gedefinieerde attributen"></asp:Label><br />
            <asp:Label ID="Label5" runat="server" meta:resourcekey="Label5Resource1" 
                Text="- Naam"></asp:Label><br />
            <asp:Label ID="Label6" runat="server" meta:resourcekey="Label6Resource1" 
                Text="- Model"></asp:Label><br />                  
            
            <asp:LinkButton
                runat="server"
                ID="substep_3_1"
                PostBackUrl="~/Wizard/Step4.aspx"
                meta:resourcekey="substep_3_1Resource1" 
                Text="Define attributes"></asp:LinkButton><br />  

        </asp:Panel>
        
    </asp:Panel>   
    
    <asp:Panel ID="step_5" runat="server" meta:resourcekey="step_5Resource1">        
        <asp:Panel ID="title_5" runat="server" CssClass="item" meta:resourcekey="title_5Resource1">
            <asp:HyperLink runat="server" CssClass="title" 
                NavigateUrl="~/Wizard/Step9.aspx" meta:resourcekey="HyperLinkResource5" 
                Text="Sleutelwoorden &amp; Index"></asp:HyperLink>                
        </asp:Panel>
        
        <asp:Panel ID="desc_5" runat="server" CssClass="subitem" Visible="False" meta:resourcekey="desc_5Resource1">
            
        </asp:Panel>
    </asp:Panel>

    <asp:Panel ID="step_6" runat="server" meta:resourcekey="step_6Resource1">    
        <asp:Panel ID="title_6" runat="server" CssClass="item" meta:resourcekey="title_6Resource1">                        
            <asp:HyperLink runat="server" CssClass="title" 
                NavigateUrl="~/Wizard/Step10.aspx" meta:resourcekey="HyperLinkResource6" 
                Text="Categorie &amp; Taxonomie"></asp:HyperLink>                
        </asp:Panel>
        
        <asp:Panel ID="desc_6" runat="server" CssClass="subitem" Visible="False" meta:resourcekey="desc_6Resource1">
            
        </asp:Panel>
    </asp:Panel>
    
    <asp:Panel ID="step_7" runat="server" meta:resourcekey="step_7Resource1">        
        <asp:Panel ID="title_7" runat="server" CssClass="item" meta:resourcekey="title_7Resource1">
            <asp:HyperLink runat="server" CssClass="title" 
                NavigateUrl="~/Wizard/Step11.aspx" meta:resourcekey="HyperLinkResource7" 
                Text="Release-informatie"></asp:HyperLink>            
        </asp:Panel>
        
        <asp:Panel ID="desc_7" runat="server" CssClass="subitem" Visible="False" meta:resourcekey="desc_7Resource1">            
        </asp:Panel>
    </asp:Panel>

</asp:Panel>

<script type="text/javascript">
    $(document).ready(function() {
        $('.item').hover(
            function() {
                $(this).css('background-color', '#84d859');
            },
            function() {
                $(this).css('background-color', '#999999');
            }
        );
    });
</script>