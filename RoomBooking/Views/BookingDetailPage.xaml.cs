using RoomBooking.Models;
using RoomBooking.Services;
using Microsoft.Maui.Storage;

namespace RoomBooking.Views
{
    // หน้าแสดงรายละเอียดการจองและจัดการสถานะ
    public partial class BookingDetailPage : ContentPage
    {
        private Booking _booking;
        private FirebaseService _firebaseService = new FirebaseService();
        private bool _isAdmin;

        // สร้างหน้าและโหลดข้อมูลการจอง
        public BookingDetailPage(Booking booking)
        {
            InitializeComponent();
            _booking = booking;
            _isAdmin = Preferences.Default.Get("user_role", "user") == "admin";

            LoadData();
        }

        // โหลดและแสดงข้อมูลการจองบนหน้าจอ
        private void LoadData()
        {
            // รูปห้อง 
            imgRoom.Source = FirebaseService.GetImageSource(_booking.RoomImagePath);

            // สถานะ
            lblStatus.Text = _booking.StatusThai;
            frmStatus.BackgroundColor = Color.FromArgb(_booking.StatusColor);

            // ข้อมูล
            lblRoomName.Text = _booking.RoomName;
            lblDate.Text = _booking.BookingDate.ToString("dd MMMM yyyy");
            lblTime.Text = $"{_booking.StartTime:hh\\:mm} - {_booking.EndTime:hh\\:mm} น.";
            lblUser.Text = !string.IsNullOrEmpty(_booking.UserName) 
                ? _booking.UserName 
                : _booking.UserEmail;
            lblTotal.Text = $"฿{_booking.TotalPrice:N0}";

            // แสดงปุ่มตาม role และสถานะ
            if (_isAdmin)
            {
                gridAdminActions.IsVisible = _booking.IsPending;
            }
            else
            {
                // User ยกเลิกได้เฉพาะที่ยัง Pending
                btnCancel.IsVisible = _booking.IsPending;
            }
        }

        // Admin อนุมัติการจอง
        private async void OnApproveClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("ยืนยัน",
                $"อนุมัติการจองห้อง {_booking.RoomName} ใช่หรือไม่?", "อนุมัติ", "ยกเลิก");
            if (!confirm) return;

            _booking.Status = "Confirmed";
            await _firebaseService.UpdateBookingStatus(_booking);
            await DisplayAlert("สำเร็จ", "อนุมัติการจองเรียบร้อย", "ตกลง");
            await Navigation.PopAsync();
        }

        // Admin ปฏิเสธการจอง
        private async void OnRejectClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("ยืนยัน",
                "ปฏิเสธการจองนี้ใช่หรือไม่?", "ปฏิเสธ", "ยกเลิก");
            if (!confirm) return;

            _booking.Status = "Cancelled";
            await _firebaseService.UpdateBookingStatus(_booking);
            await Navigation.PopAsync();
        }

        // ผู้ใช้ยกเลิกการจองของตัวเอง
        private async void OnCancelBookingClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("ยืนยัน",
                "ยกเลิกการจองนี้ใช่หรือไม่?", "ยกเลิกการจอง", "ไม่");
            if (!confirm) return;

            _booking.Status = "Cancelled";
            await _firebaseService.UpdateBookingStatus(_booking);
            await DisplayAlert("สำเร็จ", "ยกเลิกการจองเรียบร้อย", "ตกลง");
            await Navigation.PopAsync();
        }

        // กดปุ่มย้อนกลับ
        private async void OnBackClicked(object sender, EventArgs e)
            => await Navigation.PopAsync();
    }
}
