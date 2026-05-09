using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.ViewModels
{

    //โหลดห้อง, ค้นหาห้อง, กรองห้องแนะนำ
    public class UserRoomViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;

        public ObservableCollection<Room> AllRooms { get; set; } = new();
        public ObservableCollection<Room> RecommendedRooms { get; set; } = new();
        public ObservableCollection<Room> FilteredRooms { get; set; } = new();

        public ICommand LoadRoomsCommand { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplySearch(value);
            }
        }

        public UserRoomViewModel()
        {
            _firebaseService = new FirebaseService();
            LoadRoomsCommand = new Command(async () => await LoadRoomsAsync());
        }

        //โหลดห้องทั้งหมดจาก Firebase แยกเป็นแนะนำและทั้งหมด
        public async Task LoadRoomsAsync()
        {
            IsLoading = true;
            try
            {
                var rooms = await _firebaseService.GetAllRooms();
                AllRooms.Clear();
                RecommendedRooms.Clear();
                FilteredRooms.Clear();

                foreach (var room in rooms)
                {
                    // ไม่แสดงห้องที่ปิดปรับปรุงในรายการแนะนำ
                    AllRooms.Add(room);
                    FilteredRooms.Add(room);
                    if (room.IsRecommended && !room.IsUnderMaintenance)
                        RecommendedRooms.Add(room);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        //กรองห้องตามคำค้นหา
        private void ApplySearch(string searchText)
        {
            FilteredRooms.Clear();
            var filtered = string.IsNullOrWhiteSpace(searchText)
                ? AllRooms
                : AllRooms.Where(r => r.RoomDisplayName
                    .ToLower().Contains(searchText.ToLower()));

            foreach (var room in filtered)
                FilteredRooms.Add(room);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
