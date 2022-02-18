var selectedRole = 0;
var pageSize = 10;
var orderid = getParameter("id");

$(function () {
    selectLabel();
    datePickerStyle();
    chooselabel();
    loadTimeRange();
    initTooltip();

    //地区联动
    $("#Province").on('change', function (e) {
        loadCity($(this).val());
        //provincename = ($(this).selectedIndex).text;
    });
    $("#City").on('change', function (e) {
        loadDistrict($("#Province").val(), $(this).val());
    });
    loadProvince();

    //素材类型选择
    $("input[name='MateralType']").change(function () {
        loadTables(1, pageSize, $(this).val());
        $("#selectTableBody").html("");
        $('#input_materialtotaltime').html(0);

    });

    //上传素材
    $("#btnAdd").click(function () { add(); });
    $("#btnDelete").click(function () { deleteMulti(); });
    $("#btnSave").click(function () { save(); });
    $("#btnSelect").click(function () { selectMulti() });
    $("#checkAll").click(function () { checkAll(this) });

    $("#Dir").fileinput({
        uploadUrl: "/Material/Add",
        enctype: 'multipart/form-data',
        allowedFileExtensions: ['mp4', '3gp', 'avi', 'jpeg', 'bmp', 'jpg', 'png'],//接收的文件后缀
        language: "zh",
        uploadExtraData: function (previewId, index) {
            if (!previewId) {
                return;
            }
            var obj = {};
            if ($('#' + previewId).find('video')[0] == null) {
                return obj.duration = 0;
            }
            var duration = $('#' + previewId).find('video')[0].duration;
            obj.duration = duration;
            return obj;
        }
    });

    //导入文件上传完成之后的事件
    $("#Dir").on("fileuploaded", function (event, data, previewId, index) {
        var data = data.response;
        if (data.result != "Success") {
            layer.alert('上传失败！' + data.message);
            return;
        }
        else {
            var robj = document.getElementsByName("MateralType");
            for (i = 0; i < robj.length; i++) {
                if (robj[i].checked) {
                    materialtype = robj[i].value;
                }
            }
            if (data.data.materialType == materialtype) {
                addMaterialListItem(materialtype, data.data.id, data.data.name, data.data.duration, data.data.remarks);
                selectedIds.push(data.data.id);
            }
            else {
                //toastr.error('请上传正确的素材类型');
                layer.alert("请上传正确的素材类型(素材已上传至我的推广素材，但无法选择为本次推广的播放素材。)");

            }
        }
        $("#uploadModal").modal("hide");
        $('#Dir').fileinput('reset');
    });
});

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

//获取订单基础信息
function getOrder() {
    
    $.ajax({
        type: "Get",
        url: "/Order/GetOrder?id=" + orderid + "&_t=" + new Date().getTime(),
        success: function (data) {
            if (data.result = "Success") {
                $("#orderId").val(data.order.id);
                $("#input_name").val(data.order.name);
                $("#input_totaltime").val(data.order.totalSeconds);
                $("#input_industry").val(data.order.industry);
                $("#input_introduce").val(data.order.url);
                $("#input_remarks").val(data.order.remarks);
                var a = $("#select_label").children('div');
                var ads = data.order.adsTag.split(',');
                for (i = 0; i < ads.length; i++) {
                    $("#select_label").children('div').filter(function () { return $(this).text() === ads[i]; }).addClass('abc-li-on');
                }
                $("input[name='MateralType']").removeAttr("checked");
                if (data.order.materalType == 1) {
                    $("#MateralType1").attr("checked", "checked");
                    loadTables(1, pageSize, 1);
                }
                else if (data.order.materalType == 2) {
                    $("#MateralType2").attr("checked", "checked");
                    loadTables(1, pageSize, 2);
                }
                getOrderMaterial();

            }
        }
    })
}

//获取订单区域与时间段
function getOrderTime() {
    $.ajax({
        type: "Get",
        url: "/Order/GetOrderTimes?id=" + orderid + "&_t=" + new Date().getTime(),
        success: function (data) {
            if (data.result = "Success") {
                $.each(data.areas, function (i, item) {
                    var area = item.province + ' ' + item.city + ' ' + item.district;
                    var areaCode = item.provinceCode + '_' + item.cityCode + '_' + item.districtCode;
                    var tr = '<span class="seledCityItem" value="' + areaCode + '" name="area">' + area +
                        '<a href="javascript:void(0);" class="seledAreaClose" onclick="seledAreaClose(this)">&nbsp;&nbsp;</a>';
                    tr += '<input hidden="hidden" type="text" class="seledProvince" value="' + item.provinceCode + '">';
                    tr += '<input hidden="hidden" type="text" class="seledCity" value="' + item.cityCode + '">';
                    tr += '<input hidden="hidden" type="text" class="seledDistrict" value="' + item.districtCode + '">';
                    tr += '</span > ';
                    $("#selectedarea").append(tr);
                });

                $('#input_date').val(data.ordertimes[0].startDate.substring(0, 10) + ' - ' + data.ordertimes[0].endDate.substring(0, 10));
                for (i = 0; i < data.ordertimes.length; i++) {
                    var timeranges = data.ordertimes[i].timeRange.split(',');
                    var timerangetype = data.ordertimes[i].timeRangeType;
                    if (timeranges.length > 0) {
                        for (j = 0; j < timeranges.length; j++) {
                            $('.abc-li-timerange').filter(function () { return $(this).text() === timeranges[j]; }).addClass('abc-li-timerange-on');                           
                        }
                        var inputname = "count_" + timerangetype;
                        $("input[Name="+inputname+"]").val(data.ordertimes[i].exposureCount);
                    }                
                }
                loadOrderInfo();
            }
        }
    })
}

//获取订单素材
function getOrderMaterial() {
    $.ajax({
        type: "Get",
        url: "/Order/GetOrderMaterials?id=" + orderid + "&_t=" + new Date().getTime(),
        success: function (data) {
            materialType = getMaterialType();
            if (data.result = "Success") {
                $.each(data.ordermaterials, function (i, item) {
                    addMaterialListItem(materialType, item.id, item.name, item.duration, item.remarks);
                    selectedIds.push(item.id);
                })               
            }
        }
    })
}


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
            getOrder();
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
                getOrderTime();
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
    var districtCode = $("#District").val();
    if (province == '--请选择--') {
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
                tr += '        <h3 class="box-title">请点击选择/取消可播放的' + item.timeRangeType + '时间段(单价为:' + item.price + '元/秒)</h3>';
                tr += '    </div>';
                tr += '    <div class="box-body">';
                tr += '         <div class="row" id= "timerangetype_' + item.id + '">';
                tr += '             <input type="text" hidden="hidden" name="timerangetype_price" value="' + item.price + '">';
                tr += '             <input type="text" hidden="hidden" name="timerangetype_id" value="' + item.id + '">';
                tr += '             <input type="text" hidden="hidden" name="timerangetype_type" value="' + item.timeRangeType + '">';
                for (i = 0; i < timerangeArray.length; i++) {
                    tr += '         <div class="abc-li-timerange" name="timerangetype_timerange">' + timerangeArray[i] + '</div>';
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
                infotr += '<span class="bk-items-item-value" id="info_timerange_' + item.timeRangeType + '">' + "无" + '</span>';
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
                //loadOrderInfo();
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
        "minDate": new Date(now.valueOf() + 24 * 60 * 60 * 1000),
        "startDate": new Date(now.valueOf() + 24 * 60 * 60 * 1000),
        "endDate": new Date(now.valueOf() + 2 * 24 * 60 * 60 * 1000)
    }
    );
}


//***4.选择素材*********************************************************
var selectedIds = [];
//获取素材类型
function getMaterialType() {
    var robj = document.getElementsByName("MateralType");
    for (i = 0; i < robj.length; i++) {
        if (robj[i].checked) {
            var materialType = robj[i].value;
        }
    }
    return materialType;
}

//加载素材库列表数据
function loadTables(startPage, pageSize, materalType) {
    var materialType = getMaterialType();
    $("#tableBody").html("");
    $("#checkAll").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/Material/GetAllPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&materialType=" + materialType + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                addSelectorMaterialListItem(materialType, item.id, item.name, item.duration, item.remarks);
            })
            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadTables(newPage, pageSize);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
            $("table > tbody > tr").click(function () {
                $("table > tbody > tr").removeAttr("style")
                $(this).attr("style", "background-color:#beebff");
            });
        }
    })
}
//增加素材库列表项
function addSelectorMaterialListItem(materialType, id, name, duration, remarks) {
    if ($.inArray(id, selectedIds) == -1) {
        var tr = "<tr id='id_" + id + "'>";
        tr += "<td align='center' hidden='hidden'><input type='checkbox' class='checkboxs' value='" + id + "' hidden/></td>";
        tr += "<td>" + (name == null ? "" : name) + "</td>";
        tr += "<td>" + (duration == null ? "" : duration) + "</td>";
        tr += "<td>" + (remarks == null ? "" : remarks) + "</td>";
        tr += "<td>" +
            "<button class='btn btn-info btn-xs'   href= 'javascript:;' onclick =\"playMaterial('" + id + "','" + materialType + "')\"> <i class='fa fa-edit'></i> 查看 </button >" +
            "<button class='btn btn-primary btn-xs' href='javascript:;' onclick='addMaterialListItem(\"" + materialType + "\",\"" + id + "\", \"" + name + "\",\"" + duration + "\", \"" + remarks + "\")'><i class='fa fa-send'></i> 选择 </button>" +
            "</td > ";
        tr += "</tr>";
        $("#tableBody").append(tr);
    }
}

//新增
function add() {
    $("#Title").text("新增媒体");
    //弹出新增窗体
    $("#uploadModal").modal("show");
};

//播放
function playMaterial(id, materialtype) {
    //弹出新增窗体
    $("#playModal").modal("show");
    if (materialtype = 1) {
        $("#myvideo").attr("poster", "/Material/PlayAsync?id=" + id);
        $("#myvideo").attr("src", "/Material/PlayAsync?id=" + id);
    }
    else {

        $("#myvideo").attr("src", "/Material/PlayAsync?id=" + id);
    }
};

//增加播放素材列表项
function addMaterialListItem(materialType, id, name, duration, remarks) {
    var tr = "<tr id='selectedid_" + id + "'>";
    tr += "<td align='center' hidden='hidden'><input type='text' class='selectTableBodyId' value='" + id + "'/></td>";
    tr += "<td >" + (name == null ? "" : name) + "</td>";
    if (materialType == 1 || materialType=="1") {
        tr += "<td >" + "<input type='text' id='input_seconds_" + id + "' name='input_seconds' value='5' onchange='countMaterialtotaltime()'>" + "</td>";
    }
    else if (materialType == 2 || materialType == "2") {
        tr += "<td >" + "<input type='text' id='input_seconds_" + id + "' name='input_seconds' value='" + duration + "' disabled='disabled'>" + "</td>";
    }
    tr += "<td>" +
        "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'upTR(\"" + id + "\")' > <i class='fa fa-edit'></i> 上调 </button >" +
        "<button class='btn btn-info btn-xs' href= 'javascript:;' onclick= 'downTR(\"" + id + "\")' > <i class='fa fa-edit'></i> 下调</button >" +
        "<button class='btn btn-danger btn-xs' href='javascript:;' onclick='deleteSelectedSingleTR(\"" + materialType + "\",\"" + id + "\", \"" + name + "\",\"" + duration + "\", \"" + remarks + "\")'><i class='fa fa-trash-o'></i> 删除 </button>" +
        "</td > ";
    tr += "</tr>";
    $("#selectTableBody").append(tr);
    if ($('#id_' + id)) {
        $('#id_' + id).remove();
    }
    countMaterialtotaltime();
    selectedIds.push(id);
}


//UpTR
function upTR(id) {
    var table = document.getElementById("selectTableBody");
    var selectedTr = document.getElementById("selectedid_" + id);
    var preTr = selectedTr.previousSibling;
    if (preTr) {
        table.insertBefore(selectedTr, preTr);
        $("#selectedid_" + id).focus();
    }
}

//DownTR
function downTR(id) {
    var table = document.getElementById("selectTableBody");
    var selectedTr = document.getElementById("selectedid_" + id);
    var nextTr = selectedTr.nextSibling;
    if (nextTr) {
        table.insertBefore(nextTr, selectedTr);
        $("#selectedid_" + id).focus();
    }
}

//删除单条本订单素材
function deleteSelectedSingleTR(materialType, id, name, duration, remarks) {
    addSelectorMaterialListItem(materialType, id, name, duration, remarks);
    $("#selectedid_" + id).remove();
    countMaterialtotaltime();
};

//计算素材总时间
function countMaterialtotaltime() {
    var materialtotaltime = 0;
    var input_seconds = $('input[name=input_seconds]');
    $.each(input_seconds, function (i, item) {
        materialtotaltime += parseFloat($(item).val());
    });
    $("#input_materialtotaltime").html(materialtotaltime);
}


//***5.订单详情*********************************************************
//订单详情
function loadOrderInfo() {
    var params = [];
    var TimeRange = $('#TimeRange').children('.timeRange');
    time = parseFloat($("#input_totaltime").val()) ? parseFloat($("#input_totaltime").val()) : 0;
    document.getElementById("info_name").innerText = $("#input_name").val();
    document.getElementById("info_totaltime").innerText = $("#input_totaltime").val() + "秒";
    if (time > 0) {
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
    var totalseconds = $("#input_totaltime").val();
    var startdate = $('#input_date').data('daterangepicker').startDate.format("YYYY-MM-DD");
    var enddate = $('#input_date').data('daterangepicker').endDate.format("YYYY-MM-DD");
    var robj = document.getElementsByName("MateralType");
    for (i = 0; i < robj.length; i++) {
        if (robj[i].checked) {
            materalType = robj[i].value;
        }
    }

    //用户名不能为空
    if (name == null || name == "") {
        $("#input_name").attr("data-original-title", "方案名称不能为空")
        $("#input_name").focus();
        return false;
    }

    //校验广告播放时长
    if (totalseconds == null || totalseconds == "" || parseFloat(totalseconds) <= 0) {
        $("#input_totaltime").attr("data-original-title", "广告时长不可为空，且必须大于0秒");
        $("#input_totaltime").focus();
        return false;
    }
    else {
        if (parseFloat(totalseconds) > 3600) {
            $("#input_totaltime").attr("data-original-title", "广告时长必须小于3600秒");
            $("#input_totaltime").focus();
            return false;
        }
        else if (parseFloat(totalseconds) < parseFloat($("#input_materialtotaltime").text())) {
            $("#input_totaltime").attr("data-original-title", "广告时长不应小于播放素材总时长");
            $("#input_totaltime").focus();
            return false;
        }
    }    

    //行业类型不为空
    if (industry == null || industry == "" || industry == "-请选择-") {
        $("#input_industry").attr("data-original-title", "行业类型不能为空");
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
            $("#input_introduce").attr("data-original-title", "您输入的:" + url + "网址格式有误，请输入正确网址！");
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
        MateralType: materalType,
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
                    content: '请选择' + timerangetype + '时间段播放次数或者取消已选择的' + timerangetype + '时间段',
                    btn: ['确认'],
                    yes: function (index) {
                        layer.close(index);
                        changeCss($("#TimeRange"));
                        $("#TimeRange").focus();
                        $("#TimeRange").attr("data-original-title", "请选择" + timerangetype + "时间段的播放次数或者取消已选择的" + timerangetype + "时间段");
                    }
                })
                result = 1;
            }
            else if (parseFloat(exposurecount) < 0) {
                timerangetype = $($(this).find('[name="timerangetype_type"]')).val();
                layer.open({
                    content: '' + timerangetype + '时间段播放次数必须大于0',
                    btn: ['确认'],
                    yes: function (index) {
                        layer.close(index);
                        changeCss($("#TimeRange"));
                        $("#TimeRange").focus();
                        $("#TimeRange").attr("data-original-title", "" + timerangetype + "时间段播放次数必须大于0");
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

//检查订单素材信息是否正确
function checkOrderMaterials() {
    var orderMaterials = [];
    var orderby = 0;
    $("#selectTableBody .selectTableBodyId").each(function () {
        var id = $(this).val();
        orderby += 1;
        orderMaterials.push({
            MaterialId: id,
            Orderby: orderby,
            Seconds: $("#input_seconds_" + id).val(),
        })
    });

    //播放素材不能为空
    if (orderMaterials.length == 0) {
        layer.open({
            content: '请选择本次推广需要的素材',
            btn: ['确认'],
            yes: function (index) {
                layer.close(index);
                changeCss($("#foucs_material"));
                $("#foucs_material").focus();
            }
        })
        return false;
    }
    return orderMaterials;
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

    var orderMaterials = checkOrderMaterials();
    if (!orderMaterials) {
        return;
    }


    $.ajax({
        type: "POST",
        url: "/Order/SaveOrder",
        data: { order: order, orderMaterials: orderMaterials, orderAreas: orderAreas, orderTimes: orderTimes },
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

    var orderMaterials = checkOrderMaterials();
    if (!orderMaterials) {
        return;
    }


    $.ajax({
        type: "POST",
        url: "/Order/SaveOrder",
        data: { order: order, orderMaterials: orderMaterials, orderAreas: orderAreas, orderTimes: orderTimes },
        success: function (data) {
            if (data.result == "Success") {
                pay(data.orderNo, data.amount, data.name);
            }
            else {
                layer.alert(data.message);
            }
        }
    });
}

function changeCss(selector) {
    selector.focus(function () {
        selector.css("border", "2px solid red");
    });
}

$(function () {
    $("#TimeRange").mouseleave(function () {
        $("#TimeRange").css("border", "0px solid red");
    });
    $("#foucs_material").mouseleave(function () {
        $("#foucs_material").css("border", "0px solid red");
    });
})

//付款
function pay(orderNo, amount, subject) {
    $('#totalAmout').val(amount);
    $('#orderNo').val(orderNo);
    $('#subject').val();
    $("#tradeMethodModal").modal();
};




