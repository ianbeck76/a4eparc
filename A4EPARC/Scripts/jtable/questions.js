$(document).ready(function() {

    if ($('#questionform').length > 0) {

        $('#questionlist').jtable({
            title: ' ',
            paging: true,
            pageSize: 1000,
            sorting: true,
            defaultSorting: 'Id ASC',
            actions: {
                listAction: '/QuestionAdmin/GetRows',
                updateAction: '/QuestionAdmin/EditRow'
            },
            fields: {
                Description: {
                    title: 'Description',
                    width: '90%',
                    type: 'textarea'
                },
                Id: {
                    key: true,
                    list: false
                }
            }
        });

        function loadQuestionTable() {
            $('#questionlist').jtable('load',
            {
                scheme: $("#AdminSchemeSelect").val(),
                language: $("#AdminLanguageSelect").val()
            });
        }

        $('#searchquestionsbutton').click(function (e) {
            e.preventDefault();
            loadQuestionTable();
        });

        loadQuestionTable();

        $("select[id='AdminLanguageSelect']").on('change', function () {
            loadQuestionTable();
        });

        $("select[id='AdminSchemeSelect']").on('change', function () {
            loadQuestionTable();
        });
    }

});