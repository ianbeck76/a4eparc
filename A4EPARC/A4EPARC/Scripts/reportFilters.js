
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
                '/' + getUrlString($('#caseidinput'));
        }

        $('#print').click(function () {
            $("#filterForm").attr('action', getPrintUrl());
            $("#filterForm").submit();
            return false;
        });

        function getPrintUrl() {
            return '/Results/PrintList/' + getUrlString($('#datefrominput')) +
                '/' + getUrlString($('#datetoinput')) +
                '/' + getUrlString($('#caseidinput'));
        }


        function getUrlString(element) {
            return (element.val().length < 1 ? 'null' : element.val()).replace('/', '-').replace('/', '-');
        }
    }
});