namespace DKI.Bot.App.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Autofac;
    using Microsoft.Bot.Builder.ConnectorEx;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    //using Services;
    //using Util;
    using Newtonsoft.Json.Linq;

    [LuisModel("ca69a739-6d9c-4d0a-bab4-d41abc322d54", "52a0ca570af84ed5a7e770244223d4be", LuisApiVersion.V2, "westus.api.cognitive.microsoft.com", Staging = false)]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
      

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Maaf saya belum paham maksudnya: '{result.Query}'.\nCoba ketik 'tolong' untuk informasi tentang saya :)");
            context.Done<object>(null);
        }

        [LuisIntent("SayHello")]
        public async Task Hello(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello, ketemu lagi sama Bang Jaly.\n" +
                        "Ketik 'menu' untuk lihat apa aja yang abang bisa bantu.");
            context.Done<object>(null);
        }

        [LuisIntent("AskHelp")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Kenalin ya, nama ane ini Bang Jaly, ane siap melayani warga jakarte.\n" +
                        "Jika ente butuh informasi tentang jakarta, ane siap bantu. ketik 'menu' untuk lihat apa aja yang ane bisa bantu.");
            context.Done<object>(null);
        }

        [LuisIntent("AskCondition")]
        public async Task TanyaKabar(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Alhamdulillah ane baik dan sehat.\n" +
                        "Ente gimana ? semoga sehat-sehat ya. Kalau ada yang ane bisa bantu, coba aja ketik 'menu' ya");
            context.Done<object>(null);
        }

        [LuisIntent("HandOffToHuman")]
        public async Task HandOff(IDialogContext context, LuisResult result)
        {
            var conversationReference = context.Activity.ToConversationReference();
            var provider = Conversation.Container.Resolve<HandOff.Provider>();

            if (provider.QueueMe(conversationReference))
            {
                var waitingPeople = provider.Pending() > 1 ? $", ada { provider.Pending() - 1 } orang menunggu" : string.Empty;

                await context.PostAsync($"Ane akan hubungkan kamu dengan operator yang ada... tunggu sebentar {waitingPeople}.");
            }

            context.Done<object>(null);
        }

        [LuisIntent("AskMenu")]
        public async Task MenuShow(IDialogContext context, LuisResult result)
        {
            context.Call(new MenuDialog(), ResumeAndEndMenuDialogAsync);
        }

        private async Task ResumeAndEndMenuDialogAsync(IDialogContext context, IAwaitable<object> argument)
        {
            context.Done<object>(null);
        }
        /*
        [LuisIntent("SubmitTicket")]
        public async Task SubmitTicket(IDialogContext context, IAwaitable<IMessageActivity> messageActivity, LuisResult result)
        {
            EntityRecommendation categoryEntityRecommendation, severityEntityRecommendation;

            result.TryFindEntity("category", out categoryEntityRecommendation);
            result.TryFindEntity("severity", out severityEntityRecommendation);

            this.category = ((List<object>)categoryEntityRecommendation?.Resolution["values"])?[0]?.ToString();
            this.severity = ((List<object>)severityEntityRecommendation?.Resolution["values"])?[0]?.ToString();
            this.description = result.Query;

            await this.EnsureTicket(context);

            var activity = await messageActivity;
            await this.SendSearchToBackchannel(context, activity, this.description);
        }

        [LuisIntent("ExploreKnowledgeBase")]
        public async Task ExploreCategory(IDialogContext context, LuisResult result)
        {
            EntityRecommendation categoryEntityRecommendation;
            result.TryFindEntity("category", out categoryEntityRecommendation);
            var category = ((Newtonsoft.Json.Linq.JArray)categoryEntityRecommendation?.Resolution["values"])?[0]?.ToString();

            context.Call(new CategoryExplorerDialog(category, result.Query), ResumeAndEndDialogAsync);
        }

        private async Task ResumeAndEndDialogAsync(IDialogContext context, IAwaitable<object> argument)
        {
            context.Done<object>(null);
        }

        private async Task EnsureTicket(IDialogContext context)
        {
            if (this.severity == null)
            {
                var severities = new string[] { "high", "normal", "low" };
                PromptDialog.Choice(context, this.SeverityMessageReceivedAsync, severities, "Which is the severity of this problem?");
            }
            else if (this.category == null)
            {
                PromptDialog.Text(context, this.CategoryMessageReceivedAsync, "Which would be the category for this ticket (software, hardware, networking, security or other)?");
            }
            else
            {
                var text = $"Great! I'm going to create a \"{this.severity}\" severity ticket in the \"{this.category}\" category. " +
                       $"The description I will use is \"{this.description}\". Can you please confirm that this information is correct?";

                PromptDialog.Confirm(context, this.IssueConfirmedMessageReceivedAsync, text);
            }
        }

        private async Task SeverityMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.severity = await argument;
            await this.EnsureTicket(context);
        }

        private async Task CategoryMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            this.category = await argument;
            await this.EnsureTicket(context);
        }

        private async Task IssueConfirmedMessageReceivedAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirmed = await argument;

            if (confirmed)
            {
                var api = new TicketAPIClient();
                var ticketId = await api.PostTicketAsync(this.category, this.severity, this.description);

                if (ticketId != -1)
                {
                    var message = context.MakeMessage();
                    message.Attachments = new List<Attachment>
                    {
                        new Attachment
                        {
                            ContentType = "application/vnd.microsoft.card.adaptive",
                            Content = this.CreateCard(ticketId, this.category, this.severity, this.description)
                        }
                    };
                    await context.PostAsync(message);
                }
                else
                {
                    await context.PostAsync("Ooops! Something went wrong while I was saving your ticket. Please try again later.");
                }

                context.Call(new UserFeedbackRequestDialog(), this.ResumeAndEndDialogAsync);
            }
            else
            {
                await context.PostAsync("Ok. The ticket was not created. You can start again if you want.");
                context.Done<object>(null);
            }
        }

        private async Task SendSearchToBackchannel(IDialogContext context, IMessageActivity activity, string textSearch)
        {
            var searchService = new AzureSearchService();
            var searchResult = await searchService.Search(textSearch);
            if (searchResult != null && searchResult.Value.Length != 0)
            {
                var reply = ((Activity)activity).CreateReply();

                reply.Type = ActivityTypes.Event;
                reply.Name = "searchResults";
                reply.Value = searchResult.Value;
                await context.PostAsync(reply);
            }
        }

        private AdaptiveCard CreateCard(int ticketId, string category, string severity, string description)
        {
            AdaptiveCard card = new AdaptiveCard();

            var headerBlock = new TextBlock()
            {
                Text = $"Ticket #{ticketId}",
                Weight = TextWeight.Bolder,
                Size = TextSize.Large,
                Speak = $"<s>You've created a new Ticket #{ticketId}</s><s>We will contact you soon.</s>"
            };

            var columnsBlock = new ColumnSet()
            {
                Separation = SeparationStyle.Strong,
                Columns = new List<Column>
                {
                    new Column
                    {
                        Size = "1",
                        Items = new List<CardElement>
                        {
                            new FactSet
                            {
                                Facts = new List<AdaptiveCards.Fact>
                                {
                                    new AdaptiveCards.Fact("Severity:", severity),
                                    new AdaptiveCards.Fact("Category:", category),
                                }
                            }
                        }
                    },
                    new Column
                    {
                        Size = "auto",
                        Items = new List<CardElement>
                        {
                            new Image
                            {
                                Url = "https://raw.githubusercontent.com/GeekTrainer/help-desk-bot-lab/master/assets/botimages/head-smiling-medium.png",
                                Size = ImageSize.Small,
                                HorizontalAlignment = HorizontalAlignment.Right
                            }
                        }
                    }
                }
            };

            var descriptionBlock = new TextBlock
            {
                Text = description,
                Wrap = true
            };

            card.Body.Add(headerBlock);
            card.Body.Add(columnsBlock);
            card.Body.Add(descriptionBlock);

            return card;
        }*/
    }
}