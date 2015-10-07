<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasksPanel.ascx.cs" Inherits="AssetSite.Controls.TasksPanel" %>
<asp:ScriptManagerProxy runat="server">
     <Services>
        <asp:ServiceReference Path="~/amDataService.asmx" />
    </Services>
</asp:ScriptManagerProxy>
<div class="active">
    <asp:Label ID="Label1" runat="server" meta:resourcekey="lblTasks"></asp:Label>
</div>
<div style="font-size:smaller;" id="tasksPlaceholder">
    <div class="loader"></div>
</div>
<script type="text/javascript">
    var assetUid = <%= this.AssetUID %>;
    $(document).ready(function () {
        $('#taskDialog').dialog({
                resizable: false,
                draggable: false,
                width: 550,
                height: 350,             
                modal: true,
                autoOpen: false,
                buttons: { "Ok": function() { $(this).dialog("close"); } }
            });
        AssetSite.amDataService.GetTasksByAssetTypeId(<%= AssetType.ID %>, function(result){ 
            if (result) {
                var container = $('#tasksPlaceholder');
                container.empty();
                var list = $('<ul>');
                $(result).each(function(){                    
                    if ($(this).attr('ActionOnComplete') == 0) {
                        list.append('<li><a href="javascript:void(0);" onclick="ExecuteTask('+$(this).attr('Id')+', false);">'
                            +$(this).attr('Name')+'</a></li>');  
                    } else {
                        list.append('<li><a href="javascript:void(0);" onclick="ExecuteTask('+$(this).attr('Id')+', true);">'
                            +$(this).attr('Name')+'</a></li>');                            
                    }
                });
                container.append(list);
                $('ul').not(':has(li)').remove();
            }
        }, function(error){ 
             var container = $('#tasksPlaceholder');
             container.empty();
             container.append(error);
        });
    });

    function ExecuteTask(taskId, showDialog) {
        if (showDialog) {
            var dialog = $('#taskDialog');
            dialog.append('<div class="loader"></div>');
            dialog.dialog('open');
        }
        AssetSite.amDataService.ExecuteTask(taskId, assetUid, function(result){             
            if (result.NavigationResult){
                location.href = result.NavigationResult;                
            } else {                
                dialog.empty();                
                if ($(result.Messages).size() > 0) {                    
                    $(result.Messages).each(function (index, item) {
                        dialog.append('<p>' + item + '</p>');
                    });                    
                } else {
                    dialog.append('<p class="status ok">Status: Sussess</p>');
                }                
            }
        });
    }
</script>
<div id="taskDialog" title="Task execution..." class="dialog hidden tasks">
</div>