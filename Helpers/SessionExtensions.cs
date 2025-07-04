using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

// Classe estática com métodos de extensão para a sessão
public static class SessionExtensions
{
    // Método para armazenar um objeto genérico na sessão como string JSON
    public static void SetObject<T>(this ISession session, string key, T value)
    {
       
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    // Método para recuperar um objeto genérico da sessão a partir da chave
    public static T GetObject<T>(this ISession session, string key)
    {
        // Obtém a string JSON da sessão
        var value = session.GetString(key);

        // Se não existir valor, retorna o valor padrão (null para classes)
        // Caso contrário, desserializa o JSON para o objeto do tipo T
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}
