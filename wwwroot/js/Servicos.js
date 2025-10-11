
// Configurar data mínima quando a página carregar
document.addEventListener('DOMContentLoaded', function () {
    const dateInput = document.getElementById('dataDesejada');
    if (dateInput) {
        // Data mínima = amanhã
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        dateInput.min = tomorrow.toISOString().split('T')[0];
        dateInput.value = tomorrow.toISOString().split('T')[0];
    }
});

// ===== FUNÇÕES DO MODAL =====

// Abrir modal
function openModal(tipo) {
    const modal = document.getElementById('modalOrcamento');
    const titulo = document.getElementById('modalTitle');

    // Definir título
    const titulos = {
        'instalacao': 'Solicitar Orçamento - Instalação',
        'agendamento': 'Agendar Instalação',
        'manutencao': 'Solicitar Orçamento - Manutenção',
        'reparo': 'Solicitar Orçamento - Reparo',
        'suporte': 'Solicitar Orçamento - Suporte'
    };

    titulo.textContent = titulos[tipo] || 'Solicitar Orçamento';

    // Mostrar modal
    modal.style.display = 'block';
    document.body.style.overflow = 'hidden';
}

// Fechar modal
function closeModal() {
    const modal = document.getElementById('modalOrcamento');
    modal.style.display = 'none';
    document.body.style.overflow = 'auto';

    // Limpar formulário
    document.getElementById('formOrcamento').reset();
}

// Fechar modal de sucesso
function closeSuccessModal() {
    const modal = document.getElementById('modalSucesso');
    modal.style.display = 'none';
    document.body.style.overflow = 'auto';
}

// ===== ENVIO DO FORMULÁRIO ==============
function submitForm(event) {
    event.preventDefault();

    const form = event.target;
    const nome = form.nomeCliente.value.trim();
    const data = form.dataDesejada.value;
    const tipo = form.tipoExtintor.value;
    const quantidade = form.quantidade.value;

    // Validação simples
    if (!nome || !data || !tipo || !quantidade) {
        alert('Por favor, preencha todos os campos obrigatórios!');
        return;
    }

    if (nome.length < 2) {
        alert('Nome deve ter pelo menos 2 caracteres!');
        return;
    }

    if (parseInt(quantidade) < 1) {
        alert('Quantidade deve ser pelo menos 1!');
        return;
    }

    // Verificar se data é futura
    const dataSelecionada = new Date(data);
    const hoje = new Date();
    hoje.setHours(0, 0, 0, 0);

    if (dataSelecionada <= hoje) {
        alert('Data deve ser futura!');
        return;
    }

    // Simular envio
    const botao = form.querySelector('.btn-submit');
    botao.disabled = true;
    botao.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Enviando...';

    setTimeout(() => {
        closeModal();
        showSuccessModal();
    }, 1500);
}

// ===== MODAL DE SUCESSO =====
function showSuccessModal() {
    const modal = document.getElementById('modalSucesso');
    modal.style.display = 'block';
    document.body.style.overflow = 'hidden';
}

// ===== FECHAR MODAL COM ESC OU CLIQUE FORA =====
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closeModal();
        closeSuccessModal();
    }
});

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