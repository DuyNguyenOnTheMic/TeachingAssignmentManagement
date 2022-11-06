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
        var myDropzone = this;

        this.on('maxfilesexceeded', function (file) {
            // Remove file and add again if user input more than 1
            myDropzone.removeAllFiles();
            myDropzone.addFile(file);
        });

        $("#submit-all").click(function (e) {
            // Prevent dropzone from auto submit file
            e.preventDefault();
            e.stopPropagation();

            var count = myDropzone.getQueuedFiles().length;

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
                myDropzone.processQueue();

                myDropzone.on("success", function (response, data) {
                    console.log(data);
                    if (data.length) {
                        // Show lecturers which hasn't been imported into the system
                        Swal.fire("Thông báo!", "Đã import dữ liệu! \nCó một số giảng viên chưa có trong hệ thống, vui lòng xem chi tiết ở cuối trang.", "error");

                        populateDatatable(data);

                    } else {
                        Swal.fire({
                            title: 'Good job!',
                            text: 'You clicked the button!',
                            icon: 'success',
                            customClass: {
                                confirmButton: 'btn btn-primary'
                            },
                            buttonsStyling: false
                        });
                    }
                    window.onbeforeunload = null;
                });
            }
        });

        this.on('sending', function (data, xhr, formData) {
            $('.form-data').each(function () {
                // Send form data along with submit
                formData.append($(this).attr('name'), $(this).val());
            });

            Swal.fire({
                title: 'Vui lòng đợi...',
                allowEscapeKey: false,
                allowOutsideClick: false,
                html: '<div class="progress mb-2 mt-4" style="height: 30px"><div id="myprogress" class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100"></div></div>',
                didOpen: () => {
                    Swal.showLoading();
                }
            })

            startProgressBar();

            // Show confirmation message when user closes tab
            window.onbeforeunload = function () {
                return "Bạn có chắc muốn thoát, tiến trình import có thể sẽ không được lưu?";
            };
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

                            // Update timetable
                            var message = "Cập nhật thời khoá biểu thành công!"
                            importAgain(myDropzone, message, true);

                        } else if (result.isDenied) {
                            // Show waiting message while delete
                            Swal.fire({
                                title: 'Đang xoá dữ liệu...',
                                allowEscapeKey: false,
                                allowOutsideClick: false,
                                didOpen: () => {
                                    Swal.showLoading();
                                }
                            })

                            var term = $('#term').val();
                            var major = $('#major').val();

                            $.ajax({
                                type: 'POST',
                                url: rootUrl + 'FacultyBoard/Timetable/DeleteAll',
                                data: { term, major },
                                success: function (data) {
                                    if (data.success) {
                                        Swal.close();

                                        // Import timetable again
                                        var message = "Thay thế thời khoá biểu thành công!"
                                        importAgain(myDropzone, message, false);
                                    }
                                }
                            });
                        }
                    })
                }
            }
        });
    }
});

function startProgressBar() {
    // Reference the auto-generated proxy for the hub.
    var progress = $.connection.progressHub;

    // Create a function that the hub can call back to display messages.
    progress.client.AddProgress = function (message, percentage) {
        if (percentage >= 100) {
            return;
        }
        else {
            $('#myprogress')
                .attr({ 'aria-valuenow': percentage })
                .text(message + ' ' + percentage + ' %')
                .width(percentage);
        }
    };

    $.connection.hub.start();
}

function importAgain(myDropzone, message, state) {

    // Get state of isUpdate
    var isUpdate = $('#isUpdate');

    $.each(myDropzone.files, function (i, file) {
        // Add file to Dropzone again
        file.status = Dropzone.QUEUED
        file.previewElement.classList.remove("dz-error");
        return file.previewElement.classList.add("dz-success");
    });
    // Set state for isUpdate
    isUpdate.val(state);
    // Process import
    myDropzone.processQueue();

    myDropzone.on("success", function (file) {
        isUpdate.val(false);
        Swal.fire("Thông báo", message, "success");
    });
}

function populateDatatable(data) {
    $('#errorLecturers-section').show();
    var dataTable;

    if (!$.fn.DataTable.isDataTable('#tblErrorLecturers')) {
        // Populate Error lecturers datatable
        dataTable = $('#tblErrorLecturers').DataTable(
            {
                deferRender: true,
                columns: [
                    { 'data': '', defaultContent: '' },
                    { 'data': 'Item1' },
                    { 'data': 'Item2' }
                ],

                columnDefs: [
                    {
                        searchable: false,
                        orderable: false,
                        className: 'text-center',
                        width: '5%',
                        targets: 0
                    }
                ],
                order: [[1, 'asc']],
                dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 ps-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
                displayLength: 7,
                lengthMenu: [7, 10, 25, 50, 75, 100],
                buttons: [
                    {
                        extend: 'collection',
                        className: 'btn btn-outline-secondary dropdown-toggle me-2',
                        text: feather.icons['share'].toSvg({ class: 'font-small-4 me-50' }) + 'Export',
                        buttons: [
                            {
                                extend: 'print',
                                className: 'dropdown-item'
                            },
                            {
                                extend: 'csv',
                                className: 'dropdown-item'
                            },
                            {
                                extend: 'excel',
                                className: 'dropdown-item',
                                title: '',
                                customize: function (xlsx) {
                                    var sheet = xlsx.xl.worksheets['sheet1.xml'];
                                    $('row:first c', sheet).attr('s', '42');
                                }
                            },
                            {
                                extend: 'pdf',
                                className: 'dropdown-item'
                            },
                            {
                                extend: 'copy',
                                className: 'dropdown-item'
                            }
                        ],
                        init: function (api, node, config) {
                            $(node).removeClass('btn-secondary');
                            $(node).parent().removeClass('btn-group');
                            setTimeout(function () {
                                $(node).closest('.dt-buttons').removeClass('btn-group').addClass('d-inline-flex mt-50');
                            }, 50);
                        }
                    }
                ],

                language: {
                    'url': rootUrl + 'app-assets/language/datatables/vi.json'
                }
            });
    }

    // Update current datatable
    dataTable = $('#tblErrorLecturers').DataTable();
    dataTable.clear().rows.add(data).draw();

    // Create Index column datatable
    dataTable.on('draw.dt', function () {
        dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
            dataTable.cell(cell).invalidate('dom');
        });
    });
}