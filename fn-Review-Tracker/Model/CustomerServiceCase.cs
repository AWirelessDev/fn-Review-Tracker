using Newtonsoft.Json;
using Victra.Integrations.Models;

namespace fn_Review_Tracker.Model {
    public class CustomerServiceCase  {
        public string SystemId { get; set; } = null!;
        public string ServiceCaseNo { get; set; } = null!;       
        public string Category { get; set; }       
        public string SubCategory { get; set; } = null!;
        public string Location { get; set; }
        public string Priority { get; set; } 
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string IssueReasonCode { get; set; }
        public string Assignment_Group { get; set; }
        public string Assign_To { get; set; }
        public string Contact_Type { get; set; }
        public string Comments { get; set; }
            
    }
}
