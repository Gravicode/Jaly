using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace DKI.Bot.App.Helpers
{
    public class Effects
    {
        private static List<EffectItem> effectItems;

        public static List<EffectItem> Items
        {
            get {
                if (effectItems == null)
                {
                    HttpClient client = new HttpClient();
                    var hasil = client.GetAsync("http://artropica.azurewebsites.net/api/Images/GetEffects").GetAwaiter().GetResult();
                    if (hasil.IsSuccessStatusCode)
                    {
                        var datas = JsonConvert.DeserializeObject<List<EffectItem>>(hasil.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                        effectItems = datas;
                    }
                }
                return effectItems; }
          
        }

    }


    public class EffectItem
    {
        public string id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string description { get; set; }
    }

}