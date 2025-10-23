document.addEventListener('DOMContentLoaded', function () {
    setupModalActionButtons();
    setupModalCloseListeners();
});

function openDetailsModal(buttonElement) {
    const chamadoItem = buttonElement.closest('.chamado-item');
    if (!chamadoItem) return;

    const modal = document.getElementById('modalDetalhes');
    const detailsContainer = document.getElementById('chamadoDetails');
    const modalIdSpan = document.getElementById('modalChamadoId');
    const statusFormIdInput = document.getElementById('modalStatusFormId');

    if (!modal || !detailsContainer || !modalIdSpan || !statusFormIdInput) return;

    const data = chamadoItem.dataset; // Lê todos os data-*

    modalIdSpan.textContent = data.id || 'N/A';
    detailsContainer.innerHTML = `
        <div class="chamado-details-grid">
            <div class="detail-section">
                <h3><i class="fas fa-user"></i> Cliente</h3>
                <p><strong>Nome:</strong> ${data.cliente || 'N/A'}</p>
                <p><strong>Email Conta:</strong> ${data.email || 'N/A'}</p>
                <p><strong>Telefone:</strong> ${data.telefone || 'N/A'}</p>
            </div>
            <div class="detail-section">
                <h3><i class="fas fa-calendar-alt"></i> Datas</h3>
                <p><strong>Solicitação:</strong> ${data.datasolicitacao || 'N/A'}</p>
                <p><strong>Desejada:</strong> ${data.datadesejada || 'N/A'}</p>
            </div>
             <div class="detail-section">
                <h3><i class="fas fa-info-circle"></i> Serviço</h3>
                <p><strong>Tipo Serviço:</strong> ${data.tiposervico || 'N/A'}</p>
                <p><strong>Equipamento:</strong> ${data.tipoextintor || 'N/A'}</p>
                <p><strong>Quantidade:</strong> ${data.quantidade || 'N/A'}</p>
                <p><strong>Status Atual:</strong> <span class="status-badge status-${data.statustexto || ''}">${data.statustexto || 'N/A'}</span></p>
            </div>
            <div class="detail-section">
                <h3><i class="fas fa-map-marker-alt"></i> Endereço</h3>
                <p>${data.logradouro || ''}, ${data.numero || ''} ${data.complemento ? '- ' + data.complemento : ''}</p>
                <p>${data.bairro || ''} - ${data.cidade || ''}/${data.estado || ''}</p>
                <p><strong>CEP:</strong> ${data.cep || 'N/A'}</p>
            </div>
            <div class="detail-section full-width">
                 <h3><i class="fas fa-comment-dots"></i> Observações Cliente</h3>
                 <p>${data.observacoes || 'Nenhuma.'}</p>
            </div>
        </div>`;

    statusFormIdInput.value = data.id || '0';
    const currentStatusEnum = parseInt(data.statusenum, 10);
    const actionButtons = modal.querySelectorAll('.btn-status-action');

    actionButtons.forEach(btn => {
        const nextStatus = parseInt(btn.dataset.nextStatus, 10);
        if (nextStatus === 1 /* EmAndamento */) {
            btn.style.display = (currentStatusEnum === 0 /* Novo */) ? 'inline-block' : 'none';
        } else if (nextStatus === 2 /* Finalizado */) {
            btn.style.display = (currentStatusEnum === 1 /* EmAndamento */) ? 'inline-block' : 'none';
        } else {
            btn.style.display = 'none';
        }
    });

    modal.style.display = 'block';
    document.body.style.overflow = 'hidden';
}

function closeModal() {
    const modal = document.getElementById('modalDetalhes');
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
    }
}

function setupModalActionButtons() {
    const modal = document.getElementById('modalDetalhes');
    if (!modal) return;
    const form = document.getElementById('formAtualizaStatus');
    const novoStatusInput = document.getElementById('modalStatusFormNovoStatus');
    const actionButtons = modal.querySelectorAll('.btn-status-action');

    if (!form || !novoStatusInput) return;

    actionButtons.forEach(button => {
        button.addEventListener('click', function () {
            const nextStatus = this.dataset.nextStatus;
            if (nextStatus) {
                novoStatusInput.value = nextStatus;
                form.submit(); 
            }
        });
    });
}

function setupModalCloseListeners() {
    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape') closeModal();
    });
    window.addEventListener('click', function (event) {
        const modal = document.getElementById('modalDetalhes');
        if (event.target === modal) closeModal();
    });
}