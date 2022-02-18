var pageSize = 10;
var player_ids = getParameter("c[player_ids]");
var price = getParameter("c[price]");
var orderId = getParameter("c[order_id]");
var playerIdArr = player_ids.split(",");
var player_count = playerIdArr.length;

$(function () {
    //loadTables(1, pageSize);
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

    var programTableConfig = {
        tableBodyId: "selectBody",
        selectCallback: function (id, displayName, duration, width, height) {
            $("#select_" + id).click();
        }
    };
    ProgramSelectModal.init(programTableConfig);

    //选择方案
    $("#choose_area").click(function () { selectModal(); })

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
        "startDate": new Date(now.valueOf() + 1 * 24 * 60 * 60 * 1000),
        "endDate": new Date(now.valueOf() + 3 * 24 * 60 * 60 * 1000)
    });

    //时间控件动作
    $(document).on('change', '#input_date', function () {
        countDay();
    });

}

function datePickerStyle2(startDate, endDate) {
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
    var startdate = $('#input_date').data('daterangepicker').startDate;
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
                $("input[name='MateralType']").removeAttr("checked");
                if (data.order.materalType == 1) {
                    $("#MateralType1").attr("checked", "checked");
                    loadTables(1, pageSize, 1);
                }
                else if (data.order.materalType == 2) {
                    $("#MateralType2").attr("checked", "checked");
                    loadTables(1, pageSize, 2);
                }
                var programId = data.order.orderPlayerPrograms[0].playerProgram.programId;
                display_program_doc(programId);
                $("#input_remarks").val(data.order.remarks);
            }
        }
    })
}

//*****选择设备详情***************************************************
function loadLabels() {
    $("#equipmentCountLabel").text("投放媒体：" + player_count + "个");
    $("#equipmentPriceLabel").text("投放媒体单价：" + parseFloat(price) + "元");
}

/***选择方案*********************************************************/
//弹出选择方案窗体
function selectModal() {
    $("#programSelectModal").modal("show");
}

//确定方案
function chose_program() {
    var radio = document.getElementsByName("post_program");
    for (var i = 0; i < radio.length; i++) {
        if (radio[i].checked == true) {
            programId = radio[i].value;
            display_program_doc(programId);
            return;
        }
    }
    layer.alert('请选择方案', { icon: 2 });
}

function display_program_doc(programId) {
    $.ajax({
        type: "GET",
        url: "/Program/Get",
        data: { id: programId },
        success: function (data) {
            if (data.type == 0) {
                var inHTML = ''
                    + '<input hidden="hidden" id="programId" value=' + data.id + '>'
                    + '<input hidden="hidden" id="program_duration" value=' + data.duration + '>'
                    + '<div class="public_doc_module" doc_id=' + data.id + '>'
                    + '<h3>' + data.displayName + '</h3>'
                    + '<div class="box_h mainbox">'
                    + data.content
                    + '<div class="ctrol_btns">'
                    + '<span onclick="open_choose_doc_popup()">重选</span>'
                    + '<span onclick="cancel_program_doc()">取消</span>'
                    + '</div>'
                    + '</div>'
                    + '</div>';
            }
            else {
                var inHTML = ''
                    + '<input hidden="hidden" id="programId" value=' + data.id + '>'
                    + '<div class="public_doc_module" doc_id =' + data.id + '>'
                    + '<input hidden="hidden" id="program_duration" value=' + data.duration + '>'
                    + '<h3>' + data.displayName + '</h3>'
                    + '<div class="box_h mainbox">'
                    + '<div align="center" style="background:#000; color:#FFF;width:100%;height:100%;">'
                    + '<video id= "myvideo" width= "100%" height= "100%" src="/Material/PlayAsync?id=' + data.programMaterials[0].materialId + '" poster="/Material/PlayAsync?id=' + data.programMaterials[0].materialId + '" controls autoplay/> '
                    + '</div >'
                    + '<div class="ctrol_btns">'
                    + '<span onclick="open_choose_doc_popup()">重选</span>'
                    + '<span onclick="cancel_program_doc()">取消</span>'
                    + '</div>'
                    + '</div>'
                    + '</div>';
            }
            $('#choose_area').hide();
            document.getElementById('choose_result_box').innerHTM = '';
            document.getElementById('choose_result_box').innerHTML = inHTML;
            $('#choose_result_box').show();
            close_choose_doc_popup();
        }
    })
}

function close_choose_doc_popup() {
    $("#programSelectModal").find(".close").click();
}

function open_choose_doc_popup() {
    $("#programSelectModal").modal("show");
}

function cancel_program_doc() {
    $('#choose_result_box').hide().text('');
    $('#program_doc_result').val('');
    $('#choose_area').show();
    $("#programSelectModal").modal("show");
}

/**********************校验***************************/
function checkOrderInfo() {
    var name = $("#input_name").val();
    var industry = $("#input_industry").val();
    var remarks = $("#input_remarks").val();
    var id = $("#orderId").val();
    var totalseconds = $("#program_duration").val();
    var robj = document.getElementsByName("MateralType");
    var amount = $("#amount").text();
    var startdate = $('#input_date').data('daterangepicker').startDate.format("YYYY-MM-DD");
    var enddate = $('#input_date').data('daterangepicker').endDate.format("YYYY-MM-DD");
    for (i = 0; i < robj.length; i++) {
        if (robj[i].checked) {
            materalType = robj[i].value;
        }
    }

    DeleteBorder("choose_area");

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
        Status: 1,
        Type: 12,
        StartDate: startdate,
        EndDate: enddate,
    }
    return orderInfo;
}

//检查订单方案是否正确
function checkProgram() {
    var post_program = $("#programId").val();
    if (!post_program) {
        layer.open({
            content: '请选择方案',
            btn: ['确认'],
            yes: function (index) {
                layer.close(index);
                AddBorder($("#choose_area"));
                $("#choose_area").focus();
            }
        })
        return false;
    }
    var programId = $("#programId").val();
    return programId;
}

//检查订单媒体位是否为空
function checkOrderPlayers() {
    if (player_count == 0) {
        return false;
    }
    /*var players= [];
    for (i = 0; i < playerIdArr.length; i++) {
        players.push(playerIdArr[i]);
    }*/
    var playerIds = playerIdArr;
    return playerIds;
}

//保存方案
function Save() {
    var order = checkOrderInfo();
    if (!order) {
        return;
    }

    var playerIds = checkOrderPlayers();
    if (!playerIds) {
        return;
    }

    var programID = checkProgram();
    if (!programID) {
        return;
    }

    var shoppingCartType = $("#shoppingCartType").val();
    $.ajax({
        type: "POST",
        url: "/Order/SaveAll",
        data: { order: order, programID: programID, playIds: playerIds, shoppingCartType: shoppingCartType },
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

    var playerIds = checkOrderPlayers();
    if (!playerIds) {
        return;
    }

    var programID = checkProgram();
    if (!programID) {
        return;
    }

    var shoppingCartType = $("#shoppingCartType").val();
    $.ajax({
        type: "POST",
        url: "/Order/SaveAll",
        data: { order: order, programID: programID, playIds: playerIds, shoppingCartType: shoppingCartType },
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
}

//格式-增删红色方框
function AddBorder(selector) {
    selector.css("border", "2px solid red");
}

function DeleteBorder(selector) {
    var obj = document.getElementById(selector);
    obj.style.border = "none";
}

