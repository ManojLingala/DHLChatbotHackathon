using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace AceBotApp.Forms
{
    public enum ParcelTypes
    {
        Envelop1,
        Box2,
        Box3,
        Box4,
        Box5,
        Box6,
        Box7
    }    

    [Serializable]
    public class FetchParcelTypeForm
    {
        public string name { get; set; }
        public ParcelTypes? parcelTypes { get; set; }

        public static IForm<FetchParcelTypeForm> BuildForm()
        {
            return new FormBuilder<FetchParcelTypeForm>()
               .Build();
        }


    }
}