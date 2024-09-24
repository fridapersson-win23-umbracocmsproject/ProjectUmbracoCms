const toggleMenu = () => {
    const container = document.querySelector(".container");
    const navbar = document.querySelector(".navbar");
    const menuOpened = container.classList.contains("toggle-mobile-menu");

    //document.querySelector("#btn-switch").classList.toggle("hide");
    //document.querySelector(".nav").classList.toggle("hide");
    //document.querySelector(".account-buttons").classList.toggle("hide");

    //changing bars-icon to X-icon
    document.querySelector("#mobile-icon").classList.toggle("fa-bars");
    document.querySelector("#mobile-icon").classList.toggle("fa-x");

    navbar.classList.toggle("toggle-mobile-menu-active");
    container.classList.toggle("toggle-mobile-menu");

    container.setAttribute("aria-expanded", !menuOpened);

}

const checkScreenSize = () => {
    if (window.innerWidth >= 992) {
        if (document.querySelector(".container").classList.contains("toggle-mobile-menu")) {
            toggleMenu();
        }
    }
}

window.addEventListener("resize", checkScreenSize);