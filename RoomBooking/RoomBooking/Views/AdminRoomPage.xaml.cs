using RoomBooking.Models;
using RoomBooking.ViewModels;

namespace RoomBooking.Views
{
    // หน้าจัดการห้องสำหรับ Admin
    public partial class AdminRoomPage : ContentPage
    {
        // ใช้ ViewModel แทนการเรียก Service โดยตรง
        private readonly AdminRoomViewModel _viewModel;

        // สร้างหน้าและผูก ViewModel กับ View
        public AdminRoomPage()
        {
            InitializeComponent();

            // สร้าง ViewModel และผูกเข้ากับ View ผ่าน BindingContext
            _viewModel = new AdminRoomViewModel();
            BindingContext = _viewModel;

            // ผูก ItemsSource จาก ViewModel
            cvRecommended.ItemsSource = _viewModel.RecommendedRooms;
            cvAllRooms.ItemsSource = _viewModel.AllRooms;
        }

        // โหลดข้อมูลใหม่ทุกครั้งที่เข้าหน้านี้
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadRoomsAsync();
        }

        // ปุ่มเพิ่มห้อง — Navigation อยู่ใน View เพราะต้องการ Page reference
        private async void OnAddRoomClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddRoomPage());
        }

        // ค้นหาห้อง — ส่งค่าไปให้ ViewModel จัดการ
        private void OnSearchChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.ToLower() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                cvAllRooms.ItemsSource = _viewModel.AllRooms;
            }
            else
            {
                cvAllRooms.ItemsSource = _viewModel.AllRooms
                    .Where(r => r.RoomDisplayName.ToLower().Contains(searchText))
                    .ToList();
            }
        }

        // กดที่การ์ดห้อง เปิด Popup รายละเอียด
        private async void OnRoomTapped(object sender, TappedEventArgs e)
        {
            var selectedRoom = e.Parameter as Room;
            if (selectedRoom != null)
                await Navigation.PushModalAsync(new RoomDetailPopup(selectedRoom));
        }
    }
}
