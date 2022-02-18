var pageSize = 10;
var getStatus= 0;
$(function () {
    initTooltip();
    loadEquipmentRepairList(1, pageSize, getStatus);
    $("#ConfirmedSave").click(function () { saveConfirmedInfo(); });
    $("#CompleteSave").click(function () { SaveCompleteInfo(); });

    $("#btnStatusUnConfirmed").click(function () { getStatus = 0; loadEquipmentRepairList(1, pageSize, getStatus); });
    $("#btnStatusConfirmed").click(function ()   { getStatus = 1; loadEquipmentRepairList(1, pageSize, getStatus); });
    $("#btnStatusComplete").click(function ()    { getStatus = 2; loadEquipmentRepairList(1, pageSize, getStatus); });

});

//加载列表数据
function loadEquipmentRepairList(startPage, pageSize, status) {
    $("#equipmentrepairlist").html("");
    $.ajax({
        type: "GET",
        url: "/EquipmentRepair/GetOwnerEquipmentRepairPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&status=" + status +   "&_t=" + new Date().getTime(),
        success: function (data) {

            /*switch (status) {
                case 0: $("#state").html("待确认"); break;
                case 1: $("#state").html("已确认"); break;
                case 2: $("#state").html("已完成"); break;
            }*/

            $("#SpanStatusUnConfirmed").html(data.statusUnConfirmed);
            $("#SpanStatusConfirmed").html(data.statusConfirmed);
            $("#SpanStatusComplete").html(data.statusComplete);

            $.each(data.rows, function (i, item) {
                var li = " <li class='item'>";
                li += "<div class='product-img'> <img src='/EquipmentModel/GetThumbnail?id=" + item.equipmentDto.equipmentModelId + "'/></div>"; 
                li += "<div class='product-info'>";
                // li += "<a id='em_" + item.id + "' class='products-title btn-link'>" + (item.equipmentDto.equipmentModelDto.manufacturer == null ? "" : item.equipmentDto.equipmentModelDto.manufacturer + "：") + item.equipmentDto.equipmentModelDto.model + "</a>&nbsp;&nbsp;&nbsp;&nbsp;";
                li += "<a id='em_" + item.id + "' class='products-title btn-link'>" + (item.equipmentDto.name == null ? "" : item.equipmentDto.name)  + "</a>&nbsp;&nbsp;&nbsp;&nbsp;";

                li += "<a  id='p_" + item.id + "' class='product-title btn-link'>" + item.placeDto.name + "(" + item.placeDto.address + (item.placeContact == null ? "" : "--" + item.placeContact) + (item.placeContactPhone == null ? "" : "--" + item.placeContactPhone) + ")</a>";
               
                if (item.status==0){
                    li += "<small class='label label-warning'>" + item.statusString + "</small>";
                    
                } else if (item.status == 1) {
                    li += "<small class='label label-primary'>" + item.statusString + "</small>";
                } else {
                    li += "<small class='label label-success'>" + item.statusString + "</small>";
                }
                li += "<small class='label label-danger'><i class='fa fa-clock-o'></i> " + item.timeAgo + "</small>";
                if (item.processingMethodString != "") {
                    li += "<small class='label label-info'>" + item.processingMethodString + "</small>";
                }

                li += "<br class='product-description text-red'><b class='text-red'>报障信息：" + item.warningDate + "--</b>"+ item.problemDescription + "</br>";

                if (item.processingPerson != null && item.processingPerson != "" ){
                    li += "<span class='product-description  text-blue '>维修人员：" + item.processingPerson +(item.processingPersonPhone ==null ? "" :"--"+ item.processingPersonPhone)  + "</span>";
                }

                if (item.processingResults != null && item.processingResults != "" ){
                    li += "<p class='product-description  text-green' >处理结果：" +item.processingResults + "</p>";
                }
                li += "</div >";

                li += "<div class='tools'>";
              
                if (item.status == 0) {
                    li += "<button class='btn btn-primary btn-xs' href='javascript: ;'   onclick = 'showConfirmedModel(\"" + item.id + "\")' > <i class='fa fa-legal'></i>确认</button>";

                } else if (item.status == 1) {
                    li += "<button class='btn btn-primary btn-xs' href='javascript: ;'   onclick = 'showConfirmedModel(\"" + item.id + "\")' > <i class='fa fa-legal'></i>确认</button>";
                    li += "<button class='btn btn-success btn-xs' href='javascript: ;'   onclick = 'showCompleteModal(\"" + item.id + "\")' > <i class='fa fa-check'></i>完成</button>";
                } else {
                    li += "<button class='btn btn-info btn-xs' href='javascript: ;'   onclick = 'cancelComplete(\"" + item.id + "\")'> <i class='fa fa-reply'></i>取消完成</button>";
                }

                li += "</div>"; 
                li += "</li>";
              
                $("#equipmentrepairlist").append(li);
                getPlacePopoverInfo('p_' + item.id, item.placeDto);
                getequipmentmodelPopoverInfo('em_' + item.id, item.equipmentDto.equipmentModelDto);
               
            })

            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadEquipmentRepairList(newPage, pageSize, getStatus);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
            $("ul > li").click(function () {
                $("ul > li").removeAttr("style")
                $(this).attr("style", "background-color:#beebff");
            });
        }
    })
}


//保存-确认信息
function saveConfirmedInfo() {   
    $("#ConfirmedSave").attr("disabled", false);
    
    var selectedId = $('#inputId').val();
    var processingPerson = $('#inputProcessingPerson').val();
    var processingPersonPhone = $('#inputProcessingPersonPhone').val();


    if (processingPerson == null || processingPerson== "") {
        $('#inputProcessingPerson').focus();
        return;
    }

    if (processingPersonPhone == null || processingPersonPhone == "") {
        $('#inputProcessingPersonPhone').focus();
        return;
    }

    $("#ConfirmedSave").attr("disabled", "disabled");  
    $.ajax({
        type: "POST",
        url: "/EquipmentRepair/SaveConfirmedInfo",
        data: { id: selectedId, processingPerson: processingPerson, processingPersonPhone: processingPersonPhone },

        success: function (data) {
            $("#ConfirmedSave").attr("disabled", false);  
              
            if (data.result == "Success") {
                layer.alert('保存成功');
                loadEquipmentRepairList(1, pageSize, getStatus);
                $("#ConfirmedModal").modal("hide");
            }
            else {
                layer.alert('保存失败：' + (data.message== null ? "":data.message) );
                if (data.ErrorField = "processingPersonPhone") { $('#inputProcessingPersonPhone').focus(); } else { $('#inputProcessingPerson').focus(); }
            }
        }
    });
}

// 保存完成信息
function SaveCompleteInfo() {
    $("#CompleteSave").attr("disabled", false);

    var selectedId = $('#inputIdS').val();
    var processingMethod = $('input:radio[name="inputProcessingMethod"]:checked').val();
    var processingResults = $('#inputProcessingResults').val();

    if (processingMethod == null) {

        layer.alert('请选择处理方式');
        return;
    }

    if (processingResults == null || processingResults == "") {
        $('#inputProcessingResults').focus();
        return;
    }

    $("#CompleteSave").attr("disabled", "disabled");
    $.ajax({
        type: "POST",
        url: "/EquipmentRepair/SaveCompleteInfo",
        data: { id: selectedId, processingMethod: processingMethod, processingResults: processingResults },

        success: function (data) {
            $("#CompleteSave").attr("disabled", false);

            if (data.result == "Success") {
                layer.alert('保存成功');
                loadEquipmentRepairList(1, pageSize,getStatus);
                $("#CompleteModal").modal("hide");
            }
            else {
                layer.alert('保存失败：' + (data.message == null ? "" : data.message));
            }
        }
    });
}


//显示确认model
function showConfirmedModel(id) {
    $("#ConfirmedSave").attr("disabled", false);
    $.ajax({
            type: "Get",
            url: "/EquipmentRepair/Get?id=" + id + "&_t=" + new Date().getTime(),
            success: function (data) {
            $("#inputId").val(data.id);
            $("#inputProcessingPerson").val(data.processingPerson);
            $("#inputProcessingPersonPhone").val(data.processingPersonPhone);
            $("#ConfirmedModal").modal("show");  
            $("#inputProcessingPerson").focus();
        }
    })
};

//显示完成model
function showCompleteModal(id) {
    $("#CompleteSave").attr("disabled", false);
    $.ajax({
        type: "Get",
        url: "/EquipmentRepair/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#inputIdS").val(data.id);
            $("#inputProcessingPersonS").val(data.processingPerson);
            $("#inputProcessingPersonPhoneS").val(data.processingPersonPhone);
            $("#inputProcessingResults").val(data.ProcessingResults);

            switch (data.processingMethod) {
                case 2:
                    $("#replace").prop('checked', true);
                    break;
                case 3:
                    $("#other").prop('checked', true);
                    break;
                default:
                    $("#repair").prop('checked', true);
            }
            $("#CompleteModal").modal("show");
            $("#inputProcessingResults").focus();
        }
    })
};

function cancelComplete(id) {
    layer.confirm("确认取消记录完成信息？", {
        btn: ["确定", "取消"]
    }, function () {
        $.ajax({
            type: "POST",
            url: "/EquipmentRepair/CancelComplete",
            data: { "id": id },
            success: function (data) {
                if (data.result == "Success") {
                    layer.alert("保存成功！");
                    loadEquipmentRepairList(1, pageSize, getStatus);
                    layer.closeAll();
                }
                else {
                    layer.alert("操作失败！");
                }
            }
        })
    });
};


//提示框初始化
function initTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
    $("#inputProcessingPerson").attr("data-original-title", "维修人员不能为空");
    $("#inputProcessingPersonPhone").attr("data-original-title", "维修人员联系信息不能为空");
    $("#inputProcessingResults").attr("data-original-title", "处理结果不能为空");
}
