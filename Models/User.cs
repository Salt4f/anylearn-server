using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyLearnServer.Models
{
    public class User
    {
        public string? Name { get; set; }
        
        public string? Surname { get; set; }
        
        public bool Linkedin { get; set; }
        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Email { get; set; } = string.Empty;
        
        public string? Password { get; set; }
        
        public string? Photo { get; set; }
    }
}
