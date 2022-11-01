var select = $('.select2'),
    myDropzone = $('#dpz-single-file');

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
    url: '/FacultyBoard/Timetable/Import',
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
                        didOpen: () => {
                            Swal.showLoading();
                        }
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
    }
});