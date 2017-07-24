using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace AceBotApp.Forms
{

    [Serializable]
    public class FetchDHLLocationsForm
    {
        [Prompt("Please enter your city")]
        public string location { get; set; }        

        public static IForm<FetchDHLLocationsForm> BuildForm()
        {
            return new FormBuilder<FetchDHLLocationsForm>()             
               .Build();
        }


    }
}