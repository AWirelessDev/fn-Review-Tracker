using fn_Review_Tracker.Contract;
using fn_Review_Tracker.Model;
using fn_Review_Tracker.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace fn_Review_Tracker.Helper {
    public class EDWDataHelper : IEDWDataHelper{

        private readonly ILogger<EDWDataHelper> _logger;
        private readonly IDataRepository _edwData;

        public EDWDataHelper(IDataRepository dataRepository, ILogger<EDWDataHelper> logger) {

            _edwData = dataRepository;
            _logger = logger;
        }

        public async Task<DataTable> GetReviewTrackerDetails(DateTime date) {
            DataTable dt = new DataTable();
            try {
                dt = await _edwData.GetReviewTrackerDetails(date);
            }
            catch (Exception ex) {
                _logger.LogError("Error occured {ex.Message}");

            }
            return dt;
        }

        public async Task<ReviewTrackerResponse> SaveSnowTicketDetails(ServiceNowTicketModel serviceNowTicket) {
            return await _edwData.SaveSnowTicketDetails(serviceNowTicket);
        }
    }
}
