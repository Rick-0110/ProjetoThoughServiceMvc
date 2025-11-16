document.addEventListener('DOMContentLoaded', function () {
    const toggle = document.getElementById('toggleSenha');
    const input = document.getElementById('senhaInput');

    if (toggle && input) {
        toggle.addEventListener('click', function () {
            const type = input.getAttribute('type') === 'password' ? 'text' : 'password';

            input.setAttribute('type', type);

            this.classList.toggle('fa-eye');
            this.classList.toggle('fa-eye-slash');
        });
    }
});