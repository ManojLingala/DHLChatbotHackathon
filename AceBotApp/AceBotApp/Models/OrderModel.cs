using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AceBotApp.Models
{
    public class Dimension
    {
        public double length { get; set; }
        public double width { get; set; }
        public int height { get; set; }
    }

    public class To
    {
        public string name { get; set; }
        public string address { get; set; }
        public string zip { get; set; }
    }

    public class From
    {
        public string name { get; set; }
        public string address { get; set; }
        public string zip { get; set; }
    }

    public class Address
    {
        public To to = new To();
        public From from = new From();
    }

    public class Notification
    {
        public object email { get; set; }
        public string text { get; set; }
    }

    public class Updates
    {
        public List<string> upd1 { get; set; }
        public List<string> upd2 { get; set; }
        public List<string> upd3 { get; set; }
        public List<string> upd4 { get; set; }
    }

    public class Tracking
    {
        public string code { get; set; }
        public string current_status { get; set; }
        public Notification notification = new Notification();
        public Updates updates = new Updates();
    }

    public class OrderModel
    {
        public long id { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public List<string> service { get; set; }
        public Dimension dimension = new Dimension();
        public string weight { get; set; }
        public Address address = new Address();
        public Tracking tracking = new Tracking();
    }
}