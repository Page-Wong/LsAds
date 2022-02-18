$(function () {
    $("#RegistEquipmentoModal").modal("show");
    $("#inputEditEquipmentName").focus();
    InitEquipmentModeListOption();
    $("#EditEquipmentInfoSave").click(function () { SaveEquipmentInfo(); });
});


//设备型号列表项初始化
function InitEquipmentModeListOption() {
    $.ajax({
        type: "Get",
        url: "/EquipmentRegist/GetEquipmentModeList?_t=" + new Date().getTime(),
        success: function (data) {
            var option = "";
            $.each(data.equipmentModeList, function (i, equipmentMode) {
                option += "<option value='" + equipmentMode.id + "' >" + equipmentMode.model + "----(" + equipmentMode.manufacturer + ")" + "</option>";
            })
            $("#inputEditEquipmentMode").html(option);
        }
    })  
}

function SaveEquipmentInfo() {
    if ($('#registeid').val() == null || $('#registeid').val() == "") {
        layer.alert("你提交的数据有误，请尝试重新操作！");
        return;
    }
    
    if ($('#inputEditEquipmentName').val() == null || $('#inputEditEquipmentName').val() == "") {
        layer.alert("设备名称不能为空！");
        $('#inputEditEquipmentName').focus();
        return;
    }

    if ($('#inputEditEquipmentMode').val() == null || $('#inputEditEquipmentMode').val() == "") {
        layer.alert("设备型号不能为空！");
        $('#inputEditEquipmentMode').focus();
        return;
    }

    var formData = new FormData();
    formData.append('registeid', $('#registeid').val());
    formData.append('equipmentName', $('#inputEditEquipmentName').val());
    formData.append('equipmentModeId', $('#inputEditEquipmentMode').val());

    $("#EditEquipmentInfoSave").attr("disabled", "disabled");
    $.ajax({
        type: "Post",
        url: "/EquipmentRegist/Registered",
        data: formData,
        processData: false,
        contentType: false,
        cache: false,
        success: function (data) {
            $("#EditEquipmentInfoSave").attr("disabled", false);

            if (data.result == "Success") {
                $("#RegistEquipmentoModal").modal("hide");
                layer.alert("恭喜您，设备注册成功！");
                window.location.href = '/EquipmentOwner';
            }
            else {
                layer.alert("操作失败！" + data.message);
            }
        }
    });
       
}
