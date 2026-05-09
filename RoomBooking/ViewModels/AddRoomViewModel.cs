using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.ViewModels
{
    //เก็บข้อมูลห้อง, ตรวจสอบข้อมูล, ส่งต่อไปหน้า Summary
    public class AddRoomViewModel : INotifyPropertyChanged
    {
        private string _roomName = "";
        public string RoomName
        {
            get => _roomName;
            set { _roomName = value; OnPropertyChanged(); }
        }

        private string _capacity = "";
        public string Capacity
        {
            get => _capacity;
            set { _capacity = value; OnPropertyChanged(); }
        }

        private string _price = "";
        public string Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        private string _equipment = "";
        public string Equipment
        {
            get => _equipment;
            set { _equipment = value; OnPropertyChanged(); }
        }

        private string _insurance = "";
        public string Insurance
        {
            get => _insurance;
            set { _insurance = value; OnPropertyChanged(); }
        }

        private bool _isAvailable = true;
        public bool IsAvailable
        {
            get => _isAvailable;
            set { _isAvailable = value; OnPropertyChanged(); }
        }

        private bool _isRecommended;
        public bool IsRecommended
        {
            get => _isRecommended;
            set { _isRecommended = value; OnPropertyChanged(); }
        }

        private bool _isUnderMaintenance;
        public bool IsUnderMaintenance
        {
            get => _isUnderMaintenance;
            set { _isUnderMaintenance = value; OnPropertyChanged(); }
        }

        private string _imageBase64 = "";
        public string ImageBase64
        {
            get => _imageBase64;
            set { _imageBase64 = value; OnPropertyChanged(); }
        }

        private string _errorText = "";
        public string ErrorText
        {
            get => _errorText;
            set { _errorText = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasError)); }
        }

        public bool HasError => !string.IsNullOrEmpty(_errorText);

        public event Action<Room> NavigateToSummary;

        public ICommand NextCommand { get; }

        public AddRoomViewModel()
        {
            NextCommand = new Command(ValidateAndNext);
        }

        //ตรวจสอบข้อมูลและสร้าง Room object ส่งต่อไปหน้า Summary
        private void ValidateAndNext()
        {
            if (string.IsNullOrWhiteSpace(RoomName) || string.IsNullOrWhiteSpace(Price))
            {
                ErrorText = "กรุณากรอกชื่อห้องและราคา";
                return;
            }

            ErrorText = "";

            string statusText = IsUnderMaintenance ? "ปรับปรุง"
                              : IsAvailable ? "ว่าง" : "ไม่ว่าง";
            string statusColor = IsUnderMaintenance ? "#F59E0B"
                               : IsAvailable ? "#10B981" : "#EF4444";

            var room = new Room
            {
                RoomDisplayName = RoomName,
                CapacityText = Capacity,
                PriceText = Price,
                PriceWeekendText = Price,
                StatusText = statusText,
                StatusColorHex = statusColor,
                ImagePath = ImageBase64,
                IsRecommended = IsRecommended,
                IsUnderMaintenance = IsUnderMaintenance,
                Equipment = Equipment,
                InsuranceAmount = Insurance
            };

            NavigateToSummary?.Invoke(room);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
