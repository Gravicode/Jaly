using DKI.Bot.App.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace DKI.Bot.App.Dialogs
{
    [Serializable]
    public class PhotoDialog : IDialog<object>
    {
        private OutputCls ImageSel = null;
        private string StyleId { set; get; }
        private string StyleName { set; get; }
        public PhotoDialog(string EffectId, string StyleName)
        {
            this.StyleName = StyleName;
            this.StyleId = EffectId;
        }
        public async Task StartAsync(IDialogContext context)
        {
            this.ImageSel = null;
            await this.EnsurePhoto(context);
        }



        //minta user kirim gambar
        private async Task EnsurePhoto(IDialogContext context)
        {

            if (this.ImageSel == null)
            {
                PromptDialog.Attachment(context, this.PhotoReceivedAsync, "Tolong kirim gambar kamu yah.");
            }
            else
            {
                var text = $"Oke, konfirm ya photonya uda bener ?";

                PromptDialog.Confirm(context, this.PhotoConfirmedReceivedAsync, text);
            }
        }

        private async Task PhotoConfirmedReceivedAsync(IDialogContext context, IAwaitable<bool> argument)
        {

            var confirmed = await argument;

            if (confirmed)
            {
                var res = this.ImageSel;
                if (res == null)
                {
                    await context.PostAsync("proses gagal.");
                    context.Done<object>(null);
                    return;
                }
                Activity replyToConversation = context.MakeMessage() as Activity; //message.CreateReply("Should go to conversation, in list format");
                replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
                replyToConversation.Attachments = new List<Attachment>();
                //replyToConversation.ReplyToId = context.Activity.ReplyToId;
                Dictionary<string, string> cardContentList = new Dictionary<string, string>();

                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: res.Result.Url));

                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = res.Result.Url,
                    Type = "openUrl",
                    Title = "Open Image"
                };

                cardButtons.Add(plButton);

                ThumbnailCard plCard = new ThumbnailCard()
                {
                    Title = $"Ini foto kamu dengan effect : {this.StyleName}",
                    Images = cardImages,
                    Buttons = cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);

                await context.PostAsync(replyToConversation);
                context.Done<object>(null);
                //context.Call(new ReportDialog(this.PhotoUrl), this.ResumeAndEndDialogAsync);
            }
            else
            {
                await context.PostAsync("Ok proses dibatalin, karena kamu cancel.");
                context.Done<object>(null);
            }
        }
        private async Task ResumeAndEndDialogAsync(IDialogContext context, IAwaitable<object> argument)
        {
            context.Done<object>(null);
        }


        private async Task PhotoReceivedAsync(IDialogContext context, IAwaitable<IEnumerable<Attachment>> argument)
        {
            var message = await argument;

            if (message != null && message.Any())
            {
                var attachment = message.First();
                using (HttpClient httpClient = new HttpClient())
                {
                    // Skype & MS Teams attachment URLs are secured by a JwtToken, so we need to pass the token from our bot.
                    if ((context.Activity.ChannelId.Equals("skype", StringComparison.InvariantCultureIgnoreCase) || context.Activity.ChannelId.Equals("msteams", StringComparison.InvariantCultureIgnoreCase))
                        && new Uri(attachment.ContentUrl).Host.EndsWith("skype.com"))
                    {
                        var token = await new MicrosoftAppCredentials().GetTokenAsync();
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    var responseMessage = await httpClient.GetAsync(attachment.ContentUrl);

                    var contentLenghtBytes = responseMessage.Content.Headers.ContentLength;
                    var data = responseMessage.Content.ReadAsByteArrayAsync().Result;
                    var newname = Guid.NewGuid().ToString().Replace("-", "_");
                    var res = await Upload(this.StyleId, data);
                    this.ImageSel = res;

                }
            }


            await this.EnsurePhoto(context);
        }
        public static async Task<OutputCls> Upload(string styleid, byte[] image)
        {
            try
            {
                //var styleid = @"c7984b32-1560-11e7-afe2-06d95fe194ed";
                using (var client = new HttpClient())
                {
                    using (var content =
                        new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        content.Add(new StreamContent(new MemoryStream(image)), "bilddatei", "upload.jpg");

                        using (
                           var message =
                               await client.PostAsync($"http://artropica.azurewebsites.net/api/Images/ProcessImage?StyleId={styleid}&Username=anto", content))
                        {
                            var input = await message.Content.ReadAsStringAsync();
                            var hasil = JsonConvert.DeserializeObject<OutputCls>(input);
                            return hasil;
                            //return !string.IsNullOrWhiteSpace(input) ? Regex.Match(input, @"http://\w*\.directupload\.net/images/\d*/\w*\.[a-z]{3}").Value : null;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}