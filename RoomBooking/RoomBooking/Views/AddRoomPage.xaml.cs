using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.Views
{
    // หน้าเพิ่มห้องใหม่โดย Admin
    public partial class AddRoomPage : ContentPage
    {
        private string _selectedImageBase64 = "";

        // สร้างหน้าเพิ่มห้อง
        public AddRoomPage()
        {
            InitializeComponent();
        }

        // กดปุ่มย้อนกลับ
        private async void OnBackClicked(object sender, EventArgs e)
            => await Navigation.PopAsync();

        // แตะเลือกรูปภาพห้องจาก Gallery
        private async void OnPickImageTapped(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "เลือกรูปภาพห้อง",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    imgPreview.Source = ImageSource.FromFile(result.FullPath);
                    imgPreview.IsVisible = true;
                    stackUploadInfo.IsVisible = false;

                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    byte[] imageBytes = ms.ToArray();
                    _selectedImageBase64 = Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ข้อผิดพลาด", $"ไม่สามารถเลือกรูปได้: {ex.Message}", "ตกลง");
            }
        }

        // กดถัดไป ตรวจสอบข้อมูลและไปหน้าสรุป
        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(entryRoomName.Text) ||
                string.IsNullOrWhiteSpace(entryPrice.Text))
            {
                lblError.Text = "กรุณากรอกชื่อห้องและราคา";
                lblError.IsVisible = true;
                return;
            }

            lblError.IsVisible = false;

            string statusText = switchMaintenance.IsToggled ? "ปรับปรุง"
                              : switchAvailable.IsToggled ? "ว่าง" : "ไม่ว่าง";
            string statusColor = switchMaintenance.IsToggled ? "#F59E0B"
                               : switchAvailable.IsToggled ? "#10B981" : "#EF4444";

            var newRoom = new Room
            {
                RoomDisplayName = entryRoomName.Text,
                CapacityText = entryCapacity.Text,
                PriceText = entryPrice.Text,
                PriceWeekendText = entryPrice.Text, 
                StatusText = statusText,
                ImagePath = _selectedImageBase64,
                IsRecommended = switchRecommended.IsToggled,
                IsUnderMaintenance = switchMaintenance.IsToggled,
                StatusColorHex = statusColor,
                Equipment = editorEquipment.Text,
                InsuranceAmount = entryInsurance.Text
            };

            await Navigation.PushAsync(new SummaryPage(newRoom));
        }
    }
}
