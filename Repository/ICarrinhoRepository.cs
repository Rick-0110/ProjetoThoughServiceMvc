using ToughService.Models;

    namespace ToughService.Repository
    {
        public interface ICarrinhoRepository
        {
            // Métodos que o CarrinhoController (novo) espera
            Task<List<ItemCarrinhoModel>> GetCarrinhoByUserIdAsync(string userId);
            Task<ItemCarrinhoModel> GetItemAsync(int produtoId, string userId);
            Task AddItemAsync(ItemCarrinhoModel item);
            Task UpdateItemAsync(ItemCarrinhoModel item);
            Task RemoveItemAsync(int produtoId, string userId);
            Task ClearCarrinhoAsync(string userId);
        }
    }