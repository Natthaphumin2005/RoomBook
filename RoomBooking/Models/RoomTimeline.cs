namespace RoomBooking.Models
{
    // Model สำหรับแสดงตารางเวลาของแต่ละห้อง ใช้ใน Admin Timeline view
    public class RoomTimeline
    {
        // ชื่อห้องที่แสดงในตาราง
        public string RoomName { get; set; }

        // รายการ time slot ทั้งหมดของห้องนี้ (08:00 - 19:00)
        public List<TimeSlot> Slots { get; set; } = new();
    }
}
