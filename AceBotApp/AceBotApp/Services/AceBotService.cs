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

namespace AceBotApp.Services
{
    public static class AceBotService
    {
        public static string GetParcelAsync(FetchParcelStatusForm statusQuery)
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString("http://localhost:1111/parcelinfo/" + statusQuery.TrackingNo);
                var serializer = new JavaScriptSerializer();
                OrderModel model = serializer.Deserialize<OrderModel>(json);
                return model.tracking.current_status.ToString();
            }

        }

        public static List<LocationsModel> GetDHLLocationsAsync(string location)
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString("http://localhost:1111/serviceLocations/" + location);
                var serializer = new JavaScriptSerializer();
                List<LocationsModel> model = serializer.Deserialize<List<LocationsModel>>(json);
                return model;
            }
        }

        public static async Task<string> CreateShippingOrderAsync(OrderModel orderModel)
        {
            var json = new JavaScriptSerializer().Serialize(orderModel);
            string requestUri = "http://localhost:1111/createParcel";
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("http://localhost:1111/");
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    StringContent content = new StringContent(json);
            //    HttpResponseMessage response = await client.PostAsync("createParcel", content);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        return await response.Content.ReadAsStringAsync();                    
            //    }
            //}
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
            var images = new List<ParcelTypeModel>();
            string directory = @".\Images\";
            DirectoryInfo di = new DirectoryInfo(directory);    
            foreach (var item in di.GetFiles("*.png"))
            {
                ParcelTypeModel parcel = new ParcelTypeModel()
                {
                    Name = item.Name,
                    ImageUrl = item.FullName
                };

                images.Add(parcel);
            }
            return images;
        }


    }
}