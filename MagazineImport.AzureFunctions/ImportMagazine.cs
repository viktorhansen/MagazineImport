using System;
using System.Collections.Generic;
using System.IO;
using MagazineImport.Code.Importers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MagazineImport.AzureFunctions
{
    public class ImportMagazine
    {
        [FunctionName("Function1")]
        public void Run([BlobTrigger("samples-workitems/{name}", Connection = "")]Stream myBlob, string name, ILogger log)
        {
            var importers = new List<BaseMultiImporter>
                {
                    //Insert your importers here
                    new PrenaxImporter(),
                    //new MyNewCustomImporter(),
                };

            try
            {
                //Run jobs while result is true
                foreach (var import in importers)
                {
                    var success = import.Import();
                }
            }
            catch (Exception ex)
            {
            }
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
