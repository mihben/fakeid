using System.ComponentModel.DataAnnotations;

namespace FakeID.Client.Blazor.Models
{
    public record Client
    {
        public Guid Id { get; init; }
        [Required(AllowEmptyStrings = false)]
        public string? Name { get; set; }
    }
}
