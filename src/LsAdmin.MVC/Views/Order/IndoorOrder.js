var selectedRole = 0;
var pageSize = 10;

$(function () {
    selectLabel();
    datePickerStyle();
    chooselabel();
    loadTimeRange();
    initTooltip();

    //地区联动
    loadProvince();
    $("#Province").on('change', function (e) {
        loadCity($(this).val());
        //provincename = ($(this).selectedIndex).text;
    });
    $("#City").on('change', function (e) {
        loadDistrict($("#Province").val(), $(this).val());
    });  

    //选择方案
    $("#choose_area").click(function () { selectModal(); })

    var programTableConfig = {
        tableBodyId: "selectBody",
        selectCallback: function (id, displayName, duration, width, height) {
            $("#select_" + id).click();
        }
    };
    ProgramSelectModal.init(programTableConfig);
});

//提示框初始化
function initTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
    $("#input_name").attr("data-original-title", "字数不能超过50个字且不能为空");
    $("#input_introduce").attr("data-original-title", "可为空，正确网址格式如：www.lsinfo.com");
    $("#input_totaltime").attr("data-original-title", "广告时长最短10s,最长3600s");

}


//***1.创建方案*********************************************************

//标签选择方法
function selectLabel() {
    $.ajax({
        type: "GET",
        url: "/Order/AdsListOption",
        success: function (data) {
            $.each(data, function (i, item) {
                if (item.name != "其他") {
                    var tr = "<div class='abc-li'>" + (item.name == null ? "" : item.name) + "</div>";
                    $("#select_label").append(tr);
                }
            })
            $.each(data, function (i, item) {
                if (item.name == "其他") {
                    var tr = "<div class='abc-li'>" + (item.name == null ? "" : item.name) + "</div>";
                    $("#select_label").append(tr);
                }
            })
            var i = data.length-1;
            $($("#select_label").find(".abc-li")[i]).addClass("abc-li-on");
        }
    })
}

//广告标签选择效果
function chooselabel() { 
    $(document).on('click', '.abc-li', function () {
        var labelArr = [];
        ($("#select_label").children(".abc-li-on")).each(function () {
            var text = $(this).html();
            labelArr.push(text);
        })
        if (!$(this).hasClass("abc-li-on")) {
            if (labelArr.length < 3) {
                $(this).addClass("abc-li-on");              
            }
        } else {
            $(this).removeClass("abc-li-on");
        }
    });
}

//***2.推广区域与时间*********************************************************

//省地县三级联动
function loadProvince() {
    $.get(
        '/Order/GetProvinces',
        function (data) {
            if (data.result == 'Success') {
                $('#Province').append('<option value="">' + "--请选择--" + '</option>');
                $.each(data.data, function (i, item) {
                    $('#Province').append('<option value="' + item.code + '">' + item.name + '</option>');
                });
            }
        }
    );
}

function loadCity(province, selected, callback) {
    if (province == null || province == '') {
        return false;
    }
    $("#District").val('');
    $('#District').empty();
    $("#City").val('');
    $('#City').empty();
    $.get(
        '/Order/GetCitys?province=' + province,
        function (data) {
            if (data.result == 'Success') {
                $.each(data.data, function (i, item) {
                    $('#City').append('<option value="' + item.code + '">' + item.name + '</option>');
                });
                $('#City').val(selected);
                eval(callback);
            }
        }
    );
}

function loadDistrict(province, city, selected, callback) {
    if (city == null || city == '') {
        return false;
    }
    $("#District").val('');
    $('#District').empty();
    $.get(
        '/Order/GetDistricts?province=' + province + '&city=' + city,
        function (data) {
            if (data.result == 'Success') {
                $.each(data.data, function (i, item) {
                    $('#District').append('<option value="' + item.code + '">' + item.name + '</option>');
                });
                $('#District').val(selected);
                eval(callback);
            }
        }
    );
}

//添加地区
function addArea() {
    var province = $("#Province").find("option:selected").text();
    var city = $("#City").find("option:selected").text();
    var district = $("#District").find("option:selected").text();
    var provinceCode = $("#Province").val();
    var cityCode = $("#City").val();
    var districtCode = $("#District").val() ;
    if (province =='--请选择--') {
        layer.alert('请选择省份');
    }
    else {
        if (province == null || province == '') {
            $("#Province").focus();
        }
        else {
            var area = province + ' ' + city + ' ' + district;
            var areaCode = provinceCode + '_' + cityCode + '_' + districtCode;
            var areaCodeArr = [];
            $($('#selectedarea').children('.seledCityItem')).each(function () {
                areaCodeArr.push($(this).attr('value'));
            });
            if ($.inArray(areaCode, areaCodeArr) == -1) {
                var tr = '<span class="seledCityItem" value="' + areaCode + '" name="area">' + area +
                      '<a href="javascript:void(0);" class="seledAreaClose" onclick="seledAreaClose(this)">&nbsp;&nbsp;</a>';
                tr += '<input hidden="hidden" type="text" class="seledProvince" value="' + $("#Province").val() + '">';
                tr += '<input hidden="hidden" type="text" class="seledCity" value="' + $("#City").val() + '">';
                tr += '<input hidden="hidden" type="text" class="seledDistrict" value="' + $("#District").val() + '">';
                tr += '</span > ';
                $("#selectedarea").append(tr);
                $("#District").val('');
            }
            else {
                layer.alert('已选择该地区，不需重复选择');
            }
        }
    }
}

//移除地区
function seledAreaClose() {
    $(document).on('click', '.seledAreaClose', function () {
        var a = $(this).parent().remove(); //.parentElement.html();
    })
}


//***3.推广时间*********************************************************
//加载时间段
function loadTimeRange() {
    $.get(
        '/Order/GetAreaPlayPrices',
        function (data) {
            $.each(data, function (i, item) {
                var timerange = item.timeRange;
                var timerangeArray = timerange.split(",");


                var tr = '<div class="box box-success box-solid timeRange ' + item.timeRangeType + '">';
                tr += '    <div class="box-header with-border">';
                tr += '        <h3 class="box-title">请点击选择/取消可播放的' + item.timeRangeType + '时间段(单价为:' + item.price+'元/秒)</h3>';
                tr += '    </div>';
                tr += '    <div class="box-body">';
                tr += '         <div class="row" id= "timerangetype_' + item.id + '">';
                tr += '             <input type="text" hidden="hidden" name="timerangetype_price" value="' + item.price + '">';
                tr += '             <input type="text" hidden="hidden" name="timerangetype_id" value="' + item.id + '">';
                tr += '             <input type="text" hidden="hidden" name="timerangetype_type" value="' + item.timeRangeType + '">';
                for (i = 0; i < timerangeArray.length; i++) {
                    tr += '         <div class="abc-li-timerange abc-li-timerange-on" name="timerangetype_timerange">' + timerangeArray[i] + '</div>';
                }
                tr += '         </div>';
                tr += '        <div class="form-group" id="input_count_group_' + item.id + '">';
                tr += '             播放次数：';
                //tr += '            <span id="input_count_label_' + item.id + '">1000</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
                //tr += '            <input id="input_count_label_' + item.id + '"style="width:50px" value="1000">';
                //tr += '            <input id="input_count_' + item.id + '" class="slider" type="text" data-slider-min="0" data-slider-max="10000" data-slider-step="100" data-slider-value="1000" data-toggle="tooltip"/>';
                tr += '             <input type="number" id="input_count_' + item.id + '" name="count_' + item.timeRangeType + '" value="1000" min="0" max="10000" step="1000">';
                tr += '        </div>';
                tr += '    </div>';
                tr += '</div>';
                $("#TimeRange").append(tr);

                $("#input_count_" + item.id).on("change", function (obj) {
                    loadOrderInfo();
                });

                $('#timerangetype_' + item.id).click(function () {
                    //点击事件比class变更快，延时执行判断
                    setTimeout(function () {
                        if ($('#timerangetype_' + item.id).children('.abc-li-timerange-on').length > 0) {
                            $('#input_count_group_' + item.id).removeAttr('hidden');
                        }
                        else {
                            $('#input_count_group_' + item.id).attr('hidden', 'hidden');
                        }
                    }, 100);
                });

                var infotr = '';
                infotr += '<span class="bk-items-item">';
                infotr += '<span class="bk-items-item-name">' + item.timeRangeType + '播放时间段：</span > ';
                infotr += '<span class="bk-items-item-value" id="info_timerange_' + item.timeRangeType + '">'+"无"+'</span>';
                infotr += '</span> ';
                infotr += '<span class="bk-items-item">';
                infotr += '<span class="bk-items-item-name">' + item.timeRangeType + '播放次数：</span > ';
                infotr += '<span class="bk-items-item-value" id="info_count_' + item.timeRangeType + '">' + 0 + '次</span>';
                infotr += '</span> ';
                $("#timerangeinfo").append(infotr);
            });
            $('input.slider').slider();

            $(function () {
                $(document).on('click', '.abc-li-timerange', function () {
                    if (!$(this).hasClass("abc-li-timerange-on")) {
                        $(this).addClass("abc-li-timerange-on");
                        loadOrderInfo();
                    }
                    else {
                        $(this).removeClass("abc-li-timerange-on");
                        loadOrderInfo();
                    }
                });
                loadOrderInfo();
            })
        });
}


//时间选择器的样式及方法
function datePickerStyle() {
    var nowTemp = new Date();
    var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);
    $('#input_date').daterangepicker({
        locale: {
            format: 'YYYY-MM-DD'
        },
        "autoApply": true,
        // "minDate": new Date(now.valueOf() + 24 * 60 * 60 * 1000),
        //"startDate": new Date(now.valueOf() + 24 * 60 * 60 * 1000),
        "minDate": new Date(now.valueOf()),
        "startDate": new Date(now.valueOf()),
        "endDate": new Date(now.valueOf() + 2 * 24 * 60 * 60 * 1000)
    }
    );
}


//***5.订单详情*********************************************************
//订单详情
function loadOrderInfo() {
    var params = [];
    var TimeRange = $('#TimeRange').children('.timeRange');
    var a = parseFloat($("#program_duration").val());
    time = parseFloat($("#program_duration").val()) ? parseFloat($("#program_duration").val()):0;
    document.getElementById("info_name").innerText = $("#input_name").val();
    document.getElementById("info_totaltime").innerText = time + "秒";
    TimeRange.each(function () {
        var TimeRangeArr = [];
        var timeRangeType = "";
        id = $($(this).find('[name="timerangetype_id"]')).val();
        timerangetype = $($(this).find('[name="timerangetype_type"]')).val();
        price = parseFloat($($(this).find('[name="timerangetype_price"]')).val()) ? parseFloat($($(this).find('[name="timerangetype_price"]')).val()) : 0;
        $($('#timerangetype_' + id).children('.abc-li-timerange-on')).each(function () {
            var text = $(this).html();
            TimeRangeArr.push(text);
        });
        timerange = TimeRangeArr.join(",");
        if (timerange.length > 0) {
            exposurecount = parseFloat($('#input_count_' + id).val());
        }
        else {
            exposurecount = 0;
        }
        params.push({
            Id: id,
            TimeRangeType: timerangetype,
            Price: price,
            Time: time,
            Count: exposurecount,
        })
        document.getElementById("info_timerange_" + timerangetype).innerText = timerange;
        if (exposurecount > 0) {
            document.getElementById("info_count_" + timerangetype).innerText = exposurecount + "次";
        }
        else {
            document.getElementById("info_count_" + timerangetype).innerText = "";
        }
    });
    $.ajax({
        type: "Post",
        url: "/Order/GetAmount",
        data: { dtos: params },
        dataType: 'json',
        success: function (data) {
            if (data.result == "Success") {
                document.getElementById("totalAmount").innerText = data.amount;
            }
        }
    })
}

//检查订单信息是否正确
function checkOrderInfo() {
    var name = $("#input_name").val();
    var industry = $("#input_industry").val();
    var url = $("#input_introduce").val();
    var AdsArr = [];
    ($("#select_label").children(".abc-li-on")).each(function () {
        var text = $(this).html();
        AdsArr.push(text);
    });
    var adstag = AdsArr.join(",");
    var remarks = $("#input_remarks").val();
    var id = $("#orderId").val();
    var totalseconds = $("#program_duration").val();
    var startdate = $('#input_date').data('daterangepicker').startDate.format("YYYY-MM-DD");
    var enddate = $('#input_date').data('daterangepicker').endDate.format("YYYY-MM-DD");

    DeleteBorder("choose_area");

    //用户名不能为空
    if (name == null || name == "") {
        $("#input_name").attr("data-original-title", "方案名称不能为空")
        $("#input_name").focus();
        return false;
    }

    //行业类型不为空
    if (industry == null || industry == "" || industry == "-请选择-") {
        $("#input_industry").attr("data-original-title","行业类型不能为空");
        $("#input_industry").focus();
        return false;
    }

    //广告标签不能为空
    if (adstag == null || adstag == "") {
        //layer.alert("请选择广告标签");
        $("#select_label").attr("data-original-title", "请选择广告标签");
        $("#select_label").focus();
        return false;
    }


    //网址地址格式校验
    if (url != null && url != "") {
        //var reg = /^([hH][tT]{2}[pP]:\/\/|[hH][tT]{2}[pP][sS]:\/\/)(([A-Za-z0-9-~]+)\.)+([A-Za-z0-9-~\/])+$/;
        var reg = /(([A-Za-z0-9-~]+)\.)+([A-Za-z0-9-~\/])+$/;
        if (!reg.test(url)) {
            $("#input_introduce").attr("data-original-title","您输入的:" + url + "网址格式有误，请输入正确网址！");
            $("#input_introduce").focus();
            return false;
        };
    }
    
    var orderInfo = {
        Id: id ? id : "00000000-0000-0000-0000-000000000000",
        Name: name,
        Industry: industry,
        Url: url,
        AdsTag: adstag,
        Name: name,
        TotalSeconds: totalseconds,
        Remarks: remarks,
        Amount: $('#totalAmount').html(),
        Status: 1,
        Type: 11,
        StartDate: startdate,
        EndDate: enddate,
        }
    return orderInfo;
}

function checkOrderArea() {
    var orderAreas = [];
    var Area = $("#selectedarea").children('.seledCityItem');
    //区域不能为空
    if (Area.length != 0) {
        Area.each(function () {
            var province = $($(this).children('.seledProvince')).val();
            if ($($(this).children('.seledCity')).val() == "null") {
                var city = "";
            }
            else {
                var city = $($(this).children('.seledCity')).val();
            }
            if ($($(this).children('.seledDistrict')).val() == "null") {
                var district = "";
            }
            else {
                var district = $($(this).children('.seledDistrict')).val();
            }
            orderAreas.push({
                Province: province,
                City: city,
                District: district,
            })
        })
    }
    else {
        $("#Province").attr("data-original-title", "请选择投放区域");
        $("#Province").focus();
        return false;
    }
    return orderAreas;
}
function checkOrderTimes() {
    var orderTimes = [];
    var TimeRangeArr = [];
    var AllTimeRangeArr = [];
    var TimeRange = $('#TimeRange').children('.timeRange');
    var timeRangeType = "";
    
    var startdate = $('#input_date').data('daterangepicker').startDate.format("YYYY-MM-DD");
    var enddate = $('#input_date').data('daterangepicker').endDate.format("YYYY-MM-DD");

    TimeRange.each(function () {
        id = $($(this).find('[name="timerangetype_id"]')).val();
        timerangetype = $($(this).find('[name="timerangetype_type"]')).val();
        exposurecount = $('#input_count_' + id).val();
        $($('#timerangetype_' + id).children('.abc-li-timerange-on')).each(function () {
            var text = $(this).html();
            AllTimeRangeArr.push(text);
        });
    })


    //时间段不能为空
    if (AllTimeRangeArr.length == 0) {
        layer.open({
            content: '请选择播放时间段！',
            btn: ['确认'],
            yes: function (index) {
                layer.close(index);               
                changeCss($("#TimeRange"));
                $("#TimeRange").focus();
                $("#TimeRange").attr("data-original-title", "请选择播放时间段");
            }
        })
        
        return false;
    }
    
    //播放次数不能为空
    function checkTimeRangeCount() {
        var result = 0;
        TimeRange.each(function () {
            id = $($(this).find('[name="timerangetype_id"]')).val();           
            exposurecount = $('#input_count_' + id).val();
            timerangecount = $($('#timerangetype_' + id).children('.abc-li-timerange-on')).length;
            if (timerangecount > 0 && exposurecount == "0") {
                timerangetype = $($(this).find('[name="timerangetype_type"]')).val();
                layer.open({
                    content: '请选择' + timerangetype +'时间段播放次数或者取消已选择的' + timerangetype+'时间段',
                    btn: ['确认'],
                    yes: function (index) {
                        layer.close(index);
                        changeCss($("#TimeRange"));
                        $("#TimeRange").focus();
                        $("#TimeRange").attr("data-original-title", "请选择" + timerangetype + "时间段的播放次数或者取消已选择的" + timerangetype+"时间段");
                    }
                })                
                result = 1;
            }
            else if (parseFloat(exposurecount)<0) {
                timerangetype = $($(this).find('[name="timerangetype_type"]')).val();
                layer.open({
                    content: ''+timerangetype + '时间段播放次数必须大于0',
                    btn: ['确认'],
                    yes: function (index) {
                        layer.close(index);
                        changeCss($("#TimeRange"));
                        $("#TimeRange").focus();
                        $("#TimeRange").attr("data-original-title", ""+timerangetype + "时间段播放次数必须大于0");
                    }
                })
                result = 1;
            }
        });
        return result;
    }

    var timerangecount = checkTimeRangeCount();
    if (timerangecount == 1) {
        return false;
    }

    //获取数据
    var areaArr = [];
    $($('#selectedarea').children('.seledCityItem')).each(function () {
        var text = $(this).attr("value");
        areaArr.push(text);
    })
    var area = areaArr.join(",");

    TimeRange.each(function () {
        TimeRangeArr = [];
        id = $($(this).find('[name="timerangetype_id"]')).val();
        timerangetype = $($(this).find('[name="timerangetype_type"]')).val();
        price = parseFloat($($(this).find('[name="timerangetype_price"]')).val()) ? parseFloat($($(this).find('[name="timerangetype_price"]')).val()) : 0;
        $($('#timerangetype_' + id).children('.abc-li-timerange-on')).each(function () {
            var text = $(this).html();
            TimeRangeArr.push(text);
        });
        timerange = TimeRangeArr.join(",");
        if (timerange.length > 0) {
            exposurecount = $('#input_count_' + id).val();
        }
        else {
            exposurecount = 0;
        }
        orderTimes.push({
            Area: area,
            StartDate: startdate,
            EndDate: enddate,
            TimerangeType: timerangetype,
            ExposureCount: exposurecount,
            TimeRange: timerange,
            UnitPrice: price,
        })
    });

    return orderTimes;
}

//检查订单方案是否正确
function checkProgram() {
    var post_program = $("#programId").val();
    if (!post_program) {
        layer.open({
            content: '请选择方案',
            btn: ['确认'],
            yes: function (index) {
                layer.close(index);
                AddBorder($("#choose_area"));
                $("#choose_area").focus();
            }
        })
        return false;
    }
    var programId = $("#programId").val();
    return programId;
}


//保存方案
function saveOrder() {

    var order = checkOrderInfo();
    if (!order) {
        return;
    }

    var orderAreas = checkOrderArea();
    if (!orderAreas) {
        return;
    }
    
    var orderTimes = checkOrderTimes();
    if (!orderTimes) {
        return;
    }

    var programID = checkProgram();
    if (!programID) {
        return;
    }


    $.ajax({
        type: "POST",
        url: "/Order/SaveOrder",
        data: { order: order, programID: programID, orderAreas: orderAreas,orderTimes: orderTimes },
        success: function (data) {
            if (data.result == "Success") {
                layer.open({
                    content: '保存成功',
                    btn: ['确认'],
                    yes: function (index) {
                        layer.close(index);
                        window.location.href = "List";
                    }
                }) 
            }
            else {
                layer.alert(data.message);
            }
        }
    });
}

function Pay() {
    var order = checkOrderInfo();
    if (!order) {
        return;
    }

    var orderAreas = checkOrderArea();
    if (!orderAreas) {
        return;
    }

    var orderTimes = checkOrderTimes();
    if (!orderTimes) {
        return;
    }

    var programID = checkProgram();
    if (!programID) {
        return;
    }


    $.ajax({
        type: "POST",
        url: "/Order/SaveOrder",
        data: { order: order, programID: programID, orderAreas: orderAreas, orderTimes: orderTimes },
        success: function (data) {
            if (data.result == "Success") {
                $("#orderId").val(data.id);
                 pay(data.orderNo, data.amount, data.name, data.id);                   
            }
            else {
                layer.alert(data.message);
            }
        }
    });
}

function changeCss(selector) {
    selector.css("border", "2px solid red");
}

function DeleteBorder(selector) {
    var obj = document.getElementById(selector);
    obj.style.border = "none";
}

$(function () {
    $("#TimeRange").mouseleave(function () {
        $("#TimeRange").css("border", "0px solid red");
    });
    $("#choose_area").mouseleave(function () {
        $("#choose_area").css("border", "0px solid red");
    });
})

//付款
function pay(orderNo, amount, subject, orderId) {
    $('#totalAmout').val(amount);
    $('#orderNo').val(orderNo);
    $('#subject').val(subject);
    $('#payOrderId').val(orderId);   
    $("#tradeMethodModal").modal();
};




