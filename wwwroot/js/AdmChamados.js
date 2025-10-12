// ===== JAVASCRIPT SIMPLES PARA CHAMADOS =====

// Dados simulados mais completos
let chamadosData = [
    {
        id: 1,
        nome: 'João Silva',
        email: 'joao.silva@email.com',
        data: '2024-01-15',
        tipo: 'instalacao',
        status: 'novo',
        prioridade: 'media',
        tipoExtintor: 'Extintor PQS 4kg',
        quantidade: 5,
        observacoes: 'Instalação em escritório comercial no centro da cidade',
        telefone: '(11) 99999-9999',
        endereco: 'Rua das Flores, 123 - Centro',
        cidade: 'São Paulo',
        valor: 450.00,
        tecnico: '',
        dataInicio: '',
        dataFim: '',
        historico: [
            { data: '2024-01-15', acao: 'Chamado criado', usuario: 'Sistema' }
        ]
    },
    {
        id: 2,
        nome: 'Maria Santos',
        email: 'maria.santos@empresa.com',
        data: '2024-01-16',
        tipo: 'agendamento',
        status: 'em-andamento',
        prioridade: 'alta',
        tipoExtintor: 'Sistema de Sprinklers',
        quantidade: 1,
        observacoes: 'Sistema completo para indústria - urgente',
        telefone: '(11) 88888-8888',
        endereco: 'Av. Industrial, 456 - Zona Industrial',
        cidade: 'São Paulo',
        valor: 2500.00,
        tecnico: 'Carlos Oliveira',
        dataInicio: '2024-01-16',
        dataFim: '',
        historico: [
            { data: '2024-01-16', acao: 'Chamado criado', usuario: 'Sistema' },
            { data: '2024-01-16', acao: 'Técnico designado: Carlos Oliveira', usuario: 'Admin' },
            { data: '2024-01-16', acao: 'Status alterado para: Em Andamento', usuario: 'Admin' }
        ]
    },
    {
        id: 3,
        nome: 'Pedro Costa',
        email: 'pedro.costa@loja.com',
        data: '2024-01-14',
        tipo: 'manutencao',
        status: 'finalizado',
        prioridade: 'baixa',
        tipoExtintor: 'Extintor CO2 5kg',
        quantidade: 3,
        observacoes: 'Manutenção preventiva trimestral',
        telefone: '(11) 77777-7777',
        endereco: 'Rua Comercial, 789 - Centro',
        cidade: 'São Paulo',
        valor: 180.00,
        tecnico: 'Ana Silva',
        dataInicio: '2024-01-14',
        dataFim: '2024-01-14',
        historico: [
            { data: '2024-01-14', acao: 'Chamado criado', usuario: 'Sistema' },
            { data: '2024-01-14', acao: 'Técnico designado: Ana Silva', usuario: 'Admin' },
            { data: '2024-01-14', acao: 'Status alterado para: Em Andamento', usuario: 'Admin' },
            { data: '2024-01-14', acao: 'Serviço finalizado', usuario: 'Ana Silva' },
            { data: '2024-01-14', acao: 'Status alterado para: Finalizado', usuario: 'Admin' }
        ]
    },
    {
        id: 4,
        nome: 'Ana Oliveira',
        email: 'ana.oliveira@hotel.com',
        data: '2024-01-17',
        tipo: 'reparo',
        status: 'novo',
        prioridade: 'alta',
        tipoExtintor: 'Detector de Fumaça',
        quantidade: 10,
        observacoes: 'Reparo urgente - sistema de detecção com falha',
        telefone: '(11) 66666-6666',
        endereco: 'Av. Hotel, 321 - Zona Sul',
        cidade: 'São Paulo',
        valor: 320.00,
        tecnico: '',
        dataInicio: '',
        dataFim: '',
        historico: [
            { data: '2024-01-17', acao: 'Chamado criado', usuario: 'Sistema' }
        ]
    }
];

// Filtros ativos
let activeFilters = {
    status: '',
    tipo: '',
    prioridade: '',
    busca: '',
    ordenacao: 'data-desc'
};

// Carregar quando a página abrir
document.addEventListener('DOMContentLoaded', function () {
    initializeChamadosPage();
    loadChamados();
    updateStats();
    setupEventListeners();
});

// Inicializar página de chamados
function initializeChamadosPage() {
    // Adicionar filtros avançados se não existirem
    addAdvancedFilters();

    // Adicionar estatísticas de chamados
    addChamadosStats();

    // Configurar busca em tempo real
    setupRealTimeSearch();
}

// Adicionar filtros avançados
function addAdvancedFilters() {
    const filtersSection = document.querySelector('.filters-section');
    if (!filtersSection || document.getElementById('advancedFilters')) return;

    const advancedFilters = `
        <div id="advancedFilters" class="advanced-filters">
            <div class="filter-group">
                <label for="statusFilter">Status:</label>
                <select id="statusFilter" onchange="applyFilters()">
                    <option value="">Todos os status</option>
                    <option value="novo">Novo</option>
                    <option value="em-andamento">Em Andamento</option>
                    <option value="finalizado">Finalizado</option>
                </select>
            </div>
            
            <div class="filter-group">
                <label for="tipoFilter">Tipo:</label>
                <select id="tipoFilter" onchange="applyFilters()">
                    <option value="">Todos os tipos</option>
                    <option value="instalacao">Instalação</option>
                    <option value="agendamento">Agendamento</option>
                    <option value="manutencao">Manutenção</option>
                    <option value="reparo">Reparo</option>
                    <option value="suporte">Suporte</option>
                </select>
            </div>
            
            <div class="filter-group">
                <label for="prioridadeFilter">Prioridade:</label>
                <select id="prioridadeFilter" onchange="applyFilters()">
                    <option value="">Todas as prioridades</option>
                    <option value="baixa">Baixa</option>
                    <option value="media">Média</option>
                    <option value="alta">Alta</option>
                </select>
            </div>
            
            <div class="filter-group">
                <label for="ordenacaoFilter">Ordenar por:</label>
                <select id="ordenacaoFilter" onchange="applyFilters()">
                    <option value="data-desc">Data (mais recente)</option>
                    <option value="data-asc">Data (mais antigo)</option>
                    <option value="prioridade-desc">Prioridade (alta)</option>
                    <option value="prioridade-asc">Prioridade (baixa)</option>
                    <option value="valor-desc">Valor (maior)</option>
                    <option value="valor-asc">Valor (menor)</option>
                </select>
            </div>
            
            <div class="filter-actions">
                <button class="btn-secondary" onclick="clearFilters()">
                    <i class="fas fa-times"></i> Limpar Filtros
                </button>
                <button class="btn-primary" onclick="exportChamados()">
                    <i class="fas fa-download"></i> Exportar
                </button>
            </div>
        </div>
    `;

    filtersSection.insertAdjacentHTML('beforeend', advancedFilters);
}

// Adicionar estatísticas de chamados
function addChamadosStats() {
    const chamadosSection = document.querySelector('.chamados-section');
    if (!chamadosSection || document.getElementById('chamadosStats')) return;

    const stats = calculateChamadosStats();

    const statsHTML = `
        <div id="chamadosStats" class="chamados-stats">
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-clipboard-list"></i>
                </div>
                <div class="stat-content">
                    <span class="stat-number">${stats.total}</span>
                    <span class="stat-label">Total de Chamados</span>
                </div>
            </div>
            
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-exclamation-circle"></i>
                </div>
                <div class="stat-content">
                    <span class="stat-number">${stats.novos}</span>
                    <span class="stat-label">Novos</span>
                </div>
            </div>
            
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-cog"></i>
                </div>
                <div class="stat-content">
                    <span class="stat-number">${stats.andamento}</span>
                    <span class="stat-label">Em Andamento</span>
                </div>
            </div>
            
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-check-circle"></i>
                </div>
                <div class="stat-content">
                    <span class="stat-number">${stats.finalizados}</span>
                    <span class="stat-label">Finalizados</span>
                </div>
            </div>
            
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-dollar-sign"></i>
                </div>
                <div class="stat-content">
                    <span class="stat-number">${formatCurrency(stats.receitaTotal)}</span>
                    <span class="stat-label">Receita Total</span>
                </div>
            </div>
        </div>
    `;

    chamadosSection.insertAdjacentHTML('afterbegin', statsHTML);
}

// Calcular estatísticas de chamados
function calculateChamadosStats() {
    const total = chamadosData.length;
    const novos = chamadosData.filter(c => c.status === 'novo').length;
    const andamento = chamadosData.filter(c => c.status === 'em-andamento').length;
    const finalizados = chamadosData.filter(c => c.status === 'finalizado').length;
    const receitaTotal = chamadosData.reduce((total, c) => total + c.valor, 0);

    return { total, novos, andamento, finalizados, receitaTotal };
}

// Configurar busca em tempo real
function setupRealTimeSearch() {
    const searchInput = document.getElementById('searchChamado');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function () {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                activeFilters.busca = this.value;
                applyFilters();
            }, 300);
        });
    }
}

// Configurar event listeners
function setupEventListeners() {
    // Filtros
    const statusFilter = document.getElementById('statusFilter');
    const tipoFilter = document.getElementById('tipoFilter');
    const prioridadeFilter = document.getElementById('prioridadeFilter');
    const ordenacaoFilter = document.getElementById('ordenacaoFilter');

    if (statusFilter) statusFilter.addEventListener('change', applyFilters);
    if (tipoFilter) tipoFilter.addEventListener('change', applyFilters);
    if (prioridadeFilter) prioridadeFilter.addEventListener('change', applyFilters);
    if (ordenacaoFilter) ordenacaoFilter.addEventListener('change', applyFilters);
}

// Carregar lista de chamados
function loadChamados() {
    const chamadosList = document.getElementById('chamadosList');
    if (!chamadosList) return;

    // Aplicar filtros
    const filteredChamados = applyChamadosFilters();

    if (filteredChamados.length === 0) {
        chamadosList.innerHTML = `
            <div class="no-chamados">
                <i class="fas fa-search"></i>
                <h3>Nenhum chamado encontrado</h3>
                <p>Tente ajustar os filtros ou aguarde novos chamados</p>
            </div>
        `;
        return;
    }

    chamadosList.innerHTML = filteredChamados.map(chamado => `
        <div class="chamado-item ${getPrioridadeClass(chamado.prioridade)}" onclick="showChamadoDetails(${chamado.id})">
            <div class="chamado-header">
                <div>
                    <div class="chamado-title">${chamado.nome}</div>
                    <div class="chamado-meta">
                        <span class="chamado-status status-${chamado.status}">${getStatusText(chamado.status)}</span>
                        <span class="chamado-prioridade prioridade-${chamado.prioridade}">${getPrioridadeText(chamado.prioridade)}</span>
                    </div>
                </div>
                <div class="chamado-date">
                    <i class="fas fa-calendar"></i>
                    ${formatDate(chamado.data)}
                </div>
            </div>
            
            <div class="chamado-info">
                <div class="chamado-detail">
                    <i class="fas fa-tag"></i>
                    <strong>Tipo:</strong> ${getTipoText(chamado.tipo)}
                </div>
                <div class="chamado-detail">
                    <i class="fas fa-box"></i>
                    <strong>Equipamento:</strong> ${chamado.tipoExtintor}
                </div>
                <div class="chamado-detail">
                    <i class="fas fa-hashtag"></i>
                    <strong>Quantidade:</strong> ${chamado.quantidade}
                </div>
                <div class="chamado-detail">
                    <i class="fas fa-phone"></i>
                    <strong>Contato:</strong> ${chamado.telefone}
                </div>
                <div class="chamado-detail">
                    <i class="fas fa-dollar-sign"></i>
                    <strong>Valor:</strong> ${formatCurrency(chamado.valor)}
                </div>
                ${chamado.tecnico ? `
                <div class="chamado-detail">
                    <i class="fas fa-user-tie"></i>
                    <strong>Técnico:</strong> ${chamado.tecnico}
                </div>
                ` : ''}
            </div>
            
            <div class="chamado-actions">
                <button class="btn-view btn-sm" onclick="event.stopPropagation(); showChamadoDetails(${chamado.id})" title="Ver Detalhes">
                    <i class="fas fa-eye"></i>
                </button>
                ${chamado.status === 'novo' ? `
                <button class="btn-success btn-sm" onclick="event.stopPropagation(); marcarEmAndamento(${chamado.id})" title="Marcar em Andamento">
                    <i class="fas fa-play"></i>
                </button>
                ` : ''}
                ${chamado.status === 'em-andamento' ? `
                <button class="btn-warning btn-sm" onclick="event.stopPropagation(); finalizarChamado(${chamado.id})" title="Finalizar">
                    <i class="fas fa-check"></i>
                </button>
                ` : ''}
                <button class="btn-primary btn-sm" onclick="event.stopPropagation(); editarChamado(${chamado.id})" title="Editar">
                    <i class="fas fa-edit"></i>
                </button>
            </div>
        </div>
    `).join('');
}

// Aplicar filtros aos chamados
function applyChamadosFilters() {
    let filtered = [...chamadosData];

    // Filtro por status
    if (activeFilters.status) {
        filtered = filtered.filter(c => c.status === activeFilters.status);
    }

    // Filtro por tipo
    if (activeFilters.tipo) {
        filtered = filtered.filter(c => c.tipo === activeFilters.tipo);
    }

    // Filtro por prioridade
    if (activeFilters.prioridade) {
        filtered = filtered.filter(c => c.prioridade === activeFilters.prioridade);
    }

    // Filtro por busca
    if (activeFilters.busca) {
        const searchTerm = activeFilters.busca.toLowerCase();
        filtered = filtered.filter(c =>
            c.nome.toLowerCase().includes(searchTerm) ||
            c.observacoes.toLowerCase().includes(searchTerm) ||
            c.telefone.includes(searchTerm) ||
            c.email.toLowerCase().includes(searchTerm) ||
            c.endereco.toLowerCase().includes(searchTerm)
        );
    }

    // Ordenação
    switch (activeFilters.ordenacao) {
        case 'data-desc':
            filtered.sort((a, b) => new Date(b.data) - new Date(a.data));
            break;
        case 'data-asc':
            filtered.sort((a, b) => new Date(a.data) - new Date(b.data));
            break;
        case 'prioridade-desc':
            const prioridadeOrder = { 'alta': 3, 'media': 2, 'baixa': 1 };
            filtered.sort((a, b) => prioridadeOrder[b.prioridade] - prioridadeOrder[a.prioridade]);
            break;
        case 'prioridade-asc':
            const prioridadeOrderAsc = { 'alta': 3, 'media': 2, 'baixa': 1 };
            filtered.sort((a, b) => prioridadeOrderAsc[a.prioridade] - prioridadeOrderAsc[b.prioridade]);
            break;
        case 'valor-desc':
            filtered.sort((a, b) => b.valor - a.valor);
            break;
        case 'valor-asc':
            filtered.sort((a, b) => a.valor - b.valor);
            break;
    }

    return filtered;
}

// Aplicar filtros (função chamada pelos selects)
function applyFilters() {
    // Atualizar filtros ativos
    const statusFilter = document.getElementById('statusFilter');
    const tipoFilter = document.getElementById('tipoFilter');
    const prioridadeFilter = document.getElementById('prioridadeFilter');
    const ordenacaoFilter = document.getElementById('ordenacaoFilter');

    if (statusFilter) activeFilters.status = statusFilter.value;
    if (tipoFilter) activeFilters.tipo = tipoFilter.value;
    if (prioridadeFilter) activeFilters.prioridade = prioridadeFilter.value;
    if (ordenacaoFilter) activeFilters.ordenacao = ordenacaoFilter.value;

    // Recarregar chamados
    loadChamados();

    // Atualizar estatísticas
    updateChamadosStats();
}

// Limpar filtros
function clearFilters() {
    activeFilters = {
        status: '',
        tipo: '',
        prioridade: '',
        busca: '',
        ordenacao: 'data-desc'
    };

    // Limpar campos
    const statusFilter = document.getElementById('statusFilter');
    const tipoFilter = document.getElementById('tipoFilter');
    const prioridadeFilter = document.getElementById('prioridadeFilter');
    const ordenacaoFilter = document.getElementById('ordenacaoFilter');
    const searchInput = document.getElementById('searchChamado');

    if (statusFilter) statusFilter.value = '';
    if (tipoFilter) tipoFilter.value = '';
    if (prioridadeFilter) prioridadeFilter.value = '';
    if (ordenacaoFilter) ordenacaoFilter.value = 'data-desc';
    if (searchInput) searchInput.value = '';

    loadChamados();
    updateChamadosStats();
}

// Atualizar estatísticas de chamados
function updateChamadosStats() {
    const stats = calculateChamadosStats();
    const filteredStats = calculateFilteredChamadosStats();

    // Atualizar números se os elementos existirem
    const totalElement = document.querySelector('#chamadosStats .stat-item:nth-child(1) .stat-number');
    const novosElement = document.querySelector('#chamadosStats .stat-item:nth-child(2) .stat-number');
    const andamentoElement = document.querySelector('#chamadosStats .stat-item:nth-child(3) .stat-number');
    const finalizadosElement = document.querySelector('#chamadosStats .stat-item:nth-child(4) .stat-number');
    const receitaElement = document.querySelector('#chamadosStats .stat-item:nth-child(5) .stat-number');

    if (totalElement) totalElement.textContent = filteredStats.total;
    if (novosElement) novosElement.textContent = filteredStats.novos;
    if (andamentoElement) andamentoElement.textContent = filteredStats.andamento;
    if (finalizadosElement) finalizadosElement.textContent = filteredStats.finalizados;
    if (receitaElement) receitaElement.textContent = formatCurrency(filteredStats.receitaTotal);
}

// Calcular estatísticas dos chamados filtrados
function calculateFilteredChamadosStats() {
    const filtered = applyChamadosFilters();
    const total = filtered.length;
    const novos = filtered.filter(c => c.status === 'novo').length;
    const andamento = filtered.filter(c => c.status === 'em-andamento').length;
    const finalizados = filtered.filter(c => c.status === 'finalizado').length;
    const receitaTotal = filtered.reduce((total, c) => total + c.valor, 0);

    return { total, novos, andamento, finalizados, receitaTotal };
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
function marcarEmAndamento(id) {
    const chamado = chamadosData.find(c => c.id === id);
    if (!chamado) return;

    chamado.status = 'em-andamento';
    chamado.dataInicio = new Date().toISOString().split('T')[0];
    chamado.tecnico = 'Técnico Padrão'; // Em um sistema real, isso viria de um select

    // Adicionar ao histórico
    chamado.historico.push({
        data: new Date().toISOString().split('T')[0],
        acao: 'Status alterado para: Em Andamento',
        usuario: 'Admin'
    });

    loadChamados();
    updateStats();
    showNotification(`Chamado #${id} marcado como "Em Andamento"`, 'success');
}

function finalizarChamado(id) {
    const chamado = chamadosData.find(c => c.id === id);
    if (!chamado) return;

    chamado.status = 'finalizado';
    chamado.dataFim = new Date().toISOString().split('T')[0];

    // Adicionar ao histórico
    chamado.historico.push({
        data: new Date().toISOString().split('T')[0],
        acao: 'Serviço finalizado',
        usuario: chamado.tecnico || 'Admin'
    });

    chamado.historico.push({
        data: new Date().toISOString().split('T')[0],
        acao: 'Status alterado para: Finalizado',
        usuario: 'Admin'
    });

    loadChamados();
    updateStats();
    showNotification(`Chamado #${id} finalizado com sucesso!`, 'success');
}

function editarChamado(id) {
    const chamado = chamadosData.find(c => c.id === id);
    if (!chamado) return;

    showNotification('Funcionalidade de edição será implementada', 'info');
    // Aqui você pode implementar um modal de edição
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

function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(value);
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

function getPrioridadeText(prioridade) {
    const prioridadeMap = {
        'baixa': 'Baixa',
        'media': 'Média',
        'alta': 'Alta'
    };
    return prioridadeMap[prioridade] || prioridade;
}

function getPrioridadeClass(prioridade) {
    return `prioridade-${prioridade}`;
}

// Notificação simples
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
