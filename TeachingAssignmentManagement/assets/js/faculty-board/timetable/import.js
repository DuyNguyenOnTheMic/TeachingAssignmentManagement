var select = $('.select2'),
    myDropzone = $('#dpz-single-file');
var rootUrl = $('#loader').data('request-url');

// Populate select2 for choosing term and major
select.each(function () {
    var $this = $(this);
    $this.wrap('<div class="position-relative"></div>');
    $this.select2({
        language: 'vi',
        dropdownAutoWidth: true,
        dropdownParent: $this.parent(),
        placeholder: $this[0][0].innerHTML
    });
})

// Populate Dropzone
myDropzone.dropzone({
    url: rootUrl + 'FacultyBoard/Timetable/Import',
    autoProcessQueue: false,
    paramName: 'postedFile',
    timeout: null,
    maxFiles: 1,
    maxFilesize: 50,
    acceptedFiles: '.xlsx, .xls',
    dictFileTooBig: "Tệp quá lớn ({{filesize}}MB). Kích thước tối đa: {{maxFilesize}}MB.",
    dictInvalidFileType: "Tệp tin sai định dạng, chỉ được upload file excel",
    init: function () {

        // Using a closure
        var _this = this;

        this.on('maxfilesexceeded', function (file) {
            // Remove file and add again if user input more than 1
            _this.removeAllFiles();
            _this.addFile(file);
        });

        $("#submit-all").click(function (e) {
            // Prevent dropzone from auto submit file
            e.preventDefault();
            e.stopPropagation();

            var count = _this.getQueuedFiles().length;

            var termId = $("#term").val();
            var majorId = $("#major").val();

            // Validation for form
            if (termId == '') {
                toastr.options.positionClass = 'toast-bottom-right';
                toastr.warning('Bạn chưa chọn ngành và khoá');
            }
            else if (majorId == '') {
                toastr.options.positionClass = 'toast-bottom-right';
                toastr.warning('Bạn chưa chọn khoá');
            }
            else if (count == 0) {
                toastr.options.positionClass = 'toast-bottom-right';
                toastr.warning('File chưa được upload hoặc sai định dạng ');
            }
            else {
                // Begin to import file
                _this.processQueue();

                _this.on("success", function () {
                    Swal.fire({
                        title: 'Good job!',
                        text: 'You clicked the button!',
                        icon: 'success',
                        customClass: {
                            confirmButton: 'btn btn-primary'
                        },
                        buttonsStyling: false
                    });
                    window.onbeforeunload = null;
                });
            }
        });

        this.on('sending', function (data, xhr, formData) {
            $('.form-select').each(function () {
                // Send form data along with submit
                formData.append($(this).attr('name'), $(this).val());

                Swal.fire({
                    title: 'Vui lòng đợi...',
                    allowEscapeKey: false,
                    allowOutsideClick: false,
                    html: '<div class="progress mb-2 mt-4" style="height: 30px"><div id="myprogress" class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100"></div></div>',
                    didOpen: () => {
                        Swal.showLoading();
                    }
                })

                //StartProgressBar();

                // Show confirmation message when user closes tab
                window.onbeforeunload = function () {
                    return "Changes you made may not be saved";
                };
            })
        });

        this.on('error', function (data, errorMessage, xhr) {

            Swal.close();
            window.onbeforeunload = null;

            if (xhr) {
                var errorDisplay = document.querySelectorAll('[data-dz-errormessage]');
                if (xhr.status == 417) {
                    // Show message if the file is not in the right format (like missing columns, etc, ...)
                    errorDisplay[errorDisplay.length - 1].innerHTML = 'Đã xảy ra lỗi, vui lòng kiểm tra lại tệp tin';

                    Swal.fire({
                        title: 'Thông báo',
                        html: errorMessage,
                        icon: 'error',
                        customClass: {
                            confirmButton: 'btn btn-primary'
                        },
                        buttonsStyling: false
                    });
                } else if (xhr.status == 409) {
                    // Handle alternative flows when user chooses term and major which already has data
                    errorDisplay[errorDisplay.length - 1].innerHTML = 'Đã xảy ra lỗi!';

                    Swal.fire({
                        title: 'Thông báo',
                        text: errorMessage,
                        icon: 'question',
                        showDenyButton: true,
                        showCancelButton: true,
                        confirmButtonText: 'Cập nhật',
                        denyButtonText: 'Thay thế',
                        cancelButtonText: 'Huỷ',
                        customClass: {
                            confirmButton: 'btn btn-primary',
                            denyButton: 'btn btn-success ms-1',
                            cancelButton: 'btn btn-outline-danger ms-1'
                        },
                        buttonsStyling: false
                    }).then((result) => {
                        if (result.isConfirmed) {
                            Swal.fire('Saved!', '', 'success')
                        } else if (result.isDenied) {
                            Swal.fire('Changes are not saved', '', 'info')
                        }
                    })
                }
            }
        });
    }
});