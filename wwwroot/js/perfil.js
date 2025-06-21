document.addEventListener('DOMContentLoaded', function() {
    // Carregar dados do usuário do localStorage
    const usuario = JSON.parse(localStorage.getItem('usuarioLogado'));
    
    if (usuario) {
        // Preencher informações do usuário
        document.querySelector('.perfil-header h1').textContent = usuario.nome;
        document.querySelector('.perfil-header .email').textContent = usuario.email;
        
        // Preencher informações pessoais
        document.querySelector('input[placeholder="Nome"]').value = usuario.nome;
        document.querySelector('input[placeholder="CPF / CNPJ"]').value = usuario.cpf;
        document.querySelector('input[placeholder="Email"]').value = usuario.email;
    }

    // Evento para alterar foto de perfil
    document.querySelector('.btn-alterar-foto').addEventListener('click', function() {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/*';
        input.click();
        
        input.addEventListener('change', function() {
            const file = this.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function(e) {
                    document.querySelector('.foto-perfil img').src = e.target.result;
                }
                reader.readAsDataURL(file);
            }
        });
    });

    // Evento para salvar alterações
    document.querySelectorAll('.btn-salvar').forEach(button => {
        button.addEventListener('click', function() {
            const card = this.closest('.info-card');
            const inputs = card.querySelectorAll('input');
            let dados = {};
            
            inputs.forEach(input => {
                dados[input.previousElementSibling.textContent] = input.value;
            });
            
            // Aqui você pode enviar os dados para o servidor
            alert('Alterações salvas com sucesso!');
        });
    });

    // Evento para o botão Meus Pedidos
    document.querySelector('.btn-pedidos').addEventListener('click', function() {
        window.location.href = 'pedidos.html';
    });

    // Evento para o botão Meus Serviços
    document.querySelector('.btn-servicos').addEventListener('click', function() {
        window.location.href = 'servicos.html';
    });

    // Evento para o botão Sair
    document.querySelector('.btn-sair').addEventListener('click', function() {
        localStorage.removeItem('usuarioLogado');
        window.location.href = 'login.html';
    });
});
