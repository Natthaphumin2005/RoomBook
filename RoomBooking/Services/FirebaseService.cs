using Firebase.Database;
using Firebase.Database.Query;
using RoomBooking.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace RoomBooking.Services
{
    // Service หลักสำหรับติดต่อกับ Firebase Realtime Database
    // ครอบคลุม: ห้อง, ผู้ใช้, การจอง
    public class FirebaseService
    {
        // URL ของ Firebase Realtime Database
        private readonly string FirebaseUrl = "https://roombook-967c5-default-rtdb.asia-southeast1.firebasedatabase.app/";

        // Firebase client สำหรับเรียก API
        private readonly FirebaseClient firebaseClient;

        public FirebaseService()
        {
            // สร้าง client เชื่อมต่อกับ Firebase
            firebaseClient = new FirebaseClient(FirebaseUrl);
        }

        // ==================== ส่วนที่ 1: จัดการห้อง ====================

        // ดึงห้องทั้งหมดจาก Firebase
        public async Task<ObservableCollection<Room>> GetAllRooms()
        {
            try
            {
                var rooms = await firebaseClient.Child("Rooms").OnceAsync<Room>();
                var roomList = new ObservableCollection<Room>();
                foreach (var item in rooms)
                {
                    // นำ Key ของ Firebase มาใช้เป็น Id
                    var room = item.Object;
                    room.Id = item.Key;
                    roomList.Add(room);
                }
                return roomList;
            }
            catch { return new ObservableCollection<Room>(); }
        }

        // อัปเดตข้อมูลห้องที่มีอยู่แล้ว
        public async Task UpdateRoom(Room room)
        {
            if (string.IsNullOrEmpty(room.Id)) return;
            await firebaseClient.Child("Rooms").Child(room.Id).PutAsync(room);
        }

        // เพิ่มห้องใหม่ลง Firebase
        public async Task AddRoom(Room room)
        {
            await firebaseClient.Child("Rooms").PostAsync(room);
        }

        // ลบห้องออกจาก Firebase ตาม roomId
        public async Task<bool> DeleteRoom(string roomId)
        {
            try
            {
                await firebaseClient.Child("Rooms").Child(roomId).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting room: {ex.Message}");
                return false;
            }
        }

        // ==================== ส่วนที่ 2: จัดการผู้ใช้งาน ====================

        // ลงทะเบียนผู้ใช้ใหม่ โดยใช้ email เป็น key (แทนที่ . และ @ ด้วย _)
        public async Task<bool> RegisterUser(User user)
        {
            try
            {
                string safeEmail = user.Email.Replace(".", "_").Replace("@", "_");
                await firebaseClient.Child("Users").Child(safeEmail).PutAsync(user);
                return true;
            }
            catch { return false; }
        }

        // ดึงข้อมูลโปรไฟล์ผู้ใช้จาก email
        public async Task<User> GetUserProfile(string email)
        {
            try
            {
                string safeEmail = email.Replace(".", "_").Replace("@", "_");
                return await firebaseClient
                    .Child("Users")
                    .Child(safeEmail)
                    .OnceSingleAsync<User>();
            }
            catch { return null; }
        }

        // อัปเดตข้อมูลโปรไฟล์ผู้ใช้
        public async Task<bool> UpdateUserProfile(User user)
        {
            try
            {
                string safeEmail = user.Email.Replace(".", "_").Replace("@", "_");
                await firebaseClient
                    .Child("Users")
                    .Child(safeEmail)
                    .PutAsync(user);
                return true;
            }
            catch { return false; }
        }

        // ==================== ส่วนที่ 3: จัดการการจอง ====================

        // ดึงการจองทั้งหมดของวันที่ระบุ
        public async Task<List<Booking>> GetBookingsByDate(DateTime date)
        {
            try
            {
                var bookings = await firebaseClient.Child("Bookings").OnceAsync<Booking>();
                return bookings
                    .Select(item => {
                        var b = item.Object;
                        b.Id = item.Key;
                        return b;
                    })
                    .Where(b => b.BookingDate.Date == date.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetBookings: {ex.Message}");
                return new List<Booking>();
            }
        }

        // บันทึกการจองใหม่ลง Firebase และคืน Key ที่ได้
        public async Task<string> AddBooking(Booking booking)
        {
            try
            {
                var result = await firebaseClient.Child("Bookings").PostAsync(booking);
                return result.Key;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error AddBooking: {ex.Message}");
                return null;
            }
        }

        // อัปเดตสถานะการจอง (Confirmed / Cancelled)
        public async Task UpdateBookingStatus(Booking booking)
        {
            try
            {
                if (string.IsNullOrEmpty(booking.Id)) return;
                await firebaseClient.Child("Bookings").Child(booking.Id).PutAsync(booking);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error UpdateStatus: {ex.Message}");
            }
        }

        // ดึงประวัติการจองทั้งหมดของ User คนหนึ่ง เรียงจากใหม่ไปเก่า
        public async Task<List<Booking>> GetBookingsByUser(string email)
        {
            try
            {
                var bookings = await firebaseClient.Child("Bookings").OnceAsync<Booking>();
                return bookings
                    .Select(item => { var b = item.Object; b.Id = item.Key; return b; })
                    .Where(b => b.UserEmail == email)
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetBookingsByUser: {ex.Message}");
                return new List<Booking>();
            }
        }

        // ดึงการจองของห้องหนึ่งในวันที่ระบุ (ไม่รวมที่ยกเลิก)
        public async Task<List<Booking>> GetBookingsByRoom(string roomId, DateTime date)
        {
            try
            {
                var bookings = await firebaseClient.Child("Bookings").OnceAsync<Booking>();
                return bookings
                    .Select(item => { var b = item.Object; b.Id = item.Key; return b; })
                    .Where(b => b.RoomId == roomId
                             && b.BookingDate.Date == date.Date
                             && b.Status != "Cancelled")
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetBookingsByRoom: {ex.Message}");
                return new List<Booking>();
            }
        }

        // ==================== Helper ====================

        // แปลง ImagePath (Base64 หรือ file path) เป็น ImageSource สำหรับแสดงผล
        public static ImageSource GetImageSource(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return ImageSource.FromFile("dotnet_bot.png");

            // ถ้าเป็น Base64 string (ความยาวมากกว่า 100 ตัวอักษร)
            if (imagePath.Length > 100)
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(imagePath);
                    return ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                catch
                {
                    return ImageSource.FromFile("dotnet_bot.png");
                }
            }

            // ถ้าเป็น file path ปกติ
            return ImageSource.FromFile(imagePath);
        }
    }
}
