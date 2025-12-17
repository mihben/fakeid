using Microsoft.AspNetCore.Components.Forms;

namespace FakeID.Client.Blazor.Extensions
{
    public static class EditContextExtensions
    {
        public static bool IsValid(this EditContext? context)
        {
            return context?.GetValidationMessages().Any() == true;
        }
    }
}
