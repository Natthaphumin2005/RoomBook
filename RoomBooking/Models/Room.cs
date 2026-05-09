namespace RoomBooking.Models
{
    // Model สำหรับเก็บข้อมูลห้องประชุม
    public class Room
    {
        // รหัสห้องที่ Firebase สร้างให้อัตโนมัติ
        public string Id { get; set; }

        // ชื่อห้องที่แสดงบนหน้าจอ
        public string RoomDisplayName { get; set; }

        // ข้อความแสดงความจุ เช่น "จุได้ 10 คน"
        public string CapacityText { get; set; }

        // ราคาต่อชั่วโมง เช่น "ราคา: ฿500/ชม."
        public string PriceText { get; set; }

        // ราคาเสาร์-อาทิตย์ (ปัจจุบันใช้ราคาเดียวกับ PriceText)
        public string PriceWeekendText { get; set; }

        // สถานะห้อง: "ว่าง", "ไม่ว่าง", "ปรับปรุง"
        public string StatusText { get; set; }

        // รูปภาพห้องในรูปแบบ Base64 string
        public string ImagePath { get; set; }

        // true = แสดงในรายการห้องแนะนำ
        public bool IsRecommended { get; set; }

        // true = ห้องปิดปรับปรุง ไม่รับการจอง
        public bool IsUnderMaintenance { get; set; }

        // รายการอุปกรณ์ภายในห้อง เช่น "โปรเจคเตอร์, ไวท์บอร์ด"
        public string Equipment { get; set; }

        // ค่าประกันความเสียหาย (ตัวเลขล้วน)
        public string InsuranceAmount { get; set; }

        // สี Hex ของ badge สถานะ เช่น "#10B981"
        public string StatusColorHex { get; set; }

        // แปลงคำเก่าในราคาให้เป็น "ราคา:" เพื่อแสดงผลที่สม่ำเสมอ
        public string PriceDisplayText => PriceText?
            .Replace("ธรรมดา: ", "ราคา: ")
            .Replace("ราคาปกติ: ", "ราคา: ");
    }
}
