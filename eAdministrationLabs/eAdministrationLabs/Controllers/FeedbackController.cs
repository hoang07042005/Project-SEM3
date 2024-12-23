using Microsoft.AspNetCore.Mvc;
using eAdministrationLabs.Models;
using Microsoft.EntityFrameworkCore;
using eAdministrationLabs.Services;

namespace eAdministrationLabs.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly EAdministrationLabsContext _context;
        private readonly EmailService _emailService;

        public FeedbackController(EAdministrationLabsContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Feedback/Index?RequestId={RequestId}
        public async Task<IActionResult> Index(int requestId)
        {
            var currentFullName = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentFullName))
            {
                return RedirectToAction("login", "Account");
            }

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == currentFullName);

            if (currentUser == null)
            {
                return RedirectToAction("login", "Account");
            }

            var historyRequest = await _context.HistoryRequests
                .Include(h => h.User)
                .FirstOrDefaultAsync(h => h.RequestId == requestId);

            if (historyRequest == null)
            {
                return NotFound("Request not found.");
            }

            var feedbackModel = new Feedback
            {
                RequestId = requestId
            };

            return View(feedbackModel);
        }

        


        [HttpPost]
        public async Task<IActionResult> Submit(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation failed.", errors });
            }

            var historyRequest = await _context.HistoryRequests
                .Include(h => h.User)
                .FirstOrDefaultAsync(h => h.RequestId == feedback.RequestId);

            if (historyRequest == null)
            {
                return Json(new { success = false, message = "Request not found." });
            }

            // Gán UserId từ HistoryRequest
            feedback.UserId = historyRequest.UserId;

            try
            {
                // Không cần gán HistoryRequest và User vào Feedback
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                // Gửi email thông báo
                var adminEmail = "administrator@example.com";
                var subject = "New Feedback Submitted";
                var body = $@"
            <html lang=""en"">
                <body>
                    <h1>Feedback Received</h1>
                    <p>Request ID: {feedback.RequestId}</p>
                    <p>Rating: {feedback.Rating}</p>
                    <p>Satisfaction: {feedback.Satisfaction}</p>
                    <p>Quality: {feedback.Quality}</p>
                    <p>Comment: {feedback.Comment}</p>
                </body>
            </html>";

                await _emailService.SendEmailAsync(adminEmail, subject, body);

                return Json(new { success = true, message = "Feedback submitted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }




    }
}
