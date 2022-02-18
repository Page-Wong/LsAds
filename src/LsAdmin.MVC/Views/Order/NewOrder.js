var pageSize = 10;
var equipment_ids = getParameter("c[equipment_ids]");
var price = getParameter("c[price]");
var orderId = getParameter("c[order_id]");
var equipmentIdArr = equipment_ids.split(",");
var equipment_count = equipmentIdArr.length;

$(function () {     
    loadTables(1, pageSize);
    loadLabels();
    initTooltip();

    if (orderId == "" || orderId == null) {
        datePickerStyle(); 
        countDay();
    }
    else {        
        getOrder();        
    }

    //素材类型选择
    $("input[name='MateralType']").change(function () {
        loadTables(1, pageSize, $(this).val());
        $("#selectTableBody").html("");
        $('#input_materialtotaltime').html(0);

    });

    //上传素材
    $("#btnAdd").click(function () { add(); });
    $("#btnDelete").click(function () { deleteMulti(); });
    $("#btnSave").click(function () { save(); });
    $("#btnSelect").click(function () { selectMulti() });
    $("#checkAll").click(function () { checkAll(this) });

    $("#Dir").fileinput({
        uploadUrl: "/Material/Add",
        enctype: 'multipart/form-data',
        allowedFileExtensions: ['mp4', '3gp', 'avi', 'jpeg', 'bmp', 'jpg', 'png'],//接收的文件后缀
        language: "zh",
        uploadExtraData: function (previewId, index) {
            if (!previewId) {
                return;
            }
            var obj = {};
            if ($('#' + previewId).find('video')[0] == null) {
                return obj.duration = 0;
            }
            var duration = $('#' + previewId).find('video')[0].duration;
            obj.duration = duration;
            return obj;
        }
    });

    //导入文件上传完成之后的事件
    $("#Dir").on("fileuploaded", function (event, data, previewId, index) {
        var data = data.response;
        if (data.result != "Success") {
            layer.alert('上传失败！' + data.message);
            return;
        }
        else {
            var robj = document.getElementsByName("MateralType");
            for (i = 0; i < robj.length; i++) {
                if (robj[i].checked) {
                    materialtype = robj[i].value;
                }
            }
            if (data.data.materialType == materialtype) {
                addMaterialListItem(materialtype, data.data.id, data.data.name, data.data.duration, data.data.remarks);
                selectedIds.push(data.data.id);
            }
            else {
                //toastr.error('请上传正确的素材类型');
                layer.alert("请上传正确的素材类型(素材已上传至我的推广素材，但无法选择为本次推广的播放素材。)");

            }
        }
        $("#uploadModal").modal("hide");
        $('#Dir').fileinput('reset');
    });
})

//提示框初始化
function initTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
    $("#input_name").attr("data-original-title", "字数不能超过50个字且不能为空");
    $("#input_industry").attr("data-original-title", "请选择公司所属行业");
}

//获取url的参数
function getParameter(param) {
    var query = window.location.search;
    var iLen = param.length;
    var iStart = query.indexOf(param);
    if (iStart == -1)
        return "";
    iStart += iLen + 1;
    var iEnd = query.indexOf("&", iStart);
    if (iEnd == -1)
        return query.substring(iStart);

    return query.substring(iStart, iEnd);
}

//时间选择器的样式及方法
function datePickerStyle() {
    var nowTemp = new Date();
    var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);
    $('#input_date').daterangepicker({
        locale: {
            format: 'YYYY-MM-DD'
        },
        "autoApply": true,
        "minDate": new Date(now.valueOf() + 1 * 24 * 60 * 60 * 1000),
        "startDate": new Date(now.valueOf()+1 * 24 * 60 * 60 * 1000),
        "endDate": new Date(now.valueOf() + 3 * 24 * 60 * 60 * 1000)
    });

    //时间控件动作
    $(document).on('change', '#input_date', function () {
        countDay();
    });

}

function datePickerStyle2(startDate,endDate) {
    var nowTemp = new Date();
    var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);
    var s = new Date(startDate);
    if (s.getTime() < nowTemp.getTime()) {
        $('#input_date').daterangepicker({
            locale: {
                format: 'YYYY-MM-DD'
            },
            "autoApply": true,
            "minDate": new Date(now.valueOf() + 1 * 24 * 60 * 60 * 1000),
            "startDate": new Date(now.valueOf() + 1 * 24 * 60 * 60 * 1000),
            "endDate": new Date(now.valueOf() + 3 * 24 * 60 * 60 * 1000)
        });
    }
    else {
        $('#input_date').daterangepicker({
            locale: {
                format: 'YYYY-MM-DD'
            },
            "autoApply": true,
            "minDate": new Date(now.valueOf() + 1 * 24 * 60 * 60 * 1000),
            "startDate": startDate,
            "endDate": endDate,
        }); 
    }   
    //时间控件动作
    $(document).on('change', '#input_date', function () {
        countDay();
    });
    
}

function countDay() {
    var startdate =$('#input_date').data('daterangepicker').startDate;
    var enddate = $('#input_date').data('daterangepicker').endDate;
    var days = Math.round((enddate - startdate) / 1000 / 60 / 60 / 24);
    $("#dayLabel").text("共  " + days + "  天");
    $("#amount").text((parseFloat(price) * days).toFixed(2));   
}


//获取订单信息
function getOrder() {
    $.ajax({
        type: "Get",
        url: "/Order/GetOrder?id=" + orderId + "&_t=" + new Date().getTime(),
        success: function (data) {
            if (data.result = "Success") {
                $("#orderId").val(data.order.id);
                $("#shoppingCartType").val(2);
                $("#input_name").val(data.order.name);
                $("#input_industry").val(data.order.industry);
                datePickerStyle2(data.order.startDate, data.order.endDate);
                countDay();
                //$('#input_date').val(data.order.startDate.substring(0, 10) + ' - ' + data.order.endDate.substring(0, 10));
                $("input[name='MateralType']").removeAttr("checked");
                if (data.order.materalType == 1) {
                    $("#MateralType1").attr("checked", "checked");
                    loadTables(1, pageSize, 1);
                }
                else if (data.order.materalType == 2) {
                    $("#MateralType2").attr("checked", "checked");
                    loadTables(1, pageSize, 2);
                }
                
                getOrderMaterial();
                $("#input_remarks").val(data.order.remarks);
            }
        }
    })
}

//获取订单素材
function getOrderMaterial() {
    $.ajax({
        type: "Get",
        url: "/Order/GetOrderMaterials?id=" + orderId + "&_t=" + new Date().getTime(),
        success: function (data) {
            materialType = getMaterialType();
            if (data.result = "Success") {
                $.each(data.ordermaterials, function (i, item) {
                    addMaterialListItem(materialType, item.id, item.name, item.duration, item.remarks);
                    selectedIds.push(item.id);
                })
            }
        }
    })
}


//*****选择设备详情***************************************************
function loadLabels() {
    $("#equipmentCountLabel").text("投放媒体：" + equipment_count + "个");
    $("#equipmentPriceLabel").text("投放媒体单价：" + parseFloat(price) + "元");
}

//***选择素材*********************************************************
var selectedIds = [];
//获取素材类型
function getMaterialType() {
    var robj = document.getElementsByName("MateralType");
    for (i = 0; i < robj.length; i++) {
        if (robj[i].checked) {
            var materialType = robj[i].value;
        }
    }
    return materialType;
}

//加载素材库列表数据
function loadTables(startPage, pageSize, materalType) {
    //materalType = materalType ? materalType : $("input[name='MateralType'][checked]").val();
    var robj = document.getElementsByName("MateralType");
    for (i = 0; i < robj.length; i++) {
        if (robj[i].checked) {
            materialType = robj[i].value;
        }
    }
    $("#tableBody").html("");
    $("#checkAll").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/Material/GetAllPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&materialType=" + materialType + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                addSelectorMaterialListItem(materialType, item.id, item.name, item.duration, item.remarks);
            })
            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadTables(newPage, pageSize);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
            $("table > tbody > tr").click(function () {
                $("table > tbody > tr").removeAttr("style")
                $(this).attr("style", "background-color:#beebff");
            });
        }
    })
}
//增加素材库列表项
function addSelectorMaterialListItem(materialType, id, name, duration, remarks) {
    if ($.inArray(id, selectedIds) == -1) {
        var tr = "<tr id='id_" + id + "'>";
        tr += "<td align='center' hidden='hidden'><input type='checkbox' class='checkboxs' value='" + id + "' hidden/></td>";
        tr += "<td style='word-wrap:break-word; word-break:break-all;'>" + (name == null ? "" : name) + "</td>";
        tr += "<td>" + (duration == null ? "" : duration) + "</td>";
        tr += "<td>" + (remarks == null ? "" : remarks) + "</td>";
        tr += "<td>" +
            "<button class='btn btn-info btn-xs'   href= 'javascript:;' onclick =\"playMaterial('" + id + "','" + materialType + "')\"> <i class='fa fa-edit'></i> 查看 </button >" +
            "<button class='btn btn-primary btn-xs' href='javascript:;' onclick='addMaterialListItem(\"" + materialType + "\",\"" + id + "\", \"" + name + "\",\"" + duration + "\", \"" + remarks + "\")'><i class='fa fa-send'></i> 选择 </button>" +
            "</td > ";
        tr += "</tr>";
        $("#tableBody").append(tr);
    }
}

//新增
function add() {
    $("#Title").text("新增媒体");
    //弹出新增窗体
    $("#uploadModal").modal("show");
};

//播放
function playMaterial(id, materialtype) {
    //弹出新增窗体
    $("#playModal").modal("show");
    if (materialtype = 1) {
        $("#myvideo").attr("poster", "/Material/PlayAsync?id=" + id);
        $("#myvideo").attr("src", "/Material/PlayAsync?id=" + id);
    }
    else {

        $("#myvideo").attr("src", "/Material/PlayAsync?id=" + id);
    }
};

//增加播放素材列表项
function addMaterialListItem(materialType, id, name, duration, remarks) {
    var tr = "<tr id='selectedid_" + id + "'>";
    tr += "<td align='center' hidden='hidden'><input type='text' class='selectTableBodyId' value='" + id + "'/></td>";
    tr += "<td style='word-wrap:break-word; word-break:break-all;'>" + (name == null ? "" : name) + "</td>";
    if (materialType == 1) {
        tr += "<td>" + "<input type='text' id='input_seconds_" + id + "' name='input_seconds' value='5' onchange='countMaterialtotaltime()'>" + "</td>";
    }
    else if (materialType == 2) {
        tr += "<td>" + "<input type='text' id='input_seconds_" + id + "' name='input_seconds' value='" + duration + "' disabled='disabled'>" + "</td>";
    }
    tr += "<td>" +
        "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'upTR(\"" + id + "\")' > <i class='fa fa-edit'></i> 上调 </button >" +
        "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'downTR(\"" + id + "\")' > <i class='fa fa-edit'></i> 下调</button >" +
        "<button class='btn btn-danger btn-xs' href='javascript:;' onclick='deleteSelectedSingleTR(\"" + materialType + "\",\"" + id + "\", \"" + name + "\",\"" + duration + "\" ,\"" + remarks + "\")'><i class='fa fa-trash-o'></i> 删除 </button>" +
        "</td > ";
    tr += "</tr>";
    $("#selectTableBody").append(tr);
    if ($('#id_' + id)) {
        $('#id_' + id).remove();
    }
    countMaterialtotaltime();
    selectedIds.push(id);
}


//UpTR
function upTR(id) {
    var table = document.getElementById("selectTableBody");
    var selectedTr = document.getElementById("selectedid_" + id);
    var preTr = selectedTr.previousSibling;
    if (preTr) {
        table.insertBefore(selectedTr, preTr);
        $("#selectedid_" + id).focus();
    }
}

//DownTR
function downTR(id) {
    var table = document.getElementById("selectTableBody");
    var selectedTr = document.getElementById("selectedid_" + id);
    var nextTr = selectedTr.nextSibling;
    if (nextTr) {
        table.insertBefore(nextTr, selectedTr);
        $("#selectedid_" + id).focus();
    }
}

//删除单条本订单素材
function deleteSelectedSingleTR(materialType, id, name, duration, remarks) {
    var index = selectedIds.indexOf(id);
    selectedIds.splice(index, 1);
    addSelectorMaterialListItem(materialType, id, name, duration, remarks);
    $("#selectedid_" + id).remove();
    countMaterialtotaltime();
};

//计算素材总时间
function countMaterialtotaltime() {
    var materialtotaltime = 0;
    var input_seconds = $('input[name=input_seconds]');
    $.each(input_seconds, function (i, item) {
        materialtotaltime += parseFloat($(item).val());
    });
    $("#input_materialtotaltime").html(materialtotaltime);
}


function checkOrderInfo() {
    var name = $("#input_name").val();
    var industry = $("#input_industry").val();
    var remarks = $("#input_remarks").val();
    var id = $("#orderId").val();
    var totalseconds = $("#input_materialtotaltime").text();
    var robj = document.getElementsByName("MateralType");
    var amount = $("#amount").text();
    var startdate = $('#input_date').data('daterangepicker').startDate.format("YYYY-MM-DD");
    var enddate = $('#input_date').data('daterangepicker').endDate.format("YYYY-MM-DD");
    for (i = 0; i < robj.length; i++) {
        if (robj[i].checked) {
            materalType = robj[i].value;
        }
    }

    DeleteBorder("foucs_material");

    //用户名不能为空
    if (name == null || name == "") {
        $("#input_name").attr("data-original-title", "方案名称不能为空")
        $("#input_name").focus();
        return false;
    }

    //行业类型不为空
    if (industry == null || industry == "" || industry == "-请选择-") {
        $("#input_industry").attr("data-original-title", "行业类型不能为空");
        $("#input_industry").focus();
        return false;
    }

    var orderInfo = {
        Id: id ? id : "00000000-0000-0000-0000-000000000000",
        Name: name,
        Industry: industry,
        TotalSeconds: totalseconds,
        Remarks: remarks,
        Amount: amount,
        MateralType: materalType,
        Status: 1,
        Type: 12,
        StartDate: startdate,
        EndDate: enddate,
    }
    return orderInfo;
}

//检查订单素材信息是否正确
function checkOrderMaterials() {
    var orderMaterials = [];
    var orderby = 0;
    $("#selectTableBody .selectTableBodyId").each(function () {
        var id = $(this).val();
        orderby += 1;
        orderMaterials.push({
            MaterialId: id,
            Orderby: orderby,
            Seconds: $("#input_seconds_" + id).val(),
        })
    });

    //播放素材不能为空
    if (orderMaterials.length == 0) {
        layer.open({
            content: '请选择本次推广需要的素材',
            btn: ['确认'],
            yes: function (index) {
                layer.close(index);
                AddBorder($("#foucs_material"));
                $("#foucs_material").focus();
            }
        })
        return false;
    }
    return orderMaterials;
}

//检查订单媒体位是否为空
function checkOrderEquipments() {
    if (equipment_count == 0) {
        return false;
    }
    var orderEquipments = [];
    for (i = 0; i < equipmentIdArr.length; i++) {
        orderEquipments.push({
            EquipmentId: equipmentIdArr[i],
        })
    }
    return orderEquipments;
}


//保存方案
function Save() {
    var order = checkOrderInfo();
    if (!order) {
        return;
    }

    var orderEquipments = checkOrderEquipments();
    if (!orderEquipments) {
        return;
    }

    var orderMaterials = checkOrderMaterials();
    if (!orderMaterials) {
        return;
    }

    var shoppingCartType = $("#shoppingCartType").val();
    $.ajax({
        type: "POST",
        url: "/Order/Save",
        data: { order: order, orderMaterials: orderMaterials, orderEquipments: orderEquipments, shoppingCartType:shoppingCartType },
        success: function (data) {
            if (data.result == "Success") {
                layer.open({
                    content: '保存成功',
                    btn: ['确认'],
                    yes: function (index) {
                        layer.close(index);
                        window.location.href = "List";
                    }
                })
            }
            else {
                layer.alert(data.message);
            }
        }
    });

}

//保存方案
function Pay() {
    var order = checkOrderInfo();
    if (!order) {
        return;
    }

    var orderEquipments = checkOrderEquipments();
    if (!orderEquipments) {
        return;
    }

    var orderMaterials = checkOrderMaterials();
    if (!orderMaterials) {
        return;
    }

    var shoppingCartType = $("#shoppingCartType").val();
    $.ajax({
        type: "POST",
        url: "/Order/Save",
        data: { order: order, orderMaterials: orderMaterials, orderEquipments: orderEquipments, shoppingCartType: shoppingCartType },
        success: function (data) {
            if (data.result == "Success") {
                $("#orderId").val(data.id);
                pay(data.orderNo, data.amount, data.name, data.id);
            }
            else {
                layer.alert(data.message);
            }
        }
    });

}

//付款
function pay(orderNo, amount, subject, orderId) {
    $('#totalAmout').val(amount);
    $('#orderNo').val(orderNo);
    $('#subject').val(subject);
    $('#payOrderId').val(orderId);
    $("#tradeMethodModal").modal();
};

//增删红色方框
function AddBorder(selector) {
    selector.focus(function () {
        selector.css("border", "2px solid red");
    });
}

function DeleteBorder(selector) {
    var obj = document.getElementById(selector);
    obj.style.border = "none";
}