function toggleProfileDropdown() {
    document.getElementById("profileDropdownMenu").classList.toggle("show");
}

// Fecha o dropdown se o usuário clicar fora dele
window.onclick = function (event) {
    if (!event.target.matches('.profile-btn') && !event.target.closest('.profile-btn')) {
        var dropdowns = document.getElementsByClassName("dropdown-menu");
        var i;
        for (i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show') && !openDropdown.contains(event.target)) {
                openDropdown.classList.remove('show');
            }
        }
    }
}