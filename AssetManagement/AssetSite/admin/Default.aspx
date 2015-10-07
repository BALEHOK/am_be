<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageDefault.master" AutoEventWireup="true" Inherits="AssetSite.admin.Default" Codebehind="Default.aspx.cs" meta:resourcekey="PageResource1" %>
<%@ Register Assembly="AppFramework.Core" Namespace="AppFramework.Core.PL.Components" TagPrefix="dync" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMiddleColumn" Runat="Server">

    <div class="panel configobjects" style="float:left;" runat="server" ID="panelConfigureObjects">
        <div class="panelheader">
            <asp:Label ID="Label1" runat="server" Text="Configuratie objecten (assets)" 
                meta:resourcekey="Label1Resource1"></asp:Label>
        </div>
        <div class="panelcontent">
            <p>       
                <asp:LinkButton runat="server" OnClick="NewAT_Click" meta:resourcekey="HyperLink1Resource1">Create AssetType</asp:LinkButton>       
                <br />                
                <asp:Label runat="server" Text="Creeer via een wizard een nieuw type object (asset/bedrijfsmiddel)." 
                    ID="lblCreateAssetDesc" meta:resourcekey="lblCreateAssetDescResource1"></asp:Label>
            </p>            
            <p>
                <asp:HyperLink ID="HyperLink2" 
                    runat="server"
                    NavigateUrl="~/Wizard/EditAssetType.aspx"
                    Text="Edit AssetType" meta:resourcekey="HyperLink2Resource1"></asp:HyperLink>
                <br />                
                <asp:Label runat="server" ID="lblEditATDesc" 
                    Text="Wijzig een bestaand type object. Voeg attributen toe of verander labels, indexes, enz." 
                    meta:resourcekey="lblEditATDescResource1"></asp:Label>
            </p>            
            <p>                
                <asp:HyperLink ID="HyperLink3" runat="server" 
                    NavigateUrl="~/admin/DynList/Default.aspx" Text="Onderhoud lijsten" 
                    meta:resourcekey="HyperLink3Resource1"></asp:HyperLink>
                <br />                
                <asp:Label runat="server" ID="lblDynListDesc" 
                    Text="Onderhoud de algemene selectielijsten. Voeg items toe of wijzig de omschrijvingen." 
                    meta:resourcekey="lblDynListDescResource1"></asp:Label>
            </p> 
            <p>
                <asp:HyperLink ID="HyperLink16" runat="server" NavigateUrl="~/admin/Tasks/TasksList.aspx" meta:resourcekey="HyperLinkTasksRes"></asp:HyperLink>
            </p> 
        </div>    
    </div>
    
     <div class="panel configtaxonomy" style="float:right;" runat="server" ID="panelConfigureTaxonomies">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblTaxHeader" 
                Text="Configuratie categorieen/taxononomieen" 
                meta:resourcekey="lblTaxHeaderResource1"></asp:Label>
        </div>
        <div class="panelcontent">
             <p>                
                <asp:HyperLink ID="HyperLink4" runat="server" 
                     NavigateUrl="~/admin/Taxonomies/Default.aspx" 
                     Text="Creatie/onderhoud taxonomieen" meta:resourcekey="HyperLink4Resource1"></asp:HyperLink>
                <br />                
                <asp:Label runat="server" ID="lblCreateTaxDesc" 
                     Text="Creeer nieuwe taxonomieen of bestaande aan door ze uit te breiden of bepaalde takken anders in te delen ..." 
                     meta:resourcekey="lblCreateTaxDescResource1"></asp:Label>
            </p>  
        </div>    
    </div>

    <div style="clear:both"></div>
    
    <div class="panel configusers" style="float:left;">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblUsersHeader" 
                Text="Configuratie gebruikers en gebruikersgroepen" 
                meta:resourcekey="lblUsersHeaderResource1"></asp:Label>
        </div>
        <div class="panelcontent">
            <p>               
                <asp:HyperLink ID="HyperLink5" runat="server" 
                    NavigateUrl="~/admin/Users/Default.aspx" Text="Manage users" 
                    meta:resourcekey="HyperLink5Resource1"></asp:HyperLink>
                <br />                
                <asp:Label runat="server" 
                    Text="Creeer en/of onderhoud gebruikers en gebruikersgroepen." 
                    ID="lblUsersCreatDesc" meta:resourcekey="lblUsersCreatDescResource1"></asp:Label> 
            </p>
            <p>                
                <asp:HyperLink ID="HyperLink6" runat="server" 
                    NavigateUrl="~/admin/Users/ViewRights.aspx" Text="Rights to users" 
                    meta:resourcekey="HyperLink6Resource1"></asp:HyperLink>
                <br />                
                <asp:Label runat="server" ID="lblRightsDesc" 
                    Text="Wijs rechten en functionaliteiten toe aan gebruikers en gebruikersgroepen." 
                    meta:resourcekey="lblRightsDescResource1"></asp:Label>
            </p>   
        </div>    
    </div>

    
    <div class="panel appsettings" style="float:right;">
        <div class="panelheader">
            <asp:Label runat="server" ID="lblGlobalSettHeader" 
                Text="Applicatie instellingen en configuraties" 
                meta:resourcekey="lblGlobalSettHeaderResource1"></asp:Label>
        </div>
        <div class="panelcontent">
            <p runat="server" ID="linkTypesAndValidation">                
                <asp:HyperLink ID="HyperLink7" runat="server" 
                    NavigateUrl="~/admin/DataTypes/DataTypes.aspx" Text="Onderhoud datatypes en validaties" 
                    meta:resourcekey="HyperLink7Resource1"></asp:HyperLink>
                <br />
                <asp:Label runat="server" ID="lblDataTypesDesc" 
                    meta:resourcekey="lblDataTypesDescResource1"></asp:Label>
            </p> 
            <p runat="server" ID="linkContexts">
                <asp:HyperLink ID="lnkContexts" runat="server"
                    NavigateUrl="~/admin/Contexts/Default.aspx" Text="Contexts" meta:resourcekey="lnkContextsResource1"></asp:HyperLink>            
                <asp:Label runat="server" ID="lblContextsDesc" 
                        meta:resourcekey="lblContextsDescResource1"></asp:Label>
            </p>
            <p>                
                <asp:HyperLink ID="HyperLink8" runat="server" 
                    NavigateUrl="~/admin/Batch/Default.aspx" Text="Batch system" 
                    meta:resourcekey="HyperLink8Resource1"></asp:HyperLink>
                <br />
                <asp:Label runat="server" ID="lblBatchDesc" 
                    meta:resourcekey="lblBatchDescResource1"></asp:Label>
            </p>
            <p>                
                <asp:HyperLink ID="HyperLink9" runat="server" 
                    NavigateUrl="~/admin/Reports/Default.aspx" Text="Reports" 
                    meta:resourcekey="HyperLink9Resource1"></asp:HyperLink>
                <br />
                <asp:Label runat="server" ID="lblReportsDesc" 
                    meta:resourcekey="lblReportsDescResource1"></asp:Label>
            </p>
            <p><asp:HyperLink ID="menuMobile" runat="server" NavigateUrl="~/Mobile/Default.aspx"   meta:resourcekey="HyperLinkMobileResource1"></asp:HyperLink></p>   
            <p>                
                <asp:HyperLink ID="HyperLink10" runat="server" 
                    NavigateUrl="~/admin/Import/Default.aspx" Text="Import" 
                    meta:resourcekey="HyperLink10Resource1"></asp:HyperLink>
                <br />
                <asp:Label runat="server" ID="lblImportDesc" 
                    meta:resourcekey="lblImportDescResource1"></asp:Label>
            </p>   
            <p>                
                <asp:HyperLink ID="HyperLink11" runat="server" 
                    NavigateUrl="~/admin/Export/Default.aspx" Text="Export" 
                    meta:resourcekey="HyperLink11Resource1"></asp:HyperLink>                
                <br />
                <asp:Label runat="server" ID="lblExportDesc" 
                    meta:resourcekey="lblExportDescResource1"></asp:Label>
            </p>   
            <p runat="server" ID="linkSynchronization">                
                <asp:HyperLink ID="HyperLink14" runat="server" 
                    NavigateUrl="~/admin/Synk/Default.aspx" Text="Synchronization" 
                    meta:resourcekey="HyperLink14Resource1"></asp:HyperLink>                
                <br />
                <asp:Label runat="server" ID="Label3" 
                    meta:resourcekey="lblSynkDescResource1"></asp:Label>
            </p>   
             <p runat="server" ID="linkSearch">                
                <asp:HyperLink ID="SearchLink" runat="server" 
                    NavigateUrl="~/admin/Search/Default.aspx" Text="Search" 
                    meta:resourcekey="SearchLinkResource1"></asp:HyperLink>                
                <br />
                <asp:Label runat="server" ID="lblSearchDesc" 
                    meta:resourcekey="lblSearchDescResource1"></asp:Label>
            </p>   
            <p runat="server" ID="linkGlobalSettings"> 
                <asp:HyperLink ID="HyperLink12" runat="server" 
                    NavigateUrl="~/Configuration/Default.aspx" Text="Global application settings" 
                    meta:resourcekey="HyperLink12Resource1"></asp:HyperLink>
                <br />
                <asp:Label runat="server" ID="lblGSettDesc" 
                    meta:resourcekey="lblGSettDescResource1"></asp:Label>
            </p>

            <p><asp:HyperLink ID="linkLocationMove" runat="server" NavigateUrl="~/admin/LocationMove.aspx" meta:resourcekey="HyperLinkLocationRes"></asp:HyperLink></p>
            <p><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/admin/FAQItems.aspx" meta:resourcekey="HyperLinkFaqRes"></asp:HyperLink></p>
            <p><asp:HyperLink ID="HyperLink15" runat="server" NavigateUrl="~/admin/ZipsAndPlaces.aspx" meta:resourcekey="HyperLinkZipsRes"></asp:HyperLink></p>
            
            <p>
                <asp:HyperLink ID="HyperLink13" runat="server" 
                    NavigateUrl="~/admin/ServiceOps.aspx" Text="Service operations"></asp:HyperLink>
                <br />
                <asp:Label runat="server" ID="Label2" meta:resourcekey="LabelApplicationLabelRes" Text="Additional service operations for reindexing search, monitoring cache"></asp:Label>
            </p>
        </div>    
    </div>

</asp:Content>


