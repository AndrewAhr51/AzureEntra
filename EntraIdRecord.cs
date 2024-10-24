
using System.Text.Json.Serialization;

namespace EntraIdBL.Models
{
    public class EntraIdRecord
    {
        [JsonPropertyName("userPrincipalName")]
        public string UserPrincipalName { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("region")]
        public string Region { get; set; }
        [JsonPropertyName("jobTitle")]
        public string JobTitle { get; set; }
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }
        [JsonPropertyName("officeLocation")]
        public string OfficeLocation { get; set; }
        [JsonPropertyName("department")]
        public string Department { get; set; }
        [JsonPropertyName("manager")]
        public string Manager { get; set; }
    }
}
