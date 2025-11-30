// =========================================================================
// 0. INICIALIZAÇÃO E CHAMADAS INICIAIS
// =========================================================================
document.addEventListener('DOMContentLoaded', () => {
    // Inicialização
    setupAddressSelection();
    setupShippingSelection();
    setupPaymentTabs();
    setupCouponBehavior();
    setupMasks();
    setupCepLookup();
    setupConfirmButton(); // Garante que o botão seja configurado
    
    // NOVO: Verifica qualificação de frete ao carregar a página
    checkShippingQualification(); 

    // Inicializa validações com base na aba ativa atual
    const activePaymentTab = document.querySelector('.payment-tab.is-active');
    if (activePaymentTab) {
        updatePaymentValidators(activePaymentTab.dataset.payment);
        const paymentField = document.getElementById('paymentMethodField');
        if (paymentField) paymentField.value = activePaymentTab.dataset.payment;
    }

    // Calcula totais iniciais
    updateSummaryTotals();
});

// =========================================================================
// 1. ENDEREÇO
// =========================================================================
function setupAddressSelection() {
    const addressList = document.getElementById('addressList');
    if (!addressList) return;

    addressList.addEventListener('click', (event) => {
        const card = event.target.closest('.address-card');
        if (!card) return;

        // Visual
        document.querySelectorAll('.address-card').forEach((addr) => {
            addr.classList.remove('is-selected');
        });
        card.classList.add('is-selected');

        // Lógica
        const selectedAddressField = document.getElementById('selectedAddressId');
        if (selectedAddressField) {
            selectedAddressField.value = card.dataset.addressId || '';
        }
    });
}

// =========================================================================
// 2. CEP (VIA CEP)
// =========================================================================
function setupCepLookup() {
    const cepInput = document.getElementById('CheckoutCep');
    if (!cepInput) return;

    cepInput.addEventListener('blur', function () {
        const cep = this.value.replace(/\D/g, '');

        if (cep.length !== 8) return;

        // Feedback visual de carregamento
        document.getElementById('CheckoutEndereco').placeholder = "Buscando...";

        const url = `https://viacep.com.br/ws/${cep}/json/`;

        fetch(url)
            .then(response => response.json())
            .then(data => {
                if (!data.erro) {
                    const enderecoInput = document.getElementById('CheckoutEndereco');
                    const cidadeInput = document.getElementById('CheckoutCidade');
                    const estadoInput = document.getElementById('CheckoutEstado');
                    const bairroInput = document.getElementById('CheckoutBairro'); // Se existir

                    if (enderecoInput) enderecoInput.value = data.logradouro;
                    if (cidadeInput) cidadeInput.value = data.localidade;
                    if (estadoInput) estadoInput.value = data.uf;
                    // Se tiver campo de bairro: if(bairroInput) bairroInput.value = data.bairro;

                    document.getElementById('CheckoutNumero').focus();
                    showCheckoutToast(`Endereço encontrado!`, 'success');
                } else {
                    showCheckoutToast('CEP não encontrado.', 'warning');
                    document.getElementById('CheckoutEndereco').focus();
                }
            })
            .catch(() => {
                showCheckoutToast('Erro ao consultar CEP.', 'error');
            })
            .finally(() => {
                document.getElementById('CheckoutEndereco').placeholder = "Rua, avenida, etc.";
            });
    });
}

// =========================================================================
// 3. ENTREGA (FRETE)
// =========================================================================
function setupShippingSelection() {
    const cards = document.querySelectorAll('.shipping-card');
    if (!cards.length) return;

    cards.forEach((card) => {
        card.addEventListener('click', () => {
            cards.forEach((other) => other.classList.remove('is-selected'));
            card.classList.add('is-selected');

            const radio = card.querySelector('input[type="radio"]');
            if (radio) radio.checked = true;

            // Atualiza resumo visualmente
            updateSummaryTotals();
        });
    });
}

/**
 * NOVO: Checa se o frete foi qualificado (5 ou mais extintores) e desabilita o botão
 * de confirmação se não houver qualificação.
 */
function checkShippingQualification() {
    // Busca o hidden field injetado na View
    const isQualifiedElement = document.getElementById('freteQualificado');
    const freteQualificado = isQualifiedElement ? isQualifiedElement.value === 'true' : true; 
    const confirmButton = document.getElementById('confirmOrderButton');

    if (confirmButton) {
        if (!freteQualificado) {
            // Regra: Se NÃO qualificado (menos de 5 extintores), desabilita o botão
            confirmButton.disabled = true;
            confirmButton.textContent = 'Adicione extintores para liberar a entrega';
            confirmButton.classList.add('btn-disabled-frete'); 
            confirmButton.setAttribute('title', 'É necessário no mínimo 5 extintores no carrinho para liberar a entrega ou retirada.');
        } else {
            // Regra: Se qualificado, garante que o botão esteja habilitado (se não houver outros erros)
            confirmButton.disabled = false;
            confirmButton.textContent = 'Confirmar pedido';
            confirmButton.classList.remove('btn-disabled-frete');
            confirmButton.removeAttribute('title');
            
            // Verifica se o frete foi de fato SELECIONADO (só se aplica quando qualificado)
            const selectedShipping = document.querySelector('.shipping-card.is-selected');
            if (!selectedShipping) {
                confirmButton.disabled = true;
                confirmButton.textContent = 'Selecione uma opção de Entrega/Retirada';
            }
        }
    }
}


// =========================================================================
// 4. PAGAMENTO (A Correção Principal está aqui)
// =========================================================================
function setupPaymentTabs() {
    const tabs = document.querySelectorAll('.payment-tab');
    const panels = document.querySelectorAll('[data-payment-panel]');

    if (!tabs.length) return;

    tabs.forEach((tab) => {
        tab.addEventListener('click', () => {
            const paymentType = tab.dataset.payment;

            // 1. Atualiza Abas
            tabs.forEach((other) => other.classList.remove('is-active'));
            tab.classList.add('is-active');

            // 2. Atualiza Painéis
            panels.forEach((panel) => {
                panel.classList.toggle('is-visible', panel.dataset.paymentPanel === paymentType);
            });

            // 3. Atualiza Campo Hidden
            const paymentField = document.getElementById('paymentMethodField');
            if (paymentField) {
                paymentField.value = paymentType;
            }

            // 4. ATIVA/DESATIVA VALIDATORS (CRUCIAL!)
            updatePaymentValidators(paymentType);
        });
    });
}

/**
 * Função que adiciona ou remove 'required' dos campos de cartão
 * dependendo se a aba Cartão está ativa ou não.
 */
function updatePaymentValidators(paymentType) {
    const cardInputs = document.querySelectorAll('[data-payment-panel="card"] input, [data-payment-panel="card"] select');

    if (paymentType === 'card') {
        // Se for cartão, torna os campos obrigatórios
        cardInputs.forEach(input => {
            // Ignora o checkbox "Salvar Cartão"
            if (input.type !== 'checkbox') {
                input.setAttribute('required', 'required');
            }
        });
    } else {
        // Se for Pix ou Boleto, remove a obrigatoriedade
        cardInputs.forEach(input => {
            input.removeAttribute('required');
            // Opcional: Limpa erros visuais se houver
            input.classList.remove('input-validation-error');
        });
    }
}

// =========================================================================
// 5. CUPOM
// =========================================================================
function setupCouponBehavior() {
    const applyButton = document.getElementById('applyCouponButton');
    const couponInput = document.getElementById('CouponInput');

    if (!applyButton || !couponInput) return;

    applyButton.addEventListener('click', () => {
        const code = couponInput.value.trim();
        if (!code) {
            showCheckoutToast('Digite um código de cupom.', 'warning');
            return;
        }

        // Simulação Visual (A lógica real deve ser validada no C# ao enviar)
        if (code.toUpperCase() === 'PROMO10') {
            const hiddenDiscount = document.getElementById('hiddenDiscount');
            if (hiddenDiscount) hiddenDiscount.value = "10.00"; // Exemplo

            document.getElementById('summaryDiscounts').textContent = "- R$ 10,00";
            updateSummaryTotals();
            showCheckoutToast('Cupom aplicado!', 'success');
        } else {
            showCheckoutToast('Cupom inválido (Simulação).', 'error');
        }
    });
}

// =========================================================================
// 6. CÁLCULOS E TOTAIS
// =========================================================================
function updateSummaryTotals() {
    // Pega valores
    const subtotal = parseCurrency(document.getElementById('hiddenSubtotal')?.value || '0');
    const discount = parseCurrency(document.getElementById('hiddenDiscount')?.value || '0');

    // Pega frete selecionado
    let shippingPrice = 0;
    let shippingLabel = "Indisponível";

    const selectedShipping = document.querySelector('.shipping-card.is-selected');
    const freteQualificado = document.getElementById('freteQualificado')?.value === 'true';

    if (freteQualificado && selectedShipping) {
        shippingPrice = parseCurrency(selectedShipping.dataset.shippingPrice || '0');
        shippingLabel = selectedShipping.dataset.shippingLabel;
    } else if (freteQualificado && !selectedShipping) {
        // Se qualificado, mas não selecionado, mantém a label de "Selecione"
        shippingLabel = "Selecione";
    }

    const priceText = shippingPrice === 0 ? "Grátis" : formatCurrency(shippingPrice);
    document.getElementById('summaryShipping').textContent = `${shippingLabel} · ${priceText}`;
    
    // Atualiza o custo de frete no hidden field
    const hiddenShippingCost = document.getElementById('hiddenShippingCost');
    if (hiddenShippingCost) hiddenShippingCost.value = shippingPrice.toFixed(2).replace('.', ',');

    // Calcula Total
    const total = subtotal + shippingPrice - discount;

    // Atualiza HTML
    document.getElementById('summaryTotal').textContent = formatCurrency(total);

    // Atualiza Input Hidden para envio (opcional, pois o C# recalcula)
    const hiddenTotal = document.getElementById('hiddenTotal');
    if (hiddenTotal) hiddenTotal.value = total.toFixed(2).replace('.', ',');

    // Atualiza opções de parcelamento
    updateInstallmentOptions(total);
    
    // NOVO: Revalida o botão de Confirmação após o cálculo (no caso de re-seleção de frete)
    checkShippingQualification(); 
}

function updateInstallmentOptions(totalValue) {
    const select = document.getElementById('Installments');
    if (!select) return;

    // Apenas atualiza o texto da opção '1x' para refletir o novo total
    if (select.options.length > 0) {
        select.options[0].text = `1x de ${formatCurrency(totalValue)} sem juros`;
    }
}

function parseCurrency(value) {
    // Converte "1.200,50" ou "1200.50" para float JS
    if (typeof value === 'number') return value;
    return parseFloat(value.toString().replace('R$', '').replace(/\./g, '').replace(',', '.')) || 0;
}

function formatCurrency(value) {
    return value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

// =========================================================================
// 7. BOTÃO CONFIRMAR E SUBMIT
// =========================================================================
function setupConfirmButton() {
    const confirmButton = document.getElementById('confirmOrderButton');
    const form = document.getElementById('checkoutForm');

    if (!confirmButton || !form) return;
    
    // NOVO: Chama para garantir o estado inicial do botão
    checkShippingQualification();

    confirmButton.addEventListener('click', (e) => {
        // NOVO: Verifica novamente a qualificação e se o botão estiver desabilitado, impede o envio
        if (confirmButton.disabled) {
             e.preventDefault();
             showCheckoutToast(confirmButton.title || 'Atenção! É necessário qualificar o frete e selecioná-lo.', 'error');
             return;
        }

        // Verifica validação HTML5 nativa
        if (!form.checkValidity()) {
            e.preventDefault(); 
            
            const invalidField = form.querySelector(':invalid');
            console.warn("Campo inválido:", invalidField);

            showCheckoutToast(`Preencha o campo: ${invalidField.previousElementSibling?.textContent || invalidField.name}`, 'error');

            invalidField.focus();
            return;
        }

        // Se válido, muda estado do botão e submete o formulário
        confirmButton.disabled = true;
        confirmButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processando...';

        // Submete o formulário explicitamente
        form.submit();
    });
}

// =========================================================================
// 8. MÁSCARAS E UTILITÁRIOS
// =========================================================================
function showCheckoutToast(message, type = 'info') {
    // Remove anterior
    const old = document.querySelector('.checkout-toast');
    if (old) old.remove();

    const toast = document.createElement('div');
    toast.className = `checkout-toast checkout-toast--${type}`;
    toast.textContent = message;
    document.body.appendChild(toast);

    // CSS deve tratar a classe .is-visible para animar
    setTimeout(() => toast.classList.add('is-visible'), 10);
    setTimeout(() => {
        toast.classList.remove('is-visible');
        setTimeout(() => toast.remove(), 300);
    }, 4000);
}

function setupMasks() {
    const masks = {
        'CheckoutDocument': (val) => val.replace(/\D/g, '').replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4'),
        'CheckoutPhone': (val) => val.replace(/\D/g, '').replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3'),
        'CheckoutCep': (val) => val.replace(/\D/g, '').replace(/(\d{5})(\d{3})/, '$1-$3'),
        'CardNumber': (val) => val.replace(/\D/g, '').replace(/(\d{4})/g, '$1 ').trim(),
        'CardExpiration': (val) => val.replace(/\D/g, '').replace(/(\d{2})(\d{2})/, '$1/$2'),
        'CardCvv': (val) => val.replace(/\D/g, '').substring(0, 4)
    };

    Object.keys(masks).forEach(id => {
        const el = document.getElementById(id);
        if (el) {
            el.addEventListener('input', (e) => {
                e.target.value = masks[id](e.target.value);
            });
        }
    });
}