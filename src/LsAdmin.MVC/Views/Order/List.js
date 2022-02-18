var pageSize = 15;
$(function () {
    //$("#btnAdd11").click(function () { add(); });
    //$("#btnAdd12").click(function () { add(); });
    $("#btnDelete").click(function () { deleteMulti(); });
    $("#checkAll").click(function () { checkAll(this) });
    $(".modal #btnSave").click(function () { applyRefund() });
    loadTables(1, pageSize);
});
 
//加载列表数据
function loadTables(startPage, pageSize) {
    $("#tableBody").html("");
    $("#checkAll").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/Order/GetAllPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {                
                var tr = "<tr>";
                tr += "<td align='center'><input type='checkbox' class='checkboxs' value='" + item.id + "'/></td>";
                tr += "<td>" + (item.statusString == null ? "" : item.statusString) + "</td>";
                tr += "<td>" + (item.name == null ? "" : item.name) + "</td>";
                tr += "<td>" + (item.totalSeconds == null ? "" : item.totalSeconds) + "</td>";
                tr += "<td>" + (item.amount == null ? "" : item.amount) + "</td>";
                tr += "<td>";
                tr += "<button class='btn btn-default btn-xs' href= 'javascript:;' onclick= 'getInfo(\"" + item.id + "\"," + item.type+")' > <i class='fa fa-info'></i> 查看 </button > &nbsp;";
                if (item.status == 1) {
                    tr += "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'edit(\"" + item.id + "\"," + item.type +")' > <i class='fa fa-edit'></i> 编辑 </button > &nbsp;" +
                        "<button class='btn btn-primary btn-xs' href='javascript:;' onclick='pay(\"" + item.orderNo + "\",\"" + item.amount + "\",\"" + item.name + "\",\"" + item.id + "\")'><i class='fa fa-rmb'></i> 付款 </button>" +"&nbsp; " +
                        "<button class='btn btn-danger btn-xs' href='javascript:;' onclick='deleteSingle(\"" + item.id + "\")'><i class='fa fa-trash-o'></i> 删除 </button>&nbsp;";
                }                
                if (item.status == 2 || item.status == 3) {
                    tr += "<button class='btn-link btn-xs' href='javascript:;' onclick='refund(\"" + item.id + "\")'><i class='fa fa-rmb'></i> 退款 </button>"
                }
                tr += "</td > "
                tr += "</tr>";
                $("#tableBody").append(tr);
            })
            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
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
//全选
function checkAll(obj) {
    $(".checkboxs").each(function () {
        if (obj.checked == true) {
            $(this).prop("checked", true)

        }
        if (obj.checked == false) {
            $(this).prop("checked", false)
        }
    });
};
//新增
//TODO G 改为正确的新增方法
function add() {
    /*$("#Id").val("00000000-0000-0000-0000-000000000000");
    $("#Dir").val("");
    $("#Name").val("");
    $("#Remarks").val("");*/
    $("#Title").text("新增媒体");
    //弹出新增窗体
    $("#uploadModal").modal("show");
};
//编辑
//TODO G 改为正确的修改方法
/*function edit(id) {
    $.ajax({
        type: "Get",
        url: "/Order/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#Id").val(data.id);
            $("#Name").val(data.name);
            $("#OwnerUserId").val(data.ownerUserId);
            $("#Remarks").val(data.remarks);

            $("#Title").text("编辑媒体")
            $("#editModal").modal("show");
        }
    })
};*/

function edit(id, type) {
    if (type == 11) {
        window.location.href = "Edit?id=" + id;   
    }
    else {
        window.location.href = "showequipments?c[order_id]=" + id;
    }            
}

function getInfo(id,type) {
    if (type == 11) {
        getInfo_11(id);
    }
    else {
        getInfo_12(id);
    }
}

function getInfo_11(id) {
    $.ajax({
        type: "Get",
        url: "/Order/GetOrder?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            if (data.result = "Success") {
                $("#modal_name").text(data.order.name);
                $("#modal_totaltime").text(data.order.totalSeconds + "秒");
                $("#modal_industry").text(data.order.industry);
                $("#modal_adstag").text(data.order.adsTag);
                $("#modal_url").text(data.order.url);
                $("#modal_remarks").text(data.order.remarks);
                $("#modal_totalAmount").text(data.order.amount);
                var programId = data.order.orderPlayerPrograms[0].playerProgram.programId;
                $.ajax({
                    type: "Get",
                    url: "/Program/Get",
                    data: { id: programId },
                    success: function (data) {
                        $("#modal_program").text(data.displayName);
                    }
                })    
            }
        }
    })
    $.ajax({
        type: "Get",
        url: "/Order/GetOrderTimes?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            if (data.result = "Success") {
                var areas = [];
                $.each(data.areas, function (i, item) {
                    var area = item.province + " " + item.city + " " + item.district;
                    areas.push(area);
                })
                $("#modal_areas").text(areas.join(" , "));
                $("#modal_daterange").text(data.ordertimes[0].startDate.substring(0, 10) + ' 至 ' + data.ordertimes[0].endDate.substring(0, 10));
                var tr = '';
                $.each(data.ordertimes, function (i, item) {
                    $("#modal_timerangeinfo").html("");
                    tr += '<span class="modal-items-item">';
                    tr += '<span class="modal-items-item-name">' + item.timeRangeType + '播放时间段：</span > ';
                    tr += '<span class="modal-items-item-value" id="info_timerange_' + item.timeRangeType + '">' + item.timeRange + '</span>';
                    tr += '</span> ';
                    tr += '<span class="modal-items-item">';
                    tr += '<span class="modal-items-item-name">' + item.timeRangeType + '播放次数：</span > ';
                    tr += '<span class="modal-items-item-value" id="info_count_' + item.timeRangeType + '">' + item.exposureCount + '次</span>';
                    tr += '</span> ';
                    $("#modal_timerangeinfo").append(tr);
                })
            }
        }
    })

    $("#InfoModal11").modal("show");      
}

function getInfo_12(id) {
    $.ajax({
        type: "Get",
        url: "/Order/GetOrder?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            if (data.result = "Success") {
                $("#modal_name12").text(data.order.name);
                $("#modal_totaltime12").text(data.order.totalSeconds + "秒");
                $("#modal_industry12").text(data.order.industry);
                $("#modal_remarks12").text(data.order.remarks);
                $("#modal_totalAmount12").text(data.order.amount);
                $("#modal_daterange12").text(data.order.startDate.substring(0, 10) + ' 至 ' + data.order.endDate.substring(0, 10));
                var programId = data.order.orderPlayerPrograms[0].playerProgram.programId;
                $.ajax({
                    type: "Get",
                    url: "/Program/Get",
                    data: { id: programId },
                    success: function (data) {
                        $("#modal_program12").text(data.displayName);
                    }
                })    
            }
        }
    })
    $.ajax({
        type: "Get",
        url: "/Order/GetOrderPlayers?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            if (data.result = "Success") {
                var players = [];
                $.each(data.orderplayers, function (i, item) {
                    var player = item.equipment.placeDto.name;
                    if (player != "" || player != null) {
                        players.push(player);
                    }
                })
                $("#modal_playernum12").text(players.length);
                $("#modal_places12").text(players.join(" , "));
            }
         }
    })
    /*$.ajax({
        type: "Get",
        url: "/Order/GetOrderEquipments?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            if (data.result = "Success") {
                var equipments = [];
                $.each(data.orderequipments, function (i, item) {
                    var equipment = item.placeDto.name;
                    if (equipment != "" || equipment != null) {
                        equipments.push(equipment);
                    }
                })
                $("#modal_places12").text(equipments.join(" , "));
            }
        }
    })*/

    $("#InfoModal12").modal("show");
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
            url: "/Order/DeleteMulti",
            data: sendData,
            success: function (data) {
                if (data.result == "Success") {
                    loadTables(1, pageSize);
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
            url: "/Order/DeleteWithPlayerProgram",
            data: { "id": id },
            success: function (data) {
                if (data.result == "Success") {
                    loadTables(1, pageSize)
                    layer.closeAll();
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        })
    });
};

//退款
function refund(id) {
    $.ajax({
        type: "Get",
        url: "/Order/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#OrderId").val(data.id);
            $("#Name").val(data.name);
            $("#Amount").val(data.amount);
            $("#ApplyRefundReason").val("");

            $("#orderRefundModal").modal("show");
        }
    })
};

//申请退款
function applyRefund() {
    $.post("/Order/ApplyRefund", {
        orderId: $("#OrderId").val(),
        applyRefundReason: $("#ApplyRefundReason").val()
    }, function (data) {
        if (data.result == "Success") {
            $("#orderRefundModal").modal("hide");
            loadTables(1, pageSize)
            layer.closeAll();
        }
        else {
            layer.alert(data.message);
        }
    });
};

//付款
function pay(orderNo, amount, subject, orderId) {
    $('#totalAmout').val(amount);
    $('#orderNo').val(orderNo);
    $('#subject').val(subject);
    $('#payOrderId').val(orderId);    
    $("#tradeMethodModal").modal();
};

//取消退款
function cancelRefund(id) {
    layer.confirm('是否取消退款？', {
        btn: ['是', '否'] //按钮
    }, function () {
        $.ajax({
            type: "POST",
            url: "/Order/CancelRefund?&_t=" + new Date().getTime(),
            data: { orderId: id },
            success: function (data) {
                if (data.result == 'Success') {
                    loadTables(1, pageSize);
                    layer.closeAll();
                } else {
                    layer.alert(data.message);
                }
            }
        })
    },
        function () {
            layer.closeAll();
        });
}
