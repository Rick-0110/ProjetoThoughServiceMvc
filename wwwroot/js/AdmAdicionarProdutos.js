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
 * Esta função é puramente VISUAL.
 * @param {string} tabName - O nome da aba para exibir ('adicionar' ou 'listar').
 */
function showTab(tabName) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));

    document.getElementById(`tab-${tabName}`).classList.add('active');
    document.querySelector(`.tab-btn[onclick="showTab('${tabName}')"]`).classList.add('active');
}

/**
 * Abre o modal de edição e PREENCHE os campos com os dados
 * passados diretamente pelo HTML (via Razor).
 * Esta função é puramente VISUAL e não busca dados na internet.
 */
function openEditModal(id, nome, categoria, preco, quantidade, descricao) {
    const modal = document.getElementById('modalEditar');

    // Preenche os campos do formulário do modal
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
 * Fecha o modal de edição.
 * Esta função é puramente VISUAL.
 */
function closeEditModal() {
    document.getElementById('modalEditar').style.display = 'none';
}

// Adiciona um listener global para fechar o modal ao clicar fora dele.
window.onclick = function (event) {
    if (event.target == document.getElementById('modalEditar')) {
        closeEditModal();
    }
}