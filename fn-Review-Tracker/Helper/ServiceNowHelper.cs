using Microsoft.Extensions.Configuration;
using Victra.Integrations.Models;

namespace fn_Review_Tracker.Helper {
    public class ServiceNowHelper {
        public IConfiguration _configuration;

        public ServiceNowHelper(IConfiguration configuration) {
            _configuration = configuration;
        }
        public CustomerServiceCaseModel GetCustomerServiceCaseTemplate() {

            return new CustomerServiceCaseModel()
            {
                description = _configuration["CustomerServiceTicket:Description"],
                short_description = _configuration["CustomerServiceTicket:ShortDescription"],
                category = _configuration["CustomerServiceTicket:Category"],
                subcategory = _configuration["CustomerServiceTicket:SubCategory"],
                assignment_group = _configuration["CustomerServiceTicket:Assignment_group"],
                assigned_to  = _configuration["CustomerServiceTicket:Assign_To"],
                priority = _configuration["CustomerServiceTicket:Priority"],
                u_issue_reason_code = _configuration["CustomerServiceTicket:IssueReasonCode"],
                contact_type = _configuration["CustomerServiceTicket:Contact_type"]
            };
        }

    }
}
