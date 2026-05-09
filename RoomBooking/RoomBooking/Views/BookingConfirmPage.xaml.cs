using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.Views
{
    // หน้ายืนยันรายละเอียดการจองก่อนชำระเงิน
    public partial class BookingConfirmPage : ContentPage
    {
        private Booking _booking;
        private Room _room;

        // สร้างหน้าและแสดงสรุปการจอง
        public BookingConfirmPage(Booking booking, Room room)
        {
            InitializeComponent();
            _booking = booking;
            _room = room;

            // โหลดรูปห้อง
            imgRoom.Source = FirebaseService.GetImageSource(_room.ImagePath);

            lblRoom.Text = booking.RoomName;
            lblDate.Text = booking.BookingDate.ToString("dd/MM/yyyy");
            lblTime.Text = $"{booking.StartTime:hh\\:mm} - {booking.EndTime:hh\\:mm}";
            lblUser.Text = !string.IsNullOrEmpty(booking.UserName) ? booking.UserName : booking.UserEmail;
            lblTotal.Text = $"฿{booking.TotalPrice:N0}";
        }

        // กดยืนยัน ไปหน้าชำระเงิน
        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            // ไปหน้าชำระเงิน
            await Navigation.PushAsync(new PaymentPage(_booking));
        }

        // กดปุ่มย้อนกลับ
        private async void OnBackClicked(object sender, EventArgs e)
            => await Navigation.PopAsync();
    }
}
