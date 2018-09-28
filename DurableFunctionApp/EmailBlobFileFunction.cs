using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DurableFunctionApp
{
    public static class EmailBlobFileFunction
    {
        [FunctionName("EmailBlobFileFunction")]
        public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            myBlob.Position = 0;
            string blobData = string.Empty;
            using (StreamReader reader = new StreamReader(myBlob, Encoding.UTF8))
            {
                blobData = reader.ReadToEnd();
            }

             SendEmail(blobData);

            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }

        private static async Task  SendEmail(string blobData)
        {
            //Declare Mail object
            var msg = new SendGridMessage();

            msg.SetFrom(new EmailAddress("AIPSendGrid@azure.com", "SendGrid AIP Team"));

            var recipients = new List<EmailAddress>
            {
                 new EmailAddress("sarfrazmalik@tfl.gov.uk", "Sarfraz Malik"),
                new EmailAddress("Sarfraz.Malik@Gmail.com", "Sarfraz Malik Gmail"),
            };
            msg.AddTos(recipients);

            msg.SetSubject("A New Blob file was added");

            msg.AddContent(MimeType.Text, "The blob file data is " + blobData);

            var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var client = new SendGridClient(apiKey);

            // Send
            var response = await client.SendEmailAsync(msg);

            
        }
    }
}
