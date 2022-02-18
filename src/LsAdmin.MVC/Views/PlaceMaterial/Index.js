var pageSize = 5;
var selectpageSize = 5;
$(function () {
    $("#btnAdd").click(function () { add(); });
    $("#btnDelete").click(function () { deleteMulti(); });
    $("#btnSave").click(function () { save(); });
    /*$("#btnShow").click(function () { loadselectTables(); });*/
    $("#btnSaveSelect").click(function () { saveSelect(); });
    $("#btnDeleteSelect").click(function () { deleteSelect(); });
    $("#checkAll").click(function () { checkAll(this) });
    $("#selectcheckAll").click(function () { selectcheckAll(this) });

    $("#Dir").fileinput({
        uploadUrl: "/Material/Add",
        enctype: 'multipart/form-data',
        allowedFileExtensions: ['mp4', '3gp', 'jpg', 'png'],//接收的文件后缀
        language: "zh"
    });

    //导入文件上传完成之后的事件
    $("#Dir").on("fileuploaded", function (event, data, previewId, index) {
        debugger
        var data = data.response;
        if (data.result != "Success") {
            toastr.error('上传失败！' + data.message);
            return;
        }
        loadTables(1, 5)
        $("#uploadModal").modal("hide");
    });

    loadTables(1, pageSize);
    loadselectTables(1, selectpageSize);
});

//加载所有上传素材
function loadTables(startPage, pageSize) {
    $("#tableBody").html("");
    $("#checkAll").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/Material/GetAllPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                var tr = "<tr>";
                tr += "<td align='center'><input type='checkbox' class='checkboxs' value='" + item.id + "'/></td>";
                tr += "<td>" + (item.name == null ? "" : item.name) + "</td>";
                tr += "<td>" + (item.remarks == null ? "" : item.remarks) + "</td>";
                tr += "<td>" +
                    "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'play(\"" + item.id + "\")' > <i class='fa fa-play'></i> 播放 </button >" +
                    "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'edit(\"" + item.id + "\")' > <i class='fa fa-edit'></i> 编辑备注 </button >" +
                    "<button class='btn btn-danger btn-xs' href='javascript:;' onclick='deleteSingle(\"" + item.id + "\")'><i class='fa fa-trash-o'></i> 删除 </button>" +
                    "<button class='btn btn-info btn-xs' href='javascript:;' onclick='select(\"" + item.id + "\")'><i class='fa fa-plus-square'></i> 添加播放列表 </button>" +
                    "</td > "
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

//上传选中素材
function select(id) {

    var ids = "";
    $(".checkboxs").each(function () {
        if ($(this).prop("checked") == true) {
            ids += $(this).val() + ","
        }
    });
    ids = ids.substring(0, ids.length - 1);
    if (ids.length == 0) {
        layer.alert("请选择要添加的记录。");
        return;
    };

    layer.confirm("您确认想要添加选定记录到播放列表吗？", {
        btn: ["确定", "取消"]
    }, function () {
        $.ajax({
            type: "POST",
            url: "/PlaceMaterial/Add",
            data: { "id": id },
            success: function (data) {
                if (data.result == "Success") {
                    layer.alert("添加成功！")
                    loadselectTables(1, 5);
                }
                else {
                    layer.alert("添加失败！请勿重复添加同名文件！");
                }
            }
        })
    });
};

//加载所有播放素材
function loadselectTables(selectstartPage, selectpageSize) {
    $("#selecttableBody").html("");
    $("#selectcheckAll").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/PlaceMaterial/GetAllPageList?startPage=" + selectstartPage + "&pageSize=" + selectpageSize + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                var tr = "<tr>";
                tr += "<td align='center'><input type='checkbox' class='selectcheckboxs' value='" + item.id + "'/></td>";
                tr += "<td>" + (item.materialName == null ? "" : item.materialName) + "</td>";
                tr += "<td>" + (item.remarks == null ? "" : item.remarks) + "</td>";
                tr += "<td>" +
                    "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'editSelect(\"" + item.id + "\")' > <i class='fa fa-edit'></i> 编辑备注 </button >" +
                    "<button class='btn btn-danger btn-xs' href='javascript:;' onclick='deleteSelectSingle(\"" + item.id + "\")'><i class='fa fa-trash-o'></i> 删除 </button>" +
                    "</td > "
                tr += "</tr>";
                tr += "</tr>";
                $("#selecttableBody").append(tr);
            })
            var selectelment = $("#selectgrid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: selectstartPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadselectTables(newPage, selectpageSize);
                    }
                }
                selectelment.bootstrapPaginator(options); //分页插件初始化
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

//全选播放
function selectcheckAll(obj) {
    $(".selectcheckboxs").each(function () {
        if (obj.checked == true) {
            $(this).prop("checked", true)

        }
        if (obj.checked == false) {
            $(this).prop("checked", false)
        }
    });
};

//新增
function add() {
    $("#Title").text("新增媒体");
    //弹出新增窗体
    $("#uploadModal").modal("show");
};
//编辑
function edit(id) {
    $.ajax({
        type: "Get",
        url: "/Material/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#Id").val(data.id);
            $("#Name").val(data.name);
            $("#OwnerUserId").val(data.ownerUserId);
            $("#Remarks").val(data.remarks);
            $("#Title").text("编辑备注")
            $("#editModal").modal("show");
        }
    })
};
//保存
function save() {
    var postData = { "dto": { "Id": $("#Id").val(), "Name": $("#Name").val(), "OwnerUserId": $("#OwnerUserId").val(), "Remarks": $("#Remarks").val() } };
    $.ajax({
        type: "Post",
        url: "/Material/Edit",
        data: postData,
        success: function (data) {
            if (data.result == "Success") {
                loadTables(1, pageSize)
                $("#editModal").modal("hide");
            } else {
                layer.tips(data.message, "#btnSave");
            };
        }
    });
};
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
            url: "/Material/DeleteMuti",
            data: sendData,
            success: function (data) {
                if (data.result == "Success") {
                    loadTables(1, pageSize)
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
            url: "/Material/Delete",
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

//播放
function play(id) {
    //弹出新增窗体
    $("#playModal").modal("show");
    $("#playModal").find("source").attr("src", "/Material/PlayAsync?id=" + id);
};

//编辑播放素材
function editSelect(id) {
    $.ajax({
        type: "Get",
        url: "/PlaceMaterial/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#Id").val(data.id);
            $("#MaterialName").val(data.materialName);
            $("#SelectRemarks").val(data.remarks);
            $("#SelectTitle").text("编辑备注")
            $("#selecteditModal").modal("show");
        }
    })
};
//保存播放素材
function saveSelect() {
    var postData = { "dto": { "Id": $("#Id").val(), "MaterialName": $("#MaterialName").val(), "Remarks": $("#SelectRemarks").val() } };
    $.ajax({
        type: "Post",
        url: "/PlaceMaterial/Edit",
        data: postData,
        success: function (data) {
            if (data.result == "Success") {
                loadselectTables(1, pageSize)
                $("#selecteditModal").modal("hide");
            } else {
                layer.tips(data.message, "#btnSaveSelect");
            };
        }
    });
};

//删除想要播放的素材
function deleteSelect() {
    var ids = "";
    $(".selectcheckboxs").each(function () {
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
            url: "/PlaceMaterial/Delete",
            data: sendData,
            success: function (data) {
                if (data.result == "Success") {
                    loadselectTables(1, pageSize)
                    layer.closeAll();
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        });
    });
};
//删除单条想要播放记录
function deleteSelectSingle(id) {
    layer.confirm("您确认删除选定的记录吗？", {
        btn: ["确定", "取消"]
    }, function () {
        $.ajax({
            type: "POST",
            url: "/PlaceMaterial/DeleteSingle",
            data: { "id": id },
            success: function (data) {
                if (data.result == "Success") {
                    loadselectTables(1, pageSize)
                    layer.closeAll();
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        })
    });
};