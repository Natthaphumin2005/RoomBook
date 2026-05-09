using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.Views
{
    // หน้าแก้ไขข้อมูลห้องที่มีอยู่
    public partial class EditRoomPage : ContentPage
    {
        private Room _roomToEdit;
        private FirebaseService _firebaseService = new FirebaseService();
        private string _newImageBase64 = "";

        // สร้างหน้าแก้ไขและโหลดข้อมูลห้อง
        public EditRoomPage(Room room)
        {
            InitializeComponent();
            _roomToEdit = room;
            LoadRoomData();
        }

        // โหลดข้อมูลห้องเดิมใส่ฟอร์ม
        private void LoadRoomData()
        {
            if (_roomToEdit == null) return;

            // โหลดรูป
            if (!string.IsNullOrEmpty(_roomToEdit.ImagePath))
            {
                if (_roomToEdit.ImagePath.Length > 100)
                {
                    byte[] imageBytes = Convert.FromBase64String(_roomToEdit.ImagePath);
                    imgPreview.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                else
                {
                    imgPreview.Source = ImageSource.FromFile(_roomToEdit.ImagePath);
                }
                imgPreview.IsVisible = true;
            }

            entryRoomName.Text = _roomToEdit.RoomDisplayName;

            entryCapacity.Text = _roomToEdit.CapacityText?
                .Replace("ความจุ: ", "").Replace("จุได้ ", "").Replace(" คน", "").Trim();

            // ดึงราคาจาก PriceText (รองรับทุก format)
            entryPrice.Text = _roomToEdit.PriceText?
                .Replace("ราคาปกติ: ", "").Replace("ธรรมดา: ", "")
                .Replace("฿", "").Replace("/ชม.", "").Trim();

            editorEquipment.Text = _roomToEdit.Equipment;
            entryInsurance.Text = _roomToEdit.InsuranceAmount;
            switchAvailable.IsToggled = (_roomToEdit.StatusText == "ว่าง");
            switchRecommended.IsToggled = _roomToEdit.IsRecommended;
            switchMaintenance.IsToggled = _roomToEdit.IsUnderMaintenance;
        }

        // แตะเลือกรูปภาพห้องใหม่จาก Gallery
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

                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    byte[] imageBytes = ms.ToArray();
                    _newImageBase64 = Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ข้อผิดพลาด", $"ไม่สามารถเลือกรูปได้: {ex.Message}", "ตกลง");
            }
        }

        // บันทึกการแก้ไขห้องลง Firebase
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                _roomToEdit.RoomDisplayName = entryRoomName.Text;
                _roomToEdit.CapacityText = $"จุได้ {entryCapacity.Text} คน";
                _roomToEdit.PriceText = $"ราคา: ฿{entryPrice.Text}/ชม.";
                _roomToEdit.PriceWeekendText = _roomToEdit.PriceText;
                _roomToEdit.Equipment = editorEquipment.Text;
                _roomToEdit.InsuranceAmount = entryInsurance.Text;
                _roomToEdit.IsRecommended = switchRecommended.IsToggled;
                _roomToEdit.IsUnderMaintenance = switchMaintenance.IsToggled;

                if (switchMaintenance.IsToggled)
                {
                    _roomToEdit.StatusText = "ปรับปรุง";
                    _roomToEdit.StatusColorHex = "#F59E0B";
                }
                else if (switchAvailable.IsToggled)
                {
                    _roomToEdit.StatusText = "ว่าง";
                    _roomToEdit.StatusColorHex = "#10B981";
                }
                else
                {
                    _roomToEdit.StatusText = "ไม่ว่าง";
                    _roomToEdit.StatusColorHex = "#EF4444";
                }

                if (!string.IsNullOrEmpty(_newImageBase64))
                    _roomToEdit.ImagePath = _newImageBase64;

                await _firebaseService.UpdateRoom(_roomToEdit);
                await DisplayAlert("สำเร็จ", "แก้ไขข้อมูลห้องเรียบร้อย", "ตกลง");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("ข้อผิดพลาด", ex.Message, "ตกลง");
            }
        }

        // กดปุ่มย้อนกลับ
        private async void OnBackClicked(object sender, EventArgs e) => await Navigation.PopAsync();
    }
}
