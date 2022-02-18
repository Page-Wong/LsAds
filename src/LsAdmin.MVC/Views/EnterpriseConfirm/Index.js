var pageSize = 999999;

$(document).ready(function () {
    $('#rootwizard').bootstrapWizard({
        onTabShow: function (tab) { }
    });
});

$(function () {
    $("#btnSave").click(function () { save(); });
    $("#Period").datepicker({
        autoclose: true
    })
});


//保存
function save() {

    var formData = new FormData();
    formData.append('Id', $('#Id').val());
    formData.append('Name', $('#Name').val());
    formData.append('RegisteredNumber', $('#RegisteredNumber').val());
    formData.append('LicenseAddress', $('#LicenseAddress').val());
    formData.append('Period', $('#Period').val());
    formData.append('Location', $('#Location').val());
    formData.append('Phone', $('#Phone').val());
    formData.append('DuplicateLicenseScanFile', $('#DuplicateLicenseScan')[0].files[0]);
    formData.append('DuplicateLicenseSealFile', $('#DuplicateLicenseSeal')[0].files[0]);
    formData.append('OrganizationCode', $('#OrganizationCode').val());
    formData.append('ManagementScope', $('#ManagementScope').val());
    formData.append('RegisteredCapital', $('#RegisteredCapital').val());
    formData.append('Fax', $('#Fax').val());

    $.ajax({
        type: "Post",
        url: "/EnterpriseConfirm/Add",
        data: formData,
        processData: false,
        contentType: false,
        cache: false,
        success: function (data) {
            if (data.result == "Success") {
                layer.alert("您已成功提交实名认证申请，请耐心等待审核结果，谢谢！", function () {
                    window.location.href = "/Home/Index"; 
                });              
            }
            else {
                layer.tips(data.message, "#btnSave");
            };
        }
    });
}

