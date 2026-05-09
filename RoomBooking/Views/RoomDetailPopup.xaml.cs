using System;
using RoomBooking.Models;
using Microsoft.Maui.Controls;
using RoomBooking.Services; // เพิ่มการใช้งาน Service

namespace RoomBooking.Views
{
    // Popup แสดงรายละเอียดห้องสำหรับ Admin
    public partial class RoomDetailPopup : ContentPage
    {
        private Room _currentRoom;
        // ประกาศเรียกใช้ FirebaseService
        private FirebaseService _firebaseService = new FirebaseService();

        // สร้าง Popup และผูกข้อมูลห้อง
        public RoomDetailPopup(Room room)
        {
            InitializeComponent();
            _currentRoom = room;
            BindingContext = _currentRoom;
        }

        // กดปุ่มปิด Popup
        private async void OnCloseTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        // กดปุ่มแก้ไข ไปหน้า EditRoomPage
        private async void OnEditClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PopModalAsync();
                await Shell.Current.Navigation.PushAsync(new EditRoomPage(_currentRoom));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // กดปุ่มลบห้อง ตรวจสอบการจองก่อนลบ
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            // เช็คก่อนว่ามีการจองที่ยังใช้งานอยู่ไหม
            var today = DateTime.Today;
            var upcomingBookings = await _firebaseService.GetBookingsByRoom(_currentRoom.Id, today);

            // เช็คการจองในอนาคตด้วย (ดึงทั้งหมดแล้วกรอง)
            bool hasActiveBookings = upcomingBookings.Any(b => b.Status != "Cancelled");

            if (hasActiveBookings)
            {
                await DisplayAlert("ไม่สามารถลบได้",
                    $"ห้อง '{_currentRoom.RoomDisplayName}' มีการจองที่ยังใช้งานอยู่\nกรุณายกเลิกการจองทั้งหมดก่อน",
                    "ตกลง");
                return;
            }

            bool isConfirm = await DisplayAlert("ยืนยันการลบ",
                $"คุณต้องการลบห้อง '{_currentRoom.RoomDisplayName}' ใช่หรือไม่?",
                "ลบ", "ยกเลิก");

            if (isConfirm)
            {
                try
                {
                    await _firebaseService.DeleteRoom(_currentRoom.Id);
                    await Navigation.PopModalAsync();
                    await DisplayAlert("สำเร็จ", "ลบข้อมูลห้องเรียบร้อยแล้ว", "ตกลง");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("เกิดข้อผิดพลาด", $"ไม่สามารถลบข้อมูลได้: {ex.Message}", "ตกลง");
                }
            }
        }
    }
}
