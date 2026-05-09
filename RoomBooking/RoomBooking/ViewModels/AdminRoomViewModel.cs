using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.ViewModels
{
    // โหลดห้อง, ค้นหา, ลบห้อง

    public class AdminRoomViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;

        public ObservableCollection<Room> AllRooms { get; set; } = new();
        public ObservableCollection<Room> RecommendedRooms { get; set; } = new();

        public ICommand LoadRoomsCommand { get; }
        public ICommand DeleteRoomCommand { get; }

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
                FilterRooms(value);
            }
        }

        public AdminRoomViewModel()
        {
            _firebaseService = new FirebaseService();
            LoadRoomsCommand = new Command(async () => await LoadRoomsAsync());
            DeleteRoomCommand = new Command<Room>(async (room) => await DeleteRoomAsync(room));
        }

        //โหลดข้อมูลห้องทั้งหมดจาก Firebase
        public async Task LoadRoomsAsync()
        {
            IsLoading = true;
            try
            {
                var rooms = await _firebaseService.GetAllRooms();
                AllRooms.Clear();
                RecommendedRooms.Clear();

                foreach (var room in rooms)
                {
                    AllRooms.Add(room);
                    if (room.IsRecommended)
                        RecommendedRooms.Add(room);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        //กรองห้องตามคำค้นหา
        private void FilterRooms(string searchText)
        {
        }

        //ลบห้องออกจาก Firebase และ Collection
        private async Task DeleteRoomAsync(Room room)
        {
            if (room == null) return;
            bool success = await _firebaseService.DeleteRoom(room.Id);
            if (success)
            {
                AllRooms.Remove(room);
                RecommendedRooms.Remove(room);
            }
        }

        // --- INotifyPropertyChanged ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
