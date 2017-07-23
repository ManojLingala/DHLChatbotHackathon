using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AceBotApp.Forms;

namespace AceBotApp.Models
{
    public class ParcelTypeModel
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public static CreateParcelOrderForm orderForm { get; set; }

        public static ParcelTypeModel Parse(dynamic o)
        {
            try
            {
                return new ParcelTypeModel
                {
                    Name = o.name.ToString(),
                    ImageUrl = o.imageUrl.ToString()
                };
            }
            catch
            {
                throw new InvalidCastException("HotelQuery could not be read");

            }
        }
    }
}