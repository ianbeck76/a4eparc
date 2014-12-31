$(document).ready(function() {

    if ($('#sitetextform').length > 0) {

        $('#sitetextdata').jtable({
            title: ' ',
            paging: true,
            pageSize: 50,
            sorting: true,
            defaultSorting: 'Code ASC',
            actions: {
                listAction: '/SiteText/GetRows',
                updateAction: '/SiteText/EditRow'
            },
            fields: {
                OrderNumber: {
                    title: '#',
                    width: '2%',
                    type: 'number'
                },
                Code: {
                    title: 'Code',
                    width: '10%',
                    options: { 1: 'PreContemplation', 2: 'Unauthentic Action', 3: 'Contemplation', 4: 'Preparation', 5: 'Action', 6: 'Aim Statement' },
                    edit: false
                },
                SchemeId: {
                    title: 'Scheme',
                    width: '5%',
                    options: { 1: 'Adult', 2: 'Youth' },
                    edit: false
                },
                Name: {
                    title: 'Name',
                    width: '10%'
                },
                Description: {
                    title: 'Description',
                    width: '48%',
                    type: 'textarea'
                },
                Summary: {
                    title: 'Summary',
                    width: '25%',
                    type: 'textarea'
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
            $('#sitetextdata').jtable('load',
            {
                code: $("#code").val()
            });
        }

        $('#searchbutton').click(function (e) {
            e.preventDefault();
            loadTable();
        });
        
        loadTable();

    }

});