document.addEventListener('DOMContentLoaded', () => {
    const qtyControls = document.querySelectorAll('.produto-qty-control');
    if (!qtyControls.length) return;

    qtyControls.forEach(control => {
        const input = control.querySelector('.produto-qty-input');
        if (!input) return;

        control.addEventListener('click', (e) => {
            const btn = e.target.closest('.produto-qty-btn');
            if (!btn) return;

            const delta = parseInt(btn.dataset.qtyDelta || '0', 10);
            if (!delta) return;

            const current = parseInt(input.value || '1', 10) || 1;
            const min = parseInt(input.getAttribute('min') || '1', 10) || 1;

            let next = current + delta;
            if (next < min) next = min;

            input.value = next;
        });
    });
});


