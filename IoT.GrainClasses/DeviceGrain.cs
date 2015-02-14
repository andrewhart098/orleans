using System;
using System.Threading.Tasks;
using IoT.GrainInterfaces;
using Orleans;
using Orleans.Providers;

namespace IoT.GrainClasses
{
    public interface IDeviceGrainState : IGrainState
    {
        double LastValue { get; set; }
    }

    [StorageProvider(ProviderName = "CustomStore")]
    public class DeviceGrain : Orleans.Grain<IDeviceGrainState>, IDeviceGrain
    {

        public override Task OnActivateAsync()
        {
            var id = this.GetPrimaryKeyLong();
            Console.WriteLine("Activated {0}", id);
            Console.WriteLine("Activated State: {0}", this.State.LastValue);
            return base.OnActivateAsync();
        }

        public async Task SetTemperature(double value)
        {
            if (this.State.LastValue < 100 && value >= 100)
            {
                Console.WriteLine("High temperature recorded of {0}", value);
            }
            if (this.State.LastValue != value)
            {
                this.State.LastValue = value;
                await this.State.WriteStateAsync();
            }
        }
    }
}
