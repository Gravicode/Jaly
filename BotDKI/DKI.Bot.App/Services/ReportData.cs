using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DKI.Bot.App.Dialogs;
using DKI.Bot.App.Helpers;
using DKI.Bot.App.Models;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
//using Microsoft.Azure.CosmosDB.Table;
//using Microsoft.Azure.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace DKI.Bot.App.Services
{
    public class ReportData
    {
        public static async Task<bool> InsertData(Complain data)
        {
            try
            {
                /*
                  // Retrieve the storage account from the connection string.
                  var storageAccount = CloudStorageAccount.Parse(
             ConfigurationManager.AppSettings["StorageConnectionString"]);

                  // Create the table client.
                  CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                  // Retrieve a reference to the table.
                  CloudTable table = tableClient.GetTableReference("laporan");

                  // Create the table if it doesn't exist.
                  table.CreateIfNotExists();
                  // Create the TableOperation object that inserts the customer entity.
                  TableOperation insertOperation = TableOperation.Insert(data);

                  // Execute the insert operation.
                  table.Execute(insertOperation);*/
                var db = ObjectContainer.Get<RedisDB>();
                data.Id = db.GetId<Complain>();
                var res = await db.InsertDoc<Complain>("complain",data);
                return res;
                
            }
            catch(Exception ex) {
                Console.WriteLine(ex.StackTrace+" : "+ex.Message);
                return false;
            }
            
        }

        public static string UploadPhoto(byte[] data, string filename)
        {
            var UrlPrefix = ConfigurationManager.AppSettings["UrlPrefix"];
            // Parse the connection string and return a reference to the storage account.
            var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            var container = blobClient.GetContainerReference("laporan");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            // Retrieve reference to a blob named "myblob".
            var fname = DateTime.Now.ToString("yyyyMMdd") + "/" + Guid.NewGuid().ToString().Replace("-", "_") + Path.GetExtension(filename);
            var fUrl = UrlPrefix + fname;
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fname);

            // Create or overwrite the "myblob" blob with contents from a local file.
           
            // Create or overwrite the "myblob" blob with contents from a local file.
            blockBlob.UploadFromByteArray(data, 0, data.Length);

            return fUrl;

        }
    }

  
}