document.addEventListener('DOMContentLoaded', () => {
    // Inicialização de todas as funcionalidades de Front-end
    setupAddressSelection();
    setupShippingSelection();
    setupPaymentTabs();
    setupCouponBehavior();
    setupMasks(); // Aplica as máscaras de input
    setupConfirmButton(); // A função que envia os dados para o C#

    // Calcula o total inicial ao carregar a página
    // É crucial que o C# recalcule, mas o JS mantém a visualização correta
    updateSummaryTotals();
});

// =========================================================================
// FUNÇÕES DE INTERAÇÃO VISUAL
// =========================================================================

function setupAddressSelection() {
    const addressList = document.getElementById('addressList');
    if (!addressList) return;

    addressList.addEventListener('click', (event) => {
        const card = event.target.closest('.address-card');
        if (!card) return;

        // Remove a seleção de todos os cartões e adiciona ao clicado
        document.querySelectorAll('.address-card').forEach((address) => {
            address.classList.remove('is-selected');
        });
        card.classList.add('is-selected');

        const selectedAddressField = document.getElementById('selectedAddressId');
        if (selectedAddressField) {
            selectedAddressField.value = card.dataset.addressId || '';
        }

        // Em um sistema real, aqui haveria uma chamada AJAX para calcular o frete para o novo CEP.
    });
}

function setupShippingSelection() {
    const cards = document.querySelectorAll('.shipping-card');

    if (!cards.length) return;

    cards.forEach((card) => {
        card.addEventListener('click', () => {
            cards.forEach((other) => other.classList.remove('is-selected'));
            card.classList.add('is-selected');

            const radio = card.querySelector('input[type="radio"]');
            if (radio) {
                radio.checked = true;
            }

            const hiddenShipping = document.getElementById('hiddenShipping');
            if (hiddenShipping) {
                hiddenShipping.value = card.getAttribute('data-shipping-price') || '0';
            }

            // Atualiza o resumo visualmente
            updateSummaryTotals();
        });
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

            const paymentField = document.getElementById('paymentMethodField');
            if (paymentField) {
                paymentField.value = payment;
            }
        });
    });
}

function setupCouponBehavior() {
    const couponInput = document.getElementById('CouponInput');
    const applyButton = document.getElementById('applyCouponButton');

    if (!couponInput || !applyButton) return;

    applyButton.addEventListener('click', () => {
        const code = couponInput.value.trim().toUpperCase();

        // NOTA: A validação e aplicação real do desconto é feita no C#.
        // Aqui, apenas simulamos para a UX do cliente.

        if (!code) {
            showCheckoutToast('Digite um cupom antes de aplicar.', 'warning');
            return;
        }

        const summaryDiscounts = document.getElementById('summaryDiscounts');
        const hiddenDiscount = document.getElementById('hiddenDiscount');
        const hiddenSubtotal = document.getElementById('hiddenSubtotal');
        let discount = 0;

        // Simulação de desconto
        if (code === 'TGS10') {
            discount = parseDecimalValue(hiddenSubtotal?.value) * 0.1;
        } else if (code === 'FRETEGRATIS') {
            discount = getCurrentShippingPrice();
        } else {
            showCheckoutToast('Cupom inválido ou expirado.', 'error');
            summaryDiscounts.textContent = formatCurrency(0, true);
            if (hiddenDiscount) hiddenDiscount.value = '0';
            updateSummaryTotals();
            return;
        }

        if (!Number.isFinite(discount)) {
            discount = 0;
        }

        summaryDiscounts.textContent = formatCurrency(discount, true); // O 'true' adiciona o '-'
        if (hiddenDiscount) {
            hiddenDiscount.value = discount.toFixed(2);
        }
        updateSummaryTotals();
        showCheckoutToast('Cupom aplicado com sucesso!', 'success');
    });
}

// =========================================================================
// FUNÇÕES DE CÁLCULO E FORMATAÇÃO (APENAS FRONT-END)
// =========================================================================

function getCurrentShippingPrice() {
    const selectedShipping = document.querySelector('.shipping-card.is-selected');
    if (selectedShipping) {
        const datasetValue = selectedShipping.getAttribute('data-shipping-price');
        if (datasetValue) {
            return parseDecimalValue(datasetValue);
        }
    }

    const hiddenShipping = document.getElementById('hiddenShipping');
    if (hiddenShipping) {
        return parseDecimalValue(hiddenShipping.value);
    }

    return 0;
}

function getCurrentShippingSummary() {
    const selectedShipping = document.querySelector('.shipping-card.is-selected');
    if (!selectedShipping) {
        return 'Entrega grátis';
    }

    const label = selectedShipping.getAttribute('data-shipping-label') || 'Entrega';
    const price = parseDecimalValue(selectedShipping.getAttribute('data-shipping-price') || '0');
    const priceText = price <= 0 ? 'Grátis' : formatCurrency(price);

    return `${label} · ${priceText}`;
}

function updateSummaryTotals() {
    const summaryShipping = document.getElementById('summaryShipping');
    const summaryTotal = document.getElementById('summaryTotal');
    const summaryDiscounts = document.getElementById('summaryDiscounts');
    const hiddenSubtotal = document.getElementById('hiddenSubtotal');
    const hiddenDiscount = document.getElementById('hiddenDiscount');
    const hiddenTotal = document.getElementById('hiddenTotal');

    const subtotalValue = parseDecimalValue(hiddenSubtotal?.value);
    const shippingValue = getCurrentShippingPrice();
    const discountValue = parseDecimalValue(hiddenDiscount?.value);

    const total = subtotalValue + shippingValue - discountValue;

    if (summaryShipping) summaryShipping.textContent = getCurrentShippingSummary();
    if (summaryDiscounts) summaryDiscounts.textContent = formatCurrency(discountValue, true);
    if (summaryTotal) summaryTotal.textContent = formatCurrency(total);
    if (hiddenTotal) hiddenTotal.value = total.toFixed(2);
    updateInstallmentOptions(total);
}

function parseCurrency(value) {
    if (!value) return 0;
    // Remove R$, espaços, pontos de milhar e substitui vírgula por ponto decimal
    return Number(
        value
            .replace(/[R$\s-]/g, '')
            .replace(/\./g, '')
            .replace(',', '.')
    ) || 0;
}

function parseDecimalValue(value) {
    if (value === undefined || value === null) return 0;
    return Number(
        value
            .toString()
            .replace(/\s/g, '')
            .replace(/\./g, '')
            .replace(',', '.')
    ) || 0;
}

function formatCurrency(value, isNegative = false) {
    const formatted = value.toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });
    // Adiciona o sinal negativo se necessário
    return isNegative ? `- ${formatted}` : formatted;
}

function updateInstallmentOptions(baseAmount) {
    const installmentSelect = document.getElementById('Installments');
    if (!installmentSelect) return;

    const hiddenSubtotal = document.getElementById('hiddenSubtotal');
    const amount = baseAmount > 0 ? baseAmount : parseDecimalValue(hiddenSubtotal?.value);
    if (amount <= 0) return;

    const interestRateSixInstallments = 0.12;
    const config = {
        '1': { parts: 1, interest: false },
        '2': { parts: 2, interest: false },
        '3': { parts: 3, interest: false },
        '6': { parts: 6, interest: true }
    };

    Array.from(installmentSelect.options).forEach((option) => {
        const optionConfig = config[option.value];
        if (!optionConfig) return;

        const total = optionConfig.interest ? amount * (1 + interestRateSixInstallments) : amount;
        const perInstallment = total / optionConfig.parts;
        const suffix = optionConfig.interest ? 'com juros' : 'sem juros';
        let text = `${optionConfig.parts}x de ${formatCurrency(perInstallment)} ${suffix}`;
        if (optionConfig.interest) {
            text += ` (Total ${formatCurrency(total)})`;
        }
        option.textContent = text;
    });
}

// =========================================================================
// FUNÇÕES DE VALIDAÇÃO E ENVIO (PONTE PARA O C#)
// =========================================================================

/**
 * Aplica máscaras básicas para melhorar a experiência do usuário.
 */
function setupMasks() {
    function applyMask(element, mask) {
        if (!element) return;
        element.addEventListener('input', (e) => {
            let value = e.target.value.replace(/\D/g, '');
            let maskedValue = '';
            let k = 0;
            for (let i = 0; i < mask.length; i++) {
                if (k >= value.length) break;
                if (mask[i] === '#') {
                    maskedValue += value[k++];
                } else {
                    maskedValue += mask[i];
                }
            }
            e.target.value = maskedValue;
        });
    }

    applyMask(document.getElementById('CheckoutDocument'), '###.###.###-##'); // CPF (Poderia ser melhorada para CNPJ)
    applyMask(document.getElementById('CheckoutPhone'), '(##) #####-####');
    applyMask(document.getElementById('CheckoutCep'), '#####-###');
    applyMask(document.getElementById('CardNumber'), '#### #### #### ####');
    applyMask(document.getElementById('CardExpiration'), '##/##');
    applyMask(document.getElementById('CardCvv'), '###');
}


function setupConfirmButton() {
    const confirmButton = document.getElementById('confirmOrderButton');
    const form = document.getElementById('checkoutForm');
    if (!confirmButton || !form) return;

    confirmButton.addEventListener('click', () => {
        if (!form.checkValidity()) {
            return;
        }

        confirmButton.disabled = true;
        confirmButton.classList.add('is-loading');
        confirmButton.dataset.originalLabel = confirmButton.dataset.originalLabel || confirmButton.innerHTML;
        confirmButton.innerHTML = 'Processando...';
    });
}

// =========================================================================
// FUNÇÕES DE UTILIDADE
// =========================================================================

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

//API viacep
function setupCepLookup() {
    document.getElementById('CheckoutCep').addEventListener('blur', function () {
        const cep = this.value.replace(/\D/g, ''); 

        if (cep.length !== 8) {
            clearAddressFields();
            return;
        }


        setAddressFieldsDisabled(true);

        const url = `https://viacep.com.br/ws/${cep}/json/`;

        fetch(url)
            .then(response => response.json())
            .then(data => {
                setAddressFieldsDisabled(false); 

                if (!data.erro) {
                   
                    document.getElementById('CheckoutEndereco').value = data.logradouro;
                    document.getElementById('CheckoutCidade').value = data.localidade;
                    document.getElementById('CheckoutEstado').value = data.uf;
        

                    document.getElementById('CheckoutNumero').focus();
                    showCheckoutToast(`Endereço encontrado: ${data.logradouro}, ${data.localidade}-${data.uf}`, 'info');

                } else {
                    alert('CEP não encontrado. Preencha o endereço manualmente.');
                    clearAddressFields();
                    document.getElementById('CheckoutEndereco').focus();
                }
            })
            .catch(error => {
                setAddressFieldsDisabled(false);
                console.error('Erro na consulta do CEP:', error);
                showCheckoutToast('Erro ao consultar o CEP. Preencha manualmente.', 'error');
                clearAddressFields();
            });
    });
}