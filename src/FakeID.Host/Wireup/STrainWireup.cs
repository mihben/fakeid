using FakeID.Api;
using FakeID.Application.Handlers;
using STrain;
using STrain.CQS.NetCore.Builders;

namespace FakeID.Host.Wireup
{
    public static class STrainWireup
    {
        public static void AddCQS(CQSBuilder builder)
        {
            builder.AddGenericRequestHandler()
                .AddMvcRequestReceiver();

            builder.AddRequestValidator()
                .UseFluentRequestValidator(builder => builder.RegistrateFrom<CreateClientCommandValidator>());

            builder.AddPerformer<IQueryPerformer<GetClientsQuery, IEnumerable<GetClientsQuery.Result>>, ClientPerformers>();
            builder.AddPerformer<ICommandPerformer<CreateClientCommand>, ClientPerformers>();
            builder.AddPerformer<ICommandPerformer<UpdateClientCommand>, ClientPerformers>();
            builder.AddPerformer<ICommandPerformer<DeleteClientCommand>, ClientPerformers>();
        }
    }
}
