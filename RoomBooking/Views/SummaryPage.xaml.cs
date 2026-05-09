using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.Views
{
    // หน้าสรุปข้อมูลห้องก่อนบันทึก
    public partial class SummaryPage : ContentPage
    {
        private Room _roomData;
        private FirebaseService _firebaseService = new FirebaseService();

        // สร้างหน้าสรุปและแสดงข้อมูลห้อง
        public SummaryPage(Room room)
        {
            InitializeComponent();
            _roomData = room;
            DisplayData();
        }

        // แสดงข้อมูลห้องและคำนวณยอดรวม
        private void DisplayData()
        {
            // แสดงข้อมูลพื้นฐาน
            lblRoomName.Text = _roomData.RoomDisplayName;
            lblCapacity.Text = $"{_roomData.CapacityText} คน";
            lblPrice.Text = $"฿{_roomData.PriceText}";
            lblEquipment.Text = string.IsNullOrEmpty(_roomData.Equipment) ? "-" : _roomData.Equipment;
            lblInsurance.Text = $"฿{_roomData.InsuranceAmount}";
            lblAvailable.Text = _roomData.StatusText;

            // คำนวณยอดรวม (ราคา + ประกัน)
            decimal price = 0, insurance = 0;
            decimal.TryParse(_roomData.PriceText, out price);
            decimal.TryParse(_roomData.InsuranceAmount, out insurance);

            lblTotalResult.Text = $"฿{(price + insurance):N0}";
        }

        // กดปุ่มย้อนกลับ
        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        // บันทึกห้องใหม่ลง Firebase
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                // เก็บ format สะอาด ไม่มีคำนำหน้า
                _roomData.CapacityText = $"จุได้ {_roomData.CapacityText} คน";
                _roomData.PriceText = $"ราคา: ฿{_roomData.PriceText}/ชม.";
                _roomData.PriceWeekendText = _roomData.PriceText;

                await _firebaseService.AddRoom(_roomData);
                await DisplayAlert("สำเร็จ", "บันทึกข้อมูลเรียบร้อยแล้ว", "ตกลง");
                await Navigation.PopToRootAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("ข้อผิดพลาด", ex.Message, "ตกลง");
            }
        }
    }
}
