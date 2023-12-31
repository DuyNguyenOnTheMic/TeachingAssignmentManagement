﻿var profileForm = $('#profile-form'),
  majorForm = $('#major-form'),
  termForm = $('#term-form'),
  userForm = $('#user-form'),
  academicDegreeForm = $('#academicdegree-form'),
  academicDegreeRankForm = $('#academicdegreerank-form'),
  unitPriceForm = $('#unitprice-form'),
  coefficientForm = $('#coefficient-form'),
  lecturerRankForm = $('#lecturerrank-form')

// Close dialog on button click
$('#btnClose').click(function () {
  $('.ui-dialog-titlebar-close').click()
})

// Create custom id check
var regexpId = '^[a-zA-Z0-9_-]*$'
jQuery.validator.addMethod('idCheck', function (value, element) {
  var re = new RegExp(regexpId)
  return this.optional(element) || re.test(value)
})

if (profileForm.length) {
  profileForm.validate({
    rules: {
      staff_id: {
        idCheck: true,
        maxlength: 50
      },
      full_name: {
        maxlength: 255
      }
    },
    messages: {
      staff_id: {
        idCheck: 'Chỉ được nhập số-chữ không dấu và không có khoảng trắng!',
        maxlength: 'Tối đa 50 kí tự được cho phép'
      },
      full_name: {
        maxlength: 'Tối đa 255 kí tự được cho phép'
      }
    },
    submitHandler: function (form, e) {
      e.preventDefault()
      var post_url = $(form).attr('action')
      var request_method = $(form).attr('method')
      var form_data = $(form).serialize()
      $.ajax({
        url: post_url,
        type: request_method,
        data: form_data,
        success: function (data) {
          $('input').blur()
          if (data.success) {
            // Show message when update succeeded
            Swal.fire({
              title: 'Thông báo',
              text: data.message,
              icon: 'success',
              customClass: {
                confirmButton: 'btn btn-primary'
              },
              buttonsStyling: false
            })
          } else {
            // Show message when update succeeded
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
      })
      return false
    }
  })
}

if (majorForm.length) {
  var select = $('.form-data')
  // select2
  select.each(function () {
    var $this = $(this)
    $this.wrap('<div class="position-relative"></div>')
    $this
      .select2({
        placeholder: $this[0][0].innerHTML,
        minimumResultsForSearch: Infinity,
        forceabove: true,
        dropdownParent: $this.parent()
      })
      .change(function () {
        $(this).valid()
      })
  })

  // Form validation for major
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
      },
      abbreviation: {
        required: true,
        maxlength: 50
      },
      program_type: {
        required: true
      }
    },
    messages: {
      id: {
        required: 'Bạn chưa nhập mã ngành',
        idCheck: 'Chỉ được nhập số-chữ không dấu và không có khoảng trắng!',
        maxlength: 'Tối đa 50 kí tự được cho phép'
      },
      name: {
        required: 'Bạn chưa nhập tên ngành',
        maxlength: 'Tối đa 255 kí tự được cho phép'
      },
      abbreviation: {
        required: 'Bạn chưa nhập tên viết tắt của ngành',
        maxlength: 'Tối đa 50 kí tự được cho phép'
      },
      program_type: {
        required: 'Bạn chưa chọn CTĐT'
      }
    },
    errorPlacement: function (error, element) {
      if (element.hasClass('form-data')) {
        error.insertAfter(element.siblings('.select2'))
      } else {
        error.insertAfter(element)
      }
    }
  })
}

if (termForm.length) {
  // Populate term form
  var yearSelect = $('.select2'),
    picker = $('.picker')

  yearSelect.each(function () {
    // Select2 without search bar
    var $this = $(this)
    $this.wrap('<div class="position-relative"></div>')
    $this
      .select2({
        dropdownAutoWidth: true,
        dropdownParent: $this.parent(),
        minimumResultsForSearch: Infinity
      })
      .change(function () {
        if ($('#start_year').val() <= $('#end_year').val()) {
          $(this).valid()
        }
      })
  })

  // Populate touchspin
  $('.touchspin').each(function () {
    var $this = $(this)
    $this
      .TouchSpin({
        min: getMin($this),
        max: getMax($this),
        buttondown_txt: feather.icons['minus'].toSvg(),
        buttonup_txt: feather.icons['plus'].toSvg()
      })
      .on('touchspin.on.startdownspin', function () {
        $('.bootstrap-touchspin-up').removeClass('disabled-max-min')
        if ($this.val() == getMin($this)) {
          $(this).siblings().find('.bootstrap-touchspin-down').addClass('disabled-max-min')
        }
      })
      .on('touchspin.on.startupspin', function () {
        $('.bootstrap-touchspin-down').removeClass('disabled-max-min')
        if ($this.val() == getMax($this)) {
          $(this).siblings().find('.bootstrap-touchspin-up').addClass('disabled-max-min')
        }
      })
  })

  // Use to get min number of each touchspin
  function getMin(touchspin) {
    if (touchspin.hasClass('touchspin-startweek')) {
      return 1
    } else if (touchspin.hasClass('touchspin-maxlesson')) {
      return 3
    } else if (touchspin.hasClass('touchspin-maxclass')) {
      return 1
    }
  }

  // Use to get max number of each touchspin
  function getMax(touchspin) {
    if (touchspin.hasClass('touchspin-startweek')) {
      return 52
    } else if (touchspin.hasClass('touchspin-maxlesson')) {
      return 15
    } else if (touchspin.hasClass('touchspin-maxclass')) {
      return 30
    }
  }

  // Custom vietnamese language for flatpickr
  var vn = {
    weekdays: {
      shorthand: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
      longhand: ['Chủ nhật', 'Thứ hai', 'Thứ ba', 'Thứ tư', 'Thứ năm', 'Thứ sáu', 'Thứ bảy']
    },

    months: {
      shorthand: ['Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6', 'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12'],
      longhand: [
        'Tháng một',
        'Tháng hai',
        'Tháng ba',
        'Tháng tư',
        'Tháng năm',
        'Tháng sáu',
        'Tháng bảy',
        'Tháng tám',
        'Tháng chín',
        'Tháng mười',
        'Tháng mười một',
        'Tháng mười hai'
      ]
    },
    firstDayOfWeek: 1
  }

  // Picker
  picker.flatpickr({
    locale: vn,
    altInput: true,
    altFormat: 'j F, Y',
    defaultDate: toDate('#start_date'),
    parseDate: (datestr, format) => {
      return new Date(datestr.replace(/(\d{2})-(\d{2})-(\d{4})/, '$1/$2/$3'))
    },
    onReady: function (selectedDates, dateStr, instance) {
      if (instance.isMobile) {
        $(instance.mobileInput).attr('step', null)
      }
    }
  })
  picker.change(function () {
    $(this).valid()
  })

  // Convert string to date
  function toDate(selector) {
    var from = $(selector).val().split('/')
    if (from.length === 1) {
      return null
    } else {
      return new Date(from[2], from[1] - 1, from[0])
    }
  }

  jQuery.validator.addMethod('moreCheck', function (value, element, param) {
    return this.optional(element) || parseInt(value) >= parseInt($(param).val())
  })

  // Form validation for term
  termForm.validate({
    ignore: [],
    rules: {
      id: {
        required: true,
        digits: true,
        number: false,
        minlength: 3,
        maxlength: 3
      },
      start_date: {
        required: true
      },
      end_year: {
        moreCheck: '#start_year'
      },
      start_week: {
        required: true,
        number: false,
        min: 1,
        max: 52
      },
      max_lesson: {
        required: true,
        number: false,
        min: 3,
        max: 15
      },
      max_class: {
        required: true,
        number: false,
        min: 1,
        max: 30
      }
    },
    messages: {
      id: {
        required: 'Bạn chưa nhập học kỳ',
        digits: 'Vui lòng nhập số!',
        minlength: 'Vui lòng nhập đúng 3 kí tự!',
        maxlength: 'Vui lòng nhập đúng 3 kí tự!'
      },
      start_date: {
        required: 'Bạn chưa chọn ngày bắt đầu'
      },
      end_year: {
        moreCheck: 'Năm kết thúc không thể nhỏ hơn năm bắt đầu!'
      },
      start_week: {
        required: 'Bạn chưa nhập tuần bắt đầu',
        min: 'Vui lòng nhập lớn hơn hoặc bằng 1',
        max: 'Vui lòng nhập nhỏ hơn hoặc bằng 52'
      },
      max_lesson: {
        required: 'Bạn chưa nhập số tiết tối đa',
        min: 'Vui lòng nhập lớn hơn hoặc bằng 3',
        max: 'Vui lòng nhập nhỏ hơn hoặc bằng 15'
      },
      max_class: {
        required: 'Bạn chưa nhập số lớp tối đa',
        min: 'Vui lòng nhập lớn hơn hoặc bằng 1',
        max: 'Vui lòng nhập nhỏ hơn hoặc bằng 30'
      }
    },
    errorPlacement: function (error, element) {
      if (element.hasClass('select2')) {
        error.insertAfter(element.siblings('.select2'))
      } else if (element.hasClass('picker')) {
        error.insertAfter(element.siblings('.picker'))
      } else if (element.hasClass('touchspin')) {
        error.insertAfter(element.closest('.bootstrap-touchspin'))
      } else {
        error.insertAfter(element)
      }
    }
  })
}

if (userForm.length) {
  var select = $('.form-data')
  // select2
  select.each(function () {
    var $this = $(this)
    $this.wrap('<div class="position-relative"></div>')
    $this
      .select2({
        placeholder: $this[0][0].innerHTML,
        minimumResultsForSearch: Infinity,
        forceabove: true,
        dropdownParent: $this.parent()
      })
      .change(function () {
        $(this).valid()
      })
  })

  // Create custom email check for VLU
  var email_domain = ['vlu.edu', 'vanlanguni']
  var regexpEmail = '^[A-Za-z0-9._%+-]+@(' + email_domain[0] + '|' + email_domain[1] + ').vn$'
  jQuery.validator.addMethod('emailCheck', function (value, element) {
    var re = new RegExp(regexpEmail)
    return this.optional(element) || re.test(value)
  })

  // Form validation for user
  userForm.validate({
    rules: {
      staff_id: {
        required: true,
        idCheck: true,
        maxlength: 50
      },
      full_name: {
        required: true,
        maxlength: 255
      },
      email: {
        required: function () {
          return $('#type').val() == 'CH' ? true : false
        },
        emailCheck: true
      },
      type: {
        required: true
      },
      role_id: {
        required: true
      }
    },
    messages: {
      staff_id: {
        required: 'Bạn chưa nhập mã giảng viên',
        idCheck: 'Chỉ được nhập số-chữ không dấu và không có khoảng trắng!',
        maxlength: 'Tối đa 50 kí tự được cho phép'
      },
      full_name: {
        required: 'Bạn chưa nhập tên giảng viên',
        maxlength: 'Tối đa 255 kí tự được cho phép'
      },
      email: {
        required: 'Bạn chưa nhập email',
        emailCheck: 'Vui lòng nhập email Văn Lang hợp lệ!'
      },
      type: {
        required: 'Bạn chưa chọn loại giảng viên'
      },
      role_id: {
        required: 'Bạn chưa chọn role'
      }
    },
    errorPlacement: function (error, element) {
      if (element.hasClass('form-data')) {
        error.insertAfter(element.siblings('.select2'))
      } else {
        error.insertAfter(element)
      }
    }
  })
}

if (academicDegreeForm.length) {
  var counterMin = 1,
    counterMax = 100
  // Populate touchspin
  $('.touchspin').each(function () {
    var $this = $(this)
    $this
      .TouchSpin({
        min: counterMin,
        max: counterMax,
        buttondown_txt: feather.icons['minus'].toSvg(),
        buttonup_txt: feather.icons['plus'].toSvg()
      })
      .on('touchspin.on.startdownspin', function () {
        $('.bootstrap-touchspin-up').removeClass('disabled-max-min')
        if ($this.val() == counterMin) {
          $(this).siblings().find('.bootstrap-touchspin-down').addClass('disabled-max-min')
        }
      })
      .on('touchspin.on.startupspin', function () {
        $('.bootstrap-touchspin-down').removeClass('disabled-max-min')
        if ($this.val() == counterMax) {
          $(this).siblings().find('.bootstrap-touchspin-up').addClass('disabled-max-min')
        }
      })
  })

  // Form validation for academic degree form
  academicDegreeForm.validate({
    rules: {
      id: {
        required: true,
        idCheck: true,
        maxlength: 50
      },
      name: {
        required: true,
        maxlength: 100
      },
      level: {
        required: true,
        number: false,
        min: counterMin,
        max: counterMax
      }
    },
    messages: {
      id: {
        required: 'Bạn chưa nhập mã học hàm, học vị',
        idCheck: 'Chỉ được nhập số-chữ không dấu và không có khoảng trắng!',
        maxlength: 'Tối đa 50 kí tự được cho phép'
      },
      name: {
        required: 'Bạn chưa nhập tên học hàm, học vị',
        maxlength: 'Tối đa 100 kí tự được cho phép'
      },
      level: {
        required: 'Bạn chưa nhập thứ tự',
        min: 'Vui lòng nhập lớn hơn hoặc bằng 1',
        max: 'Vui lòng nhập nhỏ hơn hoặc bằng 100'
      }
    },
    errorPlacement: function (error, element) {
      if (element.hasClass('touchspin')) {
        error.insertAfter(element.closest('.bootstrap-touchspin'))
      } else {
        error.insertAfter(element)
      }
    }
  })
}

if (academicDegreeRankForm.length) {
  var select = $('.form-data')
  // select2
  select.each(function () {
    var $this = $(this)
    $this.wrap('<div class="position-relative"></div>')
    $this
      .select2({
        placeholder: $this[0][0].innerHTML,
        minimumResultsForSearch: Infinity,
        dropdownParent: $this.parent()
      })
      .on('select2:open', function () {
        // Set dropdown height for select2
        var dropdown = $this.parent().find('.select2-results__options')
        if (dropdown.css('max-height') != '150px') {
          dropdown.css({ 'max-height': '150px' })
        }
      })
      .change(function () {
        $(this).valid()
      })
  })

  // Form validation for academic degree rank form
  academicDegreeRankForm.validate({
    rules: {
      id: {
        required: true,
        idCheck: true,
        maxlength: 50
      },
      academic_degree_id: {
        required: true
      }
    },
    messages: {
      id: {
        required: 'Bạn chưa nhập mã cấp bậc',
        idCheck: 'Chỉ được nhập số-chữ không dấu và không có khoảng trắng!',
        maxlength: 'Tối đa 50 kí tự được cho phép'
      },
      academic_degree_id: {
        required: 'Bạn chưa chọn học hàm, học vị'
      }
    },
    errorPlacement: function (error, element) {
      if (element.hasClass('form-data')) {
        error.insertAfter(element.siblings('.select2'))
      } else {
        error.insertAfter(element)
      }
    }
  })
}

if (unitPriceForm.length) {
  new Cleave($('#price'), {
    numeral: true,
    numeralThousandsGroupStyle: 'thousand'
  })

  jQuery.validator.addMethod('numberOnly', function (value, element) {
    return this.optional(element) || /^[0-9,.]+$/i.test(value)
  })

  // Form validation for unit price
  unitPriceForm.validate({
    rules: {
      price: {
        required: true,
        maxlength: 18,
        numberOnly: true
      }
    },
    messages: {
      price: {
        required: 'Bạn chưa nhập đơn giá',
        maxlength: 'Tối đa 18 kí tự được cho phép',
        numberOnly: 'Đơn giá phải lớn hơn hoặc bằng 0'
      }
    }
  })
}

if (coefficientForm.length) {
  var counterMin = 1,
    counterMax = 9.99
  // Populate touchspin
  $('.touchspin').each(function () {
    var $this = $(this)
    $this.val() || $this.val(counterMin)
    $this
      .TouchSpin({
        min: counterMin,
        max: counterMax,
        forcestepdivisibility: 'none',
        buttondown_txt: feather.icons['minus'].toSvg(),
        buttonup_txt: feather.icons['plus'].toSvg()
      })
      .on('touchspin.on.startdownspin', function () {
        $('.bootstrap-touchspin-up').removeClass('disabled-max-min')
        if ($this.val() == counterMin) {
          $(this).siblings().find('.bootstrap-touchspin-down').addClass('disabled-max-min')
        }
      })
      .on('touchspin.on.startupspin', function () {
        $('.bootstrap-touchspin-down').removeClass('disabled-max-min')
        if ($this.val() == counterMax) {
          $(this).siblings().find('.bootstrap-touchspin-up').addClass('disabled-max-min')
        }
      })
  })

  // Form validation for coefficient
  coefficientForm.validate({
    rules: {
      vietnamese_coefficient: {
        required: true,
        number: false,
        min: counterMin,
        max: counterMax
      },
      foreign_coefficient: {
        required: true,
        number: false,
        min: counterMin,
        max: counterMax
      },
      theoretical_coefficient: {
        required: true,
        number: false,
        min: counterMin,
        max: counterMax
      },
      practice_coefficient: {
        required: true,
        number: false,
        min: counterMin,
        max: counterMax
      }
    },
    messages: {
      vietnamese_coefficient: {
        required: 'Bạn chưa nhập hệ số tiếng Việt',
        min: 'Vui lòng nhập lớn hơn hoặc bằng 1',
        max: 'Vui lòng nhập nhỏ hơn hoặc bằng 9.99'
      },
      foreign_coefficient: {
        required: 'Bạn chưa nhập hệ số tiếng Anh',
        min: 'Vui lòng nhập lớn hơn hoặc bằng 1',
        max: 'Vui lòng nhập nhỏ hơn hoặc bằng 9.99'
      },
      theoretical_coefficient: {
        required: 'Bạn chưa nhập hệ số lý thuyết',
        min: 'Vui lòng nhập lớn hơn hoặc bằng 1',
        max: 'Vui lòng nhập nhỏ hơn hoặc bằng 9.99'
      },
      practice_coefficient: {
        required: 'Bạn chưa nhập hệ số thực hành',
        min: 'Vui lòng nhập lớn hơn hoặc bằng 1',
        max: 'Vui lòng nhập nhỏ hơn hoặc bằng 9.99'
      }
    },
    errorPlacement: function (error, element) {
      if (element.hasClass('touchspin')) {
        error.insertAfter(element.closest('.bootstrap-touchspin'))
      } else {
        error.insertAfter(element)
      }
    }
  })
}

if (lecturerRankForm.length) {
  var select = $('.form-data')
  // select2
  select.each(function () {
    var $this = $(this)
    $this.wrap('<div class="position-relative"></div>')
    $this
      .select2({
        placeholder: $this[0][0].innerHTML,
        forceabove: true,
        dropdownParent: $this.parent()
      })
      .on('select2:open', function () {
        // Set dropdown height for select2
        var dropdown = $this.parent().find('.select2-results__options')
        if (dropdown.css('max-height') != '115px') {
          dropdown.css({ 'max-height': '115px' })
        }
      })
      .change(function () {
        $(this).valid()
      })
  })

  // Form validation for academic degree rank form
  lecturerRankForm.validate({
    rules: {
      academic_degree_rank_id: {
        required: true
      }
    },
    messages: {
      academic_degree_rank_id: {
        required: 'Bạn chưa chọn cấp bậc cho giảng viên'
      }
    },
    errorPlacement: function (error, element) {
      if (element.hasClass('form-data')) {
        error.insertAfter(element.siblings('.select2'))
      } else {
        error.insertAfter(element)
      }
    }
  })
}
