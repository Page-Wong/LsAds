var lsads = lsads || {};
var ProgramSelectModal = lsads.ProgramSelectModal || {};


ProgramSelectModal.init = function (config) {
    $.getScript("/Views/Program/_SelectProgramTable.js", function () {
        var programTableConfig = {
            tableBodyId: "selectBody"
        };
        if (config != null) {
            programTableConfig.selectCallback = config.selectCallback
        }
        ProgramTable.init(programTableConfig);
    }) 
}

ProgramSelectModal.show = function (isShow) {
    if (isShow) {
        $('#programSelectModal').modal("show");
    }
    else {
        $('#programSelectModal').modal('hide');
    }
}

/***选择方案*********************************************************/
//弹出选择方案窗体
function selectModal() {
    $("#programSelectModal").modal("show");
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

function display_program_doc(programId) {
    $.ajax({
        type: "POST",
        url: "/Program/Get",
        data: { id: programId },
        success: function (data) {
            if (data.type == 0) {
                var inHTML = ''
                    + '<input hidden="hidden" id="programId" value=' + data.id + '>'
                    + '<input hidden="hidden" id="program_duration" value=' + data.duration + '>'
                    + '<div class="public_doc_module" doc_id=' + data.id + '>'
                    + '<h3>' + data.displayName + '</h3>'
                    + '<div class="box_h mainbox">'
                    + data.content
                    + '<div class="ctrol_btns">'
                    + '<span onclick="open_choose_doc_popup()">重选</span>'
                    + '<span onclick="cancel_program_doc()">取消</span>'
                    + '</div>'
                    + '</div>'
                    + '</div>';
            }
            else {
                var inHTML = ''
                    + '<input hidden="hidden" id="programId" value=' + data.id + '>'
                    + '<input hidden="hidden" id="program_duration" value=' + data.duration + '>'
                    + '<div class="public_doc_module" doc_id =' + data.id + '>'
                    + '<h3>' + data.displayName + '</h3>'
                    + '<div class="box_h mainbox">'
                    + '<div align="center" style="background:#000; color:#FFF;width:100%;height:100%;">'
                    + '<video id= "myvideo" width= "100%" height= "100%" src="/Material/PlayAsync?id=' + data.programMaterials[0].materialId + '" poster="/Material/PlayAsync?id=' + data.programMaterials[0].materialId + '" controls autoplay/> '
                    + '</div >'
                    + '<div class="ctrol_btns">'
                    + '<span onclick="open_choose_doc_popup()">重选</span>'
                    + '<span onclick="cancel_program_doc()">取消</span>'
                    + '</div>'
                    + '</div>'
                    + '</div>';
            }
            $("#program_duration").val(data.duration);
            $('#choose_area').hide();
            document.getElementById('choose_result_box').innerHTM = '';
            document.getElementById('choose_result_box').innerHTML = inHTML;
            $('#choose_result_box').show();
            close_choose_doc_popup();
            //loadOrderInfo();
        }
    })
}

function close_choose_doc_popup() {
    $("#programSelectModal").find(".close").click();
}

function open_choose_doc_popup() {
    $("#programSelectModal").modal("show");
}

function cancel_program_doc() {
    $('#choose_result_box').hide().text('');
    $('#program_doc_result').val('');
    $('#choose_area').show();
    $("#programSelectModal").modal("show");
}
