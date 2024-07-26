using System;

namespace fn_Review_Tracker.Model {
    public class ServiceNowTicketModel : CustomerServiceCase {
        public string SysId { get; set; }
        public string ReviewId { get; set; }
        public string TicketNo { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
