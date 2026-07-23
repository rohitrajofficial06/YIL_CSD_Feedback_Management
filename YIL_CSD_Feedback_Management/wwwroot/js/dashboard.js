//-----------------------------------------------------
// Navbar Scroll Effect
//-----------------------------------------------------

window.addEventListener("scroll", function () {

    const nav = document.querySelector(".navbar");

    if (window.scrollY > 60)
        nav.classList.add("scrolled");
    else
        nav.classList.remove("scrolled");

});

//-----------------------------------------------------
// Fade Animation
//-----------------------------------------------------

const observer = new IntersectionObserver(entries => {

    entries.forEach(entry => {

        if (entry.isIntersecting) {

            entry.target.classList.add("show");

        }

    });

}, {

    threshold: .15

});

document.querySelectorAll(".fade-up").forEach(el => {

    observer.observe(el);

});

//-----------------------------------------------------
// Counter Animation
//-----------------------------------------------------

const counters = document.querySelectorAll(".counter");

counters.forEach(counter => {

    counter.innerText = "0";

    const updateCounter = () => {

        const target = +counter.getAttribute("data-target");

        const current = +counter.innerText;

        const increment = target / 100;

        if (current < target) {

            counter.innerText = `${Math.ceil(current + increment)}`;

            setTimeout(updateCounter, 20);

        }
        else {

            counter.innerText = target;

        }

    };

    updateCounter();

});

//-----------------------------------------------------
// Scroll Top
//-----------------------------------------------------

const scrollBtn = document.querySelector(".scroll-top");

window.addEventListener("scroll", () => {

    if (window.pageYOffset > 300)
        scrollBtn.style.display = "flex";
    else
        scrollBtn.style.display = "none";

});

scrollBtn.addEventListener("click", () => {

    window.scrollTo({

        top: 0,

        behavior: "smooth"

    });

});

//-----------------------------------------------------
// Card Hover Glow
//-----------------------------------------------------

document.querySelectorAll(".feature-card").forEach(card => {

    card.addEventListener("mousemove", e => {

        const rect = card.getBoundingClientRect();

        const x = e.clientX - rect.left;

        const y = e.clientY - rect.top;

        card.style.background =
            `radial-gradient(circle at ${x}px ${y}px,
             rgba(255,212,0,.15),
             white 55%)`;

    });

    card.addEventListener("mouseleave", () => {

        card.style.background = "white";

    });

});

//-----------------------------------------------------
// Button Ripple
//-----------------------------------------------------

document.querySelectorAll(".btn").forEach(button => {

    button.addEventListener("click", function (e) {

        const ripple = document.createElement("span");

        const rect = button.getBoundingClientRect();

        ripple.style.left = e.clientX - rect.left + "px";

        ripple.style.top = e.clientY - rect.top + "px";

        ripple.className = "ripple";

        button.appendChild(ripple);

        setTimeout(() => {

            ripple.remove();

        }, 700);

    });

});