using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Models;
using RoomBooking.Services;
using Microsoft.Maui.Storage;

namespace RoomBooking.ViewModels
{

    // โหลดการจองของ User, ยกเลิกการจอง
    public class UserBookingViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;

        public ObservableCollection<Booking> Bookings { get; set; } = new();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ICommand LoadBookingsCommand { get; }
        public ICommand CancelBookingCommand { get; }

        public UserBookingViewModel()
        {
            _firebaseService = new FirebaseService();
            LoadBookingsCommand = new Command(async () => await LoadBookingsAsync());
            CancelBookingCommand = new Command<Booking>(async (b) => await CancelBookingAsync(b));
        }

        //โหลดประวัติการจองของ User ที่ login อยู่
        public async Task LoadBookingsAsync()
        {
            IsLoading = true;
            try
            {
                string email = Preferences.Default.Get("user_email", "");
                if (string.IsNullOrEmpty(email)) return;

                var list = await _firebaseService.GetBookingsByUser(email);
                Bookings.Clear();
                foreach (var b in list) Bookings.Add(b);
            }
            finally { IsLoading = false; }
        }

        //ยกเลิกการจอง
        private async Task CancelBookingAsync(Booking booking)
        {
            if (booking == null) return;
            booking.Status = "Cancelled";
            await _firebaseService.UpdateBookingStatus(booking);
            await LoadBookingsAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
