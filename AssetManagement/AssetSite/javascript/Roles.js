var OnlyPersonRoleId = 3;
function OnRolesComboBoxIndexChanged(sender) {

    if ($(sender).length > 0) {
        if ($(sender).val() == OnlyPersonRoleId) {
            $(".Password").hide();
            $(".Email").hide();
        } else {
            $(".Email").show();
            $(".Password").show();            
        }
    }
}