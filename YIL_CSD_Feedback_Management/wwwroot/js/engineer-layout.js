document.addEventListener("DOMContentLoaded", function () {

    const menu = document.getElementById("menuBtn");

    const sidebar = document.querySelector(".sidebar");

    menu.addEventListener("click", function () {

        sidebar.classList.toggle("show");

    });

});