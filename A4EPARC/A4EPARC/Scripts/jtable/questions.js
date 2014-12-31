$(document).ready(function() {

    if ($('#questionform').length > 0) {

        $('#questionlist').jtable({
            title: ' ',
            paging: true,
            pageSize: 50,
            sorting: true,
            defaultSorting: 'Id ASC',
            actions: {
                listAction: '/QuestionAdmin/GetRows',
                updateAction: '/QuestionAdmin/EditRow'
            },
            fields: {
                SchemeId: {
                    title: 'Scheme',
                    width: '5%',
                    options: { 1: 'Adult', 2: 'Youth' },
                    edit: false
                },
                Description: {
                    title: 'Description',
                    width: '90%',
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
        $('#questionlist').jtable('load');
    }
    

    if ($('#questionformall').length > 0) {

        $('#questionlistall').jtable({
            title: ' ',
            paging: true,
            pageSize: 50,
            sorting: true,
            defaultSorting: 'Id ASC',
            actions: {
                listAction: '/QuestionAdmin/GetRows',
                updateAction: '/QuestionAdmin/EditRow'
            },
            fields: {
                SchemeId: {
                    title: 'Scheme',
                    width: '5%',
                    options: { 1: 'Adult', 2: 'Youth' },
                    edit: false
                },
                Description: {
                    title: 'Description',
                    width: '90%',
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
        $('#questionlistall').jtable('load');
    }

});