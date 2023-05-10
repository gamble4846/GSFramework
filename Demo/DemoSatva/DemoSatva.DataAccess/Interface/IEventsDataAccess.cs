using DemoSatva.Model;
using DemoSatva.Utility;
using System.Collections.Generic;

namespace DemoSatva.DataAccess.Interface
{
    public interface IEventsDataAccess
    {
        List<EventsModel> GetAllEvents(int page, int itemsPerPage,List<OrderByModel> orderBy);
        List<EventsModel> SearchEvents(string searchKey,int page, int itemsPerPage,List<OrderByModel> orderBy);
        List<EventsModel> FilterEvents(List<FilterModel> filterModels,string andOr, int page, int itemsPerPage, List<OrderByModel> orderBy);
        EventsModel GetEventsByID(int Id);
        bool UpdateEvents(EventsModel model);
        int GetAllTotalRecordEvents();
        int GetSearchTotalRecordEvents(string searchKey);
        int GetFilterTotalRecordEvents(List<FilterModel> filterBy,string andOr);
        long AddEvents(EventsModel model);
        bool DeleteEvents(int Id);
        bool DeleteMultipleEvents(List<DeleteMultipleModel> deleteParam,string andOr);
        
        
    }
}

