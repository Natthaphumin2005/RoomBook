using RoomBooking.Models;
using RoomBooking.Services;
using RoomBooking.ViewModels;
using Microsoft.Maui.Storage;

namespace RoomBooking.Views
{
    // หน้าโปรไฟล์และตั้งค่าบัญชีผู้ใช้
    public partial class ProfilePage : ContentPage
    {
        // ใช้ ViewModel แทนการเรียก Service โดยตรง
        private readonly ProfileViewModel _viewModel;
        private string _selectedImagePath;

        // สร้างหน้าโปรไฟล์และผูก ViewModel
        public ProfilePage()
        {
            InitializeComponent();

            // สร้าง ViewModel และผูกเข้ากับ View
            _viewModel = new ProfileViewModel();
            BindingContext = _viewModel;
        }

        // โหลดข้อมูลใหม่ทุกครั้งที่เข้าหน้านี้
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ViewLayout.IsVisible = true;
            EditLayout.IsVisible = false;
            await _viewModel.LoadProfileAsync();
            UpdateUI();
        }

        // อัปเดต UI จากข้อมูลใน ViewModel
        private void UpdateUI()
        {
            lblDisplayUsername.Text = _viewModel.DisplayName;
            lblRole.Text = _viewModel.RoleBadgeText;
            frmRoleBadge.IsVisible = _viewModel.IsAdmin;
            frmRoleBadge.BackgroundColor = Color.FromArgb("#7C3AED");
            btnEditProfile.IsVisible = _viewModel.IsUser;
            lblChangePhoto.IsVisible = _viewModel.IsUser;

            if (_viewModel.CurrentUser != null)
            {
                lblDisplayEmail.Text = _viewModel.CurrentUser.Email;
                lblDisplayPhone.Text = _viewModel.CurrentUser.Phone;
                entName.Text = _viewModel.CurrentUser.FullName;
                entEmail.Text = _viewModel.CurrentUser.Email;
                entPhone.Text = _viewModel.CurrentUser.Phone;
                entPassword.Text = _viewModel.CurrentUser.Password;
                LoadAvatar(_viewModel.CurrentUser.ProfileImage);
            }
            else
            {
                lblDisplayEmail.Text = Preferences.Default.Get("user_email", "");
                lblDisplayPhone.Text = "-";
                if (_viewModel.IsAdmin) imgAvatar.Source = "admin_avartar.png";
            }
        }

        // โหลดรูปโปรไฟล์จาก path ที่กำหนด
        private void LoadAvatar(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                imgAvatar.Source = ImageSource.FromFile(imagePath);
                imgAvatarEdit.Source = ImageSource.FromFile(imagePath);
            }
            else
            {
                imgAvatar.Source = "profile.png";
                imgAvatarEdit.Source = "profile.png";
            }
        }

        // แตะเปลี่ยนรูปโปรไฟล์จาก Gallery
        private async void OnChangePhotoTapped(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "เลือกรูปโปรไฟล์",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    _selectedImagePath = result.FullPath;
                    imgAvatar.Source = ImageSource.FromFile(_selectedImagePath);
                    imgAvatarEdit.Source = ImageSource.FromFile(_selectedImagePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ข้อผิดพลาด", $"ไม่สามารถเลือกรูปได้: {ex.Message}", "ตกลง");
            }
        }

        // กดปุ่มแก้ไขโปรไฟล์ สลับไปโหมดแก้ไข
        private void OnEditProfileClicked(object sender, EventArgs e)
        {
            if (_viewModel.IsAdmin) return;
            ViewLayout.IsVisible = false;
            EditLayout.IsVisible = true;
        }

        // กดยกเลิกการแก้ไข กลับโหมดดูข้อมูล
        private void OnCancelEditClicked(object sender, EventArgs e)
        {
            EditLayout.IsVisible = false;
            ViewLayout.IsVisible = true;
        }

        // บันทึกการเปลี่ยนแปลงโปรไฟล์ลง Firebase
        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            if (_viewModel.CurrentUser == null) return;

            // อัปเดตข้อมูลใน ViewModel
            _viewModel.CurrentUser.FullName = entName.Text;
            _viewModel.CurrentUser.Phone = entPhone.Text;

            if (!string.IsNullOrWhiteSpace(entPassword.Text))
                _viewModel.CurrentUser.Password = entPassword.Text;

            if (!string.IsNullOrEmpty(_selectedImagePath))
                _viewModel.CurrentUser.ProfileImage = _selectedImagePath;

            // สั่งบันทึกผ่าน ViewModel
            var firebaseService = new FirebaseService();
            bool success = await firebaseService.UpdateUserProfile(_viewModel.CurrentUser);

            if (success)
            {
                await DisplayAlert("สำเร็จ", "อัปเดตข้อมูลเรียบร้อย", "ตกลง");
                await _viewModel.LoadProfileAsync();
                UpdateUI();
                EditLayout.IsVisible = false;
                ViewLayout.IsVisible = true;
            }
            else
            {
                await DisplayAlert("ผิดพลาด", "ไม่สามารถอัปเดตข้อมูลได้", "ตกลง");
            }
        }

        // กดปุ่ม Logout ออกจากระบบ
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("ยืนยัน", "ออกจากระบบใช่หรือไม่?", "ใช่", "ไม่");
            if (confirm)
            {
                // Logout ผ่าน ViewModel
                await _viewModel.LogoutAsync();
                (Shell.Current as AppShell)?.NavigateToLogin();
            }
        }
    }
}
