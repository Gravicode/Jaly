using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Builder.Resource;
using System.Resources;
using System.Text;
using System.Threading;
using Microsoft.Azure; // Namespace for CloudConfigurationManager

using System.Configuration;
using Newtonsoft.Json;
using DKI.Bot.App.Services;
using DKI.Bot.App.Models;
using DKI.Bot.App.Helpers;

namespace DKI.Bot.App.Dialogs
{
    [Serializable]
    public class ReportDialog : IDialog<object>
    {
        public string PhotoUrl { get; set; }
        public ReportDialog(string PhotoUrl)
        {
            this.PhotoUrl = PhotoUrl;
        }
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Sekarang tolong isi form keluhannya ya.");
            var ReportFormDialog = FormDialog.FromForm<Laporan>(Laporan.BuildForm, FormOptions.PromptInStart);
            context.Call(ReportFormDialog, this.ResumeAfterReportFormDialog);
        }
        private async Task ResumeAfterReportFormDialog(IDialogContext context, IAwaitable<Laporan> result)
        {
            try
            {
                var state = await result;
                //context.Activity.From.Name
                Complain com = new Complain() { Email = state.Email, Keterangan = state.Keterangan, Lokasi = state.Lokasi, Nama = state.Nama, NoLaporan = state.NoLaporan, SkalaPrioritas = state.SkalaPrioritas, Telpon = state.Telpon, TglLaporan = state.TglLaporan, TipeLaporan = state.TipeLaporan, Waktu = state.Waktu, PhotoUrl = this.PhotoUrl };
                await ReportData.InsertData(com);

                //do nothing
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "Anda membatalkan laporan, dialog ditutup.";
                }
                else
                {
                    reply = $"Ada masalah teknis euy:( Detailnya: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

    }
    public enum TipeLaporan
    {
        [Terms("Sanitasi", "Sanitation")]
        Sanitation = 1,
        [Terms("Pelanggaran", "Violation")]
        Violation,
        [Terms("Jalan Rusak", "RoadDamage")]
        RoadDamage,
        [Terms("Fasilitas Publik", "PublicFacility")]
        PublicFacility,
        [Terms("Parkir Ilegal", "IllegalParking")]
        IllegalParking
            ,
        [Terms("Pedagang Liar", "IllegalHawker")]
        IllegalHawker
            ,
        [Terms("Iklan Ilegal", "IlegalAds")]
        IlegalAds
            ,
        [Terms("Lampu Jalan Rusak", "BrokenStreetlight")]
        BrokenStreetlight
            ,
        [Terms("Palak Liar", "IllegalCharges")]
        IllegalCharges
            ,
        [Terms("Black Campaign", "BlackCampaign")]
        BlackCampaign
            ,
        [Terms("Kecelakaan Lantas", "TrafficAccident")]
        TrafficAccident
            ,
        [Terms("Rumah Tangga", "HouseHolds")]
        HouseHolds
            ,
        [Terms("Transportasi Publik", "PublicTransport")]
        PublicTransport
            ,
        [Terms("Rokok Sembarangan", "NoSmoking")]
        NoSmoking
            ,
        [Terms("Pohon Tumbang", "FallenTree")]
        FallenTree
            ,
        [Terms("Kemacetan", "TrafficJam")]
        TrafficJam
            ,
        [Terms("Banjir", "Flood")]
        Flood
            ,
        [Terms("Pengontrolan Banjir", "FloodControl")]
        FloodControl
            ,
        [Terms("Pengemis", "Beggar")]
        Beggar
            ,
        [Terms("Narkoba", "IllegalDrugs")]
        IllegalDrugs
            ,
        [Terms("Kriminal", "Criminal")]
        Criminal
            ,
        [Terms("Perijinan Bangunan", "BuildingPermit")]
        BuildingPermit
            ,
        [Terms("Kebakaran", "Fire")]
        Fire
            ,
        [Terms("Ruang Publik Terpadu", "RPTRA")]
        RPTRA
            ,
        [Terms("Fogging DBD", "FoggingDBD")]
        FoggingDBD
            ,
        [Terms("Makanan Tidak Sehat", "UnhigyenicFood")]
        UnhigyenicFood
            ,
        [Terms("Pajak Kost", "KostTax")]
        KostTax
            ,
        [Terms("Teroris", "Terrorist")]
        Terrorist
            ,
        [Terms("Demensia", "Dementia")]
        Dementia

    }
    [Serializable]
    public class Laporan
    {
        public string NoLaporan;
        public DateTime TglLaporan;
        [Prompt("Nama kamu siapa ? {||}")]
        public string Nama;

        [Prompt("Nomor telponnya berapa ? {||}")]
        public string Telpon;

        [Prompt("Emailnya ? {||}")]
        public string Email;

        [Prompt("Laporan apa yang mau disampaikan ? {||}")]
        public TipeLaporan TipeLaporan;

        [Prompt("Dimana kejadiannya ? {||}")]
        public string Lokasi;

        [Prompt("Kapan kejadiannya (year/month/date jam:menit) ? {||}")]
        public DateTime Waktu;

        [Prompt("Keluhannya apa ? {||}")]
        public string Keterangan;
        
        [Prompt("Kira-kira ini prioritas ga ? 1 [tidak penting] - 10 [sangat penting] ? ")]
        public int SkalaPrioritas = 1;

        public static IForm<Laporan> BuildForm()
        {

            OnCompletionAsyncDelegate<Laporan> processReport = async (context, state) =>
            {
                await Task.Run(() =>
                {
                    state.NoLaporan = $"LP-{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}";
                    state.TglLaporan = DateTime.Now;
                   
                    
                }
                );

            };
            var builder = new FormBuilder<Laporan>(false);
            var form = builder
                        .Field(nameof(Nama), validate:
                            async (state, value) =>
                            {
                                var result = new ValidateResult { IsValid = true, Value = value, Feedback = "ok, nama valid" };
                                bool res = value != null ? !string.IsNullOrEmpty(value.ToString()):false;
                                if (!res)
                                {
                                    result.Feedback = "tolong namanya di isi ya";
                                    result.IsValid = false;
                                }
                                return result;
                            })
                        .Field(nameof(Telpon), validate:
                            async (state, value) =>
                            {
                                var result = new ValidateResult { IsValid = true, Value = value, Feedback = "ok, telpon valid" };
                                bool res = FormValidation.isValidPhoneNumber(value.ToString());
                                if (!res)
                                {
                                    result.Feedback = "tolong isi dengan nomor telpon yang benar ya, cth : 08171234567";
                                    result.IsValid = false;
                                }
                                return result;
                            })
                        .Field(nameof(Email), validate:
                            async (state, value) =>
                            {
                                var result = new ValidateResult { IsValid = true, Value = value, Feedback = "ok, email valid" };
                                bool res = FormValidation.IsValidEmail(value.ToString());
                                if (!res)
                                {
                                    result.Feedback = "tolong isi dengan email yang benar ya";
                                    result.IsValid = false;
                                }
                                return result;
                            })
                        .Field(nameof(TipeLaporan))
                        .Field(nameof(Keterangan), validate:
                            async (state, value) =>
                            {
                                var result = new ValidateResult { IsValid = true, Value = value, Feedback = "ok, keterangan valid" };
                                bool res = value != null ? !string.IsNullOrEmpty(value.ToString()) : false;
                                if (!res)
                                {
                                    result.Feedback = "tolong keterangannya di isi ya";
                                    result.IsValid = false;
                                }
                                return result;
                            })
                        .Field(nameof(Lokasi), validate:
                            async (state, value) =>
                            {
                                var result = new ValidateResult { IsValid = true, Value = value, Feedback = "ok, lokasi valid" };
                                bool res = value != null ? !string.IsNullOrEmpty(value.ToString()) : false;
                                if (!res)
                                {
                                    result.Feedback = "tolong lokasi di isi ya";
                                    result.IsValid = false;
                                }
                                return result;
                            })
                        .Field(nameof(Waktu))
                        .Field(nameof(SkalaPrioritas), validate:
                            async (state, value) =>
                            {
                                var result = new ValidateResult { IsValid = true, Value = value, Feedback = "ok, skala valid" };
                                bool res = int.TryParse(value.ToString(), out int jml);
                                if (res)
                                {
                                    if (jml <= 0)
                                    {
                                        result.Feedback = "input data yang benar ya, minimum di isi 1";
                                        result.IsValid = false;
                                    }
                                    else if (jml > 10)
                                    {
                                        result.Feedback = "input data yang benar ya, maximum di isi 10";
                                        result.IsValid = false;
                                    }
                                }
                                else
                                {
                                    result.Feedback = "tolong isi dengan angka ya";
                                    result.IsValid = false;
                                }
                                return result;
                            })
                        .Confirm(async (state) =>
                        {
                            var pesan = $"Abang uda terima laporan dari {state.Nama} tentang {state.TipeLaporan.ToString()}, sudah ok ?";
                            return new PromptAttribute(pesan);
                        })
                        .Message($"Makasih ya atas laporannya.")
                        .OnCompletion(processReport)
                        .Build();
            return form;
        }
    }


}