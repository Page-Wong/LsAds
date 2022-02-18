function EquipmentmodelContentMethod(equipmentmodelDto) {


    var  equipmentmodeIntroduction = "<div class='thumbnail'>";
    equipmentmodeIntroduction += " <img src='/EquipmentModel/GetThumbnail?id=" + equipmentmodelDto.id + "'/>";
    equipmentmodeIntroduction += " <div class='caption'>";
    equipmentmodeIntroduction += " <div> 型号:" + (equipmentmodelDto.model == null ? "" : equipmentmodelDto.model) + "</div>";
    equipmentmodeIntroduction += " <div> 品牌: " + (equipmentmodelDto.manufacturer == null ? "" : equipmentmodelDto.manufacturer) + "</div>";
    equipmentmodeIntroduction += " <div> 屏幕: " + (equipmentmodelDto.screenSize == null ? "" : equipmentmodelDto.screenSize + "寸") + "&nbsp;" + (equipmentmodelDto.screenMaterial == null ? "" : equipmentmodelDto.screenMaterial)+" &nbsp;";
    equipmentmodeIntroduction += (equipmentmodelDto.resolutionRatio == null ? "" : equipmentmodelDto.resolutionRatio) + "&nbsp;" + (equipmentmodelDto.screenRatio == null ? "" : equipmentmodelDto.screenRatio) + "</div>";
    equipmentmodeIntroduction += " <div>宽*高: " + (equipmentmodelDto.screenWidth == null ? "" : equipmentmodelDto.screenWidth + "cm") + "*" + (equipmentmodelDto.screenHeight == null ? "" : equipmentmodelDto.screenHeight + "cm") + "</div>";
    equipmentmodeIntroduction += " <div>声音: " + (equipmentmodelDto.voicedName == null ? "" : equipmentmodelDto.voicedName) + " &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;副屏数:" + (equipmentmodelDto.sideScreensNumber == null ? "" : equipmentmodelDto.sideScreensNumber) + "</div>";
    equipmentmodeIntroduction += " <div>适用: ";
    var applyTos = equipmentmodelDto.applyTo == null ? "" : equipmentmodelDto.applyTo.split(",");
    if (applyTos != "") {
        $.each(applyTos, function (i, applyTo) {
            equipmentmodeIntroduction += "<small class='label label-info'>" + applyTo + " </small>";
        })
    }
    equipmentmodeIntroduction += " </div>";
    equipmentmodeIntroduction += " <div> 简介：<small><br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + (equipmentmodelDto.describe == null ? "" : equipmentmodelDto.describe) + "</small></div>";
    equipmentmodeIntroduction += "</div>";
    equipmentmodeIntroduction += "</div>";
    return equipmentmodeIntroduction;
  
}
function getequipmentmodelPopoverInfo(objId, equipmentmodelDto) {
if (objId == null || equipmentmodelDto == null) return;
    //top, bottom, left or right
    var obj_p = $("#" + objId);
    if (obj_p == null) return;
    // data-toggle='popover'
    obj_p.attr('data-toggle', 'popover');
    var content;

    obj_p.popover({
        //  trigger: 'manual',
        placement: 'bottom',
        title: '设备型号信息',
        html: 'true',
        content: EquipmentmodelContentMethod(equipmentmodelDto),

    }).on("mouseenter", function () {
        var _this = this;
        $(this).popover("show");
        $(this).siblings(".popover").on("mouseleave", function () {
            $(_this).popover('hide');
        });
    }).on("mouseleave", function () {
        var _this = this;
        setTimeout(function () {
            if (!$(".popover:hover").length) {
                $(_this).popover("hide")
            }
        }, 100);
    });

}