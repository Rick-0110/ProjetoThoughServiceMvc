function expandirDetalhes(id) {
    const card = document.querySelector(`.servico-card[data-id="${id}"]`);
    if (card) {
        const body = card.querySelector('.servico-card-body');
        body.classList.toggle('expanded');
    }
}