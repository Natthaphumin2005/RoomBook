using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Services;
using Microsoft.Maui.Storage;

namespace RoomBooking.ViewModels
{
    //ตรวจสอบ email/password, แยก role Admin/User

    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;
 
        private string _email = "";
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _errorText = "";
        public string ErrorText
        {
            get => _errorText;
            set { _errorText = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasError)); }
        }

        public bool HasError => !string.IsNullOrEmpty(_errorText);

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public event Action<string> LoginSuccess; // ส่ง role กลับไป

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            _firebaseService = new FirebaseService();
            LoginCommand = new Command(async () => await LoginAsync());
        }

        //ตรวจสอบข้อมูล Login และแยก role
        private async Task LoginAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorText = "กรุณากรอกอีเมลและรหัสผ่าน";
                return;
            }

            IsLoading = true;
            ErrorText = "";

            try
            {
                // ตรวจสอบ Admin
                if (Email == "admin@test.com" && Password == "1234")
                {
                    Preferences.Default.Set("user_email", Email);
                    Preferences.Default.Set("user_role", "admin");
                    LoginSuccess?.Invoke("admin");
                    return;
                }

                // ตรวจสอบ User จาก Firebase
                var user = await _firebaseService.GetUserProfile(Email);
                if (user != null && user.Password == Password)
                {
                    Preferences.Default.Set("user_email", Email);
                    Preferences.Default.Set("user_role", "user");
                    LoginSuccess?.Invoke("user");
                }
                else
                {
                    ErrorText = "อีเมลหรือรหัสผ่านไม่ถูกต้อง";
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
