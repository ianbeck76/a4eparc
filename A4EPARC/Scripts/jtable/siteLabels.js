$(document).ready(function() {

    if ($('#sitelabelsform').length > 0) {

        $('#sitelabelsdata').jtable({
            title: ' ',
            paging: true,
            pageSize: 50,
            sorting: true,
            defaultSorting: 'Id ASC',
            actions: {
                listAction: '/SiteLabels/GetRows',
                updateAction: '/SiteLabels/EditRow'
            },
            fields: {
                Name: {
                    title: 'Name',
                    width: '10%'
                },
                Description: {
                    title: 'Description',
                    width: '80%',
                    type: 'textarea'
                },
                SchemeId: {
                    title: 'Scheme',
                    width: '5%',
                    options: { 1: 'Adult', 2: 'Youth', 3: 'Exemplar', 4: 'GSM' },
                    edit: false
                },
                LanguageCode: {
                    title: 'Language',
                    width: '5%',
                    options: ['en-GB'],
                    edit: false
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
                name: $("#name").val()
            });
        }

        $('#searchbutton').click(function (e) {
            e.preventDefault();
            loadTable();
        });
        
        loadTable();

    }

});