using FakeID.Api;
using FakeID.Application.Handlers;
using FakeID.Application.Performers;
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

            builder.AddPerformer<IQueryPerformer<GetPersonasQuery, IEnumerable<GetPersonasQuery.Result>>, PersonaPerformers>();
            builder.AddPerformer<ICommandPerformer<CreatePersonaCommand>, PersonaPerformers>();
            builder.AddPerformer<ICommandPerformer<UpdatePersonaCommand>, PersonaPerformers>();
            builder.AddPerformer<ICommandPerformer<DeletePersonaCommand>, PersonaPerformers>();
        }
    }
}
