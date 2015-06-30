$(document).ready(function() {

    if ($('#userListForm').length > 0) {

        $('#userlist').jtable({
            title: ' ',
            paging: true,
            pageSize: 100,
            sorting: true,
            defaultSorting: 'Email ASC',
            actions: {
                listAction: '/User/GetUsers',
                createAction: '/User/AddUser',
                updateAction: '/User/EditUser'
            },
            fields: {
                Email: {
                    title: 'Email',
                    width: '25%',
                    sorting: true,
                    edit: false
                },
                IsAdmin: {
                    title: 'Admin',
                    width: '15%',
                    type: 'checkbox',
                    values: { 'false': '', 'true': '' },
                    sorting: true,
                    display: function(data) {
                        if (data.record.IsAdmin == true) {
                            return '<i aria-hidden="true" class="icon icon-tick"></i>';
                        } else {
                            return '<i aria-hidden="true" class="icon icon-cross"></i>';
                        }
                    },
                    edit: function(data) {
                        if (data.record.IsAdmin == true) {
                            return '<i aria-hidden="true" class="icon icon-tick"></i>';
                        } else {
                            return '<i aria-hidden="true" class="icon icon-cross"></i>';
                        }
                    }
                },
                IsViewer: {
                    title: 'Viewer',
                    width: '15%',
                    type: 'checkbox',
                    values: { 'false': '', 'true': '' },
                    sorting: true,
                    display: function(data) {
                        if (data.record.IsViewer == true) {
                            return '<i aria-hidden="true" class="icon icon-tick"></i>';
                        } else {
                            return '<i aria-hidden="true" class="icon icon-cross"></i>';
                        }
                    },
                    edit: function(data) {
                        if (data.record.IsViewer == true) {
                            return '<i aria-hidden="true" class="icon icon-tick"></i>';
                        } else {
                            return '<i aria-hidden="true" class="icon icon-cross"></i>';
                        }
                    }
                },
                Id: {
                    key: true,
                    list: false
                },
                ResetPassword: {
                    title: 'Password',
                    edit: false,
                    create: true,
                    display: function (data) {
                        return resetPassword(data, "/User/ResetPassword")
                    }
                },
                IsActive: {
                    title: 'Active',
                    edit: false,
                    create: false,
                    display: function (data) {
                        if (data.record.IsActive == true) {
                            return deactivateUser(data, "/User/Deactivate")
                        }
                        else {
                            return reactivateUser(data, "/User/Reactivate")
                        }
                    }
                }
            }
        });
        
        function loadTable() {
            $('#userlist').jtable('load',
            {
                emailaddress: $("#emailaddress").val()
            });
        }

        $('#searchbutton').click(function (e) {
            e.preventDefault();
            loadTable();
        });
        
        loadTable();

    }

    if ($('#superAdminUserListForm').length > 0) {
        $('#superadminuserlist').jtable({
            title: ' ',
            paging: true,
            pageSize: 20,
            sorting: true,
            defaultSorting: 'Email ASC',
            actions: {
                listAction: '/User/GetUsers',
                createAction: '/User/AddUser',
                updateAction: '/User/EditUser'
            },
            fields: {
                Email: {
                    title: 'Email',
                    width: '25%',
                    sorting: false,
                    edit: false
                },
                IsSuperAdmin: {
                    title: 'Super Admin',
                    width: '15%',
                    type: 'checkbox',
                    values: { 'false': '', 'true': '' },
                    sorting: true,
                    display: function (data) {
                        if (data.record.IsSuperAdmin == true) {
                            return '<i aria-hidden="true" class="icon icon-tick"></i>';
                        } else {
                            return '<i aria-hidden="true" class="icon icon-cross"></i>';
                        }
                    },
                    edit: function (data) {
                        if (data.record.IsSuperAdmin == true) {
                            return '<i aria-hidden="true" class="icon icon-tick"></i>';
                        } else {
                            return '<i aria-hidden="true" class="icon icon-cross"></i>';
                        }
                    }
                },
                IsAdmin: {
                    title: 'Admin',
                    width: '15%',
                    type: 'checkbox',
                    values: { 'false': '', 'true': '' },
                    sorting: true,
                    display: function (data) {
                        if (data.record.IsAdmin == true) {
                            return '<i aria-hidden="true" class="icon icon-tick"></i>';
                        } else {
                            return '<i aria-hidden="true" class="icon icon-cross"></i>';
                        }
                    },
                    edit: function (data) {
                        if (data.record.IsAdmin == true) {
                            return '<i aria-hidden="true" class="icon icon-tick"></i>';
                        } else {
                            return '<i aria-hidden="true" class="icon icon-cross"></i>';
                        }
                    }
                },
                IsViewer: {
                    title: 'Viewer',
                    width: '15%',
                    type: 'checkbox',
                    values: { 'false': '', 'true': '' },
                    sorting: true,
                    display: function (data) {
                        if (data.record.IsViewer == true) {
                            return '<i aria-hidden="true" class="icon icon-tick"></i>';
                        } else {
                            return '<i aria-hidden="true" class="icon icon-cross"></i>';
                        }
                    },
                    edit: function (data) {
                        if (data.record.IsViewer == true) {
                            return '<i aria-hidden="true" class="icon icon-tick"></i>';
                        } else {
                            return '<i aria-hidden="true" class="icon icon-cross"></i>';
                        }
                    }
                },
                CompanyName: {
                    title: 'Company',
                    width: '20%',
                    sorting: true,
                    edit: true,
                    options: '/User/GetCompanies'
                },
                Id: {
                    key: true,
                    list: false
                },
                ResetPassword: {
                    title: 'Password',
                    edit: false,
                    create: true,
                    display: function (data) {
                        return resetPassword(data, "/User/AdminResetPassword")
                    }
                },
                IsActive: {
                    title: 'Active',
                    edit: false,
                    create: false,
                    display: function (data) {
                        if (data.record.IsActive == true) {
                            return deactivateUser(data, "/User/Deactivate")
                        }
                        else {
                            return reactivateUser(data, "/User/Reactivate")
                        }
                    }
                }
            }
        });

        
        function loadSuperAdminTable() {
            $('#superadminuserlist').jtable('load',
            {
                emailaddress: $("#superadminemailaddress").val()
            });
        }
        
        $('#superadminsearchbutton').click(function (e) {
            e.preventDefault();
            loadSuperAdminTable();
        });

        loadSuperAdminTable();

    }

    function resetPassword(data, url) {

        var $link = $('<a href="#" id="ResetLink_' + data.record.Id + '">Reset</a>');

        var password = '';

        $link.click(function () {

            $.ajax({
                url: url,
                dataType: 'json',
                async: false,
                type: 'POST',
                data: { id: data.record.Id },
                success: function (result) {
                    if (result.IsValid == true) {
                        alert('Password reset and email sent!')
                    }
                    else {
                        alert('Password reset but email failed to send!')
                    }
                    $('#ResetLink_' + data.record.Id).html(result.Password);
                },
                error: function () {
                    alert('Password reset but email failed to send!')
                }
            });
        });
        return $link;
    }

    function deactivateUser(data, url) {

        var $link = $('<a href="#" id="Activate_' + data.record.Id + '">Deactivate</a>');

        $link.click(function () {

            $.ajax({
                url: url,
                dataType: 'json',
                async: false,
                type: 'POST',
                data: { id: data.record.Id },
                success: function (result) {
                    if (result.IsValid == true) {
                        alert('Account deactivated!')
                    }
                    $('#Activate_' + data.record.Id).html("Deactivated!");
                },
                error: function () {
                }
            });
        });
        return $link;
    }

    function reactivateUser(data, url) {

        var $link = $('<a href="#" id="Activate_' + data.record.Id + '">Reactivate</a>');

        $link.click(function () {

            $.ajax({
                url: url,
                dataType: 'json',
                async: false,
                type: 'POST',
                data: { id: data.record.Id },
                success: function (result) {
                    if (result.IsValid == true) {
                        alert('Account reactivated!')
                    }
                    $('#Activate_' + data.record.Id).html("Reactivated!");
                },
                error: function () {
                }
            });
        });
        return $link;
    }

});