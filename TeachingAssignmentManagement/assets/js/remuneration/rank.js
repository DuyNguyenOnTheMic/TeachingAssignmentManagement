var popup;
var rootUrl = $('#loader').data('request-url');

function disableButtons(state) {
    if (state === true) {
        // disable buttons
        $('.createNew').prop('disabled', true);
        $('.editRow').each(function () {
            this.style.pointerEvents = 'none';
        });
        $('.deleteRow').each(function () {
            this.style.pointerEvents = 'none';
        });
    } else {
        // enable buttons
        $('.createNew').prop('disabled', false);
        $('.editRow').each(function () {
            this.style.pointerEvents = 'auto';
        });
        $('.deleteRow').each(function () {
            this.style.pointerEvents = 'auto';
        });
    }
}

// Show Create and Edit form
function popupForm(url) {
    var formDiv = $('<div/>')
    $.get(url)
        .done(function (response) {
            formDiv.html(response);

            popup = formDiv.dialog({
                autoOpen: true,
                resizable: false,
                title: 'Quản lý hệ số cấp bậc',
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

function deleteAcademicDegree(id) {
    Swal.fire({
        title: 'Thông báo',
        text: 'Bạn có chắc muốn xoá học hàm, học vị này?',
        icon: 'warning',
        showCancelButton: true,
        cancelButtonText: 'Huỷ',
        confirmButtonText: 'Xoá',
        customClass: {
            confirmButton: 'btn btn-primary',
            cancelButton: 'btn btn-outline-danger ms-1'
        },
        buttonsStyling: false
    }).then((result) => {
        if (result.isConfirmed) {
            // Delete item
            $.ajax({
                type: 'POST',
                url: rootUrl + 'Remuneration/DeleteAcademicDegree/' + id,
                success: function (data) {
                    if (data.success) {
                        refreshTable();

                        // Show message when delete succeeded
                        toastr["success"](data.message);
                    }
                    else {
                        // Show message when delete failed
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
    })
}