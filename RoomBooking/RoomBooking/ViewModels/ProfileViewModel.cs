using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Models;
using RoomBooking.Services;
using Microsoft.Maui.Storage;

namespace RoomBooking.ViewModels
{

    //โหลดข้อมูล User, แก้ไขโปรไฟล์, Logout
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set { _isAdmin = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsUser)); }
        }

        public bool IsUser => !_isAdmin;

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set { _isEditMode = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsViewMode)); }
        }

        public bool IsViewMode => !_isEditMode;

        private string _displayName = "กำลังโหลด...";
        public string DisplayName
        {
            get => _displayName;
            set { _displayName = value; OnPropertyChanged(); }
        }

        private string _roleBadgeText = "";
        public string RoleBadgeText
        {
            get => _roleBadgeText;
            set { _roleBadgeText = value; OnPropertyChanged(); }
        }

        public ICommand LoadProfileCommand { get; }
        public ICommand EditProfileCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand LogoutCommand { get; }

        public ProfileViewModel()
        {
            _firebaseService = new FirebaseService();
            LoadProfileCommand = new Command(async () => await LoadProfileAsync());
            EditProfileCommand = new Command(() => IsEditMode = true);
            CancelEditCommand = new Command(() => IsEditMode = false);
            SaveChangesCommand = new Command(async () => await SaveChangesAsync());
            LogoutCommand = new Command(async () => await LogoutAsync());
        }
        //โหลดข้อมูลโปรไฟล์จาก Firebase หรือ hardcode สำหรับ Admin
        public async Task LoadProfileAsync()
        {
            string email = Preferences.Default.Get("user_email", "");
            string role = Preferences.Default.Get("user_role", "user");
            IsAdmin = role == "admin";

            if (IsAdmin)
            {
                DisplayName = "Administrator";
                RoleBadgeText = "ผู้ดูแลระบบ";
            }
            else
            {
                RoleBadgeText = "ผู้ใช้งาน";
                CurrentUser = await _firebaseService.GetUserProfile(email);
                DisplayName = CurrentUser?.FullName ?? email;
            }
        }

        //บันทึกการแก้ไขโปรไฟล์ลง Firebase
        private async Task SaveChangesAsync()
        {
            if (CurrentUser == null) return;
            bool success = await _firebaseService.UpdateUserProfile(CurrentUser);
            if (success)
            {
                IsEditMode = false;
                DisplayName = CurrentUser.FullName;
            }
        }

        //ออกจากระบบและล้าง Preferences
        public async Task LogoutAsync()
        {
            Preferences.Default.Remove("user_email");
            Preferences.Default.Remove("user_role");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
