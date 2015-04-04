$(document).ready(function() {

    if ($('#sitelabelsform').length > 0) {

        $('#sitelabelsdata').jtable({
            title: ' ',
            paging: true,
            pageSize: 1000,
            sorting: true,
            defaultSorting: 'Name ASC',
            actions: {
                listAction: '/SiteLabels/GetRows',
                updateAction: '/SiteLabels/EditRow',
                createAction: '/SiteLabels/AddRow'
            },
            fields: {
                Name: {
                    title: 'Name',
                    edit: false,
                    width: '20%'
                },
                Description: {
                    title: 'Description',
                    width: '80%',
                    type: 'textarea'
                },
                Id: {
                    key: true,
                    list: false
                }
            }
        });
        
        function loadTable() {
            $('#sitelabelsdata').jtable('load',
            {
                name: '',
                scheme: $("#AdminSchemeSelect").val(),
                language: $("#AdminLanguageSelect").val()
            });
        }

        $('#searchlabelsbutton').click(function (e) {
            e.preventDefault();
            loadTable();
        });

        loadTable();

        $("select[id='AdminLanguageSelect']").on('change', function () {
            loadTable();
        });

        $("select[id='AdminSchemeSelect']").on('change', function () {
            loadTable();
        });

    }

});