using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text.Json;


namespace ToughService.Extensions
{
    public static class SessionExtensions
    {
        // Método para SALVAR um objeto na sessão
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            // CORREÇÃO: Usando o nome completo e explícito
            var jsonString = System.Text.Json.JsonSerializer.Serialize(value);
            session.SetString(key, jsonString);
        }

        // Método para LER um objeto da sessão
        public static T GetObject<T>(this ISession session, string key)
        {
            var jsonString = session.GetString(key);

            if (string.IsNullOrEmpty(jsonString))
            {
                return default(T);
            }

            // CORREÇÃO: Usando o nome completo e explícito
            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
