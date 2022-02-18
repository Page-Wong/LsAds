var selectPage = 1;
var pageSize = 15;
var getStatus = "all";//getStatus :all 表示加载所有状态记录
var thisPageUrl = window.location.href;
var equipmentRepairUrl = thisPageUrl.replace('EquipmentOwner', 'EquipmentRepair');

$(function () {
    $("#checkAllOwnewEquipment").click(function () { checkAlleEquipments(this) });
    $("#btnStatusAll").click(function () { getStatus = "all"; loadOwnerEquipmentsByStatus(1, pageSize, getStatus); });
    $("#btnSTATUSUNINUSE").click(function () { getStatus = "0"; loadOwnerEquipmentsByStatus(1, pageSize, getStatus); });
    $("#btnSTATUSINUSE").click(function () { getStatus = "1"; loadOwnerEquipmentsByStatus(1, pageSize, getStatus); });
    $("#btnSTATUSREPAIRING").click(function () { getStatus = "2"; loadOwnerEquipmentsByStatus(1, pageSize, getStatus); });
    $("#btnSTATUSSCRAP").click(function () { getStatus = "3"; loadOwnerEquipmentsByStatus(1, pageSize, getStatus); });
    $("#LaunchdSave").click(function () { saveLaunchd(); });
    $("#EditEquipmentInfoSave").click(function () { SaveEquipmentInfo(); });
    loadOwnerEquipmentsByStatus(selectPage, pageSize, "all");
    initMonitorModal();
    
});

//加载列表数据
function loadOwnerEquipmentsByStatus(startPage, pageSize, status) {
    $("#tableBody").html("");
    $("#myEquipmentList").html("");

    if (status != "all") {
        urlString = "/EquipmentOwner/GetOwnerEquipmentsByStatus?status=" + status + "&startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime();
    }else{
        urlString = "/EquipmentOwner/GetOwnerEquipmentsWithStatusRowCount?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime();
    }

    $("#checkAllModel").prop("checked", false);
    $.ajax({
        type: "GET",
        url: urlString,
        cache: true,
        async: false,
        success: function (data) {
            $("#SpanStatusAll").html(data.allrowCount == 0 ? "" : data.allrowCount);
            $("#SpanSTATUSUNINUSE").html(data.statusRowCount["0"] == null ? "" : data.statusRowCount["0"]);
            $("#SpanSTATUSINUSE").html(data.statusRowCount["1"] == null ? "" : data.statusRowCount["1"]);
            $("#SpanSTATUSREPAIRING").html(data.statusRowCount["2"] == null ? "" : data.statusRowCount["2"]);
            $("#SpanSTATUSSCRAP").html(data.statusRowCount["3"] == null ? "" : data.statusRowCount["3"]);

            showMyEquipmentList(data);
            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页             
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        selectPage =newPage;
                        loadOwnerEquipmentsByStatus(newPage, pageSize, getStatus);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
            $("ul > li").click(function () {
                $("ul > li").removeAttr("style")
                $(this).attr("style", "background-color:#beebff");
            });

            refreshAcvtieStatus();
        }
    })
}
function showMyEquipmentTable(data) {
    $.each(data.rows, function (i, item) {
        var tr = "<tr valign='middle'>";
        tr += "<td  >" + (i + 1) + "</td>";
        tr += "<td align='center' valign='middle'><input type='checkbox' class='checkboxs' value='" + item.id + "'/></td>";
        tr += "<td align='center' valign='middle'><div ><img style='height:100px;' class='img-thumbnail' src='/EquipmentModel/GetThumbnail?id=" + item.equipmentModelId + "'/></div>" +
            //  "<div class='caption'><p>厂家：" + (item.equipmentModel.manufacturer == null ? "" : item.equipmentModel.manufacturer) + "&nbsp&nbsp 型号：" + (item.equipmentModel.model == null ? "" : item.equipmentModel.model)  + " </p></td>";
            "</td>";
        tr += "<td><p>名称：" + (item.name == null ? "" : item.name) + "</p><p>状态：" + (item.status == null ? "" : item.status) + "</p><p>设备号：" + (item.deviceId == null ? "" : item.deviceId) +
            "</p><p>启用时间：" + (item.startDate == null ? "" : item.startDate) + "</p><p>停用时间：" + (item.discontinuationDate == null ? "" : item.discontinuationDate) + "</p></td>";

        tr += "<td><p>型号：" + (item.equipmentModelDto.model == null ? "" : item.equipmentModelDto.model) + "</p><p>厂家：" + + (item.equipmentModelDto.manufacturer == null ? "" : item.equipmentModelDto.manufacturer) +
            "</p><p>尺寸：" + (item.equipmentModelDto.screenSize == null ? "0" : item.equipmentModelDto.screenSize) + "英寸&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp 宽：" +
            (item.equipmentModelDto.screenWidth == null ? "0" : item.equipmentModelDto.screenWidth) + "(cm) &nbsp&nbsp&nbsp 高：" +
            (item.equipmentModelDto.screenHeight == null ? "0" : item.equipmentModelDto.screenHeight) + "(cm)" +
            "<p>屏幕素材：" + (item.equipmentModelDto.screenMaterial == null ? "0" : item.equipmentModelDto.screenMaterial) + " &nbsp&nbsp声音：" + (item.equipmentModelDto.voicedName == null ? "" : item.equipmentModelDto.voicedName) +
            "&nbsp&nbsp副屏：" + (item.equipmentModelDto.sideScreensNumber == null ? "0" : item.equipmentModelDto.sideScreensNumber) + "</p>" +
            "<p>适用范围：" + (item.equipmentModelDto.applyTo == null ? "" : item.equipmentModelDto.applyTo) + "</P>" +
            "</td>";

        tr += "<td><p>场所名称：" + (item.placeDto == null || item.placeDto.name == null ? "" : item.placeDto.name) + "</p>" +
            "<p>地址：" + (item.placeDto == null || item.placeDto.address == null ? "" : item.placeDto.address) + "</p>" +
            "<p>负责人：" + (item.placeDto == null || item.placeDto.contact == null ? "" : item.placeDto.contact) + "&nbsp&nbsp&nbsp联系电话：" + (item.placeDto == null || item.placeDto.phone == null ? "" : item.placeDto.phone) + "</P>" +
            "<p>场所类型：" + (item.placeDto == null || item.placeDto.typeName == null ? "" : item.placeDto.typeName) + "</p>" +
            "<p>场所介绍：" + (item.placeDto == null || item.placeDto.introduction == null ? "" : item.placeDto.introduction) + "</p>" +
            "</td>";

        tr += "<td>" +
            "<button class='btn btn-info btn-xs'   href= 'javascript:;' onclick= 'edit(\"" + item.id + "\")' > <i class='fa fa-edit'></i> 编辑 </button>" +
            "<button class='btn btn-info btn-xs'   href= 'javascript:;' onclick= 'edit(\"" + item.id + "\")' > <i class='fa fa-edit'></i> 投放 </button>" +
            "<button class='btn btn-info btn-xs'   href= 'javascript:;' onclick= 'edit(\"" + item.id + "\")' > <i class='fa fa-edit'></i> 更换设备 </button>" +
            "<button class='btn btn-info btn-xs'   href= 'javascript:;' onclick= 'edit(\"" + item.id + "\")' > <i class='fa fa-edit'></i> 维修登记 </button>" +
            "<button class='btn btn-danger btn-xs' href='javascript:;'  onclick='deleteSingle(\"" + item.id + "\")'><i class='fa fa-trash-o'></i> 报废 </button>" +
            "</td > "
        tr += "</tr>";
        $("#tableBody").append(tr);
    })
}

function showMyEquipmentList(data){   
    $.each(data.rows, function (i, item) {
        var liStr = "";
        liStr += "<li class='item'>";
        liStr += "<input hidden name='equipmentId' value='"+item.id+"'>";
        liStr += " <div class='col-md-10'>";
        liStr += " <div class='product-img'>";
        liStr += " <img src='/EquipmentModel/GetThumbnail?id=" + item.equipmentModelId + "'/>";
        liStr += "</div>";
        liStr += " <div class='product-info'>";
        liStr += "<div class='product-description text-light-blue'>名称：" + (item.name == null ? "" : item.name);
        liStr += "   &nbsp;&nbsp;" + (item.startDate == null ? "" : "启用时间：" + item.startDate);   
        liStr += "   &nbsp;" + (item.discontinuationDate == null ? "" : "停用时间" + item.discontinuationDate); 
        liStr += " &nbsp;设备号：" + (item.deviceId == null ? "" : item.deviceId) + "&nbsp;&nbsp;";

        switch (item.status) {
            case 0:
                liStr += "<small class='label label-warning'>" + "<i class='fa fa-hourglass-2'></i>" + (item.statusString == null ? "" : item.statusString) + "</small>"
                break;
            case 1:
                liStr += "<small class='label label-primary'>" + "<i class='fa fa-send'></i>" + (item.statusString == null ? "" : item.statusString) + "</small>"
                break;
            case 2:
                liStr += "<small class='label label-success'>" + "<i class='fa fa-wrench'></i>" + (item.statusString == null ? "" : item.statusString) + "</small>"
                break;
            case 3:
                liStr += "<small class='label label-danger'>" + "<i class='fa fa-trash'></i>" + (item.statusString == null ? "" : item.statusString) + "</small>"
                break;
            default:
                liStr += "<small class='label label-danger'>" + "<i class='fa fa-warning'></i>" + (item.statusString == null ? "" : item.statusString) + "</small>"
        }
        liStr += "</div > ";

        liStr += "<div>";
        liStr += "   型号:<a   id='em_" + item.id + "'class='product-title btn-link'>"+(item.equipmentModelDto.manufacturer == null ? "" : item.equipmentModelDto.manufacturer);
        liStr += "  &nbsp;&nbsp;&nbsp; " + (item.equipmentModelDto.model == null ? "" : item.equipmentModelDto.model) + "</a>";
        liStr += "  &nbsp;&nbsp;" + (item.equipmentModelDto.screenSize == null ? "0" : item.equipmentModelDto.screenSize + "寸");
        liStr += "  &nbsp;&nbsp; "+(item.equipmentModelDto.screenMaterial == null ? "0" : item.equipmentModelDto.screenMaterial);
        liStr += "  &nbsp;&nbsp;" + (item.equipmentModelDto.resolutionRatio == null ? "" : item.equipmentModelDto.resolutionRatio) + "&nbsp; " + (item.equipmentModelDto.screenRatio == null ? "" : item.equipmentModelDto.screenRatio);
        liStr += "  &nbsp;&nbsp;" + (item.equipmentModelDto.voicedName == null ? "" : item.equipmentModelDto.voicedName);
        liStr += "  &nbsp;&nbsp; 宽 * 高:" + (item.equipmentModelDto.screenWidth == null ? "" : item.equipmentModelDto.screenWidth + "cm") + "*" + (item.equipmentModelDto.screenHeight == null ? "" : item.equipmentModelDto.screenHeight + "cm");
        liStr += "  &nbsp;&nbsp; 副屏数:" + (item.equipmentModelDto.sideScreensNumber == null ? "" : item.equipmentModelDto.sideScreensNumber);
        liStr += "</div>";

        liStr += " <div>";
        liStr += "   场所:<a id='p_" + item.id + "'class='product-title btn-link'>" + (item.placeDto == null || item.placeDto.name == null ? "" : item.placeDto.name);     
        liStr += (item.placeDto == null || item.placeDto.address == null ? "" : "(" + item.placeDto.address + ")") +"</a>";
        liStr +=" &nbsp;&nbsp; 负责人:"+ (item.placeDto == null || item.placeDto.contact == null ? "" : item.placeDto.contact);
        liStr += " &nbsp;&nbsp; 联系电话:" + (item.placeDto == null || item.placeDto.phone == null ? "" : item.placeDto.phone);
        liStr += " &nbsp;&nbsp; 类型: "+ (item.placeDto == null || item.placeDto.typeName == null ? "" : item.placeDto.typeName);
        liStr += "  </div>";

        liStr += "</div>";
        liStr += "</div>";
        
        liStr += " <div class='col-md-2'>";
        liStr += "<div class='tools'>";
        
        switch (item.status) {
            case 0://待投放
                liStr += " <button class='btn btn-info btn-xs' href='javascript:;' onclick='showEditEquipmentInfoModal(\"" + item.id + "\")'> <i class='fa fa-edit'></i>编辑</button>";
                liStr += " <button class='btn btn-primary btn-xs' href='javascript:;'  onclick= 'showLaunchModel(\"" + item.id + "\")'> <i class='fa fa-send'></i>投放</button>";
                liStr += " <button class='btn btn-danger btn-xs' href='javascript:;' onclick= 'UpdateEquipmentStats(\"" + item.id + "\",0,3)'> <i class='fa fa-trash'></i>报废</button>";
                break;
            case 1://已投放
                liStr += " <button class='btn btn-warning btn-xs' href='javascript:;' onclick= 'UpdateEquipmentStats(\"" + item.id + "\",1,0)'> <i class='fa fa-hourglass-2'></i>回收</button>";
                liStr += " <button class='btn btn-danger btn-xs' href='javascript:;' onclick= 'UpdateEquipmentStats(\"" + item.id + "\",1,3)'> <i class='fa fa-trash'></i>报废</button>";
  
                break;
            case 2://维修
                liStr += "<a class='btn btn-success btn-xs' href='" + equipmentRepairUrl+"'> <i class='fa fa-wrench'></i>维修</a>";
                //liStr += " <button class='btn btn-danger btn-xs' href='javascript:;' onclick= 'UpdateEquipmentStats(\"" + item.id + "\",2,3)'> <i class='fa fa-trash'></i>报废</button>";
                break;
            case 3://报废
                liStr += " <button class='btn btn-danger btn-xs' href='javascript:;' onclick= 'UpdateEquipmentStats(\"" + item.id + "\",3,0)'> <i class='fa fa-remove'></i>取消报废</button>";
                break;
          /*
            default:
                liStr += "<button class='btn btn-danger btn-xs' href='javascript:;' onclick=''> <i class='fa fa-remove'></i>不通过</button>";
                liStr += " <button class='btn btn-warning btn-xs' href='javascript:;' onclick=''> <i class='fa fa-reply'></i>报废</button>";*/
        }
        liStr += " </div>";
        liStr += " </div>";
        liStr += " </li>";
         
        $("#myEquipmentList").append(liStr);
        if (item.placeDto != null){
            getPlacePopoverInfo('p_' + item.id, item.placeDto);
        }

        getequipmentmodelPopoverInfo('em_' + item.id, item.equipmentModelDto);
    })

}

//显示设备修改model
function showEditEquipmentInfoModal(id) {
    $("#EditEquipmentInfoSave").attr("disabled", false);
    $.ajax({
        type: "Get",
        url: "/EquipmentOwner/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            InitEquipmentModeListOption(data.equipmentModeList)

            $("#EditEquipmentInfoEquipmentId").val(data.dto.id);
            $("#ConfirmedSave").attr("disabled", false);
            $("#inputEditEquipmentName").val(data.dto.name);
            

            $("#inputEditEquipmentMode").val(data.dto.equipmentModelId).trigger("change"); 
            $("#inputEditEquipmentMode").select2("val", data.dto.equipmentModelId == null ? "" : data.dto.equipmentModelId);

            $("#inputEditEquipmentName").focus();
            $("#EditEquipmentInfoModal").modal("show");
        }
    }) 
}


//显示投放model
function showLaunchModel(id) {
    $("#LaunchdSave").attr("disabled", false);
    $.ajax({
        type: "Get",
        url: "/EquipmentOwner/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            InitplaceListOption(data.placelist);
            $("#equipmentId").val(data.dto.id);
            $("#ConfirmedSave").attr("disabled", false);
            $("#inputPlace").select2("val", data.dto.placeId == null ? "" : data.dto.placeId);
            $("#inputPlace").val(data.dto.placeId).trigger("change"); 
            $("#inputPlace").focus();
            $("#LaunchPlaceModal").modal("show");
        }
    }) 
};

//场所列表项初始化
function InitplaceListOption(placelist) {
    var option = "";
    $.each(placelist, function (i, place) {
        option += "<option value='" + place.id + "' >" + place.name + "----(" + place.address + ")" + "</option>";
    })
    $("#inputPlace").html(option);
}

//设备型号列表项初始化
function InitEquipmentModeListOption(equipmentModeList) {
    var option = "";
    $.each(equipmentModeList, function (i, equipmentMode) {
        option += "<option value='" + equipmentMode.id + "' >" + equipmentMode.model + "----(" + equipmentMode.manufacturer + ")" + "</option>";
    })
    $("#inputEditEquipmentMode").html(option);
}

//全选
function checkAlleEquipments(obj) {
    $(".checkboxs").each(function () {
        if (obj.checked == true) {
            $(this).prop("checked", true)

        }
        if (obj.checked == false) {
            $(this).prop("checked", false)
        }
    });
};

//保存-投放场所信息
function saveLaunchd() {
    $("#LaunchdSave").attr("disabled", false);

    if ($('#inputPlace').val() == null || $('#inputPlace').val() == "" || $('#inputPlace').val().toString() == "--请选择投放场所--") {
        $('#inputPlace').focus();
        return;
    }

    var formData = new FormData();
    formData.append('equipmentId', $('#equipmentId').val());
    formData.append('placeId', $('#inputPlace').val());
    
    $("#LaunchdSave").attr("disabled", "disabled");
    $.ajax({
        type: "Post",
        url: "/EquipmentOwner/SavePlace",
        data: formData,
        processData: false,
        contentType: false,
        cache: false,
        success: function (data) {

            $("#LaunchdSave").attr("disabled", false);

            if (data.result == "Success") {
                loadOwnerEquipmentsByStatus(selectPage, pageSize, getStatus)
                $("#LaunchPlaceModal").modal("hide");
                layer.alert("数据保存成功！");
            }
            else {
                layer.alert("操作失败！" + data.message);
            }
        }
    });
}

function UpdateEquipmentStats(equipmentId, oldStatus, newStatus) {
    var showmessage = "";
    if (newStatus == 3) {
        showmessage = "确定报废该设备？";
    } else if (newStatus == 0 && oldStatus == 3) {
        showmessage = "确定取消报废该设备？";
    } else if (newStatus == 0){
        showmessage = "确定回收该设备？";
    } else {
        showmessage = "确定变更设备状态？";
    }

    layer.confirm(showmessage, {
        btn: ['确定', '取消']//按钮
    }, function () {
        $.ajax({
            type: "Post",
            url: "/EquipmentOwner/UpdateEquipmentStats",
            data: { "equipmentId": equipmentId, "Status": newStatus },
            success: function (data) {
                if (data.result == "Success") {
                    loadOwnerEquipmentsByStatus(selectPage, pageSize, getStatus)
                    layer.alert("数据保存成功！");
                }
                else {
                    layer.alert("操作失败！" + data.message);
                }
            }
        });
    }); 
}

function SaveEquipmentInfo() {
    if ($('#EditEquipmentInfoEquipmentId').val() == null || $('#EditEquipmentInfoEquipmentId').val() == "") {
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
    formData.append('equipmentId', $('#EditEquipmentInfoEquipmentId').val());
    formData.append('name', $('#inputEditEquipmentName').val());
    formData.append('equipmentModelId', $('#inputEditEquipmentMode').val());

    $("#EditEquipmentInfoSave").attr("disabled", "disabled");
    $.ajax({
        type: "Post",
        url: "/EquipmentOwner/SaveEquipmentInfo",
        data: formData,
        processData: false,
        contentType: false,
        cache: false,
        success: function (data) {
            $("#EditEquipmentInfoSave").attr("disabled", false);

            if (data.result == "Success") {
                loadOwnerEquipmentsByStatus(selectPage, pageSize, getStatus)
                $("#EditEquipmentInfoModal").modal("hide");
                layer.alert("数据保存成功！");
            }
            else {
                layer.alert("操作失败！" + data.message);
            }
        }
    });
}

function refreshAcvtieStatus() {
    $("li.item").each(function (i, item) {
        var id = $(item).find("input[name='equipmentId']").val();
        $.get("/Equipment/GetActiveEquipmentInfo?id=" + id, function (result) {
            var info = $.parseJSON(result)
            $(item).find(".product-description #labelAcvtieStatus" + id).remove();
            $(item).find(".tools #btnMonitor" + id).remove();
            if (info.code == 1) {
                var label = $("<small id='labelAcvtieStatus" + id + "' class='label label-info pull-right'>在线</small>")
                $(item).find(".product-description").append(label);

                var btn = $(" <button id='btnMonitor" + id + "' class='btn btn-info btn-xs' onclick= 'showMonitorModal(\"" + id + "\",1,3)'> <i class='fa fa-tv'></i>监视器</button>");
                $(item).find(".tools").append(btn);
            }
            else {
                var label = $("<small id='labelAcvtieStatus" + id + "' class='label label-default pull-right'>离线</small>")
                $(item).find(".product-description").append(label);
            }


            var p = $("<p/>");
            $.each(result.rows, function (i, item) {
                var btn = $("<button/>").html(item.name).addClass("btn btn-info margin pull-left").click(function () { sendInstruction(item.paramRole) });
                p.append(btn);
            });
            $("#MonitorModal #instructionPanel").append(p);
        });
    });
    setTimeout('refreshAcvtieStatus()', 10000); 
    
}

function initMonitorModal() {
    $.get("/Equipment/GetInstructionList", function (result) {
        var p = $("<p/>");
        $.each(result.rows, function (i, item) {
            var btn = $("<button/>").html(item.name).addClass("btn btn-info margin pull-left").click(function () { sendInstruction(item.id, item.paramRole) });
            p.append(btn);
        });
        $("#MonitorModal #instructionPanel").append(p);
    });


    var autoLoad = false;
    $('#img').on('load', function () {
        if (autoLoad) {
            changeScreen();
        }
    });
    $("input[name='autoRefresh']").change(function () {
        //clearInterval(refreshInterval);
        if ($(this).is(':checked')) {
            autoLoad = true;
            $("#manualRefresh").attr("disabled", true);
            changeScreen();
        }
        else {
            autoLoad = false;
            $("#manualRefresh").removeAttr("disabled");
        }
    });
}

//显示设备指令发送model
function showMonitorModal(id) {
    $("#MonitorModal").modal("show");
    $("#MonitorModal input[name='id']").val(id);
    changeScreen();
}

//发送指令
function sendInstruction(methodId, paramRole) {
    var id = $("#MonitorModal input[name='id']").val();
    var params = {};
    $.each(eval(paramRole), function (i, item) {
        var val = prompt('请输入' + item.Name + "（" + item.Remarks+"）", "")
        params[item.Name] = val;
    })
    var data = { equipmentId: id, methodId: methodId, params: JSON.stringify(params)};
    $.post("/Equipment/SendInstruction", data, function (result) {
        if (result.code == 1) {
            layer.alert("发送成功");
        }
        else {
            layer.alert("发送失败，" + result.msg);
        }
    })
}

function changeScreen() {
    $('#img').attr("src", "/Equipment/GetScreen?id=" + $("input[name='id']").val() + "&_t=" + new Date().getTime());
}