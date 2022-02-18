var pageSize = 5;
var selectPageNo = 1;



$(function () {
    loadList(selectPageNo, pageSize);

    $("#checkall").click(function () { checkAll(); }); 
    var programTableConfig = {
        tableBodyId: "selectBody",
        selectCallback: function (id, displayName, duration, width, height) {
            $("#select_" + id).click();
        }
    };
    ProgramSelectModal.init(programTableConfig);


    $('#EditBatch').click(function () { EditBatch(); })
});

//全选
function checkAll() {
    $("input[name='playercheck']").prop("checked", $("#checkall").prop('checked'));  
}

function EditBatch() {
    var selectedPlayerids = "";
    var i = 0;
    $("input[name='playercheck']").each(function () {
        if ($(this).prop("checked") == true) {
            if (i == 0) {
                selectedPlayerids += $(this).val();
            } else {
                selectedPlayerids += "," + $(this).val();
            }
            i++;

        }
    });
    window.location.href = '/MyPromotion/EditProgram?selectedPlayerids=' + selectedPlayerids;
}



//加载页面
function loadList(startPage, pageSize) {
    $("#Playerlist").html("");

    $.ajax({
        type: "GET",
        url: "/MyPromotion/GetPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(),
        success: function (data) {

            initPlayerList(data);

            var elment = $("#list_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadList(newPage, pageSize);
                        selectPageNo = newPage;
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
           
            $("#Playerlist > li").click(function () {
                $("#Playerlist > li").removeAttr("style")
                $(this).attr("style", "background-color:#beebff");
            });
            
        }
    })
}

// 加载每行（每个播放器）数据
function initPlayerList(data) {
    $.each(data.rows, function (i, item) {

        var _placeId = item.id;
        var equipment = item.equipment;
        var place = equipment.placeDto;
        var equipmentModel = equipment.equipmentModelDto;

        if (place != null && place.id != null){
        var placePhotos;
        $.ajax({
            type: "GET",
            url: "/Place/GetMaterialPageList?OwnerObjId=" + place.id + "&startPage=1&pageSize=50&_t=" + new Date().getTime(),
            cache: true,
            async: false,
            success: function (data) {
                placePhotos = data;
            }
        })
        }
        //    / EquipmentModel / GetThumbnail ? id = " + item.equipmentModelId


        var playerli = "";
        playerli += "<li class='item' >";

        playerli += "<div class='padding_left_checkbox'>";
        playerli += "    <input  name='playercheck' type='checkbox'    value='" + item.id + "' >";
        playerli += "</div>";

        playerli += "        <div class='photo_img' >";
        playerli += "            <div class='box box-solid'>";
        playerli += "                <div class='box-body'>";
        playerli += "                    <div id='carousel-example-generic" + item.id + "' class='carousel slide' data-ride='carousel'>";

        /*
        playerli += "                        <ol class='carousel-indicators'>";
        playerli += "                            <li data-target='#carousel-example-generic' data-slide-to='" + 0 + "' class='active'></li>";

        if (placePhotos.rowCount > 0) {
            $.each(placePhotos.rows, function (i, item) {
                var ii = i + 1;
                playerli += "                            <li data-target='#carousel-example-generic' data-slide-to='" + ii + "' class=''></li>";         
            });
        }

        playerli += "                        </ol>";
        */

        playerli += "                        <div class='carousel-inner' >";
        playerli += "                            <div class='item active' style='height:160px;text-align:center;'>";
        playerli += "                                <img  style='height:150px' src='/EquipmentModel/GetThumbnail?id=" + equipmentModel.id + "'/>"
        playerli += "                            </div>";
        if (placePhotos != null && placePhotos.rowCount > 0) {
            $.each(placePhotos.rows, function (i, item) {
                playerli += "                            <div class='item' style='height:160px;text-align:center;'>";             
                playerli += "                                <img style='height:150px' src='/Place/GetThumbnail?id=" + item.id + "'/>";
                playerli += "                            </div>";
            });
        };
        playerli += "                        </div>";

        playerli += "                        <a class='left carousel-control' href='#carousel-example-generic" + item.id + "' data-slide='prev'>";
        playerli += "                            <span class='fa fa-angle-left'></span>";
        playerli += "                        </a>";
        playerli += "                        <a class='right carousel-control' href='#carousel-example-generic" + item.id + "' data-slide='next'>";
        playerli += "                            <span class='fa fa-angle-right'></span>";
        playerli += "                        </a>";
        playerli += "                    </div>";
        playerli += "                </div>";
        playerli += "            </div>";
        playerli += "        </div>";


        playerli += " <div class='product-info'>";
        playerli += "<div class='product-description text-light-blue'><b>设备名称：</b>" + (equipment==null || equipment.name == null ? "" : equipment.name);
        playerli += "   &nbsp;&nbsp&nbsp;&nbsp;<b>播放器</b>： &nbsp; 长*宽 &nbsp;" + (item.width == null ? "" : item.width) + "*" + (item.height == null ? "" : item.height);
        /*
        switch (item.status) {
            case 0:
                playerli += "<small class='label label-warning'>" + "<i class='fa fa-hourglass-2'></i>" + (equipment == null || equipment.statusString == null ? "" : equipment.statusString) + "</small>"
                break;
            case 1:
                playerli += "<small class='label label-primary'>" + "<i class='fa fa-send'></i>" + (equipment == null || equipment.statusString == null ? "" : equipment.statusString) + "</small>"
                break;
            case 2:
                playerli += "<small class='label label-success'>" + "<i class='fa fa-wrench'></i>" + (equipment == null || equipment.statusString == null ? "" : equipment.statusString) + "</small>"
                break;
            case 3:
                playerli += "<small class='label label-danger'>" + "<i class='fa fa-trash'></i>" + (equipment == null || equipment.statusString == null ? "" : equipment.statusString) + "</small>"
                break;
            default:
                playerli += "<small class='label label-danger'>" + "<i class='fa fa-warning'></i>" + (equipment == null || equipment.statusString == null ? "" : equipment.statusString) + "</small>"
        }
        */
        playerli += "</div > ";

        playerli += "</div > ";       
        playerli += "<div>";
        if (equipmentModel != null) {
            playerli += "   <b>型号：</b><a   id='em_" + item.id + "'class='product-title btn-link'>" + (equipmentModel.manufacturer == null ? "" : equipmentModel.manufacturer);
            playerli += "   &nbsp; " + (equipmentModel.model == null ? "" : equipmentModel.model) + "</a>";
        }
        if (place != null) {
            playerli += "   &nbsp;&nbsp;&nbsp;&nbsp;<b>场所：</b><a id='p_" + item.id + "'class='product-title btn-link'>" + (place.name == null ? "" : place.name) + "</a>";;
        }
       playerli += "</div>";

       playerli += "<div>";
       playerli += (place==null || place.address == null ? "" : "<b>地址：</b><u>" + place.address+"</u>") ;
       playerli += "</div>";

       if (place != null){
           playerli += "<div text-black> <strong>类型：</strong>" + (place.typeName == null ? "" : place.typeName) + "</div>";
           playerli += "<div text-black> <strong>播放时间：</strong><small>" + (place.timeRange == null ? "" : place.timeRange) + "</small></div>";
       }

       // 加载播放列表
       playerli += initPlayerProgram(item.id);

  
       playerli += "</div>";

        playerli += "</li>";
        $("#Playerlist").append(playerli);

        if (place != null) {
            getPlacePopoverInfo('p_' + item.id, place);
        }
        if (equipmentModel != null) {
            getequipmentmodelPopoverInfo('em_' + item.id, equipmentModel);
        }
    })
}

//加载播放器的播放列表
function initPlayerProgram(playerid) {
    var playerPrograms = "<ul class='products-list product-list-in-box  todo-list' id='orderPlayerProgramlist_"+playerid+"'>";

    $.ajax({
        type: "GET",
        url: "/MyPromotion/GetProgramList?playerid=" + playerid + "&_t=" + new Date().getTime(),
        cache: true,
        async: false,
        success: function (data) {
            playerPrograms += "<div><b>节目清单：</b>";
            $.each(data.orderPlayerPrograms, function (i, orderPlayerProgram) {
                /*
                playerPrograms += "<small class='label label-primary'><b  class='text-orange'>名称：</b>" + (orderPlayerProgram.playerProgram.program.displayName == null ? "" : orderPlayerProgram.playerProgram.program.displayName);
                playerPrograms += "&nbsp;&nbsp;&nbsp;&nbsp;<b  class='text-orange'>时段：</b>" + (orderPlayerProgram.playerProgram.dateTimeSetting == null ? "" : orderPlayerProgram.playerProgram.dateTimeSetting);
                playerPrograms += "&nbsp;&nbsp;&nbsp;&nbsp;<b  class='text-orange'>状态：</b>" + (orderPlayerProgram.playerProgram.statusString == null ? "" : orderPlayerProgram.playerProgram.statusString);
                playerPrograms += "&nbsp;&nbsp;<i class='fa   fa-remove content_lier_close' onclick='deleteSingleprogram(\"" + orderPlayerProgram.playerProgram.id + "\")' ></i></small>"
                */
                playerPrograms += "<li class='item' >";
                playerPrograms += "    <div>";
                playerPrograms += "<b class='text-orange'>名称：</b>" + (orderPlayerProgram.playerProgram.program.displayName == null ? "" : orderPlayerProgram.playerProgram.program.displayName);
                playerPrograms += "&nbsp;&nbsp;&nbsp;&nbsp;<b  class='text-orange'>时段：</b>" + (orderPlayerProgram.playerProgram.dateTimeSetting == null ? "" : orderPlayerProgram.playerProgram.dateTimeSetting);
                playerPrograms += "&nbsp;&nbsp;&nbsp;&nbsp;<b  class='text-orange'>审核状态：</b>" + (orderPlayerProgram.order.statusString == null ? "" : orderPlayerProgram.order.statusString);
                playerPrograms += "&nbsp;&nbsp;&nbsp;&nbsp;<b  class='text-orange'>播放状态：</b>" + (orderPlayerProgram.playerProgram.statusString == null ? "" : orderPlayerProgram.playerProgram.statusString);

                playerPrograms += "    <div class='btn-group right'>";

                //状态类型： 未发布\准备\播放中\完成\取消\暂定
                switch (orderPlayerProgram.playerProgram.statusString) {
                    case "未发布":
                        //显示发布按钮
                        playerPrograms += "        <button class='btn btn-primary  btn-xs' href='javascript:;' onclick='SetplayerprogramToReady(\"" + orderPlayerProgram.playerProgram.id + "\")'><i class='fa fa-send'></i>发布 </button>";
                        //显示取消按钮
                        playerPrograms += "        <button class='btn btn-danger  btn-xs' href='javascript:;' onclick='SetplayerprogramToCancel(\"" + orderPlayerProgram.playerProgram.id + "\")'><i class='fa fa-remove'>取消</i> </button>";   
                        break;
                        // 显示播放按钮
                    case "暂定":
                        playerPrograms += "        <button class='btn btn-info  btn-xs' onclick='SetplayerprogramToPlaying(\"" + orderPlayerProgram.playerProgram.id+ "\")'><i class='fa fa-play'></i> 播放</button>";
                        //显示取消按钮
                        playerPrograms += "        <button class='btn btn-danger  btn-xs' href='javascript:;' onclick='SetplayerprogramToCancel(\"" + orderPlayerProgram.playerProgram.id + "\")'><i class='fa fa-remove'>取消</i> </button>"; 
                        break;
                    case "播放中":
                        //显示暂停按钮
                        playerPrograms += "        <button class='btn btn-warning  btn-xs' onclick='SetplayerprogramToPause(\"" + orderPlayerProgram.playerProgram.id + "\"><i class='fa fa-pause'></i> 暂停</button>";
                        //显示取消按钮
                        playerPrograms += "        <button class='btn btn-danger  btn-xs' href='javascript:;' onclick='SetplayerprogramToCancel(\"" + orderPlayerProgram.playerProgram.id + "\")'><i class='fa fa-remove'>取消</i> </button>";   
                        break;
                    case"准备" :
                        //显示取消按钮
                        playerPrograms += "        <button class='btn btn-danger  btn-xs' href='javascript:;' onclick='SetplayerprogramToCancel(\"" + orderPlayerProgram.playerProgram.id + "\")'><i class='fa fa-remove'>取消</i> </button>";
                        break;

                    //default:
                }
                
                playerPrograms += "    </div>";
                playerPrograms += "    </div>";
   
                playerPrograms += "</li>";

            })
            playerPrograms += "</div>";

          
        }
    })
     playerPrograms += " </ul>";
    return playerPrograms;
}


// 将播放器节目状态变为播放中
function SetplayerprogramToPlaying(playerProgramid) {
    $.ajax({
        type: "POST",
        url: "/MyPromotion/SetplayerprogramToPlaying",
        data: { playerProgramid: playerProgramid },
        success: function (data) {
            if (data.result == "Success") {
                layer.alert("数据保存成功！");
                loadList(selectPageNo, pageSize);
            }
            else {
                layer.alert("数据保存失败！" + data.message);
            }
        }
    });
}

//将播放器节目状态变更为暂停播放
function SetplayerprogramToPause(playerProgramid) {
    $.ajax({
        type: "POST",
        url: "/MyPromotion/SetplayerprogramToPause",
        data: { playerProgramid: playerProgramid },
        success: function (data) {
            if (data.result == "Success") {
                layer.alert("数据保存成功！");
                loadList(selectPageNo, pageSize);
            }
            else {
                layer.alert("数据保存失败！" + data.message);
            }
        }
    });
}

//将播放器节目状态变更为取消播放
function SetplayerprogramToCancel(playerProgramid) {
    $.ajax({
        type: "POST",
        url: "/MyPromotion/SetplayerprogramToCancel",
        data: { playerProgramid: playerProgramid },
        success: function (data) {
            if (data.result == "Success") {
                layer.alert("数据保存成功！");
                loadList(selectPageNo, pageSize);
            }
            else {
                layer.alert("数据保存失败！" + data.message);
            }
        }
    });
}

//将播放器节目状态变更为准备播放
function SetplayerprogramToReady(playerProgramid) {
    $.ajax({
        type: "POST",
        url: "/MyPromotion/SetplayerprogramToReady",
        data: { playerProgramid: playerProgramid },
        success: function (data) {
            if (data.result == "Success") {
                layer.alert("数据保存成功！");
                loadList(selectPageNo, pageSize);
            }
            else {
                layer.alert("数据保存失败！" + data.message);
            }
        }
    });
}

//将播放器节目状态变更为完成播放
function SetplayerprogramToComplete(playerProgramid) {
    $.ajax({
        type: "POST",
        url: "/MyPromotion/SetplayerprogramToComplete",
        data: { playerProgramid: playerProgramid },
        success: function (data) {
            if (data.result == "Success") {
                layer.alert("数据保存成功！");
                loadList(selectPageNo, pageSize);
            }
            else {
                layer.alert("数据保存失败！" + data.message);
            }
        }
    });
}