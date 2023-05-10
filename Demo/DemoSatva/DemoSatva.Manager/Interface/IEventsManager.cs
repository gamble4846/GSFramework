using DemoSatva.Model;
using DemoSatva.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSatva.Manager.Interface
{
    public interface IEventsManager
    {
        APIResponse GetEvents(int page, int itemsPerPage,List<OrderByModel> orderBy);
        APIResponse SearchEvents(string searchKey, int page, int itemsPerPage,List<OrderByModel> orderBy);
        APIResponse FilterEvents(List<FilterModel> filterModels, string andOr, int page, int itemsPerPage, List<OrderByModel> orderBy);
        APIResponse GetEventsByID(int Id);
        APIResponse UpdateEvents(int Id,EventsModel model);
        APIResponse AddEvents(EventsModel model);
		APIResponse DeleteEvents(int Id);
        APIResponse DeleteMultipleEvents(List<DeleteMultipleModel> deleteParam,string andOr);
    }
}

