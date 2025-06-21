function home(){
window.location.href = "paginaPrincipal.html";
};

document.addEventListener('DOMContentLoaded', function() {
    carregarCarrinho();
});

function carregarCarrinho() {
    const carrinho = JSON.parse(localStorage.getItem('carrinho')) || [];
    const container = document.querySelector('.carrinho-items');
    
    if (carrinho.length === 0) {
        container.innerHTML = '<div class="carrinho-vazio">Seu carrinho está vazio</div>';
        return;
    }
    
    container.innerHTML = '';
    
    carrinho.forEach((item, index) => {
        const div = document.createElement('div');
        div.className = 'carrinho-item';
        div.innerHTML = `
            <div class="item-imagem">
                <img src="${item.imagem}" alt="${item.nome}">
            </div>
            <div class="item-detalhes">
                <h3>${item.nome}</h3>
                <div class="item-preco">${item.preco}</div>
            </div>
            <div class="item-acoes">
                <button onclick="removerItem(${index})">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        `;
        container.appendChild(div);
    });
    
    atualizarResumo();
}

function removerItem(index) {
    let carrinho = JSON.parse(localStorage.getItem('carrinho')) || [];
    carrinho.splice(index, 1);
    localStorage.setItem('carrinho', JSON.stringify(carrinho));
    carregarCarrinho();
}

function atualizarResumo() {
    const carrinho = JSON.parse(localStorage.getItem('carrinho')) || [];
    const subtotal = carrinho.reduce((total, item) => {
        const preco = parseFloat(item.preco.replace('R$', '').replace(',', '.').trim());
        return total + preco;
    }, 0);
    
    const frete = 10.00;
    const total = subtotal + frete;
    
    document.querySelector('.subtotal').textContent = `R$ ${subtotal.toFixed(2).replace('.', ',')}`;
    document.querySelector('.frete').textContent = `R$ ${frete.toFixed(2).replace('.', ',')}`;
    document.querySelector('.total').textContent = `R$ ${total.toFixed(2).replace('.', ',')}`;
}


document.querySelector('.btn-limpar').addEventListener('click', function() {
    localStorage.removeItem('carrinho');
    carregarCarrinho();
});


document.querySelector('.btn-continuar').addEventListener('click', function() {
    window.location.href = 'paginaPrincipal.html';
});


