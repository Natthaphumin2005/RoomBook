namespace RoomBooking.Models
{
    // Model สำหรับเก็บข้อมูลการจองห้อง
    public class Booking
    {
        // รหัสการจองที่ Firebase สร้างให้อัตโนมัติ
        public string Id { get; set; }

        // รหัสห้องที่จอง (อ้างอิงกับ Room.Id)
        public string RoomId { get; set; }

        // ชื่อห้องที่จอง (เก็บไว้เพื่อแสดงผลโดยไม่ต้อง query ห้องซ้ำ)
        public string RoomName { get; set; }

        // รูปห้องในรูปแบบ Base64 สำหรับแสดงในรายการจอง
        public string RoomImagePath { get; set; }

        // อีเมลของผู้จอง
        public string UserEmail { get; set; }

        // ชื่อจริงของผู้จอง (ดึงจาก User profile)
        public string UserName { get; set; }

        // วันที่จอง
        public DateTime BookingDate { get; set; }

        // เวลาเริ่มต้น
        public TimeSpan StartTime { get; set; }

        // เวลาสิ้นสุด
        public TimeSpan EndTime { get; set; }

        // ยอดรวมที่ต้องชำระ (ค่าห้อง + ค่าประกัน)
        public double TotalPrice { get; set; }

        // สถานะการจอง: "Pending", "Confirmed", "Cancelled"
        public string Status { get; set; }

        // แปลงเวลาเป็นข้อความ เช่น "09:00 - 11:00"
        public string TimeRange => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";

        // แปลงสถานะเป็นภาษาไทย
        public string StatusThai
        {
            get
            {
                if (Status == "Confirmed") return "จองแล้ว";
                if (Status == "Cancelled") return "ยกเลิก";
                return "รอตรวจสอบ";
            }
        }

        // สี Hex ของ badge สถานะ
        public string StatusColor
        {
            get
            {
                if (Status == "Confirmed") return "#10B981"; // สีเขียว
                if (Status == "Cancelled") return "#EF4444"; // สีแดง
                return "#F59E0B";                            // สีส้ม (Pending)
            }
        }

        // true = ยังรอการตรวจสอบ ใช้ซ่อน/แสดงปุ่มอนุมัติ-ปฏิเสธ
        public bool IsPending => Status == "Pending" || string.IsNullOrEmpty(Status);
    }
}
