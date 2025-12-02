/**
 * AdmAdicionarProdutos.js
 * Script para interações VISUAIS na página de gerenciamento de produtos.
 * A listagem e manipulação de dados é feita pelo servidor (C#).
 */

// Executa quando o conteúdo da página é totalmente carregado.
document.addEventListener('DOMContentLoaded', function () {
    showTab('adicionar');
});

/**
 * Alterna a visibilidade entre as abas 'adicionar' e 'listar'.
 * @param {string} tabName - O nome da aba para exibir ('adicionar' ou 'listar').
 */
function showTab(tabName) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));

    document.getElementById(`tab-${tabName}`).classList.add('active');
    document.querySelector(`.tab-btn[onclick="showTab('${tabName}')"]`).classList.add('active');
}

/**
 * Abre o modal de edição e PREENCHE os campos com os dados.
 */
function openEditModal(id, nome, categoria, preco, quantidade, descricao) {
    const modal = document.getElementById('modalEditar');

    // Preenche os campos do formulário do modal
    document.getElementById('editId').value = id;
    document.getElementById('editNome').value = nome;
    document.getElementById('editCategoria').value = categoria.toLowerCase(); // Ajuste conforme seus values
    document.getElementById('editPreco').value = preco;
    document.getElementById('editQuantidade').value = quantidade;
    document.getElementById('editDescricao').value = descricao;

    // Exibe o modal
    modal.style.display = 'block';
}

/**
 * Fecha o modal de edição.
 */
function closeEditModal() {
    document.getElementById('modalEditar').style.display = 'none';
}

/**
 * Abre o modal de GERENCIAR estoque (Adicionar ou Remover).
 * @param {number} id - ID do produto
 * @param {string} nome - Nome do produto
 * @param {number} quantidadeAtual - Quantidade atual em estoque
 */
function openManageStockModal(id, nome, quantidadeAtual) {
    const modal = document.getElementById('modalGerenciarEstoque');

    // Preenche os campos do formulário
    document.getElementById('stockProdutoId').value = id;
    document.getElementById('stockProdutoNome').value = nome;

    // Preenche o campo visual e o campo oculto (se houver lógica extra)
    const campoTexto = document.getElementById('stockQuantidadeAtualTexto');
    if (campoTexto) campoTexto.value = quantidadeAtual + ' unidade(s)';

    const campoHidden = document.getElementById('stockQuantidadeAtual');
    if (campoHidden) campoHidden.value = quantidadeAtual;

    // Reseta o input de quantidade
    document.getElementById('stockQuantidade').value = '';

    // Reseta o select para "Adicionar" por padrão
    const selectOperacao = document.getElementById('stockOperacao');
    if (selectOperacao) selectOperacao.value = 'entrada';

    // Exibe o modal
    modal.style.display = 'block';
}

/**
 * Fecha o modal de gerenciar estoque.
 */
function closeManageStockModal() {
    document.getElementById('modalGerenciarEstoque').style.display = 'none';
    // Limpa o formulário
    const form = document.getElementById('form-gerenciar-estoque');
    if (form) form.reset();
}

/**
 * Listener global para fechar os modais ao clicar fora deles.
 */
window.onclick = function (event) {
    const modalEditar = document.getElementById('modalEditar');
    const modalEstoque = document.getElementById('modalGerenciarEstoque');

    if (event.target == modalEditar) {
        closeEditModal();
    }
    if (event.target == modalEstoque) {
        closeManageStockModal();
    }
}

/**
 * Confirma a adição de um produto antes de enviar o formulário.
 */
function confirmarAdicionarProduto(event) {
    // Obtém os valores do formulário para mostrar na confirmação
    const nome = document.getElementById('nome').value;
    const categoria = document.getElementById('categoria');
    const categoriaTexto = categoria.options[categoria.selectedIndex].text;
    const preco = document.getElementById('preco').value;

    // Valida se os campos obrigatórios estão preenchidos
    if (!nome || !categoria.value || !preco) {
        return true; // Deixa o HTML5 validation funcionar
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