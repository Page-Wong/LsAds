var selectedRole = 0;
var pageSize = 15;
$(function () {
    $("#btnAudit").click(function () { addAudit(); });
    
    loadTables(1, pageSize);
});

//加载列表数据
function loadTables(startPage, pageSize) {
    $("#tableBody").html("");
    $("#checkAllMaterial").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/ProgramReview/GetPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                var tr = "<tr>";  
                tr += "<td hidden='hidden'><input hidden value='" + item.id + "'/></td>";
                tr += "<td>" + (item.displayName == null ? "" : item.displayName) + "</td>";
                tr += "<td>" + (item.duration == null || item.duration == "0"? "" : item.duration/1000 + "秒") + "</td>";
                tr += "<td>" + (item.remarks == null ? "" : item.remarks) + "</td>"; 
                tr += "<td></td>";
                tr += "<td>" +                                                                                
                    "<button class='btn btn-info btn-xs'   href= 'javascript:;' onclick =\"play('" + item.id + "','" + item.type + "')\"> <i class='fa fa-edit'></i> 查看 </button >" +                     
                    "</td > "
                tr += "</tr>";
                $("#tableBody").append(tr);
            })
            getProgramAuditStatus();
            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadTablesMaterial(newPage, pageSize);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
            $("table > tbody > tr").click(function () {
                $("table > tbody > tr").removeAttr("style")
                $(this).attr("style", "background-color:#beebff");
                //selectedRole = $(this).find("input").val();
                //loadPermissionByRole(selectedRole);
            });
        }
    })
}

//新增
function audit(id) {
    layer.confirm('请选择审核结果', {
        btn: ['通过', '不通过', '取消'] //按钮
    }, function () {
        $.post('/ProgramReview/Audit', { programId: id, result: 0 }, function (data) {
            if (data.result == "Success") {
                layer.msg('审核成功');
                loadTables(1, pageSize);
            }
            else {
                layer.msg('审核失败，' + data.message);
            }
        })
        }, function () {
            $.post('/ProgramReview/Audit', { programId: id, result: 1 }, function (data) {
                if (data.result == "Success") {
                    layer.msg('审核成功');
                    loadTables(1, pageSize);
                }
                else {
                    layer.msg('审核失败，' + data.message);

                }
            })
    });
};

//播放
function play(id, type) {
    //弹出新增窗体
    $("#playModal").modal("show");
    if (type == '1') {
        $("#player").html("");
        var video = $('<video width="600" height="480" controls autoplay/>')
        $("#player").append(video);
        video.attr("src", "/ProgramReview/PlayAsync?programId=" + id);  
    }
    else {
        $("#player").html("");
        $.get('/Program/get?id=' + id, function (data) {
            var div = $('<div  width="600" height="480"/>');
            div.html(data.content);
            $("#player").append(div);
        })
    }
};

function getProgramAuditStatus() {
    $("#tableBody tr").each(function (i, item) {
        var programId = $(item).find("input").first().val();
        $.get('/ProgramReview/GetProgramAuditStatus?programId=' + programId, function (data) {
            if (data.result == 'Success') {
                $(item).find("td").last().append(" <button class='btn btn-primary btn-xs' onclick= 'audit(\"" + programId + "\")' > <i class='fa fa-edit'></i> 审核 </button >")

            }
            $(item).find("td").last().prev().html(data.message)
        });
    })
}