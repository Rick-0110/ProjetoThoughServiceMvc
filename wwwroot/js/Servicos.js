document.addEventListener('DOMContentLoaded', function () {

 
    const metaTagSucesso = document.querySelector("meta[name='show-success-modal']");

    if (metaTagSucesso) {
        console.log("Meta tag 'show-success-modal' encontrada. Abrindo modal...");
        const modalSucesso = document.getElementById('modalSucesso');
        if (modalSucesso) {
            modalSucesso.style.display = 'block'; 
            document.body.style.overflow = 'hidden';
        }
        metaTagSucesso.remove();
    }



    const dateInput = document.getElementById('dataDesejada');
    if (dateInput) {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        const minDate = tomorrow.toISOString().split('T')[0];
        dateInput.min = minDate;
    }


    const cepInput = document.getElementById('cep');
    if (cepInput) {
        cepInput.addEventListener('blur', buscarCepPelaApi);
    }

}); 




async function buscarCepPelaApi() {
    const cepInput = document.getElementById('cep');
    const cep = cepInput.value.replace(/\D/g, ''); 

    if (cep.length !== 8) {
        limparCamposEndereco();
        return;
    }

    setCamposEnderecoLoading(true);

    try {
        const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`);

        if (!response.ok) {
            throw new Error('Erro na rede ao buscar CEP.');
        }

        const data = await response.json();

     
        if (data.erro) {
         
            alert('CEP não encontrado. Por favor, digite o endereço manualmente.');
            limparCamposEndereco();
        } else {
     
            preencherCamposEndereco(data);
        }

    } catch (error) {
        // 6. Erro de rede/fetch (ex: sem internet)
        console.error('Falha ao buscar CEP:', error);
        alert('Não foi possível buscar o CEP. Verifique sua conexão.');
        limparCamposEndereco();
    } finally {
        // 7. Remove o feedback de "carregando" (mesmo se der erro)
        setCamposEnderecoLoading(false);
    }
}

// Função auxiliar para preencher os campos com os dados da API
function preencherCamposEndereco(data) {
    document.getElementById('logradouro').value = data.logradouro;
    document.getElementById('bairro').value = data.bairro;
    document.getElementById('cidade').value = data.localidade; // API ViaCEP usa 'localidade' para cidade
    document.getElementById('estado').value = data.uf;

    // Foca no campo "Número", que é o próximo a ser preenchido
    document.getElementById('numero').focus();
}

// Função para limpar os campos (se o CEP for inválido)
function limparCamposEndereco() {
    document.getElementById('logradouro').value = '';
    document.getElementById('bairro').value = '';
    document.getElementById('cidade').value = '';
    document.getElementById('estado').value = '';
}

// Função para travar/destravar campos e mostrar "Buscando..."
function setCamposEnderecoLoading(isLoading) {
    const campos = ['logradouro', 'bairro', 'cidade', 'estado'];
    campos.forEach(id => {
        const el = document.getElementById(id);
        if (el) {
            el.readOnly = isLoading;
            el.placeholder = isLoading ? 'Buscando...' : ''; 
        }
    });
}







function openModal(tipo) {
    const modal = document.getElementById('modalOrcamento');
    const titulo = document.getElementById('modalTitle');
    const tipoServicoInput = document.getElementById('tipoServicoHidden'); // Campo oculto

    const servicosInfo = {
        'manutencao': { titulo: 'Solicitar Orçamento - Manutenção', valor: 'Manutenção Preventiva' },
        'reparo': { titulo: 'Solicitar Orçamento - Reparo', valor: 'Reparo Técnico' },
        'instalacao': { titulo: 'Solicitar Orçamento - Instalação', valor: 'Instalação' },
        'suporte': { titulo: 'Solicitar Orçamento - Suporte', valor: 'Suporte Técnico' }
    };

    const info = servicosInfo[tipo] || { titulo: 'Solicitar Orçamento', valor: tipo };

    if (titulo) {
        titulo.textContent = info.titulo;
    }

    if (tipoServicoInput) {
        tipoServicoInput.value = info.valor;
    } else {
        console.error("Campo oculto 'tipoServicoHidden' não encontrado!");
    }

    if (modal) {
        modal.style.display = 'block';
        document.body.style.overflow = 'hidden';
    } else {
        console.error("Modal 'modalOrcamento' não encontrado.");
    }
} 

// Função para fechar o modal de orçamento
function closeModal() {
    const modal = document.getElementById('modalOrcamento');
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
    }
} 

// Função para fechar o modal de sucesso
function closeSuccessModal() {
    const modal = document.getElementById('modalSucesso');
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
    }
} 

// ===== FECHAR MODAL COM ESC OU CLIQUE FORA (Seu código original) =====

// Listener para a tecla ESC
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closeModal();
        closeSuccessModal();
    }
});

// Listener para cliques fora dos modais
window.addEventListener('click', function (event) {
    const modalOrcamento = document.getElementById('modalOrcamento');
    const modalSucesso = document.getElementById('modalSucesso');

    if (event.target === modalOrcamento) {
        closeModal();
    }
    if (event.target === modalSucesso) {
        closeSuccessModal();
    }
});