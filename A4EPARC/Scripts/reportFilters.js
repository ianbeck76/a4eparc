
$(document).ready(function() {

    if ($('#results').length > 0) {

        $('#filterLink').click(function () {
            refresh();
        });
        
        function refresh() {
            $("#filterForm").attr('action', getUrl());
            $("#filterForm").submit();
            return false;
        }

        function getUrl() {
            return '/results/' + getUrlString($('#datefrominput')) +
                '/' + getUrlString($('#datetoinput')) +
                '/' + getUrlString($('#jobseekeridinput')) +
                '/' + getUrlString($('#surnameinput')) +
                 '/' + getUrlString($('#usernameinput')) +
            '/' + getUrlString($('#companyinput'));
        }

        $('#export').click(function () {
            $("#filterForm").attr('action', getExportUrl());
            $("#filterForm").submit();
            return false;
        });

        function getExportUrl() {
            return '/Results/ExportList/' + getUrlString($('#datefrominput')) +
                '/' + getUrlString($('#datetoinput')) +
                '/' + getUrlString($('#jobseekeridinput')) +
                '/' + getUrlString($('#surnameinput')) +
                 '/' + getUrlString($('#usernameinput')) +
                '/' + getUrlString($('#companyinput'));
        }
    }

    if ($('#wsresults').length > 0) {

        $('#wssearch').click(function () {
            refreshws();
        });

        function refreshws() {
            $("#wsfilterForm").attr('action', getwsUrl());
            $("#wsfilterForm").submit();
            return false;
        }

        function getwsUrl() {
            return '/WebServiceResults/Index/' + getUrlString($('#datefrominput')) +
                '/' + getUrlString($('#datetoinput')) +
                '/' + getUrlString($('#jobseekeridinput')) +
                '/' + getUrlString($('#environmentinput')) +
                '/' + getUrlString($('#companyinput'));
        }

        $('#wsexport').click(function () {
            $("#wsfilterForm").attr('action', getwsExportUrl());
            $("#wsfilterForm").submit();
            return false;
        });

        function getwsExportUrl() {
            return '/webserviceresults/ExportList/' + getUrlString($('#datefrominput')) +
                '/' + getUrlString($('#datetoinput')) +
                '/' + getUrlString($('#jobseekeridinput')) +
                '/' + getUrlString($('#environmentinput')) +
                '/' + getUrlString($('#companyinput'));
        }

    }

    function getUrlString(element) {
        return (element.val().length < 1 ? 'null' : element.val()).replace('/', '-').replace('/', '-');
    }

});