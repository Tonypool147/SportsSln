using System.Text.Json;

namespace SportsStore.Infrastructure {

    // session data can only be stored as int,string or byte[]. As we want to store a list of objects we use an extemsion method to store the session objects in JSON format and covert to/from it when accessing it.
    public static class SessionExtensions {

        public static void SetJson(this ISession session, string key, object value) {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetJson<T>(this ISession session, string key) {
            var sessionData = session.GetString(key);
            return sessionData == null
                ? default(T) : JsonSerializer.Deserialize<T>(sessionData);
        }
    }
}
