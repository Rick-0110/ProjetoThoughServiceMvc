// Arquivo: ~/js/Servicos.js

// Define a data mínima no campo de data quando a página carrega
document.addEventListener('DOMContentLoaded', function () {
    const dateInput = document.getElementById('dataDesejada');
    if (dateInput) {
        // Define a data mínima como amanhã
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        const minDate = tomorrow.toISOString().split('T')[0];
        dateInput.min = minDate;
        // Não pré-seleciona a data, deixa o usuário escolher
    }
}); // <<<--- Fechamento do DOMContentLoaded

// ===== FUNÇÕES VISUAIS DO MODAL =====

// Função para abrir o modal de orçamento
function openModal(tipo) {
    const modal = document.getElementById('modalOrcamento');
    const titulo = document.getElementById('modalTitle');
    const tipoServicoInput = document.getElementById('tipoServicoHidden'); // Campo oculto do formulário

    // Mapeamento dos tipos de serviço para os textos
    const servicosInfo = {
        'manutencao': { titulo: 'Solicitar Orçamento - Manutenção', valor: 'Manutenção Preventiva' },
        'reparo': { titulo: 'Solicitar Orçamento - Reparo', valor: 'Reparo Técnico' },
        'instalacao': { titulo: 'Solicitar Orçamento - Instalação', valor: 'Instalação' },
        'suporte': { titulo: 'Solicitar Orçamento - Suporte', valor: 'Suporte Técnico' }
        // Adicione mais tipos se houver outros botões
    };

    // Pega as informações do serviço ou usa um padrão
    const info = servicosInfo[tipo] || { titulo: 'Solicitar Orçamento', valor: tipo };

    // Atualiza o título do modal (visual)
    if (titulo) {
        titulo.textContent = info.titulo;
    }

    // Preenche o valor do campo oculto que será enviado para o C#
    if (tipoServicoInput) {
        tipoServicoInput.value = info.valor;
    } else {
        // Alerta no console se o campo oculto não for encontrado no HTML
        console.error("Campo oculto 'tipoServicoHidden' não encontrado no formulário!");
    }

    // Mostra o modal (visual)
    if (modal) {
        modal.style.display = 'block';
        document.body.style.overflow = 'hidden'; // Trava o scroll da página
    } else {
        // Alerta no console se o modal não for encontrado no HTML
        console.error("Modal com ID 'modalOrcamento' não encontrado.");
    }
} // <<<--- Fechamento da função openModal

// Função para fechar o modal de orçamento
function closeModal() {
    const modal = document.getElementById('modalOrcamento');
    if (modal) {
        modal.style.display = 'none'; // Esconde o modal
        document.body.style.overflow = 'auto'; // Libera o scroll da página
        // Não precisa resetar o formulário aqui, o C# fará o redirect/reload
    }
} // <<<--- Fechamento da função closeModal

// Função para fechar o modal de sucesso (mantida caso use em outro contexto)
function closeSuccessModal() {
    const modal = document.getElementById('modalSucesso');
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
    }
} // <<<--- Fechamento da função closeSuccessModal

// ===== FECHAR MODAL COM ESC OU CLIQUE FORA (Comportamento visual) =====

// Listener para a tecla ESC
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closeModal(); // Fecha o modal de orçamento
        closeSuccessModal(); // Tenta fechar o de sucesso também, se estiver aberto
    }
}); // <<<--- Fechamento do listener keydown

// Listener para cliques fora dos modais
window.addEventListener('click', function (event) {
    const modalOrcamento = document.getElementById('modalOrcamento');
    const modalSucesso = document.getElementById('modalSucesso');

    // Se o clique foi diretamente no fundo escuro do modal de orçamento
    if (event.target === modalOrcamento) {
        closeModal();
    }
    // Se o clique foi diretamente no fundo escuro do modal de sucesso
    if (event.target === modalSucesso) {
        closeSuccessModal();
    }
}); // <<<--- Fechamento do listener click