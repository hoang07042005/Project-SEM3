using Microsoft.AspNetCore.Mvc;
using eAdministrationLabs.Models;
using Microsoft.EntityFrameworkCore;
using eAdministrationLabs.Services;
using eAdministrationLabs.Models.ViewModels;

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



        public async Task<IActionResult> Index(int requestId)
        {
            // Check if the user is authenticated
            var currentFullName = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentFullName))
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the current user from the database
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == currentFullName);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the HistoryRequest based on the provided requestId including related RequestCompletions
            var historyRequest = await _context.HistoryRequests
                .Include(h => h.User)
                .Include(h => h.RequestCompletions)
                .FirstOrDefaultAsync(h => h.RequestId == requestId);
            if (historyRequest == null)
            {
                return NotFound("Request not found.");
            }

            // Create the ViewModel
            var viewModel = new FeedbackViewModel
            {
                Feedback = new Feedback { RequestId = requestId },
                RequestCompletions = historyRequest.RequestCompletions.ToList()
            };

            // Return the view with the ViewModel
            return View(viewModel);
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
                .Include(h => h.RequestCompletions)
                .FirstOrDefaultAsync(h => h.RequestId == feedback.RequestId);

            if (historyRequest == null)
            {
                return Json(new { success = false, message = "Request not found." });
            }

            // Gán UserId từ HistoryRequest
            feedback.UserId = historyRequest.UserId;

            try
            {
                // Thêm feedback vào cơ sở dữ liệu
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                var requestCompletionsContent = string.Join("<br/>", historyRequest.RequestCompletions.Select(rc => $@"
                    <p><strong>Completion Time:</strong> {rc.CompletionTime}</p>
                    <p><strong>Completed By:</strong> {rc.CompletedBy}</p>
                    <p><strong>Image:</strong> <img src='data:image/png;base64,{rc.ImageBase64}' alt='Completion Image' style='width: 40%;' /></p>
                "));

                //< img src = "~/layoutlabs/images/1.png" />
                //< img src = "~/layoutlabs/images/2.png" />
                //< img src = "~/layoutlabs/images/3.png" />

                var ratingStars = string.Join("", Enumerable.Range(1, feedback.Rating).Select(i =>
                    "<img src ='https://localhost:7085/layoutlabs/images/1.png' alt='' width='24' height='24'>")) +
                    string.Join("", Enumerable.Range(1, 5 - feedback.Rating).Select(i =>
                    "<img src ='https://localhost:7085/layoutlabs/images/1-1.png' alt='' width='24' height='24'>"));

                var satisfaction = string.Join("", Enumerable.Range(1, feedback.Satisfaction).Select(i =>
                   "<img src ='https://localhost:7085/layoutlabs/images/2.png' alt='' width='24' height='24'>")) +
                   string.Join("", Enumerable.Range(1, 5 - feedback.Satisfaction).Select(i =>
                   "<img src ='https://localhost:7085/layoutlabs/images/2-2.png' alt='' width='24' height='24'>"));

                var quality = string.Join("", Enumerable.Range(1, feedback.Quality).Select(i =>
                   "<img src ='https://localhost:7085/layoutlabs/images/3.png' alt='' width='24' height='24'>")) +
                   string.Join("", Enumerable.Range(1, 5 - feedback.Quality).Select(i =>
                   "<img src ='https://localhost:7085/layoutlabs/images/3-3.png' alt='' width='24' height='24'>"));


                // Gửi email thông báo (nếu cần)
                var adminEmail = "administrator@example.com";
                var subject = "New Feedback Submitted";
                var body = $@"
                <html lang=""en"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Feedback Received</title>
                        <style>
                            body {{
                                font-family: 'Helvetica Neue', Arial, sans-serif;
                                line-height: 1.8;
                                color: #333;
                                background-color: #f5f7fa;
                                margin: 0;
                                padding: 0;
                            }}
                            .email-container {{
                                max-width: 700px;
                                margin: 30px auto;
                                background: #ffffff;
                                padding: 25px;
                                border-radius: 10px;
                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                                overflow: hidden;
                            }}
                            h1 {{
                                color: #4a90e2;
                                font-size: 24px;
                                margin-bottom: 20px;
                                text-align: center;
                            }}
                            p {{
                                margin: 10px 0;
                                font-size: 16px;
                            }}
                            .content {{
                                margin-top: 15px;
                                padding: 15px;
                                background: #f9f9f9;
                                border: 1px solid #e0e0e0;
                                border-radius: 8px;
                            }}
                            .btn {{
                                display: block;
                                background-color: #4a90e2;
                                color: #fff;
                                padding: 12px 20px;
                                text-align: center;
                                text-decoration: none;
                                border-radius: 6px;
                                margin: 20px auto;
                                width: 200px;
                                font-size: 16px;
                                font-weight: bold;
                            }}
                            .btn:hover {{
                                background-color: #357ab8;
                            }}
                            .footer {{
                                margin-top: 30px;
                                font-size: 14px;
                                color: #888;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""email-container"">
                            <h1>Thank You for Your Feedback</h1>
                            <p>We appreciate you taking the time to provide us with your valuable feedback. Your insights help us improve our services and better serve you in the future.</p>
                            <div class=""content"">
                                <p><strong>Request ID:</strong> {feedback.RequestId}</p>
                                <p><strong>Rating:</strong> {ratingStars}</p>
                                <p><strong>Satisfaction:</strong> {satisfaction}</p>
                                <p><strong>Quality:</strong> {quality}</p>
                                <p style=""white-space: pre-wrap;""><strong>Comment:</strong> {feedback.Comment}</p>
                            </div>
                            <h2 style=""color: #4a90e2; margin-top: 30px;"">Request Completions:</h2>
                            <div class=""content"">
                                {requestCompletionsContent}
                            </div>

                            <p>If you have any additional comments or suggestions, please feel free to reach out to us. Your feedback is invaluable in helping us enhance our offerings.</p>

                            <div class=""footer"">
                                &copy; 2024 Your Company. All rights reserved. <br>
                                Need help? <a href=""mailto:support@yourcompany.com"" style=""color: #4a90e2; text-decoration: none;"">Contact Us</a>
                            </div>
                        </div>
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
