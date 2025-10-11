// ===== JAVASCRIPT SIMPLES PARA ADMIN =====


// Carregar dados quando a página abrir
document.addEventListener('DOMContentLoaded', function () {
    loadDashboardData();
});

// Navegar para outras páginas
function navigateTo(page) {
    if (page === 'chamados') {
        window.location.href = 'admin-chamados.html';
    } else if (page === 'produtos') {
        window.location.href = 'AdicionarProdutoADM.html';
    } else {
        alert('Funcionalidade em desenvolvimento!');
    }
}

// Carregar dados do dashboard
function loadDashboardData() {
    // Atualizar números
    document.getElementById('totalChamados').textContent = chamadosData.length;
    document.getElementById('totalProdutos').textContent = produtosData.length;

    // Atualizar atividades
    updateRecentActivity();
}

// Atualizar atividades recentes
function updateRecentActivity() {
    const activityList = document.getElementById('activityList');
    if (!activityList) return;

    activityList.innerHTML = `
        <div class="activity-item">
            <div class="activity-icon">
                <i class="fas fa-plus text-success"></i>
            </div>
            <div class="activity-content">
                <p><strong>Novo produto adicionado: Extintor PQS 4kg</strong></p>
                <span class="activity-time">Há 2 horas</span>
            </div>
        </div>
        <div class="activity-item">
            <div class="activity-icon">
                <i class="fas fa-clipboard-list text-warning"></i>
            </div>
            <div class="activity-content">
                <p><strong>Novo chamado recebido: Solicitação de instalação</strong></p>
                <span class="activity-time">Há 4 horas</span>
            </div>
        </div>
        <div class="activity-item">
            <div class="activity-icon">
                <i class="fas fa-check text-success"></i>
            </div>
            <div class="activity-content">
                <p><strong>Chamado finalizado: Orçamento de manutenção</strong></p>
                <span class="activity-time">Ontem</span>
            </div>
        </div>
    `;
}

// Exportar dados
function exportData() {
    const data = {
        chamados: chamadosData,
        produtos: produtosData,
        exportadoEm: new Date().toISOString()
    };

    const dataStr = JSON.stringify(data, null, 2);
    const dataBlob = new Blob([dataStr], { type: 'application/json' });

    const link = document.createElement('a');
    link.href = URL.createObjectURL(dataBlob);
    link.download = `dados-admin-${new Date().toISOString().split('T')[0]}.json`;
    link.click();

    showNotification('Dados exportados com sucesso!');
}

// Mostrar notificação
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
