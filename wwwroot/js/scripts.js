/*!
* Start Bootstrap - Shop Homepage v5.0.6 (https://startbootstrap.com/template/shop-homepage)
* Copyright 2013-2023 Start Bootstrap
* Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-shop-homepage/blob/master/LICENSE)
*/

// Função para toggle do dropdown do perfil
function toggleProfileDropdown() {
    const dropdown = document.getElementById('profileDropdownMenu');
    if (dropdown) {
        dropdown.classList.toggle('show');
    }
}

// Fechar dropdown ao clicar fora
document.addEventListener('DOMContentLoaded', function() {
    document.addEventListener('click', function(event) {
        const dropdown = document.getElementById('profileDropdownMenu');
        const profileBtn = document.querySelector('.profile-btn');
        
        if (dropdown && profileBtn && 
            !dropdown.contains(event.target) && 
            !profileBtn.contains(event.target)) {
            dropdown.classList.remove('show');
        }
    });
});