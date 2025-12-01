using AngleSharp.Dom;
using Bogus;
using Bunit;
using FakeID.Api;
using FakeID.Client.Blazor.Pages;
using FakeID.Client.Blazor.Test.Unit.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Moq;
using STrain;

namespace FakeID.Client.Blazor.Test.Unit.Pages
{
    public class HomeTests
    {
        private Mock<IRequestSender> _senderMock = null!;

        private IRenderedComponent<Home> CreateCUT()
        {
            var context = new BunitContext();

            _senderMock = new Mock<IRequestSender>();

            context.Services.AddSingleton(_ => _senderMock.Object);
            context.Services.AddSingleton(new LibraryConfiguration()
            {
                CollocatedJavaScriptQueryString = null,
            });
            context.Services.AddSingleton<IKeyCodeService>(new KeyCodeService());

            return context.Render<Home>();
        }

        [Fact(DisplayName = "[UNIT][HOM-001] - Add new client")]
        public async Task Home_Add_AddNewClient()
        {
            // Arrange
            var cut = CreateCUT();
            var name = new Faker().Random.String2(10);

            // Act
            await cut.Find(Selectors.ByTestId("btnAddNewClient")).ClickAsync();
            await cut.WaitForElementAsync("dlgClientEdit");
            cut.Find(Selectors.ByTestId("inpClientName")).SetInnerText(name);
            await cut.Find(Selectors.ByTestId("btnSave")).ClickAsync();

            // Assert
            _senderMock.VerifyCommand<CreateClientCommand>(c => c.Name.Equals(name));
        }
    }
}
