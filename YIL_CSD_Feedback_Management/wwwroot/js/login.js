function togglePassword() {

    const password = document.getElementById("passwordField");

    const icon = document.getElementById("passwordIcon");

    if (password.type === "password") {

        password.type = "text";

        icon.classList.remove("bi-eye");

        icon.classList.add("bi-eye-slash");

    }
    else {

        password.type = "password";

        icon.classList.remove("bi-eye-slash");

        icon.classList.add("bi-eye");

    }
}

function loginLoading(button) {

    button.disabled = true;

    button.innerHTML =
        '<span class="spinner-border spinner-border-sm me-2"></span>Signing In...';

    return true;
}


function updatePortalUI() {

    const userName = document.getElementById("UserName")
        .value
        .trim()
        .toLowerCase();

    const isAdmin =
        userName === "admin@ss" ||
        userName === "admin@br" ||
        userName === "admin@trg";

    const badge = document.getElementById("adminBadge");
    const title = document.getElementById("loginTitle");
    const description = document.getElementById("loginDescription");

    if (isAdmin) {

        if (badge)
            badge.classList.remove("d-none");

        if (title)
            title.innerText = "Administrator Login";

        if (description)
            description.innerText =
                "Sign in to manage users, reports, departments, feedback data and system configuration.";

    }
    else {

        if (badge)
            badge.classList.add("d-none");

        if (title)
            title.innerText = "Welcome Back!";

        if (description)
            description.innerText =
                "Sign in to continue to the Customer Feedback Management Portal.";

    }
}

document.addEventListener("DOMContentLoaded", function () {

    // Login Form
    const form = document.getElementById("loginForm");

    if (form) {

        form.addEventListener("submit", function () {

            if (!form.checkValidity()) {
                return;
            }

            const button = form.querySelector("button[type='submit']");

            button.disabled = true;

            button.innerHTML =
                '<span class="spinner-border spinner-border-sm me-2"></span> Signing In...';
        });
    }

    // Username Textbox
    const txtUser = document.getElementById("UserName");

    if (txtUser) {

        txtUser.addEventListener("input", updatePortalUI);

        txtUser.addEventListener("blur", updatePortalUI);

        // Set correct labels on page load
        updatePortalUI();
    }

    // Focus on Username
    if (txtUser) {
        txtUser.focus();
    }

    // Clock & Greeting
    updateClock();
    updateGreeting();

    setInterval(updateClock, 1000);

});
function updateClock() {

    const now = new Date();

    const optionsDate = {
        weekday: 'long',
        day: '2-digit',
        month: 'long',
        year: 'numeric'
    };

    document.getElementById("currentDate").innerHTML =
        now.toLocaleDateString('en-IN', optionsDate);

    document.getElementById("currentTime").innerHTML =
        now.toLocaleTimeString('en-IN');
}

function updateGreeting() {

    const hour = new Date().getHours();

    let greeting = "";

    if (hour < 12)
        greeting = "Good Morning";

    else if (hour < 17)
        greeting = "Good Afternoon";

    else
        greeting = "Good Evening";

    document.getElementById("greeting").innerHTML = greeting;
}