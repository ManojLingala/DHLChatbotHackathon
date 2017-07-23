using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace AceBotApp.Forms
{
    [Serializable]
    public class FetchParcelStatusForm
    {
       [Prompt("Please enter your tracking or reference number")]
        public string TrackingNo { get; set; }

        public static IForm<FetchParcelStatusForm> BuildForm()
        {           
            return new FormBuilder<FetchParcelStatusForm>()
               .Field(nameof(FetchParcelStatusForm.TrackingNo))               
               .Build();
        }
    }
}
