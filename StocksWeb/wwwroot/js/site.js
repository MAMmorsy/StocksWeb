$(function () {
    var PlaceHolderElement = $('#PlaceHolder');

    //$('span[data-bs-toggle="modal"]').click(function (event) {
    //    debugger;
    //    PlaceHolderElement.empty();
    //    var url = $(this).data('url');
        
    //    var decodedUrl = decodeURIComponent(url);
    //    $.get(decodedUrl).done(function (data) {
    //        PlaceHolderElement.html(data);
    //        PlaceHolderElement.find('.modal').modal('show');
    //    })
    //})
    $('span[data-toggle="modal"]').click(function (event) {
        debugger;
        PlaceHolderElement.empty();
        var url = $(this).data('url');
        
        var decodedUrl = decodeURIComponent(url);
        $.get(decodedUrl).done(function (data) {
            PlaceHolderElement.html(data);
            PlaceHolderElement.find('.modal').modal('show');
        })
    })

    PlaceHolderElement.on('click', '[data-save="modal"]', function (event) {
        debugger;
       PlaceHolderElement.find('.modal').modal('hide');
        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        
        var url = actionUrl;
        var sendData = form.serialize();
        /*var sendData = form.serializeArray();*/
        var formData = new FormData(form[0]);
        console.log(formData);

        //$.post(url, sendData).done(function (data) {
        //    console.log(data);
        //    if (data.indexOf("<!DOCTYPE") === -1) {

        //        /*PlaceHolderElement.empty();*/
        //        $('#PlaceHolder').html(data);
        //        /*$('#profile-modal').modal('show');*/
        //        PlaceHolderElement.find('.modal').modal('show');
        //    }
        //    else {
        //        PlaceHolderElement.empty();
        //        document.location.reload();
        //    }
            
        //})

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            async: false,
            cache: false,
            contentType: false,
            processData: false,
            success: function (data) {
                console.log(data);
                if (data.indexOf("<!DOCTYPE") === -1) {

                    /*PlaceHolderElement.empty();*/
                    $('#PlaceHolder').html(data);
                    /*$('#profile-modal').modal('show');*/
                    PlaceHolderElement.find('.modal').modal('show');
                }
                else {
                    PlaceHolderElement.empty();
                    document.location.reload();
                }
            },
            error: function (data) {
            }
        });
        
    })

    PlaceHolderElement.on('click', '[data-bs-dismiss="modal"]', function (event) {
        PlaceHolderElement.find('.modal').modal('hide');
            /*PlaceHolderElement.empty();*/
        })
});

//const tableSearch = $("#myTable").DataTable({
//  // "scrollY": "500px",
//  "scrollCollapse": true,
//  "paging": true,
//  'pageLength': 10,
//});


$(document).ready(function () {
    $('#myTable').DataTable({
        "scrollY": "450px",
        "scrollCollapse": true,
        "paging": true,
        'pageLength': 10,
        
        "sorting": true, "order": [],
        "language": {
            "emptyTable": "No data available"
        },
        //"columnDefs": [
        //    { "targets": [1], "searchable": false }]
    });
});

$('input[data-filter="search"]').on( 'keyup', function () {
  tableSearch.search( this.value ).draw();
} );

// $(document).ready(function () {
//    var nameElem = document.getElementById("myTable_paginate")
//    nameElem.classList.add("page-pagination d-flex justify-content-end")
//    document.getElementById("myTable_paginate").className += " page-pagination d-flex justify-content-end";
//    $('#myTable').DataTable({
//        "scrollY": "450px",
//        "scrollCollapse": true,
//        "paging": true,
//        "sorting": false,
//        "language": {
//            "emptyTable": "No data available",
//            paginate: {
//                previous: "<<",
//                next: ">>"
//            }
//        },
//        "columnDefs": [
//            { "targets": [1], "searchable": false }]
//    });

//    $('#myTable_paginate').addClass('page-pagination d-flex justify-content-end');
// });

// $(document).ready(function () {
//    var table = $('#myTable_New').DataTable({
//        ordering: true,
//        bLengthChange: false,
//        iDisplayLength: 10,
//        bFilter: false,
//        pagingType: "full_numbers",
//        bInfo: false,
//        dom: "Bfrtip",
//        buttons: [{
//            extend: 'pdf',
//            text: 'Exportar PDF',
//            title: 'Nuse'
//        }, {
//            extend: 'excel',
//            text: 'Exportar Excel',
//            title: 'Nuse'
//        }],
//        language: {
//            emptyTable: "No data available",
//            paginate: {
//                previous: "<<",
//                next: ">>"
//            }
//        }
//    });
// })

$('.QuestionSets input[type="checkbox"]').on('change', function() {
  $('.QuestionSets input[type="checkbox"]').not(this).prop('checked', false);
});