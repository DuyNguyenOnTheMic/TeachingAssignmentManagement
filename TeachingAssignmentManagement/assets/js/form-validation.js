$(function () {
    // Create custom regex check
    var regexp = "^[A-Za-z0-9_@@./#&+-]*$";
    jQuery.validator.addMethod("idCheck", function (value, element) {
        var re = new RegExp(regexp);
        return this.optional(element) || re.test(value);
    });

    // Form validation for major
    var majorForm = $('#major-form');
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
                    idCheck: "Chỉ được nhập chữ cái không dấu và không có khoảng trắng!",
                    maxlength: "Tối đa 50 kí tự được cho phép"
                },
                name: {
                    required: "Bạn chưa nhập tên ngành",
                    maxlength: "Tối đa 255 kí tự được cho phép"
                }
            }
        });
    }

    // Form validation for user
    var userForm = $('#user-form');
    if (userForm.length) {
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
                    email: true
                }
            },
            messages: {
                staff_id: {
                    idCheck: "Chỉ được nhập chữ cái không dấu và không có khoảng trắng!",
                    maxlength: "Tối đa 50 kí tự được cho phép!"
                },
                full_name: {
                    maxlength: "Tối đa 255 kí tự được cho phép!"
                },
                email: {
                    required: "Vui lòng điền email!",
                    email: "Vui lòng nhập email hợp lệ!"
                }
            }
        });
    }
});
