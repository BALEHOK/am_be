<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScreensPanel.ascx.cs" Inherits="AssetSite.Controls.ScreensPanel" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
     <Services>
        <asp:ServiceReference Path="~/amDataService.asmx" />
    </Services>
</asp:ScriptManagerProxy>
<div class="active">
    <asp:Label ID="Label1" runat="server" meta:resourcekey="lblScreens"></asp:Label>
</div>
<div style="font-size:smaller;" id="screensPlaceholder">
    <div class="loader"></div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        AssetSite.amDataService.GetScreensByAssetTypeUid(<%= AssetType.UID %>, function(result){ 
            if (result) {
                var container = $('#screensPlaceholder');
                container.empty();
                var list = $('<ul>');
                $(result).each(function(){
                    var url = insertParam(document.location.search, 'ScreenId',  $(this).attr('Id'));                    
                    list.append('<li><a href="' + location.pathname + url + '">'+$(this).attr('Name')+'</a></li>');
                });
                container.append(list);
            }
        }, function(error){ 
             var container = $('#screensPlaceholder');
             container.empty();
             container.append(error);
        });
    });

    function insertParam(queryString, key, value)
    {
        key = escape(key); value = escape(value);
        var kvp = queryString.split('&');
        var i=kvp.length; var x; while(i--) 
        {
    	    x = kvp[i].split('=');

    	    if (x[0]==key)
    	    {
    		    x[1] = value;
    		    kvp[i] = x.join('=');
    		    break;
    	    }
        }
        if(i<0) {kvp[kvp.length] = [key,value].join('=');}
        return kvp.join('&'); 
    }
</script>