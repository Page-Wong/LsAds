var selectedRole = 0;
var pageSize = 15;
$(function () {
    /*$('#mediaTable').DataTable({
        "processing": true,
        "serverSide": true,
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "remarks" }
        ],
        "ajax": "/Material/GetAllPageList"
    });*/
    $("#btnAddMaterial").click(function () { addMaterial(); });
    $("#btnDeleteMaterial").click(function () { deleteMultiMaterial(); });
    $("#btnSaveMaterial").click(function () { saveMaterial(); });
    //$("#btnSavePermission").click(function () { savePermission(); });
    $("#checkAllMaterial").click(function () { checkAllMaterial(this) });

    $("#Dir").fileinput({
        uploadUrl: "/Material/Add",
        enctype: 'multipart/form-data',
        allowedFileExtensions: ['mp4', '3gp', 'avi', 'jpeg', 'bmp', 'jpg', 'png'],//接收的文件后缀
        language: "zh",
        maxFileCount: 10,  
        uploadExtraData: function (previewId, index) {
            if (!previewId) {
                return;
            }
            var obj = {};
            if ($('#' + previewId).find('video')[0] == null) {
                return  obj.duration = 0;
            }
            var duration = $('#' + previewId).find('video')[0].duration;
            obj.duration = duration;

            return obj;
        }
    });

    //导入文件上传完成之后的事件
    $("#Dir").on("fileuploaded", function (event, data, previewId, index) {
        debugger
        var data = data.response;
        if (data.result != "Success") {
            toastr.error('上传失败！' + data.message);
            return;
        }
        loadTablesMaterial(1, 10)
        $("#uploadModal").modal("hide");
        $('#Dir').fileinput('reset');
    });

    
    //initTree();
    loadTablesMaterial(1, pageSize);
});

//加载列表数据
function loadTablesMaterial(startPage, pageSize) {
    $("#tableBody").html("");
    $("#checkAllMaterial").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/Material/GetAllPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                var tr = "<tr>";  
                tr += "<td>" + (i + 1) + "</td>";
                tr += "<td align='center'><input type='checkbox' class='checkboxs' value='" + item.id + "'/></td>";
                tr += "<td>" + (item.name == null ? "" : item.name) + "</td>";
                tr += "<td>" + (item.materialType == 1 ? "图片" : "视频") + "</td>";
                tr += "<td>" + (item.duration == null || item.duration == "0"? "" : item.duration + "秒") + "</td>";
                tr += "<td>" + (item.remarks == null ? "" : item.remarks) + "</td>"; 
                tr += "<td>" +                                                                                
                    "<button class='btn btn-info btn-xs'   href= 'javascript:;' onclick =\"playMaterial('" + item.id + "','" + item.materialType + "')\"> <i class='fa fa-edit'></i> 查看 </button >" +
                    "<button class='btn btn-info btn-xs'   href= 'javascript:;' onclick= 'editMaterial(\"" + item.id + "\")' > <i class='fa fa-edit'></i> 编辑备注 </button >" +
                    "<button class='btn btn-danger btn-xs' href='javascript:;'  onclick='deleteSingleMaterial(\"" + item.id + "\")'><i class='fa fa-trash-o'></i> 删除 </button>" +
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
                        loadTablesMaterial(newPage, pageSize);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
            $("table > tbody > tr").click(function () {
                $("table > tbody > tr").removeAttr("style")
                $(this).attr("style", "background-color:#beebff");
                //selectedRole = $(this).find("input").val();
                //loadPermissionByRole(selectedRole);
            });
        }
    })
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
//新增
function addMaterial() {
    /*$("#Id").val("00000000-0000-0000-0000-000000000000");
    $("#Dir").val("");
    $("#Name").val("");
    $("#Remarks").val("");*/
    $("#Title").text("新增媒体");
    //弹出新增窗体
    $("#uploadModal").modal("show");
};
//编辑
function editMaterial(id) {
    $.ajax({
        type: "Get",
        url: "/Material/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#Id").val(data.id);
            $("#Name").val(data.name);
            $("#OwnerUserId").val(data.ownerUserId);
            $("#Remarks").val(data.remarks);

            $("#Title").text("编辑媒体")
            $("#editModal").modal("show");
        }
    })
};
//保存
function saveMaterial() {
    var postData = { "dto": { "Id": $("#Id").val(), "Name": $("#Name").val(), "OwnerUserId": $("#OwnerUserId").val(), "Remarks": $("#Remarks").val() } };
    $.ajax({
        type: "Post",
        url: "/Material/Edit",
        data: postData,
        success: function (data) {
            if (data.result == "Success") {
                loadTablesMaterial(1, pageSize)
                $("#editModal").modal("hide");
            } else {
                layer.tips(data.message, "#btnSave");
            };
        }
    });
};
//批量删除
function deleteMultiMaterial() {
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
                    loadTablesMaterial(1, pageSize)
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
function deleteSingleMaterial(id) {
    layer.confirm("您确认删除选定的记录吗？", {
        btn: ["确定", "取消"]
    }, function () {
        $.ajax({
            type: "POST",
            url: "/Material/Delete",
            data: { "id": id },
            success: function (data) {
                if (data.result == "Success") {
                    loadTablesMaterial(1, pageSize)
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
function playMaterial(id,materialtype) {
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