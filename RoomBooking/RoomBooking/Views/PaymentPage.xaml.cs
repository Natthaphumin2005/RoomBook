using RoomBooking.Models;
using RoomBooking.Services;
using QRCoder;

namespace RoomBooking.Views
{
    // หน้าชำระเงินผ่าน PromptPay QR Code
    public partial class PaymentPage : ContentPage
    {
        private Booking _booking;
        private FirebaseService _firebaseService = new FirebaseService();

        // เบอร์ PromptPay ของร้าน — เปลี่ยนเป็นเบอร์จริงได้เลย
        private const string PromptPayNumber = "0812345678";

        // สร้างหน้าชำระเงินและสร้าง QR Code
        public PaymentPage(Booking booking)
        {
            InitializeComponent();
            _booking = booking;

            // โหลดรูปห้อง
            imgRoom.Source = FirebaseService.GetImageSource(_booking.RoomImagePath);

            lblRoom.Text = booking.RoomName;
            lblDate.Text = booking.BookingDate.ToString("dd/MM/yyyy");
            lblTime.Text = $"{booking.StartTime:hh\\:mm} - {booking.EndTime:hh\\:mm}";
            lblTotal.Text = $"฿{booking.TotalPrice:N0}";
            lblPromptPayNumber.Text = $"เบอร์: {PromptPayNumber}";

            GenerateQrCode();
        }

        // สร้าง QR Code จากข้อมูลการจอง 
        private void GenerateQrCode()
        {
            try
            {
                // สร้าง text สรุปการจองแทน PromptPay payload
                string qrContent = $"RoomBooking\n" +
                                   $"ห้อง: {_booking.RoomName}\n" +
                                   $"วันที่: {_booking.BookingDate:dd/MM/yyyy}\n" +
                                   $"เวลา: {_booking.StartTime:hh\\:mm} - {_booking.EndTime:hh\\:mm}\n" +
                                   $"ยอด: {_booking.TotalPrice:N0} บาท\n" +
                                   $"PromptPay: {PromptPayNumber}";

                var qrGenerator = new QRCodeGenerator();
                var qrData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.M);
                var qrCode = new BitmapByteQRCode(qrData);
                byte[] qrBytes = qrCode.GetGraphic(10);

                imgQrCode.Source = ImageSource.FromStream(() => new MemoryStream(qrBytes));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"QR Error: {ex.Message}");
            }
        }

        // ยืนยันการชำระเงินและบันทึกการจองลง Firebase
        private async void OnConfirmPaymentClicked(object sender, EventArgs e)
        {
            try
            {
                // เปลี่ยนเป็น Confirmed ทันทีที่ชำระเงิน
                _booking.Status = "Confirmed";
                string bookingId = await _firebaseService.AddBooking(_booking);

                if (!string.IsNullOrEmpty(bookingId))
                {
                    await DisplayAlert("จองสำเร็จ",
                        "ชำระเงินและจองห้องเรียบร้อยแล้ว", "ตกลง");
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    await DisplayAlert("ข้อผิดพลาด", "ไม่สามารถบันทึกการจองได้ กรุณาลองใหม่", "ตกลง");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ข้อผิดพลาด", ex.Message, "ตกลง");
            }
        }

        // กดปุ่มย้อนกลับ
        private async void OnBackClicked(object sender, EventArgs e)
            => await Navigation.PopAsync();
    }
}
