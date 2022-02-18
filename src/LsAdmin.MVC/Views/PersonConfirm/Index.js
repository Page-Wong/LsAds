var pageSize = 999999;
$(document).ready(function () {
    $('#rootwizard').bootstrapWizard({
        onTabShow: function (tab) { }
    }); 
});
$(function () {
    $("#btnSave").click(function () { save(); });
    $("#IdDuration").datepicker({
        autoclose: true
    })
});



//保存
function save() {
  
    var formData = new FormData();
    formData.append('Id', $('#Id').val());
    formData.append('Name', $('#Name').val());
    formData.append('UserName', $('#UserName').val());
    formData.append('IdNumber', $('#IdNumber').val());
    formData.append('IdType', $('#IdType').val());
    formData.append('UploadIdFrontFile', $('#UploadIdFront')[0].files[0]);
    formData.append('UploadIdBackFile', $('#UploadIdBack')[0].files[0]);
    formData.append('IdDuration', $('#IdDuration').val());
    formData.append('Location', $('#Location').val());
   
    $.ajax({
        type: "Post",
        url: "/PersonConfirm/Add",
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
            }
        }
    });
}



