using System;
using System.Data;
using Microsoft.Data.SqlClient; // Thư viện đóng gói tham số an toàn chống injection
using System.Windows.Forms;
using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers; // Thư mục chứa DatabaseHelper của bạn

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class ucKhachHang : UserControl
    {
        public ucKhachHang()
        {
            InitializeComponent();
            // Kích hoạt sự kiện lắng nghe khi người dùng click vào lưới
            this.dgvKhachHang.CellClick += dgvKhachHang_CellClick;
        }

        private void ucKhachHang_Load(object sender, EventArgs e)
        {
            LoadData();
            ResetForm();
        }

        /// <summary>
        /// 1. ĐỔ DỮ LIỆU LÊN LƯỚI DATA-GRID-VIEW
        /// </summary>
        private void LoadData()
        {
            try
            {
               
                string sql = "SELECT MaKH AS [Mã KH], HoTen AS [Họ Tên], GioiTinh AS [Giới Tính], " +
                             "NgaySinh AS [Ngày Sinh], SoDienThoai AS [Số Điện Thoại], DiaChi AS [Địa Chỉ], " +
                             "LoaiKhach AS [Phân Hạng], dbo.fn_TinhTongChiTieuKhachHang(MaKH) AS [Tổng Chi Tiêu], NgayTao AS [Ngày Tham Gia] " +
                             "FROM KhachHang";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql);

                if (dt != null)
                {
                    dgvKhachHang.DataSource = dt;
                    // Định dạng dấu phẩy phân tách hàng nghìn cho cột chi tiêu trên lưới
                    if (dgvKhachHang.Columns.Contains("Tổng Chi Tiêu"))
                    {
                        dgvKhachHang.Columns["Tổng Chi Tiêu"].DefaultCellStyle.Format = "N0";
                    }
                    dgvKhachHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị danh sách: " + ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 2. LÀM SẠCH FORM NHẬP LIỆU & TỰ ĐỘNG SINH MÃ KHÁCH HÀNG MỚI
        /// </summary>
        private void ResetForm()
        {
            txtHoTen.Text = "";
            txtSoDienThoai.Text = "";
            txtDiaChi.Text = "";

            if (cboGioiTinh.Items.Count > 0) cboGioiTinh.SelectedIndex = 0;
            if (cboLoaiKhach.Items.Count > 0) cboLoaiKhach.SelectedIndex = 0;
            dtpNgaySinh.Value = new DateTime(2000, 1, 1);

            // ĐƯA LABEL VỀ MẶC ĐỊNH BẰNG 0 KHI THÊM MỚI
            lblTongChiTieu.Text = "Tổng chi tiêu: 0 VND";

            // Gọi hàm sinh mã tự động từ DatabaseHelper tĩnh của bạn
            try
            {
                txtMaKH.Text = DatabaseHelper.GenerateCode("KH", "KhachHang", "MaKH");
            }
            catch
            {
                txtMaKH.Text = "KH001";
            }

            txtMaKH.Enabled = false; // Khóa mờ ô mã khách hàng
            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        /// <summary>
        /// 3. ĐỒNG BỘ DỮ LIỆU: ĐỔ DỮ LIỆU TỪ LƯỚI LÊN FORM KHI CELL CLICK
        /// </summary>
        private void dgvKhachHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvKhachHang.Rows[e.RowIndex];

                txtMaKH.Text = row.Cells["Mã KH"].Value?.ToString();
                txtHoTen.Text = row.Cells["Họ Tên"].Value?.ToString();
                txtSoDienThoai.Text = row.Cells["Số Điện Thoại"].Value?.ToString();
                txtDiaChi.Text = row.Cells["Địa Chỉ"].Value?.ToString();
                cboGioiTinh.SelectedItem = row.Cells["Giới Tính"].Value?.ToString();
                cboLoaiKhach.SelectedItem = row.Cells["Phân Hạng"].Value?.ToString();

                if (DateTime.TryParse(row.Cells["Ngày Sinh"].Value?.ToString(), out DateTime ngaySinh))
                {
                    dtpNgaySinh.Value = ngaySinh;
                }

                // ĐỔ SỐ TIỀN VÀO LABEL VÀ ĐỊNH DẠNG ĐẸP MẮT (Ví dụ hiển thị: Tổng chi tiêu: 24,500,000 VND)
                if (decimal.TryParse(row.Cells["Tổng Chi Tiêu"].Value?.ToString(), out decimal chiTieu))
                {
                    lblTongChiTieu.Text = $"Tổng chi tiêu: {chiTieu:N0} VND";
                }
                else
                {
                    lblTongChiTieu.Text = "Tổng chi tiêu: 0 VND";
                }

                txtMaKH.Enabled = false;
                btnThem.Enabled = false; // Tắt nút thêm, bật nút sửa và xóa
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
            }
        }

        /// <summary>
        /// 4. CHỨC NĂNG THÊM MỚI KHÁCH HÀNG (Mặc định chi tiêu = 0)
        /// </summary>
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
            {
                MessageBox.Show("Họ tên và Số điện thoại là trường bắt buộc!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Đóng gói mảng tham số chuẩn 9 trường gửi xuống SQL
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaKH", txtMaKH.Text.Trim()),
                    new SqlParameter("@HoTen", txtHoTen.Text.Trim()),
                    new SqlParameter("@GioiTinh", cboGioiTinh.SelectedItem?.ToString() ?? "Nam"),
                    new SqlParameter("@NgaySinh", dtpNgaySinh.Value.Date),
                    new SqlParameter("@SoDienThoai", txtSoDienThoai.Text.Trim()),
                    new SqlParameter("@DiaChi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@LoaiKhach", cboLoaiKhach.SelectedItem?.ToString() ?? "Thường"),
                    new SqlParameter("@TongChiTieu", 0m),
                    new SqlParameter("@NgayTao", DateTime.Now)
                };

                int result = DatabaseHelper.ExecuteStoredProcedure("sp_InsertKhachHang", parameters);

                if (result != -99)
                {
                    MessageBox.Show("Đăng ký thông tin khách hàng mới thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thực thi: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
            {
                MessageBox.Show("Họ tên và Số điện thoại không được để trống!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                decimal chiTieuHienTai = 0;
                string cleanText = lblTongChiTieu.Text
                                    .Replace("Tổng chi tiêu:", "")
                                    .Replace("VND", "")
                                    .Replace(",", "")
                                    .Replace(".", "")
                                    .Replace(" ", "")
                                    .Trim();
                decimal.TryParse(cleanText, out chiTieuHienTai);

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaKH", txtMaKH.Text.Trim()),
                    new SqlParameter("@HoTen", txtHoTen.Text.Trim()),
                    new SqlParameter("@GioiTinh", cboGioiTinh.SelectedItem?.ToString() ?? "Nam"),
                    new SqlParameter("@NgaySinh", dtpNgaySinh.Value.Date),
                    new SqlParameter("@SoDienThoai", txtSoDienThoai.Text.Trim()),
                    new SqlParameter("@DiaChi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@LoaiKhach", cboLoaiKhach.SelectedItem?.ToString() ?? "Thường"),
                    new SqlParameter("@TongChiTieu", chiTieuHienTai) 
                };

                int result = DatabaseHelper.ExecuteStoredProcedure("sp_UpdateKhachHang", parameters);

                if (result != -99)
                {
                    MessageBox.Show("Cập nhật thông tin khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể cập nhật thông tin: " + ex.Message, "Lỗi thực thi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 6. CHỨC NĂNG XÓA HỒ SƠ KHÁCH HÀNG
        /// </summary>
        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maKH = txtMaKH.Text.Trim();
            DialogResult confirm = MessageBox.Show($"Bạn có thực sự muốn xóa vĩnh viễn hồ sơ khách hàng [{maKH}] không?",
                                                    "Xác nhận hệ thống", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@MaKH", maKH) };
                    int result = DatabaseHelper.ExecuteStoredProcedure("sp_DeleteKhachHang", parameters);

                    if (result != -99)
                    {
                        MessageBox.Show("Xóa dữ liệu khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        ResetForm();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message, "Hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 7. TÌM KIẾM THỜI GIAN THỰC (REAL-TIME FILTER)
        /// </summary>
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string key = txtTimKiem.Text.Trim();
                string sql = "SELECT MaKH AS [Mã KH], HoTen AS [Họ Tên], GioiTinh AS [Giới Tính], " +
                             "NgaySinh AS [Ngày Sinh], SoDienThoai AS [Số Điện Thoại], DiaChi AS [Địa Chỉ], " +
                             "LoaiKhach AS [Phân Hạng], TongChiTieu AS [Tổng Chi Tiêu], NgayTao AS [Ngày Tham Gia] " +
                             "FROM KhachHang " +
                             "WHERE MaKH LIKE @key OR HoTen LIKE @key OR SoDienThoai LIKE @key";

                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@key", "%" + key + "%") };

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);
                if (dt != null)
                {
                    dgvKhachHang.DataSource = dt;
                    if (dgvKhachHang.Columns.Contains("Tổng Chi Tiêu"))
                    {
                        dgvKhachHang.Columns["Tổng Chi Tiêu"].DefaultCellStyle.Format = "N0";
                    }
                }
            }
            catch { }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = "";
            LoadData();
            ResetForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetForm();
        }
    }
}