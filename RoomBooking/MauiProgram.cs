using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace RoomBooking
{
    // จุดเริ่มต้นของแอป MAUI กำหนดค่า services และ fonts
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader() // เปิดใช้งาน ZXing สำหรับสร้าง QR Code
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
