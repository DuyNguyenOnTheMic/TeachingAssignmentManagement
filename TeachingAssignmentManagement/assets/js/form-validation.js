var majorForm = $('#major-form'),
    termForm = $('#term-form'),
    userForm = $('#user-form');

// Close dialog on button click
$('#btnClose').click(function () {
    $('.ui-dialog-titlebar-close').click();
});

// Create custom id check
var regexpId = "^[a-zA-Z0-9_-]*$";
jQuery.validator.addMethod("idCheck", function (value, element) {
    var re = new RegExp(regexpId);
    return this.optional(element) || re.test(value);
});

// Create custom email check for VLU
const email_domain = ["vlu.edu", "vanlanguni"]
var regexpEmail = "^[A-Za-z0-9._%+-]+@(" + email_domain[0] + "|" + email_domain[1] + ").vn$";
jQuery.validator.addMethod("emailCheck", function (value, element) {
    var re = new RegExp(regexpEmail);
    return this.optional(element) || re.test(value);
});

// Form validation for major
if (majorForm.length) {
    majorForm.validate({
        rules: {
            id: {
                required: true,
                idCheck: true,
                maxlength: 50
            },
            name: {
                required: true,
                maxlength: 255
            }
        },
        messages: {
            id: {
                required: "Bạn chưa nhập mã ngành",
                idCheck: "Chỉ được nhập số-chữ không dấu và không có khoảng trắng!",
                maxlength: "Tối đa 50 kí tự được cho phép"
            },
            name: {
                required: "Bạn chưa nhập tên ngành",
                maxlength: "Tối đa 255 kí tự được cho phép"
            }
        }
    });
}

if (termForm.length) {
    var yearSelect = $('.year-select'),
        touchspin = $('.touchspin'),
        picker = $('.picker');

    yearSelect.each(function () {
        // Select2 without search bar
        var $this = $(this);
        $this.wrap('<div class="position-relative"></div>');
        $this.select2({
            dropdownAutoWidth: true,
            dropdownParent: $this.parent(),
            minimumResultsForSearch: Infinity
        });
    });

    // Default Spin
    touchspin.TouchSpin({
        buttondown_class: 'btn btn-primary',
        buttonup_class: 'btn btn-primary',
        buttondown_txt: feather.icons['minus'].toSvg(),
        buttonup_txt: feather.icons['plus'].toSvg()
    });

    var touchspinValue = $('.touchspin-min-max'),
        counterMin = 1,
        counterMax = 52;
    if (touchspinValue.length > 0) {
        touchspinValue
            .TouchSpin({
                min: counterMin,
                max: counterMax,
                buttondown_txt: feather.icons['minus'].toSvg(),
                buttonup_txt: feather.icons['plus'].toSvg()
            })
            .on('touchspin.on.startdownspin', function () {
                var $this = $(this);
                $('.bootstrap-touchspin-up').removeClass('disabled-max-min');
                if ($this.val() == counterMin) {
                    $(this).siblings().find('.bootstrap-touchspin-down').addClass('disabled-max-min');
                }
            })
            .on('touchspin.on.startupspin', function () {
                var $this = $(this);
                $('.bootstrap-touchspin-down').removeClass('disabled-max-min');
                if ($this.val() == counterMax) {
                    $(this).siblings().find('.bootstrap-touchspin-up').addClass('disabled-max-min');
                }
            });
    }

    // Custom vietnamese language for flatpickr
    var vn = {
        weekdays: {
            shorthand: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
            longhand: [
                "Chủ nhật",
                "Thứ hai",
                "Thứ ba",
                "Thứ tư",
                "Thứ năm",
                "Thứ sáu",
                "Thứ bảy",
            ],
        },

        months: {
            shorthand: [
                "Th1",
                "Th2",
                "Th3",
                "Th4",
                "Th5",
                "Th6",
                "Th7",
                "Th8",
                "Th9",
                "Th10",
                "Th11",
                "Th12",
            ],
            longhand: [
                "Tháng một",
                "Tháng hai",
                "Tháng ba",
                "Tháng tư",
                "Tháng năm",
                "Tháng sáu",
                "Tháng bảy",
                "Tháng tám",
                "Tháng chín",
                "Tháng mười",
                "Tháng mười một",
                "Tháng mười hai",
            ],
        },
        firstDayOfWeek: 1
    };

    // Picker
    picker.flatpickr({
        locale: vn,
        altInput: true,
        altFormat: "j F, Y",
        defaultDate: toDate('#start_date'),
        parseDate: (datestr, format) => {
            return new Date(datestr.replace(/(\d{2})-(\d{2})-(\d{4})/, "$1/$2/$3"));
        },
        onReady: function (selectedDates, dateStr, instance) {
            if (instance.isMobile) {
                $(instance.mobileInput).attr('step', null);
            }
        },
    });

    // Convert string to date
    function toDate(selector) {
        var from = $(selector).val().split("/");
        if (from.length === 1) {
            return null;
        } else {
            return new Date(from[2], from[1] - 1, from[0]);
        }
    }
}

// Form validation for user
if (userForm.length) {

    var select = $('.select2');
    // select2
    select.each(function () {
        var $this = $(this);
        $this.wrap('<div class="position-relative"></div>');
        $this
            .select2({
                placeholder: "---- Chọn role ----",
                minimumResultsForSearch: Infinity,
                forceabove: true,
                dropdownParent: $this.parent()
            })
            .change(function () {
                $(this).valid();
            });
    });

    userForm.validate({
        rules: {
            staff_id: {
                idCheck: true,
                maxlength: 50
            },
            full_name: {
                maxlength: 255
            },
            email: {
                required: true,
                emailCheck: true
            },
            role_id: {
                required: true
            }
        },
        messages: {
            staff_id: {
                idCheck: "Chỉ được nhập số-chữ không dấu và không có khoảng trắng!",
                maxlength: "Tối đa 50 kí tự được cho phép"
            },
            full_name: {
                maxlength: "Tối đa 255 kí tự được cho phép"
            },
            email: {
                required: "Bạn chưa nhập email",
                emailCheck: "Vui lòng nhập email Văn Lang hợp lệ!"
            },
            role_id: {
                required: "Bạn chưa chọn role"
            }
        },
        errorPlacement: function (error, element) {
            if (element.hasClass("select2")) {
                error.appendTo(element.siblings(".select2"));
            } else {
                error.insertAfter(element);
            }
        }
    });
}