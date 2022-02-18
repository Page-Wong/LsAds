var selectTypeId = "00000000-0000-0000-0000-000000000000";
var selectedPlaceId = "00000000-0000-0000-0000-000000000000";
var pageSize = 5;
var selectPageNo=1;

$(function () {
    initPlaceTypeTabs();
    $("#btnAdd").click(function () { add(); });
    $("#btnSave").click(function () { save(); });
    $("#btnDelete").click(function () { deleteMulti(); });
    $("#checkAll").click(function () { checkAll(this) });

    $("#btnAddMaterial").click(function () { addMaterial(); });
    $("#btnSaveMaterial").click(function () { saveMaterial(); });
    $("#btnDeleteMaterial").click(function () { deleteMultiMaterial(); });
    $("#checkAllMaterial").click(function () { checkAllMaterial(this) });

    $("#PlaceTag").select2();
    $("#AdsWhiteTag").select2();
    $("#AdsBlackTag").select2();
    $("#TimeRange").select2();

    $("#Dir").fileinput({
        uploadUrl: "/Place/AddMaterial",
        enctype: 'multipart/form-data',
        allowedFileExtensions: ['mp4', '3gp', 'avi', 'jpeg', 'bmp', 'jpg', 'png'],//接收的文件后缀
        language: "zh",
        maxFileCount: 10,
        uploadExtraData: function (previewId, index) {
            if (!previewId) {
                return;
            }
            var obj = {}
            obj.ownerObjId = selectedPlaceId;
            return obj;
        }
    });

    //导入文件上传完成之后的事件
    $("#Dir").on("fileuploaded", function (event, data, previewId, index) {
       
        var data = data.response;
        if (data.result != "Success") {
            toastr.error('上传失败！' + data.message);
            return;
        }
        $("#materialModal").modal("hide");
        $('#Dir').fileinput('reset');
        loadList(selectPageNo, pageSize, selectTypeId);
    });

});
//全选场所
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

//PlaceTypeTabs
//初始化场所类型页签
function initPlaceTypeTabs() {
    $("#PlaceTypeTabs").html("");
    var li="";

    $.ajax({
        type: "GET",
        url: "/PlaceType/GetGridData",  //获取数据的ajax请求地址
        success: function (data) {
            $.each(data, function (i, item) {
                if (i == 0) {
                    li += "<li  class='active'  onclick='loadList(\"1\", \"" + pageSize + "\", \"" + item.id +"\")'> <a role='tab' DATA-toggle='tab'><i class='fa fa- fa-bars'>" + item.text + "</i><span class='badge bg-black'  ></span></a></li >";
                    selectTypeId = item.id;
                 } else {
                    li += "<li   onclick='loadList(\"1\", \"" + pageSize + "\", \"" + item.id +"\")'> <a role='tab' DATA-toggle='tab'><i class='fa fa- fa-bars'>" + item.text + "</i><span class='badge bg-black' ></span></a></li >";
                }
            })    
            selectPageNo=1;
            $("#PlaceTypeTabs").append(li);  
            loadList(selectPageNo, pageSize, selectTypeId)
        }
    })  
    var li_button = "<li class='pull-right' id='btnAdd' ><a><i class='fa fa-plus-square'></i>新增场所</a></li>";

    $("#PlaceTypeTabs").append(li_button);   
}

// 加载场所列表数据
function loadList(startPage, pageSize, typeId) {
    $("#Placelist").html("");
    $("#list_paging_part").html("");
    selectTypeId = typeId;
    $.ajax({
        type: "GET",
        url: "/Place/GetPlaceByType?startPage=" + startPage + "&pageSize=" + pageSize + "&typeId=" + typeId + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                var _placeId = item.id;

                var Materials;
                $.ajax({
                    type: "GET",
                    url: "/Place/GetMaterialPageList?OwnerObjId=" + item.id + "&startPage=1&pageSize=50&_t=" + new Date().getTime(),
                    cache: true,
                    async: false,
                    success: function (data) {
                    Materials = data;
                    }
                })

                var placeli = "";
                placeli += "<li class='item' >";

                placeli += "        <div class='col-md-2' >";
                placeli += "            <div class='box box-solid'>";
                placeli += "                <div class='box-body'>";
                placeli += "                    <div id='carousel-example-generic" + item.id + "' class='carousel slide' data-ride='carousel'>";

                placeli += "                        <ol class='carousel-indicators'>";
   
                if (Materials.rowCount > 0) {
                    $.each(Materials.rows, function (i, item) {
                        if (i == 0) {
                            placeli += "                            <li data-target='#carousel-example-generic' data-slide-to='" + i + "' class='active'></li>";
                        } else {
                            placeli += "                            <li data-target='#carousel-example-generic' data-slide-to='" + i + "' class=''></li>";
                        }
                    });
                }

                placeli += "                        </ol>";

                placeli += "                        <div class='carousel-inner' >";

                if (Materials.rowCount >0) {
                    $.each(Materials.rows, function (i, item) {
                        if (i == 0) {
                            placeli += "                            <div class='item active' style='height:160px'>";
                        } else {
                            placeli += "                            <div class='item' style='height:160px;text-align:center;'>";
                        }
                        placeli += "                                <img style='height:150px' src='/Place/GetThumbnail?id=" + item.id + "'/>";

                        placeli += "<div class='tools'>";
                      //  placeli += " <button type='button' class='btn btn-info btn-xs'  onclick= 'editMaterial(\"" + item.id + "\")'><i class='fa fa-edit'></i>编辑备注</button>";
                        placeli += " <button class='btn btn-primary btn-xs'  onclick='addMaterial(\"" + _placeId + "\")'><i class='fa fa-arrow-circle-up'></i> 上传素材 </button>";
                        placeli += " <button class='btn btn-danger btn-xs'   onclick='deleteSingleMaterial(\"" + item.id + "\")'><i class='fa fa-trash-o'></i>删除素材</button>";
                        placeli += "</div>";

                        placeli += "                            </div>";
                    });
                } else {
                    placeli += "                            <div class='item active' style='height:160px;text-align:center;'>";
                    placeli += "                                <img  style='height:150px' src='/Place/GetThumbnail?id=" + item.id + "'/>"
                    placeli += "                            </div>";
                    placeli += "<div class='tools'>";             
                    placeli += " <button class='btn btn-primary btn-xs'  onclick='addMaterial(\"" + _placeId + "\")'><i class='fa fa-arrow-circle-up'></i> 上传素材 </button>";
                    placeli += "</div>";
                }

                placeli += "                        </div>";

                placeli += "                        <a class='left carousel-control' href='#carousel-example-generic" + item.id + "' data-slide='prev'>";
                placeli += "                            <span class='fa fa-angle-left'></span>";
                placeli += "                        </a>";
                placeli += "                        <a class='right carousel-control' href='#carousel-example-generic" + item.id + "' data-slide='next'>";
                placeli += "                            <span class='fa fa-angle-right'></span>";
                placeli += "                        </a>";
                placeli += "                    </div>";
                placeli += "                </div>";
                placeli += "            </div>";
                placeli += "        </div>";

                //placeli += "                            <div class='item' style='height:160px'>";
                //placeli += "                                <img src='http://placehold.it/900x500/3c8dbc/ffffff&text=I+Love+Bootstrap' alt='Second slide'>";
                //placeli += "                            </div>";
               
                placeli += "    <div class='product-info'>";
                placeli += "  <div><a class='products-title  btn-link'>场所：" + (item.name == null ? "" : item.name) + " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </a>地址: " + (item.address == null ? "" : item.address) + "</div>";
                placeli += "  <div> 负责人: " + (item.contact == null ? "" : item.contact) + " &nbsp;&nbsp; 电话：" + (item.phone == null ? "" : item.phone) + " &nbsp;&nbsp; <i class='label label-info'>" + (item.placeTag == null ? "" : item.placeTag) + "</i> <div>";

                placeli += "              <div>可播放广告时间段：" + (item.timeRange == null ? "" : item.timeRange) + "</div>";
                placeli += "        <div> 黑名单：" + (item.adsBlackTag == null ? "" : item.adsBlackTag) + "</div>";
                placeli += "        <div>  备注：" + (item.remarks == null ? "" : item.remarks) + "</div>";
                placeli += "        <div> 场所介绍：<small> " + (item.introduction == null ? "" : item.introduction) + "</small></div>";
                placeli += "    </div>";

                placeli += "    <div class='tools'>";
                placeli += "        <button class='btn btn-info btn-sm' href='javascript:;' onclick='edit(\"" + item.id + "\")'><i class='fa fa-edit'></i> 编辑 </button>";
                //placeli += "        <button class='btn btn-primary btn-xs' href='javascript:;' onclick='addMaterial(\"" + item.id + "\")'><i class='fa fa-arrow-circle-up'></i> 上传素材 </button>";
                placeli += "        <button class='btn btn-danger btn-sm' href='javascript:;' onclick='deleteSingle(\"" + item.id + "\")'><i class='fa fa-trash-o'></i> 删除 </button>";

                placeli += "    </div>";
                placeli += "</li>";
                $("#Placelist").append(placeli);
            })
            var elment = $("#list_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadList(newPage, pageSize, selectTypeId);
                        selectPageNo=newPage;
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
            $("#Placelist > li").click(function () {
                $("#Placelist > li").removeAttr("style")
                $(this).attr("style", "background-color:#beebff");
            });
            loadPlaceTags(data);
            loadAdsWhiteTags(data);
            loadAdsBlackTags(data);
        }
    })


}

//场所标签选择框
function loadPlaceTags(data) {
    var option = "";
    $.each(data.placetags, function (i, item) {
        option += "<option>" + item.name + "</option>"
    })
    $("#PlaceTag").html(option);
}

//广告白标签选择框
function loadAdsWhiteTags(data) {
    var option = "";
    /*alert(data.labels)*/
    $.each(data.adstags, function (i, item) {
        option += "<option>" + item.name + "</option>"
    })
    $("#AdsWhiteTag").html(option);
}

//广告黑标签选择框
function loadAdsBlackTags(data) {
    var option = "";
    $.each(data.adstags, function (i, item) {
        option += "<option>" + item.name + "</option>"
    })
    $("#AdsBlackTag").html(option);
}

//新增场所
function add() {
    $("#Id").val("00000000-0000-0000-0000-000000000000");
    $("#Name").val("");
    $("#Province").val("");
    $("#City").val("");
    $("#District").val("");
    $("#Street").val("");
    $("#Introduction").val("");
    $("#Phone").val("");
    $("#Contact").val("");
    $("#PlaceTag").select2("val", "");
   /* $("#AdsWhiteTag").select2("val", "");*/
    $("#AdsBlackTag").select2("val", "");
    $("#TimeRange").select2("val", "");
    $("#Remarks").val("");
    $("#Title").text("新增场所");
    //弹出新增窗体
    $("#editModal").modal("show");
};

//编辑场所
function edit(id) {
    $.ajax({
        type: "Get",
        url: "/Place/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#Id").val(data.id);
            $("#Name").val(data.name);
            $("#Province").val(data.province);
            loadCity(data.province, data.city);
            loadDistrict(data.city, data.district)
            $("#Street").val(data.street);
            $("#Introduction").val(data.introduction);
            $("#Phone").val(data.phone);
            $("#Contact").val(data.contact);
            $("#PlaceTag").select2("val", data.placeTag ? data.placeTag.split(',') : '');
         /* $("#AdsWhiteTag").select2("val", data.adsWhiteTag ? data.adsWhiteTag.split(',') : '');*/
            $("#AdsBlackTag").select2("val", data.adsBlackTag ? data.adsBlackTag.split(',') : '');
            $("#TimeRange").select2("val", data.timeRange ? data.timeRange.split(',') : ''); 
            $("#Remarks").val(data.remarks);
            $("#Title").text("编辑场所");
            $("#editModal").modal("show");
        }
    })
};

//保存场所
function save() {
   var postData = {
        "dto": {
            "Id": $("#Id").val(),
            "Name": $("#Name").val(),
            "Province": $("#Province").val(),
            "City": $("#City").val(),
            "District": $("#District").val(),
            "Street":$("#Street").val(),
            "Introduction": $("#Introduction").val(),
            "Phone": $("#Phone").val(),
            "Contact": $("#Contact").val(),
            "PlaceTag": $("#PlaceTag").val().toString(),
          /*  "AdsWhiteTag": $("#AdsWhiteTag").val().toString(),*/
            "AdsBlackTag": $("#AdsBlackTag").val().toString(),
            "TimeRange": $("#TimeRange").val().toString(),
            "Remarks": $("#Remarks").val(),
            "TypeId": selectTypeId    
       }
    };


    $.ajax({
        type: "Post",
        url: "/Place/Edit",
        data: postData,
        success: function (data) {
            if (data.result == "Success") {
                $("#editModal").modal("hide");
                loadList(selectPageNo, pageSize, selectTypeId)
            } else {
                layer.tips(data.message, "#btnSave");
            };
        }
    });
    
};

//批量删除场所
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
            url: "/Place/DeleteMuti",
            data: sendData,
            success: function (data) {
                if (data.result == "Success") {
                    selectPageNo=1;
                    loadList(selectPageNo, pageSize, selectTypeId)
                    layer.closeAll();
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        });
    });
};

//删除单条场所
function deleteSingle(id) {
    layer.confirm("您确认删除选定的记录吗？", {
        btn: ["确定", "取消"]
    }, function () {
        $.ajax({
            type: "POST",
            url: "/Place/Delete",
            data: { "id": id },
            success: function (data) {
                if (data.result == "Success") {
                     selectPageNo=1;
                     loadList(selectPageNo, pageSize, selectTypeId)
                    layer.closeAll();
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        })
    });
};



//全选素材
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

//新增素材
function addMaterial(id) {
    selectedPlaceId = id;
    $("#Title").text("新增素材");
    //弹出新增窗体
    $("#materialModal").modal("show");
};

//编辑素材备注
function editMaterial(id) {
    $.ajax({
        type: "Get",
        url: "/Place/GetMaterial?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#MaterialId").val(data.id);
            $("#MaterialName").val(data.name);
            $("#MaterialRemarks").val(data.remarks);
            $("#Title").text("编辑备注");
            $("#editMaterialModal").modal("show");
        }
    })
};

//保存素材
function saveMaterial() {
    var postData = {
        "dto": {
            "Id": $("#MaterialId").val(),
            "Name": $("#MaterialName").val(),
            "Remarks": $("#MaterialRemarks").val(),
        }
    };

    $.ajax({
        type: "Post",
        url: "/Place/EditMaterial",
        data: postData,
        success: function (data) {
            if (data.result == "Success") {
                $("#editMaterialModal").modal("hide");
               loadList(selectPageNo, pageSize, selectTypeId)
            } else {
                layer.tips(data.message, "#btnSaveMaterial");
            };
        }
    });

};

//批量删除素材
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
            url: "/Place/DeleteMutiMaterial",
            data: sendData,
            success: function (data) {
                if (data.result == "Success") {   
                    selectPageNo=1;
                    loadList(selectPageNo, pageSize, selectTypeId)
                    layer.closeAll();
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        });
    });
};
//删除单条素材
function deleteSingleMaterial(id) {
    layer.confirm("您确认删除选定的记录吗？", {
        btn: ["确定", "取消"]
    }, function () {
        $.ajax({
            type: "POST",
            url: "/Place/DeleteMaterial",
            data: { "id": id },
            success: function (data) {
                if (data.result == "Success") {
                    loadList(selectPageNo, pageSize, selectTypeId)
                    layer.closeAll();
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        })
    });
};
