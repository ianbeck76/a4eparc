$(document).ready(function() {

    if ($('#userListForm').length > 0) {

        $('#userlist').jtable({
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
});