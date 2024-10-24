using System;
using System.Text.Json.Serialization;

namespace EntraIdBL.Models
{
    public class EntraIdGroup
    {
        [JsonPropertyName("region")]
        public string Region { get; set; }
        [JsonPropertyName("groupId")]
        public string GroupId { get; set; }
    }
}
