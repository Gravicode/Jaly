namespace DKI.Bot.App.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using DKI.Bot.App.Helpers;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using DKI.Bot.App.Services;

    [Serializable]
    public class MenuDialog : IDialog<object>
    {
        private string PhotoUrl;
     
        const string LaporanOption = "Saya ingin melaporkan keluhan";
        const string PublicFacilityOption = "Informasi fasilitas umum";
        const string PhoneNumberOption = "Mencari nomor penting";
        const string LocationInfoOption = "Info tentang lokasi (restoran)";
        const string LatestNewsOption = "Berita terbaru";
        const string ReportStatusOption = "Status laporan";
        const string CityAppOption = "Aplikasi-aplikasi jakarta smart city";
        const string ThirdPartyOption = "Service pihak ketiga";
        const string FAQOption = "FAQ (Pertanyaan yang sering ditanyakan)";
        public async Task StartAsync(IDialogContext context)
        {
            this.ShowOptions(context);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.ToLower().Contains("tolong") || message.Text.ToLower().Contains("bantuan") || message.Text.ToLower().Contains("help") || message.Text.ToLower().Contains("support") || message.Text.ToLower().Contains("problem"))
            {
                await context.Forward(new SupportDialog(), this.ResumeAfterSupportDialog, message, CancellationToken.None);
            }
            else
            {
                this.ShowOptions(context);
            }
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { LaporanOption, PublicFacilityOption, PhoneNumberOption, LocationInfoOption, LatestNewsOption, ReportStatusOption, CityAppOption, ThirdPartyOption, FAQOption }, "Abang bisa bantu hal-hal berikut. Silakan pilih ya", "Silakan pilih kembali.", 9);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case LaporanOption:

                        this.PhotoUrl = "";

                        await this.EnsurePhoto(context);
                        //context.Call(new ReportDialog(this.PhotoUrl), this.ResumeAfterOptionDialog);
                        break;

                    case PublicFacilityOption:
                    //context.Call(new FlightDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    case FAQOption:
                    //context.Call(new FAQDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    case LocationInfoOption:
                    //context.Call(new FacilityDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    case LatestNewsOption:
                    //context.Call(new ImportantNoDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    case PhoneNumberOption:
                    //context.Call(new LuggageDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    case ReportStatusOption:
                    //context.Call(new APTVDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    case CityAppOption:
                    //context.Call(new NewsDialog(), this.ResumeAfterOptionDialog);
                    //break;

                    case ThirdPartyOption:
                        //context.Call(new OtherDialog(), this.ResumeAfterOptionDialog);
                        await context.PostAsync("Maaf abang belum bisa bantu, sabar ya.");
                        context.Done<object>(null);
                        break;


                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync(MESSAGESINFO.TOO_MANY_ATTEMPT);

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            var ticketNumber = await result;

            await context.PostAsync($"Thanks for contacting our support team. Your ticket number is {ticketNumber}.");
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
                await context.PostAsync($"Thank you for using our services, if you don't get any result, please try again.");
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        //minta user kirim gambar
        private async Task EnsurePhoto(IDialogContext context)
        {

            if (string.IsNullOrEmpty(this.PhotoUrl))
            {
                PromptDialog.Attachment(context, this.PhotoReceivedAsync, "Abang butuh gambar bukti, tolong kirim kesini yah.");
            }
            else
            {
                var text = $"Oke, abang uda terima photonya nih, konfirm ya photonya uda bener ?";

                PromptDialog.Confirm(context, this.PhotoConfirmedReceivedAsync, text);
            }
        }

        private async Task PhotoConfirmedReceivedAsync(IDialogContext context, IAwaitable<bool> argument)
        {

            var confirmed = await argument;

            if (confirmed)
            {
                context.Call(new ReportDialog(this.PhotoUrl), this.ResumeAndEndDialogAsync);
            }
            else
            {
                await context.PostAsync("Ok laporan dibatalin karena kamu ga konfirm, silakan coba lagi ya kalau ingin melapor.");
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
                    this.PhotoUrl = ReportData.UploadPhoto(data, newname + ".jpg");
                    //save to azure blob
                    //await context.PostAsync($"Attachment of {attachment.ContentType} type and size of {contentLenghtBytes} bytes received.");
                }
            }


            await this.EnsurePhoto(context);
        }

    }
}