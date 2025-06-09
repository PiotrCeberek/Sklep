// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.querySelector('form').addEventListener('submit', function () {
    let priceInput = document.querySelector('input[name="Price"]');
    priceInput.value = priceInput.value.replace(',', '.');
});