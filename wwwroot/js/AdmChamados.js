

let currentChamadoId = 0; // Guarda o ID do chamado aberto no modal



function closeModal() {
    const modal = document.getElementById('modalDetalhes');
    if (modal) {
        modal.style.display = 'none';
        document.getElementById('chamadoDetails').innerHTML = '<p>Carregando...</p>'; // Reseta conteúdo
        currentChamadoId = 0; // Reseta ID
        // Resetar visibilidade dos botões
        const btnAndamento = document.getElementById('btnEmAndamento');
        const btnFinalizado = document.getElementById('btnFinalizado');
        if (btnAndamento) btnAndamento.style.display = 'inline-block';
        if (btnFinalizado) btnFinalizado.style.display = 'inline-block';
    }
}

async function openDetalhesModal(id) {
    const modal = document.getElementById('modalDetalhes');
    const detailsContainer = document.getElementById('chamadoDetails');
    const modalTitleId = document.getElementById('modalChamadoId');
    const btnEmAndamento = document.getElementById('btnEmAndamento');
    const btnFinalizado = document.getElementById('btnFinalizado');

    if (!modal || !detailsContainer || !modalTitleId || !btnEmAndamento || !btnFinalizado) {
        console.error("Elementos do modal de detalhes não encontrados.");
        return;
    }

    modal.style.display = 'block'; // Mostra o modal
    detailsContainer.innerHTML = '<p><i class="fas fa-spinner fa-spin"></i> Carregando detalhes...</p>'; // Feedback visual
    modalTitleId.textContent = `#${id}`; // Atualiza título
    currentChamadoId = id; // Guarda o ID para os botões de status

    try {
        // Busca os detalhes específicos via AJAX/Fetch no endpoint C#
        const response = await fetch(`/ADM/ObterDetalhesChamado/${id}`);
        if (!response.ok) {
            throw new Error(`Erro ${response.status}: Falha ao buscar dados.`);
        }
        const data = await response.json(); // Pega a resposta JSON

        // Preenche o conteúdo do modal com os dados recebidos
        detailsContainer.innerHTML = `
            <div class="detail-grid">
                <div><strong>ID:</strong> ${data.id}</div>
                <div><strong>Status:</strong> <span class="badge-${data.status.toLowerCase()}">${data.status}</span></div>
                <div><strong>Cliente:</strong> ${escapeHtml(data.cliente)}</div>
                <div><strong>Email Usuário:</strong> ${escapeHtml(data.userEmail)}</div>
                <div><strong>Solicitado em:</strong> ${data.dataSolicitacao}</div>
                <div><strong>Data Desejada:</strong> ${data.dataDesejada}</div>
                <div><strong>Telefone:</strong> ${escapeHtml(data.telefone)}</div>
                <div><strong>Tipo Serviço:</strong> ${escapeHtml(data.tipoServico)}</div>
                <div><strong>Item:</strong> ${escapeHtml(data.tipoExtintor)} (${data.quantidade} unid.)</div>
                <div class="full-width"><strong>Endereço:</strong> ${escapeHtml(data.logradouro)}, ${escapeHtml(data.numero)} ${escapeHtml(data.complemento)} - ${escapeHtml(data.bairro)}, ${escapeHtml(data.cidade)}/${escapeHtml(data.estado)} - CEP: ${escapeHtml(data.cep)}</div>
                <div class="full-width"><strong>Observações:</strong> ${escapeHtml(data.observacoes)}</div>
            </div>
        `;

        // Controla a visibilidade dos botões de ação baseado no status atual
        // Certifique-se que os valores (0, 1, 2) correspondem ao seu StatusChamadoEnum (Novo, EmAndamento, Finalizado)
        if (data.statusRaw === 1) { // EmAndamento
            btnEmAndamento.style.display = 'none';
            btnFinalizado.style.display = 'inline-block';
        } else if (data.statusRaw === 2) { // Finalizado
            btnEmAndamento.style.display = 'none';
            btnFinalizado.style.display = 'none';
        } else { // Novo (statusRaw === 0)
            btnEmAndamento.style.display = 'inline-block';
            btnFinalizado.style.display = 'inline-block'; // Ou 'none' se não puder finalizar direto de 'Novo'
        }

    } catch (error) {
        detailsContainer.innerHTML = `<p style="color: red;">Erro ao carregar detalhes: ${error.message}</p>`;
        console.error("Fetch error:", error);
    }
}

// --- Função para Disparar a Atualização de Status (via Form Submit para C#) ---

// Renomeando as funções que estavam no onclick dos botões no HTML antigo
function marcarEmAndamento() {
    submitStatusUpdate('EmAndamento');
}
function finalizarChamado() {
    submitStatusUpdate('Finalizado');
}

function submitStatusUpdate(novoStatus) {
    if (currentChamadoId <= 0) {
        alert("ID do chamado inválido.");
        return;
    }

    // Preenche os campos ocultos do formulário no rodapé do modal
    const idInput = document.getElementById('chamadoIdHidden');
    const statusInput = document.getElementById('novoStatusHidden');
    const form = document.getElementById('formAtualizarStatus');

    if (idInput && statusInput && form) {
        idInput.value = currentChamadoId;
        statusInput.value = novoStatus; // 'EmAndamento' ou 'Finalizado'

        // Submete o formulário HTML. O C# processará e redirecionará.
        form.submit();

        // Opcional: Desabilitar botões enquanto submete para evitar duplo clique
        const footerButtons = form.querySelectorAll('button');
        footerButtons.forEach(btn => btn.disabled = true);
        // Não precisa reabilitar aqui, pois a página vai recarregar.
    } else {
        console.error("Formulário de atualização de status ou seus campos ocultos não encontrados.");
        alert("Erro ao tentar atualizar o status.");
    }
}

// --- Funções Visuais: Filtros Client-Side e Mensagens ---

// Função para filtrar os cards que JÁ ESTÃO na página (renderizados pelo C#)
function filterChamados() {
    const statusFilter = document.getElementById('statusFilter')?.value.toLowerCase() || '';
    const tipoFilter = document.getElementById('tipoFilter')?.value.toLowerCase() || '';
    const cards = document.querySelectorAll('#chamadosList .chamado-card');
    let visibleCount = 0;

    cards.forEach(card => {
        const cardStatus = card.getAttribute('data-status') || '';
        const cardTipo = card.getAttribute('data-tipo') || '';

        // Verifica se o card corresponde aos filtros selecionados
        const statusMatch = !statusFilter || cardStatus === statusFilter;
        const tipoMatch = !tipoFilter || (cardTipo && cardTipo.includes(tipoFilter)); // Usar includes pode ser útil

        // Mostra ou esconde o card
        if (statusMatch && tipoMatch) {
            card.style.display = ''; // Mostra (padrão do CSS)
            visibleCount++;
        } else {
            card.style.display = 'none'; // Esconde
        }
    });

    // Atualiza as estatísticas com base nos cards visíveis
    updateStatsBasedOnVisibleCards();

    // Mostra/Esconde a mensagem de "Nenhum chamado encontrado"
    const noChamadosDiv = document.querySelector('.no-chamados');
    if (noChamadosDiv) {
        noChamadosDiv.style.display = visibleCount === 0 ? 'block' : 'none';
    }
}

// Função para atualizar os cards de estatísticas baseado nos cards visíveis
function updateStatsBasedOnVisibleCards() {
    const visibleCards = document.querySelectorAll('#chamadosList .chamado-card:not([style*="display: none"])');
    const allCards = document.querySelectorAll('#chamadosList .chamado-card'); // Para o total geral

    let novos = 0;
    let emAndamento = 0;
    let finalizados = 0;

    visibleCards.forEach(card => {
        const status = card.getAttribute('data-status');
        if (status === 'novo') novos++;
        // Certifique-se que 'emandamento' corresponde ao valor do enum C# convertido para lower case
        else if (status === 'emandamento') emAndamento++;
        else if (status === 'finalizado') finalizados++;
    });

    // Atualiza os elementos HTML das estatísticas (verifique os IDs no seu HTML)
    const totalEl = document.getElementById('totalChamados');
    const novosEl = document.getElementById('chamadosNovos');
    const andamentoEl = document.getElementById('chamadosAndamento');
    const finalizadosEl = document.getElementById('chamadosFinalizados');

    if (totalEl) totalEl.textContent = allCards.length; // Mostra o total geral sempre
    if (novosEl) novosEl.textContent = novos;
    if (andamentoEl) andamentoEl.textContent = emAndamento;
    if (finalizadosEl) finalizadosEl.textContent = finalizados;
}

// Função para recarregar a página (botão Atualizar)
function refreshChamados() {
    window.location.reload();
}

// Função para exportar (Mantida como placeholder, idealmente seria C#)
function exportChamados() {
    alert('Funcionalidade de exportação será implementada no backend.');
    // Ou, se quiser exportar SÓ OS VISÍVEIS via JS (menos robusto):
    // const visibleData = [];
    // document.querySelectorAll('#chamadosList .chamado-card:not([style*="display: none"])').forEach(card => { /* extrair dados do card */ });
    // /* Lógica para converter visibleData em CSV/JSON e baixar */
}


// Função para exibir Toast (Mensagem "Bonitinha")
function showToast(message, isError = false) {
    const toastId = isError ? 'errorToast' : 'successToast';
    const messageId = isError ? 'errorToastMessage' : 'successToastMessage';
    const toastElement = document.getElementById(toastId);
    const messageElement = document.getElementById(messageId);

    if (toastElement && messageElement && message) { // Só mostra se tiver mensagem
        messageElement.textContent = message;
        toastElement.style.display = 'block';
        toastElement.style.opacity = 1; // Garante visibilidade

        // Esconde a mensagem após 5 segundos com fade out
        setTimeout(() => {
            toastElement.style.transition = 'opacity 0.5s ease-out';
            toastElement.style.opacity = 0;
            setTimeout(() => {
                toastElement.style.display = 'none';
                toastElement.style.transition = ''; // Limpa a transição
            }, 500); // Tempo do fade out
        }, 5000); // Tempo que fica visível
    }
}

// Função auxiliar para escapar HTML (segurança básica)
function escapeHtml(unsafe) {
    if (!unsafe) return "";
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}


// --- Lógica Principal Executada Após Carregar a Página ---

document.addEventListener('DOMContentLoaded', () => {
    // 1. Verifica se há mensagens do C# (via TempData) para exibir
    //    As variáveis 'successMsgFromTempData' e 'errorMsgFromTempData' são definidas no .cshtml
    if (typeof successMsgFromTempData !== 'undefined' && successMsgFromTempData) {
        showToast(successMsgFromTempData, false);
    }
    if (typeof errorMsgFromTempData !== 'undefined' && errorMsgFromTempData) {
        showToast(errorMsgFromTempData, true);
    }

    // 2. Configura os listeners dos filtros <select>
    const statusFilter = document.getElementById('statusFilter');
    const tipoFilter = document.getElementById('tipoFilter');
    if (statusFilter) statusFilter.addEventListener('change', filterChamados);
    if (tipoFilter) tipoFilter.addEventListener('change', filterChamados);

    // 3. Executa a filtragem inicial (caso algum filtro já venha selecionado) e calcula stats
    filterChamados();

    // 4. Configura listeners para fechar modal
    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape') closeModal();
    });
    window.addEventListener('click', (event) => {
        // Fecha se clicar fora do conteúdo do modal
        if (event.target == document.getElementById('modalDetalhes')) closeModal();
    });
});