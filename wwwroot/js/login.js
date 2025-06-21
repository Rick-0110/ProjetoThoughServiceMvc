document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.querySelector('.login-form');
    const registerLink = document.querySelector('.links a[href="registro.html"]');

    loginForm.addEventListener('submit', function(e) {
        e.preventDefault();
        
        const email = document.querySelector('input[type="email"]').value;
        const senha = document.querySelector('input[type="password"]').value;
        
        if (email && senha) {
            // Armazenar dados do usuário no localStorage
            const usuario = {
                email: email,
                nome: email.split('@')[0],
                logado: true
            };
            
            localStorage.setItem('usuarioLogado', JSON.stringify(usuario));
            
            // Redirecionar para a página principal
            window.location.href = 'paginaPrincipal.html';
        } else {
            alert('Por favor, preencha todos os campos!');
        }
    });

    if (registerLink) {
        registerLink.addEventListener('click', function(e) {
            e.preventDefault();
            window.location.href = 'registro.html';
        });
    }
});