using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ApprovalSystem.Types;

namespace ApprovalSystem.Interfaces
{
    public static class ExtendableObjectExtensions
    {
        public static T GetData<T>(this IExtendableEntity extendableObject, string name, bool handleType = false)
        {
            CheckNotNull(extendableObject, name);

            if (extendableObject == null)
            {
                throw new ArgumentNullException(nameof(extendableObject));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return extendableObject.GetData<T>(
                name,
                handleType
                    ? new JsonSerializer { TypeNameHandling = TypeNameHandling.All }
                    : JsonSerializer.CreateDefault()
            );
        }

        public static T GetData<T>(this IExtendableEntity extendableObject, string name, JsonSerializer jsonSerializer)
        {
            CheckNotNull(extendableObject, name);

            if (extendableObject.ExtensionProperty == null)
            {
                return default(T);
            }

            var json = JObject.Parse(extendableObject.ExtensionProperty);

            var prop = json[name];
            if (prop == null)
            {
                return default(T);
            }

            if (TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(T)))
            {
                return prop.Value<T>();
            }
            else
            {
                return (T)prop.ToObject(typeof(T), jsonSerializer ?? JsonSerializer.CreateDefault());
            }
        }

        public static void SetData<T>(this IExtendableEntity extendableObject, string name, T value, bool handleType = false)
        {
            extendableObject.SetData(
                name,
                value,
                handleType
                    ? new JsonSerializer { TypeNameHandling = TypeNameHandling.All }
                    : JsonSerializer.CreateDefault()
            );
        }

        public static void SetData<T>(this IExtendableEntity extendableObject, string name, T value, JsonSerializer jsonSerializer)
        {
            CheckNotNull(extendableObject, name);

            if (jsonSerializer == null)
            {
                jsonSerializer = JsonSerializer.CreateDefault();
            }

            if (extendableObject.ExtensionProperty == null)
            {
                if (EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    return;
                }

                extendableObject.ExtensionProperty = "{}";
            }

            var json = JObject.Parse(extendableObject.ExtensionProperty);

            if (value == null || EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                if (json[name] != null)
                {
                    json.Remove(name);
                }
            }
            else if (TypeHelper.IsPrimitiveExtendedIncludingNullable(value.GetType()))
            {
                json[name] = new JValue(value);
            }
            else
            {
                json[name] = JToken.FromObject(value, jsonSerializer);
            }

            var data = json.ToString(Formatting.None);
            if (data == "{}")
            {
                data = null;
            }

            extendableObject.ExtensionProperty = data;
        }

        public static bool RemoveData(this IExtendableEntity extendableObject, string name)
        {
            CheckNotNull(extendableObject, name);

            if (extendableObject.ExtensionProperty == null)
            {
                return false;
            }

            var json = JObject.Parse(extendableObject.ExtensionProperty);

            var token = json[name];
            if (token == null)
            {
                return false;
            }

            json.Remove(name);

            var data = json.ToString(Formatting.None);
            if (data == "{}")
            {
                data = null;
            }

            extendableObject.ExtensionProperty = data;

            return true;
        }

        private static void CheckNotNull(params object[] values)
        {
            foreach (var value in values)
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
            }
        }
    }
}
