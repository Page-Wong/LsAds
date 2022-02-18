var pageSize = 15;
$(function () {
    loadTables(1, pageSize);
});

//加载列表数据
function loadTables(startPage, pageSize) {
    $("#tableBody").html("");
    $("#checkAll").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/AdminOrder/GetAllPageList?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                var tr = "<tr>";
                tr += "<td align='center'><input type='checkbox' class='checkboxs' value='" + item.id + "'/></td>";
                tr += "<td>" + (item.statusString == null ? "" : item.statusString) + "</td>";
                tr += "<td>" + (item.name == null ? "" : item.name) + "</td>";
                tr += "<td>" + (item.adsTag == null ? "" : item.adsTag) + "</td>";
                tr += "<td>" + (item.amount == null ? "" : item.amount) + "</td>";
                tr += "<td>" 
       
                if (item.status == 2) {
                    tr += "<button class='btn btn-primary btn-xs' href='javascript:;' onclick='audit(\"" + item.id + "\")'><i class='fa fa-legal'></i> 审核 </button>"
                }
                tr += "</td > "
                tr += "</tr>";
                $("#tableBody").append(tr);
            })
            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
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
//全选
function checkAll(obj) {
    $(".checkboxs").each(function () {
        if (obj.checked == true) {
            $(this).prop("checked", true)

        }
        if (obj.checked == false) {
            $(this).prop("checked", false)
        }
    });
};
//审核
function audit(id) {
    layer.confirm('请审核', {
        btn: ['通过', '不通过', '取消审核'] //按钮
    }, function () {
        auditSubmit(id, 1);
    },
    function () {
        auditSubmit(id, 0);
    },
    function () {
        layer.closeAll();
    });
};

function auditSubmit(id, audit) {

    $.ajax({
        type: "POST",
        url: "/AdminOrder/Audit?&_t=" + new Date().getTime(),
        data: { orderId: id, audit: audit},
        success: function (data) {
            if (data.result == 'Success') {
                loadTables(1, pageSize);
                layer.closeAll();
            } else {
                layer.alert(data.message);
            }
        }
    })
}