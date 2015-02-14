using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using System.IO;
using Newtonsoft.Json;

namespace IoT.FileStorage
{
    public class FileStorageProvider : IStorageProvider
    {
        public string Name { get; set; }
        public string directory;
        public Logger Log {get; set;}

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var fileInfo = GetFileInfo(grainType, grainReference);
            fileInfo.Delete();
            return TaskDone.Done;
        }

        public Task Close()
        {
            return TaskDone.Done;
        }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            this.Name = name;
            this.directory = config.Properties["directory"];
            return TaskDone.Done;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var fileInfo = GetFileInfo(grainType, grainReference);
            if (!fileInfo.Exists) return;

            using (var stream = fileInfo.OpenText())
            {
                var json = await stream.ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                grainState.SetAll(data);
            }
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var json = JsonConvert.SerializeObject(grainState.AsDictionary());
            var fileInfo = GetFileInfo(grainType, grainReference);
            using (var stream = fileInfo.OpenWrite())
            using (var writer = new StreamWriter(stream))
            {
                return writer.WriteAsync(json);
            }
        }

        FileInfo GetFileInfo(string graintype, GrainReference grainReference)
        {
            return new FileInfo(Path.Combine(directory, string.Format("{0}--{1}.json", graintype, grainReference.ToKeyString())));
        }
    }
}
