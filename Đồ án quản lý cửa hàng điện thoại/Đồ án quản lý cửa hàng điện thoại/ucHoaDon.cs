using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers;

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class ucHoaDon : UserControl
    {
        public ucHoaDon()
        {
            InitializeComponent();
        }

        
        private void UcHoaDon_Load(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now;
            dtpTuNgay.Value = new DateTime(today.Year, today.Month, 1);
            dtpDenNgay.Value = today;

            btnHuyHD.Enabled = false; 
            LoadDanhSachHoaDon();
            LoadChiTietHoaDon("");
        }

        private void LoadDanhSachHoaDon()
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@TuNgay", dtpTuNgay.Value.Date),
                    new SqlParameter("@DenNgay", dtpDenNgay.Value.Date),
                    new SqlParameter("@TuKhoa", txtTimKiem.Text.Trim())
                };

                DataTable dt = DatabaseHelper.ExecuteQuery("EXEC sp_LayDanhSachHoaDon @TuNgay, @DenNgay, @TuKhoa", parameters);

                if (dt != null)
                {
                    dgvHoaDon.DataSource = dt;

                    if (dgvHoaDon.Columns.Contains("TongTien")) dgvHoaDon.Columns["TongTien"].DefaultCellStyle.Format = "N0";
                    if (dgvHoaDon.Columns.Contains("GiamGia")) dgvHoaDon.Columns["GiamGia"].DefaultCellStyle.Format = "N0";
                    if (dgvHoaDon.Columns.Contains("ThanhTien")) dgvHoaDon.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";

                    dgvHoaDon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }

                ClearChiTiet(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách hóa đơn: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadChiTietHoaDon(string maHD)
        {
            try
            {
                string sql = @"
            SELECT SoIMEI AS [Số IMEI], 
                   MaChiTietSP AS [Mã Sản Phẩm], 
                   SoLuong AS [Số Lượng], 
                   DonGia AS [Giá Bán] 
            FROM ChiTietHoaDon 
            WHERE MaHD = @MaHD";

                SqlParameter[] param = new SqlParameter[] { new SqlParameter("@MaHD", maHD) };

                DataTable dtChiTiet = DatabaseHelper.ExecuteQuery(sql, param);

                if (dtChiTiet != null)
                {
                    dgvChiTiet.DataSource = dtChiTiet;

                    if (dgvChiTiet.Columns.Contains("Giá Bán"))
                    {
                        dgvChiTiet.Columns["Giá Bán"].DefaultCellStyle.Format = "N0";
                    }
                    dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết máy: " + ex.Message, "Lỗi truy vấn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private void BtnLoc_Click(object sender, EventArgs e)
        {
            LoadDanhSachHoaDon();
        }

        private void TxtTimKiem_TextChanged(object sender, EventArgs e)
        {
            LoadDanhSachHoaDon();
        }

     
        private void DgvHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvHoaDon.Rows[e.RowIndex];

                string maHD = row.Cells["Mã HĐ"].Value?.ToString();
                string trangThai = row.Cells["Trạng Thái"].Value?.ToString();

                txtMaHD.Text = maHD;
                txtNgayLap.Text = row.Cells["Ngày Lập"].Value?.ToString();
                txtTenKH.Text = row.Cells["Khách Hàng"].Value?.ToString();
                txtSoDienThoaiKH.Text = row.Cells["SĐT"].Value?.ToString();
                txtNhanVien.Text = row.Cells["Thu Ngân"].Value?.ToString();
                txtGhiChu.Text = row.Cells["GhiChu"].Value?.ToString();

                decimal tongTien = row.Cells["TongTien"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["TongTien"].Value) : 0;
                decimal giamGia = row.Cells["GiamGia"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["GiamGia"].Value) : 0;
                decimal thanhTien = row.Cells["ThanhTien"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["ThanhTien"].Value) : 0;

                lblTongTien.Text = $"Cộng tiền hàng: {tongTien:N0} VND";
                txtGiamGia.Text = giamGia.ToString("N0");
                lblThanhTien.Text = $"ĐÃ THU: {thanhTien:N0} VND";

                LoadChiTietHoaDon(maHD);

                if (trangThai == "Đã Hủy")
                {
                    btnHuyHD.Enabled = false;
                    btnHuyHD.FillColor = System.Drawing.Color.Gray; // Đổi màu xám báo hiệu vô hiệu hóa
                    btnHuyHD.Text = "Hóa Đơn Này Đã Bị Hủy";
                }
                else
                {
                    btnHuyHD.Enabled = true;
                    btnHuyHD.FillColor = System.Drawing.Color.FromArgb(231, 76, 60); // Đỏ
                    btnHuyHD.Text = "Hủy Hóa Đơn && Trả Kho";
                }
            }
        }

        private void BtnHuyHD_Click(object sender, EventArgs e)
        {
            string maHD = txtMaHD.Text.Trim();
            if (string.IsNullOrEmpty(maHD))
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn từ danh sách để hủy!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn HỦY hóa đơn [{maHD}] không?\n\nLưu ý: Hành động này sẽ tự động hoàn trả toàn bộ máy IMEI trong hóa đơn này về Kho!",
                "Cảnh báo bảo mật", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    SqlParameter[] param = new SqlParameter[] { new SqlParameter("@MaHD", maHD) };

                    int result = DatabaseHelper.ExecuteStoredProcedure("sp_HuyHoaDon", param);

                    if (result != -99)
                    {
                        MessageBox.Show("Đã hủy hóa đơn và hoàn trả điện thoại về kho thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDanhSachHoaDon(); 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể hủy hóa đơn:\n" + ex.Message, "Lỗi Transaction", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

       
        private void ClearChiTiet()
        {
            txtMaHD.Text = "";
            txtNgayLap.Text = "";
            txtTenKH.Text = "";
            txtSoDienThoaiKH.Text = "";
            txtNhanVien.Text = "";
            txtGhiChu.Text = "";
            txtGiamGia.Text = "";
            lblTongTien.Text = "Cộng tiền hàng: 0 VND";
            lblThanhTien.Text = "ĐÃ THU: 0 VND";

            dgvChiTiet.DataSource = null;
            btnHuyHD.Enabled = false;
            btnHuyHD.FillColor = System.Drawing.Color.FromArgb(231, 76, 60);
            btnHuyHD.Text = "Hủy Hóa Đơn Trả Kho";
        }

        private void splitContainerMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            
        }
    }
}