// ===== FILTRO DE CATEGORIAS - HOME INDEX =====

// Garante que os produtos sejam exibidos imediatamente, mesmo antes do DOMContentLoaded
(function() {
    // Função para inicializar o filtro
    function initializeFilter() {
        // Inicializa mostrando todas as categorias
        const allSections = document.querySelectorAll('.category-section');
        allSections.forEach(section => {
            section.style.display = 'block';
            section.classList.remove('hidden');
        });

        // Adiciona event listeners a todos os botões de filtro (exceto os desabilitados)
        const filterButtons = document.querySelectorAll('.filter-btn:not(.disabled)');
        if (filterButtons.length > 0) {
            filterButtons.forEach(btn => {
                // Remove listeners anteriores para evitar duplicação
                const newBtn = btn.cloneNode(true);
                btn.parentNode.replaceChild(newBtn, btn);
                
                newBtn.addEventListener('click', function() {
                    // Verifica se o botão não está desabilitado
                    if (!this.classList.contains('disabled')) {
                        const category = this.getAttribute('data-category');
                        filterByCategory(category, this);
                    }
                });
            });
        }
    }

    // Se o DOM já está carregado, inicializa imediatamente
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeFilter);
    } else {
        // DOM já está carregado
        initializeFilter();
    }
})();

function filterByCategory(category, clickedBtn) {
    // Remove a classe active de todos os botões
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.classList.remove('active');
    });

    // Adiciona a classe active ao botão clicado
    if (clickedBtn) {
        clickedBtn.classList.add('active');
    }

    // Obtém todas as seções de categoria
    const allSections = document.querySelectorAll('.category-section');

    if (category === 'all') {
        // Mostra todas as seções
        allSections.forEach(section => {
            section.classList.remove('hidden');
            section.style.display = 'block';
        });
    } else {
        // Esconde todas as seções
        allSections.forEach(section => {
            section.classList.add('hidden');
            section.style.display = 'none';
        });

        // Mostra apenas a seção da categoria selecionada
        const selectedSection = document.querySelector(`.category-section[data-category="${category}"]`);
        if (selectedSection) {
            selectedSection.classList.remove('hidden');
            selectedSection.style.display = 'block';
        }
    }

    // Scroll suave para o topo da seção de produtos
    const productsSection = document.querySelector('.products-section');
    if (productsSection) {
        productsSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}

