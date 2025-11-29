/**
 * AdmAdicionarProdutos.js
 * Script para intera��es VISUAIS na p�gina de gerenciamento de produtos.
 * A listagem e manipula��o de dados � feita pelo servidor (C#).
 */

// Executa quando o conte�do da p�gina � totalmente carregado.
document.addEventListener('DOMContentLoaded', function () {
    showTab('adicionar');
});

/**
 * Alterna a visibilidade entre as abas 'adicionar' e 'listar'.
 * Esta fun��o � puramente VISUAL.
 * @param {string} tabName - O nome da aba para exibir ('adicionar' ou 'listar').
 */
function showTab(tabName) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));

    document.getElementById(`tab-${tabName}`).classList.add('active');
    document.querySelector(`.tab-btn[onclick="showTab('${tabName}')"]`).classList.add('active');
}

/**
 * Abre o modal de edi��o e PREENCHE os campos com os dados
 * passados diretamente pelo HTML (via Razor).
 * Esta fun��o � puramente VISUAL e n�o busca dados na internet.
 */
function openEditModal(id, nome, categoria, preco, quantidade, descricao) {
    const modal = document.getElementById('modalEditar');

    // Preenche os campos do formul�rio do modal
    document.getElementById('editId').value = id;
    document.getElementById('editNome').value = nome;
    document.getElementById('editCategoria').value = categoria.toLowerCase();
    document.getElementById('editPreco').value = preco;
    document.getElementById('editQuantidade').value = quantidade;
    document.getElementById('editDescricao').value = descricao;

    // Exibe o modal
    modal.style.display = 'block';
}

/**
 * Fecha o modal de edi��o.
 * Esta fun��o � puramente VISUAL.
 */
function closeEditModal() {
    document.getElementById('modalEditar').style.display = 'none';
}

// Adiciona um listener global para fechar o modal ao clicar fora dele.
window.onclick = function (event) {
    const modalEditar = document.getElementById('modalEditar');
    const modalEstoque = document.getElementById('modalAdicionarEstoque');
    
    if (event.target == modalEditar) {
        closeEditModal();
    }
    if (event.target == modalEstoque) {
        closeAddStockModal();
    }
}

/**
 * Abre o modal de adicionar estoque e preenche os dados do produto.
 * @param {number} id - ID do produto
 * @param {string} nome - Nome do produto
 * @param {number} quantidadeAtual - Quantidade atual em estoque
 */
function openAddStockModal(id, nome, quantidadeAtual) {
    const modal = document.getElementById('modalAdicionarEstoque');
    
    // Preenche os campos do formulário
    document.getElementById('stockProdutoId').value = id;
    document.getElementById('stockProdutoNome').value = nome;
    document.getElementById('stockQuantidadeAtual').value = quantidadeAtual + ' unidade(s)';
    document.getElementById('stockQuantidade').value = '';
    
    // Exibe o modal
    modal.style.display = 'block';
}

/**
 * Fecha o modal de adicionar estoque.
 */
function closeAddStockModal() {
    document.getElementById('modalAdicionarEstoque').style.display = 'none';
    // Limpa o formulário
    document.getElementById('form-adicionar-estoque').reset();
}

/**
 * Confirma a adição de um produto antes de enviar o formulário.
 * Exibe uma mensagem de confirmação com os dados principais do produto.
 * @param {Event} event - Evento de submit do formulário
 * @returns {boolean} - Retorna false para cancelar o envio, true para permitir
 */
function confirmarAdicionarProduto(event) {
    // Obtém os valores do formulário para mostrar na confirmação
    const nome = document.getElementById('nome').value;
    const categoria = document.getElementById('categoria');
    const categoriaTexto = categoria.options[categoria.selectedIndex].text;
    const preco = document.getElementById('preco').value;
    
    // Valida se os campos obrigatórios estão preenchidos
    if (!nome || !categoria.value || !preco) {
        // Se não estiverem preenchidos, deixa o HTML5 validation funcionar
        return true;
    }
    
    // Monta a mensagem de confirmação
    let mensagem = 'Tem certeza que deseja adicionar o produto abaixo?\n\n';
    mensagem += `Nome: ${nome}\n`;
    mensagem += `Categoria: ${categoriaTexto}\n`;
    mensagem += `Preço: R$ ${parseFloat(preco).toFixed(2).replace('.', ',')}\n\n`;
    mensagem += 'Esta ação não pode ser desfeita facilmente.';
    
    // Mostra a confirmação
    const confirmacao = confirm(mensagem);
    
    // Se o usuário cancelar, impede o envio do formulário
    if (!confirmacao) {
        event.preventDefault();
        return false;
    }
    
    // Se confirmar, permite o envio
    return true;
}