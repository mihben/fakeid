namespace FakeID.Client.Blazor.Test.Unit.Helpers
{
    public static class Selectors
    {
        public static string ByTestId(string testId)
        {
            return $"[data-testId={testId}]";
        }
    }
}
