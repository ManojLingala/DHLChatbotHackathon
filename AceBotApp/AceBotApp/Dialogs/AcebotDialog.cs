using System;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis.Models;
using AceBotApp.Services;
using Microsoft.Bot.Builder.FormFlow;
using AceBotApp.Forms;
using global::AdaptiveCards;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using AceBotApp.Models;
using Microsoft.Bot.Connector;

namespace AceBotApp.Dialogs
{
    [LuisModel("55e242c5-1433-40e7-8d9b-fab55566bd50", "babf96aca30c42c9bd005506fc191046")]
    [Serializable]
    public class AcebotDialog : LuisDialog<object>
    {
        //private const string EntityParcelId = "ParcelId";
        private List<string> parcelOptions = new List<string> { "Envelop1","Box1","Box2","Box3","Box4","Box5","Box6","Box7" };

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";          
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);           
        }

        [LuisIntent("Tracking")]
        public async Task FetchParcelStatus(IDialogContext context, LuisResult result)
        {
            
            //var fetchParcelStatusDialog = new FormDialog<FetchParcelStatusForm>(new FetchParcelStatusForm(), FetchParcelStatusForm.BuildForm, FormOptions.PromptInStart, result.Entities);
            var fetchParcelStatusDialog = FormDialog.FromForm(FetchParcelStatusForm.BuildForm);            
            context.Call<FetchParcelStatusForm>(fetchParcelStatusDialog, ResumeAfterFetchStatusFormDialog);   
         
        }

        [LuisIntent("Location")]
        public async Task FetchDHLLocations(IDialogContext context, LuisResult result)
        { 
     
            var fetchParcelStatusDialog = FormDialog.FromForm(FetchDHLLocationsForm.BuildForm);
            context.Call<FetchDHLLocationsForm>(fetchParcelStatusDialog, ResumeAfterFetchDHLLocationsFormDialog);
        }

        [LuisIntent("Shipping")]
        public async Task CreateParcelOrder(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sure, let me help you with that, Shall we proceed?");
            //var fetchParcelStatusDialog = new FormDialog<FetchParcelStatusForm>(new FetchParcelStatusForm(), FetchParcelStatusForm.BuildForm, FormOptions.PromptInStart, result.Entities);
            var createParcelOrderDialog = FormDialog.FromForm(CreateParcelOrderForm.BuildForm);
            context.Call<CreateParcelOrderForm>(createParcelOrderDialog, ResumeAfterCreateParcelOrderFormDialog);

        }
        private async Task ResumeAfterFetchDHLLocationsFormDialog(IDialogContext context, IAwaitable<FetchDHLLocationsForm> result)
        {
            try
            {
                var message = context.MakeMessage();
                var statusQuery = await result;
                List<LocationsModel> status = AceBotService.GetDHLLocationsAsync(statusQuery.location);
                foreach (LocationsModel item in status)
                {
                    message.Attachments.Add(GetHeroCard(item));
                }
                await context.PostAsync(message);
            }
            catch (FormCanceledException ex)
            {
                string reply;
                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }
                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

        private async Task ResumeAfterFetchStatusFormDialog(IDialogContext context, IAwaitable<FetchParcelStatusForm> result)
        {
            try
            {
                var statusQuery = await result;
                var status = AceBotService.GetParcelAsync(statusQuery);
                await context.PostAsync($"The status of your application is : {status}");
            }
            catch (FormCanceledException ex)
            {
                string reply;
                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }
                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }
        private async Task ResumeAfterCreateParcelOrderFormDialog(IDialogContext context, IAwaitable<CreateParcelOrderForm> result)
        {
            try
            {
                var statusQuery = await result;
                ParcelTypeModel.orderForm = statusQuery;
                var Images = AceBotService.GetParcelImages();
                
                var title = $"Please select the parcel type";
                var intro = new List<CardElement>()
            {
                    new TextBlock()
                    {
                        Text = title,
                        Size = TextSize.ExtraLarge
                    }
            };

                var rows = Split(Images, 2)
                    .Select(group => new ColumnSet()
                    {
                        Columns = new List<Column>(group.Select(AsParcelTypeItem))
                    });

                var card = new AdaptiveCard()
                {
                    Body = intro.Union(rows).ToList()
                };

                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card
                };

                var reply = context.MakeMessage();
                reply.Attachments.Add(attachment);

                await context.PostAsync(reply);
                //var fetchParcelTypeDialog = new FormDialog<FetchParcelTypeForm>(new FetchParcelTypeForm(), FetchParcelTypeForm.BuildForm, FormOptions.None, null);
                //var fetchParcelTypeDialog = FormDialog.FromForm(FetchParcelTypeForm.BuildForm);
                //context.Call<FetchParcelTypeForm>(fetchParcelTypeDialog, ResumeAfterFetchParcelTypeFormDialog);

            }
            catch (FormCanceledException ex)
            {
                string reply;
                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }
                await context.PostAsync(reply);
            }

            finally
            {
                context.Done<object>(null);
            }
        }
        private async Task OnOptionSelected(IDialogContext context, IAwaitable<Image> result)
        {
            await context.PostAsync($"Parcel order placed");
        }
        public async Task DisplaySelectedCard(IDialogContext context, IAwaitable<string> result)
        {
            var selectedCard = await result;  
            await context.PostAsync(selectedCard);            
        }

        [LuisIntent("Utilities.Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi! Try asking me things like 'status please', dont ask me more now. I'm yet to be trained well.");
            context.Wait(this.MessageReceived);
        }
        [LuisIntent("Salutation")]
        public async Task Salutation(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi! How can i help you today?");
            context.Wait(this.MessageReceived);
        }

        private Column AsParcelTypeItem(ParcelTypeModel parcelType)
        {
            var submitActionData = JObject.Parse("{ \"Type\": \"ParcelTypeSelection\" }");
            submitActionData.Merge(JObject.FromObject(parcelType));

            return new Column()
            {
                Size = "40",
                Items = new List<CardElement>()
                {                  
                    new Image()
                    {
                        Size = ImageSize.Large,
                        Url = parcelType.ImageUrl
                    }
                }
                ,
                SelectAction = new SubmitAction()
                {
                    DataJson = submitActionData.ToString()
                }
            };
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> list, int parts)
        {
            return list.Select((item, ix) => new { ix, item })
                       .GroupBy(x => x.ix % parts)
                       .Select(x => x.Select(y => y.item));
        }
        private static Attachment GetHeroCard(LocationsModel locationsModel)
        {
            var heroCard = new HeroCard
            {
                Title = "Address: "+ Environment.NewLine + locationsModel.address,
                Subtitle = "Phone: "+ Environment.NewLine + locationsModel.phone,
                Text = "Opening housrs: "+ string.Join(",", locationsModel.openingHours) +Environment.NewLine+"Latest Drop Off: "+ string.Join(",", locationsModel.LatestDropOff)             
            };

            return heroCard.ToAttachment();
        }
        private async Task ResumeAfterFetchParcelTypeFormDialog(IDialogContext context, IAwaitable<FetchParcelTypeForm> result)
        {
            try
            {
                await context.PostAsync("order placed");

            }
            catch (FormCanceledException ex)
            {
                string reply;
                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }
                await context.PostAsync(reply);
            }

            finally
            {
                context.Done<object>(null);
            }
        }
    }
 }