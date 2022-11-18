/*var termId = $('#term').val(),
    majorId = $('#major').val();
// function uses for real time update
setInterval(function () {
    $("#tblAssign").load("Timetable/GetData #tblAssign", { termId, majorId });
}, 2000); //refresh every 2 seconds*/
//alert($('#tblAssign tbody tr').length)
$(function () {
    var select = $('.select2');
    // select2
    select.each(function () {
        var $this = $(this);
        $this.wrap('<div class="position-relative"></div>');
        $this
            .select2({
                placeholder: "--- Chưa phân công ---",
                dropdownParent: $this.parent()
            })
    });
});