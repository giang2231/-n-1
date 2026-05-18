using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers; // Nhớ đổi đúng namespace của bác

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class ucNhanVien : UserControl
    {
        public ucNhanVien()
        {
            InitializeComponent();

            // Tự động gán sự kiện click vào lưới để đổ dữ liệu lên Form
          
        }

        private void UcNhanVien_Load(object sender, EventArgs e)
        {
            // Thiết lập giá trị mặc định cho ComboBox tránh bị lỗi Null
            if (cboGioiTinh.Items.Count > 0) cboGioiTinh.SelectedIndex = 0;
            if (cboChucVu.Items.Count > 0) cboChucVu.SelectedIndex = 0;
            if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0;

            LoadData();
        }

        // =========================================================================
        // 1. HÀM TẢI DỮ LIỆU LÊN LƯỚI (Đã giấu cột Mật Khẩu đi cho bảo mật)
        // =========================================================================
        private void LoadData()
        {
            try
            {
                string sql = @"
            SELECT MaNV AS [Mã NV], HoTen AS [Họ Tên], GioiTinh AS [Giới Tính], 
                   NgaySinh AS [Ngày Sinh], CMND AS [CMND/CCCD], SoDienThoai AS [SĐT], 
                   Email, DiaChi AS [Địa Chỉ], ChucVu AS [Chức Vụ], 
                   LuongCoBan AS [Lương CB], TenDangNhap AS [Tài Khoản], 
                   MatKhau, 
                   -- SỬA CHỖ NÀY: Dịch bit 1/0 thành chữ cho đẹp giao diện
                   CASE WHEN TrangThai = 1 THEN N'Đang làm' ELSE N'Nghỉ việc' END AS [Trạng Thái]
            FROM NhanVien";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvNhanVien.DataSource = dt;

                if (dgvNhanVien.Columns.Count > 0)
                {
                    // Ẩn cột mật khẩu không cho nhân viên khác nhìn thấy
                    if (dgvNhanVien.Columns["MatKhau"] != null)
                        dgvNhanVien.Columns["MatKhau"].Visible = false;

                    // Định dạng cột tiền lương
                    if (dgvNhanVien.Columns["Lương CB"] != null)
                    {
                        dgvNhanVien.Columns["Lương CB"].DefaultCellStyle.Format = "N0";
                        dgvNhanVien.Columns["Lương CB"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================================
        // 2. SỰ KIỆN CLICK VÀO LƯỚI -> ĐỔ DỮ LIỆU LÊN FORM
        // =========================================================================
        private void DgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvNhanVien.CurrentRow != null && !dgvNhanVien.CurrentRow.IsNewRow)
            {
                DataGridViewRow row = dgvNhanVien.CurrentRow;

                txtMaNV.Text = row.Cells["Mã NV"].Value?.ToString();
                txtMaNV.Enabled = false; // Đã click vào dòng thì KHÔNG ĐƯỢC sửa Mã NV

                txtHoTen.Text = row.Cells["Họ Tên"].Value?.ToString();
                cboGioiTinh.Text = row.Cells["Giới Tính"].Value?.ToString();

                if (DateTime.TryParse(row.Cells["Ngày Sinh"].Value?.ToString(), out DateTime date))
                    dtpNgaySinh.Value = date;

                txtCMND.Text = row.Cells["CMND/CCCD"].Value?.ToString();
                txtSoDienThoai.Text = row.Cells["SĐT"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtDiaChi.Text = row.Cells["Địa Chỉ"].Value?.ToString();
                cboChucVu.Text = row.Cells["Chức Vụ"].Value?.ToString();

                // Ép số lương về dạng không có dấu phẩy để hiển thị lên TextBox
                decimal luong = row.Cells["Lương CB"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["Lương CB"].Value) : 0;
                txtLuong.Text = luong.ToString("0");

                txtTenDangNhap.Text = row.Cells["Tài Khoản"].Value?.ToString();
                cboTrangThai.Text = row.Cells["Trạng Thái"].Value?.ToString();

                // Lúc click vào sửa, Mật khẩu sẽ bị làm trống. Nếu muốn đổi MK thì dùng nút Đổi Mật Khẩu riêng
                txtMatKhau.Clear();
                txtMatKhau.PlaceholderText = "Đã mã hóa (Dùng nút Đổi MK để sửa)";
                txtMatKhau.Enabled = false;
            }
        }

        // =========================================================================
        // 3. THÊM NHÂN VIÊN MỚI
        // =========================================================================
        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtTenDangNhap.Text) || string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Mã NV, Họ tên, Tên đăng nhập và Mật khẩu là các trường bắt buộc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Xử lý Lương: Nếu gõ tào lao thì mặc định là 0
                decimal.TryParse(txtLuong.Text.Trim().Replace(",", ""), out decimal luongCoBan);

                SqlParameter[] p = new SqlParameter[]
                {
                    new SqlParameter("@MaNV", txtMaNV.Text.Trim()),
                    new SqlParameter("@HoTen", txtHoTen.Text.Trim()),
                    new SqlParameter("@GioiTinh", cboGioiTinh.Text),
                    new SqlParameter("@NgaySinh", dtpNgaySinh.Value.Date),
                    new SqlParameter("@CMND", txtCMND.Text.Trim()),
                    new SqlParameter("@SoDienThoai", txtSoDienThoai.Text.Trim()),
                    new SqlParameter("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : txtEmail.Text.Trim()),
                    new SqlParameter("@DiaChi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@ChucVu", cboChucVu.Text),
                    new SqlParameter("@LuongCoBan", luongCoBan),
                    new SqlParameter("@TenDangNhap", txtTenDangNhap.Text.Trim()),
                    new SqlParameter("@MatKhau", txtMatKhau.Text.Trim()), // Lưu mật khẩu mới
                    new SqlParameter("@TrangThai", cboTrangThai.Text == "Đang làm" ? 1 : 0)
                };

                DatabaseHelper.ExecuteStoredProcedure("sp_InsertNhanVien", p);
                MessageBox.Show("Thêm nhân viên mới thành công!", "Tuyệt vời", MessageBoxButtons.OK, MessageBoxIcon.Information);

                BtnClear_Click(null, null); // Xóa trắng form
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================================
        // 4. SỬA THÔNG TIN NHÂN VIÊN (Không sửa mật khẩu)
        // =========================================================================
        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                decimal.TryParse(txtLuong.Text.Trim().Replace(",", ""), out decimal luongCoBan);

                SqlParameter[] p = new SqlParameter[]
                {
                    new SqlParameter("@MaNV", txtMaNV.Text.Trim()),
                    new SqlParameter("@HoTen", txtHoTen.Text.Trim()),
                    new SqlParameter("@GioiTinh", cboGioiTinh.Text),
                    new SqlParameter("@NgaySinh", dtpNgaySinh.Value.Date),
                    new SqlParameter("@CMND", txtCMND.Text.Trim()),
                    new SqlParameter("@SoDienThoai", txtSoDienThoai.Text.Trim()),
                    new SqlParameter("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : txtEmail.Text.Trim()),
                    new SqlParameter("@DiaChi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@ChucVu", cboChucVu.Text),
                    new SqlParameter("@LuongCoBan", luongCoBan),
                    new SqlParameter("@TenDangNhap", txtTenDangNhap.Text.Trim()),
                    new SqlParameter("@TrangThai", cboTrangThai.Text)
                    // Ý ĐỒ CỦA PROCEDURE LÀ KHÔNG TRUYỀN MATKHAU VÀO ĐÂY
                };

                DatabaseHelper.ExecuteStoredProcedure("sp_UpdateNhanVien", p);
                MessageBox.Show("Cập nhật thông tin thành công!", "Tuyệt vời", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================================
        // 5. XÓA NHÂN VIÊN
        // =========================================================================
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa nhân viên {txtHoTen.Text}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SqlParameter[] p = new SqlParameter[] { new SqlParameter("@MaNV", txtMaNV.Text.Trim()) };
                    DatabaseHelper.ExecuteStoredProcedure("sp_DeleteNhanVien", p);

                    MessageBox.Show("Xóa nhân viên thành công!", "Tuyệt vời", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BtnClear_Click(null, null);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================================
        // 6. ĐỔI MẬT KHẨU NHANH BẰNG CÂU LỆNH SQL THUẦN
        // =========================================================================
        private void BtnDoiMatKhau_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần đổi mật khẩu!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mở khóa ô mật khẩu để nhập mật khẩu mới
            if (!txtMatKhau.Enabled)
            {
                txtMatKhau.Enabled = true;
                txtMatKhau.PlaceholderText = "Nhập mật khẩu mới vào đây...";
                txtMatKhau.Focus();
                MessageBox.Show("Vui lòng nhập mật khẩu mới vào ô Mật khẩu, sau đó bấm nút [Đổi mật khẩu] lần nữa để lưu!", "Hướng dẫn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Mật khẩu mới không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string sql = "UPDATE NhanVien SET MatKhau = @MatKhau WHERE MaNV = @MaNV";
                SqlParameter[] p = new SqlParameter[]
                {
                    new SqlParameter("@MatKhau", txtMatKhau.Text.Trim()),
                    new SqlParameter("@MaNV", txtMaNV.Text.Trim())
                };

                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Đổi mật khẩu thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtMatKhau.Clear();
                txtMatKhau.Enabled = false;
                txtMatKhau.PlaceholderText = "Đã mã hóa (Dùng nút Đổi MK để sửa)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đổi mật khẩu: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================================
        // 7. LÀM SẠCH FORM ĐỂ CHUẨN BỊ THÊM MỚI
        // =========================================================================
        private void BtnClear_Click(object sender, EventArgs e)
        {
            // 1. TỰ ĐỘNG SINH MÃ NHÂN VIÊN MỚI TINH TỪ DATABASE
            txtMaNV.Text = DatabaseHelper.GenerateCode("NV", "NhanVien", "MaNV");

            // 2. KHÓA Ô MÃ NV LẠI (Chống nhân viên ngứa tay gõ bậy)
            txtMaNV.Enabled = false;

            // Dọn dẹp các ô nhập liệu còn lại
            txtHoTen.Clear();
            txtCMND.Clear();
            txtSoDienThoai.Clear();
            txtEmail.Clear();
            txtDiaChi.Clear();
            txtLuong.Clear();
            txtTenDangNhap.Clear();

            txtMatKhau.Enabled = true;
            txtMatKhau.Clear();
            txtMatKhau.PlaceholderText = "Mật khẩu *";

            if (cboGioiTinh.Items.Count > 0) cboGioiTinh.SelectedIndex = 0;
            if (cboChucVu.Items.Count > 0) cboChucVu.SelectedIndex = 0;
            if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0;

            dtpNgaySinh.Value = DateTime.Now.AddYears(-18); // Gợi ý sinh năm đủ 18 tuổi

            // Đẩy con trỏ chuột sang ô Họ Tên để người dùng gõ luôn cho tiện
            txtHoTen.Focus();
        }

        // =========================================================================
        // 8. TÌM KIẾM THEO THỜI GIAN THỰC (Gõ tới đâu lọc tới đó)
        // =========================================================================
        private void TxtTimKiem_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtTimKiem.Text.Trim();
                // Nếu dgvNhanVien có bind dữ liệu qua DataTable thì dùng DataView để lọc
                if (dgvNhanVien.DataSource is DataTable dt)
                {
                    // Lọc theo Mã, Tên, hoặc SĐT
                    dt.DefaultView.RowFilter = string.Format("[Mã NV] LIKE '%{0}%' OR [Họ Tên] LIKE '%{0}%' OR [SĐT] LIKE '%{0}%'", keyword);
                }
            }
            catch { }
        }

        // =========================================================================
        // 9. NÚT TẢI LẠI LƯỚI
        // =========================================================================
        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            LoadData();
        }
    }
}