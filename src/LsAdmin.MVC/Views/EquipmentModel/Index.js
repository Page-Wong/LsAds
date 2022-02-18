var selectedRole = 0;
var PageNo;
var pageSize = 20;
var showmodel = "list";  
$(function () {
    $("#btnDeleteMulti").click(function () { deleteMulti(); });
    $("#checkAllModel").click(function () { checkAllMaterial(this) });
    $("#btnNewSave").click(function () { save() });
    $("#btnAdd").click(function () { add() });
    PageNo = 1;
    loadTableseEquipmentModel(PageNo, pageSize);
    $("#inputApplyTo").select2();

    $("#showlist").click(function () { showmodel = "list"; loadTableseEquipmentModel(PageNo, pageSize); });
    $("#showtab").click(function () { showmodel = "tab"; loadTableseEquipmentModel(PageNo, pageSize); });


   // $("#inputModel").onblur(function () { initTooltip() });
   
    initTooltip();
});

//加载列表数据
function loadTableseEquipmentModel(startPage, pageSize) {
    PageNo = startPage;
    $("#checkAllModel").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/EquipmentModel/GetAllPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(),
        cache: true,
        async: false,
        success: function (data) {
            $("#equipmentmodellist").html("");
            $("#tableBody").html("");
            $('#MatrixMode').html("");
            if (showmodel == "list") {
                showList(data);
            } else if (showmodel == "tab") {
                showMatrix(data);
            } else {
                showList(data);
            }
            //showTable(data);
           
            loadApplyTosTags(data);
            loadscreenMaterials(data)
            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadTableseEquipmentModel(newPage, pageSize);
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

function showList(data) {
    $("#equipmentmodellist").html("");
    $.each(data.rows, function (i, item) {

        var liststr = "";
        liststr += "<li class='item'>";
        liststr += "<div class='product-img'>";
        liststr += "<img src='/EquipmentModel/GetThumbnail?id=" + item.id + "'/ >";
        liststr += "</div>";
        liststr += "<div class='product-info'>";
        liststr += "   <a id='em_" + item.id + "' class='products-title  btn-link'>型号：" + (item.model == null ? "" : item.model);  
        liststr += "   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 品牌:  " +(item.manufacturer == null ? "" : item.manufacturer) +"</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
        liststr += "适用:&nbsp;";
        var applyTos = item.applyTo == null ? "" : item.applyTo.split(",");
        if (applyTos != "") {
            $.each(applyTos, function (i, applyTo) {
                liststr += "<small class='label label-info'>" + applyTo + "</small>&nbsp;";
            })
        }
        liststr += " <div class='product-description  text-black'>  参数："+ (item.screenSize == null ? "" : item.screenSize + "寸");
        liststr += "   &nbsp;&nbsp; &nbsp;&nbsp;" + (item.screenMaterial == null ? "" : item.screenMaterial); 
        liststr += "   &nbsp;&nbsp; &nbsp;&nbsp;" + (item.resolutionRatio == null ? "" : item.resolutionRatio) + " &nbsp;&nbsp; " + (item.screenRatio == null ? "" : item.screenRatio); 
        liststr += "   &nbsp;&nbsp; &nbsp;&nbsp;" + (item.voicedName == null ? "" : item.voicedName);
        liststr += "   &nbsp;&nbsp; &nbsp;&nbsp; 宽 * 高:&nbsp; " + (item.screenWidth == null ? "" : item.screenWidth + "cm") + "*" + (item.screenHeight == null ? "" : item.screenHeight + "cm");
        liststr += "   &nbsp;&nbsp; &nbsp;&nbsp; 副屏数:&nbsp;" + (item.sideScreensNumber == null ? "" : item.sideScreensNumber);
        liststr += "</div>";
        liststr += "<div class='product-description  text-black'> 简介：<small>" + (item.describe == null ? "" : item.describe) +"</small></div>";
        liststr += " </div>";
        liststr += " <div class='tools'>";
        liststr += " <button type='button' class='btn btn-info btn-xs'  onclick= 'edit(\"" + item.id + "\")'><i class='fa fa-edit'></i>编辑</button>";
        liststr += " <button type='button' class='btn btn-danger btn-xs'onclick='deleteSingle(\"" + item.id + "\")'><i class='fa fa-trash-o'></i>删除</button>";
        liststr += "   </div>";
        liststr += " </li>";

        $("#equipmentmodellist").append(liststr);
        getequipmentmodelPopoverInfo('em_' + item.id,item);
    })


}


function showTable(data) {
    $("#tableBody").html("");
    $.each(data.rows, function (i, item) {
        var tr = "<tr >";
        tr += "<td>" + (i + 1) + "</td>";
        tr += "<td align='center'><input type='checkbox' class='checkboxs' value='" + item.id + "'/></td>";
        tr += "<td align='center'><img id='thumbnail" + item.id + "' class='img-thumbnail' height='80'  src='/EquipmentModel/GetThumbnail?id=" + item.id + "'/ ></td>";
        tr += "<td>" + (item.model == null ? "" : item.model) + "</td>";
        tr += "<td>" + (item.screenSize == null ? "" : item.screenSize) + "</td>";
        tr += "<td>" + (item.screenWidth == null ? "" : item.screenWidth) + "</td>";
        tr += "<td>" + (item.screenHeight == null ? "" : item.screenHeight) + "</td>";
        tr += "<td>" + (item.screenMaterial == null ? "" : item.screenMaterial) + "</td>";
        tr += "<td>" + (item.voiced == true ? "有" : "无") + "</td>";
        tr += "<td>" + (item.sideScreensNumber == null ? "" : item.sideScreensNumber) + "</td>";
        tr += "<td>" + (item.applyTo == null ? "" : item.applyTo) + "</td>";
        tr += "<td>" + (item.describe == null ? "" : item.describe) + "</td>";
        tr += "<td>" + (item.manufacturer == null ? "" : item.manufacturer) + "</td>";
        //   tr += "<td>" + (item.remarks == null ? "" : item.remarks) + "</td>";

        tr += "<td>" +
            "<button class='btn btn-info btn-xs'   href= 'javascript:;' onclick= 'edit(\"" + item.id + "\")' > <i class='fa fa-edit'></i> 编辑 </button >" +
            "<button class='btn btn-danger btn-xs' href='javascript:;'  onclick='deleteSingle(\"" + item.id + "\")'><i class='fa fa-trash-o'></i> 删除 </button>" +
            "</td > "
        tr += "</tr>";
        $("#tableBody").append(tr);
    })
}


function showMatrix(data) {
    $('#MatrixMode').html("");
    $.each(data.rows, function (i, item) {
        
        var div = " <div class='col-md-2' style='height: 420px;'>";
        div +="<div class='thumbnail'>";
        div += " <img style='height: 160px;' src='/EquipmentModel/GetThumbnail?id=" + item.id + "'/>";
   
        div += " <div class='caption'>";
        div += " <div>型号:<a class='products-title  btn-link'> " + (item.model == null ? "" : item.model) +"</a><div>";
        div += " <div> 品牌: " + (item.manufacturer == null ? "" : item.manufacturer)+"</div>";
        div += " <div> 屏幕: " + (item.screenSize == null ? "" : item.screenSize + "寸") + "&nbsp; " + (item.screenMaterial == null ? "" : item.screenMaterial)+"</div>";
        div += " <div> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;"+ (item.resolutionRatio == null ? "" : item.resolutionRatio) + "&nbsp;&nbsp;" + (item.screenRatio == null ? "" : item.screenRatio) +"</div>";
        div += " <div>宽*高: " + (item.screenWidth == null ? "" : item.screenWidth + "cm") + "*" + (item.screenHeight == null ? "" : item.screenHeight + "cm") +"</div>";
        div += " <div>声音: " + (item.voicedName == null ? "" : item.voicedName) + " &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;副屏数:" + (item.sideScreensNumber == null ? "" : item.sideScreensNumber)+"</div>";
        div += " <div>适用: &nbsp;";
        var applyTos = item.applyTo == null ? "" : item.applyTo.split(",");
        if (applyTos != "") {
            $.each(applyTos, function (i, applyTo) {
                div += "<small class='label label-info'>" + applyTo + " </small>&nbsp;";
            })
        }
        div += " </div>";
        div += " <span> 简介：<small>" + (item.describe == null ? "" : item.describe) +"</small></span>";
        div += "</div>";
        div += "<div class='tools'>";
        div += " <button type='button' class='btn btn-info btn-xs'  onclick= 'edit(\"" + item.id + "\")'><i class='fa fa-edit'></i>编辑</button>";
        div += " <button type='button' class='btn btn-danger btn-xs'onclick='deleteSingle(\"" + item.id + "\")'><i class='fa fa-trash-o'></i>删除</button>";
        div += "</div>";
        div += "</div>";
        div += "</div>";

         $("#MatrixMode").append(div);
    })
}

//屏幕材质列表  screenMaterials
function loadscreenMaterials(data) {
    var screenMaterials = "<option selected='selected'>--请选择屏幕材质--</option>";
    screenMaterials += "<option>--请选择屏幕材质--</option>";
    $.each(data.screenMaterials, function (i, item) {
        screenMaterials += "<option>" + item + "</option>"
    })
    $("#inpuScreenMaterial").html(screenMaterials);
}

//适用于标签选择框
function loadApplyTosTags(data) {
    var option = "";
    $.each(data.applyTos, function (i, item) {
        option += "<option>" + item + "</option>"
    })
    $("#inputApplyTo").html(option);
}

//全选
function checkAllMaterial(obj) {
    $(".checkboxs").each(function () {
        if (obj.checked == true) {
            $(this).prop("checked", true)

        }
        if (obj.checked == false) {
            $(this).prop("checked", false)
        }
    });
};

//保存-新增/修改
function save() {   
    $("#btnNewSave").attr("disabled", false);  
    var model = $('#inputModel');
    if (model == null || model.val() == "") {
        $('#inputModel').attr("data-original-title", "“型号”信息不能为空！");
        $('#inputModel').focus();
        return;
    }
    if (model.val().length > 100) {
        $('#inputModel').attr("data-original-title", "“型号”信息字数长度不能超过100！");
        $('#inputModel').focus();
        return;
    }

    if ($('#inputScreenSize').val() <= 0) {
        $('#inputScreenSize').focus();
        return;
    }

    if ($('#inpuScreenMaterial').val() == null || $('#inpuScreenMaterial').val() =="" || $('#inpuScreenMaterial').val().toString()=="--请选择屏幕材质--") {
        $('#inpuScreenMaterial').focus();
        return;
    }

    if ($('#inputThumbnail')[0].files[0] != null) {
        var fileSize = $('#inputThumbnail')[0].files[0].Size;
        var fileSize = fileSize / 1024 / 1024;
        if (fileSize > 5) {
            alert("图片不能大于1M");
            return;
        }
    }


    var formData = new FormData();
    formData.append('Id', $('#inputId').val());
    formData.append('Model', $('#inputModel').val());
    formData.append('ScreenSize', $('#inputScreenSize').val());
    formData.append('ScreenWidth', $('#inputScreenWidth').val());
    formData.append('ScreenHeight', $('#inputScreenHeight').val());
    formData.append('ScreenMaterial', $('#inpuScreenMaterial').val());
    formData.append('voiced', $("#inputvoiced").prop('checked'));
    formData.append('SideScreensNumber', $('#inputSideScreensNumber').val());
    formData.append('ApplyTo', $('#inputApplyTo').val().toString());
    formData.append('Describe', $('#inputDescribe').val());
    formData.append('Manufacturer', $('#inputManufacturer').val());
    formData.append('InputThumbnail', $('#inputThumbnail')[0].files[0]);
    formData.append('Thumbnail', "1");
    formData.append('ResolutionRatioLength', $('#inputResolutionRatioLength').val());
    formData.append('ResolutionRatioWidth', $('#inputResolutionRatioWidth').val());


    $("#btnNewSave").attr("disabled", "disabled");  
    $.ajax({
        type: "Post",
        url: "/EquipmentModel/Edit",
        data: formData,
        processData: false,
        contentType: false,
        cache: false,
        success: function (data) {

            $("#btnNewSave").attr("disabled", false);  

            if (data.result == "Success") {
                loadTableseEquipmentModel(PageNo, pageSize);
                if ($('#inputId').val() != "00000000-0000-0000-0000-000000000000" && $('#inputThumbnail')[0].files[0] != null) {
                    $("#thumbnail" + $("#inputId").val()).attr('src', "/EquipmentModel/GetThumbnail?id=" + $('#inputId').val() + "&_t=" + new Date().getTime());  
                }
              
                $("#EditModal").modal("hide");
            }
            else {
                layer.alert("操作失败！"+data.message);
            }
        }
    });
}

//批量删除
function deleteMulti() {
    var ids = "";
    $(".checkboxs").each(function () {
        if ($(this).prop("checked") == true) {
            ids += $(this).val() + ","
        }
    });
    ids = ids.substring(0, ids.length - 1);
    if (ids.length == 0) {
        layer.alert("请选择要删除的记录。");
        return;
    };
    //询问框
    layer.confirm("您确认删除选定的记录吗？", {
        btn: ["确定", "取消"]
    }, function () {
        var sendData = { "ids": ids };
        $.ajax({
            type: "Post",
            url: "/EquipmentModel/DeleteMuti",
            data: sendData,
            success: function (data) {
                if (data.result == "Success") {
                    loadTableseEquipmentModel(1, pageSize);
                    layer.closeAll();
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        });
    });
};
//删除单条数据
function deleteSingle(id) {
    layer.confirm("您确认删除选定的记录吗？", {
        btn: ["确定", "取消"]
    }, function () {
        $.ajax({
            type: "POST",
            url: "/EquipmentModel/Delete",
            data: { "id": id },
            success: function (data) {
                if (data.result == "Success") {
                    loadTableseEquipmentModel(1, pageSize);
                    layer.closeAll();
                }
                else {
                    layer.alert("删除失败！" + data.message);
                }
            }
        })
    });
};


//编辑

function edit(id) {
    $.ajax({
            type: "Get",
            url: "/EquipmentModel/Get?id=" + id + "&_t=" + new Date().getTime(),
            success: function (data) {
            $("#inputId").val(data.id);
            $("#inputModel").val(data.model);
            $("#inputScreenSize").val(data.screenSize);
            $("#inputScreenWidth").val(data.screenWidth);
            $("#inputScreenHeight").val(data.screenHeight);
            $("#inpuScreenMaterial").val(data.screenMaterial);
            $("#inputvoiced").prop("checked", data.voiced);
            $("#inputSideScreensNumber").val(data.sideScreensNumber);
            $("#inputApplyTo").select2("val", data.applyTo ? data.applyTo.split(',') : '');
            $("#inputDescribe").val(data.describe);
            $("#inputManufacturer").val(data.manufacturer);
            $("#EditTitle").text("修改");
            $("#inputThumbnail").val("");

            $("#inputResolutionRatioLength").val(data.resolutionRatioLength);
            $("#inputResolutionRatioWidth").val(data.resolutionRatioWidth);

            $("#inputModel").focus();
            $("#EditModal").modal("show");  

        }
    })
};
//新增
function add() {
    $("#btnNewSave").attr("disabled", false);  
    $("#inputId").val("00000000-0000-0000-0000-000000000000");
    $("#inputModel").val("");
    $("#inputScreenSize").val(0);
    $("#inputScreenWidth").val(0);
    $("#inputScreenHeight").val(0);
    $("#inpuScreenMaterial").val(1);
    $("#inputvoiced").prop("checked", true);
    $("#inputSideScreensNumber").val(0);
    $("#inputApplyTo").select2("val", "");
    $("#inputDescribe").val("");
    $("#inputManufacturer").val("");
    $("#EditTitle").text("新增");
    $("#inputThumbnail").val("");
    $("#inputModel").focus();
    $("#EditModal").modal("show");  
    $("#inputResolutionRatioLength").val(0);
    $("#inputResolutionRatioWidth").val(0);

}

//提示框初始化
function initTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
    $("#inputModel").attr("data-original-title", "字数不能超过100个字且不能为空");
    $("#inputScreenSize").attr("data-original-title", "屏幕尺寸需要大于0");
    $("#inpuScreenMaterial").attr("data-original-title", "请选择屏幕材质");
    $("#inputThumbnail").attr("data-original-title", "图片不能超过5M");
}
