using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Models;
using RoomBooking.Services;
using Microsoft.Maui.Storage;

namespace RoomBooking.ViewModels
{
    //รับผิดชอบ: คำนวณราคา, ตรวจสอบเวลาว่าง, สร้าง Booking
    public class BookingViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;
        private readonly Room _room;

        public ObservableCollection<TimeSlot> TimeSlots { get; set; } = new();

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                _ = LoadTimeSlotsAsync(value);
                CalculatePrice();
            }
        }

        private TimeSpan _startTime = new TimeSpan(9, 0, 0);
        public TimeSpan StartTime
        {
            get => _startTime;
            set { _startTime = value; OnPropertyChanged(); CalculatePrice(); }
        }

        private TimeSpan _endTime = new TimeSpan(10, 0, 0);
        public TimeSpan EndTime
        {
            get => _endTime;
            set { _endTime = value; OnPropertyChanged(); CalculatePrice(); }
        }

        private string _roomCostText = "-";
        public string RoomCostText
        {
            get => _roomCostText;
            set { _roomCostText = value; OnPropertyChanged(); }
        }

        private string _totalText = "-";
        public string TotalText
        {
            get => _totalText;
            set { _totalText = value; OnPropertyChanged(); }
        }

        private string _errorText = "";
        public string ErrorText
        {
            get => _errorText;
            set { _errorText = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasError)); }
        }

        public bool HasError => !string.IsNullOrEmpty(_errorText);

        private bool _isLoadingSlots;
        public bool IsLoadingSlots
        {
            get => _isLoadingSlots;
            set { _isLoadingSlots = value; OnPropertyChanged(); }
        }

        public ICommand ConfirmCommand { get; }

        public BookingViewModel(Room room)
        {
            _firebaseService = new FirebaseService();
            _room = room;
            ConfirmCommand = new Command(async () => await ValidateAndConfirmAsync());
            _ = LoadTimeSlotsAsync(DateTime.Today);
            CalculatePrice();
        }

        //โหลด time slot และเช็คว่าช่วงไหนถูกจองแล้ว
        public async Task LoadTimeSlotsAsync(DateTime date)
        {
            IsLoadingSlots = true;
            try
            {
                var bookings = await _firebaseService.GetBookingsByRoom(_room.Id, date);
                TimeSlots.Clear();

                for (int h = 8; h < 20; h++)
                {
                    var slotStart = new TimeSpan(h, 0, 0);
                    var slotEnd = new TimeSpan(h + 1, 0, 0);
                    bool isBooked = bookings.Any(b => b.StartTime < slotEnd && b.EndTime > slotStart);

                    TimeSlots.Add(new TimeSlot
                    {
                        Start = slotStart,
                        End = slotEnd,
                        IsBooked = isBooked
                    });
                }
            }
            finally
            {
                IsLoadingSlots = false;
            }
        }

        //คำนวณราคาตามชั่วโมงที่เลือก
        public void CalculatePrice()
        {
            if (EndTime <= StartTime)
            {
                RoomCostText = "-";
                TotalText = "-";
                return;
            }

            double hours = (EndTime - StartTime).TotalHours;
            double pricePerHour = ParsePrice(_room.PriceText);
            double insurance = 0;
            double.TryParse(_room.InsuranceAmount, out insurance);

            double roomCost = pricePerHour * hours;
            RoomCostText = $"฿{roomCost:N0}";
            TotalText = $"฿{(roomCost + insurance):N0}";
        }

        //ตรวจสอบข้อมูลก่อนยืนยันการจอง
        private async Task ValidateAndConfirmAsync()
        {
            ErrorText = "";

            if (EndTime <= StartTime)
            {
                ErrorText = "เวลาสิ้นสุดต้องมากกว่าเวลาเริ่มต้น";
                return;
            }

            if ((EndTime - StartTime).TotalHours < 0.5)
            {
                ErrorText = "ต้องจองอย่างน้อย 30 นาที";
                return;
            }

            // เช็คห้องว่าง
            var existingBookings = await _firebaseService.GetBookingsByRoom(_room.Id, SelectedDate);
            bool isConflict = existingBookings.Any(b => b.StartTime < EndTime && b.EndTime > StartTime);

            if (isConflict)
            {
                ErrorText = "ช่วงเวลานี้มีการจองแล้ว กรุณาเลือกเวลาอื่น";
                return;
            }
        }

        //สร้าง Booking object จากข้อมูลที่กรอก
        public async Task<Booking> CreateBookingAsync(string userName)
        {
            double pricePerHour = ParsePrice(_room.PriceText);
            double insurance = 0;
            double.TryParse(_room.InsuranceAmount, out insurance);
            double hours = (EndTime - StartTime).TotalHours;
            double total = (pricePerHour * hours) + insurance;

            return new Booking
            {
                RoomId = _room.Id,
                RoomName = _room.RoomDisplayName,
                RoomImagePath = _room.ImagePath,
                UserEmail = Preferences.Default.Get("user_email", ""),
                UserName = userName,
                BookingDate = SelectedDate,
                StartTime = StartTime,
                EndTime = EndTime,
                TotalPrice = total,
                Status = "Pending"
            };
        }

        //แปลง PriceText ทุก format ให้เป็นตัวเลข
        private double ParsePrice(string priceText)
        {
            if (string.IsNullOrEmpty(priceText)) return 0;
            var clean = priceText
                .Replace("ราคา: ", "").Replace("ธรรมดา: ", "")
                .Replace("ราคาปกติ: ", "").Replace("฿", "")
                .Replace("/ชม.", "").Trim();
            double.TryParse(clean, out double result);
            return result;
        }

        // --- INotifyPropertyChanged ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
