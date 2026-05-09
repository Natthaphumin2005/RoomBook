using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.ViewModels
{
    //โหลดการจองตามวัน, อนุมัติ/ปฏิเสธ, โหลด Timeline
    public class AdminBookingViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;

        public ObservableCollection<Booking> Bookings { get; set; } = new();
        public ObservableCollection<RoomTimeline> Timelines { get; set; } = new();

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set { _selectedDate = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ICommand LoadBookingsCommand { get; }
        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }
        public ICommand LoadTimelineCommand { get; }

        public AdminBookingViewModel()
        {
            _firebaseService = new FirebaseService();
            LoadBookingsCommand = new Command<DateTime>(async (date) => await LoadBookingsAsync(date));
            ApproveCommand = new Command<Booking>(async (b) => await ApproveAsync(b));
            RejectCommand = new Command<Booking>(async (b) => await RejectAsync(b));
            LoadTimelineCommand = new Command<DateTime>(async (date) => await LoadTimelineAsync(date));
        }

        //โหลดรายการจองตามวันที่เลือก
        public async Task LoadBookingsAsync(DateTime date)
        {
            IsLoading = true;
            try
            {
                var list = await _firebaseService.GetBookingsByDate(date);
                Bookings.Clear();
                foreach (var b in list) Bookings.Add(b);
            }
            finally { IsLoading = false; }
        }

        /// <summary>อนุมัติการจอง</summary>
        private async Task ApproveAsync(Booking booking)
        {
            if (booking == null) return;
            booking.Status = "Confirmed";
            await _firebaseService.UpdateBookingStatus(booking);
            await LoadBookingsAsync(SelectedDate);
        }

        //ปฏิเสธการจอง
        private async Task RejectAsync(Booking booking)
        {
            if (booking == null) return;
            booking.Status = "Cancelled";
            await _firebaseService.UpdateBookingStatus(booking);
            await LoadBookingsAsync(SelectedDate);
        }

        //โหลด Timeline ทุกห้องสำหรับวันที่เลือก
        public async Task LoadTimelineAsync(DateTime date)
        {
            IsLoading = true;
            try
            {
                var rooms = await _firebaseService.GetAllRooms();
                Timelines.Clear();

                foreach (var room in rooms)
                {
                    var allBookings = await _firebaseService.GetBookingsByDate(date);
                    var roomBookings = allBookings
                        .Where(b => b.RoomId == room.Id && b.Status != "Cancelled")
                        .ToList();

                    var slots = new List<TimeSlot>();
                    for (int h = 8; h < 20; h++)
                    {
                        var slotStart = new TimeSpan(h, 0, 0);
                        var slotEnd = new TimeSpan(h + 1, 0, 0);
                        var matched = roomBookings.FirstOrDefault(b =>
                            b.StartTime < slotEnd && b.EndTime > slotStart);

                        slots.Add(new TimeSlot
                        {
                            Start = slotStart,
                            End = slotEnd,
                            IsBooked = matched?.Status == "Confirmed",
                            IsPending = matched?.Status == "Pending"
                        });
                    }

                    Timelines.Add(new RoomTimeline
                    {
                        RoomName = room.RoomDisplayName,
                        Slots = slots
                    });
                }
            }
            finally { IsLoading = false; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
