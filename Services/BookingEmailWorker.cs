using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ASP.Data;
using ASP.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace ASP.Services
{
    public class BookingEmailWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BookingEmailWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                    // Find bookings that are paid but email not sent
                    var pendingEmails = context.Bookings
                        .Include(b => b.Room)
                        .Where(b => b.IsDepositPaid && !b.IsEmailSent)
                        .ToList();

                    foreach (var booking in pendingEmails)
                    {
                        try
                        {
                            var subject = "Đã nhận tiền cọc - LuxManage Resort";
                            var body = $@"
                                <div style='font-family: Arial, sans-serif; padding: 25px; border: 1px solid #e0e0e0; border-radius: 12px;'>
                                    <h2 style='color: #28a745;'>Thanh toán thành công!</h2>
                                    <p>Chào <b>{booking.CustomerName}</b>,</p>
                                    <p>LuxManage Resort xác nhận đã nhận được số tiền cọc: <b>{booking.DepositAmount:N0} VNĐ</b> cho đơn đăng ký phòng <b>#{booking.Id}</b>.</p>
                                    <div style='background: #f8f9fa; padding: 15px; border-radius: 8px;'>
                                        <p><b>Phòng:</b> {booking.Room?.RoomNumber} ({booking.Room?.RoomType})</p>
                                        <p><b>Trạng thái:</b> Đã giữ phòng thành công</p>
                                    </div>
                                    <p style='margin-top: 20px;'>Hẹn gặp lại bạn vào ngày <b>{booking.CheckIn:dd/MM/yyyy}</b>. Chúc bạn có một kỳ nghỉ tuyệt vời!</p>
                                    <hr>
                                    <p style='font-size: 0.8em; color: #999;'>Đây là email xác nhận tự động từ hệ thống quản lý.</p>
                                </div>";

                            await emailService.SendEmailAsync(booking.CustomerEmail, subject, body);
                            
                            // Mark as sent
                            booking.IsEmailSent = true;
                            context.SaveChanges();
                            
                            Console.WriteLine($"[Worker] Successfully sent payment email for Booking #{booking.Id}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Worker] Error sending email for Booking #{booking.Id}: {ex.Message}");
                        }
                    }
                }

                await Task.Delay(5000, stoppingToken); // Check every 5 seconds
            }
        }
    }
}
