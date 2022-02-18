var pageSize = 15;
var selectedIds = "";
var RecordNO = 0;
var materialtotaltime = 0;
var materialcount = 0;

var maxmaterialtotaltime = 60;
var maxmaterialcount = 10;
var maxmonepicturetime = 5;

var isChange = false;


var selectedId = "00000000-0000-0000-0000-000000000000";
$(function () {
    initTree();
    $("input[name='MateralType']").change(function () {
        GetPlaceMaterial(1, 20, selectedId, $(this).val());// loadTables(1, pageSize, $(this).val());
    });
    $("#checkAll").click(function () { checkAll(this); });
    $("#btnShowMateralSelectModal").click(function () { loadTables(1, 20); });
    $("#btnUploadMaterial").click(function () { $("#uploadModal").modal("show");});
    $("#btnDeleteMulti").click(function () { deleteMulti(); });
    $("#btnSavePlaceMaterials").click(function () { savePlaceMaterials(); });


    //导入文件上传完成之后的事件
    $("#Dir").on("fileuploaded", function (event, data, previewId, index) {
        var data = data.response;
        if (data.result != "Success") {
            layer.alert('上传失败！' + data.message);
            return;
        }
        else {

            var materialtype;
            var robj = document.getElementsByName("MateralType");
            for (i = 0; i < robj.length; i++) {
                if (robj[i].checked) {
                    materialtype = robj[i].value;
                }
            }
            if (data.data.materialType == materialtype) {
                AddPlaceMaterial(data.data.id, data.data.name, materialtype, data.data.duration, data.data.remarks)
            }
            else {
                layer.alert("请上传正确的素材类型(素材已上传至我的推广素材，但无法选择为本次推广的播放素材。)");

            }
        }
        $("#uploadModal").modal("hide");
        $('#Dir').fileinput('reset');
    });

    ProgramSelectModal.init({
        selectCallback: function (id, displayName, duration, width, height) {
            alert(id);
            ProgramSelectModal.show(false);
        }
    });
    $('#testbtn').click(function () { ProgramSelectModal.show(true);})
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
                GetPlaceMaterial(1, 20, selectedId);
                /*
                if (GetPlaceMaterial(1, 20, selectedId)==0) {

                    var robj = document.getElementsByName("MateralType");
                    for (i = 0; i < robj.length; i++) {
                        if (robj[i].checked) {
                            $("#"+robj[i].id).attr("checked", false)                
                        } else {
                            $("#"+robj[i].id).attr("checked", true)
                        }
                    }
                    GetPlaceMaterial(1, 20, selectedId);
                }*/


                }             
            });
        }
    });

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

//计算素材总时间
function countMaterialtotaltime() {
     materialtotaltime = 0;
     materialcount = 0;
    var input_seconds = $('input[name=input_seconds]');
    $.each(input_seconds, function (i, item) {
        materialtotaltime += parseFloat($(item).val());
        materialcount++;
    });
    $("#input_materialtotaltime").html(materialtotaltime);
    $("#input_materialcount").html(materialcount);
    isChange = true;
}

//保存方案
function savePlaceMaterials() {
    if (selectedId == null || selectedId == "")
    {
        layer.alert('请选择场所！');
        return false;
    }
    var placeMaterials = [];
    var orderby = 0;
    $("#selectTableBody .materialTableBodyId").each(function () {
        var id = $(this).val();
        orderby += 1;

        placeMaterials.push({
            MaterialId: id,
            PlaceId: selectedId,
            Orderby:orderby,
            Remarks: $("#input_remarks_" + id).val(),
            Seconds:$("#input_seconds_" + id).val(),
        })
    });

    /*
    //播放素材不能为空
    if (placeMaterials.length == 0) {
        layer.alert('请选择推广需要的素材');
        return false;
    }
    */

    $.ajax({
        type: "POST",
        url: "/PlacePlayMyself/Save",
        data: { placeId: selectedId,dtos: placeMaterials},
        success: function (data) {
            if (data.result == "Success") {
                layer.alert('保存成功');
            }
            else {
                layer.alert('保存失败：'+data.message);
            }
        }
    });
    isChange = false;
}

//加载场所已选定素材列表项
function GetPlaceMaterial(startPage, pageSize, placeid, materialType) {
    //materialType = materialType ? materialType : $("input[name='MateralType'][checked]").val();
    var robj = document.getElementsByName("MateralType");
    for (i = 0; i < robj.length; i++) {
        if (robj[i].checked) {
            materialType = robj[i].value;
        }
    }
    $("#selectTableBody").html("");
    $("#checkAll").prop("checked", false);
    selectedIds = "";
    RecordNO = 0;

    $.ajax({
        type: "GET",
        url: "/PlacePlayMyself/GetAllPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&placeid=" + placeid + "&materialType=" + materialType + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                RecordNO++;
                AddPlaceMaterial(item.materialId, item.materialName, item.materialType, item.seconds, item.remarks);
            });
            countMaterialtotaltime();
            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        GetPlaceMaterial(newPage, pageSize, placeid, materialType);
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
    isChange = false;
    return (RecordNO); 
}

//加载场所素材库列表数据-排除已选择数据库
function loadTables(startPage, pageSize, materialType) {
   // materialType = materialType ? materialType : $("input[name='MateralType'][checked]").val();
    var robj = document.getElementsByName("MateralType");
    for (i = 0; i < robj.length; i++) {
        if (robj[i].checked) {
            materialType = robj[i].value;
        }
    }
    var isselectmaterial = [];
    $("#selectTableBody .materialTableBodyId").each(function () {     
        isselectmaterial.push($(this).val());
    });


    $("#tableBody").html("");
    $("#materialcheckAll").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/Material/GetAllPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&materialType=" + materialType + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                if ($.inArray(item.id, isselectmaterial) == -1) {
                var tr = "<tr id='id_" + item.id + "'>";
                tr += "<td align='center' hidden='hidden'><input type='checkbox' class='checkboxs' value='" + item.id + "' hidden/></td>";
                tr += "<td>" + (item.name == null ? "" : item.name) + "</td>";
                tr += "<td>" + (item.duration == null ? "" : item.duration) + "</td>";
                tr += "<td>" + (item.remarks == null ? "" : item.remarks) + "</td>";
                tr += "<td>" +
                    "<button class='btn btn-info btn-xs' href= 'javascript:;'  onclick =\"playMaterial('" + item.id + "','" + item.materialType + "')\" > <i class='fa fa-edit'></i> 查看 </button >" +
                    "<button class='btn btn-primary btn-xs' href='javascript:;' onclick='addMaterialListItem(\"" + item.id + "\",\"" + item.name + "\", \"" + item.materialType + "\",\"" + item.duration + "\", \"" + item.remarks + "\")'><i class='fa fa-send'></i> 选择 </button>" +
                    "</td > ";
                tr += "</tr>";
                $("#tableBody").append(tr);
            }
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
    });
}

//删除单条本订单素材
function deleteSingle(id) {
    $("#selectedid_" + id).remove();
    countMaterialtotaltime();
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
        $(".checkboxs").each(function () {
            if ($(this).prop("checked") == true) {
                $("#selectedid_" + $(this).val()).remove();
            }
        });
        countMaterialtotaltime();
          layer.closeAll();
        });
  
};

//增加播放素材列表项
function addMaterialListItem(materialId, materialName, materialType, seconds, remarks) {
    AddPlaceMaterial(materialId, materialName, materialType, seconds, remarks);

    if ($('#id_' + materialId)) {
        $('#id_' + materialId).remove();
    }
    countMaterialtotaltime();
}

function AddPlaceMaterial(materialId, materialName, materialType, seconds, remarks) {
    if (materialcount >= maxmaterialcount) {
        layer.alert("播放素材数量不能超过" + maxmaterialcount+"个!");
        return;
    };

   // var totaltime = parseInt(materialtotaltime) + parseInt(seconds);
    if (parseInt(materialtotaltime) + parseInt(seconds)>maxmaterialtotaltime) {
        layer.alert("播放素材总播放时长不能超过" + maxmaterialtotaltime + "秒!");
        return;
    }

    selectedIds += materialId;
     RecordNO += 1;
     var tr = "<tr id='selectedid_" + materialId + "'>";
     tr += "<td align='center' hidden='hidden'  ><input type='text' class='materialTableBodyId' value='" + materialId + "'/></td>";
     tr += "<td hidden='hidden'>" + RecordNO + "</td>";
    tr += "<td align='center'><input type='checkbox' class='checkboxs' value='" + materialId + "'/>";
    tr += "<td>" + (materialName == null ? "" : materialName) + "</td>";
    tr += "<td>" + (materialType == 1 ? "图片" : "视频") + "</td>";
    //tr += "<td>" + (item.placeId == null ? "0" : item.seconds) + "</td>";
    if (materialType == 1) {
        tr += "<td >" + "<input style='width: 50px'type='number' min='1' max='" + maxmonepicturetime+"' id='input_seconds_" + materialId + "' name='input_seconds' value='5'  onchange='countMaterialtotaltime()'>" + "</td>";
    }
    else //if(materialType == 2)
    {
        tr += "<td >" + "<input style='width: 50px' type='number'  id='input_seconds_" + materialId + "' name='input_seconds' value='" + seconds + "' disabled='disabled'>" + "</td>";
    }
    tr += "<td>" + "<input   type='text' id='input_remarks_" + materialId + "' name='input_remarks_' value='" + (remarks == null ? "" : remarks) + "' >"  + "</td>";
    tr += "<td>" +
        "<button class='btn btn-info btn-xs' href= 'javascript:;'  onclick =\"playMaterial('" + materialId + "','" + materialType + "')\" > <i class='fa fa-edit'></i> 查看 </button >" +
        "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'upTR(\"" + materialId + "\")' > <i class='fa fa-edit'></i> 上调 </button >" +
        "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'downTR(\"" + materialId + "\")' > <i class='fa fa-edit'></i> 下调</button >" +
        "<button class='btn btn-danger btn-xs' href='javascript:;' onclick='deleteSingle(\"" + materialId+ "\")'><i class='fa fa-trash-o'></i> 删除 </button>" +
    "</td > ";
    tr += "</tr>";
    $("#selectTableBody").append(tr);
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

