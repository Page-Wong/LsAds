$(function () {
    $("#Province").on('change', function (e) {
        loadCity($(this).val());
    });
    $("#City").on('change', function (e) {
        loadDistrict($(this).val());
    });
    loadProvince();
});


function loadProvince() {
    $.get(
        '/AdministrativeArea/GetProvinces',
        function (data) {
            if (data.result == 'Success') {
                $.each(data.data, function (i, item) {
                    $('#Province').append('<option value="' + item.code + '">' + item.name + '</option>');
                });
            }
        }
    );
}

function loadCity(code, selected, callback) {
    if (code == null || code == '') {
        return false;
    }
    $("#District").val('');
    $('#District').empty();
    $("#City").val('');
    $('#City').empty();
    $.get(
        '/AdministrativeArea/GetCitys?Code=' + code,
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

function loadDistrict(code, selected, callback) {
    if (code == null || code == '') {
        return false;
    }
    $("#District").val('');
    $('#District').empty();
    $.get(
        '/AdministrativeArea/GetDistricts?Code=' + code,
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