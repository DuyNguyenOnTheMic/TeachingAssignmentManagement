﻿var popup;
var rootUrl = $('#loader').data('request-url'),
    yearData = $('#yearData'),
    startYear = yearData.data('startyear'),
    endYear = yearData.data('endyear');

// Populate feather icon for partial view
feather && feather.replace({ width: 14, height: 14 });

function refreshTable() {
    // Refresh all tables when user updates
    $.get(rootUrl + 'Remuneration/GetPriceCoefficientData?startYear=' + startYear + '&endYear=' + endYear)
        .done(function (r) {
            var newDom = $(r);
            $('#tblStandard').replaceWith($('#tblStandard', newDom));
            $('#tblSpecial').replaceWith($('#tblSpecial', newDom));
            $('#tblForeign').replaceWith($('#tblForeign', newDom));
        });
}

function disableButtons(state) {
    if (state === true) {
        // disable buttons
        $('.editAll').prop('disabled', true);
        $('.editRow').each(function () {
            this.style.pointerEvents = 'none';
        });
    } else {
        // enable buttons
        $('.editAll').prop('disabled', false);
        $('.editRow').each(function () {
            this.style.pointerEvents = 'auto';
        });
    }
}

// Show Create and Edit form
function popupForm(url, titleText) {
    var formDiv = $('<div/>')
    $.get(url)
        .done(function (response) {
            formDiv.html(response);

            popup = formDiv.dialog({
                autoOpen: true,
                resizable: false,
                title: 'Quản lý hệ số cấp bậc ' + titleText,
                width: 550,
                open: function () {
                    // Add close icon class
                    $(this).closest(".ui-dialog")
                        .find(".ui-dialog-titlebar-close")
                        .addClass("btn-close");

                    // Prevent user from add edit delete while dialog is populated
                    disableButtons(true);
                },
                close: function () {
                    popup.dialog('destroy').remove();

                    // Re-enable buttons when user closes the dialog
                    disableButtons(false);
                }
            });
        });
}

function submitForm(form) {
    $.validator.unobtrusive.parse(form);

    if ($(form).valid()) {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: $(form).serialize(),
            success: function (data) {
                if (data.success) {
                    popup.dialog('close');
                    refreshTable();

                    // Show message when create or edit succeeded
                    toastr["success"](data.message);
                }
                else {
                    // Show message when create failed
                    Swal.fire({
                        title: 'Thông báo',
                        text: data.message,
                        icon: 'error',
                        customClass: {
                            confirmButton: 'btn btn-primary'
                        },
                        buttonsStyling: false
                    })
                }
            }
        });
    }
    return false;
}