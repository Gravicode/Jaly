using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngkasaPura.Botsky.Helpers
{
    public class SampleData
    {
        public static List<TagData> GetTagDatas(string Code)
        {
            return TagData.GetDatas();
        }

        public static TagData GetTagByCode(string Code)
        {
            var data = TagData.GetDatas().Where(x => x.TagCode == Code);
            if(data!=null && data.Count() > 0)
            {
                return data.SingleOrDefault();
            }
            else
            {
                return null;
            }

        }
    }
    #region data shopping
    public class AirportProduct
    {
        public string IDProduct { get; set; }
        public string Name { get; set; }

        public string UrlImage { get; set; }

        public string ProductType { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public string Vendor { set; get; }
        public double Harga { get; set; }
        public static List<AirportProduct> GetProducts()
        {
            Random rnd = new Random();
            return new List<AirportProduct>()
            {
                   new AirportProduct  (){ IDProduct="P001", Name = "Kalung Antik", Description="Kalung dari pedalaman cirebon", Harga=200000, ProductType= "Kerajinan", Stock= rnd.Next(100,1000), UrlImage="https://angkasapurastorage.blob.core.windows.net/botsky/kalung.jpg", Vendor="AA Art"},
                   new AirportProduct  (){ IDProduct="P002", Name = "Frame Ukiran", Description="Frame ukiran jati dari jepara", Harga=10000000, ProductType= "Kerajinan", Stock= rnd.Next(2,10), UrlImage="https://angkasapurastorage.blob.core.windows.net/botsky/frameukiran.jpg", Vendor="Ame Art"},
                   new AirportProduct  (){ IDProduct = "P003", Name = "Motor Kayu", Description="Motor kayu dari pengrajin bogor", Harga=200000, ProductType= "Kerajinan", Stock= rnd.Next(10,100), UrlImage="https://angkasapurastorage.blob.core.windows.net/botsky/motorkayu.jpg", Vendor="Oji Art"},
                   new AirportProduct  (){ IDProduct = "P004", Name = "Perahu Kayu", Description="Perahu kayu indah dari pohon kurma", Harga=250000, ProductType= "Kerajinan", Stock= rnd.Next(100,1000), UrlImage="https://angkasapurastorage.blob.core.windows.net/botsky/perahu.jpg", Vendor="Omen Art"},
                   new AirportProduct  (){ IDProduct="P005", Name = "Tas Kayu", Description="Tas kayu antik dari ibu ibu pengrajin dari sulawesi", Harga=500000, ProductType= "Kerajinan", Stock= rnd.Next(100,1000), UrlImage="https://angkasapurastorage.blob.core.windows.net/botsky/taskayu.jpg", Vendor="Tono Art"},

            };
        }

        public static AirportProduct GetItemByID(string IDProd)
        {
            var datas = GetProducts().Where(x => x.IDProduct == IDProd).FirstOrDefault();
            return datas == null ? null : datas;

        }

    }
    #endregion

    #region cart data
    [Serializable]

    public class CartItem
    {
        public string KodeProduk { set; get; }
        public string ProductName { set; get; }
        public int Qty { set; get; }
        public double Price { set; get; }
        public double Total { set; get; }
    }
    [Serializable]

    public class Cart
    {
        public DateTime TanggalShopping { set; get; }
        public string NoShopping { set; get; }

        public string Nama { set; get; }

        public string Alamat { set; get; }

        public string Telpon { set; get; }

        public string Email { set; get; }

        public double Total { get; set; }
        public double Tax { get; set; }

        public List<CartItem> Items { set; get; }

    }
    #endregion
    #region rfid tracker
    [Serializable]
    public enum TagTypes { People, Things, Other }
    [Serializable]
    public class TagData
    {
        public string TagCode { get; set; }
        public string Location { set; get; }
        public TagTypes TagType { get; set; }
        public string Name { set; get; }

        public static List<TagData> GetDatas()
        {
            var datas = new List<TagData>();
            for (int i = 0; i < 10; i++)
            {
                datas.Add(new TagData() { TagCode = $"000{i}", Name = $"orang-{i}", Location = "Terminal - " + i, TagType = TagTypes.People });
            }
            return datas;
        }
    }

    #endregion
}