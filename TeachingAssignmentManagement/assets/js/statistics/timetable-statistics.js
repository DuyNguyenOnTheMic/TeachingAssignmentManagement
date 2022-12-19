var termId = $('#term').val(),
    rootUrl = $('#loader').data('request-url'),
    lecturerFilter = $('#lecturerFilter');

// Display message when table have no data
var classCount = $('#tblStatistics .class-card').length;
if (classCount == 0) {
    $('#timetableStatisticsDiv').empty().append('<h4 class="text-center mt-2">Học kỳ này chưa có dữ liệu <i class="feather feather-help-circle"></i></h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="Welcome" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
} else {
    // Initialize Tooltip
    $('#tblStatistics [data-bs-toggle="tooltip"]').tooltip({
        trigger: 'hover'
    });

    // Initialize Popover
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl, {
            html: true,
            sanitize: false
        });
    });
}

$.fn.select2.amd.define('select2/selectAllAdapter', [
    'select2/utils',
    'select2/dropdown',
    'select2/dropdown/attachBody'
], function (Utils, Dropdown, AttachBody) {

    function SelectAll() { }
    SelectAll.prototype.render = function (decorated) {
        var self = this,
            $rendered = decorated.call(this),
            $selectAll = $(
                '<button class="btn btn-sm text-start filter-button" type="button"><i class="feather feather-check-square"></i> Chọn tất cả</button>'
            ),
            $unselectAll = $(
                '<button class="btn btn-sm text-start filter-button" type="button"><i class="feather feather-square"></i> Bỏ chọn tất cả</button>'
            ),
            $btnContainer = $('<div class="d-grid"></div>').append($selectAll).append($unselectAll);
        if (!this.$element.prop("multiple")) {
            // this isn't a multi-select -> don't add the buttons!
            return $rendered;
        }
        $rendered.find('.select2-dropdown').prepend($btnContainer);
        $selectAll.on('click', function () {
            hidePopover();
            lecturerFilter.find('option').prop('selected', 'selected').trigger('change'); // Select All Options
            filterCount(lecturerFilter);
            self.trigger('close');
            $('#tblStatistics').find('tbody tr').show();
        });
        $unselectAll.on('click', function () {
            hidePopover();
            lecturerFilter.val('-1').trigger('change'); // Unselect All Options
            lecturerFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc giảng viên');
            self.trigger('close');
            $('#tblStatistics').find('tbody tr').hide();
        });
        return $rendered;
    };

    return Utils.Decorate(
        Utils.Decorate(
            Dropdown,
            AttachBody
        ),
        SelectAll
    );

});

// Populate select2 for lecturer filter
lecturerFilter.wrap('<div class="position-relative my-50"></div>');
lecturerFilter.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: lecturerFilter.parent(),
    dropdownAdapter: $.fn.select2.amd.require('select2/selectAllAdapter')
})
lecturerFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc giảng viên');
lecturerFilter.on('select2:select', function (e) {
    var tableRow = $('#tblStatistics tbody tr'),
        lecturerId = e.params.data.id,
        lecturerClass = tableRow.find('[data-lecturerid="' + lecturerId + '"]');
    hidePopover();
    lecturerClass.show();
    filterCount(lecturerFilter);
    if ($('#tblStatistics tbody tr:visible').length > 0) {
        // Filter for curriculum classes which has lecturer
        tableRow.show();
        updateRow(tableRow);
    } else {
        // Filter from beginning when user deselects all options
        tableRow.show();
        $('#tblStatistics .assign-card').not(lecturerClass).hide();
        tableRow.not(lecturerClass.closest('tr')).hide();
        updateRow(tableRow);
    }
}).on('select2:unselect', function (e) {
    var tableRow = $('#tblStatistics tbody tr'),
        lecturerId = e.params.data.id,
        lecturerClass = tableRow.find('[data-lecturerid="' + lecturerId + '"]');
    hidePopover();
    lecturerClass.hide();
    filterCount(lecturerFilter);
    updateRow(tableRow);
});