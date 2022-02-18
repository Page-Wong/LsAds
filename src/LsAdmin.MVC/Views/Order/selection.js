//全局变量
var EquipmentIdCart = "";
var orderId = getParameter("id");
$("#orderId").val(orderId);
if (orderId == "" || orderId == null) {
    type = 1;
}
else {
    type = 2
}

$(function () {
    GetPlaceTypes();
    loadTabels(1, 10);

    $(document).on('click', '.sub_area_click', function () {
        if (!$("#shopping_cart_chose_list").hasClass("open")) {
            $("#shopping_cart_chose_list").addClass("open");
        } else {
            $("#shopping_cart_chose_list").removeClass("open");
        }
    });
    bindCartEvent('addto_cart');

})

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


//切换分页
function show_hided(showId, hideId, hideIdo, hideRe, o) {
    $('#' + showId).show();
    $('#' + hideId).hide();
    $('#' + hideIdo).hide();
    $('#' + hideRe).hide();
    $(o).addClass('this');
    $(o).nextAll().removeClass('this');
    $(o).prevAll().removeClass('this');
    $('body').css({ 'min-height': '0' })
}


function GetPlaceTypes() {
    $.ajax({
        type: "Get",
        url: "/PlaceType/GetGridData",    //获取数据的ajax请求地址
        success: function (data) {
            $('#PlaceType').append('<option value="">' + "场所类型" + '</option>');
            $.each(data, function (i, item) {
                $('#PlaceType').append('<option value="' + item.id + '">' + item.text + '</option>');
            });
        }
    })
}

function loadTabels(startPage, pageSize) {
    $("#allData").html("");
    $.ajax({
        type: "GET",
        url: "/Equipment/GetEqipment?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                var div = '<div class="search_result_lier box_h" gettype="search" id=equipment_' + item.id + ' dtw_price=' + item.priceDto.price + ' dtw_detail_url="/EquipmentModel/GetThumbnail?id=' + item.equipmentModelId + '">';
                div += '<div class="search_result_name_area padding_left_one">';
                div += '<input type="checkbox" class="addto_cart" name=equipment_' + item.name + ' data-id=' + item.id + '>';
                div += '<input type="hidden" name="muid" value=' + item.id + '>';
                div += '<a href=' + 'target="_blank">';
                div += '<div class="search_result_photo photo_src collection_float">';
                div += '<img class="photo_img" id="thimbnail' + item.id + '"' + 'src="/EquipmentModel/GetThumbnail?id=' + item.equipmentModelId + '">';
                div += '</div>';
                div += '</a>';
                div += '<div class="search_result_user">';
                div += '<a href=' + 'target="_blank">';
                div += '<div class="collection_float">';
                div += '<div class="search_result_user_name shopping_cart_chose_listName" title=' + item.name + '>' + item.name + '</div>';
                div += '</div>';
                div += '<div class="collection_float">';
                div += '<div class="search_result_user_second_title shopping_cart_chose_list_second_sitle" title=' + (item.placeDto.name == null ? '' : item.placeDto.name) + '>' + (item.placeDto.name == null ? '' : item.placeDto.name) + '</div>';
                div += '</div>';
                div += '</a>';
                div += '</div>';
                div += '</div>';
                div += '<div class="search_result_value box_value">';
                div += '<div class="flex1"></div>';
                div += '<div>' + (item.province == null ? '' : item.provinceName) + (item.cityName == null ? '' : item.cityName) + (item.districtName == null ? '' : item.districtName) + (item.street == null ? '' : item.street) + (item.streetNumber == null ? '' : item.streetNumber) + '</div>';
                div += '<div class="flex1"></div>';
                div += '</div>';
                div += '<div class="search_result_value box_value">';
                div += '<div class="flex1"></div>';
                div += '<div>' + item.equipmentModelDto.applyTo + '</div>';
                div += '<div class="flex1"></div>';
                div += '</div>';
                div += '<div class="search_result_value box_value">';
                div += '<div class="flex1"></div>';
                div += '<div>' + item.equipmentModelDto.screenSize + '</div>';
                div += '<div class="flex1"></div>';
                div += '</div>';
                div += '<div class="search_result_value box_value">';
                div += '<div class="flex1"></div>';
                div += '<div>' + item.equipmentModelDto.screenMaterial + '</div>';
                div += '<div class="flex1"></div>';
                div += '</div>';
                div += '<div class="search_result_value box_value">';
                div += '<div class="flex1"></div>';
                div += '<div>' + item.equipmentModelDto.voicedName + '</div>';
                div += '<div class="flex1"></div>';
                div += '</div>';
                div += '<div class="search_result_value box_value">';
                div += '<div class="flex1"></div>';
                div += '<div>' + item.equipmentModelDto.sideScreensNumber + '</div>';
                div += '<div class="flex1"></div>';
                div += '</div>';
                div += '<div class="search_result_value box_value">';
                div += '<div class="flex1"></div>';
                div += '<div class="red-color">' + '￥' + item.priceDto.price + '</div>';
                div += '<div class="flex1"></div>';
                div += '</div>';
                div == '</div>';
                $("#allData").append(div);
            })
            shoppingCart();
            /*var pg = new showPages('pg');
            pg.pageCount = 10;
            pg.argName = 'p';
            pg.printHtml();*/


            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadTabels(newPage, pageSize);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
        }
    })
}

//投标触发提交
function cartTouFang() {
    var a = $(".shopping_cart_chose_list_content_lier");
    var EquipmentIdArr = [];
    a.each(function () {
        var id = $(this).attr("id");
        var price = $("#" + id).find('.dtw_price').text();
        var EquipmentIdString = id.split("_");
        var EquipmentId = EquipmentIdString[EquipmentIdString.length - 1];
        EquipmentIdArr.push(EquipmentId);
    })
    EquipmentIdCart = EquipmentIdArr.join(",");
    var totalpriceString = $(".price").text().split("￥");
    var totalprice = totalpriceString[totalpriceString.length - 1]
    if (EquipmentIdCart == "" || EquipmentIdCart == 0) {
        layer.msg("购物车为空，请选择投放媒体位！", { icon: 2, time: 2000 });
        return false;
    }
    var orderId = $("#orderId").val();
    window.location.href = 'neworder?c[order_id]=' + orderId + '&c[equipment_ids]=' + EquipmentIdCart + '&c[price]=' + totalprice;
}

//滚动效果
$(window).scroll(function () {
    var d = $('#xxk_btn_area_box .this').attr('data-list');
    var scrollT = $(window).scrollTop();
    if (d == 1 || d == 3) {
        if (scrollT > 713) {
            $('.filter_box').addClass('locate');
            $('.scrolltop').addClass('smallnavf');
        }
        else {
            $('.filter_box').removeClass('locate');
            $('.scrolltop').removeClass('smallnavf');
        }
    } else {
        if (scrollT > 913) {
            $('.filter_box').addClass('locate');
            $('.scrolltop').addClass('smallnavf');
        }
        else {
            $('.filter_box').removeClass('locate');
            $('.scrolltop').removeClass('smallnavf');
        }
    }
});

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
                if (data.reult == "Success") {
                    return true;
                }
            }
        });
    }
    $.ajax({
        type: "GET",
        url: "/ShoppingCart/GetCartList",
        data: { type: type },
        success: function (obj) {
            if (!obj.equipmentList.length) {
                categoryStyle()
                return false;
            }
            //判断是那种显示模式下的（收藏/全部）
            var equipmentBQ = "sc_equipment_";
            if (scAreaStyle == "none" && scRecommendStyle == "none") {
                equipmentBQ = "_";
                equipmentBQ = "equipment_";
            }
            else if (scRecommendStyle == "block") {
                equipmentBQ = "tj_euipment_";
            }
            $("#shopping_cart_chose_listContent").html("");
            $.each(obj.equipmentList, function (i, item) {
                var div = document.createElement('div');
                div.className = 'shopping_cart_chose_list_content_lier box_h';
                var equipmentId = item.id;
                div.id = 'cart_' + equipmentId;
                var name = item.name;
                var dtwPrice = item.priceDto.price;
                var dtw_detail_url = "/EquipmentModel/GetThumbnail?id=" + item.equipmentModelId;

                var html =
                    '<div class="shopping_cart_chose_list_content_lier_photo"><a href="' + dtw_detail_url + '" target="_blank">' +
                    '<img class="photo_img" id= "thimbnail' + item.id + '"' + ' src=' + dtw_detail_url + '></a></div>' +
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
                //$('#'+ids).addClass("search_result_lier box_h get");
                //$('#'+ids).find('.addto_cart').click();
                //给购物车中的隐藏文本域赋值                
            });
            $("#account_ids").val(obj.accountIds);
            categoryStyle();
        }
    })
}