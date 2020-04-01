using Unity.NetCode;

namespace Sibz.NetCode
{
    public class ClientWorld : WorldBase<ClientSimulationSystemGroup>
    {
        protected ClientWorld(IWorldOptionsBase options) : base(options, ClientServerBootstrap.CreateClientWorld)
        {
        }
    }
}