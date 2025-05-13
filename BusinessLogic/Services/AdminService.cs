using BusinessLogic.Helper;
using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class AdminService:IAdminService
    {
        private readonly IProductRepository _productRepository;
        public readonly EmailService _emailService;

        public AdminService(IProductRepository productRepository, EmailService emailService)
        {
            _productRepository = productRepository;
            _emailService = emailService;
        }


        public async Task<CommonResponse<object>> UpdateSellerStatus(string id, string status)
        {
            try
            {
                if (!int.TryParse(status, out int parsedStatus))
                {
                    return new CommonResponse<object>(400, "Invalid status format. Status must be an integer.", null);
                }
                var rowsAffected = await _productRepository.UpdateSellerStatus(id, parsedStatus);
                if (rowsAffected == 0)
                {
                    return new CommonResponse<object>(404, "Seller not found or status unchanged.", null);
                }

                var sellerEmail = await _productRepository.GetSellerEmailById(id);
                var (subject, body) = EmailTemplateService.GetSellerStatusUpdateEmail(sellerEmail, status);
                await _emailService.SendEmailAsync(sellerEmail, subject, body);
                string statusMessage = parsedStatus switch
                {
                    1 => "Seller approved successfully.",
                    0 => "Seller status updated to pending.",
                    -1 => "Seller rejected successfully.",
                    _ => "Seller status updated."
                };

                return new CommonResponse<object>(200, statusMessage, null);
            }
            catch (Exception ex)
            {
                return new CommonResponse<object>(500, $"An error occurred: {ex.Message}", null);
            }
        }

    }
}
