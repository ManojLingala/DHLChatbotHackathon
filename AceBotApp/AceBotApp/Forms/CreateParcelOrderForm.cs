using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using System.Drawing;

namespace AceBotApp.Forms
{
    
    public enum ServiceTypeDeliveryOptions
    {        
        Express,     
        Normal
    }
    public enum ServiceTypeOptions
    {
        DropOff,
        PickUp,
        Online,       
        DoorToDoor,       
    }

    public enum ServiceTypeSector
    { 
        International,
        Domestic
    }

    [Serializable]
    public class SourceAddress
    {
        public string FullAddress { get; set; }
        public string Zip { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }

    }

    [Serializable]
    public class DestinationAddress
    {
        [Prompt("Please enter {&} of destination")]
        public string FullAddress { get; set; }
        [Prompt("Please enter {&} of destination")]
        public string Zip { get; set; }
    }

    [Serializable]
    public class CreateParcelOrderForm
    {
        [Prompt("May i know your name please?")]
        public string name { get; set; }
        public SourceAddress FromAddress { get; set; }
        public ServiceTypeSector? serviceSector { get; set; }
        public ServiceTypeOptions? mainService { get; set; }
        public ServiceTypeDeliveryOptions? deliveryOption { get; set; } 
        public DestinationAddress ToAddress { get; set; }

        public static IForm<CreateParcelOrderForm> BuildForm()
        {
            return new FormBuilder<CreateParcelOrderForm>()               
               .Build();
        }

        
    }
}