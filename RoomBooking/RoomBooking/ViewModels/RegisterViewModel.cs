using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RoomBooking.Models;
using RoomBooking.Services;

namespace RoomBooking.ViewModels
{
    //ตรวจสอบข้อมูล, บันทึก User ลง Firebase
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;

        private string _fullName = "";
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _email = "";
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _phone = "";
        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
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

        public event Action RegisterSuccess;

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            _firebaseService = new FirebaseService();
            RegisterCommand = new Command(async () => await RegisterAsync());
        }

        //ตรวจสอบข้อมูลและบันทึก User ลง Firebase
        private async Task RegisterAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorText = "กรุณากรอกอีเมลและรหัสผ่าน";
                return;
            }

            ErrorText = "";

            var newUser = new User
            {
                FullName = FullName,
                Email = Email,
                Phone = Phone,
                Password = Password
            };

            bool success = await _firebaseService.RegisterUser(newUser);
            if (success)
                RegisterSuccess?.Invoke();
            else
                ErrorText = "ไม่สามารถลงทะเบียนได้ กรุณาลองใหม่";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
