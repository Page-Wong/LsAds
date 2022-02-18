//购物车展开
var closeCartHandl;

var orderId = getParameter("c[order_id]");
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
$("#orderId").val(orderId);
if (orderId == "" || orderId == null) {
    type = 1;
}
else {
    type = 2;
}


$(function () {
    $(document).on('click', '.sub_area_click', function () {
        if (!$("#shopping_cart_chose_list").hasClass("open")) {
            $("#shopping_cart_chose_list").addClass("open");
        } else {
            $("#shopping_cart_chose_list").removeClass("open");
        }
    });
    //bindCartEvent("tb-btn-basket");
    //shoppingCart();
})

function openCart() {

    window.clearTimeout(closeCartHandl);
    $("#shopping_cart_chose_list").addClass('open');

}
//关闭购物车
function closeCart() {
    closeCartHandl = window.setTimeout(function () {
        $("#shopping_cart_chose_list").removeClass('open');
    }, 1000)
}

//点击加入购物车按钮
function addToCart() {
    var playerIds = $("#playerul").find(".tb-selected").attr("data-value");
    var accountIds = $("#account_ids").val();
    if (playerIds == null) {
        layer.msg("请选择播放器！", { icon: 2, time: 2000 });
        return false;
    }
    if (accountIds.indexOf(playerIds) == -1) {
        openCart();
        getData_toto_cart(playerIds);
        //购物车添加数据库
        var cartType = 'add';
        cartAD(playerIds, cartType, type);
    }
    else {
        layer.msg("该播放器已加入购物车，请查看购物车！", { icon: 2, time: 2000 });
        return false;
    }
}

//向购物车添加账号事件
function getData_toto_cart(playerId) {
    var id = playerId; //_this.id;
    var _this = $("#J_DetailMeta");
    var photo_src = $(_this).find('.tb-booth').children().html();;//replace("height:150px", "height:43px");
    var name = $(_this).find('.tb-detail-hd').find("h1").html();
    var cartId = playerId;
    var price = $(_this).find('.tm-price').text();
    var playerSort = $("#playerul").find(".tb-selected").find("span").html();
    dtw_detail_url = $(_this).attr('dtw_detail_url');
    cartAddData(photo_src, name, id, cartId, playerSort,price, dtw_detail_url);
}

function getData_toto_cart2(obj, equipmentIds) {
    var _this = obj;

    var photo_src = _this.find('.photo_src').html();
    var name = _this.find('.shopping_cart_chose_listName').text();
    var id = equipmentIds; //_this.id;
    var cartId = _this.id;
    var price = {
        dtw_price: _this.attr('dtw_price'),
    }
    dtw_detail_url = $(_this).attr('dtw_detail_url');
    cartAddData(photo_src, name, id, cartId, price, dtw_detail_url);
}


//购物车创建选中账号
function cartAddData(photo_src, name, id, cartId, playerSort, price, dtw_detail_url) {
    var div = document.createElement('div');
    div.className = 'shopping_cart_chose_list_content_lier box_h';
    var playerId = id;
    div.id = 'cart_' + id;
    //格式化价格 
    var dtwPrice = price;
    var html =
        '<div class="shopping_cart_chose_list_content_lier_photo"><a href="' + dtw_detail_url + '" target="_blank">' + photo_src + '</a></div>' +
        '<div class="shopping_cart_chose_list_content_lier_user">' +
        '<div class="box_h">' +
        '<div class="shopping_cart_chose_list_content_name">' + name + '</div>' +
        '</div>' +
        '<h3 class="cart_player" style="margin:0px;">' + playerSort + '</h3>' +
        '<h3 class="dtw_price cart_price" style="margin:0px;">' + '￥' + dtwPrice + '</h3>' +
        '</div>' +
        '<div onclick="deleteCart(\'' + playerId + '\')" class="shopping_cart_chose_list_content_lier_close"><span>+</span></div>';
    div.innerHTML = html;
    document.getElementById('shopping_cart_chose_listContent').appendChild(div);
    getCartNum();
    checkPrice();
    //closeCart();
}


//购物车移除
function delete_to_cart(id) {
    $('#' + id).remove();
    checkPrice();
}


//全选绑定向购物车(添加/删除)
$('.addto_cart_all').click(function () {
    if (this.checked) {
        //$('#shopping_cart_chose_listContent').html('');
        var mids = "";
        $(this).parent().parent().parent().parent().parent().find('.addto_cart').each(function () {
            if (this.checked == false) {
                mids += $(this).attr('data-id') + ',';
                $(this).prop('checked', true);
                getData_toto_cart2($(this).parent().parent(), $(this).attr('data-id'));
            }
        });
        if (mids !== "") {
            $('.search_result_lier').addClass('get');
            cartAD(mids.substr(0, mids.length - 1), 'add', type);
            $('#shopping_cart_chose_list').addClass('open');
        }
    } else {

        var mids = "";
        $(this).parent().parent().parent().parent().parent().find('.addto_cart').each(function () {
            mids += $(this).attr('data-id') + ',';
            $(this).prop('checked', false);
            delete_to_cart('cart_' + $(this).attr('data-id'));
        });
        $('.item').removeClass('get');
        cartAD(mids, 'del', type);
        /*$(this).parent().parent().parent().parent().parent().find('.addto_cart').each(function(){
            if(this.checked){
                this.click();
            }
        });*/
    }
    getCartNum();
    checkPrice();
})

//绑定向购物车(添加/删除)
function bindCartEvent(sclass) {
    $(document).on('click', '.' + sclass, function () {
        var obj = this.parentNode.parentNode;
        var equipmentIds = $(this).next("input").val();
        if (!$(obj).hasClass('get')) {
            openCart();
            var startX = $(obj).find('.photo_src').offset().left;
            var startY = $(obj).find('.photo_src').offset().top;
            $(obj).addClass('get');
            move_icon(startX, startY);
            getData_toto_cart(obj, equipmentIds);
            //购物车添加数据库
            var cartType = 'add';
            cartAD(equipmentIds, cartType, type);
        }
        else {
            openCart();
            $(obj).removeClass('get');
            delete_to_cart('cart_' + equipmentIds);
            //购物车删除数据库
            var cartType = 'del';
            cartAD(equipmentIds, cartType, type);
        }
        getCartNum();
    })
}


//获取购物车产品数
function getCartNum() {
    $('#addto_cart').text($('#shopping_cart_chose_list .shopping_cart_chose_list_content_lier').length);
    $('.toufang_num').text($('#shopping_cart_chose_list .shopping_cart_chose_list_content_lier').length);
    $('#addto_cart').text($('#shopping_cart_chose_list .shopping_cart_chose_list_content_lier').length);
}

//检测显示那个价格
function checkPrice() {
    var priceCart = 0;
    $('.cart_price').hide();
    $('.dtw_price').show();
    var a = $(".shopping_cart_chose_list_content_lier");
    a.each(function () {
        var id = $(this).attr("id");
        var price = $("#" + id).find('.dtw_price').text();
        var priceString = price.split("￥");
        var priceD = priceString[priceString.length - 1]
        if (isNaN(priceD)) {
            priceD = 0;
        }
        priceCart += Number(priceD);
    });
    var priceCart = priceCart.toFixed(2);

    $(".price").html("￥" + priceCart);
}

//删除购物车
function deleteCart(playerId) {
    delete_to_cart('cart_' + playerId);
    //购物车删除数据库
    var fromIs = $('#fromIs').val();
    var cartType = 'del';
    cartAD(playerId, cartType, type);
    //准确购物车数量
    getCartNum();
    checkPrice();
}

//批量删除购物车
function deleteCart_all() {
    $('#shopping_cart_chose_listContent').html('');
    var fromIs = $('#fromIs').val();
    cartAD('', 'delALL', type);
    priceCart = '0.0000';
    $(".price").html("￥" + priceCart);
    getCartNum();
    checkPrice();
}

//添加、删除购物车操作数据库
function cartAD(playerIds, cartType, type) {
    $.ajax({
        url: "/ShoppingCart/cartAD",
        data: { playerIds: playerIds, cartType: cartType, type: type },
        crossDomain: true,
        async: false,
        cache: false,
        type: "POST",
        dataType: 'json',
        success: function (data) {
            //给购物车中的隐藏文本域赋值
            $("#account_ids").val(data.accountIds);
            return true;
        },
        error: function (erg) {
            return false;
        }
    })
}



//加载购物车
function shoppingCart() {
    var scAreaStyle = $("#scArea").css("display");
    var scRecommendStyle = $("#scRecommend").css("display");
    $("#account_ids").val("");

    //页面加载时候查询购物车数
    if (type == 2) {
        $.ajax({
            type: "Post",
            url: "/ShoppingCart/AddCartList",
            data: { orderId: orderId, type: type },
            success: function (data) {
                if (data.result == "Success") {
                    getCartList();
                }
            }
        });
    }
    else {
        getCartList();
    }
}

function getCartList() {
    $.ajax({
        type: "GET",
        url: "/ShoppingCart/GetCartList",
        data: { type: type },
        success: function (obj) {
            if (!obj.playerList.length) {
                return false;
            }
            $("#shopping_cart_chose_listContent").html("");
            $.each(obj.playerList, function (i, item) {
                var div = document.createElement('div');
                div.className = 'shopping_cart_chose_list_content_lier box_h';
                var playerId = item.id;
                div.id = 'cart_' + playerId;
                var name = item.equipment.name;
                var dtwPrice = item.equipment.priceDto.price;
                var dtw_detail_url = "/EquipmentModel/GetThumbnail?id=" + item.equipment.equipmentModelId;
                var playerSort = "位置：" + item.sort + "&nbsp;&nbsp;&nbsp;&nbsp;屏幕规格：" + item.width + ' * ' + item.height;

                var html =
                    '<div class="shopping_cart_chose_list_content_lier_photo"><a href="' + dtw_detail_url + '" target="_blank">' +
                    '<img class="photo_img" id= "thimbnail' + item.id + '"' + ' src=' + dtw_detail_url + '></a></div>' +
                    '<div class="shopping_cart_chose_list_content_lier_user">' +
                    '<div class="box_h">' +
                    '<div class="shopping_cart_chose_list_content_name">' + name + '</div>' +
                    '</div>' +
                    '<h3 class="cart_player" style="margin:0px;">' + playerSort + '</h3>' +
                    '<h3 class="dtw_price cart_price" style="margin:0px;">' + '￥' + dtwPrice + '</h3>' +
                    '</div>' +
                    '<div onclick="deleteCart(\'' + playerId + '\')" class="shopping_cart_chose_list_content_lier_close"><span>+</span></div>';

                div.innerHTML = html;
                document.getElementById('shopping_cart_chose_listContent').appendChild(div);
                getCartNum();
                checkPrice();
            });
            $("#account_ids").val(obj.accountIds);
        }
    })
}


//投放广告
function cartTouFang() {
    var a = $(".shopping_cart_chose_list_content_lier");
    var PlayerIdArr = [];
    a.each(function () {
        var id = $(this).attr("id");
        var price = $("#" + id).find('.dtw_price').text();
        var PlayerIdString = id.split("_");
        var PlayerId = PlayerIdString[PlayerIdString.length - 1];
        PlayerIdArr.push(PlayerId);
    })
    PlayerIdCart = PlayerIdArr.join(",");
    var totalpriceString = $(".price").text().split("￥");
    var totalprice = totalpriceString[totalpriceString.length - 1]
    if (PlayerIdCart == "" || PlayerIdCart == 0) {
        layer.msg("购物车为空，请选择投放媒体位！", { icon: 2, time: 2000 });
        return false;
    }
    var orderId = $("#orderId").val() ? $("#orderId").val():"";
    window.location.href = 'order?c[order_id]=' + orderId + '&c[player_ids]=' + PlayerIdCart + '&c[price]=' + totalprice;
}