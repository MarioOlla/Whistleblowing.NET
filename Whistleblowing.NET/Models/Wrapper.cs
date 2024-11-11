using System.Text.Json.Serialization;

namespace Whistleblowing.NET.Models
{
    public class Wrapper
    {
        [JsonPropertyName("$id")]
        public string Id { get; set; }

        [JsonPropertyName("$values")]
        public List<PaginatedSegnalazioniRegularViewModel> Values { get; set; }

    }
}
