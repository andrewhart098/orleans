using Orleans;
using System.Threading.Tasks;

namespace IoT.GrainInterfaces
{
    /// <summary>
    /// Grain interface IGrain1
    /// </summary>
    public interface IDeviceGrain : IGrain
    {
        Task SetTemperature(double value);
    }
}
