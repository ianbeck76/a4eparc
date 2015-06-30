
$(document).ready(function() {

    if ($('#results').length > 0) {

        $('#filterLink').click(function () {
            postresults();
        });

        $('#resrefresh').click(function () {
            refresh();
        });
        
        function postresults() {
            $("#filterForm").attr('action', getUrl());
            $("#filterForm").submit();
            return false;
        }

        function refresh() {
            $("#filterForm").attr('action', '/Results/null/null/null/null/null/null');
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
            postws();
        });

        function postws() {
            $("#wsresults").attr('action', getwsUrl());
            $("#wsresults").submit();
            return false;
        }

        function refreshws() {
            $("#wsresults").attr('action', '/WebServiceResults/Index/null/null/null/null/null');
            $("#wsresults").submit();
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
            $("#wsresults").attr('action', getwsExportUrl());
            $("#wsresults").submit();
            return false;
        });

        $('#wsrefresh').click(function () {
            refreshws();
        });

        function getwsExportUrl() {
            return '/WebServiceResults/ExportList/' + getUrlString($('#datefrominput')) +
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