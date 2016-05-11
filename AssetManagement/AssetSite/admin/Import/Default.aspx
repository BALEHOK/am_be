<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AssetSite.admin.Import.Default"
    MasterPageFile="~/MasterPages/MasterPageDefault.master" Trace="false" Theme="Default" EnableViewState="true"
    meta:resourcekey="PageResource1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMiddleColumn" runat="server">
    <asp:ScriptManager runat="server" ScriptMode="Auto">
        <Services>
            <asp:ServiceReference Path="~/amDataService.asmx" />
        </Services>
    </asp:ScriptManager>
    <div id="main-container">
        <div class="wizard-header">
            <asp:Label runat="server" ID="pageTitle" meta:resourcekey="pageTitleResource1">Assets Synk</asp:Label>
        </div>
        <div class="panel">
            <div class="panelheader">
                <%= ImportingWizard.ActiveStep.Title %>
            </div>
            <amc:MessagePanel runat="server" ID="messagePanel" Style="margin: 5px 0 5px 10px"
                meta:resourcekey="messagePanelResource1" />
            <div class="panelcontent">
                <asp:Wizard ID="ImportingWizard" runat="server" Width="100%" CancelButtonText="<% $Resources:Global, CancelText %>"
                    StepNextButtonText="<% $Resources:Global, NextText %>" StartNextButtonText="<% $Resources:Global, NextText %>"
                    FinishCompleteButtonText="<% $Resources:Global, FinishText %>" DisplayCancelButton="True"
                    OnCancelButtonClick="ImportingWizard_Cancel" OnFinishButtonClick="ImportingWizard_Finish"
                    OnPreviousButtonClick="ImportingWizard_Previous"
                    OnNextButtonClick="ImportingWizard_Next" DisplaySideBar="False" ActiveStepIndex="0">
                    <WizardSteps>
                        <asp:WizardStep ID="WizardStep1" runat="server" Title="Step 1 &mdash; Asset Type for data importing"
                            meta:resourcekey="WizardStep1Resource1">
                            <table style="width: 100%;">
                                <tr>
                                    <td>
                                        <asp:DropDownList runat="server" SkinID="middle" AutoPostBack="true" OnSelectedIndexChanged="atList_Changed"
                                            ID="atList" meta:resourcekey="atListResource1">
                                        </asp:DropDownList>
                                        <asp:CompareValidator Operator="GreaterThan" ValueToCompare="0" ControlToValidate="atList"
                                            runat="server" Type="Integer" ErrorMessage="Please choose asset type to import assets"
                                            meta:resourcekey="CompareValidatorResource1">
                                        </asp:CompareValidator>
                                        <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                                            <ContentTemplate>
                                                <p>
                                                    <asp:HyperLink runat="server" ID="schemaLink" Visible="False" Text="Download schema file"
                                                        meta:resourcekey="schemaLinkResource1" /><br />
                                                    <asp:HyperLink runat="server" ID="xlsxSchemaLink" Visible="False" Text="Download xlsx schema file"
                                                        meta:resourcekey="schemaLinkResource2" /></p>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="atList" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStep2" runat="server" Title="Step 2 &mdash; DataSource"
                            meta:resourcekey="WizardStep2Resource1">
                            <table style="width: 100%;">
                                <tr>
                                    <td>
                                         <asp:FileUpload ID="fileUploadControl" CssClass="cFile" runat="server" meta:resourcekey="fileUploadControlResource1" />
                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStep3" runat="server" Title="Step 3 &mdash; DataSource settings"
                            meta:resourcekey="WizardStep3Resource1">
                            <p>
                                <asp:Literal ID="Literal1" runat="server" meta:resourcekey="Literal1Resource1">Please specify the sheets to import data from:</asp:Literal></p>
                            <asp:CheckBoxList runat="server" CssClass="sheets" ID="sheetsCheckboxes" meta:resourcekey="sheetsCheckboxesResource1">
                            </asp:CheckBoxList>
                            <asp:CustomValidator ID="sheetSelectedValidator" runat="server" ClientValidationFunction="checkSheet"
                                ErrorMessage="Please select at least one sheet" meta:resourcekey="sheetSelectedValidatorResource1"></asp:CustomValidator>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStep4" runat="server" Title="Finish &mdash; Fields binding"
                            meta:resourcekey="WizardStep4Resource1">
                            <asp:GridView ID="fieldsGrid" 
                                runat="server" 
                                CellSpacing="4" 
                                DataKeyNames="Name" 
                                EnableViewState="true"
                                OnRowDataBound="fieldsGrid_RowDataBound" 
                                AutoGenerateColumns="false" 
                                DataSource='<%# AssetTypeAttributes %>'
                                meta:resourcekey="fieldsGridResource1">
                                <Columns>
                                    <asp:TemplateField HeaderText="#" meta:resourcekey="TemplateFieldResource1" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="numerator" Text='<%# Container.DataItemIndex + 1 %>'
                                                meta:resourcekey="numeratorResource1"></asp:Label>
                                            <asp:HiddenField ID="ATA_ID" runat="server" Value='<%# Bind("ID") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="AssetType fields" meta:resourcekey="BoundFieldResource1" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Literal runat="server" ID="nameField" />
                                            <asp:Label runat="server" ID="lblRequired" Text="*" ForeColor="Red" Visible="False" />
                                            <asp:DropDownList runat="server" ID="dlRelatedAssetField" Visible="False" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="DataSource fields" meta:resourcekey="TemplateFieldResource2" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <span class="bindable bindable_<%# Container.DataItemIndex %>">
                                                <asp:DropDownList ID="datasourceField" runat="server" meta:resourcekey="datasourceFieldResource1">
                                                </asp:DropDownList>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Default value" meta:resourcekey="TemplateFieldResource3" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Panel runat="server" ID="ctrlWrapper" meta:resourcekey="ctrlWrapperResource1">
                                                <span id="<%# Container.DataItemIndex %>">
                                                    <span class="expandable" style="display: inline">
                                                        <asp:PlaceHolder runat="server" ID="defControl"></asp:PlaceHolder>
                                                    </span>
                                                    <asp:Literal runat="server" ID="ltAction"></asp:Literal>
                                                </span>
                                            </asp:Panel>
                                        </ItemTemplate>
                                        <ItemStyle Width="300px"></ItemStyle>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:CheckBox ID="useBoth" CssClass="useboth" runat="server" Checked="true" Text="Use default values only if binding data is missing"
                                meta:resourcekey="useBothResource1" />
                            <p>
                                <asp:Literal runat="server" ID="lblPressFinish" meta:resourcekey="lblPressFinishResource1">Click Finish to create the import task.</asp:Literal></p>
                        </asp:WizardStep>
                    </WizardSteps>
                </asp:Wizard>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            initControls();
            $('.datasource').click(function () {
                $('.wrapper').hide();
                initControls();
            });

            $('.expandable').hide();
        });

        function toggleDefault(id) {
            $('#' + id + ' .expandable').show();
            var cmd_link = $('#' + id + ' .cmd');
            cmd_link.removeAttr('onclick');
            cmd_link.unbind('click');
            window.setTimeout("$('#" + id + " .cmd').bind('click', function () { AssignValue("+id+"); })", 10);
            cmd_link.text('<%=this.GetLocalResourceObject("linkSave")%>');
        }

        function AssignValue(id) {

            var valContainer = $('#' + id + ' .expandable :input');
            var dispVal, sentVal;

            if (valContainer.size() > 0) {
                if (valContainer.is('select')) {
                    if (valContainer.attr('ismultilpleassetslist') == 'true') {
                        var i = 0;
                        dispVal = '';
                        sentVal = '';
                        for (i = 0; i < valContainer[0].length; i++) {
                            dispVal += valContainer[0][i].text + ',';
                            sentVal += valContainer[0][i].value + ',';
                        }

                        if (dispVal.length > 0) {
                            dispVal = dispVal.substr(0, dispVal.length - 1);
                        }
                        if (sentVal.length > 0) {
                            sentVal = sentVal.substr(0, sentVal.length - 1);
                        }
                    } else {
                        sentVal = valContainer.val();
                        dispVal = $(':selected', valContainer).text();
                    }
                }
                else {
                    if($('#' + id + ' .expandable :input').attr("type")=="checkbox")
                    {
                        sentVal = dispVal = $('#' + id + ' .expandable :input').is(':checked');
                    }
                    else
                    {
                        sentVal = dispVal = valContainer.val();
                    }                   
                }
            }
            else {
                sentVal = dispVal = $('#' + id + ' .expandable').text();
            }

            if (sentVal.toString() == '') {
                return;
            }

            $.ajax({
                type: 'POST',
                url: '/admin/Import/Default.aspx/AssignValue',
                data: '{index: ' + id + ' , value: \'' + sentVal + '\'}',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: AssignValueCallback,
                error: function (result) {
                    alert('Unable to save the value, please try again or contact a site administrator');
                },
                ContainerId: id,
                dispVal: dispVal
            });
        }

        function AssignValueCallback(result) {
            if (!result.d) {
                $('#' + this.ContainerId + ' .expandable').text();
                alert("<%=this.GetLocalResourceObject("InputDataError")%>");
            }
            else {
                $('#' + this.ContainerId + ' .expandable').before(
                '<span class="label">' + this.dispVal + '</span>');
                var cmd_link = $('#' + this.ContainerId + ' .cmd');
                cmd_link.text('<%=this.GetLocalResourceObject("linkRemove")%>');
                cmd_link.unbind('click');
                window.setTimeout("$('#" + this.ContainerId + " .cmd').bind('click', function () { RemoveValue("+this.ContainerId+"); })", 10);
                $('#' + this.ContainerId + ' .expandable').hide();
            }
        }

        function callback(result) {
        }

        function RemoveValue(id) {
            var cmd_link = $('#' + id + ' .cmd');
            cmd_link.text('<%=this.GetLocalResourceObject("linkAdd")%>');
            cmd_link.removeAttr('onclick');
            cmd_link.unbind('click');
            window.setTimeout("$('#" + id + " .cmd').bind('click', function () { toggleDefault("+id+");})", 10);
            $('#' + id + ' .label').remove();
            var bnd_ctrl = $('.bindable_' + id + ' :input');
            bnd_ctrl.removeAttr('disabled');

            $.ajax({
                type: 'POST',
                url: '/admin/Import/Default.aspx/RemoveValue',
                data: '{index: ' + id + '}',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: callback,
                error: function (result) {
                    alert('Unable to remove value, please try again or contact the site administrator');
                }
            });
        }

        function initControls() {
            var checkedSource = $('.datasource :radio:checked').val();
            if (checkedSource != 'AD') {
                $('#wFile').show();
            }
            else if (checkedSource == 'AD') {
                $('#wAD').show();
            }
        }

        function checkSheet(sender, args) {
            if ($('.sheets :checkbox:checked').size() > 0) {
                args.IsValid = true;
                return args;
            }
            args.IsValid = false;
            return args;
        }
    </script>
</asp:Content>
