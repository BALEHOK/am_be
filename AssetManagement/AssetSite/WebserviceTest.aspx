<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPageBase.Master" AutoEventWireup="true" CodeBehind="WebserviceTest.aspx.cs" Inherits="AssetSite.WebserviceTest" %>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMainContent" runat="server">
    <table>
        <tr>
            <td>Field one:</td>
            <td>
                <asp:TextBox runat="server" ID="txtFieldOne" />
            </td>
        </tr>
        <tr>
            <td>Field two:</td>
            <td>
                <asp:TextBox runat="server" ID="txtFieldTwo" />
            </td>
        </tr>
        <tr>
            <td>Webservice result:</td>
            <td>
                <asp:TextBox runat="server" ID="txtResult" Enabled="false" /> 
                <input type="button" value="Update" id="btnUpdate" />
            </td>
        </tr>
    </table>
    <script>
        $(function() {
            $('#btnUpdate').click(function () {

                var JSONObject = new Array();

                // iterate over all controls or some specific ones: use jQuery filter conditions
                // for ex.: add CssClass="autocalculated" for each autocalculated attribute and use $('input.autocalculated').each(...) 
                $('#ctl00_PlaceHolderMainContent_txtFieldOne, #ctl00_PlaceHolderMainContent_txtFieldTwo').each(function (index, item) {
                    var obj = new Object();
                    obj.Name = "Attribute " + index;
                    obj.Value = $(item).val();
                    console.log(obj);
                    JSONObject.push(obj);
                });

                $.ajax({
                    type: 'POST',
                    url: '/amDataService.asmx/UpdateCalculatedField',
                    data: JSON.stringify({ attributes: JSONObject }),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (result) {
                        // apply the result (result.d) to the target textbox
                        $('#ctl00_PlaceHolderMainContent_txtResult').val(result.d);
                    },
                    error: function (result) {
                    }
                });
            });
        });
    </script>
</asp:Content>
