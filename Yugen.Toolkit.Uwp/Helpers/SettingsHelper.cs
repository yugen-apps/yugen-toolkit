using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Windows.Storage;

namespace Yugen.Toolkit.Uwp.Helpers
{
    public static class SettingsHelper
    {
        private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        
        public static void Write<T>(string key, T value, JsonTypeInfo<T> jsonTypeInfo)
        {
            var valueString = JsonSerializer.Serialize(value, jsonTypeInfo);
            LocalSettings.Values[key] = valueString;
        }

        public static T Read<T>(string key, JsonTypeInfo<T> jsonTypeInfo) => 
            LocalSettings.Values.TryGetValue(key, out var value)
                ? JsonSerializer.Deserialize<T>((string)value, jsonTypeInfo)
                : default;
    }
}