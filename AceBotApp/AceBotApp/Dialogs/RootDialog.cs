namespace AceBotApp.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using AceBotApp.Dialogs;
    using System.Threading;
    using AceBotApp.Models;
    using AceBotApp.Services;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public async virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Value != null)
            {               
                dynamic value = message.Value;
                string submitType = value.Type.ToString();
                switch (submitType)
                {
                    case "ParcelTypeSelection":
                        ParcelTypeModel query;
                        try
                        {
                            query = ParcelTypeModel.Parse(value);
                            string orderid = await constructOrderJson(query);
                            await context.PostAsync($"Shipping order created successfully. Your orer id is {orderid}");                            
                        }
                        catch (InvalidCastException)
                        {                           
                            await context.PostAsync("Please complete all the search parameters");
                            return;
                        }                       

                        return;
                }
            }
            else
            {
               await context.Forward(new AcebotDialog(), ResumeAfterOptionDialog, message,CancellationToken.None);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task<string> constructOrderJson(ParcelTypeModel parcelTypeModel)
        {
            OrderModel orderModel = new OrderModel();
            orderModel.id = -1;
            orderModel.type = parcelTypeModel.Name.Split('.')[0];
            orderModel.description = parcelTypeModel.Name.Split('.')[0];
            orderModel.service = new List<string> { ParcelTypeModel.orderForm.mainService.ToString(),
                                                    ParcelTypeModel.orderForm.serviceSector.ToString(),
                                                    ParcelTypeModel.orderForm.deliveryOption.ToString()};
            orderModel.dimension.length = 10;
            orderModel.dimension.width =20;
            orderModel.dimension.height = 10;
            orderModel.weight = "10";
            orderModel.address.from.name = ParcelTypeModel.orderForm.name;
            orderModel.address.from.address = ParcelTypeModel.orderForm.FromAddress.FullAddress;
            orderModel.address.from.zip = ParcelTypeModel.orderForm.FromAddress.Zip;
            orderModel.address.to.name = ParcelTypeModel.orderForm.name;
            orderModel.address.to.address = ParcelTypeModel.orderForm.ToAddress.FullAddress;
            orderModel.address.to.zip = ParcelTypeModel.orderForm.ToAddress.Zip;
            orderModel.tracking.code = orderModel.id.ToString();
            orderModel.tracking.current_status = String.Empty;
            orderModel.tracking.notification.email = ParcelTypeModel.orderForm.FromAddress.email;
            orderModel.tracking.notification.text= ParcelTypeModel.orderForm.FromAddress.phoneNumber;
            orderModel.tracking.updates.upd1 = new List<string> { DateTime.Now.ToString(),
                                                                 orderModel.address.from.address,
                                                                 orderModel.tracking.current_status};
            return await AceBotService.CreateShippingOrderAsync(orderModel);
        }

    }
}
