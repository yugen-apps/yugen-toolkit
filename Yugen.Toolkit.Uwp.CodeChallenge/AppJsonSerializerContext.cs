using System.Collections.Generic;
using System.Text.Json.Serialization;
using Yugen.Toolkit.Uwp.CodeChallenge.Model;

[JsonSourceGenerationOptions(
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
	PropertyNameCaseInsensitive = true,
	DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(List<ValueModel>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}