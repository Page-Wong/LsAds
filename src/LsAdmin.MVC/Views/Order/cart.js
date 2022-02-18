
//购物车展开
var closeCartHandl;

var orderId = getParameter("id");
$("#orderId").val(orderId);
if (orderId == "" || orderId == null) {
    type = 1;
}
else {
    type = 2;
}

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

//购物车创建选中账号
function cartAddData(photo_src, name, id, cartId, price, dtw_detail_url) {
    var div = document.createElement('div');
    div.className = 'shopping_cart_chose_list_content_lier box_h';
    var equipmentId = id;
    div.id = 'cart_' + id;
    //格式化价格 
    var dtwPrice = price.dtw_price;
    var html =
        '<div class="shopping_cart_chose_list_content_lier_photo"><a href="' + dtw_detail_url + '" target="_blank">' + photo_src + '</a></div>' +
        '<div class="shopping_cart_chose_list_content_lier_user">' +
        '<div class="box_h">' +
        '<div class="shopping_cart_chose_list_content_name">' + name + '</div>' +
        '</div>' +
        '<h3 class="dtw_price cart_price" style="margin:0px;">' + '￥' + dtwPrice + '</h3>' +
        '</div>' +
        '<div onclick="deleteCart(\'' + equipmentId + '\')" class="shopping_cart_chose_list_content_lier_close"><span>+</span></div>';
    div.innerHTML = html;
    document.getElementById('shopping_cart_chose_listContent').appendChild(div);
    getCartNum();
    checkPrice();
    //closeCart();
}

//向购物车添加账号事件
function getData_toto_cart(obj, equipmentIds) {
    var _this = obj;
    var photo_src = $(_this).find('.photo_src').html().replace("height:150px","height:43px");
    var name = $(_this).find('.shopping_cart_chose_listName').text();
    var id = equipmentIds; //_this.id;
    var cartId = _this.id;
    var price = {
        dtw_price: $(_this).attr('dtw_price')
    }
    dtw_detail_url = $(_this).attr('dtw_detail_url');
    cartAddData(photo_src, name, id, cartId, price, dtw_detail_url);
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
        //$('.' + sclass).bind('click',function(){//console.log(333)
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

//购物车过渡
function move_icon(startX, startY) {
    var endX = document.body.clientWidth - 210;
    var endY = $(document).scrollTop() + 330;
    $('#move_icon').css({ 'opacity': 1 })
    $('#move_icon').css({ 'left': startX + "px", 'top': startY + "px" }).animate({ left: endX + "px", top: endY + "px" }, 800, function () {
        $('#move_icon').css({ 'opacity': 0 })
    });

}

//获取购物车产品数
function getCartNum() {
    $('#addto_cart').text($('#shopping_cart_chose_list .shopping_cart_chose_list_content_lier').length);
    $('.toufang_num').text($('#shopping_cart_chose_list .shopping_cart_chose_list_content_lier').length);
    $('#addto_cart').text($('#shopping_cart_chose_list .shopping_cart_chose_list_content_lier').length);
    //$('.contrast_two').text($('#shopping_cart_chose_list .shopping_cart_chose_list_content_lier').length);
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
function deleteCart(equipmentId) {
    //$('#'+id).find('.addto_cart').click();
    delete_to_cart('cart_' + equipmentId);
    //购物车删除数据库
    var fromIs = $('#fromIs').val();
    var cartType = 'del';
    cartAD(equipmentId, cartType, type);
    var equipmentBQ = "sc_equipment_";
    //判断是那种显示模式下的（收藏/全部）
    var scAreaStyle = $("#scArea").css("display");
    var scRecommendStyle = $("#scRecommend").css("display");
    if (scAreaStyle == "none" && scRecommendStyle == "none") {
        equipmentBQ = "equipment_";
    }
    else if (scRecommendStyle == "block") {
        equipmentBQ = "tj_equipment_";
    }
    //取消选中样式
    $("#" + equipmentBQ + '' + equipmentId).removeClass('get');
    $('#' + equipmentBQ + '' + equipmentId).find('.addto_cart').prop("checked", false);
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
    $('.addto_cart').each(function () {
        $(this).prop('checked', false);
    });
    $('.addto_cart_all').prop('checked', false);
    $('.item').removeClass('get');
    getCartNum();
    checkPrice();
    //$('.shopping_cart_chose_list_content_lier_close').click();
}

//添加、删除购物车操作数据库
function cartAD(equipmentIds, cartType, type) {
    $.ajax({
        url: "/ShoppingCart/cartAD",
        data: { equipmentIds: equipmentIds, cartType: cartType, type: type },
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



//点击分类获取样式并获取需要的值
function categoryStyle() {

    var scAreaStyle = $("#scArea").css("display");
    var schemdStyle = $("#schemd").css("display");

    var accountIds = $("#account_ids").val().split(",");
    //getChart();
    if (scAreaStyle == "none" && schemdStyle == "none")
    {
        for (var x = 0; x < accountIds.length; x++) {
            var checkboxName = "equipment_" + accountIds[x];
            var checkboxDataId = accountIds[x];
            $(":checkbox[data-id='" + checkboxDataId + "']").prop("checked", true);
            $("#" + checkboxName).addClass("item get");
        }
    }
    else if (scAreaStyle == "block")
    {
        for (var x = 0; x < accountIds.length; x++) {
            var checkboxName = "sc_equipment_" + accountIds[x];
            var checkboxDataId = accountIds[x];
            $(":checkbox[data-id='" + checkboxDataId + "']").prop("checked", true);
            $("#" + checkboxName).addClass("item get");
        }
    }
    else if (schemdStyle == "block") {
        for (var x = 0; x < accountIds.length; x++) {
            var checkboxName = "hmd_equipment_" + accountIds[x];
            var checkboxDataId = accountIds[x];
            $(":checkbox[data-id='" + checkboxDataId + "']").prop("checked", true);
            $("#" + checkboxName).addClass("item get");
        }
    }
}

//点击加入购物车按钮
function addtoCart(checkboxDataId) {
    if ($(":checkbox[data-id='" + checkboxDataId + "']")[0].checked == false) {
        $(":checkbox[data-id='" + checkboxDataId + "']").click();
    }
}
