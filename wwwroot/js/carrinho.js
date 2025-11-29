document.addEventListener('DOMContentLoaded', () => {
    setupCartSelection();
    setupQuantityAjax();
});

function setupCartSelection() {
    const selectAll = document.getElementById('selectAll');
    const itemCheckboxes = Array.from(document.querySelectorAll('.cart-select-item'));
    const selectedField = document.getElementById('selectedProdutoIdsField');

    if (!selectAll || !itemCheckboxes.length || !selectedField) return;

    function updateHiddenField() {
        const selectedIds = itemCheckboxes
            .filter(cb => cb.checked)
            .map(cb => cb.value);
        selectedField.value = selectedIds.join(',');
    }

    function syncSelectAll() {
        const allChecked = itemCheckboxes.length > 0 && itemCheckboxes.every(cb => cb.checked);
        selectAll.checked = allChecked;
    }

    // Inicializa: tudo selecionado
    itemCheckboxes.forEach(cb => { cb.checked = true; });
    selectAll.checked = true;
    updateHiddenField();

    selectAll.addEventListener('change', () => {
        const checked = selectAll.checked;
        itemCheckboxes.forEach(cb => {
            cb.checked = checked;
        });
        updateHiddenField();
    });

    itemCheckboxes.forEach(cb => {
        cb.addEventListener('change', () => {
            syncSelectAll();
            updateHiddenField();
        });
    });
}

// Evita recarregar a página quando altera quantidade e atualiza tudo via JS
function setupQuantityAjax() {
    const forms = document.querySelectorAll('.quantity-form');
    if (!forms.length) return;

    forms.forEach(form => {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            const submitButton = e.submitter || form.querySelector('button[type="submit"]');
            if (!submitButton) return;

            const delta = parseInt(submitButton.value, 10);
            const produtoIdInput = form.querySelector('input[name="produtoId"]');
            if (!produtoIdInput || isNaN(delta)) return;

            const produtoId = produtoIdInput.value;

            // Elementos de UI relacionados
            const quantitySpan = form.querySelector('.quantity-label strong');
            const itemRoot = form.closest('.carrinho-item');
            const badge = itemRoot?.querySelector('.quantity-badge');
            const unitPriceEl = itemRoot?.querySelector('.unit-price');
            const totalPriceEl = itemRoot?.querySelector('.total-price');

            const currentQty = quantitySpan ? parseInt(quantitySpan.textContent || '0', 10) || 0 : 0;
            const newQty = currentQty + delta;

            // Se zerar ou ficar negativo, deixamos o backend remover, mas já ocultamos suavemente
            if (newQty <= 0 && itemRoot) {
                itemRoot.style.opacity = '0.4';
            }

            // Atualiza visualmente de forma otimista
            if (quantitySpan && newQty > 0) {
                quantitySpan.textContent = newQty.toString();
            }
            if (badge && newQty > 0) {
                badge.textContent = newQty.toString();
            }

            // Atualiza preço total do item no front
            if (unitPriceEl && totalPriceEl) {
                const unitText = unitPriceEl.textContent.replace(/[^\d,.-]/g, '').replace('.', '').replace(',', '.');
                const unitValue = parseFloat(unitText) || 0;
                const newTotal = unitValue * Math.max(newQty, 0);
                totalPriceEl.textContent = `R$ ${newTotal.toFixed(2).replace('.', ',')}`;
            }

            // Recalcula subtotal/total do resumo
            recalcResumoTotals();

            // Envia POST para o servidor sem recarregar a página
            try {
                const url = form.getAttribute('action');
                const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
                const token = tokenInput ? tokenInput.value : '';

                const body = new URLSearchParams();
                body.append('produtoId', produtoId);
                body.append('delta', delta.toString());
                if (token) {
                    body.append('__RequestVerificationToken', token);
                }

                await fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
                    },
                    body: body.toString()
                });

                // Se quantidade ficou <= 0, removemos o item da UI após o backend processar
                if (newQty <= 0 && itemRoot) {
                    itemRoot.style.transition = 'opacity 0.2s ease, height 0.2s ease, margin 0.2s ease, padding 0.2s ease';
                    itemRoot.style.opacity = '0';
                    itemRoot.style.height = '0';
                    itemRoot.style.margin = '0';
                    itemRoot.style.padding = '0';
                    setTimeout(() => itemRoot.remove(), 220);
                }

                recalcResumoTotals();
            } catch (err) {
                console.error('Erro ao atualizar quantidade do carrinho', err);
                // Em caso de erro, recarrega a página como fallback
                window.location.reload();
            }
        });
    });
}

function recalcResumoTotals() {
    const itemRows = document.querySelectorAll('.carrinho-item');
    const subtotalEl = document.querySelector('.resumo-item .resumo-value');
    const totalEl = document.querySelector('.resumo-total-value');

    if (!itemRows.length || !subtotalEl || !totalEl) return;

    let subtotal = 0;
    itemRows.forEach(row => {
        const totalPriceEl = row.querySelector('.total-price');
        if (!totalPriceEl) return;
        const text = totalPriceEl.textContent.replace(/[^\d,,-.]/g, '').replace('.', '').replace(',', '.');
        const value = parseFloat(text) || 0;
        subtotal += value;
    });

    subtotalEl.textContent = `R$ ${subtotal.toFixed(2).replace('.', ',')}`;
    totalEl.textContent = `R$ ${subtotal.toFixed(2).replace('.', ',')}`;
}
