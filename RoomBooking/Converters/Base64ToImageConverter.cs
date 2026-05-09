using System.Globalization;

namespace RoomBooking.Converters
{
    // แปลง Base64 หรือ path เป็น ImageSource สำหรับ Binding
    public class Base64ToImageConverter : IValueConverter
    {
        // แปลงค่า string เป็น ImageSource
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                if (str.Length > 100) // Base64
                {
                    try
                    {
                        byte[] bytes = System.Convert.FromBase64String(str);
                        return ImageSource.FromStream(() => new MemoryStream(bytes));
                    }
                    catch { }
                }
                else
                {
                    return ImageSource.FromFile(str);
                }
            }
            return ImageSource.FromFile("dotnet_bot.png");
        }

        // ไม่รองรับการแปลงกลับ
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
