$(document).ready(function() {

    var defaultLanguage = $.cookie('default_language');

    if (defaultLanguage != null) {
        $("#languageselect").val(defaultLanguage);
    }

    if ($('#languageselect').length > 0) {

            var options = getLanuageOptions();
            $('#languageselect').empty();
            for (var i = 0; i < options.length; i++) {
                $('<option' + (options[i].Key == defaultLanguage ? ' selected="selected"' : '') + '>' + options[i].Value + '</option>').val(options[i].Key).appendTo($('#languageselect'));
        }

        $('#languageselect').live('change', function () {

            $.cookie('default_language', $(this).val(), { expires: 999, path: '/' });

            var labels = getSiteLabels($(this).val());

            labels.forEach(function (label) {
                $('#' + label.Key).html(label.Value).val(label.Value)
            })

            if ($('.surveyPageTwo').length > 0) {

                var questions = getQuestions($(this).val());
                var i = 0;
                $('.questionlabels').each(function () {
                    $(this).html(questions[i])
                    i++;
                });
            }
        });
    }

    if ($('#surveyPage').length > 0) {

        var pageitems = getPageItems();
        pageitems.forEach(function (item) {

            if (item.IsDisplay == true) {
                $('#' + item.Name + 'Group').show();
            }
            else {
                $('#' + item.Name + 'Group').hide();
            }
            if (item.IsRequired == true) {
                $('#' + item.Name).attr('required', 'required');
            }
            else {
                $('#' + item.Name).removeAttr('required');
            }

        })

    }

    function getSchemeId() {

        var schemeId = $('#SchemeId').val();

        if (schemeId == 'undefined') {
            schemeId = 1;
        }
        return schemeId;
    }

    function getCompanyId() {

        var companyId = $('#CompanyId').val();
        if (companyId == 'undefined') {
            companyId = 1;
        }
        return companyId;
    }

    function getLanuageOptions() {

        var options = new Array();

        $.ajax({
            url: "/Survey/GetLanguageOptions",
            dataType: 'json',
            async: false,
            type: 'GET',
            success: function (data) {
                options = data.Options;
            }
        });
        return options;
    }

    function getQuestions(languageCode) {

        var labels = new Array();

        $.ajax({
            url: "/Survey/GetQuestionLabels",
            dataType: 'json',
            async: false,
            type: 'GET' ,
            data: { schemeId: getSchemeId(), languageCode: languageCode },
            success: function (data) {
                labels = data.Labels;
            }
        });
        return labels;
    }

    function getPageItems() {

        var items = new Array();

        $.ajax({
            url: "/Survey/GetPageItems",
            dataType: 'json',
            async: false,
            type: 'GET',
            data: { companyId: getCompanyId() },
            success: function (data) {
                items = data.PageItems;
            }
        });
        return items;
    }

    function getSiteLabels(languageCode) {

        var labels = new Array();

        $.ajax({
            url: "/SiteLabels/Get",
            dataType: 'json',
            async: false,
            type: 'GET',
            data: { schemeId: getSchemeId(), languageCode: languageCode },
            success: function (data) {
                labels = data.Labels;
            }
        });
        return labels;
    }

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