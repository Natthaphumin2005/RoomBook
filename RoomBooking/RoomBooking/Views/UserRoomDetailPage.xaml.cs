using RoomBooking.Models;

namespace RoomBooking.Views
{
    // Popup แสดงรายละเอียดห้องสำหรับผู้ใช้
    public partial class UserRoomDetailPage : ContentPage
    {
        private Room _currentRoom;

        // สร้าง Popup และผูกข้อมูลห้อง
        public UserRoomDetailPage(Room room)
        {
            InitializeComponent();
            _currentRoom = room;
            BindingContext = _currentRoom;
        }

        // แตะพื้นหลังเพื่อปิด Popup
        private async void OnBackgroundTapped(object sender, TappedEventArgs e)
            => await Navigation.PopModalAsync();

        // กดปุ่มปิด Popup
        private async void OnCloseTapped(object sender, EventArgs e)
            => await Navigation.PopModalAsync();

        // กดปุ่มจอง ตรวจสอบสถานะห้องก่อนไปหน้าจอง
        private async void OnBookClicked(object sender, EventArgs e)
        {
            if (_currentRoom.IsUnderMaintenance)
            {
                await DisplayAlert("ไม่สามารถจองได้", "ห้องนี้ปิดปรับปรุงชั่วคราว", "ตกลง");
                return;
            }

            if (_currentRoom.StatusText != "ว่าง")
            {
                await DisplayAlert("ไม่สามารถจองได้", "ห้องนี้ไม่ว่างในขณะนี้", "ตกลง");
                return;
            }

            await Navigation.PopModalAsync();
            await Shell.Current.Navigation.PushAsync(new BookingPage(_currentRoom));
        }
    }
}
