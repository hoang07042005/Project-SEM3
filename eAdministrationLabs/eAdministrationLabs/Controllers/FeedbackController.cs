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

        //[HttpPost]
        //public async Task<IActionResult> Submit(Feedback feedback)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        //        return Json(new { success = false, message = "Validation failed.", errors });
        //    }

        //    var historyRequest = await _context.HistoryRequests
        //        .Include(h => h.User)
        //        .FirstOrDefaultAsync(h => h.RequestId == feedback.RequestId);

        //    if (historyRequest == null)
        //    {
        //        return Json(new { success = false, message = "Request not found." });
        //    }

        //    // Gán UserId từ HistoryRequest
        //    feedback.UserId = historyRequest.UserId;

        //    try
        //    {
        //        // Không cần gán HistoryRequest và User vào Feedback
        //        _context.Feedbacks.Add(feedback);
        //        await _context.SaveChangesAsync();

        //        // Gửi email thông báo
        //        var adminEmail = "administrator@example.com";
        //        var subject = "New Feedback Submitted";
        //        var body = $@"
        //           <html lang=""en"">
        //                <head>
        //                    <meta charset=""UTF-8"">
        //                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        //                    <title>Feedback Notification</title>
        //                    <style>
        //                        body {{
        //                            font-family: Arial, sans-serif;
        //                            line-height: 1.6;
        //                            color: #333;
        //                            margin: 0;
        //                            padding: 0;
        //                            background-color: #f4f4f4;
        //                        }}
        //                        .email-container {{
        //                            max-width: 600px;
        //                            margin: 30px auto;
        //                            background: #fff;
        //                            padding: 20px;
        //                            border: 1px solid #ddd;
        //                            border-radius: 8px;
        //                            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        //                        }}
        //                        .email-header {{
        //                            background-color: #007bff;
        //                            color: #fff;
        //                            padding: 15px;
        //                            border-radius: 8px 8px 0 0;
        //                            text-align: center;
        //                        }}
        //                        .email-header h1 {{
        //                            margin: 0;
        //                            font-size: 24px;
        //                        }}
        //                        .email-body {{
        //                            padding: 20px;
        //                        }}
        //                        .email-body p {{
        //                            margin: 10px 0;
        //                        }}
        //                        .email-body .highlight {{
        //                            color: #007bff;
        //                            font-weight: bold;
        //                        }}
        //                        .email-footer {{
        //                            text-align: center;
        //                            margin-top: 20px;
        //                            font-size: 12px;
        //                            color: #777;
        //                        }}
        //                        .action-button {{
        //                            display: inline-block;
        //                            margin-top: 20px;
        //                            padding: 10px 20px;
        //                            font-size: 16px;
        //                            background-color: #007bff;
        //                            color: #fff;
        //                            text-decoration: none;
        //                            border-radius: 5px;
        //                        }}
        //                        .action-button:hover {{
        //                            background-color: #0056b3;
        //                        }}
        //                    </style>
        //                </head>
        //                <body>
        //                    <div class=""email-container"">
        //                        <!-- Header -->
        //                        <div class=""email-header"">
        //                            <h1>Feedback Received</h1>
        //                        </div>

        //                        <!-- Body -->
        //                        <div class=""email-body"">
        //                            <h2>Request ID: <span class=""highlight"">{feedback.RequestId}</span></h2>
        //                            <p><strong>Rating:</strong> {feedback.Rating}</p>
        //                            <p><strong>Satisfaction:</strong> {feedback.Satisfaction}</p>
        //                            <p><strong>Quality:</strong> {feedback.Quality}</p>
        //                            <p><strong>Comment:</strong> {feedback.Comment}</p>

        //                            <p>We appreciate your feedback and are constantly striving to improve our services.</p>

        //                            <a href=""{{supportLink}}"" class=""action-button"">Contact Support</a>
        //                        </div>

        //                        <!-- Footer -->
        //                        <div class=""email-footer"">
        //                            <p>If you have any further questions, feel free to reach out to us at <a href=""mailto:support@example.com"">support@example.com</a>.</p>
        //                            <p>This is an automated email. Please do not reply directly to this message.</p>
        //                        </div>
        //                    </div>
        //                </body>
        //                </html>";

        //        await _emailService.SendEmailAsync(adminEmail, subject, body);

        //        return Json(new { success = true, message = "Feedback submitted successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
        //    }
        //}


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
