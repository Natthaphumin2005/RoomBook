namespace RoomBooking.Models
{
    // Model สำหรับแสดง time slot ในตารางเวลา
    public class TimeSlot
    {
        // เวลาเริ่มต้นของ slot
        public TimeSpan Start { get; set; }

        // เวลาสิ้นสุดของ slot
        public TimeSpan End { get; set; }

        // true = มีการจองที่ Confirmed แล้ว
        public bool IsBooked { get; set; }

        // true = มีการจองที่รอตรวจสอบ (Pending)
        public bool IsPending { get; set; }

        // แสดงเวลาเริ่มต้น เช่น "09:00"
        public string TimeLabel => $"{Start:hh\\:mm}";

        // ข้อความสถานะที่แสดงใน slot
        public string StatusLabel
        {
            get
            {
                if (IsPending) return "รอตรวจ";
                return IsBooked ? "ไม่ว่าง" : "ว่าง";
            }
        }

        // สีพื้นหลังของ slot ตามสถานะ
        public string BgColor
        {
            get
            {
                if (IsPending) return "#FEF9C3"; // เหลือง
                return IsBooked ? "#FEE2E2" : "#DCFCE7"; // แดง / เขียว
            }
        }

        // สีขอบของ slot ตามสถานะ
        public string BorderColor
        {
            get
            {
                if (IsPending) return "#FDE047";
                return IsBooked ? "#FECACA" : "#BBF7D0";
            }
        }

        // สีข้อความของ slot ตามสถานะ
        public string TextColor
        {
            get
            {
                if (IsPending) return "#854D0E";
                return IsBooked ? "#DC2626" : "#16A34A";
            }
        }
    }
}
