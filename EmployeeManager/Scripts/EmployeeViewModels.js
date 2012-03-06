/// <reference path="jquery-1.5.1-vsdoc.js" />
/// <reference path="knockout-1.2.1.js" />
/// <reference path="knockout.mapping-latest.debug.js" />

var employeeViewModel = function () {
    var model = {
        'Name': ko.observable(''),
        'Address': ko.observable(''),
        'NotificationMessage': ko.observable(''),
        'EditNotificationMessage': ko.observable(''),
        'ShowList': ko.observable(true),
        'Items': ko.observableArray([]),
        'Selected': ko.observable({ EmployeeId: ko.observable(''), Name: ko.observable(''), Address: ko.observable('') }),
        'FetchEmployees': function () {
            $.get('/Home/GetAll', function (result) {
                var i;
                model.Items([]);
                for (i in result) {
                    model.Items.push(result[i]);
                }
                model.ShowList(true);
            });
        },
        'AddEmployee': function () {
            var data = { 'Name': this.Name(), 'Address': this.Address() };

            var that = this;
            $.post('/Home/AddEmployee', data, function (response) {
                if (!response.Succeeded) {
                    that.NotificationMessage(response.ErrorMessage);
                } else {
                    that.NotificationMessage('Your changes have been accepted');
                    that.Name('');
                    that.Address('');

                    setTimeout(function () {
                        that.NotificationMessage('');
                        that.FetchEmployees();
                    }, 2000);
                }

            }, 'json');
        },
        'ChangeEmployeeName': function () {
            var data, that;
            that = model;
            data = { EmployeeId: model.Selected().EmployeeId(), Name: model.Selected().Name() };
            $.post('/Home/ChangeEmployeeName', data, function (response) {
                if (!response.Succeeded) {
                    that.EditNotificationMessage(response.ErrorMessage);
                } else {
                    that.EditNotificationMessage('Your changes have been accepted');
                    that.Selected().Name('');
                    that.Selected().Address('');

                    setTimeout(function () {
                        that.EditNotificationMessage('');
                        that.FetchEmployees();
                    }, 2000);
                }
            }, 'json');
        },
        'SelectEmployee': function (id) {

            $.get('/Home/GetEmployee', { 'id': id }, function (result) {
                model.Selected(ko.mapping.fromJS(result));
            });
        }
    };

    return model;
};