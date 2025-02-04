// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.querySelectorAll('.form-check-input').forEach(function (checkbox) {
    checkbox.addEventListener('change', function () {
        filterTable();
    });
});

function filterTable() {
    var selectedBranchIds = Array.from(document.querySelectorAll('.form-check-input:checked'))
        .map(function (checkbox) {
            return checkbox.value;
        });

    var rows = document.querySelectorAll('#forecastTable tbody tr');

    rows.forEach(function (row) {
        var branchId = row.cells[0].innerText;
        if (selectedBranchIds.length === 0 || selectedBranchIds.includes(branchId)) {
            row.style.display = ''; // Показываем строки, соответствующие выбранным филиалам
        } else {
            row.style.display = 'none'; // Скрываем строки, не соответствующие выбранным филиалам
        }
    });
}

