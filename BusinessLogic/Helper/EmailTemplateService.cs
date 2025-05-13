using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Helper
{
    public static class EmailTemplateService
    {
        public static (string subject, string body) GetCustomerWelcomeEmail(string username)
        {
            var subject = "Welcome to Our Application, Customer";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                    <h2 style='color: #4CAF50;'>Welcome, {username}!</h2>
                    <p>Thank you for registering with us. Your account has been successfully created.</p>
                    <p>We’re excited to have you onboard. Feel free to explore our application and let us know if you have any questions.</p>
                    <br />
                    <p style='color: #888;'>Regards,<br>The Team</p>
                </div>
            </body>
            </html>";
            return (subject, body);
        }

        public static (string subject, string body) GetSellerNotificationEmail(string username)
        {
            var subject = "New Seller Registration Pending Approval";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                    <h2 style='color: #f39c12;'>Seller Registration Notice</h2>
                    <p>A new seller has registered with the following details:</p>
                    <p><strong>Username:</strong> {username}</p>
                    <p>Please review this registration and take the necessary action to approve or reject the account.</p>
                    <br />
                    <p style='color: #888;'>Regards,<br>The Team</p>
                </div>
            </body>
            </html>";
            return (subject, body);
        }

        public static (string subject, string body) GetSellerStatusUpdateEmail(string sellerId, string status)
        {
            var subject = "Your Seller Status Has Been Updated";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                    <h2 style='color: #4CAF50;'>Hello, Seller {sellerId}!</h2>
                    <p>Your seller account status has been updated to: <strong>{status}</strong>.</p>
                    <p>If you have any questions or concerns, please contact our support team.</p>
                    <br />
                    <p style='color: #888;'>Regards,<br>The Team</p>
                </div>
            </body>
            </html>";

            return (subject, body);
        }

    }

}
