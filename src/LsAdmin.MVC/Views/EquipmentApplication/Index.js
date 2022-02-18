﻿var pageSize = 15;
var selectedId = "00000000-0000-0000-0000-000000000000";
$(function () {
    $("#btnAdd").click(function () { add(); });
    $("#btnSave").click(function () { save(); });
    $("#checkAll").click(function () { checkAll(this); });

    initTree();
});

//加载场所类型树
function initTree() {
    $.jstree.destroy();
    $.ajax({
        type: "Get",
        url: "/Place/GetGridData",    //获取数据的ajax请求地址
        success: function (data) {
            $('#treeDiv').jstree({       //创建JsTtree
                'core': {
                    'data': data,        //绑定JsTree数据
                    "multiple": false    //是否多选
                },
                "plugins": ["state", "types", "wholerow"]  //配置信息
            });
            $("#treeDiv").on("ready.jstree", function (e, data) {   //树创建完成事件
                data.instance.open_all();    //展开所有节点
            });
            $("#treeDiv").on('changed.jstree', function (e, data) {   //选中节点改变事件
                var node = data.instance.get_node(data.selected[0]);  //获取选中的节点
                if (node) {
                    selectedId = node.id;
                    loadTables(1, 10);
                }
            });
        }
    });

}

//加载列表数据
function loadTables(startPage, pageSize) {
    $("#tableBody").html("");
    $("#checkAll").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/EquipmentApplication/GetEquipmentApplicationByPlace?startPage=" + startPage + "&pageSize=" + pageSize + "&placeId=" + selectedId + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                var tr = "<tr>";
                tr += "<td align='center'><input type='checkbox' class='checkboxs' value='" + item.id + "'/></td>";
                tr += "<td>" + (item.reason == null ? "" : item.reason) + "</td>";
                tr += "<td>" + (item.createTime == null ? "" : item.createTime) + "</td>";
                tr += "<td>" + (item.statusString == null ? "" : item.statusString) + "</td>";
                tr += "<td>" + (item.progress == null ? "" : item.progress) + "</td>";
                if(item.statusString == '待确认')
                {
                tr += "<td>" +
                    "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'edit(\"" + item.id + "\")' > <i class='fa fa-edit'></i> 修改申请原因 </button ></td > ";
            
                }
                else {
                 tr += "<td>" +
                     "<button class='btn disabled btn-xs'> <i class='fa fa-edit'></i> 修改申请原因 </button ></td > ";
                }
                
                tr += "</tr>";
                $("#tableBody").append(tr);
            });
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
                };
                elment.bootstrapPaginator(options); //分页插件初始化
            }
            $("table > tbody > tr").click(function () {
                $("table > tbody > tr").removeAttr("style");
                $(this).attr("style", "background-color:#beebff");

            });
        }
    });
}

//全选
function checkAll(obj) {
    $(".checkboxs").each(function () {
        if (obj.checked == true) {
            $(this).prop("checked", true);

        }
        if (obj.checked == false) {
            $(this).prop("checked", false);
        }
    });
}

//新增
function add() {
    $("#Id").val("00000000-0000-0000-0000-000000000000");
    $("#Reason").val("");
    $("#Title").text("填写申请设备信息");
    //弹出新增窗体
    $("#editModal").modal("show");
};

//编辑
function edit(id) {
    $.ajax({
        type: "Get",
        url: "/EquipmentApplication/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#Id").val(data.id);
            $("#Reason").val(data.reason);
            $("#Status").val(data.status);
            $("#Title").text("修改申请设备信息");
            $("#editModal").modal("show");
        }
    });
}

//保存
function save() {
    var postData = {
        "dto": {
            "Id": $("#Id").val(),
            "Reason": $("#Reason").val(),
            "Status": $("#Status").val(),
            "PlaceId": selectedId    
        }
    };
    $.ajax({
        type: "Post",
        url: "/EquipmentApplication/Edit",
        data: postData,
        success: function (data) {
            if (data.result == "Success") {
                loadTables(1, pageSize);
                $("#editModal").modal("hide");
            } else {
                layer.tips(data.message, "#btnSave");
            }
        }
    });
} 