var pageSize = 10;

$(function () {

    //$('#ChoosePlayersList').val($("#selectedPlayerids").val().split(',')).trigger('change');
    loadChoosePlayersList();
    datePickerStyle();
    countDay();


    //方案选择仓库初始化
    var programTableConfig = {
        tableBodyId: "selectBody",
        selectCallback: function (id, displayName, duration, width, height) {
            $("#select_" + id).click();
        }
    };
    ProgramSelectModal.init(programTableConfig);

   /***选择方案*********************************************************/
   //弹出选择方案窗体
    $("#choose_area").click(function () { $("#programSelectModal").modal("show"); });
    $("#ChoosePlayersList").select2();
    //提示框初始化
    initTooltip();
})

//提示框初始化
function initTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
    $("#input_name").attr("data-original-title", "字数不能超过50个字且不能为空");
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
        "minDate": new Date(now.valueOf() + 1 * 24 * 60 * 60 * 1000),
        "startDate": new Date(now.valueOf() + 1 * 24 * 60 * 60 * 1000),
        "endDate": new Date(now.valueOf() + 3 * 24 * 60 * 60 * 1000)
    });

    //时间控件动作
    $(document).on('change', '#input_date', function () {
        countDay();
    });
}


function datePickerStyle2(startDate, endDate) {
    var nowTemp = new Date();
    var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);
    var s = new Date(startDate);
    if (s.getTime() < nowTemp.getTime()) {
        $('#input_date').daterangepicker({
            locale: {
                format: 'YYYY-MM-DD'
            },
            "autoApply": true,
            "minDate": new Date(now.valueOf() + 1 * 24 * 60 * 60 * 1000),
            "startDate": new Date(now.valueOf() + 1 * 24 * 60 * 60 * 1000),
            "endDate": new Date(now.valueOf() + 3 * 24 * 60 * 60 * 1000)
        });
    }
    else {
        $('#input_date').daterangepicker({
            locale: {
                format: 'YYYY-MM-DD'
            },
            "autoApply": true,
            "minDate": new Date(now.valueOf() + 1 * 24 * 60 * 60 * 1000),
            "startDate": startDate,
            "endDate": endDate,
        });
    }
    //时间控件动作
    $(document).on('change', '#input_date', function () {
        countDay();
    });

}


function countDay() {
    var startdate = $('#input_date').data('daterangepicker').startDate;
    var enddate = $('#input_date').data('daterangepicker').endDate;
    var days = Math.round((enddate - startdate) / 1000 / 60 / 60 / 24);
    $("#dayLabel").text("共  " + days + "  天");
  //  $("#amount").text((parseFloat(price) * days).toFixed(2));
}



//确定方案
function chose_program() {
    var radio = document.getElementsByName("post_program");
    for (var i = 0; i < radio.length; i++) {
        if (radio[i].checked == true) {
            programId = radio[i].value;
            display_program_doc(programId);
            return;
        }
    }
    layer.alert('请选择方案', { icon: 2 });
}


//初始化播放器列表
function loadChoosePlayersList() {

    var option = "";
    $.ajax({
        type: "Get",
        url: "/MyPromotion/GetMyPlayers?_t=" + new Date().getTime(),
        success: function (data) {
            var option = "";
            $.each(data.players, function (i, player) {
                option += "<option value='"+ player.id + "'>设备：";
                if (player.equipment != null && player.equipment.placeDto != null && player.equipment.placeDto.name != null) {
                    option += player.equipment.placeDto.name + "--";
                }
                if (player.equipment != null && player.equipment.name != null ) {
                    option += player.equipment.name + "--";
                }
                option += "播放器：长 * 宽 " + (player.width == null ? "" : player.width) + "* " + (player.height == null ? "" : player.height);

                option += "</option>";
            })
            $("#ChoosePlayersList").html(option);
            //设置默认选项
            $('#ChoosePlayersList').val($("#selectedPlayerids").val().split(',')).trigger('change.select2');
        } 
    })  
}

//取消保存方案
function CancelMyProgram() {
    layer.confirm('是否确认取消保存？', {
        btn: ['是', '否'] //按钮
    }, function () {
        window.location.href = '/MyPromotion';
    }
    );
}

//保存方案
function SaveMyProgram() {
    var name = $("#input_name").val();
    var selectedPlayerids = $('#ChoosePlayersList').select2("val");
    var startdate = $('#input_date').data('daterangepicker').startDate.format("YYYY-MM-DD");
    var enddate = $('#input_date').data('daterangepicker').endDate.format("YYYY-MM-DD");
    var remarks = $("#input_remarks").val();
    //var programId = "1b58b16c-d9fd-4209-a826-f67656358aa1";
    var programId = $("#programId").val();
 

    //方案名称不能为空
    if (name == null || name == "") {
        $("#input_name").attr("data-original-title", "方案名称不能为空！")
        $("#input_name").focus();
        return;
    }

    //播放器不能为空
    if (selectedPlayerids == null || selectedPlayerids == "") {
        $("#ChoosePlayersList").attr("data-original-title", "请选择播放器！")
        $("#ChoosePlayersList").focus();
        return;
    }

    if (startdate == null || startdate < Date.now()) {
        $("#input_date").attr("data-original-title", "播放开始日期信息有误！")
        $("#input_date").focus();
        return;
    }

    if (enddate == null || enddate < Date.now()) {
        $("#input_date").attr("data-original-title", "播放结束日期信息有误！")
        $("#input_date").focus();
        return;
    }

    if (enddate < startdate) {
        $("#input_date").attr("data-original-title", "播放结束日期必须大于播放开始日期！")
        $("#input_date").focus();
        return;
    }

    if (programId == null) {
        layer.alert("请选择节目！");
        return;
    }


    $.ajax({
        type: "POST",
        url: "/MyPromotion/SaveMyPromotion",
        data: { programId: programId, orderName: name, startDate: startdate, endDate: enddate, playerids: selectedPlayerids, orderRemarks:remarks },
        success: function (data) {
            if (data.result == "Success") {
                layer.alert("数据保存成功！");
                window.location.href = '/MyPromotion';
            }
            else {
                layer.alert(data.message);
            }
        }
    });
    


}