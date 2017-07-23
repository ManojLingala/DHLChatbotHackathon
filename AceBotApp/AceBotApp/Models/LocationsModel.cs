using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AceBotApp.Models
{
    [Serializable]
    public class LocationsModel
    {        public string address { get; set; }
        public string phone { get; set; }
        public List<string> openingHours { get; set; }
        public List<string> LatestDropOff { get; set; }
    }
}
