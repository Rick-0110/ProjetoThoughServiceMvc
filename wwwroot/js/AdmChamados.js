// ===== JAVASCRIPT SIMPLES PARA CHAMADOS =====


// Carregar quando a página abrir
document.addEventListener('DOMContentLoaded', function () {
    loadChamados();
    updateStats();
});

// Carregar lista de chamados
function loadChamados() {
    const chamadosList = document.getElementById('chamadosList');
    if (!chamadosList) return;

    if (chamadosData.length === 0) {
        chamadosList.innerHTML = '<p>Nenhum chamado encontrado</p>';
        return;
    }

    chamadosList.innerHTML = chamadosData.map(chamado => `
        <div class="chamado-item" onclick="showChamadoDetails(${chamado.id})">
            <div class="chamado-header">
                <div>
                    <div class="chamado-title">${chamado.nome}</div>
                    <div class="chamado-status status-${chamado.status}">${getStatusText(chamado.status)}</div>
                </div>
                <div class="chamado-date">${formatDate(chamado.data)}</div>
            </div>
            
            <div class="chamado-info">
                <div class="chamado-detail">
                    <strong>Tipo:</strong> ${getTipoText(chamado.tipo)}
                </div>
                <div class="chamado-detail">
                    <strong>Equipamento:</strong> ${chamado.tipoExtintor}
                </div>
                <div class="chamado-detail">
                    <strong>Quantidade:</strong> ${chamado.quantidade}
                </div>
                <div class="chamado-detail">
                    <strong>Contato:</strong> ${chamado.telefone}
                </div>
            </div>
            
            <div class="chamado-actions">
                <button class="btn-primary btn-sm" onclick="event.stopPropagation(); showChamadoDetails(${chamado.id})">
                    <i class="fas fa-eye"></i> Ver Detalhes
                </button>
            </div>
        </div>
    `).join('');
}

// Filtrar chamados
function filterChamados() {
    const statusFilter = document.getElementById('statusFilter').value;
    const tipoFilter = document.getElementById('tipoFilter').value;

    let filtered = chamadosData;

    if (statusFilter) {
        filtered = filtered.filter(c => c.status === statusFilter);
    }

    if (tipoFilter) {
        filtered = filtered.filter(c => c.tipo === tipoFilter);
    }

    // Atualizar lista com filtros
    const chamadosList = document.getElementById('chamadosList');
    if (filtered.length === 0) {
        chamadosList.innerHTML = '<p>Nenhum chamado encontrado</p>';
        return;
    }

    chamadosList.innerHTML = filtered.map(chamado => `
        <div class="chamado-item" onclick="showChamadoDetails(${chamado.id})">
            <div class="chamado-header">
                <div>
                    <div class="chamado-title">${chamado.nome}</div>
                    <div class="chamado-status status-${chamado.status}">${getStatusText(chamado.status)}</div>
                </div>
                <div class="chamado-date">${formatDate(chamado.data)}</div>
            </div>
            
            <div class="chamado-info">
                <div class="chamado-detail">
                    <strong>Tipo:</strong> ${getTipoText(chamado.tipo)}
                </div>
                <div class="chamado-detail">
                    <strong>Equipamento:</strong> ${chamado.tipoExtintor}
                </div>
                <div class="chamado-detail">
                    <strong>Quantidade:</strong> ${chamado.quantidade}
                </div>
                <div class="chamado-detail">
                    <strong>Contato:</strong> ${chamado.telefone}
                </div>
            </div>
            
            <div class="chamado-actions">
                <button class="btn-primary btn-sm" onclick="event.stopPropagation(); showChamadoDetails(${chamado.id})">
                    <i class="fas fa-eye"></i> Ver Detalhes
                </button>
            </div>
        </div>
    `).join('');
}

// Atualizar estatísticas
function updateStats() {
    const total = chamadosData.length;
    const novos = chamadosData.filter(c => c.status === 'novo').length;
    const andamento = chamadosData.filter(c => c.status === 'em-andamento').length;
    const finalizados = chamadosData.filter(c => c.status === 'finalizado').length;

    document.getElementById('totalChamados').textContent = total;
    document.getElementById('chamadosNovos').textContent = novos;
    document.getElementById('chamadosAndamento').textContent = andamento;
    document.getElementById('chamadosFinalizados').textContent = finalizados;
}

// Mostrar detalhes do chamado
function showChamadoDetails(id) {
    const chamado = chamadosData.find(c => c.id === id);
    if (!chamado) return;

    const modal = document.getElementById('modalDetalhes');
    const details = document.getElementById('chamadoDetails');

    details.innerHTML = `
        <div class="chamado-details-grid">
            <div class="detail-section">
                <h3>Informações do Cliente</h3>
                <div class="detail-item">
                    <strong>Nome:</strong> ${chamado.nome}
                </div>
                <div class="detail-item">
                    <strong>Telefone:</strong> ${chamado.telefone}
                </div>
            </div>
            
            <div class="detail-section">
                <h3>Detalhes do Serviço</h3>
                <div class="detail-item">
                    <strong>Tipo:</strong> ${getTipoText(chamado.tipo)}
                </div>
                <div class="detail-item">
                    <strong>Data:</strong> ${formatDate(chamado.data)}
                </div>
                <div class="detail-item">
                    <strong>Status:</strong> 
                    <span class="chamado-status status-${chamado.status}">${getStatusText(chamado.status)}</span>
                </div>
            </div>
            
            <div class="detail-section">
                <h3>Equipamentos</h3>
                <div class="detail-item">
                    <strong>Tipo:</strong> ${chamado.tipoExtintor}
                </div>
                <div class="detail-item">
                    <strong>Quantidade:</strong> ${chamado.quantidade}
                </div>
            </div>
            
            <div class="detail-section full-width">
                <h3>Observações</h3>
                <div class="observacoes">${chamado.observacoes}</div>
            </div>
        </div>
    `;

    modal.style.display = 'block';
    document.body.style.overflow = 'hidden';
}

// Fechar modal
function closeModal() {
    const modal = document.getElementById('modalDetalhes');
    modal.style.display = 'none';
    document.body.style.overflow = 'auto';
}

// Ações do chamado
function marcarEmAndamento() {
    const chamado = chamadosData[0]; // Simplificado
    if (chamado) {
        chamado.status = 'em-andamento';
        loadChamados();
        updateStats();
        closeModal();
        showNotification('Chamado marcado como "Em Andamento"');
    }
}

function finalizarChamado() {
    const chamado = chamadosData[0]; // Simplificado
    if (chamado) {
        chamado.status = 'finalizado';
        loadChamados();
        updateStats();
        closeModal();
        showNotification('Chamado finalizado com sucesso');
    }
}

// Atualizar chamados
function refreshChamados() {
    showNotification('Atualizando chamados...');
    setTimeout(() => {
        loadChamados();
        updateStats();
        showNotification('Chamados atualizados!');
    }, 1000);
}

// Exportar chamados
function exportChamados() {
    const data = {
        chamados: chamadosData,
        exportadoEm: new Date().toISOString()
    };

    const dataStr = JSON.stringify(data, null, 2);
    const dataBlob = new Blob([dataStr], { type: 'application/json' });

    const link = document.createElement('a');
    link.href = URL.createObjectURL(dataBlob);
    link.download = `chamados-${new Date().toISOString().split('T')[0]}.json`;
    link.click();

    showNotification('Chamados exportados com sucesso!');
}

// Funções auxiliares
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
}

function getStatusText(status) {
    const statusMap = {
        'novo': 'Novo',
        'em-andamento': 'Em Andamento',
        'finalizado': 'Finalizado'
    };
    return statusMap[status] || status;
}

function getTipoText(tipo) {
    const tipoMap = {
        'instalacao': 'Instalação',
        'agendamento': 'Agendamento',
        'manutencao': 'Manutenção',
        'reparo': 'Reparo',
        'suporte': 'Suporte'
    };
    return tipoMap[tipo] || tipo;
}

// Notificação simples
function showNotification(message) {
    const notification = document.createElement('div');
    notification.innerHTML = `
        <div style="display: flex; align-items: center; gap: 0.5rem;">
            <i class="fas fa-check-circle"></i>
            <span>${message}</span>
        </div>
    `;

    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #28a745;
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        z-index: 3000;
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        if (notification.parentNode) {
            notification.parentNode.removeChild(notification);
        }
    }, 3000);
}

// Fechar modal com ESC
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closeModal();
    }
});

// Fechar modal clicando fora
window.addEventListener('click', function (event) {
    const modal = document.getElementById('modalDetalhes');
    if (event.target === modal) {
        closeModal();
    }
});
