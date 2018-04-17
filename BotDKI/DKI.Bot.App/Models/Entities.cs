using DKI.Bot.App.Dialogs;
//using Microsoft.Azure.CosmosDB.Table;
//using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DKI.Bot.App.Models
{
    public class Entities
    {
    }

    public class Complain 
    {
        public Complain()
        {
           
        }

        public long Id { get; set; }
        public string PhotoUrl { get; set; }
        public string NoLaporan { set; get; }
        public DateTime TglLaporan { set; get; }
        public string Nama { set; get; }

        public string Telpon { set; get; }

        public string Email { set; get; }

        public TipeLaporan TipeLaporan { set; get; }

        public string Keterangan { set; get; }

        public string Lokasi { set; get; }

        public DateTime Waktu { set; get; }

        public int SkalaPrioritas { set; get; }
    }

    /*
    public class Complain : TableEntity
    {
        public Complain(string Username)
        {
            this.PartitionKey = "laporan";
            this.RowKey = Username;
        }

        public string Id { get; set; }
        public string PhotoUrl { get; set; }
        public string NoLaporan { set; get; }
        public DateTime TglLaporan { set; get; }
        public string Nama { set; get; }

        public string Telpon { set; get; }

        public string Email { set; get; }

        public string TipeLaporan { set; get; }

        public string Keterangan { set; get; }

        public string Lokasi { set; get; }

        public DateTime Waktu { set; get; }

        public int SkalaPrioritas { set; get; }
    }*/

}