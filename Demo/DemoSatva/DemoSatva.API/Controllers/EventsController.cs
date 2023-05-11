using System;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using DemoSatva.Manager.Interface;
using DemoSatva.Model;
using DemoSatva.Utility;
using log4net;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.ComponentModel;
using EasyCrudDB;

namespace DemoSatva.API.Controllers
{
    //[Authorize]
    [ApiController]
    public class EventsController : ControllerBase
    {
        ILog log4Net;
        IEventsManager Manager;
        ValidationResult ValidationResult;
        public EventsController(IEventsManager manager)
        {
            log4Net = this.Log<EventsController>();
            Manager = manager;
            ValidationResult = new ValidationResult();
        }

        [HttpGet]
        [Route(APIEndpoint.DefaultRoute)]
        public ActionResult Get(int page = 1, int itemsPerPage = 100, string orderBy = null)
        {
            try
            {
                if (page <= 0)
                {
                    ValidationResult.AddFieldError("Id", "Invalid page number");
                }
                if (ValidationResult.IsError)
                {
                    return BadRequest(new APIResponse(ResponseCode.ERROR, "Validation failed", ValidationResult));
                }
                List<OrderByModel> orderModelList = UtilityCommon.ConvertStringOrderToOrderModel(orderBy);
                return Ok(Manager.GetEvents(page, itemsPerPage, orderModelList));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(ResponseCode.ERROR, "Exception", ex.Message));
            }
        }

        [HttpGet]
        [Route(APIEndpoint.DefaultRoute + "/search")]
        public ActionResult Search(string searchKey, int page = 1, int itemsPerPage = 100, string orderBy = null)
        {
            try
            {
                if (string.IsNullOrEmpty(searchKey))
                {
                    ValidationResult.AddEmptyFieldError("SearchKey");
                }
                else if (!string.IsNullOrEmpty(searchKey) && searchKey.Length < 3)
                {
                    ValidationResult.AddFieldError("SearchKey", "Minimum 3 chracters required for search");
                }
                if (page <= 0)
                {
                    ValidationResult.AddFieldError("Id", "Invalid page number");
                }
                if (ValidationResult.IsError)
                {
                    return BadRequest(new APIResponse(ResponseCode.ERROR, "Validation failed", ValidationResult));
                }
                List<OrderByModel> orderModelList = UtilityCommon.ConvertStringOrderToOrderModel(orderBy);
                return Ok(Manager.SearchEvents(searchKey, page, itemsPerPage, orderModelList));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(ResponseCode.ERROR, "Exception", ex.Message));
            }
        }

        [HttpGet]
        [Route(APIEndpoint.DefaultRoute + "/{Id}")]
        public ActionResult GetById(int Id)
        {
            try
            {
                if (Id <= 0) { ValidationResult.AddEmptyFieldError("Id"); }

                if (ValidationResult.IsError)
                {
                    return BadRequest(new APIResponse(ResponseCode.ERROR, "Validation failed", ValidationResult));
                }
                return Ok(Manager.GetEventsByID(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(ResponseCode.ERROR, "Exception", ex.Message));
            }
        }

        [HttpPost]
        [Route(APIEndpoint.DefaultRoute)]
        public ActionResult Post(EventsModel model)
        {
            try
            {
                return Ok(Manager.AddEvents(model));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(ResponseCode.ERROR, "Exception", ex.Message));
            }
        }

        [HttpPut]
        [Route(APIEndpoint.DefaultRoute + "/{Id}")]
        public ActionResult Put(int Id, EventsModel model)
        {
            try
            {
                if (Id <= 0) { ValidationResult.AddEmptyFieldError("Id"); }

                if (ValidationResult.IsError)
                {
                    return BadRequest(new APIResponse(ResponseCode.ERROR, "Validation failed", ValidationResult));
                }
                return Ok(Manager.UpdateEvents(Id, model));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(ResponseCode.ERROR, "Exception", ex.Message));
            }
        }
        [HttpDelete]
        [Route(APIEndpoint.DefaultRoute + "/{Id}")]
        public ActionResult Delete(int Id)
        {
            try
            {
                if (Id <= 0) { ValidationResult.AddEmptyFieldError("Id"); }
                if (ValidationResult.IsError)
                {
                    return BadRequest(new APIResponse(ResponseCode.ERROR, "Validation failed", ValidationResult));
                }
                return Ok(Manager.DeleteEvents(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(ResponseCode.ERROR, "Exception", ex.Message));
            }
        }

        [HttpGet]
        [Route("/api/Query/TEST/")]
        public ActionResult Test()
        {
            try
            {
                var GSFramework = new EasyCrud("Server=db.satva.solutions,59763;Database=DemoSatva;User Id=sa;Password=Fishy1213#;");

                var em = new EventsModel()
                {
                    Id = 1,
                    EventTitle = "TEST",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    EventDescription = "DESC",
                    EventPriority = 1
                };


                //var t = GSFramework.Query("Select * From Events");
                //var p = UtilityCustom.ConvertDynamicToType<List<EventsModel>>(t);

                var h = GSFramework.Query("INSERT INTO [dbo].[Events] ([EventTitle] ,[StartDate] ,[EndDate] ,[EventDescription] ,[EventPriority]) VALUES ('asd' ,'1/1/1' ,'1/1/1' ,'1/1/1' ,1)", false, GSEnums.ExecuteType.ExecuteNonQuery);
                GSFramework.SaveChanges();

                var x = GSFramework.Add(em, "Id", "Id", false);
                em.EventDescription = "v DESC";
                var y = GSFramework.Add(em, "Id", "Id", true);
                em.EventDescription = "X DESC";

                var c1 = GSFramework.Count<EventsModel>(null, GSEnums.WithInQuery.ReadPast);
                var c2 = GSFramework.Count<EventsModel>(null, GSEnums.WithInQuery.NoLock);

                var b = GSFramework.Update(em, " Where Id = " + y, "Id", true);
                var c = GSFramework.Update(em, " Where Id = " + x, "Id", false);
                var g = GSFramework.Remove<EventsModel>(" WHERE Id = " + y, false);
                var z = GSFramework.GetList<EventsModel>(-1, -1, null, null, GSEnums.WithInQuery.ReadPast);
                var a = GSFramework.GetList<EventsModel>(-1, -1, null, null, GSEnums.WithInQuery.NoLock);
                GSFramework.SaveChanges();
                return Ok(new { });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse(ResponseCode.ERROR, "Exception", ex.Message));
            }

        }
    }
}
