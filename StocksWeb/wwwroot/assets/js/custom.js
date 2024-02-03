// collapse menu
$('.icon-toggle-side-menu').on('click', function () {
  // $(".sidebar").toggleClass("fliph");
  $('#sidebar').toggleClass('collapse');
  $('.content').toggleClass('expand');
  $('.body-overlay').toggle(500);
});

$('.body-overlay').on('click', function () {
  $('#sidebar').toggleClass('collapse');
  $('.content').toggleClass('expand');
  $('.body-overlay').hide();
});

$('.menu-icon').on('mouseleave , mouseenter', function () {
  $('.content').removeClass('expand');
  $('#sidebar').removeClass('collapse');
});

// Active Link
$('.menu-item').on('click', function () {
  $('.menu-item').removeClass('active');
  $(this).addClass('active');
});

//Dispaly none for menu on routing
$('.menu-link').on('click', function () {
  $('#sidebar').removeClass('collapse');
  $('.content').removeClass('expand');
  $('.body-overlay').hide();
});

$('.day-btn').on('click', function () {
  $(this).toggleClass('get-btn');
  $(this).parent().find('.fa-check')
    .toggle();
});

// $('#datetimepicker4').datetimepicker({
//   format: 'LT'
// });

// $('.datepicker').datepicker();