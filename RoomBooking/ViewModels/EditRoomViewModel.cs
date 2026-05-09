using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.ViewModels
{
    //รับผิดชอบ: โหลดข้อมูลห้องเดิม, บันทึกการแก้ไขลง Firebase

    public class EditRoomViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;
        private Room _room;

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

        private bool _isAvailable;
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

        private bool _isSaving;
        public bool IsSaving
        {
            get => _isSaving;
            set { _isSaving = value; OnPropertyChanged(); }
        }

        public event Action SaveSuccess;

        public ICommand SaveCommand { get; }

        public EditRoomViewModel(Room room)
        {
            _firebaseService = new FirebaseService();
            _room = room;
            SaveCommand = new Command(async () => await SaveAsync());
            LoadRoomData();
        }

        //โหลดข้อมูลห้องเดิมมาแสดงในฟอร์ม
        private void LoadRoomData()
        {
            RoomName = _room.RoomDisplayName;
            Capacity = _room.CapacityText?
                .Replace("จุได้ ", "").Replace(" คน", "").Trim();
            Price = _room.PriceText?
                .Replace("ราคา: ", "").Replace("ธรรมดา: ", "")
                .Replace("฿", "").Replace("/ชม.", "").Trim();
            Equipment = _room.Equipment;
            Insurance = _room.InsuranceAmount;
            IsAvailable = _room.StatusText == "ว่าง";
            IsRecommended = _room.IsRecommended;
            IsUnderMaintenance = _room.IsUnderMaintenance;
        }

        //บันทึกการแก้ไขลง 
        private async Task SaveAsync()
        {
            IsSaving = true;
            try
            {
                _room.RoomDisplayName = RoomName;
                _room.CapacityText = $"จุได้ {Capacity} คน";
                _room.PriceText = $"ราคา: ฿{Price}/ชม.";
                _room.PriceWeekendText = _room.PriceText;
                _room.Equipment = Equipment;
                _room.InsuranceAmount = Insurance;
                _room.IsRecommended = IsRecommended;
                _room.IsUnderMaintenance = IsUnderMaintenance;

                if (IsUnderMaintenance)
                {
                    _room.StatusText = "ปรับปรุง";
                    _room.StatusColorHex = "#F59E0B";
                }
                else if (IsAvailable)
                {
                    _room.StatusText = "ว่าง";
                    _room.StatusColorHex = "#10B981";
                }
                else
                {
                    _room.StatusText = "ไม่ว่าง";
                    _room.StatusColorHex = "#EF4444";
                }

                await _firebaseService.UpdateRoom(_room);
                SaveSuccess?.Invoke();
            }
            finally { IsSaving = false; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
