var pageSize = 10;
var getStatus = 0;
$(function () {
    $("#btnSave").click(function () { save(); });
   /* initTooltip();*/
    loadEquipmentList(1, pageSize, getStatus);

    $("#btnStatusUnInuse").click(function () { getStatus = 0; loadEquipmentList(1, pageSize, getStatus); });
    $("#btnStatusInuse").click(function () { getStatus = 1; loadEquipmentList(1, pageSize, getStatus); });
    $("#btnStatusRepairing").click(function () { getStatus = 2; loadEquipmentList(1, pageSize, getStatus); });
    $("#btnStatusScrap").click(function () { getStatus = 3; loadEquipmentList(1, pageSize, getStatus); });

   /*var id = $("#BeforeMaterialId").val;*/
    //上传文件
    $("#Dir").fileinput({
        uploadUrl: "/EquipmentRepairApply/AddMaterial",
        enctype: 'multipart/form-data',
        allowedFileExtensions: ['mp4', '3gp', 'avi', 'jpeg', 'bmp', 'jpg', 'png'],//接收的文件后缀
        language: "zh",
        maxFileCount: 10,
       uploadExtraData: function () {
            
            var obj = {}
            obj.ownerObjId = $("#BeforeMaterialId").val();
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
        layer.confirm("图片已上传成功，请勿重复上传相同图片！", {
            btn: ["确定", "取消"]
        });
        loadEquipmentList(1, pageSize, getStatus);
    });

});

//加载列表数据
function loadEquipmentList(startPage, pageSize, status) {
    $("#equipmentlist").html("");
    $.ajax({
        type: "GET",
        url: "/EquipmentRepairApply/GetPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&status=" + status + "&_t=" + new Date().getTime(),
        success: function (data) {

            $("#StatusUnInuse").html(data.statusUnInuse);
            $("#StatusInuse").html(data.statusInuse);
            $("#StatusRepairing").html(data.statusRepairing);
            $("#StatusScrap").html(data.statusScrap);

            $.each(data.rows, function (i, item) {
                var li = " <li class='item'>";
               li += "<div class='product-img'> <img src='/EquipmentModel/GetThumbnail?id=" + item.equipmentModelId + "'/></div>";
               li += "<div class='product-info'>";
               switch (item.status) {
                   case 0:
                       li += "<a id='em_" + item.id + "' class='products-title btn-link'>" + (item.name == null ? "" : item.name) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;";
                       break;
                   case 1:
                       li += "<a id='em_" + item.id + "' class='products-title btn-link'>" + (item.name == null ? "" : item.name) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;";
                       li += "<a  id='p_" + item.id + "' class='product-title btn-link'>" + (item.placeDto == null ? "" : item.placeDto.name) + "(" + (item.placeDto == null ? "" : item.placeDto.address) + ")</a>";
                       break;
                   case 2:
                       li += "<a id='em_" + item.id + "' class='products-title btn-link'>" + (item.name == null ? "" : item.name) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;";
                       li += "<a  id='p_" + item.id + "' class='product-title btn-link'>" + (item.placeDto == null ? "" : item.placeDto.name) + "(" + (item.placeDto == null ? "" : item.placeDto.address) + ")</a>";
                       break;
                   case 3:
                       li += "<a id='em_" + item.id + "' class='products-title btn-link'>" + (item.name == null ? "" : item.name) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;";
                       break;
                   default:
                      
               }

               switch (item.status) {
                   case 0:
                       li += "<small class='label label-default'>" + item.statusString + "</small>";
                       break;
                   case 1:
                       li += "<small class='label label-success'>" + "正常使用" + "</small>";
                       break;
                   case 2:
                       li += "<small class='label label-warning'>" + item.statusString + "</small>";
                       break;
                   case 3:
                       li += "<small class='label label-danger'>" + item.statusString + "</small>";
                       break;
                   default:

               }

               if (item.status == 2)
                   {
                   li += "<br class='product-description text-red'><b class='text-red'>报障信息：" + (item.equipmentRepairDto == null ? "" : item.equipmentRepairDto.problemDescription) + "--报障时间：" + (item.equipmentRepairDto == null ? "" :item.equipmentRepairDto.warningDate) + "</br>";
                   }
              
               if (item.status == 2) {                  
                   li += "<span class='product-description text-green'>场所联系人：" + (item.equipmentRepairDto == null ? "" : item.equipmentRepairDto.placeContact) + "--联系电话：" + (item.equipmentRepairDto == null ? "" :item.equipmentRepairDto.placeContactPhone) + "</span>";
               }
               
               if (item.status == 2 && (item.equipmentRepairDto == null ? "" : item.equipmentRepairDto.processingPerson) != null && (item.equipmentRepairDto == null ? "" : item.equipmentRepairDto.processingPerson)!="") {
                   li += "<span class='product-description  text-yellow '>维修人员：" + (item.equipmentRepairDto == null ? "" : item.equipmentRepairDto.processingPerson) + "--联系电话："+ (item.equipmentRepairDto == null ? "" :  item.equipmentRepairDto.processingPersonPhone) + "</span>";
                }             

              /* if (item.status == 2 && item.equipmentRepairDto.processingResults != null && item.equipmentRepairDto.processingResults != "") {
                    li += "<p class='product-description  text-green '>处理结果：" + (item.equipmentRepairDto == null ? "" :item.equipmentRepairDto.processingResults) + "</p>";
                }*/
             
                li += "</div >";

                li += "<div class='tools'>";

                if (item.status == 1) {
                    li += "<button class='btn btn-primary btn-xs' href='javascript: ;'  onclick = 'add(\"" + item.id + "\")' > <i class='fa fa-legal'></i>报修</button>";
                   

                } else if (item.status == 2) {
                    li += "<button class='btn btn-primary btn-xs' href='javascript: ;'  onclick = 'addMaterial(\"" + item.equipmentRepairDto.beforeMaterial + "\")' > <i class='fa fa-legal'></i>添加报修素材</button>";

                } 
                
                li += "</div>";
                li += "</li>";

                $("#equipmentlist").append(li);
                if (item.placeDto != null) {
                    getPlacePopoverInfo('p_' + item.id, item.placeDto);
                }
                getequipmentmodelPopoverInfo('em_' + item.id, item.equipmentModelDto);
            })

            var elment = $("#apply_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadEquipmentList(newPage, pageSize, getStatus);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
        }
    })
}



//新增报修信息
function add(id) {
    $.ajax({
        type: "Get",
        url: "/Equipment/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#EquipmentId").val(data.id);
            $("#PlaceId").val(data.placeId)
            $("#ProblemDescription").val("");
            $("#PlaceContact").val("");
            $("#PlaceContactPhone").val("");
            $("#Title").text("填写报修原因");
            //弹出新增窗体
            $("#addApplicationModal").modal("show");

        }
    })
};


//保存报修信息
function save() {
    var postData = {
        "dto": {
            "Id": $("#Id").val(),
            "EquipmentId": $("#EquipmentId").val(),
            "PlaceId": $("#PlaceId").val(),
           // "BeforeMaterial": $("#BeforeMaterial").val(),
            "ProblemDescription": $("#ProblemDescription").val(),
            "PlaceContact": $("#PlaceContact").val(),
            "PlaceContactPhone": $("#PlaceContactPhone").val(),
        }
    };
    $.ajax({
        type: "Post",
        url: "/EquipmentRepairApply/SaveRepair",
        data: postData,
        success: function (data) {
            if (data.result == "Success") {
                layer.alert('保存成功');
                loadEquipmentList(1, pageSize, getStatus);
                $("#addApplicationModal").modal("hide");
            } else {
                layer.tips(data.message, "#btnSave");
            }
        }
    });
}

//新增素材
function addMaterial(id) {
    $("#Title").text("新增素材");
    $('#BeforeMaterialId').val(id);
    //弹出新增窗体
    $("#materialModal").modal("show");
};