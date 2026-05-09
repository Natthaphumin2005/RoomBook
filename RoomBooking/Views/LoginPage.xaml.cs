using RoomBooking.ViewModels;

namespace RoomBooking.Views;

// หน้า Login ตรวจสอบสิทธิ์ผู้ใช้
public partial class LoginPage : ContentPage
{
    // ใช้ ViewModel แทนการเรียก Service โดยตรง
    private readonly LoginViewModel _viewModel;

    // สร้างหน้า Login และผูก ViewModel
    public LoginPage()
    {
        InitializeComponent();
        _viewModel = new LoginViewModel();
        BindingContext = _viewModel;

        // รับ event เมื่อ Login สำเร็จ แล้ว Navigate ไปหน้าที่ถูกต้อง
        _viewModel.LoginSuccess += (role) =>
        {
            (Shell.Current as AppShell)?.NavigateToRole(role);
        };
    }

    // กดปุ่ม Login ส่งข้อมูลไปให้ ViewModel
    private void btnLogin_Clicked(object sender, EventArgs e)
    {
        // ส่งค่าจาก View ไปให้ ViewModel จัดการ
        _viewModel.Email = entryEmail.Text;
        _viewModel.Password = entryPassword.Text;
        _viewModel.LoginCommand.Execute(null);

        // แสดง error ถ้ามี
        lblError.Text = _viewModel.ErrorText;
        lblError.IsVisible = _viewModel.HasError;
    }

    // แตะลิงก์ไปหน้าสมัครสมาชิก
    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushAsync(new RegisterPage());
    }
}
