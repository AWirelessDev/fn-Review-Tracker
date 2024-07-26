using fn_Review_Tracker.Model;
using System;
using System.Data;
using System.Threading.Tasks;

namespace fn_Review_Tracker.Contract {
    public interface IEDWDataHelper {
        Task<DataTable> GetReviewTrackerDetails(DateTime date);

        Task<ReviewTrackerResponse> SaveSnowTicketDetails(ServiceNowTicketModel serviceNowTicket);
    }
}
