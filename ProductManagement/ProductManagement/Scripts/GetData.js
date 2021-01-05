$(document).ready(function () {
    var apiBaseUrl = "https://localhost:44316/";
    $('#btnGetData').click(function () {
        $.ajax({
            url: apiBaseUrl + 'api/Products',
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var $table = $('<table/>').addClass('dataTable table table-bordered table-striped');
                var $header = $('<thead/>').html('<tr><th>Id</th><th>Name</th><th>Category</th><th>Price</th><th>Quantity</th></tr>');
                $table.append($header);
                $.each(data, function (i, val) {
                    var $row = $('<tr/>');
                    $row.append($('<td/>').html(val.Id));
                    $row.append($('<td/>').html(val.Name));
                    $row.append($('<td/>').html(val.Category));
                    $row.append($('<td/>').html(val.Price));
                    $row.append($('<td/>').html(val.Quantity));
                    $table.append($row);
                });
                $('#updatePanel').html($table);
            },
            error: function () {
                alert('Error!');
            }
        });
    });
}); 