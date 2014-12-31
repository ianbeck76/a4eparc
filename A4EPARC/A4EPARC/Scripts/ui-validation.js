$(document).ready(function() {

   // if ($('#pagetwoform').length > 0) {

        //$("input[type=checkbox]").change(function() {

        //    if ($(this).select().is(':checked')) {

        //        var id = ($(this).attr('id'));
        //        var rowId = id.substring(1, 3);

        //        $("input[type=checkbox]").each(function() {

        //            var thisId = $(this).attr('id');
        //            var thisRowId = thisId.substring(1, 3);
        //            if (thisRowId == rowId) {
        //                if (thisId != id) {
        //                    $(this).removeAttr("checked");

        //                }
        //            }

        //        })
        //    }

        //});
    //}

    // drop shadow when scrolling down the page
    var navigationbar = $('#navigation-bar');
    if (navigationbar.length) {
        var navbar = $('#navigation-bar');
    }
    function toggleShadow() {
        if (parseInt(navbar.offset().top) !== 0) {
                navbar.addClass('shadow');
        } else {
            navbar.removeClass('shadow');
        }
    }
    toggleShadow();
    $(window).scroll(function (e) {
        toggleShadow();
    });

    $(".datepicker").each(function () {
        $(this).datepicker();
    });
    
    $('.radiolist.btn[data-radio-name]').click(function () {
        $('.btn[data-radio-name="' + $(this).data('radioName') + '"]').removeClass('active');
        $('input[name="' + $(this).data('radioName') + '"]').val(
			$(this).html()
		).trigger('change');
    });


    if ($('#surveyPage').length > 0) {
        var validator = $.data($('.surveyPage')[0], 'validator');

        //$('#journeyForm').validate();
        var validatorSettings = validator.settings;
        //validatorSettings.ignore = '';
        validatorSettings.ignore = ".ignore";
    }
    
});