using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DKI.Bot.App.Dialogs
{
    [Serializable]
    public class PhotoArtDialog : IDialog<object>
    {

        public PhotoArtDialog()
        {

        }
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Silakan pilih efek gambar ya.");
            Activity replyToConversation = context.MakeMessage() as Activity; //message.CreateReply("Should go to conversation, in list format");
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
            replyToConversation.Attachments = new List<Attachment>();
            //replyToConversation.ReplyToId = context.Activity.ReplyToId;
            Dictionary<string, string> cardContentList = new Dictionary<string, string>();
            foreach (var item in Helpers.Effects.Items)
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: item.url));

                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = item.title,
                    Type = "postBack",
                    Title = "Select"
                };

                cardButtons.Add(plButton);

                ThumbnailCard plCard = new ThumbnailCard()
                {
                    Title = $"{item.title}",
                    Images = cardImages,
                    Buttons = cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
            }
            await context.PostAsync(replyToConversation);

            var OptionFormDialog = FormDialog.FromForm<FiturPhoto>(FiturPhoto.BuildForm, FormOptions.PromptInStart);
            context.Call(OptionFormDialog, this.ResumeAfterReportFormDialog);
        }
        private async Task ResumeAndEndMenuDialogAsync(IDialogContext context, IAwaitable<object> argument)
        {
            context.Done<object>(null);
        }
        private async Task ResumeAfterReportFormDialog(IDialogContext context, IAwaitable<FiturPhoto> result)
        {
            try
            {
                var state = await result;
                context.Call(new PhotoDialog(state.EffectId, state.EffectName), this.ResumeAndEndMenuDialogAsync);
                //do nothing
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "Anda membatalkan, dialog ditutup.";
                }
                else
                {
                    reply = $"Ada masalah teknis euy:( Detailnya: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
                context.Done<object>(null);
            }

        }

    }

    [Serializable]
    public class FiturPhoto
    {
        public DateTime CreatedDate;
        public string EffectId;
        [Prompt("Pilih Effect Photo ya... {||}")]
        public string EffectName;



        public static IForm<FiturPhoto> BuildForm()
        {

            OnCompletionAsyncDelegate<FiturPhoto> processReport = async (context, state) =>
            {
                await Task.Run(() =>
                {
                    state.CreatedDate = DateTime.Now;
                    var selEffectId = from x in Helpers.Effects.Items
                                      where x.title == state.EffectName
                                      select x;
                    if (selEffectId != null && selEffectId.Count() > 0)
                    {
                        state.EffectId = selEffectId.SingleOrDefault().id;
                    }
                    var pesan = $"Effect yang dipilih : {state.EffectName}.";
                    context.PostAsync(pesan);
                }
                );

            };
            var builder = new FormBuilder<FiturPhoto>(false);
            var form = builder
                    .Field(nameof(EffectName), validate:
                            async (state, value) =>
                            {
                                var result = new ValidateResult { IsValid = true, Value = value, Feedback = "ok, effect valid" };
                                var sel = from x in Helpers.Effects.Items
                                          where x.title == value.ToString()
                                          select x;

                                if (sel == null || sel.Count() <= 0)
                                {

                                    result.Feedback = "pilih effect gambar yang benar";
                                    result.IsValid = false;

                                }
                                return result;
                            })

                        .OnCompletion(processReport)
                        .Build();
            return form;
        }
    }

}