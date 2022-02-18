var lsads = lsads || {};
var ProgramTable = lsads.ProgramTable || {};

ProgramTable.init = function (config) {
    if (config != null) {
        ProgramTable.tableBodyId = config.tableBodyId;
        ProgramTable.selectCallback = config.selectCallback
        ProgramTable.hideSelectBtn = config.hideSelectBtn        
    }
    ProgramTable.loadTables(1, pageSize);
}


//加载列表数据
ProgramTable.loadTables = function (startPage, pageSize) {
    $("#" + ProgramTable.tableBodyId + " #checkAll").prop("checked", false);
    $("#" + ProgramTable.tableBodyId).load("/Program/PageListPartialView?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(), function (response, status, xhr) {
        if (status == "success") {
            var html = $(response);
            var elment = $("#" + ProgramTable.tableBodyId +" #grid_paging_part"); //分页插件的容器id
            var rowCount = html.find('input[name="rowCount"]').val();
            var pageCount = html.find('input[name="pageCount"]').val();
            if (rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: rowCount, //总数
                    totalPages: pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        ProgramTable.loadTables(newPage, pageSize);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
            

            $("table > tbody > tr").click(function () {
                $("table > tbody > tr").removeAttr("style")
                $(this).attr("style", "background-color:#beebff");

            });

            if (ProgramTable.hideSelectBtn) {
                $("button[btnProgramSelect]").hide()
            }
        }
    })
}

//删除单条数据
ProgramTable.deleteSingle = function(id) {
    layer.confirm("您确认删除选定的记录吗？", {
        btn: ["确定", "取消"]
    }, function () {
        $.ajax({
            type: "POST",
            url: "/Program/Delete",
            data: { "id": id },
            success: function (data) {
                if (data.result == "Success") {
                    ProgramTable.loadTables(1, pageSize)
                    layer.closeAll();
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        })
    });
};

//编辑
ProgramTable.edit = function(id) {
    location.href = "/program/edit/" + id;
};

//选择
ProgramTable.select = function (id, displayName, duration, width, height) {
    ProgramTable.selectCallback(id, displayName, duration, width, height);
};