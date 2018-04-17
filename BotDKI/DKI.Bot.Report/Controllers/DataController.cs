using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.DKI.Entities;
using DKI.Bot.Entities;
using DKI.Bot.Services;
using Microsoft.AspNetCore.Mvc;

namespace DKI.Bot.Report.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly RedisDB _context;

        public DataController(RedisDB context)
        {
            _context = context;
        }
        /*
        /// <summary>
        /// insert card log
        /// </summary>
        /// <param name="value"></param>  
        [HttpPost("[action]")]
        public ActionResult InsertCardlog([FromBody]CardLog value)
        {
            var hasil = new OutputData() { IsSucceed = true };
            try
            {
                value.Id = _context.GetSequence<CardLog>();
                _context.InsertData<CardLog>(value);
                hasil.Data = value;
                hasil.IsSucceed = true;
                hasil.ErrorMessage = "success";
            }
            catch (Exception ex)
            {
                hasil.ErrorMessage = ex.Message + "_" + ex.StackTrace;
                hasil.IsSucceed = false;
            }

            return Ok(hasil);
        }
        */
        

        /// <summary>
        /// get list card log
        /// </summary>
        /// <param name="UserId"></param>  
        [HttpGet("[action]")]
        public ActionResult GetKeluhanList()
        {
            var hasil = new OutputData() { IsSucceed = true };
            try
            {

                hasil.Data = _context.GetAllData<Complain>().OrderBy(x => x.Id).ToList();
                hasil.IsSucceed = true;
                hasil.ErrorMessage = "success";
            }
            catch (Exception ex)
            {
                hasil.ErrorMessage = ex.Message + "_" + ex.StackTrace;
                hasil.IsSucceed = false;
            }

            return Ok(hasil);
        }
    }
}