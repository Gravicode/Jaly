using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DKI.Bot.Entities
{
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
    public enum TipeLaporan
    {
        
        Sanitation = 1,
       
        Violation,
      
        RoadDamage,
      
        PublicFacility,
      
        IllegalParking
            ,
       
        IllegalHawker
            ,
       
        IlegalAds
            ,
    
        BrokenStreetlight
            ,
     
        IllegalCharges
            ,
       
        BlackCampaign
            ,
       
        TrafficAccident
            ,
     
        HouseHolds
            ,
       
        PublicTransport
            ,
    
        NoSmoking
            ,
      
        FallenTree
            ,
       
        TrafficJam
            ,
       
        Flood
            ,
       
        FloodControl
            ,
      
        Beggar
            ,
       
        IllegalDrugs
            ,
      
        Criminal
            ,
        
        BuildingPermit
            ,
      
        Fire
            ,
       
        RPTRA
            ,
     
        FoggingDBD
            ,
      
        UnhigyenicFood
            ,
      
        KostTax
            ,
       
        Terrorist
            ,
      
        Dementia

    }
}
