using System.Text.Json.Serialization;

namespace ItemsApi.Models;

public class Item
{
    [JsonPropertyName("id")]
    [Newtonsoft.Json.JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}

