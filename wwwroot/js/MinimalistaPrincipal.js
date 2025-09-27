/* ===== INTERATIVIDADE MINIMALISTA - TOUGH SERVICE ===== */

document.addEventListener('DOMContentLoaded', function () {
    // Elementos principais
    const mobileMenuBtn = document.querySelector('.mobile-menu-btn');
    const navMenu = document.querySelector('.nav-menu');
    const searchInput = document.querySelector('.search-input-minimal');
    const productCards = document.querySelectorAll('.product-card-minimal');

    // Inicializar funcionalidades
    initializeMinimalFeatures();

    function initializeMinimalFeatures() {
        setupMobileMenu();
        setupSearchHighlight();
        setupProductAnimations();
        setupSmoothScrolling();
    }

    // ===== MENU MOBILE =====
    function setupMobileMenu() {
        if (!mobileMenuBtn || !navMenu) return;

        mobileMenuBtn.addEventListener('click', function () {
            navMenu.classList.toggle('active');
            mobileMenuBtn.classList.toggle('active');
            document.body.classList.toggle('menu-open');
        });

        // Fechar menu ao clicar em um link
        const navLinks = navMenu.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.addEventListener('click', function () {
                navMenu.classList.remove('active');
                mobileMenuBtn.classList.remove('active');
                document.body.classList.remove('menu-open');
            });
        });

        // Fechar menu ao clicar fora
        document.addEventListener('click', function (e) {
            if (!navMenu.contains(e.target) && !mobileMenuBtn.contains(e.target)) {
                navMenu.classList.remove('active');
                mobileMenuBtn.classList.remove('active');
                document.body.classList.remove('menu-open');
            }
        });
    }

    // ===== BUSCA COM HIGHLIGHT =====
    function setupSearchHighlight() {
        if (!searchInput) return;

        searchInput.addEventListener('input', function () {
            const query = this.value.toLowerCase().trim();

            if (query.length > 2) {
                highlightProducts(query);
            } else {
                clearHighlights();
            }
        });
    }

    function highlightProducts(query) {
        productCards.forEach(card => {
            const productName = card.querySelector('.product-name');

            if (productName && productName.textContent.toLowerCase().includes(query)) {
                card.style.border = '2px solid var(--primary-red)';
                card.style.transform = 'scale(1.02)';
                card.style.boxShadow = 'var(--shadow-lg)';
            } else {
                card.style.border = 'none';
                card.style.transform = 'scale(1)';
                card.style.boxShadow = 'var(--shadow)';
            }
        });
    }

    function clearHighlights() {
        productCards.forEach(card => {
            card.style.border = 'none';
            card.style.transform = 'scale(1)';
            card.style.boxShadow = 'var(--shadow)';
        });
    }

    // ===== ANIMAÇÕES DOS PRODUTOS =====
    function setupProductAnimations() {
        // Observer para animações de entrada
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }
            });
        }, observerOptions);

        productCards.forEach((card, index) => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(20px)';
            card.style.transition = 'all 0.6s ease-out';
            card.style.transitionDelay = `${index * 0.1}s`;

            observer.observe(card);
        });

        // Efeito hover nos cards
        productCards.forEach(card => {
            card.addEventListener('mouseenter', function () {
                this.style.transform = 'translateY(-8px)';
            });

            card.addEventListener('mouseleave', function () {
                this.style.transform = 'translateY(0)';
            });
        });
    }

    // ===== SCROLL SUAVE =====
    function setupSmoothScrolling() {
        const links = document.querySelectorAll('a[href^="#"]');

        links.forEach(link => {
            link.addEventListener('click', function (e) {
                e.preventDefault();

                const targetId = this.getAttribute('href');
                const targetElement = document.querySelector(targetId);

                if (targetElement) {
                    targetElement.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }

    // ===== NOTIFICAÇÕES SIMPLES =====
    function showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.innerHTML = `
            <div class="notification-content">
                <span>${message}</span>
                <button class="notification-close">&times;</button>
            </div>
        `;

        document.body.appendChild(notification);

        // Animação de entrada
        setTimeout(() => {
            notification.classList.add('show');
        }, 100);

        // Auto-remover após 3 segundos
        setTimeout(() => {
            removeNotification(notification);
        }, 3000);

        // Botão de fechar
        const closeBtn = notification.querySelector('.notification-close');
        closeBtn.addEventListener('click', () => {
            removeNotification(notification);
        });
    }

    function removeNotification(notification) {
        notification.classList.remove('show');
        setTimeout(() => {
            if (document.body.contains(notification)) {
                document.body.removeChild(notification);
            }
        }, 300);
    }

    // ===== LOADING STATES =====
    function addLoadingState(element, text = 'Carregando...') {
        const originalContent = element.innerHTML;
        element.innerHTML = `
            <span class="loading-spinner"></span>
            ${text}
        `;
        element.disabled = true;

        return () => {
            element.innerHTML = originalContent;
            element.disabled = false;
        };
    }

    // ===== MELHORIAS DE ACESSIBILIDADE =====
    function improveAccessibility() {
        // Adicionar suporte para teclado
        document.addEventListener('keydown', function (e) {
            // ESC para fechar notificações
            if (e.key === 'Escape') {
                const notifications = document.querySelectorAll('.notification');
                notifications.forEach(notification => {
                    removeNotification(notification);
                });
            }
        });

        // Adicionar focus visible
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Tab') {
                document.body.classList.add('keyboard-navigation');
            }
        });

        document.addEventListener('mousedown', function () {
            document.body.classList.remove('keyboard-navigation');
        });
    }

    // Inicializar melhorias de acessibilidade
    improveAccessibility();

    // ===== PERFORMANCE =====
    // Debounce para eventos de scroll
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    // Aplicar debounce aos eventos de scroll
    const debouncedScrollHandler = debounce(() => {
        // Lógica de scroll otimizada se necessário
    }, 100);

    window.addEventListener('scroll', debouncedScrollHandler);
});

// ===== ESTILOS PARA NOTIFICAÇÕES =====
const notificationStyles = document.createElement('style');
notificationStyles.textContent = `
    .notification {
        position: fixed;
        top: 20px;
        right: 20px;
        background: var(--white);
        border-radius: var(--radius-lg);
        box-shadow: var(--shadow-lg);
        padding: var(--space-4);
        z-index: 9999;
        transform: translateX(100%);
        transition: transform 0.3s ease;
        max-width: 400px;
        border-left: 4px solid var(--primary-red);
    }
    
    .notification.show {
        transform: translateX(0);
    }
    
    .notification-success {
        border-left-color: #10b981;
    }
    
    .notification-error {
        border-left-color: #ef4444;
    }
    
    .notification-warning {
        border-left-color: #f59e0b;
    }
    
    .notification-content {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: var(--space-3);
        color: var(--gray-800);
    }
    
    .notification-close {
        background: none;
        border: none;
        color: var(--gray-500);
        cursor: pointer;
        font-size: 1.2rem;
        padding: 0;
        width: 20px;
        height: 20px;
        display: flex;
        align-items: center;
        justify-content: center;
    }
    
    .notification-close:hover {
        color: var(--gray-800);
    }
    
    .loading-spinner {
        display: inline-block;
        width: 16px;
        height: 16px;
        border: 2px solid var(--gray-300);
        border-top: 2px solid var(--primary-red);
        border-radius: 50%;
        animation: spin 1s linear infinite;
        margin-right: var(--space-2);
    }
    
    @keyframes spin {
        0% { transform: rotate(0deg); }
        100% { transform: rotate(360deg); }
    }
    
    .keyboard-navigation *:focus {
        outline: 2px solid var(--primary-red) !important;
        outline-offset: 2px !important;
    }
    
    /* Menu mobile ativo */
    @media (max-width: 768px) {
        .nav-menu.active {
            display: flex;
            position: absolute;
            top: 100%;
            left: 0;
            right: 0;
            background: var(--white);
            border-top: 1px solid var(--gray-200);
            flex-direction: column;
            padding: var(--space-4);
            box-shadow: var(--shadow-lg);
        }
        
        .mobile-menu-btn.active span:nth-child(1) {
            transform: rotate(45deg) translate(5px, 5px);
        }
        
        .mobile-menu-btn.active span:nth-child(2) {
            opacity: 0;
        }
        
        .mobile-menu-btn.active span:nth-child(3) {
            transform: rotate(-45deg) translate(7px, -6px);
        }
        
        body.menu-open {
            overflow: hidden;
        }
    }
`;
document.head.appendChild(notificationStyles);
