using fn_Review_Tracker.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace fn_Review_Tracker.Repository {
    public class DataRepository : IDataRepository {
        private readonly ILogger<DataRepository> _logger;
        private readonly SqlConnection _sqlConnection;

        public DataRepository(IConfiguration configuration, ILogger<DataRepository> logger) {
            _logger = logger;
            _sqlConnection = new SqlConnection(configuration.GetConnectionString("ConnectionString"));
        }

        public async Task<DataTable> GetReviewTrackerDetails(DateTime date) {
            DataTable dt = new DataTable();
            try {
                if (_sqlConnection.State != ConnectionState.Open) {
                    _sqlConnection.Open();
                }
                await using SqlCommand command = _sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "usp_GetReviewTrackerDetails";
                command.Parameters.Add(new SqlParameter("@CurrentDate", date));

                using SqlDataAdapter adapter = new SqlDataAdapter(command);
                await Task.Run(() => adapter.Fill(dt));

            }
            catch (Exception ex) {
                _logger.LogWarning(ex.Message);
            }
            finally { _sqlConnection.Close(); }

            return dt;
        }

        public async Task<ReviewTrackerResponse> SaveSnowTicketDetails(ServiceNowTicketModel serviceNowTicket) {
            try {
                if (_sqlConnection.State != ConnectionState.Open) {
                    _sqlConnection.Open();
                }
                await using SqlCommand command = _sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "usp_SaveServiceNowTicket";
                command.Parameters.Add(new SqlParameter("@SysId", serviceNowTicket.SysId));
                command.Parameters.Add(new SqlParameter("@ReviewId", serviceNowTicket.ReviewId));
                command.Parameters.Add(new SqlParameter("@TicketNo", serviceNowTicket.TicketNo));
                command.Parameters.Add(new SqlParameter("@Location", serviceNowTicket.Location));
                command.Parameters.Add(new SqlParameter("@CreatedDate", serviceNowTicket.CreatedDate));
                command.ExecuteNonQueryAsync();
                _logger.LogInformation($"{nameof(SaveSnowTicketDetails)} method executed successfully.");
                return new ReviewTrackerResponse() { StatusCode = HttpStatusCode.OK, Success = true };                
            }
            catch (Exception ex) {
                _logger.LogWarning(ex.Message);
                return new ReviewTrackerResponse() { StatusCode = HttpStatusCode.BadRequest, Success = false, ErrorMessage = ex.Message };
            }
            finally { _sqlConnection.Close(); }
        }       

        public void SaveBulkTickets(List<ServiceNowTicketModel> createdTickets) {

            if (_sqlConnection.State != ConnectionState.Open) {
                _sqlConnection.Open();
            }

            using (var transaction = _sqlConnection.BeginTransaction()) {
                try {
                    using (var bulkCopy = new SqlBulkCopy(_sqlConnection, SqlBulkCopyOptions.Default, transaction)) {
                        bulkCopy.BatchSize = 1000; 
                        bulkCopy.DestinationTableName = "ReviewTrackersSnowTickets";

                        var dataTable = new DataTable();
                        dataTable.Columns.Add("SysId", typeof(string));
                        dataTable.Columns.Add("ReviewId", typeof(string));
                        dataTable.Columns.Add("TicketNo", typeof(string));
                        dataTable.Columns.Add("Category", typeof(string));
                        dataTable.Columns.Add("SubCategory", typeof(string));
                        dataTable.Columns.Add("Description", typeof(string));
                        dataTable.Columns.Add("ShortDescription", typeof(string));
                        dataTable.Columns.Add("Priority", typeof(string));
                        dataTable.Columns.Add("Assignment_Group", typeof(string));
                        dataTable.Columns.Add("Assign_To", typeof(string));
                        dataTable.Columns.Add("IssueReasonCode", typeof(string));
                        dataTable.Columns.Add("CreatedDate", typeof(DateTime));

                        // Add rows to DataTable
                        foreach (var ticket in createdTickets) {
                            dataTable.Rows.Add(ticket.SysId, ticket.ReviewId, ticket.TicketNo, ticket.Category, ticket.SubCategory, ticket.Description,
                                ticket.ShortDescription, ticket.Priority, ticket.Assignment_Group, ticket.Assign_To, ticket.IssueReasonCode, ticket.CreatedDate);
                        }

                        // Write to server
                        bulkCopy.WriteToServer(dataTable);
                        // Commit transaction
                        transaction.Commit();
                        _logger.LogInformation("Bulk Saving executed successfully..");
                    }
                }
                catch (Exception ex) {
                    // Handle exception
                    transaction.Rollback();
                    _logger.LogError("Error occured in bulk saving method");
                    throw new Exception("Failed to save bulk tickets.", ex);
                }
                finally {
                    _sqlConnection.Close();
                }
            }

        }

    }
}
