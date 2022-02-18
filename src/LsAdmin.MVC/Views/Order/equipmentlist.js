//全局变量
var EquipmentIdCart = "";

var orderId = getParameter("id");
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
$("#orderId").val(orderId);
if (orderId == "" || orderId == null) {
    type = 1;
}
else {
    type = 2;
}

/*初始化*/
$(function () {
    GetPlaceTypes();
    loadList();

    $(document).on('click', '.sub_area_click', function () {
        if (!$("#shopping_cart_chose_list").hasClass("open")) {
            $("#shopping_cart_chose_list").addClass("open");
        } else {
            $("#shopping_cart_chose_list").removeClass("open");
        }
    });
    bindCartEvent('addto_cart');
})

//切换分页
function show_hided(showId, hideId, hideIdo, hideRe, o) {
    $('#' + showId).show();
    $('#' + hideId).hide();
    $('#' + hideIdo).hide();
    $('#' + hideRe).hide();
    $(o).addClass('this');
    $(o).nextAll().removeClass('this');
    $(o).prevAll().removeClass('this');
    $('body').css({ 'min-height': '0' });
}

/*获取场所类型*/
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

//查看全部媒体主的列表
$(".search_result_xxk_btn1").click(function () {
    CategoryStyle("all", 1, 10);
});

/*收藏-----------------------------------------------------------------------------------------------*/
$(".search_result_xxk_btn2").click(function () {
    CategoryStyle("collection", 1, 10);

});

/*黑名单-----------------------------------------------------------------------------------------------*/
$(".search_result_xxk_btn3").click(function () {
    CategoryStyle("blacklist", 1, 10);
});


/*加载设备列表*/
function loadList() {
    var scAreaStyle = $("#scArea").css("display");
    var schemdStyle = $("#schemd").css("display");
    if (scAreaStyle == "none" && schemdStyle == "none") {
        CategoryStyle("all", 1, 10);
    }
    else if (scAreaStyle == "block") {
        CategoryStyle("collection", 1, 10);
    }
    else if (schemdStyle == "block") {
        CategoryStyle("blacklist", 1, 10);
    }
}

function CategoryStyle(category,startPage,pageSize) {
    $.ajax({
        type: "GET",
        url: "/Equipment/GetCategoryEquipments?category=" + category + "&startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(),
        success: function (data) {
            var elment = "";     
            if (data.category == "all") {
                $("#allData").html("");
                elment = $("#grid_paging_part");
            }
            else if (data.category == "collection") {
                $("#shoucangData").html("");
                elment = $("#sc_grid_paging_part");
            }
            else if (data.category == "blacklist") {
                $("#heimingdanData").html("");
                elment = $("#hmd_grid_paging_part");
            }
            setformat(data);
            shoppingCart();
          
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        CategoryStyle(category,newPage, pageSize);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
        }
    })
}

function setformat(data) {
    var category = data.category;
    if (data.category == "all") {           
        var categorytitle = "equipment_";
    }
    else if (data.category == "collection") {
        var categorytitle = "sc_equipment_";
    }
    else if (data.category == "blacklist") {
        var categorytitle = "hmd_equipment_"
    }

    $.each(data.rows, function (i, item) {
        //获取图片
        $.ajax({
            type: "GET",
            url: "/Equipment/GetImagePageList?equipmentId=" + item.equipmentModelId + "&placeId=" + item.placeDto.id + "&startPage=1&pageSize=50&_t=" + new Date().getTime(),
            cache: true,
            async: false,
            success: function (data) {
                Materials = data;
            }
        })
        var placeli = "";
        placeli += "        <div class='photo_img' >";
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

        if (Materials.rowCount > 0) {
            $.each(Materials.rows, function (i, item) {
                if (i == 0) {
                    placeli += "                            <div class='item active photo_src' style='height:160px'>";
                } else {
                    placeli += "                            <div class='item' style='height:160px;text-align:center;'>";
                }
                placeli += "                                <img style='height:150px' src='/Place/GetThumbnail?id=" + item.id + "'/>";
                placeli += "                            </div>";
            });
        } else {
            placeli += "                            <div class='item active' style='height:160px;text-align:center;'>";
            placeli += "                                <img  style='height:150px' src='/Place/GetThumbnail?id=" + item.id + "'/>"
            placeli += "                            </div>";
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

        //获取场所信息
        var placeIntroduction = "";
        placeIntroduction += "<div> <strong>地址: </strong><u>" + (item.placeDto.address == null ? "" : item.placeDto.address) + "</u></div>";
        placeIntroduction += "<div text-black> <strong>类型: </strong>" + (item.placeDto.typeName == null ? "" : item.placeDto.typeName) + "</div>";
        placeIntroduction += "<div text-black> <strong>播放时间: </strong><small>" + (item.placeDto.timeRange == null ? "" : item.placeDto.timeRange) + "</small></div>";
        placeIntroduction + "<div text-black> <strong>标签: </strong>"
        var placeTags = item.placeDto.placeTag == null ? "" : item.placeDto.placeTag.split(",");
        if (placeTags != "") {
            $.each(placeTags, function (i, item) {
                placeIntroduction += "<small class='label label-info'>" + item + "</small>";
            })
        }
        placeIntroduction += "<div text-black> <strong>介绍：</strong><small><br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + (item.placeDto.introduction == null ? "" : item.placeDto.introduction) + "</small></div>";

        var li = "<li class='item' id=" + categorytitle + item.id + " dtw_price=" + item.priceDto.price + " dtw_detail_url=' / EquipmentModel / GetThumbnail ? id = " + item.equipmentModelId + "'>";
        li += "<div class='padding_left_checkbox '>";
        li += "<input type='checkbox' class='addto_cart' name=" + categorytitle + item.name + " data-id=" + item.id + ">";
        li += "<input type='hidden' name='muid' value=" + item.id + ">";
        li += "</div>";
        li += placeli;
        li += "<div class='product-info both-margin'>";
        li += "<b class='text-blue shopping_cart_chose_listName'>" + (item.name == null ? "" : item.name) + "</b>&nbsp;&nbsp;&nbsp;&nbsp;";
        li += "<a  id='p_" + item.id + "' class='products-title btn-link'>" + item.placeDto.name + "(" + item.placeDto.address + (item.placeContact == null ? "" : "--" + item.placeContact) + (item.placeContactPhone == null ? "" : "--" + item.placeContactPhone) + ")</a>&nbsp;&nbsp;&nbsp;&nbsp;";
        li += "<a id='em_" + item.id + "' class='products-title btn-link'>" + (item.equipmentModelDto.manufacturer == null ? "" : item.equipmentModelDto.manufacturer + "：") + item.equipmentModelDto.model + "</a>&nbsp;&nbsp;&nbsp;&nbsp;";
        li += "<br class='product-description text-red'><b class='text-red'>价格：" + item.priceDto.price + "/天</b></br>";
        li += placeIntroduction;
        li += "</div >";
        

        if (category == "all") {
            li += "<div class='tools display'>";
            li += "<a id='cartbutton_" + item.id + "href='javascript: ;' title= '加入购物车' style= 'margin-right:5px; display: inline - block; ' onclick = 'addtoCart(\"" + item.id + "\")' > <i class='iconfont'>&#xe64e;</i></a > ";
            if (item.favoriteType == 1) {
                li += "<a id='scbutton_" + item.id + "' href='javascript: ; ' title='取消收藏'  style='margin-right:5px; display: inline - block; ' data-list='2' class='favorite'><i class='iconfont'>&#xe635;</i></a>";
                li += "<a id='hmdbutton_" + item.id + "' href='javascript: ;' title='加入黑名单' style='margin-right:5px; visibility: hidden;'  > <i class='iconfont'>&#xe65e;</i></a>";
            }
            else {
                li += "<a id='scbutton_" + item.id + "' href='javascript: ; ' title='收藏'  style='margin-right:5px; display: inline - block; ' data-list='1'><i class='iconfont'>&#xe6b3;</i></a>";
                li += "<a id='hmdbutton_" + item.id + "' href='javascript: ;' title='加入黑名单' style='margin-right:5px; visibility: visible; '> <i class='iconfont'>&#xe65e;</i></a>";
            }
            li += "</div>";
            li += "</li>";

            $("#allData").append(li);
            $("#scbutton_" + item.id).on("click", function () {
                Favorite(item.id, 1, this);
            })
            $("#hmdbutton_" + item.id).on("click", function () {
                Black(item.id, 1, this);
            })
        }
        else if (category == "collection") {
            li += "<div class='tools display'>";
            li += "<a id='cartbutton_" + item.id + "href='javascript: ;' title= '加入购物车' style= 'margin-right:5px; display: inline - block; ' onclick = 'addtoCart(\"" + item.id + "\")' > <i class='iconfont'>&#xe64e;</i></a > ";
            li += "<a id='sc_scbutton_" + item.id + "' href='javascript: ; ' title='取消收藏'  style='margin-right:5px; display: inline - block; ' data-list='2' class='favorite'><i class='iconfont'>&#xe635;</i></a>";
            li += "</div>";
            li += "</li>";
            $("#shoucangData").append(li);
            $("#sc_scbutton_" + item.id).on("click", function () {
                Favorite(item.id, 1, this);
            })
        }
        else {
            li += "<div class='tools display'>";
            li += "<a id='cartbutton_" + item.id + "href='javascript: ;' title= '加入购物车' style= 'margin-right:5px; display: inline - block; ' onclick = 'addtoCart(\"" + item.id + "\")' > <i class='iconfont'>&#xe64e;</i></a > ";
            li += "<a id='hmd_hmdbutton_" + item.id + "' href='javascript: ;' title='取消黑名单' style='margin-right:5px; visibility: visible; ' class='favorite'> <i class='iconfont'>&#xe65e;</i></a>";
            li += "</div>";
            li += "</li>";
            $("#heimingdanData").append(li);
            $("#hmd_hmdbutton_" + item.id).on("click", function () {
                Black(item.id, 2, this);
            })
        }

        getequipmentmodelPopoverInfo('em_' + item.id, item.equipmentModelDto);

    })
}

//投放广告
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
    window.location.href = 'order?c[order_id]=' + orderId + '&c[equipment_ids]=' + EquipmentIdCart + '&c[price]=' + totalprice;
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
                    layer.msg("收藏成功", { icon: 1, time: 1000 });
                }
                else {
                    $(o).find("i").html('&#xe6b3;');
                    $('.favorite').attr('title', '收藏');
                    $(o).removeClass('favorite');
                    if (scAreaStyle == "block") {
                        var divId = "sc_equipment_" + equipmentId;
                        $("#" + divId).remove();
                    }
                    $(o).attr("data-list", "1");
                    $("#hmdbutton_" + equipmentId).css('visibility', 'visible');
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
    var resultAreaStyle = $("#search_resultArea").css("display");
    var schemdStyle = $("#schemd").css("display");
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
                if (resultAreaStyle == "block") {
                    var divId = "equipment_" + equipmentId;
                    $("#" + divId).remove();
                }
                else if (schemdStyle == "block") {
                    var divId = "hmd_equipment_" + equipmentId;
                    $("#" + divId).remove();
                }
                $(o).addClass('favorite');
                $('.favorite').attr('title', '取消收藏');
                layer.msg("操作成功", { icon: 1, time: 1000 });
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

/*查询按钮*/
function query() {
    var scAreaStyle = $("#scArea").css("display");
    var schemdStyle = $("#schemd").css("display");

    var placetype = $("#PlaceType").val() ? $("#PlaceType").val() : "";
    var province = $("#Province").val() ? $("#Province").val() : "";
    var city = $("#City").val() ? $("#City").val() : "";
    var district = $("#District").val() ? $("#District").val() : "";
    var price = $("#PriceRange").val() ? $("#PriceRange").val() : "";
    if (scAreaStyle == "none" && schemdStyle == "none") {
        getQueryData("all",placetype, province, city, district, price, 1, 10);
    }
    else if (scAreaStyle == "block") {
        getQueryData("collection",placetype, province, city, district, price, 1, 10);
    }
    else if (schemdStyle == "block") {
        getQueryData("blacklist",placetype, province, city, district, price, 1, 10);
    }
}


function getQueryData(category,placetype, province, city, district, price,startPage, pageSize) {
    var elment = "";
    if (category == "all") {
        $("#allData").html("");
        elment = $("#grid_paging_part");
        url = "/Equipment/GetEquipmentByQuery?category=" + category + "&placeType=" + placetype + "&province=" + province + "&city=" + city + "&district=" + district + "&price=" + price + "&favorite=" + 0 + "&startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime();
    }
    else if (category == "collection") {
        $("#shoucangData").html("");
        elment = $("#sc_grid_paging_part");
        url = "/Equipment/GetEquipmentByQuery?category=" + category + "&placeType=" + placetype + "&province=" + province + "&city=" + city + "&district=" + district + "&price=" + price + "&favorite=" + 1 + "&startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime();
    }
    else if (category == "blacklist") {
        $("#heimingdanData").html("");
        elment = $("#hmd_grid_paging_part");
        url = "/Equipment/GetEquipmentByQuery?category=" + category + "&placeType=" + placetype + "&province=" + province + "&city=" + city + "&district=" + district + "&price=" + price + "&favorite=" + 2 + "&startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime();
    }
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {
            setformat(data);
            shoppingCart();

            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        getQueryData(category, placetype, province, city, district, price, newPage, pageSize);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
        }
    }) 
}