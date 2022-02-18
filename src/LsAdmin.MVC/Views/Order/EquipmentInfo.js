//全局变量
var equipmentId = getParameter("c[equipment_id]");

//获取url的参数
function getParameter(param) {
    var query = window.location.search;
    var iLen = param.length;
    var iStart = query.indexOf(param);
    if (iStart == -1)
        return "";
    iStart += iLen + 1;
    var iEnd = query.indexOf("&", iStart);
    if (iEnd == -1)
        return query.substring(iStart);

    return query.substring(iStart, iEnd);
}
$("#EquipmentId").val(equipmentId);

/*初始化*/
$(function () {
    LoadEquipmentInfo();
    $(document).on('click', '#J_LinkBasket', function () {
        
    });
    choosePlayer();
    clickImg();
})

function LoadEquipmentInfo() {
    $.ajax({
        type: "GET",
        url: "/Equipment/Get?Id=" + equipmentId + "&_t=" + new Date().getTime(),
        cache: true,
        async: false,
        success: function (data) {
            setEquipmentInfoFormat(data);
            shoppingCart();
        }
    })
}

function setEquipmentInfoFormat(data) {
    $.ajax({
        type: "GET",
        url: "/Equipment/GetImagePageList?equipmentId=" + data.equipmentModelId + "&placeId=" + data.placeDto.id + "&startPage=1&pageSize=50&_t=" + new Date().getTime(),
        cache: true,
        async: false,
        success: function (data) {
            Materials = data;
        }
    })
    var Materialsdiv = '<div data-spm=' + data.id + ' class="tb-gallery" data-spm-max-idx="9">';
    Materialsdiv +=         '<div class="tb-booth">';
    Materialsdiv +=             '<a href="javascript:void(0);">';
    Materialsdiv +=                 '<img id=J_ImgBooth alt=' + data.name + ' src="/Place/GetThumbnail?id=' + Materials.rows[0].id +'">';
    Materialsdiv +=             '</a>';
    Materialsdiv +=         '</div>';
    Materialsdiv +=         '<div class="tb-thumb-warp ">';
    Materialsdiv +=             '<a class="tb-thumb-left"><i>&#xe631;</i></a>';
    Materialsdiv +=             '<div class="tb-thumb-content"> ';

    var Materialsul =                '<ul id="J_UlThumb" class="tb-thumb tm-clear">';
    if (Materials.rowCount > 0) {
        $.each(Materials.rows, function (i, item) {
            if (i == 0) {
                Materialsul +=             '<li id=' + item.id +' class="tb-selected">';
                Materialsul +=                 '<a href="#"><img src="/Place/GetThumbnail?id=' + item.id + '"></a>';
                Materialsul +=              '</li>';
            }
            else {
                Materialsul +=              '<li id=' + item.id +' class="">';
                Materialsul +=                  '<a href="#"><img src="/Place/GetThumbnail?id=' + item.id + '"></a>';
                Materialsul +=              '</li>';
            }           
        });
    }
    Materialsul +=                    '</ul>';
    Materialsdiv += Materialsul;
    Materialsdiv +=             '<a class="tb-thumb-right"><i>&#xe632;</i></a>';
    Materialsdiv +=             '</div>';
    Materialsdiv +=         '</div>';
    Materialsdiv += '<p class="tm-action tm-clear">';
    if (data.favoriteType == 0) {
        Materialsdiv += "<a id='scbutton_" + data.id + "' href='javascript: ; ' title='收藏'  style='margin-right:5px; display: inline - block; ' data-list='1'><i class='iconfont'>&#xe6b3;</i><span>收藏</span></a>";
        Materialsdiv += "<a id='hmdbutton_" + data.id + "' href='javascript: ;' title='加入黑名单' style='margin-right:5px; visibility: visible; ' data-list='1'> <i class='iconfont'>&#xe65e;</i><span>加入黑名单</span></a>";
    }
    else if (data.favoriteType == 1) {
        Materialsdiv += "<a id='scbutton_" + data.id + "' href='javascript: ; ' title='取消收藏'  style='margin-right:5px; display: inline - block; ' data-list='2' class='favorite'><i class='iconfont' style='color:#f85559;'>&#xe635;</i><span>取消收藏</span></a>";
        Materialsdiv += "<a id='hmdbutton_" + data.id + "' href='javascript: ;' title='加入黑名单' style='margin-right:5px; visibility: hidden;'  data-list='1'> <i class='iconfont'>&#xe65e;</i><span>加入黑名单</span></a>";
    }
    else {
        Materialsdiv += "<a id='scbutton_" + data.id + "' href='javascript: ; ' title='收藏'  style='margin-right:5px; visibility:hidden ' data-list='1'><i class='iconfont'>&#xe6b3;</i><span>收藏</span></a>";
        Materialsdiv += "<a id='hmdbutton_" + data.id + "' href='javascript: ;' title='加入黑名单' style='margin-right:5px; visibility: visible; ' data-list='2'> <i class='iconfont'>&#xe65e;</i><span>取消黑名单</span></a>";
    } 
    //Materialsdiv += '<a id="J_AddFavorite" href="javascript:;" equipmentId=' + equipmentId + ' class="favorite"><i></i><span>收藏商品</span></a>';
    Materialsdiv += '</p>';
    Materialsdiv += '</div>';

    $.ajax({
        type: "Get",
        url: "/Equipment/GetPlayersByEquipmentId?equipmentId=" + data.id + "&_t=" + new Date().getTime(),
        cache: true,
        async: false,
        success: function (data) {
            players = data;
        }
    })
    var playerdd = '';
    if (players.rows.length > 0) {
        $.each(players.rows, function (i, item) {
            var playerli = '<li data-value=' + item.id + ' title=' + item.sort + ' class="tb-txt">';
            playerli += '<a href="#" role="button" tabindex="0">';
            playerli += '<span>位置：' + item.sort + ' &nbsp;&nbsp;&nbsp;&nbsp;屏幕规格：' + item.width + '*' + item.height + '</span>';
            playerli += '</a>';
            playerli += '<i>已选中</i>';
            playerli += '</li>';
            playerdd += playerli;
        });
    } 

    var div = '<div class="tm-clear">';
    div += '<div class="tb-property">';
    div += '<div class="tb-wrap">';
    div += '<div class="tb-detail-hd">';
    div += '<h1 data-spdm=' + data.id + '>' + data.name + '</h1>';
    div += '</div>';
    div += '<div class="tm-fcs-panel">';
    div += '<dl class="tm-price-panel tm-price-cur" id="J_StrPriceModBox">';
    div += '<dt class="tb-metatit">价格</dt>';
    div += '<dd><em class="tm-yen">¥</em> <span class="tm-price">' + data.priceDto.price + '</span> <div class="staticPromoTip"></dd>';
    div += '</dl>';
    div += '</div>';
    div += '<div clss="tb-key">';
    div += '<div class="tb-skin">';
    div += '<div class="tb-sku">';
    div += '<dl class="tb-prop tm-sale-prop tm-clear tm-img-prop ">';
    div += '<dt class="tb-metatit">播放器</dt>';
    div += '<dd>';
    div += '<ul id="playerul" data-property="播放器" class="tm-clear J_TSaleProp tb-img">';
    div += playerdd;
    div += '</dd>';
    div += '</dl>';
    div += '<div class="tb-action tm-clear">';
    div += '<div class="tb-btn-buy tb-btn-sku"><a id="J_LinkBuy" href="#" rel="nofollow" data-addfastbuy="true" title="点击此按钮，到下一步新增订单。" role="button" onclick="buynow()">立即购买<span class="ensureText">确认</span></a></div>';  
    div += '<div class="tb-btn-basket tb-btn-sku"><a href="#" rel="nofollow" id="J_LinkBasket" role="button" onclick="addToCart()"><i></i>加入购物车<span class="ensureText">确认</span></a></div>';
    div += '<div class="tb-btn-add tb-btn-sku tb-hidden"><a href="#" rel="nofollow" id="J_LinkAdd" role="button"><i></i>加入购物车</a></div>';
    div += '</div>';        
    div += '</div>';
    div += '</div>';
    div += '</div>';
    div += '</div>';
    div += '</div>';
    div += Materialsdiv;
    div += '</div>';

    $("#J_DetailMeta").append(div);

    var Attrdiv = '<div class="tm-clear tb-hidden tm_brandAttr" id="J_BrandAttr" style="display: block;height:50px;"><div class="name">品牌名称：<b class="J_EbrandLogo">' + data.equipmentModelDto.manufacturer + '</b></div></div>';
    Attrdiv += '<p class="attr-list-hd tm-clear"><em>场所信息：</em></p>';
    Attrdiv += '<ul id="J_AttrUL">';
    Attrdiv += '<li title="&nbsp;' + (data.placeDto.address == null ? "" : data.placeDto.address) + '">地址:&nbsp;' + (data.placeDto.address == null ? "" : data.placeDto.address) + '</li>';
    Attrdiv += '<li title="&nbsp;' + (data.placeDto.typeName == null ? "" : data.placeDto.typeName) + '">类型:&nbsp;' + (data.placeDto.typeName == null ? "" : data.placeDto.typeName)+ '</li>';
    Attrdiv += '<li title="&nbsp;' + (data.placeDto.timeRange == null ? "" : data.placeDto.timeRange) + '">播放时间:&nbsp;' + (data.placeDto.timeRange == null ? "" : data.placeDto.timeRange)  + '</li>';
    Attrdiv += '<li title="&nbsp;' + (data.equipmentModelDto.voicedName == null ? "" : data.equipmentModelDto.voicedName) + '">标签:&nbsp;';
    var placeTags = data.placeDto.placeTag == null ? "" : data.placeDto.placeTag.split(",");
    if (placeTags != "") {
        $.each(placeTags, function (i, item) {
            Attrdiv += '<small class="label label-info" style="margin-left:10px;">' + item + '</small>';
        })
    }
    Attrdiv += '</li>';
    Attrdiv += '<li title="&nbsp;' + (data.placeDto.introduction == null ? "" : data.placeDto.introduction) + '">介绍:&nbsp;' + (data.placeDto.introduction == null ? "" : data.placeDto.introduction) + '</li>';
    Attrdiv += '</ul>';

    Attrdiv += '<p class="attr-list-hd tm-clear"><em>设备信息：</em></p>';
    Attrdiv += '<ul id="J_AttrUL">';
    Attrdiv += '<li title="&nbsp;' + (data.equipmentModelDto.model == null ? "" : data.equipmentModelDto.model) + '">型号:&nbsp;' + (data.equipmentModelDto.model == null ? "" : data.equipmentModelDto.model)+'</li>';
    Attrdiv += '<li title="&nbsp;' + (data.equipmentModelDto.screenSize == null ? "" : data.equipmentModelDto.screenSize) + '">屏幕:&nbsp;' + (data.equipmentModelDto.screenSize == null ? "" : data.equipmentModelDto.screenSize) + '&nbsp;' + (data.equipmentModelDto.resolutionRatio == null ? "" : data.equipmentModelDto.resolutionRatio) + "&nbsp;" + (data.equipmentModelDto.screenRatio == null ? "" : data.equipmentModelDto.screenRatio)+ '</li>';
    Attrdiv += '<li title="&nbsp;' + (data.equipmentModelDto.screenWidth == null ? "" : data.equipmentModelDto.screenWidth + 'cm') + '*' + (data.equipmentModelDto.screenHeight == null ? "" : data.equipmentModelDto.screenHeight + "cm") + '">宽*高:&nbsp;' + (data.equipmentModelDto.screenWidth == null ? "" : data.equipmentModelDto.screenWidth + 'cm') + '*' + (data.equipmentModelDto.screenHeight == null ? "" : data.equipmentModelDto.screenHeight + 'cm') + '</li>';
    Attrdiv += '<li title="&nbsp;' + (data.equipmentModelDto.voicedName == null ? "" : data.equipmentModelDto.voicedName) + '">是否有声音:&nbsp;' + (data.equipmentModelDto.voicedName == null ? "" : data.equipmentModelDto.voicedName) + '</li>';
    Attrdiv += '<li title="&nbsp;' + (data.equipmentModelDto.sideScreensNumber == null ? "" : data.equipmentModelDto.sideScreensNumber) + '">副屏数:&nbsp;' + (data.equipmentModelDto.sideScreensNumber == null ? "" : data.equipmentModelDto.sideScreensNumber) + '个</li>';
    Attrdiv += '<li title="&nbsp;' + (data.equipmentModelDto.model == null ? "" : data.equipmentModelDto.model) + '">适用场所:&nbsp;';
    var applyTos = data.equipmentModelDto.applyTo == null ? "" : data.equipmentModelDto.applyTo.split(",");
    if (applyTos != "") {
        $.each(applyTos, function (i, applyTo) {
            Attrdiv += '<small class="label label-info" style="margin-left:10px;">' + applyTo + ' </small>';
        })
    }
    Attrdiv += '</li>';
    Attrdiv += '';
    Attrdiv += '';
    Attrdiv += '';
    Attrdiv += '</ul>';
    Attrdiv += '</div>';

    $("#J_AttrList").append(Attrdiv);

    var Descriptiondiv = '<h4 class="hd">设备详情</h4>';
    Descriptiondiv += '<div class="content ke-post" style="height: auto;">';
    Descriptiondiv += '<p>';
    if (Materials.rowCount > 0) {
        $.each(Materials.rows, function (i, item) {
            Descriptiondiv += '<img align="absmiddle" src="/Place/GetThumbnail?id=' + item.id + '" style="line-height: 1.5;">';
        });
    }
    Descriptiondiv += '</div>';

    $("#description").append(Descriptiondiv);

    $("#scbutton_" + data.id).on("click", function () {
        Favorite(data.id, 1, this);
    })
    $("#hmdbutton_" + data.id).on("click", function () {
        Black(data.id, 1, this);
    })
}

//选择播放器效果
function choosePlayer() {
    $(document).on('click', '.tb-txt', function () {
        $(".tb-txt").each(function () {
            if ($(this).hasClass("tb-selected"))
            {
                $(this).removeClass("tb-selected")
            }
        })
        $(this).addClass("tb-selected");
    })
}

//选择图片效果
function clickImg() {
    $(document).on('click', '#J_UlThumb li', function () {
        $("#J_UlThumb").children("li").each(function () {
            if ($(this).hasClass("tb-selected")) {
                $(this).removeClass("tb-selected")
            }
        })
        $(this).addClass("tb-selected");
        $("#J_ImgBooth").attr("src", "/Place/GetThumbnail?id=" + this.id);
    })
}

//立即购买
function buynow() {
    var orderId = $("#orderId").val() ? $("#orderId").val():"";
    var playerIds = $("#playerul").find(".tb-selected").attr("data-value");
    var price = $("#J_DetailMeta").find('.tm-price').text();
    if (playerIds == null) {
        layer.msg("请选择播放器！", { icon: 2, time: 2000 });
        return false;
    }
    window.location.href = 'order?c[order_id]=' + orderId + '&c[player_ids]=' + playerIds + '&c[price]=' + price;
}


//点击收藏相关操作
function Favorite(equipmentId, scType, o) {
    var scAreaStyle = $("#scArea").css("display");
    var dataList = $(o).attr("data-list");
    var scTypeNew = 1;
    if (dataList == 1) {
        scTypeNew = 1;
    }
    else {
        scTypeNew = 2;
    }
    $.ajax({
        url: "/CollectionsBlacklists/favoriteAD",
        data: { equipmentId: equipmentId, favoriteType: 1, scType: scTypeNew },
        crossDomain: true,
        async: false,
        cache: false,
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data.result = "Success") {
                if (dataList == 1) {
                    $(o).find("i").html('&#xe635;');
                    $(o).addClass('favorite');
                    $('.favorite').attr('title', '取消收藏');
                    $(o).attr("data-list", "2");
                    $("#hmdbutton_" + equipmentId).css('visibility', 'hidden');
                    $("#scbutton_" + equipmentId).find("span").text("取消收藏");
                    layer.msg("收藏成功", { icon: 1, time: 1000 });
                }
                else {
                    $(o).find("i").html('&#xe6b3;');
                    $('.favorite').attr('title', '收藏');
                    $(o).removeClass('favorite');
                    $(o).attr("data-list", "1");
                    $("#hmdbutton_" + equipmentId).css('visibility', 'visible');
                    $("#scbutton_" + equipmentId).find("span").text("收藏");
                    $(o).blur();
                    layer.msg("取消收藏", { icon: 1, time: 1000 });
                }
            }
            else {
                layer.msg("操作失败", { icon: 1, time: 1000 });
            }
        },
        error: function (erg) {
            return false;
        }
    });
}

//点击黑名单相关操作
function Black(equipmentId, scType, o) {
    var scAreaStyle = $("#scArea").css("display");
    var dataList = $(o).attr("data-list");
    var scTypeNew = 1;
    if (dataList == 1) {
        scTypeNew = 1;
    }
    else {
        scTypeNew = 2;
    }
    $.ajax({
        url: "/CollectionsBlacklists/favoriteAD",
        data: { equipmentId: equipmentId, favoriteType: 2, scType: scType },
        crossDomain: true,
        async: false,
        cache: false,
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data.result = "Success") {
                if (dataList == 1) {
                    $(o).addClass('favorite');
                    $('.favorite').attr('title', '取消黑名单');
                    $(o).attr("data-list", "2");
                    $("#scbutton_" + equipmentId).css('visibility', 'hidden');
                    $("#hmdbutton_" + equipmentId).find("span").text("取消黑名单");
                    layer.msg("操作成功", { icon: 1, time: 1000 });
                }
                else {
                    $('.favorite').attr('title', '加入黑名单');
                    $(o).removeClass('favorite');
                    $(o).attr("data-list", "1");
                    $("#scbutton_" + equipmentId).css('visibility', 'visible');
                    $("#hmdbutton_" + equipmentId).find("span").text("加入黑名单");
                    layer.msg("操作成功", { icon: 1, time: 1000 });
                }              
            }
            else {
                layer.msg("操作失败", { icon: 2, time: 1000 });
            }

        },
        error: function (erg) {
            return false;
        }
    });
}
