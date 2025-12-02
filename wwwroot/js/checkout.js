document.addEventListener('DOMContentLoaded', () => {
    // Inicialização
    setupAddressSelection();
    setupShippingSelection();
    setupPaymentTabs(); // Simplificado
    setupCouponBehavior();
    setupMasks(); // Apenas CPF, CEP, Tel
    setupCepLookup();
    setupConfirmButton();
    updateSummaryTotals();
});

// =========================================================================
// 1. ENDEREÇO
// =========================================================================
function setupAddressSelection() {
    const addressList = document.getElementById('addressList');
    const selectedInput = document.getElementById('selectedAddressId');

    // Seleciona o primeiro endereço automaticamente se nenhum estiver selecionado
    const firstCard = document.querySelector('.address-card');
    if (firstCard && (!selectedInput.value || selectedInput.value === '')) {
        firstCard.classList.add('is-selected');
        selectedInput.value = firstCard.dataset.addressId;
    }

    if (!addressList) return;

    addressList.addEventListener('click', (event) => {
        const card = event.target.closest('.address-card');
        if (!card) return;

        document.querySelectorAll('.address-card').forEach((addr) => {
            addr.classList.remove('is-selected');
        });
        card.classList.add('is-selected');

        if (selectedInput) {
            selectedInput.value = card.dataset.addressId || '';
        }
    });
}

// =========================================================================
// 2. CEP (VIA CEP)
// =========================================================================
function limpa_formulário_cep() {
    // Limpa valores do formulário de cep.
    document.getElementById('rua').value = ("");
    document.getElementById('cidade').value = ("");
    document.getElementById('uf').value = ("");
    // document.getElementById('bairro').value = (""); // Se tiver campo bairro
}

function meu_callback(conteudo) {
    if (!("erro" in conteudo)) {
        //Atualiza os campos com os valores.
        document.getElementById('rua').value = (conteudo.logradouro);
        // document.getElementById('bairro').value=(conteudo.bairro);
        document.getElementById('cidade').value = (conteudo.localidade);
        document.getElementById('uf').value = (conteudo.uf);

        // Foca no número para o usuário digitar
        document.getElementById('numero').focus();
    }
    else {
        //CEP não Encontrado.
        limpa_formulário_cep();
        alert("CEP não encontrado.");

        // Habilita edição manual caso o CEP falhe
        document.getElementById('rua').removeAttribute('readonly');
        document.getElementById('cidade').removeAttribute('readonly');
        document.getElementById('uf').removeAttribute('readonly');
    }
}

function pesquisacep(valor) {
    //Nova variável "cep" somente com dígitos.
    var cep = valor.replace(/\D/g, '');

    //Verifica se campo cep possui valor informado.
    if (cep != "") {
        //Expressão regular para validar o CEP.
        var validacep = /^[0-9]{8}$/;

        if (validacep.test(cep)) {
            document.getElementById('rua').value = "...";
            document.getElementById('cidade').value = "...";
            document.getElementById('uf').value = "...";

            var script = document.createElement('script');

            script.src = 'https://viacep.com.br/ws/' + cep + '/json/?callback=meu_callback';

            document.body.appendChild(script);

        } else {
            limpa_formulário_cep();
            alert("Formato de CEP inválido.");
        }
    } else {
        limpa_formulário_cep();
    }
};

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

            updateSummaryTotals();
        });
    });
}

// =========================================================================
// 4. PAGAMENTO (SIMPLIFICADO - SEM VALIDAÇÃO DE CARTÃO)
// =========================================================================
function setupPaymentTabs() {
    const tabs = document.querySelectorAll('.payment-tab');
    const panels = document.querySelectorAll('[data-payment-panel]');

    if (!tabs.length) return;

    // Garante valor inicial
    const activeTab = document.querySelector('.payment-tab.is-active');
    if (activeTab) {
        const paymentField = document.getElementById('paymentMethodField');
        if (paymentField) paymentField.value = activeTab.dataset.payment;

        // Mostra o painel inicial
        panels.forEach((panel) => {
            panel.classList.toggle('is-visible', panel.dataset.paymentPanel === activeTab.dataset.payment);
        });
    }

    tabs.forEach((tab) => {
        tab.addEventListener('click', () => {
            const paymentType = tab.dataset.payment;

            // Atualiza Abas
            tabs.forEach((other) => other.classList.remove('is-active'));
            tab.classList.add('is-active');

            // Atualiza Painéis (apenas troca a div visível)
            panels.forEach((panel) => {
                panel.classList.toggle('is-visible', panel.dataset.paymentPanel === paymentType);
            });

            // Atualiza Campo Hidden para enviar ao Controller
            const paymentField = document.getElementById('paymentMethodField');
            if (paymentField) paymentField.value = paymentType;
        });
    });
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
        showCheckoutToast('Validando cupom...', 'info');
    });
}

// =========================================================================
// 6. CÁLCULOS E TOTAIS
// =========================================================================
function updateSummaryTotals() {
    const subtotalEl = document.getElementById('hiddenSubtotal');
    const discountEl = document.getElementById('hiddenDiscount');

    const subtotal = subtotalEl ? parseCurrency(subtotalEl.value) : 0;
    const discount = discountEl ? parseCurrency(discountEl.value) : 0;

    let shippingPrice = 0;
    const selectedShipping = document.querySelector('.shipping-card.is-selected');
    if (selectedShipping) {
        shippingPrice = parseCurrency(selectedShipping.dataset.shippingPrice || '0');
        const label = selectedShipping.dataset.shippingLabel;
        const priceText = shippingPrice === 0 ? "Grátis" : formatCurrency(shippingPrice);

        const summaryShipping = document.getElementById('summaryShipping');
        if (summaryShipping) summaryShipping.textContent = `${label} · ${priceText}`;
    }

    const total = subtotal + shippingPrice - discount;

    const summaryTotal = document.getElementById('summaryTotal');
    if (summaryTotal) summaryTotal.textContent = formatCurrency(total);

    const hiddenTotal = document.getElementById('hiddenTotal');
    if (hiddenTotal) hiddenTotal.value = total.toFixed(2).replace('.', ',');
}

function parseCurrency(value) {
    if (!value) return 0;
    if (typeof value === 'number') return value;
    return parseFloat(value.toString().replace('R$', '').replace(/\./g, '').replace(',', '.')) || 0;
}

function formatCurrency(value) {
    return value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

// =========================================================================
// 7. BOTÃO CONFIRMAR
// =========================================================================
function setupConfirmButton() {
    const confirmButton = document.getElementById('confirmOrderButton');
    const form = document.getElementById('checkoutForm');

    if (!confirmButton || !form) return;

    confirmButton.addEventListener('click', (e) => {
        // 1. IMPEDE O ENVIO AUTOMÁTICO (Para controlarmos o fluxo)
        e.preventDefault();

        // 2. VALIDAÇÃO
        if (!form.checkValidity()) {
            const invalidField = form.querySelector(':invalid');
            console.warn("Campo inválido:", invalidField);

            let msg = "Preencha todos os campos obrigatórios.";
            if (invalidField) {
                const label = invalidField.previousElementSibling?.textContent || invalidField.name;
                msg = `Verifique o campo: ${label}`;
                invalidField.focus();
            }

            showCheckoutToast(msg, 'error');
            return;
        }

        // 3. MUDA O VISUAL (Feedback para o usuário)
        confirmButton.classList.add('disabled'); // Usa classe CSS em vez de prop disabled por enquanto
        confirmButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Redirecionando para Mercado Pago...';
        confirmButton.style.pointerEvents = 'none'; // Evita clique duplo

        // 4. ENVIA O FORMULÁRIO MANUALMENTE (Pulo do Gato)
        // Pequeno delay para garantir que a UI atualizou
        setTimeout(() => {
            form.submit();
        }, 100);
    });
}

// =========================================================================
// 8. UTILITÁRIOS
// =========================================================================
function showCheckoutToast(message, type = 'info') {
    const old = document.querySelector('.checkout-toast');
    if (old) old.remove();

    const toast = document.createElement('div');
    toast.className = `checkout-toast checkout-toast--${type}`;
    toast.textContent = message;
    document.body.appendChild(toast);

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
        'CheckoutCep': (val) => val.replace(/\D/g, '').replace(/(\d{5})(\d{3})/, '$1-$3')
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