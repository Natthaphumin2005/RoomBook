using RoomBooking.Models;
using RoomBooking.ViewModels;

namespace RoomBooking.Views
{
    // หน้าแสดงรายการห้องสำหรับผู้ใช้
    public partial class UserRoomPage : ContentPage
    {
        // ใช้ ViewModel แทนการเรียก Service โดยตรง
        private readonly UserRoomViewModel _viewModel;

        // สร้างหน้าและผูก ViewModel กับ View
        public UserRoomPage()
        {
            InitializeComponent();

            // สร้าง ViewModel และผูกเข้ากับ View ผ่าน BindingContext
            _viewModel = new UserRoomViewModel();
            BindingContext = _viewModel;

            // ผูก ItemsSource จาก ViewModel
            cvRecommended.ItemsSource = _viewModel.RecommendedRooms;
            cvAllRooms.ItemsSource = _viewModel.FilteredRooms;
        }

        // โหลดข้อมูลใหม่ทุกครั้งที่เข้าหน้านี้
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadRoomsAsync();
            // รีเซ็ต ItemsSource หลังโหลดเสร็จ
            cvAllRooms.ItemsSource = _viewModel.FilteredRooms;
        }

        // ค้นหาห้อง 
        private void OnSearchChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SearchText = e.NewTextValue ?? string.Empty;
            cvAllRooms.ItemsSource = _viewModel.FilteredRooms;
        }

        // กดที่การ์ดห้อง เปิด Popup รายละเอียด
        private async void OnRoomTapped(object sender, TappedEventArgs e)
        {
            var selectedRoom = e.Parameter as Room;
            if (selectedRoom != null)
                await Navigation.PushModalAsync(new UserRoomDetailPage(selectedRoom));
        }
    }
}
