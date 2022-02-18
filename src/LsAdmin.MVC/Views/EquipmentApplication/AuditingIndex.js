var pageSize = 10;
var getStatus = 0;//getStatus :all 表示加载所有状态记录

$(function () {
    loadStatusEquipmentApplicationList(1, pageSize, getStatus);
    $("#btnStatusAll").click(function () { getStatus = "all"; loadStatusEquipmentApplicationList(1, pageSize, getStatus); });
    $("#btnStatusWaiting").click(function () { getStatus = "0"; loadStatusEquipmentApplicationList(1, pageSize, getStatus); });
    $("#btnStatusChecking").click(function () { getStatus = "1"; loadStatusEquipmentApplicationList(1, pageSize, getStatus); });
    $("#btnStatusPass").click(function () { getStatus = "2"; loadStatusEquipmentApplicationList(1, pageSize, getStatus); });
    $("#btnStatusFailed").click(function () { getStatus = "3"; loadStatusEquipmentApplicationList(1, pageSize, getStatus); });
    $('[data-toggle="popover"]').popover();
   
});

/*
function ContentMethod(placeDto) {

    var placeIntroduction="";
    //placeIntroduction += "<small class='text-left'> ";
    placeIntroduction += "<div> <strong>地址: </strong><u>" + (placeDto.address == null ? "" : placeDto.address) + "</u></div>";
    placeIntroduction += "<div text-black> <strong>类型: </strong>" + (placeDto.typeName == null ? "" : placeDto.typeName) + "</div>";
    placeIntroduction += "<div text-black> <strong>播放时间: </strong><small>" + (placeDto.timeRange == null ? "" : placeDto.timeRange) + "</small></div>";
    placeIntroduction += "<div text-black> <strong>标签: </strong>"

    var placeTags = placeDto.placeTag == null? "":placeDto.placeTag.split(","); 
    $.each(placeTags, function (i, item) {
        placeIntroduction += "<small class='label label-info'>" + item+"</small>";
    })
    placeIntroduction += "<div text-black> <strong>介绍：</strong><small><br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + (placeDto.introduction == null ? "" : placeDto.introduction) +"</small></div>";
   // placeIntroduction += "</small>";
    return placeIntroduction;
}
function getPlacePopoverInfo(objId, placeDto) {
    //top, bottom, left or right
    var obj_p = $("#"+objId);
    if (obj_p == null) return;
    // data-toggle='popover'
    obj_p.attr('data-toggle', 'popover');
    var content;

    obj_p.popover({
        //  trigger: 'manual',
        placement: 'bottom',
        title: placeDto.name,
        html: 'true',
        content: ContentMethod(placeDto),

    }).on("mouseenter", function () {
        var _this = this;
        $(this).popover("show");
        $(this).siblings(".popover").on("mouseleave", function () {
            $(_this).popover('hide');
        });
    }).on("mouseleave", function () {
        var _this = this;
        setTimeout(function () {
            if (!$(".popover:hover").length) {
                $(_this).popover("hide")
            }
        }, 100);
    }); 
}
*/
//加载列表数据
function loadStatusEquipmentApplicationList(startPage, pageSize, status) {
    $("#equipmentApplicationList").html("");
    var urlString;

    if(status != "all") {
        urlString= "/EquipmentApplication/GetEquipmentApplicationByStatus?status=" + status + "&startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime();
     }else {
        urlString= "/EquipmentApplication/GetEquipmentApplicationWithStatusRowCount?startPage=" + startPage + "&pageSize=" + pageSize +  "&_t=" + new Date().getTime();
    }

    $.ajax({
        type: "GET",
        url: urlString,
        success: function (data) {          
            var statusRowCount = data.statusRowCount;

            $("#SpanStatusAll").html(data.allrowCount == 0 ? "" : data.allrowCount);
            $("#SpanStatusWaiting").html(statusRowCount["0"] == null ? "" : statusRowCount["0"]);
            $("#SpanStatusChecking").html(statusRowCount["1"] == null ? "" : statusRowCount["1"]);
            $("#SpanStatusPass").html(statusRowCount["2"] == null ? "" : statusRowCount["2"]);
            $("#SpanStatusFailed").html(statusRowCount["3"] == null ? "" : statusRowCount["3"]);
      
            $.each(data.rows, function (i, item) {
                /*
                  <li class="item">
                    <div class="product-img">
                        <img src="/EquipmentModel/GetThumbnail?id=08d56ece-c585-9a95-c5da-5bba039cb53b" alt="Product Image">
                    </div>
                    <div class="product-info">
                        <span class="product-description text-uppercase text-light-blue"> 2018-01-25 20:11:07<small class="label label-danger"><i class="fa fa-clock-o"></i> 2 mins</small></span>
                        <a href="javascript:void(0)" class="product-title btn-link">王府井百货(广东省东莞市石龙XXXXXXXXXXX)<small class="label label-warning">待确认</small> </a>
                        <br class="text-green">需申请适合尺寸为10cm*20cm的播放设备。需申请适合尺寸为10cm*20cm的播放设备。需申请适合尺寸为10cm*20cm的播放设备。需申请适合尺寸为10cm*20cm的播放设备。需申请适合尺寸为10cm*20cm的播放设备。需申请适合尺寸为10cm*20cm的播放设备<br />
                    </div>
                    <div class="tools">
                        <button class="btn btn-info btn-xs" href="javascript:;" onclick=""> <i class="fa fa-legal"></i>确认</button>
                        <button class="btn btn-success btn-xs" href="javascript:;" onclick=""> <i class="fa fa-check"></i>通过</button>
                        <button class="btn btn-danger btn-xs" href="javascript:;" onclick=""> <i class="fa fa-remove"></i>不通过</button>
                        <button class="btn btn-warning btn-xs" href="javascript:;" onclick=""> <i class="fa fa-reply"></i>取消审核</button>
                    </div>
                </li>   
                 */

                var li = " <li class='item'>";
                li += "<div class='product-img'>";
                li += "<img src='/EquipmentModel/GetThumbnail?id=" + item.placeId + "'/>";
                li += "</div>";
                li += "<div class='product-info'>";
                li += "<div class='product-description  text-black'>" + (item.createTime == null ? "" : item.createTime) + "<small class='label label-info'><i class='fa fa-clock-o'></i>" + (item.timeAgo == null ? "" : item.timeAgo) + "</small>";

                switch (item.status) {
                    case 0:
                        li += "<small class='label label-warning'>" + "<i class='fa fa-bullhorn'></i>" + (item.statusString == null ? "" : item.statusString) + "</small>"
                        break;
                    case 1:
                        li += "<small class='label label-primary'>" + "<i class='fa fa-legal'></i>" + (item.statusString == null ? "" : item.statusString) + "</small>"
                        break;
                    case 2:
                        li += "<small class='label label-success'>" + "<i class='fa fa-check'></i>" + (item.statusString == null ? "" : item.statusString) + "</small>"
                        break;
                    case 3:
                        li += "<small class='label label-danger'>" + "<i class='fa fa-remove'></i>" + (item.statusString == null ? "" : item.statusString) + "</small>"
                        break;
                }
          
                li += "</div>";
                li += "<a id='p_" + item.id +"' class='product-title btn-link'>" + (item.placeDto.name == null ? "" : item.placeDto.name) + "(" + (item.placeDto.address == null ? "" : item.placeDto.address) +  "---" +(item.placeDto.contact == null ? "" : item.placeDto.contact) + "---" + (item.placeDto.phone == null ? "" : item.placeDto.phone)+") </a>";
                li += "<br class='text-green'>" + (item.reason == null ? "" : item.reason) + "<br />";
                li += "</div>";
                li += "<div class='tools'>";
                switch (item.status) {
                    case 0:
                        li += "    <button class='btn btn-warning btn-xs btnauditing'    href='javascript: ;'   onclick = 'SaveCheckingInfo(\"" + item.id + "\")' >  <i class='fa fa-legal'></i>确认</button>"; break;
                    case 1:
                        li += "    <button class='btn btn-success btn-xs btnauditing' href='javascript: ;'   onclick = 'SavePassInfo(\"" + item.id + "\")' >  <i class='fa fa-check'></i>通过</button>";
                        li += "    <button class='btn btn-danger btn-xs btnauditing'  href='javascript: ;'   onclick = 'SaveFailedInfo(\"" + item.id + "\")' >  <i class='fa fa-remove'></i>不通过</button>";
                        break;
                    case 2:
                    case 3:
                        li += "    <button class='btn btn-info btn-xs btnauditing' href='javascript: ;'   onclick = 'SaveCancelAuditingInfo(\"" + item.id + "\")' >  <i class='fa fa-reply'></i>取消审核</button>";
                        break;
                }
                         li += "</div>";
                li += "</li> ";

               
                $("#equipmentApplicationList").append(li);
                getPlacePopoverInfo('p_' + item.id, item.placeDto);
            })

            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadStatusEquipmentApplicationList(newPage, pageSize, getStatus);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
            $("ul > li").click(function () {
                $("ul > li").removeAttr("style")
                $(this).attr("style", "background-color:#beebff");
            });
        }
    })
}


function SaveCheckingInfo(id) {
    SaveStates(id, 1);
}

function SavePassInfo(id) {
    SaveStates(id, 2);
}

function SaveFailedInfo(id) {
    SaveStates(id, 3);
}

function SaveCancelAuditingInfo(id) {
    SaveStates(id, 1);
}

// 保存审核信息
function SaveStates(id,status) {
   
   // $(".btnauditing").attr("disabled", disabled)
    
    $.ajax({
        type: "POST",
        url: "/EquipmentApplication/UpdateStatus",
        data: { id: id, status: status },
        success: function (data) {
        //    $("#CompleteSave").attr("disabled", false);

            if (data.result == "Success") {
                layer.alert('保存成功');
                loadStatusEquipmentApplicationList(1, pageSize, getStatus);
            }
            else {
                layer.alert('保存失败：' + (data.message == null ? "" : data.message)+"。请刷新数据!");
            }
        }
    });
}


