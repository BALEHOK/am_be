<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageWizard.master"
    AutoEventWireup="true" Inherits="AssetSite.Wizard.Step10" Codebehind="Step10.aspx.cs" meta:resourcekey="PageResource1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="WizardContent" runat="Server">
   <script runat="server">
   
       
   </script>
   <script language="javascript" type="text/javascript">

       function postBackByObject() {
           var o = window.event.srcElement;
           if (o.tagName == "INPUT" && o.type == "checkbox") {
               __doPostBack("", "");
           }
       }
    </script>
        <div class="wizard-header">
            <asp:Label runat="server" ID="lblheader" Text="Asset Wizard" 
                meta:resourcekey="lblheaderResource1"></asp:Label>&nbsp;&mdash;&nbsp; 
            <asp:Literal runat="server" ID="stepTitle" 
                 Text="Schermweergave — Categorie & Taxonomie" 
                meta:resourcekey="stepTitleResource1"></asp:Literal>    
        </div>
        <p>
            <asp:Literal runat="server" ID="stepDesc" 
                Text="Mauris congue consectetuer quam. " meta:resourcekey="stepDescResource1"></asp:Literal>
        </p>        
        <div class="panel">
            <div class="panelheader">
                <asp:Label runat="server" ID="panelTitle" 
                    Text="Selecteer standaard categorie (optioneel)" 
                    meta:resourcekey="panelTitleResource1"></asp:Label>
            </div>
            <div class="panelcontent">
                <amc:TaxonomiesDropDown ID="TaxonomiesAll" runat="server" 
                    OnSelectedTaxonomyChanged="SelectedTaxonomyChanged" AutoPostBack="True"                    
                    meta:resourcekey="TaxonomiesAllResource1" />
                
                <asp:TreeView 
                    ID="TreeView1" 
                    EnableClientScript="true"
                    runat="server" 
                    ShowLines="True"
                    onclick="javascript:postBackByObject()"
                    ShowCheckBoxes="All" 
                    CssClass="treeview1" 
                    LeafNodeStyle-CssClass="tv_node" 
                    NodeStyle-CssClass="tv_node" meta:resourcekey="TreeView1Resource1"
                    OnTreeNodeCheckChanged="TreeView1_TreeNodeCheckChanged">
                    <NodeStyle CssClass="tv_node"></NodeStyle>
                    <LeafNodeStyle CssClass="tv_node"></LeafNodeStyle>
                </asp:TreeView>                
               
            </div>
        </div>
 
</asp:Content>
