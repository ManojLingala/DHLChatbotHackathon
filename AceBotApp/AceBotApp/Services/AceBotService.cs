using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Script.Serialization;
using AceBotApp.Models;
using System.Threading.Tasks;
using AceBotApp.Forms;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Web.Configuration;




namespace AceBotApp.Services
{
    public static class AceBotService
    {
        public static string dhlDomainManagerBaseAddress = WebConfigurationManager.AppSettings["DHLDomainManagerBaseAddress"].ToString();
        public static string GetParcelAsync(FetchParcelStatusForm statusQuery)
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString(dhlDomainManagerBaseAddress + "parcelinfo/" + statusQuery.TrackingNo);
                var serializer = new JavaScriptSerializer();
                OrderModel model = serializer.Deserialize<OrderModel>(json);
                return model.tracking.current_status.ToString();
            }

        }

        public static List<LocationsModel> GetDHLLocationsAsync(string location)
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString(dhlDomainManagerBaseAddress + "serviceLocations/" + location);
                var serializer = new JavaScriptSerializer();
                List<LocationsModel> model = serializer.Deserialize<List<LocationsModel>>(json);
                return model;
            }
        }

        public static async Task<string> CreateShippingOrderAsync(OrderModel orderModel)
        {
            var json = new JavaScriptSerializer().Serialize(orderModel);
            string requestUri = dhlDomainManagerBaseAddress+"createParcel";
            using (HttpClient client = new HttpClient())
            {
                var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await client.PostAsync(requestUri, httpContent))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
            }
        }

        public static IEnumerable<ParcelTypeModel> GetParcelImages()
        {
            string baseUrl = "http://52.171.59.183/Images/";
            var images = new List<ParcelTypeModel>() {  new ParcelTypeModel()
                {
                    Name = "Envelope1",
                    ImageUrl = baseUrl+"Envelope1.png"
                }};
            for (int i = 2; i < 8; i++)
            {
                images.Add(new ParcelTypeModel()
                {
                    Name = "Box" + i.ToString(),
                    ImageUrl = baseUrl + "Box" + i.ToString() + ".png"
                });
            }

            return images;
        }


    }
}