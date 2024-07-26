using System.Net;

namespace fn_Review_Tracker.Model {
    public class ReviewTrackerResponse {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public string SuccessMessage { get;set; }
    }
}
