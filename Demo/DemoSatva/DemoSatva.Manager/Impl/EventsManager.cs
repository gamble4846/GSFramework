using DemoSatva.DataAccess.Interface;
using DemoSatva.Manager.Interface;
using DemoSatva.Model;
using DemoSatva.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSatva.Manager.Impl
{
    public class EventsManager : IEventsManager
    {
        private readonly IEventsDataAccess DataAccess = null;
        public EventsManager(IEventsDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }

        public APIResponse GetEvents(int page = 1, int itemsPerPage = 100,List<OrderByModel> orderBy=null)
        {
            var result = DataAccess.GetAllEvents(page,itemsPerPage,orderBy);
            if (result != null && result.Count > 0)
            {   
                var totalRecords = DataAccess.GetAllTotalRecordEvents();
                var response = new { records = result, pageNumber = page, pageSize = itemsPerPage, totalRecords = totalRecords };
                return new APIResponse(ResponseCode.SUCCESS, "Record Found", response);
            }
            else
            {
                return new APIResponse(ResponseCode.ERROR, "No Record Found");
            }
        }

        public APIResponse SearchEvents(string searchKey,int page=1,int itemsPerPage=100,List<OrderByModel> orderBy=null)
        {
            var result = DataAccess.SearchEvents(searchKey,page,itemsPerPage,orderBy);
            if (result != null && result.Count > 0)
            { 
                var totalRecords = DataAccess.GetSearchTotalRecordEvents(searchKey);
                var response = new { records = result, pageNumber = page, pageSize = itemsPerPage, totalRecords = totalRecords };
                return new APIResponse(ResponseCode.SUCCESS, "Record Found", response);
            }
            else
            {
                return new APIResponse(ResponseCode.ERROR, "No Record Found");
            }
        }

        public APIResponse GetEventsByID(int Id)
        {
            var result = DataAccess.GetEventsByID(Id);
            if (result != null)
            {
                return new APIResponse(ResponseCode.SUCCESS, "Record Found", result);
            }
            else
            {
                return new APIResponse(ResponseCode.ERROR, "No Record Found");
            }
        }

        public APIResponse UpdateEvents(int Id, EventsModel model)
        {
			model.Id= Id;
           
            var result = DataAccess.UpdateEvents(model);
            if (result)
            {
                return new APIResponse(ResponseCode.SUCCESS, "Record Updated");
            }
            else
            {
                return new APIResponse(ResponseCode.ERROR, "Record Not Updated");
            }
        }

        public APIResponse AddEvents(EventsModel model)
        {
            var result = DataAccess.AddEvents(model);
            if (result > 0)
            {
                return new APIResponse(ResponseCode.SUCCESS, "Record Created", result);
            }
            else
            {
                return new APIResponse(ResponseCode.ERROR, "Record Not Created");
            }
        }

        

		public APIResponse DeleteEvents(int Id)
        {
            var result = DataAccess.DeleteEvents(Id);
            if (result)
            {
                return new APIResponse(ResponseCode.SUCCESS, "Record Deleted");
            }
            else
            {
                return new APIResponse(ResponseCode.ERROR, "Record Not Deleted");
            }
        }

        public APIResponse DeleteMultipleEvents(List<DeleteMultipleModel> deleteParam,string andOr)
        {
            var result = DataAccess.DeleteMultipleEvents(deleteParam,andOr);
            if (result)
            {
                return new APIResponse(ResponseCode.SUCCESS, "Record Deleted");
            }
            else
            {
                return new APIResponse(ResponseCode.ERROR, "Record Not Deleted");
            }
        }

        public APIResponse FilterEvents(List<FilterModel> filterModels, string andOr, int page = 1, int itemsPerPage = 100, List<OrderByModel> orderBy = null)
        {
            var result = DataAccess.FilterEvents(filterModels, andOr,page, itemsPerPage, orderBy);
            if (result != null && result.Count > 0)
            {
                var totalRecords = DataAccess.GetFilterTotalRecordEvents(filterModels,andOr);
                var response = new { records = result, pageNumber = page, pageSize = itemsPerPage, totalRecords = totalRecords };
                return new APIResponse(ResponseCode.SUCCESS, "Record Found", response);
            }
            else
            {
                return new APIResponse(ResponseCode.ERROR, "No Record Found");
            }
        }
    }
}

