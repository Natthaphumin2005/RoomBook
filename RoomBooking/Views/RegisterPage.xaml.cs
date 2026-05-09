using RoomBooking.ViewModels;

namespace RoomBooking.Views
{
    // หน้าสมัครสมาชิกผู้ใช้ใหม่
    public partial class RegisterPage : ContentPage
    {
        // ใช้ ViewModel แทนการเรียก Service โดยตรง
        private readonly RegisterViewModel _viewModel;

        // สร้างหน้าสมัครสมาชิกและผูก ViewModel
        public RegisterPage()
        {
            InitializeComponent();
            _viewModel = new RegisterViewModel();
            BindingContext = _viewModel;

            // รับ event เมื่อสมัครสำเร็จ แล้วกลับหน้า Login
            _viewModel.RegisterSuccess += async () =>
            {
                await DisplayAlert("สำเร็จ", "สมัครสมาชิกเรียบร้อยแล้ว", "ตกลง");
                await Navigation.PopAsync();
            };
        }

        // กดปุ่มสมัครสมาชิก ส่งข้อมูลให้ ViewModel
        private void btnRegister_Clicked(object sender, EventArgs e)
        {
            // ส่งค่าจาก View ไปให้ ViewModel จัดการ
            _viewModel.FullName = entryName.Text;
            _viewModel.Email = entryEmail.Text;
            _viewModel.Phone = entryPhone.Text;
            _viewModel.Password = entryPassword.Text;
            _viewModel.RegisterCommand.Execute(null);

            lblError.Text = _viewModel.ErrorText;
            lblError.IsVisible = _viewModel.HasError;
        }

        // แตะลิงก์กลับหน้า Login
        private async void OnLoginTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
