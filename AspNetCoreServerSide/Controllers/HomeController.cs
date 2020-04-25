﻿using AspNetCoreServerSide.Contracts;
using AspNetCoreServerSide.Models;
using JqueryDataTables.ServerSide.AspNetCoreWeb.ActionResults;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AspNetCoreServerSide.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDemoService _demoService;

        public HomeController(IDemoService demoService)
        {
            _demoService = demoService;
        }

        public IActionResult Index()
        {
            return View(new Demo());
        }

        //public async Task<IActionResult> LoadTable([ModelBinder(typeof(JqueryDataTablesBinder))] JqueryDataTablesParameters param)
        //{
        //    try
        //    {
        //        var results = await _demoService.GetDataAsync(param);

        //        return new JsonResult(new JqueryDataTablesResult<Demo>
        //        {
        //            Draw = param.Draw,
        //            Data = results.Items,
        //            RecordsFiltered = results.TotalSize,
        //            RecordsTotal = results.TotalSize
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write(e.Message);
        //        return new JsonResult(new { error = "Internal Server Error" });
        //    }
        //}

        //public async Task<IActionResult> GetExcel([ModelBinder(typeof(JqueryDataTablesBinder))] JqueryDataTablesParameters param)
        //{
        //    var results = await _demoService.GetDataAsync(param);
        //    return new JqueryDataTablesExcelResult<Demo>(results.Items, "Demo Sheet Name", "Fingers10");
        //    return new JqueryDataTablesCSVResult<Demo>(results.Items, "Fingers10");
        //}

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody]JqueryDataTablesParameters param)
        {
            try
            {
                HttpContext.Session.SetString(nameof(JqueryDataTablesParameters), JsonSerializer.Serialize(param));
                var results = await _demoService.GetDataAsync(param);

                return new JsonResult(new JqueryDataTablesResult<Demo>
                {
                    Draw = param.Draw,
                    Data = results.Items,
                    RecordsFiltered = results.TotalSize,
                    RecordsTotal = results.TotalSize
                });
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return new JsonResult(new { error = "Internal Server Error" });
            }
        }

        public async Task<IActionResult> GetExcel()
        {
            var param = HttpContext.Session.GetString(nameof(JqueryDataTablesParameters));

            var results = await _demoService.GetDataAsync(JsonSerializer.Deserialize<JqueryDataTablesParameters>(param));
            return new JqueryDataTablesExcelResult<Demo>(results.Items, "Demo Sheet Name", "Fingers10");
            //return new JqueryDataTablesCSVResult<Demo>(results.Items, "Fingers10");
        }

        [HttpPost]
        public async Task<NoContentResult> Create(Demo item)
        {
            await _demoService.CreateDataAsync(item);

            return NoContent();
        }

        public async Task<ActionResult> Edit(int id)
        {
            var item = await _demoService.GetDataByIdAsync(id);

            return PartialView("_Edit", item);
        }

        [HttpPut]
        public async Task<NoContentResult> Edit(Demo item)
        {
            await _demoService.UpdateDataAsync(item);

            return NoContent();
        }

        [HttpDelete]
        public async Task<NoContentResult> Delete(int id)
        {
            await _demoService.DeleteDataAsync(id);

            return NoContent();
        }
    }
}
