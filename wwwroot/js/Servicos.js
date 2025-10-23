// Arquivo: wwwroot/js/Servicos.js

document.addEventListener('DOMContentLoaded', function () {

    // ===== CÓDIGO PARA MOSTRAR O MODAL DE SUCESSO =====
    // Procura pela <meta> tag que o Razor criou no <head>
    const metaTagSucesso = document.querySelector("meta[name='show-success-modal']");

    if (metaTagSucesso) {
        console.log("Meta tag 'show-success-modal' encontrada. Abrindo modal...");
        const modalSucesso = document.getElementById('modalSucesso');
        if (modalSucesso) {
            modalSucesso.style.display = 'block'; // Ou 'flex', dependendo do seu CSS
            document.body.style.overflow = 'hidden';
        }
        // Remove a tag depois de usá-la
        metaTagSucesso.remove();
    }
    // ===== FIM DO CÓDIGO DE SUCESSO =====


    // Define a data mínima no campo de data (seu código original)
    const dateInput = document.getElementById('dataDesejada');
    if (dateInput) {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        const minDate = tomorrow.toISOString().split('T')[0];
        dateInput.min = minDate;
    }

}); // Fim do DOMContentLoaded

// ===== FUNÇÕES VISUAIS DO MODAL (Seu código original) =====

// Função para abrir o modal de orçamento
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
} // Fim da função openModal

// Função para fechar o modal de orçamento
function closeModal() {
    const modal = document.getElementById('modalOrcamento');
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
    }
} // Fim da função closeModal

// Função para fechar o modal de sucesso
function closeSuccessModal() {
    const modal = document.getElementById('modalSucesso');
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
    }
} // Fim da função closeSuccessModal

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