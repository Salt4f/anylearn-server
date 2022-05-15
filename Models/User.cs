using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AnyLearnServer.Models
{
    public class User
    {
        public string? Name { get; set; }
        
        public string? Surname { get; set; }
        
        public bool Linkedin { get; set; }
        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Email { get; set; } = string.Empty;
        
        [JsonIgnore]
        public string? Password { get; set; }
        
        public string? Photo { get; set; }

        public string? Token { get; set; }
    }
}
