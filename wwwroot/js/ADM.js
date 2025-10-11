// ===== JAVASCRIPT AVANÇADO PARA ADMIN =====



// Estatísticas calculadas
let dashboardStats = {
    totalChamados: 0,
    chamadosNovos: 0,
    chamadosAndamento: 0,
    chamadosFinalizados: 0,
    totalProdutos: 0,
    produtosEstoqueBaixo: 0,
    receitaMes: 0,
    ticketMedio: 0
};

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
    calculateStats();
    updateDashboardCards();
    updateRecentActivity();
    startRealTimeUpdates();
}

// Calcular estatísticas
function calculateStats() {
    dashboardStats.totalChamados = chamadosData.length;
    dashboardStats.chamadosNovos = chamadosData.filter(c => c.status === 'novo').length;
    dashboardStats.chamadosAndamento = chamadosData.filter(c => c.status === 'em-andamento').length;
    dashboardStats.chamadosFinalizados = chamadosData.filter(c => c.status === 'finalizado').length;

    dashboardStats.totalProdutos = produtosData.length;
    dashboardStats.produtosEstoqueBaixo = produtosData.filter(p => p.quantidade < 20).length;

    // Calcular receita do mês (chamados finalizados)
    const chamadosFinalizados = chamadosData.filter(c => c.status === 'finalizado');
    dashboardStats.receitaMes = chamadosFinalizados.reduce((total, c) => total + c.valor, 0);

    // Calcular ticket médio
    dashboardStats.ticketMedio = chamadosFinalizados.length > 0
        ? dashboardStats.receitaMes / chamadosFinalizados.length
        : 0;
}

// Atualizar cards do dashboard
function updateDashboardCards() {
    // Atualizar números principais
    updateStatCard('totalChamados', dashboardStats.totalChamados);
    updateStatCard('totalProdutos', dashboardStats.totalProdutos);

    // Adicionar novos cards se não existirem
    addAdvancedStatsCards();
}

// Atualizar card de estatística
function updateStatCard(elementId, value) {
    const element = document.getElementById(elementId);
    if (element) {
        animateNumber(element, parseInt(element.textContent) || 0, value);
    }
}

// Animação de números
function animateNumber(element, start, end) {
    const duration = 1000;
    const startTime = performance.now();

    function updateNumber(currentTime) {
        const elapsed = currentTime - startTime;
        const progress = Math.min(elapsed / duration, 1);

        const current = Math.floor(start + (end - start) * progress);
        element.textContent = current;

        if (progress < 1) {
            requestAnimationFrame(updateNumber);
        }
    }

    requestAnimationFrame(updateNumber);
}

// Adicionar cards de estatísticas avançadas
function addAdvancedStatsCards() {
    const dashboardGrid = document.querySelector('.dashboard-grid');
    if (!dashboardGrid) return;

    // Verificar se os cards já existem
    if (document.getElementById('receitaCard')) return;

    const advancedCards = `
        <div class="admin-card" id="receitaCard">
            <div class="card-icon">
                <i class="fas fa-dollar-sign"></i>
            </div>
            <div class="card-content">
                <h3>Receita do Mês</h3>
                <p>Total arrecadado com serviços finalizados</p>
                <div class="card-stats">
                    <span class="stat-number" id="receitaMes">${formatCurrency(dashboardStats.receitaMes)}</span>
                    <span class="stat-label">Este mês</span>
                </div>
            </div>
            <div class="card-action">
                <i class="fas fa-chart-line"></i>
            </div>
        </div>

        <div class="admin-card" id="ticketCard">
            <div class="card-icon">
                <i class="fas fa-receipt"></i>
            </div>
            <div class="card-content">
                <h3>Ticket Médio</h3>
                <p>Valor médio por serviço finalizado</p>
                <div class="card-stats">
                    <span class="stat-number" id="ticketMedio">${formatCurrency(dashboardStats.ticketMedio)}</span>
                    <span class="stat-label">Por serviço</span>
                </div>
            </div>
            <div class="card-action">
                <i class="fas fa-calculator"></i>
            </div>
        </div>

        <div class="admin-card" id="estoqueCard">
            <div class="card-icon">
                <i class="fas fa-exclamation-triangle"></i>
            </div>
            <div class="card-content">
                <h3>Estoque Baixo</h3>
                <p>Produtos com estoque abaixo de 20 unidades</p>
                <div class="card-stats">
                    <span class="stat-number" id="produtosEstoqueBaixo">${dashboardStats.produtosEstoqueBaixo}</span>
                    <span class="stat-label">Produtos</span>
                </div>
            </div>
            <div class="card-action">
                <i class="fas fa-warehouse"></i>
            </div>
        </div>
    `;

    dashboardGrid.insertAdjacentHTML('beforeend', advancedCards);
}

// Atualizações em tempo real
function startRealTimeUpdates() {
    // Simular atualizações a cada 30 segundos
    setInterval(() => {
        // Simular novos dados
        simulateNewData();
        calculateStats();
        updateDashboardCards();
        updateRecentActivity();
    }, 30000);
}

// Simular novos dados
function simulateNewData() {
    // 10% de chance de adicionar novo chamado
    if (Math.random() < 0.1) {
        const novosChamados = [
            { id: Date.now(), nome: 'Cliente Novo', status: 'novo', tipo: 'instalacao', data: new Date().toISOString().split('T')[0], telefone: '(11) 00000-0000', valor: 0 },
            { id: Date.now(), nome: 'Empresa ABC', status: 'novo', tipo: 'manutencao', data: new Date().toISOString().split('T')[0], telefone: '(11) 11111-1111', valor: 0 }
        ];

        const novoChamado = novosChamados[Math.floor(Math.random() * novosChamados.length)];
        chamadosData.push(novoChamado);

        showNotification(`Novo chamado recebido: ${novoChamado.nome}`, 'info');
    }
}

// Atualizar atividades recentes
function updateRecentActivity() {
    const activityList = document.getElementById('activityList');
    if (!activityList) return;

    // Gerar atividades baseadas nos dados reais
    const activities = generateRecentActivities();

    activityList.innerHTML = activities.map(activity => `
        <div class="activity-item" onclick="${activity.action || ''}">
            <div class="activity-icon">
                <i class="fas fa-${activity.icon} ${activity.colorClass}"></i>
            </div>
            <div class="activity-content">
                <p><strong>${activity.title}</strong></p>
                <span class="activity-time">${activity.time}</span>
            </div>
        </div>
    `).join('');
}

// Gerar atividades recentes baseadas nos dados
function generateRecentActivities() {
    const activities = [];

    // Últimos chamados
    const recentChamados = chamadosData
        .sort((a, b) => new Date(b.data) - new Date(a.data))
        .slice(0, 3);

    recentChamados.forEach(chamado => {
        let icon, colorClass, title;

        switch (chamado.status) {
            case 'novo':
                icon = 'clipboard-list';
                colorClass = 'text-warning';
                title = `Novo chamado: ${chamado.nome} - ${getTipoText(chamado.tipo)}`;
                break;
            case 'em-andamento':
                icon = 'cog';
                colorClass = 'text-info';
                title = `Chamado em andamento: ${chamado.nome}`;
                break;
            case 'finalizado':
                icon = 'check-circle';
                colorClass = 'text-success';
                title = `Chamado finalizado: ${chamado.nome} - ${formatCurrency(chamado.valor)}`;
                break;
        }

        activities.push({
            icon,
            colorClass,
            title,
            time: formatRelativeTime(chamado.data),
            action: `showChamadoDetails(${chamado.id})`
        });
    });

    // Produtos com estoque baixo
    const produtosEstoqueBaixo = produtosData.filter(p => p.quantidade < 20);
    if (produtosEstoqueBaixo.length > 0) {
        activities.push({
            icon: 'exclamation-triangle',
            colorClass: 'text-danger',
            title: `${produtosEstoqueBaixo.length} produto(s) com estoque baixo`,
            time: 'Agora',
            action: 'navigateTo("produtos")'
        });
    }

    return activities.slice(0, 5); // Máximo 5 atividades
}

// Formatar tempo relativo
function formatRelativeTime(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now - date;
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
    const diffMinutes = Math.floor(diffMs / (1000 * 60));

    if (diffDays > 0) {
        return `Há ${diffDays} dia${diffDays > 1 ? 's' : ''}`;
    } else if (diffHours > 0) {
        return `Há ${diffHours} hora${diffHours > 1 ? 's' : ''}`;
    } else if (diffMinutes > 0) {
        return `Há ${diffMinutes} minuto${diffMinutes > 1 ? 's' : ''}`;
    } else {
        return 'Agora';
    }
}

// Formatar moeda
function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(value);
}

// Obter texto do tipo
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
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');

    const icons = {
        success: 'check-circle',
        error: 'exclamation-circle',
        warning: 'exclamation-triangle',
        info: 'info-circle'
    };

    const colors = {
        success: '#28a745',
        error: '#dc3545',
        warning: '#ffc107',
        info: '#17a2b8'
    };

    notification.innerHTML = `
        <div style="display: flex; align-items: center; gap: 0.5rem;">
            <i class="fas fa-${icons[type] || icons.success}"></i>
            <span>${message}</span>
            <button onclick="this.parentElement.parentElement.remove()" style="background: none; border: none; color: white; cursor: pointer; margin-left: 10px;">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;

    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${colors[type] || colors.success};
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        z-index: 3000;
        animation: slideInRight 0.3s ease-out;
        max-width: 400px;
        word-wrap: break-word;
    `;

    // Adicionar animação CSS se não existir
    if (!document.getElementById('notification-styles')) {
        const style = document.createElement('style');
        style.id = 'notification-styles';
        style.textContent = `
            @keyframes slideInRight {
                from {
                    transform: translateX(100%);
                    opacity: 0;
                }
                to {
                    transform: translateX(0);
                    opacity: 1;
                }
            }
        `;
        document.head.appendChild(style);
    }

    document.body.appendChild(notification);

    // Auto-remover após 5 segundos
    setTimeout(() => {
        if (notification.parentNode) {
            notification.style.animation = 'slideInRight 0.3s ease-out reverse';
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.parentNode.removeChild(notification);
                }
            }, 300);
        }
    }, 5000);
}

// Função para mostrar detalhes do chamado (será usada nas atividades)
function showChamadoDetails(id) {
    const chamado = chamadosData.find(c => c.id === id);
    if (!chamado) return;

    showNotification(`Visualizando chamado: ${chamado.nome}`, 'info');
    // Aqui você pode abrir um modal ou navegar para a página de detalhes
}
