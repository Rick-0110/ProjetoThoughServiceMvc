// ===== JAVASCRIPT SIMPLES PARA PRODUTOS =====


// Filtros ativos
let activeFilters = {
    categoria: '',
    estoque: '',
    busca: '',
    ordenacao: 'nome'
};

// Carregar quando a página abrir
document.addEventListener('DOMContentLoaded', function () {
    initializeProductPage();
    loadProducts();
    setupEventListeners();
});

// Inicializar página de produtos
function initializeProductPage() {
    // Adicionar filtros avançados se não existirem
    addAdvancedFilters();

    // Adicionar estatísticas de produtos
    addProductStats();

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
                <label for="categoriaFilter">Categoria:</label>
                <select id="categoriaFilter" onchange="applyFilters()">
                    <option value="">Todas as categorias</option>
                    <option value="extintores">Extintores</option>
                    <option value="sistemas-fixos">Sistemas Fixos</option>
                    <option value="detecao">Sistemas de Detecção</option>
                    <option value="acessorios">Acessórios</option>
                </select>
            </div>
            
            <div class="filter-group">
                <label for="estoqueFilter">Estoque:</label>
                <select id="estoqueFilter" onchange="applyFilters()">
                    <option value="">Todos</option>
                    <option value="baixo">Estoque baixo (< 20)</option>
                    <option value="medio">Estoque médio (20-50)</option>
                    <option value="alto">Estoque alto (> 50)</option>
                    <option value="zerado">Sem estoque</option>
                </select>
            </div>
            
            <div class="filter-group">
                <label for="ordenacaoFilter">Ordenar por:</label>
                <select id="ordenacaoFilter" onchange="applyFilters()">
                    <option value="nome">Nome (A-Z)</option>
                    <option value="preco-asc">Preço (menor)</option>
                    <option value="preco-desc">Preço (maior)</option>
                    <option value="estoque-asc">Estoque (menor)</option>
                    <option value="estoque-desc">Estoque (maior)</option>
                    <option value="vendidos-desc">Mais vendidos</option>
                </select>
            </div>
            
            <div class="filter-actions">
                <button class="btn-secondary" onclick="clearFilters()">
                    <i class="fas fa-times"></i> Limpar Filtros
                </button>
                <button class="btn-primary" onclick="exportProducts()">
                    <i class="fas fa-download"></i> Exportar
                </button>
            </div>
        </div>
    `;

    filtersSection.insertAdjacentHTML('beforeend', advancedFilters);
}

// Adicionar estatísticas de produtos
function addProductStats() {
    const productsSection = document.querySelector('.products-section');
    if (!productsSection || document.getElementById('productStats')) return;

    const stats = calculateProductStats();

    const statsHTML = `
        <div id="productStats" class="product-stats">
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-boxes"></i>
                </div>
                <div class="stat-content">
                    <span class="stat-number">${stats.total}</span>
                    <span class="stat-label">Total de Produtos</span>
                </div>
            </div>
            
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-exclamation-triangle"></i>
                </div>
                <div class="stat-content">
                    <span class="stat-number">${stats.estoqueBaixo}</span>
                    <span class="stat-label">Estoque Baixo</span>
                </div>
            </div>
            
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-dollar-sign"></i>
                </div>
                <div class="stat-content">
                    <span class="stat-number">${formatCurrency(stats.valorTotal)}</span>
                    <span class="stat-label">Valor Total</span>
                </div>
            </div>
            
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-chart-line"></i>
                </div>
                <div class="stat-content">
                    <span class="stat-number">${stats.totalVendidos}</span>
                    <span class="stat-label">Total Vendidos</span>
                </div>
            </div>
        </div>
    `;

    productsSection.insertAdjacentHTML('afterbegin', statsHTML);
}

// Calcular estatísticas de produtos
function calculateProductStats() {
    const total = produtosData.length;
    const estoqueBaixo = produtosData.filter(p => p.quantidade < 20).length;
    const valorTotal = produtosData.reduce((total, p) => total + (p.preco * p.quantidade), 0);
    const totalVendidos = produtosData.reduce((total, p) => total + p.vendidos, 0);

    return { total, estoqueBaixo, valorTotal, totalVendidos };
}

// Configurar busca em tempo real
function setupRealTimeSearch() {
    const searchInput = document.getElementById('searchProduct');
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
    const categoriaFilter = document.getElementById('categoriaFilter');
    const estoqueFilter = document.getElementById('estoqueFilter');
    const ordenacaoFilter = document.getElementById('ordenacaoFilter');

    if (categoriaFilter) categoriaFilter.addEventListener('change', applyFilters);
    if (estoqueFilter) estoqueFilter.addEventListener('change', applyFilters);
    if (ordenacaoFilter) ordenacaoFilter.addEventListener('change', applyFilters);
}

// Navegar entre tabs
function showTab(tabName) {
    // Esconder todas as tabs
    const tabs = document.querySelectorAll('.tab-content');
    tabs.forEach(tab => tab.classList.remove('active'));

    // Remover active de todos os botões
    const buttons = document.querySelectorAll('.tab-btn');
    buttons.forEach(btn => btn.classList.remove('active'));

    // Mostrar tab selecionada
    document.getElementById(`tab-${tabName}`).classList.add('active');
    document.querySelector(`[onclick="showTab('${tabName}')"]`).classList.add('active');

    // Carregar produtos se for a tab de listagem
    if (tabName === 'listar') {
        loadProducts();
    }
}

// Adicionar produto
function submitProduct(event) {
    event.preventDefault();

    const form = event.target;
    const nome = form.nome.value;
    const categoria = form.categoria.value;
    const preco = parseFloat(form.preco.value);
    const quantidade = parseInt(form.quantidade.value);
    const descricao = form.descricao.value;

    // Validar
    if (!nome || !categoria || !preco || !quantidade || !descricao) {
        showNotification('Preencha todos os campos!', 'error');
        return;
    }

    if (preco <= 0) {
        showNotification('Preço deve ser maior que zero!', 'error');
        return;
    }

    if (quantidade < 0) {
        showNotification('Quantidade não pode ser negativa!', 'error');
        return;
    }

    // Criar novo produto
    const novoProduto = {
        id: Date.now(),
        nome: nome,
        categoria: categoria,
        preco: preco,
        quantidade: quantidade,
        descricao: descricao
    };

    produtosData.push(novoProduto);
    form.reset();
    showNotification('Produto adicionado com sucesso!');
}

// Carregar lista de produtos
function loadProducts() {
    const productsGrid = document.getElementById('productsGrid');
    if (!productsGrid) return;

    // Aplicar filtros
    const filteredProducts = applyProductFilters();

    if (filteredProducts.length === 0) {
        productsGrid.innerHTML = `
            <div class="no-products">
                <i class="fas fa-search"></i>
                <h3>Nenhum produto encontrado</h3>
                <p>Tente ajustar os filtros ou adicionar novos produtos</p>
                <button class="btn-primary" onclick="showTab('adicionar')">
                    <i class="fas fa-plus"></i> Adicionar Produto
                </button>
            </div>
        `;
        return;
    }

    productsGrid.innerHTML = filteredProducts.map(produto => `
        <div class="product-card ${produto.quantidade < 20 ? 'low-stock' : ''}">
            <div class="product-header">
                <div>
                    <div class="product-title">${produto.nome}</div>
                    <div class="product-category">${getCategoriaText(produto.categoria)}</div>
                    <div class="product-code">Código: ${produto.codigo}</div>
                </div>
                <div class="product-actions">
                    <button class="btn-view" onclick="viewProduct(${produto.id})" title="Visualizar">
                        <i class="fas fa-eye"></i>
                    </button>
                    <button class="btn-edit" onclick="editProduct(${produto.id})" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn-delete" onclick="deleteProduct(${produto.id})" title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </div>
            
            <div class="product-info">
                <div class="product-price">${formatCurrency(produto.preco)}</div>
                <div class="product-stock ${produto.quantidade < 20 ? 'low-stock' : ''}">
                    <i class="fas fa-boxes"></i>
                    Estoque: ${produto.quantidade} unidades
                    ${produto.quantidade < 20 ? '<span class="stock-warning">Estoque baixo!</span>' : ''}
                </div>
                <div class="product-sales">
                    <i class="fas fa-chart-line"></i>
                    Vendidos: ${produto.vendidos}
                </div>
            </div>
            
            <div class="product-description">${produto.descricao}</div>
            
            <div class="product-footer">
                <div class="product-supplier">
                    <i class="fas fa-truck"></i>
                    ${produto.fornecedor}
                </div>
                <div class="product-date">
                    <i class="fas fa-calendar"></i>
                    ${formatDate(produto.dataCadastro)}
                </div>
            </div>
        </div>
    `).join('');
}

// Aplicar filtros aos produtos
function applyProductFilters() {
    let filtered = [...produtosData];

    // Filtro por categoria
    if (activeFilters.categoria) {
        filtered = filtered.filter(p => p.categoria === activeFilters.categoria);
    }

    // Filtro por estoque
    if (activeFilters.estoque) {
        switch (activeFilters.estoque) {
            case 'baixo':
                filtered = filtered.filter(p => p.quantidade < 20);
                break;
            case 'medio':
                filtered = filtered.filter(p => p.quantidade >= 20 && p.quantidade <= 50);
                break;
            case 'alto':
                filtered = filtered.filter(p => p.quantidade > 50);
                break;
            case 'zerado':
                filtered = filtered.filter(p => p.quantidade === 0);
                break;
        }
    }

    // Filtro por busca
    if (activeFilters.busca) {
        const searchTerm = activeFilters.busca.toLowerCase();
        filtered = filtered.filter(p =>
            p.nome.toLowerCase().includes(searchTerm) ||
            p.descricao.toLowerCase().includes(searchTerm) ||
            p.codigo.toLowerCase().includes(searchTerm) ||
            p.fornecedor.toLowerCase().includes(searchTerm)
        );
    }

    // Ordenação
    switch (activeFilters.ordenacao) {
        case 'nome':
            filtered.sort((a, b) => a.nome.localeCompare(b.nome));
            break;
        case 'preco-asc':
            filtered.sort((a, b) => a.preco - b.preco);
            break;
        case 'preco-desc':
            filtered.sort((a, b) => b.preco - a.preco);
            break;
        case 'estoque-asc':
            filtered.sort((a, b) => a.quantidade - b.quantidade);
            break;
        case 'estoque-desc':
            filtered.sort((a, b) => b.quantidade - a.quantidade);
            break;
        case 'vendidos-desc':
            filtered.sort((a, b) => b.vendidos - a.vendidos);
            break;
    }

    return filtered;
}

// Aplicar filtros (função chamada pelos selects)
function applyFilters() {
    // Atualizar filtros ativos
    const categoriaFilter = document.getElementById('categoriaFilter');
    const estoqueFilter = document.getElementById('estoqueFilter');
    const ordenacaoFilter = document.getElementById('ordenacaoFilter');

    if (categoriaFilter) activeFilters.categoria = categoriaFilter.value;
    if (estoqueFilter) activeFilters.estoque = estoqueFilter.value;
    if (ordenacaoFilter) activeFilters.ordenacao = ordenacaoFilter.value;

    // Recarregar produtos
    loadProducts();

    // Atualizar estatísticas
    updateProductStats();
}

// Limpar filtros
function clearFilters() {
    activeFilters = {
        categoria: '',
        estoque: '',
        busca: '',
        ordenacao: 'nome'
    };

    // Limpar campos
    const categoriaFilter = document.getElementById('categoriaFilter');
    const estoqueFilter = document.getElementById('estoqueFilter');
    const ordenacaoFilter = document.getElementById('ordenacaoFilter');
    const searchInput = document.getElementById('searchProduct');

    if (categoriaFilter) categoriaFilter.value = '';
    if (estoqueFilter) estoqueFilter.value = '';
    if (ordenacaoFilter) ordenacaoFilter.value = 'nome';
    if (searchInput) searchInput.value = '';

    loadProducts();
    updateProductStats();
}

// Atualizar estatísticas de produtos
function updateProductStats() {
    const stats = calculateProductStats();
    const filteredStats = calculateFilteredStats();

    // Atualizar números se os elementos existirem
    const totalElement = document.querySelector('#productStats .stat-item:nth-child(1) .stat-number');
    const estoqueBaixoElement = document.querySelector('#productStats .stat-item:nth-child(2) .stat-number');
    const valorTotalElement = document.querySelector('#productStats .stat-item:nth-child(3) .stat-number');
    const totalVendidosElement = document.querySelector('#productStats .stat-item:nth-child(4) .stat-number');

    if (totalElement) totalElement.textContent = filteredStats.total;
    if (estoqueBaixoElement) estoqueBaixoElement.textContent = filteredStats.estoqueBaixo;
    if (valorTotalElement) valorTotalElement.textContent = formatCurrency(filteredStats.valorTotal);
    if (totalVendidosElement) totalVendidosElement.textContent = filteredStats.totalVendidos;
}

// Calcular estatísticas dos produtos filtrados
function calculateFilteredStats() {
    const filtered = applyProductFilters();
    const total = filtered.length;
    const estoqueBaixo = filtered.filter(p => p.quantidade < 20).length;
    const valorTotal = filtered.reduce((total, p) => total + (p.preco * p.quantidade), 0);
    const totalVendidos = filtered.reduce((total, p) => total + p.vendidos, 0);

    return { total, estoqueBaixo, valorTotal, totalVendidos };
}

// Editar produto
function editProduct(id) {
    const produto = produtosData.find(p => p.id === id);
    if (!produto) return;

    // Preencher formulário
    document.getElementById('editId').value = produto.id;
    document.getElementById('editNome').value = produto.nome;
    document.getElementById('editCategoria').value = produto.categoria;
    document.getElementById('editPreco').value = produto.preco;
    document.getElementById('editQuantidade').value = produto.quantidade;
    document.getElementById('editDescricao').value = produto.descricao;

    // Mostrar modal
    document.getElementById('modalEditar').style.display = 'block';
    document.body.style.overflow = 'hidden';
}

// Atualizar produto
function updateProduct(event) {
    event.preventDefault();

    const form = event.target;
    const id = parseInt(form.id.value);
    const nome = form.nome.value;
    const categoria = form.categoria.value;
    const preco = parseFloat(form.preco.value);
    const quantidade = parseInt(form.quantidade.value);
    const descricao = form.descricao.value;

    // Validar
    if (!nome || !categoria || !preco || !quantidade || !descricao) {
        showNotification('Preencha todos os campos!', 'error');
        return;
    }

    if (preco <= 0) {
        showNotification('Preço deve ser maior que zero!', 'error');
        return;
    }

    if (quantidade < 0) {
        showNotification('Quantidade não pode ser negativa!', 'error');
        return;
    }

    // Atualizar produto
    const produtoIndex = produtosData.findIndex(p => p.id === id);
    if (produtoIndex !== -1) {
        produtosData[produtoIndex] = {
            ...produtosData[produtoIndex],
            nome: nome,
            categoria: categoria,
            preco: preco,
            quantidade: quantidade,
            descricao: descricao
        };

        loadProducts();
        closeEditModal();
        showNotification('Produto atualizado com sucesso!');
    }
}

// Excluir produto
function deleteProduct(id) {
    if (!confirm('Tem certeza que deseja excluir este produto?')) {
        return;
    }

    const produtoIndex = produtosData.findIndex(p => p.id === id);
    if (produtoIndex !== -1) {
        const produto = produtosData[produtoIndex];
        produtosData.splice(produtoIndex, 1);
        loadProducts();
        showNotification(`Produto "${produto.nome}" excluído!`);
    }
}

// Fechar modal
function closeEditModal() {
    document.getElementById('modalEditar').style.display = 'none';
    document.body.style.overflow = 'auto';
}

// Limpar formulário
function resetForm() {
    document.getElementById('form-adicionar-produto').reset();
}

// Visualizar produto
function viewProduct(id) {
    const produto = produtosData.find(p => p.id === id);
    if (!produto) return;

    const modal = document.getElementById('modalVisualizar');
    if (!modal) {
        createViewModal();
    }

    const modalContent = document.getElementById('produtoDetails');
    modalContent.innerHTML = `
        <div class="product-view-grid">
            <div class="product-view-section">
                <h3>Informações Básicas</h3>
                <div class="detail-item">
                    <strong>Nome:</strong> ${produto.nome}
                </div>
                <div class="detail-item">
                    <strong>Código:</strong> ${produto.codigo}
                </div>
                <div class="detail-item">
                    <strong>Categoria:</strong> ${getCategoriaText(produto.categoria)}
                </div>
                <div class="detail-item">
                    <strong>Preço:</strong> ${formatCurrency(produto.preco)}
                </div>
            </div>
            
            <div class="product-view-section">
                <h3>Estoque e Vendas</h3>
                <div class="detail-item">
                    <strong>Quantidade em Estoque:</strong> ${produto.quantidade} unidades
                </div>
                <div class="detail-item">
                    <strong>Total Vendidos:</strong> ${produto.vendidos} unidades
                </div>
                <div class="detail-item">
                    <strong>Valor Total em Estoque:</strong> ${formatCurrency(produto.preco * produto.quantidade)}
                </div>
                <div class="detail-item">
                    <strong>Receita Total:</strong> ${formatCurrency(produto.preco * produto.vendidos)}
                </div>
            </div>
            
            <div class="product-view-section">
                <h3>Fornecedor e Cadastro</h3>
                <div class="detail-item">
                    <strong>Fornecedor:</strong> ${produto.fornecedor}
                </div>
                <div class="detail-item">
                    <strong>Data de Cadastro:</strong> ${formatDate(produto.dataCadastro)}
                </div>
                <div class="detail-item">
                    <strong>Status:</strong> 
                    <span class="status-badge ${produto.ativo ? 'active' : 'inactive'}">
                        ${produto.ativo ? 'Ativo' : 'Inativo'}
                    </span>
                </div>
            </div>
            
            <div class="product-view-section full-width">
                <h3>Descrição</h3>
                <div class="product-description-full">${produto.descricao}</div>
            </div>
        </div>
    `;

    modal.style.display = 'block';
    document.body.style.overflow = 'hidden';
}

// Criar modal de visualização
function createViewModal() {
    const modalHTML = `
        <div id="modalVisualizar" class="modal">
            <div class="modal-content modal-large">
                <div class="modal-header">
                    <h2><i class="fas fa-eye"></i> Visualizar Produto</h2>
                    <span class="close" onclick="closeViewModal()">&times;</span>
                </div>
                <div class="modal-body">
                    <div id="produtoDetails"></div>
                </div>
                <div class="modal-footer">
                    <button class="btn-secondary" onclick="closeViewModal()">
                        <i class="fas fa-times"></i> Fechar
                    </button>
                    <button class="btn-primary" onclick="editProductFromView()">
                        <i class="fas fa-edit"></i> Editar Produto
                    </button>
                </div>
            </div>
        </div>
    `;

    document.body.insertAdjacentHTML('beforeend', modalHTML);
}

// Fechar modal de visualização
function closeViewModal() {
    const modal = document.getElementById('modalVisualizar');
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
    }
}

// Editar produto a partir da visualização
function editProductFromView() {
    closeViewModal();
    // Aqui você pode implementar a lógica para editar o produto
    showNotification('Funcionalidade de edição será implementada', 'info');
}

// Exportar produtos
function exportProducts() {
    const filteredProducts = applyProductFilters();

    const data = {
        produtos: filteredProducts,
        filtros: activeFilters,
        estatisticas: calculateFilteredStats(),
        exportadoEm: new Date().toISOString(),
        totalRegistros: filteredProducts.length
    };

    const dataStr = JSON.stringify(data, null, 2);
    const dataBlob = new Blob([dataStr], { type: 'application/json' });

    const link = document.createElement('a');
    link.href = URL.createObjectURL(dataBlob);
    link.download = `produtos-${new Date().toISOString().split('T')[0]}.json`;
    link.click();

    showNotification(`Exportados ${filteredProducts.length} produtos com sucesso!`);
}

// Buscar produtos (função legada - mantida para compatibilidade)
function searchProducts() {
    const searchTerm = document.getElementById('searchProduct').value.toLowerCase();
    activeFilters.busca = searchTerm;
    applyFilters();
}

// Funções auxiliares
function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(value);
}

function getCategoriaText(categoria) {
    const categoriaMap = {
        'extintores': 'Extintores',
        'sistemas-fixos': 'Sistemas Fixos',
        'detecao': 'Sistemas de Detecção',
        'acessorios': 'Acessórios'
    };
    return categoriaMap[categoria] || categoria;
}

// Notificação simples
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.innerHTML = `
        <div style="display: flex; align-items: center; gap: 0.5rem;">
            <i class="fas fa-${type === 'error' ? 'exclamation-circle' : 'check-circle'}"></i>
            <span>${message}</span>
        </div>
    `;

    const bgColor = type === 'error' ? '#dc3545' : '#28a745';

    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${bgColor};
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

// Formatar data
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
}

// Fechar modal com ESC
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closeEditModal();
        closeViewModal();
    }
});

// Fechar modal clicando fora
window.addEventListener('click', function (event) {
    const modalEditar = document.getElementById('modalEditar');
    const modalVisualizar = document.getElementById('modalVisualizar');

    if (event.target === modalEditar) {
        closeEditModal();
    }

    if (event.target === modalVisualizar) {
        closeViewModal();
    }
});
