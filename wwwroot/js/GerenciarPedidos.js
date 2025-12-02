document.addEventListener('DOMContentLoaded', () => {
    setupModalActionButtons();
    setupModalCloseListeners();
});

function openDetailsModal(buttonElement) {
    // Tenta encontrar o elemento pai por várias classes ou pela tag TR
    const pedidoItem = buttonElement.closest('.chamado-item') ||
        buttonElement.closest('.pedido-item') ||
        buttonElement.closest('tr');

    if (!pedidoItem) {
        console.error("Erro: Não foi possível encontrar o elemento pai do pedido/chamado.");
        return;
    }

    const modal = document.getElementById('modalDetalhes');
    const detailsContainer = document.getElementById('pedidoDetails');
    const modalIdSpan = document.getElementById('modalPedidoId');
    const statusFormIdInput = document.getElementById('modalStatusFormId');

    if (!modal || !detailsContainer || !modalIdSpan || !statusFormIdInput) {
        console.error("Erro: Elementos do modal não encontrados no DOM.");
        return;
    }

    const data = pedidoItem.dataset;
    const formatCurrency = createCurrencyFormatter();

    modalIdSpan.textContent = data.id || 'N/A';

    // ... (Restante do seu código de preenchimento do HTML continua igual)
    detailsContainer.innerHTML = `
        <div class="chamado-details-grid">
            <div class="detail-section">
                <h3><i class="fas fa-user"></i> Cliente</h3>
                <p><strong>Nome:</strong> ${data.cliente || 'N/A'}</p>
                <p><strong>Email:</strong> ${data.email || 'N/A'}</p>
                <p><strong>Telefone:</strong> ${data.telefone || 'N/A'}</p>
            </div>
            <div class="detail-section">
                <h3><i class="fas fa-calendar-alt"></i> Data do Pedido</h3>
                <p><strong>Data:</strong> ${data.datapedido || 'N/A'}</p>
                <p><strong>Status Atual:</strong> <span class="status-badge status-${(data.status || '').toLowerCase()}">${data.status || 'N/A'}</span></p>
            </div>
            <div class="detail-section">
                <h3><i class="fas fa-map-marker-alt"></i> Endereço de Entrega</h3>
                <p>${data.logradouro || ''}, ${data.numero || ''} ${data.complemento ? '- ' + data.complemento : ''}</p>
                <p>${data.bairro || ''} - ${data.cidade || ''}/${data.estado || ''}</p>
                <p><strong>CEP:</strong> ${data.cep || 'N/A'}</p>
            </div>
            <div class="detail-section">
                <h3><i class="fas fa-shipping-fast"></i> Entrega e Pagamento</h3>
                <p><strong>Método de Envio:</strong> ${data.metodoenvio || 'N/A'}</p>
                <p><strong>Método de Pagamento:</strong> ${data.metodopagamento || 'N/A'}</p>
            </div>
            <div class="detail-section full-width">
                <h3><i class="fas fa-box"></i> Itens do Pedido</h3>
                <div id="pedidoItensList">
                    <p>Carregando itens...</p>
                </div>
            </div>
            <div class="detail-section">
                <h3><i class="fas fa-calculator"></i> Valores</h3>
                <p><strong>Subtotal:</strong> ${formatCurrency(data.subtotal)}</p>
                <p><strong>Frete:</strong> ${formatCurrency(data.envio)}</p>
                <p><strong>Desconto:</strong> ${formatCurrency(data.desconto)}</p>
                <p><strong style="font-size: 1.2em; color: #28a745;">Total:</strong> <strong style="font-size: 1.2em; color: #28a745;">${formatCurrency(data.total)}</strong></p>
            </div>
            <div class="detail-section full-width">
                <h3><i class="fas fa-comment-dots"></i> Observações</h3>
                <p>${data.observacoes || 'Nenhuma observação.'}</p>
            </div>
        </div>`;

    renderPedidoItens(data.itens);

    statusFormIdInput.value = data.id || '0';

    // ... (Restante da lógica dos botões continua igual)
    const currentStatusEnum = parseInt(data.statusenum, 10);
    const actionButtons = modal.querySelectorAll('.btn-status-action');

    actionButtons.forEach(btn => {
        const nextStatus = parseInt(btn.dataset.nextStatus, 10);
        btn.style.display = 'none';

        if (currentStatusEnum === 0) {
            if (nextStatus === 1 || nextStatus === 5) btn.style.display = 'inline-block';
        } else if (currentStatusEnum === 1) {
            if (nextStatus === 2 || nextStatus === 3 || nextStatus === 5) btn.style.display = 'inline-block';
        } else if (currentStatusEnum === 2) {
            if (nextStatus === 3 || nextStatus === 5) btn.style.display = 'inline-block';
        } else if (currentStatusEnum === 3) {
            if (nextStatus === 4) btn.style.display = 'inline-block';
        }
    });

    modal.style.display = 'block';
    document.body.style.overflow = 'hidden';
}
function renderPedidoItens(itensJson) {
    const itensContainer = document.getElementById('pedidoItensList');
    if (!itensContainer) return;

    // Verifica se está vazio ou nulo
    if (!itensJson || itensJson === "[]") {
        itensContainer.innerHTML = '<p>Nenhum item encontrado.</p>';
        return;
    }

    try {
        let safeJson = itensJson;

        // 1. Decodifica HTML Entities se houver (&quot; -> ")
        if (safeJson.includes('&quot;')) {
            const parser = new DOMParser();
            safeJson = parser.parseFromString(`<!doctype html><body>${itensJson}`, 'text/html').body.textContent;
        }

        // 2. CORREÇÃO DO SEU ERRO: Troca aspas simples por duplas se necessário
        // O erro "position 2" indica que começa com [{'
        if (safeJson.indexOf("[{'") === 0 || safeJson.indexOf("['") === 0) {
            // Substitui todas as aspas simples por duplas
            // CUIDADO: Isso pode quebrar se houver apóstrofos no nome do produto (ex: McDonald's)
            safeJson = safeJson.replace(/'/g, '"');
        }

        const itens = JSON.parse(safeJson);
        const formatCurrency = createCurrencyFormatter();

        if (!Array.isArray(itens) || itens.length === 0) {
            itensContainer.innerHTML = '<p>Nenhum item encontrado.</p>';
            return;
        }

        let html = '<table style="width: 100%; border-collapse: collapse; margin-top: 10px; font-size: 0.9rem;">';
        html += '<thead><tr style="background: #f8f9fa; text-align: left;">';
        html += '<th style="padding: 8px; border-bottom: 2px solid #dee2e6;">Produto</th>';
        html += '<th style="padding: 8px; border-bottom: 2px solid #dee2e6; text-align: center;">Qtd</th>';
        html += '<th style="padding: 8px; border-bottom: 2px solid #dee2e6; text-align: right;">Preço Unit.</th>';
        html += '<th style="padding: 8px; border-bottom: 2px solid #dee2e6; text-align: right;">Subtotal</th>';
        html += '</tr></thead><tbody>';

        itens.forEach(item => {
            html += `<tr style="border-bottom: 1px solid #eee;">
                <td style="padding: 8px;">${item.ProdutoNome || item.Nome || 'Produto sem nome'}</td>
                <td style="padding: 8px; text-align: center;">${item.Quantidade || 0}</td>
                <td style="padding: 8px; text-align: right;">${formatCurrency(item.PrecoUnitario || item.Preco)}</td>
                <td style="padding: 8px; text-align: right; font-weight: bold;">${formatCurrency(item.Subtotal || (item.Quantidade * (item.PrecoUnitario || item.Preco)))}</td>
            </tr>`;
        });

        html += '</tbody></table>';
        itensContainer.innerHTML = html;

    } catch (error) {
        console.error('Erro CRÍTICO ao processar JSON:', error);
        console.log('JSON Recebido:', itensJson); // Isso vai ajudar você a ver o erro no F12
        itensContainer.innerHTML = '<p style="color: red;">Erro de dados nos itens. Verifique o console.</p>';
    }
}

function createCurrencyFormatter() {
    return (value) => {
        const num = parseFloat(value) || 0;
        return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(num);
    };
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
            const statusText = this.textContent.trim();
            
            if (nextStatus && confirm(`Tem certeza que deseja ${statusText.toLowerCase()}?`)) {
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

