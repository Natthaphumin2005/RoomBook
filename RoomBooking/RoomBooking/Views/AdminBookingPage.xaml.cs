using RoomBooking.Models;
using RoomBooking.ViewModels;
using RoomBooking.Extensions;

namespace RoomBooking.Views
{
    // หน้าจัดการการจองสำหรับ Admin
    public partial class AdminBookingPage : ContentPage
    {
        // ใช้ ViewModel แทนการเรียก Service โดยตรง
        private readonly AdminBookingViewModel _viewModel;
        private bool _isTimelineTab = false;

        // สร้างหน้าและผูก ViewModel
        public AdminBookingPage()
        {
            InitializeComponent();
            _viewModel = new AdminBookingViewModel();
            BindingContext = _viewModel;
        }

        // โหลดข้อมูลการจองเมื่อเข้าหน้า
        protected override void OnAppearing()
        {
            base.OnAppearing();
            datePicker.Date = DateTime.Today;
            _ = _viewModel.LoadBookingsAsync(DateTime.Today);
            collBookings.ItemsSource = _viewModel.Bookings;
        }

        // เปลี่ยนวันที่แล้วโหลดข้อมูลใหม่
        private void OnDateChanged(object sender, DateChangedEventArgs e)
        {
            var date = e.NewDate ?? DateTime.Today;
            _viewModel.SelectedDate = date;
            _ = _viewModel.LoadBookingsAsync(date);

            if (_isTimelineTab)
                _ = _viewModel.LoadTimelineAsync(date);
        }

        // รีเฟรชรายการการจอง
        private void OnRefreshing(object sender, EventArgs e)
        {
            _ = _viewModel.LoadBookingsAsync(datePicker.Date ?? DateTime.Today);
            refreshView.IsRefreshing = false;
        }

        // สลับ Tab รายการจอง
        private void OnTabListClicked(object sender, EventArgs e)
        {
            _isTimelineTab = false;
            refreshView.IsVisible = true;
            scrollTimeline.IsVisible = false;
            btnTabList.BackgroundColor = Color.FromArgb("#2563EB");
            btnTabList.TextColor = Colors.White;
            btnTabTimeline.BackgroundColor = Colors.White;
            btnTabTimeline.TextColor = Color.FromArgb("#2563EB");
        }

        // สลับ Tab ตารางเวลา
        private async void OnTabTimelineClicked(object sender, EventArgs e)
        {
            _isTimelineTab = true;
            refreshView.IsVisible = false;
            scrollTimeline.IsVisible = true;
            btnTabTimeline.BackgroundColor = Color.FromArgb("#2563EB");
            btnTabTimeline.TextColor = Colors.White;
            btnTabList.BackgroundColor = Colors.White;
            btnTabList.TextColor = Color.FromArgb("#2563EB");

            await _viewModel.LoadTimelineAsync(datePicker.Date ?? DateTime.Today);
            cvTimeline.ItemsSource = _viewModel.Timelines;
        }

        // อนุมัติการจองที่เลือก
        private async void OnApproveClicked(object sender, EventArgs e)
        {
            var booking = (sender as Button)?.CommandParameter as Booking;
            if (booking == null) return;

            bool confirm = await DisplayAlert("ยืนยัน",
                $"อนุมัติการจองห้อง {booking.RoomName} ใช่หรือไม่?", "อนุมัติ", "ยกเลิก");
            if (!confirm) return;

            await _viewModel.ApproveCommand.ExecuteAsync(booking);
            await DisplayAlert("สำเร็จ", "อนุมัติการจองเรียบร้อย", "ตกลง");
        }

        // ปฏิเสธการจองที่เลือก
        private async void OnRejectClicked(object sender, EventArgs e)
        {
            var booking = (sender as Button)?.CommandParameter as Booking;
            if (booking == null) return;

            bool confirm = await DisplayAlert("ยืนยัน",
                "ปฏิเสธการจองนี้ใช่หรือไม่?", "ปฏิเสธ", "ยกเลิก");
            if (!confirm) return;

            await _viewModel.RejectCommand.ExecuteAsync(booking);
        }

        // กดที่รายการจอง เปิดหน้ารายละเอียด
        private async void OnBookingTapped(object sender, TappedEventArgs e)
        {
            var booking = e.Parameter as Booking;
            if (booking != null)
                await Navigation.PushAsync(new BookingDetailPage(booking));
        }
    }
}
