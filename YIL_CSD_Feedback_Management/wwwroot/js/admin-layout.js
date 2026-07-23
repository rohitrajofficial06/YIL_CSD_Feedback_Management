document.addEventListener("DOMContentLoaded", function () {

    console.log("JavaScript Loaded");

    const menu = document.getElementById("menuBtn");
    const sidebar = document.getElementById("sidebar");
    const main = document.getElementById("mainContent");

    console.log(menu);
    console.log(sidebar);
    console.log(main);

    if (menu) {
        menu.addEventListener("click", function () {

            console.log("Button Clicked");

            sidebar.classList.toggle("collapsed");
            main.classList.toggle("expanded");

        });
    }

});