using fn_Review_Tracker.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace fn_Review_Tracker.Repository {
    public interface IDataRepository {

        public Task<DataTable> GetReviewTrackerDetails(DateTime date);

        Task<ReviewTrackerResponse> SaveSnowTicketDetails(ServiceNowTicketModel serviceNowTicket);

        void SaveBulkTickets(List<ServiceNowTicketModel> createdTickets);
    }
}
