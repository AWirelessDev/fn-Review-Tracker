using fn_Review_Tracker.Model;
using fn_Review_Tracker.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Victra.Integrations.Helpers;
using Victra.Integrations.Models;

namespace fn_Review_Tracker.Helper {
    public class ReviewTrackerHelper {

        private readonly ILogger<ReviewTrackerHelper> _logger;
        private readonly IDataRepository _edwData;
        private readonly ServiceNowHelper _serviceNowHelper;
        private readonly ServiceNowSettings _serviceNowSettings;
        private readonly CustomerServiceCaseSettings _customerServiceCaseSettings;
        private readonly CustomerServiceCaseHelper _customerServiceCaseHelper;
        public ReviewTrackerHelper(ILogger<ReviewTrackerHelper> logger, IDataRepository edwData, ServiceNowHelper serviceNowHelper, ServiceNowSettings serviceNowSettings, CustomerServiceCaseSettings customerServiceCaseSettings, CustomerServiceCaseHelper customerServiceCaseHelper) {
            _logger = logger;
            _edwData = edwData;
            _serviceNowHelper = serviceNowHelper;
            _serviceNowSettings = serviceNowSettings;
            _customerServiceCaseSettings = customerServiceCaseSettings;
            _customerServiceCaseHelper = customerServiceCaseHelper;
        }

        public async Task<ReviewTrackerResponse> ProcessReviewTracker(DateTime date) {
            _logger.LogInformation($"Calling {nameof(ProcessReviewTracker)} parameter date={date}");
            ReviewTrackerResponse reviewTrackerResponse=new ReviewTrackerResponse();
            DataTable returnData = new DataTable();
            List<ServiceNowTicketModel> createdTickets = new List<ServiceNowTicketModel>();
            try {             
                StringBuilder description = new StringBuilder();  
                returnData = await _edwData.GetReviewTrackerDetails(date);
                var serviceNowTemplate = _serviceNowHelper.GetCustomerServiceCaseTemplate();
                _logger.LogInformation($"Return {returnData.Rows.Count} records from EDW DATAMART_SOCIAL_MEDIA database");
                
                foreach (DataRow item in returnData.Rows) {
                    description.Clear();
                    serviceNowTemplate.location = item["Location"].ToString();
                    description.AppendLine($"<b>Date of post:</b>{date.ToShortDateString()}");
                    description.Append("<br/>");
                    description.AppendLine("<b>Comment:</b><br/>");
                    description.AppendLine (item["Review"].ToString());

                    serviceNowTemplate.description = description.ToString();
                    serviceNowTemplate.short_description = $"SMR – {item["Source"].ToString()} Review – {item["Author"].ToString()} ";

                    var result = await _customerServiceCaseHelper.CustomerServiceCaseCreation(_serviceNowSettings, _customerServiceCaseSettings, serviceNowTemplate, _customerServiceCaseHelper);
                    if (result.ResponseStatus.StatusCode == HttpStatusCode.Created) {
                        ServiceNowTicketModel serviceNowTicket = new ServiceNowTicketModel()
                        {
                            SysId = result.Content.SystemId,
                            ReviewId = item["ReviewId"].ToString(),
                            TicketNo = result.Content.ServiceCaseNo,
                            Category = serviceNowTemplate.category,
                            SubCategory = serviceNowTemplate.subcategory,
                            Description = serviceNowTemplate.description,
                            ShortDescription = serviceNowTemplate.short_description,
                            Priority = serviceNowTemplate.priority,
                            Assignment_Group = serviceNowTemplate.assignment_group,
                            Assign_To = serviceNowTemplate.assigned_to,
                            IssueReasonCode = serviceNowTemplate.u_issue_reason_code,
                            CreatedDate = DateTime.Today
                        };
                        createdTickets.Add(serviceNowTicket);
                    }
                
                }              
                
            }
            catch (Exception ex) {
                _logger.LogError($"Error occured {ex.Message}");
                reviewTrackerResponse = new ReviewTrackerResponse { ErrorMessage = ex.Message, StatusCode = HttpStatusCode.BadRequest, Success = false };
            }
            finally {
                if (createdTickets.Count > 0) {
                    // call db bulk saving
                    _edwData.SaveBulkTickets(createdTickets);
                    _logger.LogInformation($"{createdTickets.Count} Tickets created..");
                    reviewTrackerResponse = new ReviewTrackerResponse { StatusCode = HttpStatusCode.OK, Success = true, SuccessMessage = $"{createdTickets.Count} Tickets created for Date: {date.ToShortDateString()}" };

                }
            }
            return reviewTrackerResponse;
        }            

    }
}
