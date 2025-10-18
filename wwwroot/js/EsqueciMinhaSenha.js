document.addEventListener('DOMContentLoaded', function () {
   
    const form = document.getElementById('forgotPasswordForm');
    const emailInput = document.getElementById('email');
    const mensagemSucesso = document.getElementById('mensagemSucesso');
    const submitButton = form ? form.querySelector('.botao-entrar') : null; 


    if (!form) {
        console.error("Erro Crítico: Formulário com ID 'forgotPasswordForm' não encontrado no HTML.");
        return;
    }
    if (!emailInput) {
        console.error("Erro Crítico: Input com ID 'email' não encontrado no HTML.");
        return;
    }
    if (!mensagemSucesso) {
        console.error("Aviso: Elemento com ID 'mensagemSucesso' não encontrado. A mensagem de sucesso não será exibida.");
        
    }
    if (!submitButton) {
        console.error("Aviso: Botão de submit com classe '.botao-entrar' não encontrado dentro do formulário.");
       
    }

  
    emailInput.addEventListener('input', function () {
        const email = this.value.trim();
        
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        
        if (email && !emailRegex.test(email)) {
           
            this.style.borderColor = '#dc3545'; // Vermelho
        } else {
           
            this.style.borderColor = '#ddd';
        }
    });


    form.addEventListener('submit', async function (e) {
        
        e.preventDefault();

        
        limparErro();

        const email = emailInput.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

      
        if (!email) {
            mostrarErro('Por favor, digite seu email.');
            return;
        }
        if (!emailRegex.test(email)) {
            mostrarErro('Por favor, digite um email válido.');
            return;
        }

        if (submitButton) {
            submitButton.disabled = true;
            submitButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Enviando...';
        }

        
        try {
            
            const response = await fetch('/Registro/EnviarLinkRecuperacao', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    
                },
                body: JSON.stringify({ email: email }) 
            });

       
            if (response.ok) {
                
                form.style.display = 'none';
                if (mensagemSucesso) {
                    mensagemSucesso.style.display = 'block';
                }
            } else {
                
                const errorData = await response.json(); 
                mostrarErro(errorData.message || 'Ocorreu um erro. Tente novamente.');
            }

        } catch (error) {
            
            console.error('Erro na requisição fetch:', error);
            mostrarErro('Erro de conexão. Verifique sua internet e tente novamente.');
        } finally {
          
            if (submitButton) {
                submitButton.disabled = false;
                submitButton.innerHTML = 'Enviar Link'; 
            }
        }
    });

    // --- Funções Visuais Auxiliares ---

 
    function mostrarErro(mensagem) {
        limparErro(); 

        const mensagemErro = document.createElement('div');
        mensagemErro.className = 'mensagem-erro'; 
       
        mensagemErro.style.cssText = `
            background: #f8d7da; color: #721c24; padding: 12px;
            border-radius: 8px; margin-bottom: 20px; border: 1px solid #f5c6cb;
            font-size: 14px; text-align: center;
        `;
        mensagemErro.innerHTML = `<i class="fas fa-exclamation-triangle" style="margin-right: 8px;"></i> ${mensagem}`;

        
        form.insertBefore(mensagemErro, form.firstChild);
    }


    function limparErro() {
        const erroExistente = form.querySelector('.mensagem-erro');
        if (erroExistente) {
            erroExistente.remove();
        }
    }

    
    const inputs = form.querySelectorAll('input');
    inputs.forEach(input => {
        if (input.parentElement) {
            input.addEventListener('focus', function () {
                this.parentElement.style.boxShadow = '0 4px 8px rgba(0,0,0,0.1)'; 
                this.parentElement.style.transition = 'box-shadow 0.2s ease-in-out';
            });
            input.addEventListener('blur', function () {
                this.parentElement.style.boxShadow = 'none';
            });
        }
    });
});


window.voltarLogin = function () {
    
    window.location.href = '/Registro/Login';
};