
document.addEventListener('DOMContentLoaded', function () {
    const dateInput = document.getElementById('dataDesejada');
    if (dateInput) {
      
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        const minDate = tomorrow.toISOString().split('T')[0];
        dateInput.min = minDate;

    }
});

// ===== FUNÇÕES VISUAIS DO MODAL =====


function openModal(tipo) {
    const modal = document.getElementById('modalOrcamento');
    const titulo = document.getElementById('modalTitle');
    const tipoServicoInput = document.getElementById('tipoServicoHidden');


    const servicosInfo = {
        'manutencao': { titulo: 'Solicitar Orçamento - Manutenção', valor: 'Manutenção Preventiva' },
        'reparo': { titulo: 'Solicitar Orçamento - Reparo', valor: 'Reparo Técnico' },
        'instalacao': { titulo: 'Solicitar Orçamento - Instalação', valor: 'Instalação' },
        'suporte': { titulo: 'Solicitar Orçamento - Suporte', valor: 'Suporte Técnico' }
       
    };

    const info = servicosInfo[tipo] || { titulo: 'Solicitar Orçamento', valor: tipo }; 

  
    if (titulo) {
        titulo.textContent = info.titulo;
    }


    if (tipoServicoInput) {
        tipoServicoInput.value = info.valor;
    } else {
        console.error("Campo oculto 'tipoServicoHidden' não encontrado!"); 
    }

  
    if (modal) {
        modal.style.display = 'block';
        document.body.style.overflow = 'hidden'; 
    } else {
        console.error("Modal 'modalOrcamento' não encontrado.");
    }
}


function closeModal() {
    const modal = document.getElementById('modalOrcamento');
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto'; 
}

