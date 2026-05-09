using RoomBooking.Models;
using RoomBooking.Services;
using Microsoft.Maui.Storage;

namespace RoomBooking.Views
{
    // หน้าเลือกวันเวลาและจองห้อง
    public partial class BookingPage : ContentPage
    {
        private Room _room;
        private FirebaseService _firebaseService = new FirebaseService();
        private string _userName = "";

        // สร้างหน้าจองและแสดงข้อมูลห้อง
        public BookingPage(Room room)
        {
            InitializeComponent();
            _room = room;

            imgRoom.Source = FirebaseService.GetImageSource(_room.ImagePath);
            lblRoomName.Text = _room.RoomDisplayName;
            lblCapacity.Text = _room.CapacityText;
            lblPrice.Text = _room.PriceText;
            lblInsurance.Text = $"฿{_room.InsuranceAmount}";

            datePicker.Date = DateTime.Today;
            timeStart.Time = new TimeSpan(9, 0, 0);
            timeEnd.Time = new TimeSpan(10, 0, 0);

            CalculatePrice();
            LoadUserName();
            LoadTimeSlots(DateTime.Today);
        }

        // แปลง PriceText ทุก format ให้เป็นตัวเลข
        private double ParsePrice(string priceText)
        {
            if (string.IsNullOrEmpty(priceText)) return 0;
            var clean = priceText
                .Replace("ราคา: ", "").Replace("ธรรมดา: ", "")
                .Replace("ราคาปกติ: ", "").Replace("เสาร์-อาทิตย์: ", "")
                .Replace("฿", "").Replace("/ชม.", "").Trim();
            double.TryParse(clean, out double result);
            return result;
        }

        // โหลดชื่อผู้ใช้จาก Firebase
        private async void LoadUserName()
        {
            string email = Preferences.Default.Get("user_email", "");
            var user = await _firebaseService.GetUserProfile(email);
            _userName = user?.FullName ?? email;
        }

        // โหลด Time Slot ที่ว่างและถูกจองแล้ว
        private async void LoadTimeSlots(DateTime date)
        {
            loadingSlots.IsVisible = true;
            loadingSlots.IsRunning = true;

            var bookings = await _firebaseService.GetBookingsByRoom(_room.Id, date);
            var slots = new List<TimeSlot>();

            for (int h = 8; h < 20; h++)
            {
                var slotStart = new TimeSpan(h, 0, 0);
                var slotEnd = new TimeSpan(h + 1, 0, 0);
                bool isBooked = bookings.Any(b => b.StartTime < slotEnd && b.EndTime > slotStart);
                slots.Add(new TimeSlot { Start = slotStart, End = slotEnd, IsBooked = isBooked });
            }

            cvTimeSlots.ItemsSource = slots;
            loadingSlots.IsRunning = false;
            loadingSlots.IsVisible = false;
        }

        // เปลี่ยนวันที่แล้วคำนวณราคาและโหลด Slot ใหม่
        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            CalculatePrice();
            LoadTimeSlots(e.NewDate ?? DateTime.Today);
        }

        // เปลี่ยนเวลาแล้วคำนวณราคาใหม่
        private void OnTimeChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Time") CalculatePrice();
        }

        // คำนวณราคารวมจากชั่วโมงที่เลือก
        private void CalculatePrice()
        {
            var date = datePicker.Date ?? DateTime.Today;
            TimeSpan start = timeStart.Time ?? new TimeSpan(9, 0, 0);
            TimeSpan end = timeEnd.Time ?? new TimeSpan(10, 0, 0);

            if (end <= start)
            {
                lblRoomCost.Text = "-";
                lblTotal.Text = "-";
                return;
            }

            double hours = (end - start).TotalHours;
            double pricePerHour = ParsePrice(_room.PriceText);
            double insurance = 0;
            double.TryParse(_room.InsuranceAmount, out insurance);

            double roomCost = pricePerHour * hours;
            lblRoomCost.Text = $"฿{roomCost:N0}";
            lblTotal.Text = $"฿{(roomCost + insurance):N0}";
        }

        // ยืนยันการจองและตรวจสอบความขัดแย้งของเวลา
        private async void OnConfirmClicked(object sender, EventArgs e)
        {
            var date = datePicker.Date ?? DateTime.Today;
            TimeSpan start = timeStart.Time ?? new TimeSpan(9, 0, 0);
            TimeSpan end = timeEnd.Time ?? new TimeSpan(10, 0, 0);

            if (end <= start)
            {
                lblError.Text = "เวลาสิ้นสุดต้องมากกว่าเวลาเริ่มต้น";
                lblError.IsVisible = true;
                return;
            }

            if ((end - start).TotalHours < 0.5)
            {
                lblError.Text = "ต้องจองอย่างน้อย 30 นาที";
                lblError.IsVisible = true;
                return;
            }

            lblError.IsVisible = false;

            var existingBookings = await _firebaseService.GetBookingsByRoom(_room.Id, date);
            bool isConflict = existingBookings.Any(b => b.StartTime < end && b.EndTime > start);

            if (isConflict)
            {
                lblError.Text = "ช่วงเวลานี้มีการจองแล้ว กรุณาเลือกเวลาอื่น";
                lblError.IsVisible = true;
                return;
            }

            double pricePerHour = ParsePrice(_room.PriceText);
            double insurance = 0;
            double.TryParse(_room.InsuranceAmount, out insurance);
            double hours = (end - start).TotalHours;
            double total = (pricePerHour * hours) + insurance;

            var booking = new Booking
            {
                RoomId = _room.Id,
                RoomName = _room.RoomDisplayName,
                RoomImagePath = _room.ImagePath,
                UserEmail = Preferences.Default.Get("user_email", ""),
                UserName = _userName,
                BookingDate = date,
                StartTime = start,
                EndTime = end,
                TotalPrice = total,
                Status = "Pending"
            };

            await Navigation.PushAsync(new BookingConfirmPage(booking, _room));
        }

        // กดปุ่มย้อนกลับ
        private async void OnBackClicked(object sender, EventArgs e)
            => await Navigation.PopAsync();
    }
}
