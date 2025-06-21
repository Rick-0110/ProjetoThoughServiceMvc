document.addEventListener('DOMContentLoaded', function() {
    const registerForm = document.querySelector('.register-form');
    const loginLink = document.querySelector('.login-link');

    registerForm.addEventListener('submit', function(e) {
        e.preventDefault();
        
        // Capturar dados do formulário
        const nome = document.querySelector('input[placeholder="Nome"]').value;
        const email = document.querySelector('input[placeholder="Email"]').value;
        const senha = document.querySelector('input[placeholder="Senha"]').value;
        const confirmarSenha = document.querySelector('input[placeholder="Confirmar Senha"]').value;
        
        // Validar senhas
        if (senha !== confirmarSenha) {
            alert('As senhas não coincidem!');
            return;
        }
        
        // Armazenar dados do usuário no localStorage
        const usuario = {
            nome: nome,
            email: email,
            logado: true
        };
        
        localStorage.setItem('usuarioLogado', JSON.stringify(usuario));
        
        // Redirecionar para a página principal
        window.location.href = 'paginaPrincipal.html';
    });

    loginLink.addEventListener('click', function(e) {
        e.preventDefault();
        window.location.href = 'login.html';
    });
}); 