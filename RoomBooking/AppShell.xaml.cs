using RoomBooking.Views;

namespace RoomBooking;
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // ลงทะเบียน route สำหรับหน้าที่ push เข้ามา 
        Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
        Routing.RegisterRoute("AddRoomPage", typeof(AddRoomPage));
        Routing.RegisterRoute("EditRoomPage", typeof(EditRoomPage));
        Routing.RegisterRoute("SummaryPage", typeof(SummaryPage));
        Routing.RegisterRoute("BookingPage", typeof(BookingPage));
        Routing.RegisterRoute("BookingConfirmPage", typeof(BookingConfirmPage));
        Routing.RegisterRoute("UserRoomDetailPage", typeof(UserRoomDetailPage));
        Routing.RegisterRoute("PaymentPage", typeof(PaymentPage));
        Routing.RegisterRoute("BookingDetailPage", typeof(BookingDetailPage));
    }

    // เรียกหลัง Login สำเร็จ เพื่อสลับไปยัง TabBar ที่ถูกต้องตาม role
    public void NavigateToRole(string role)
    {
        if (role == "admin")
        {
            // ซ่อน Login และ User TabBar แสดง Admin TabBar
            LoginShell.IsVisible = false;
            UserTabBar.IsVisible = false;
            AdminTabBar.IsVisible = true;
        }
        else
        {
            // ซ่อน Login และ Admin TabBar แสดง User TabBar
            LoginShell.IsVisible = false;
            AdminTabBar.IsVisible = false;
            UserTabBar.IsVisible = true;
        }
    }

    // เรียกตอน Logout เพื่อกลับมาหน้า Login
    public void NavigateToLogin()
    {
        AdminTabBar.IsVisible = false;
        UserTabBar.IsVisible = false;
        LoginShell.IsVisible = true;
    }
}
