using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.ViewModels
{
    // แสดงสรุปข้อมูล, คำนวณยอดรวม, บันทึกห้องลง Firebase
    public class SummaryViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;
        private Room _room;

        public string RoomName => _room?.RoomDisplayName;
        public string Capacity => _room?.CapacityText;
        public string Equipment => string.IsNullOrEmpty(_room?.Equipment) ? "-" : _room.Equipment;
        public string Insurance => $"฿{_room?.InsuranceAmount}";
        public string Status => _room?.StatusText;

        public string PriceDisplay => $"฿{_room?.PriceText}";

        public string TotalDisplay
        {
            get
            {
                decimal price = 0, ins = 0;
                decimal.TryParse(_room?.PriceText, out price);
                decimal.TryParse(_room?.InsuranceAmount, out ins);
                return $"฿{(price + ins):N0}";
            }
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get => _isSaving;
            set { _isSaving = value; OnPropertyChanged(); }
        }

        public event Action SaveSuccess;

        public ICommand SaveCommand { get; }

        public SummaryViewModel(Room room)
        {
            _firebaseService = new FirebaseService();
            _room = room;
            SaveCommand = new Command(async () => await SaveAsync());
        }

        //บันทึกห้องใหม่ลง Firebase พร้อม format ข้อมูล
        private async Task SaveAsync()
        {
            IsSaving = true;
            try
            {
                _room.CapacityText = $"จุได้ {_room.CapacityText} คน";
                _room.PriceText = $"ราคา: ฿{_room.PriceText}/ชม.";
                _room.PriceWeekendText = _room.PriceText;

                await _firebaseService.AddRoom(_room);
                SaveSuccess?.Invoke();
            }
            finally { IsSaving = false; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
