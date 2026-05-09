namespace RoomBooking.Models
{
    // Model สำหรับเก็บข้อมูลผู้ใช้งาน
    public class User
    {
        // ชื่อ-นามสกุล
        public string FullName { get; set; }

        // อีเมล (ใช้เป็น key ใน Firebase โดยแทนที่ . และ @ ด้วย _)
        public string Email { get; set; }

        // เบอร์โทรศัพท์
        public string Phone { get; set; }

        // รหัสผ่าน (เก็บเป็น plain text สำหรับ demo)
        public string Password { get; set; }

        // path รูปโปรไฟล์ (local path หรือ Base64)
        public string ProfileImage { get; set; }
    }
}
