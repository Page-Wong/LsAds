$(function () {
    $('#tradeMethodModal').on('show.bs.modal', function () {
        $.ajax({
            type: "GET",
            url: "/SettlementCenter/GetPointPayBalanceByAmout?amout=" + $('#totalAmout').val() ,
            success: function (data) {
                if (data.result == "Success") {
                    $('#pointPayExpense').html(data.data.expense);
                    if (data.data.expense > data.data.balance) {
                        $('#pointPayBalance').addClass('text-danger').html(data.data.balance + '（积分不足）');
                        $('#pointpay').find('.progress-bar').css('width', '100%');

                    }
                    else {
                        $('#pointPayBalance').removeClass('text-danger').html(data.data.balance);

                        $('#pointpay').find('.progress-bar').css('width', (data.data.expense / data.data.balance) * 100 + '%');
                    }
                }
                else {
                    layer.alert(data.message);
                }
            }
        });
    })

});
function checkPay() {
    if (!$('#totalAmout').val() || $('#totalAmout').val() == 0) {

        layer.alert('请输入充值金额', function (index) {
            layer.close(index);
            $('#totalAmout').focus();
        });
        return false;
    }
    if ($('#orderNo').val() == '') {
        layer.alert('订单号为空，不能支付');
        return false;
    }
    return true;
}

function alipay() {
    $('#tradeMethod').val(1);
    if (!checkPay()) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/SettlementCenter/AlipayPayRequest",
        data: {
            totalAmout: $('#totalAmout').val(),
            orderNo: $('#orderNo').val(),
            subject: $('#subject').val(),
            payOrderId: $('#payOrderId').val(),
            tradeMethod: $('#tradeMethod').val(),
        },
        success: function (data) {
            if (data.result == "Success") {
                $('#tradeMethodModal').modal('hide');
                $('#awaitingPaymentModal').modal();
                openwin(data.url);
            }
            else {
                layer.alert(data.message);
            }
        }
    });
}

function pointpay() {
    $('#tradeMethod').val(4);
    if (!checkPay()) {
        return false;
    }

    function pay(paymentPassword) {
        $.ajax({
            type: "POST",
            url: "/SettlementCenter/PointPay",
            data: {
                totalAmout: $('#totalAmout').val(),
                orderNo: $('#orderNo').val(),
                subject: $('#subject').val(),
                payOrderId: $('#payOrderId').val(),
                tradeMethod: $('#tradeMethod').val(),
                paymentPassword: paymentPassword
            },
            success: function (data) {
                if (data.result == "Success") {
                    $('#tradeMethodModal').modal('hide');
                    layer.msg('支付成功',
                        {
                        icon: 6,
                        time: 1000,
                        shade: 0.3
                        },
                        function () {
                            location.href = "/Order/List";
                        }
                    );
                }
                else {
                    layer.alert(data.message, function () {
                        inputPassword();
                    });
                }
            }
        });
    }

    function inputPassword() {
        layer.prompt({
            formType: 1,
            title: '请输入支付密码',
        }, function (value, index, elem) {
            pay(value);
            layer.close(index);
            });
    }

    $.ajax({
        type: "POST",
        url: "/SettlementCenter/PointPayRequest",
        data: {
            totalAmout: $('#totalAmout').val(),
            orderNo: $('#orderNo').val(),
            subject: $('#subject').val(),
            payOrderId: $('#payOrderId').val()
        },
        success: function (data) {
            if (data.result == "Success") {
                inputPassword();
            }
            else {
                layer.alert(data.message);
            }
        }
    });
}
 
function unionpay() {
    layer.alert('暂不支持银联付款，请使用支付宝！');
    return false;
}