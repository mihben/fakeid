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
                .UseFluentRequestValidator(builder => { });
        }
    }
}
