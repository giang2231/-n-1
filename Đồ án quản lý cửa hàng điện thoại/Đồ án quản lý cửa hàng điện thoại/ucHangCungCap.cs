using System;
using System.Data;
using Microsoft.Data.SqlClient; 
using System.Windows.Forms;
using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers; 
using static Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers.DatabaseHelper; 
namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class ucHangCungCap : UserControl
    {
        public ucHangCungCap()
        {
            InitializeComponent();
   
            this.dgvHangCungCap.CellClick += dgvHangCungCap_CellClick;
        }

        private void ucHangCungCap_Load(object sender, EventArgs e)
        {
            LoadData();
            ResetForm();
        }

    
        private void LoadData()
        {
            try
            {
                string sql = "SELECT MaHang AS [Mã Hãng], TenHang AS [Tên Hãng], QuocGia AS [Quốc Gia], " +
                             "DiaChi AS [Địa Chỉ], SoDienThoai AS [Số Điện Thoại], Email AS [Email], " +
                             "CASE WHEN TrangThai = 1 THEN N'Đang hợp tác' ELSE N'Ngừng hợp tác' END AS [Trạng Thái] " +
                             "FROM HangCungCap";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql);

                if (dt != null)
                {
                    dgvHangCungCap.DataSource = dt;
                    dgvHangCungCap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị danh sách: " + ex.Message, "Lỗi Giao Diện", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetForm()
        {
            txtMaHang.Text = DatabaseHelper.GenerateCode("NCC", "HangCungCap", "MaHang");
            txtTenHang.Text = "";
            txtQuocGia.Text = "";
            txtDiaChi.Text = "";
            txtSDT.Text = "";
            txtEmail.Text = "";
            if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0;

            txtMaHang.Enabled = true;
            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private void dgvHangCungCap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvHangCungCap.Rows[e.RowIndex];

                txtMaHang.Text = row.Cells["Mã Hãng"].Value?.ToString();
                txtTenHang.Text = row.Cells["Tên Hãng"].Value?.ToString();
                txtQuocGia.Text = row.Cells["Quốc Gia"].Value?.ToString();
                txtDiaChi.Text = row.Cells["Địa Chỉ"].Value?.ToString();
                txtSDT.Text = row.Cells["Số Điện Thoại"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();

                string trangThai = row.Cells["Trạng Thái"].Value?.ToString();
                cboTrangThai.SelectedItem = trangThai;

                txtMaHang.Enabled = false; 
                btnThem.Enabled = false;
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
            }
        }

        
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenHang.Text)) 
            {
                MessageBox.Show(" Tên Hãng không được để trống!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHang", txtMaHang.Text.Trim()),
                new SqlParameter("@TenHang", txtTenHang.Text.Trim()),
                new SqlParameter("@QuocGia", txtQuocGia.Text.Trim()),
                new SqlParameter("@DiaChi", txtDiaChi.Text.Trim()),
                new SqlParameter("@SoDienThoai", txtSDT.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@TrangThai", cboTrangThai.SelectedIndex == 0 ? 1 : 0)
            };

            int result = DatabaseHelper.ExecuteStoredProcedure("sp_InsertHangCungCap", parameters);

            if (result != -99)
            {
                MessageBox.Show("Thêm mới hãng cung cấp thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ResetForm();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenHang.Text))
            {
                MessageBox.Show("Tên hãng sản xuất không được bỏ trống!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHang", txtMaHang.Text.Trim()),
                new SqlParameter("@TenHang", txtTenHang.Text.Trim()),
                new SqlParameter("@QuocGia", txtQuocGia.Text.Trim()),
                new SqlParameter("@DiaChi", txtDiaChi.Text.Trim()),
                new SqlParameter("@SoDienThoai", txtSDT.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@TrangThai", cboTrangThai.SelectedIndex == 0 ? 1 : 0)
            };

            int result = DatabaseHelper.ExecuteStoredProcedure("sp_UpdateHangCungCap", parameters);

            if (result != -99)
            {
                MessageBox.Show("Cập nhật thông tin thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ResetForm();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maHang = txtMaHang.Text.Trim();
            DialogResult confirm = MessageBox.Show($"Bạn có thực sự muốn ngừng hợp tác và thực hiện xóa mềm hãng [{maHang}] không?",
                                                    "Xác nhận hệ thống", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaHang", maHang)
                };

                int result = DatabaseHelper.ExecuteStoredProcedure("sp_DeleteHangCungCap", parameters);

                if (result != -99)
                {
                    MessageBox.Show("Đã xóa và cập nhật trạng thái ngừng hoạt động!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ResetForm();
                }
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string key = txtTimKiem.Text.Trim();
                string sql = "SELECT MaHang AS [Mã Hãng], TenHang AS [Tên Hãng], QuocGia AS [Quốc Gia], " +
                             "DiaChi AS [Địa Chỉ], SoDienThoai AS [Số Điện Thoại], Email AS [Email], " +
                             "CASE WHEN TrangThai = 1 THEN N'Đang hợp tác' ELSE N'Ngừng hợp tác' END AS [Trạng Thái] " +
                             "FROM HangCungCap " +
                             "WHERE MaHang LIKE @key OR TenHang LIKE @key";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@key", "%" + key + "%")
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);
                if (dt != null) dgvHangCungCap.DataSource = dt;
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