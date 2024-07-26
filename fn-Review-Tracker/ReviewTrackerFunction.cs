using fn_Review_Tracker.Helper;
using fn_Review_Tracker.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace fn_Review_Tracker
{
    public class ReviewTrackerFunction
    {
        private readonly ReviewTrackerHelper _reviewTrackerHelper;
        private readonly ILogger<ReviewTrackerFunction> _logger;

        public ReviewTrackerFunction(ReviewTrackerHelper reviewTrackerHelper, ILogger<ReviewTrackerFunction> logger)
        {
            _reviewTrackerHelper = reviewTrackerHelper;
            _logger = logger;
        }

        [Function("ReviewTrackerFunction")]
        public async Task<IActionResult> ReviewTracker([HttpTrigger(AuthorizationLevel.Function,  "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger ReviewTrackerFunction processed a request.");
            ReviewTrackerResponse reviewTrackerResponse = new ReviewTrackerResponse();
            try {
                DateTime currentDate = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(req.Query["Date"])) {
                    currentDate = DateTime.Parse(req.Query["date"]);
                }
                var response = await _reviewTrackerHelper.ProcessReviewTracker(currentDate);

                return new OkObjectResult(response);
            }
            catch (UnauthorizedAccessException ex) {
                ExceptionLogger.LogException(nameof(ReviewTrackerFunction), ex, _logger);
                return new UnauthorizedResult();
            }
            catch (ArgumentException ex) {
                ExceptionLogger.LogException(nameof(ReviewTrackerFunction), ex, _logger);
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex) {
                ExceptionLogger.LogException(nameof(ReviewTrackerFunction), ex, _logger);
                return new BadRequestObjectResult(ex.Message);
            }
           // return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
