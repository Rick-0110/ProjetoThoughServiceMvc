document.addEventListener('DOMContentLoaded', () => {
    setupAddressSelection();
    setupShippingSelection();
    setupPaymentTabs();
    setupCouponBehavior();
    setupConfirmButton();
});

function setupAddressSelection() {
    const addressList = document.getElementById('addressList');
    if (!addressList) return;

    addressList.addEventListener('click', (event) => {
        const card = event.target.closest('.address-card');
        if (!card) return;

        document.querySelectorAll('.address-card').forEach((address) => {
            address.classList.remove('is-selected');
        });
        card.classList.add('is-selected');
    });
}

function setupShippingSelection() {
    const cards = document.querySelectorAll('.shipping-card');
    const summaryShipping = document.getElementById('summaryShipping');
    const summaryTotal = document.getElementById('summaryTotal');

    if (!cards.length) return;

    cards.forEach((card) => {
        card.addEventListener('click', () => {
            cards.forEach((other) => other.classList.remove('is-selected'));
            card.classList.add('is-selected');

            const radio = card.querySelector('input[type="radio"]');
            if (radio) {
                radio.checked = true;
            }

            const shippingPrice = card.querySelector('.shipping-price')?.textContent ?? 'R$ 0,00';
            if (summaryShipping) {
                summaryShipping.textContent = shippingPrice;
            }

            if (summaryTotal) {
                summaryTotal.textContent = calculateTotal(shippingPrice);
            }
        });
    });
}

function calculateTotal(shippingText) {
    const baseTotal = 1550; // Subtotal mockado em reais
    const shippingValue = parseCurrency(shippingText);
    const discountValue = parseCurrency(document.getElementById('summaryDiscounts')?.textContent ?? '0');

    const total = baseTotal + shippingValue - discountValue;
    return formatCurrency(total);
}

function parseCurrency(value) {
    if (!value) return 0;
    return Number(
        value
            .replace(/[R$\s]/g, '')
            .replace(/\./g, '')
            .replace(',', '.')
    ) || 0;
}

function formatCurrency(value) {
    return value.toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });
}

function setupPaymentTabs() {
    const tabs = document.querySelectorAll('.payment-tab');
    const panels = document.querySelectorAll('[data-payment-panel]');
    if (!tabs.length) return;

    tabs.forEach((tab) => {
        tab.addEventListener('click', () => {
            const payment = tab.dataset.payment;

            tabs.forEach((other) => other.classList.remove('is-active'));
            tab.classList.add('is-active');

            panels.forEach((panel) => {
                panel.classList.toggle('is-visible', panel.dataset.paymentPanel === payment);
            });
        });
    });
}

function setupCouponBehavior() {
    const couponInput = document.getElementById('couponInput');
    const applyButton = document.getElementById('applyCouponButton');
    const summaryDiscounts = document.getElementById('summaryDiscounts');
    const summaryTotal = document.getElementById('summaryTotal');

    if (!couponInput || !applyButton || !summaryDiscounts) return;

    applyButton.addEventListener('click', () => {
        const code = couponInput.value.trim().toUpperCase();
        if (!code) {
            showCheckoutToast('Digite um cupom antes de aplicar.', 'warning');
            return;
        }

        let discount = 0;
        if (code === 'TGS10') {
            discount = 155;
        } else if (code === 'FRETEGRATIS') {
            discount = parseCurrency(document.getElementById('summaryShipping')?.textContent ?? '0');
        } else {
            showCheckoutToast('Cupom inválido ou expirado.', 'error');
            summaryDiscounts.textContent = '- R$ 0,00';
            summaryTotal.textContent = calculateTotal(document.getElementById('summaryShipping')?.textContent ?? 'R$ 0,00');
            return;
        }

        summaryDiscounts.textContent = `- ${formatCurrency(discount)}`;
        summaryTotal.textContent = calculateTotal(document.getElementById('summaryShipping')?.textContent ?? 'R$ 0,00');
        showCheckoutToast('Cupom aplicado com sucesso!', 'success');
    });
}

function setupConfirmButton() {
    const confirmButton = document.getElementById('confirmOrderButton');
    if (!confirmButton) return;

    confirmButton.addEventListener('click', () => {
        confirmButton.disabled = true;
        confirmButton.classList.add('is-loading');
        confirmButton.textContent = 'Processando...';

        // Apenas simulação UX; backend cuidará da real submissão
        setTimeout(() => {
            confirmButton.disabled = false;
            confirmButton.classList.remove('is-loading');
            confirmButton.innerHTML = 'Confirmar pedido <i class="fas fa-lock"></i>';
            showCheckoutToast('Pedido pronto para ser enviado ao backend.', 'info');
        }, 1200);
    });
}

function showCheckoutToast(message, type = 'info') {
    const existing = document.querySelector('.checkout-toast');
    if (existing) existing.remove();

    const toast = document.createElement('div');
    toast.className = `checkout-toast checkout-toast--${type}`;
    toast.textContent = message;
    document.body.appendChild(toast);

    setTimeout(() => {
        toast.classList.add('is-visible');
    }, 10);

    setTimeout(() => {
        toast.classList.remove('is-visible');
        setTimeout(() => toast.remove(), 300);
    }, 2500);
}

