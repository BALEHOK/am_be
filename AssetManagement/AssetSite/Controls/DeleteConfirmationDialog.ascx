<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeleteConfirmationDialog.ascx.cs" Inherits="AssetSite.Controls.DeleteConfirmationDialog" %>
    <div class="dialog hidden confirmation" title="<asp:Label runat="server" Text="Confirmation"  meta:resourcekey="DialogTitle" />">
        <p>
            <asp:Label runat="server" Text="Please confirm the deletion."  meta:resourcekey="LabelConfirm" />
        </p>
    </div>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.confirmation').dialog({
                resizable: false,
                draggable: false,
                width: 450,
                height: 250,             
                modal: true,
                autoOpen: false                
            });
        });
    </script>