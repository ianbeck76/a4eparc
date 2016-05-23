$(document).ready(function () {

    var defaultLanguage = $.cookie('default_language');

    if (defaultLanguage != null) {
        $("#languageselect").val(defaultLanguage);
    }
    else {
        defaultLanguage = 'en-GB';
    }

    if ($('#languageselect').length > 0) {

            var options = getLanuageOptions();
            $('#languageselect').empty();
            for (var i = 0; i < options.length; i++) {
                $('<option' + (options[i].Key == defaultLanguage ? ' selected="selected"' : '') + '>' + options[i].Value + '</option>').val(options[i].Key).appendTo($('#languageselect'));
                getTranslations(defaultLanguage);
            }

        $('#languageselect').live('change', function () {

            $.cookie('default_language', $(this).val(), { expires: 999, path: '/' });

            var labels = getSiteLabels($(this).val());

            labels.forEach(function (label) {
                $('#' + label.Name).html(label.Description).val(label.Description);
            })

            getTranslations($(this).val());

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

    function getTranslations(languageCode) {

        var options = getSiteOptions(languageCode);

        var optionName = '';
        var selectedValue = '';

        options.forEach(function (option) {

            if (optionName != option.ParentName) {
                optionName = option.ParentName;
                selectedValue = $('#' + option.ParentName).val();
                $('#' + option.ParentName).empty();
            }
            $('<option' + (option.Key == selectedValue ? ' selected="selected"' : '') + '>' + option.Value + '</option>').val(option.Key).appendTo($('#' + option.ParentName));
        })

        var radios = getSiteRadioOptions(languageCode);

        radios.forEach(function (radio) {
            $("button[data-text='" + radio.Key + "']").text(radio.Value);
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

    function getSiteOptions(languageCode) {

        var options = new Array();

        $.ajax({
            url: "/SiteLabels/GetOptions",
            dataType: 'json',
            async: false,
            type: 'GET',
            data: { languageCode: languageCode },
            success: function (data) {
                options = data.Options;
            }
        });
        return options;
    }

    function getSiteRadioOptions(languageCode) {

        var options = new Array();

        $.ajax({
            url: "/SiteLabels/GetRadioOptions",
            dataType: 'json',
            async: false,
            type: 'GET',
            data: { languageCode: languageCode },
            success: function (data) {
                options = data.Options;
            }
        });
        return options;
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

    $('.yesno.btn[data-radio-name]').click(function () {
        $('.btn[data-radio-name="' + $(this).data('radioName') + '"]').removeClass('active');
        $('input[name="' + $(this).data('radioName') + '"]').val(
			$(this).attr('data-text')
		).trigger('change');
    });

    if ($('#surveyPageOne').length > 0) {

        $('button[data-radio-name="Gender"]').click(function () {
            $('#Gender').data('val', false);
        });

        $('#DateOfBirthDay').change(function () {
            dateChange();
        });

        $('#DateOfBirthMonth').change(function () {
            dateChange();
        });

        $('#DateOfBirthYear').change(function () {
            dateChange();
        });

        function dateChange() {
            if ($('#DateOfBirthDay').val() != ''
                && $('#DateOfBirthMonth').val() != ''
                && $('#DateOfBirthYear').val() != '') {
                $('#DateOfBirth').val('01/01/2000');
                $('#DateOfBirth').data('val', false);
            }
        }


    }

    if ($('#incitePageOne').length > 0) {

        submitInciteForm("incitePageOne");
    }

    if ($('#incitePageTwo').length > 0) {

        submitInciteForm("incitePageTwo");
    }


    function submitInciteForm(formname) {

        $('#' + formname).submit(function (e) {

            $("#" + formname + " input").each(function () {

                if ($(this).valid() == false) {
                    $('#warningmessage').show();
                    e.preventDefault();
                }
            });
        });


    }

    if ($('#resultsgrid').length > 0) {


        $('.active').each(function() {
            $(this).click(function (e) {
                var id = $(this).attr('id');
                e.preventDefault();
                $.ajax({
                    url: "/Results/Delete",
                    dataType: 'json',
                    type: 'POST',
                    data: { id: id },
                    success: function (data) {
                        changeActive(data, id);
                    }
                });
            });
        });

        function changeActive(data, row) {

            if (data == false) {
                $('#' + row).removeClass('active icon icon-tick');
                $('#'+ row).addClass('active icon icon-cross');
                $('#' + row).attr('title', 'Active - click to remove from excel results');
            }
            else {
                $('#' + row).removeClass('active icon icon-cross');
                $('#' + row).addClass('active icon icon-tick');
                $('#' + row).attr('title', 'Inactive - click to include in excel results');
            }
        }
    }


    $.validator.setDefaults({
        ignore: [],
        // any other default options and/or rules
    });

});