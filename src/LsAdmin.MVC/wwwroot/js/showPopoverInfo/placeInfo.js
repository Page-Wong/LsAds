function PlaceContentMethod(placeDto, Materials) {

    var placeIntroduction = "";
    if (Materials != null && Materials.rowCount >0) {


        //class="" 
        placeIntroduction += " <div class='col-md-16'   >";
        placeIntroduction += "<div class='carousel slide' data-ride='carousel'>";
        placeIntroduction += "<!-- /.box-header -->";
        placeIntroduction += "<div class='box-body'>";

        placeIntroduction += " <div id='carousel-example-generic' class='carousel slide' data-ride='carousel'>";
        placeIntroduction += "<ol class='carousel-indicators'>";
        $.each(Materials.rows, function (i, item) {

            if (i == 0) { placeIntroduction += "<li data-target='#carousel-example-generic' data-slide-to='0' class='active'></li>"; }
            else {
                placeIntroduction += "<li data-target='#carousel-example-generic' data-slide-to='" + i+"' class=''></li>";
            }      
        })
        placeIntroduction += " </ol>";
        placeIntroduction += " <div class='carousel-inner'>";

        $.each(Materials.rows, function (i, item) {
            if (i == 0) { placeIntroduction += " <div class='item active'>"; } else { placeIntroduction += " <div class='item'>"; } 
            placeIntroduction += "<img src='/Place/GetThumbnail?id=" + item.id + "'alt='" + item.name +"'/>";
            placeIntroduction += "</div>";
        })
        placeIntroduction += "</div>";

        placeIntroduction += "<a class='left carousel-control' href='#carousel-example-generic' data-slide='prev'>";
        placeIntroduction += "<span class='fa fa-angle-left'></span>";
        placeIntroduction += " </a>";
        placeIntroduction += "<a class='right carousel-control' href='#carousel-example-generic' data-slide='next'>";
        placeIntroduction += " <span class='fa fa-angle-right'></span>";
        placeIntroduction += " </a>";
        placeIntroduction += "</div>";
        placeIntroduction += "</div>";
        placeIntroduction += "<!-- /.box-body -->";
        placeIntroduction += "</div>";
        placeIntroduction += " <!-- /.box -->";
        placeIntroduction += "</div>";
    }
   
    placeIntroduction += "<div> <strong>地址: </strong><u>" + (placeDto.address == null ? "" : placeDto.address) + "</u></div>";
    placeIntroduction += "<div text-black> <strong>类型: </strong>" + (placeDto.typeName == null ? "" : placeDto.typeName) + "</div>";
    placeIntroduction += "<div text-black> <strong>播放时间: </strong><small>" + (placeDto.timeRange == null ? "" : placeDto.timeRange) + "</small></div>";
    placeIntroduction += "<div text-black> <strong>标签: </strong>"

    var placeTags = placeDto.placeTag == null ? "" : placeDto.placeTag.split(",");
    if (placeTags!=""){
    $.each(placeTags, function (i, item) {
        placeIntroduction += "<small class='label label-info'>" + item + "</small>";
        })
    }
    placeIntroduction += "<div text-black> <strong>介绍：</strong><small><br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + (placeDto.introduction == null ? "" : placeDto.introduction) + "</small></div>";
    return placeIntroduction;
}
function getPlacePopoverInfo(objId, placeDto) {
    if (objId == null || placeDto == null) return;
    var Materials;
    $.ajax({
        type: "GET",
        url: "/Place/GetMaterialPageList?OwnerObjId=" + placeDto.id+"&startPage=1&pageSize=20&_t=" + new Date().getTime(),
        cache: true,
        async: false,
        success: function (data) {
             Materials = data;
        }
    })

    //top, bottom, left or right
    var obj_p = $("#" + objId);
    if (obj_p == null) return;
    // data-toggle='popover'
    obj_p.attr('data-toggle', 'popover');
    var content;

    obj_p.popover({
        //  trigger: 'manual',
        placement: 'bottom',
        title: placeDto.name,
        html: 'true',
        content: PlaceContentMethod(placeDto, Materials),

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