using RoomBooking.Models;
using RoomBooking.ViewModels;

namespace RoomBooking.Views
{
    // หน้าแสดงรายการจองของผู้ใช้
    public partial class UserBookingPage : ContentPage
    {
        // ใช้ ViewModel แทนการเรียก Service โดยตรง
        private readonly UserBookingViewModel _viewModel;

        // สร้างหน้าและผูก ViewModel
        public UserBookingPage()
        {
            InitializeComponent();
            _viewModel = new UserBookingViewModel();
            BindingContext = _viewModel;
        }

        // โหลดรายการจองเมื่อเข้าหน้า
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = _viewModel.LoadBookingsAsync();
            collBookings.ItemsSource = _viewModel.Bookings;
        }

        // รีเฟรชรายการจอง
        private void OnRefreshing(object sender, EventArgs e)
        {
            _ = _viewModel.LoadBookingsAsync();
            refreshView.IsRefreshing = false;
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
