using System.ComponentModel.DataAnnotations;

namespace FakeID.Client.Blazor.Models
{
    public class Persona
    {
        public Guid Id { get; init; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Role { get; set; }

        public bool IsNew { get; init; } = false;
    }
}
