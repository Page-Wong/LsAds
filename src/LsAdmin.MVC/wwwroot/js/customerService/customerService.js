$(document).ready(function () {

    $("#customerService a").hover(function () {
        if ($(this).prop("className") == "youhui") {
            $(this).children("img.hides").show();
        } else {
            $(this).children("div.hides").show();
            $(this).children("img.shows").hide();
            $(this).children("div.hides").animate({ marginRight: '0px' }, '0');
        }
    }, function () {
        if ($(this).prop("className") == "youhui") {
            $(this).children("img.hides").hide();
        } else {
            $(this).children("div.hides").animate({ marginRight: '-163px' }, 0, function () { $(this).hide(); $(this).next("img.shows").show(); });
        }
    });

    $("#top_btn").click(function () { if (scroll == "off") return; $("html,body").animate({ scrollTop: 0 }, 600); });

    //ÓÒ²àµ¼º½ - ¶þÎ¬Âë
    $(".youhui").mouseover(function () {
        $(this).children(".customerServiceQrcode").show();
    })
    $(".youhui").mouseout(function () {
        $(this).children(".customerServiceQrcode").hide();
    });


});