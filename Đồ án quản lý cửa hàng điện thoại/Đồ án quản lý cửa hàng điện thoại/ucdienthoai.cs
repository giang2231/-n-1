using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers;
using Guna.UI2.WinForms;

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class ucDienThoai : UserControl
    {
        public ucDienThoai()
        {
            InitializeComponent();
            PhanQuyenUI();
            LoadDanhMucCoDinh();
            LoadHangCungCap();
            LoadData_DongMay();
            LoadData_PhienBan("");
            LoadMauSacVaDungLuong();
        }

        private void LoadMauSacVaDungLuong()
        {
            try
            {
                cboMauSac.Items.Clear();
                cboMauSac.Items.AddRange(new string[] {
            "Đen", "Trắng", "Xám", "Bạc", "Vàng", "Đỏ",
            "Xanh dương", "Xanh lá", "Hồng", "Tím", "Titan"
        });
                if (cboMauSac.Items.Count > 0) cboMauSac.SelectedIndex = 0;

                cboDungLuong.Items.Clear();
                cboDungLuong.Items.AddRange(new string[] {
            "16GB", "32GB", "64GB", "128GB", "256GB", "512GB", "1TB", "2TB", "Không có"
        });
                if (cboDungLuong.Items.Count > 0) cboDungLuong.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp màu sắc và dung lượng: " + ex.Message);
            }
        }

        private void LoadDanhMucCoDinh()
        {
            try
            {
                cboDanhMucSP.Items.Clear();
                cboDanhMucSP.Items.AddRange(new object[] { "Smartphone", "Tablet", "Phụ kiện" });
                if (cboDanhMucSP.Items.Count > 0) cboDanhMucSP.SelectedIndex = 0;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi nạp danh mục: " + ex.Message); }
        }

        private void PhanQuyenUI()
        {
            try
            {
                string role = Session.ChucVu != null ? Session.ChucVu.Trim().ToUpper() : "";

                if (role != "ADMIN" && role != "KẾ TOÁN")
                {
                    txtGiaNhap.Visible = false;
                    btnThem.Visible = false;
                    btnSua.Visible = false;
                    btnXoa.Visible = false;
                    btnXuatExcel.Visible = false;

                    pnlForm.Enabled = false;
                }
                else
                {
                    txtGiaNhap.Visible = true;
                    btnThem.Visible = true;
                    btnSua.Visible = true;
                    btnXoa.Visible = true;
                    btnXuatExcel.Visible = true;
                    pnlForm.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi phân quyền UI: " + ex.Message);
            }
        }

        private void LoadData_DongMay(string keyword = "")
        {
            try
            {
                
                string sql = @"
            SELECT d.MaMay, d.TenMay, h.TenHang, d.DanhMuc, d.CPU, d.RAM, d.Camera, d.Pin, d.HeDieuHanh,
                   CASE WHEN d.TrangThai = 1 THEN N'Đang bán' ELSE N'Ngừng bán' END AS TrangThai
            FROM DienThoai d
            LEFT JOIN HangCungCap h ON d.MaHang = h.MaHang
            WHERE (@kw = '' OR d.TenMay LIKE '%' + @kw + '%' OR d.MaMay LIKE '%' + @kw + '%')
            ORDER BY d.TenMay";

                dgvDienThoai.DataSource = DatabaseHelper.ExecuteQuery(sql, new[] { new SqlParameter("@kw", keyword) });
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách dòng máy: " + ex.Message); }
        }

        private void LoadData_PhienBan(string maMay)
        {
            try
            {
                string sql = @"
            SELECT 
                ct.MaChiTiet AS [Mã Phiên Bản], 
                ct.DungLuong AS [Dung Lượng], 
                ct.MauSac AS [Màu Sắc], 
                FORMAT(ct.GiaNhap, '#,##0') AS [Giá Vốn], 
                FORMAT(ct.GiaBan, '#,##0') AS [Giá Bán],
                ISNULL(Kho.TonKho, 0) AS [Tồn Kho]
            FROM ChiTietDienThoai ct
            LEFT JOIN (
                SELECT MaChiTietSP, COUNT(SoIMEI) AS TonKho
                FROM ChiTietPhieuNhap
                WHERE TrangThaiIMEI = N'Trong kho'
                GROUP BY MaChiTietSP
            ) Kho ON ct.MaChiTiet = Kho.MaChiTietSP
            WHERE ct.MaMay = @MaMay AND ct.TrangThai = 1";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, new[] { new SqlParameter("@MaMay", maMay) });

                if (dt != null)
                {
                    dgvChiTietDienThoai.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách phiên bản: " + ex.Message); }
        }


        private void LoadHangCungCap()
        {
            try
            {
                string sql = "SELECT MaHang, TenHang FROM HangCungCap WHERE TrangThai = 1";
                var dt = DatabaseHelper.ExecuteQuery(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    cboHang.DataSource = dt;
                    cboHang.DisplayMember = "TenHang";
                    cboHang.ValueMember = "MaHang";
                }
                if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi lấy danh sách Hãng: " + ex.Message); }
        }
       
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTenMay.Text))
                {
                    MessageBox.Show("Vui lòng gõ Tên máy điện thoại!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string maMayTuDong = DatabaseHelper.GenerateCode("MM", "DienThoai", "MaMay");
                var maHang = cboHang.SelectedValue?.ToString();

                int result = DatabaseHelper.ExecuteStoredProcedure("sp_ThemDienThoai", new[] {
                    new SqlParameter("@MaMay", maMayTuDong),
                    new SqlParameter("@TenMay", txtTenMay.Text.Trim()),
                    new SqlParameter("@MaHang", maHang ?? (object)DBNull.Value),
                    new SqlParameter("@CPU", txtCPU.Text.Trim()),
                    new SqlParameter("@RAM", txtRAM.Text.Trim()),
                    new SqlParameter("@Camera", txtCamera.Text.Trim()),
                    new SqlParameter("@Pin", txtPin.Text.Trim()),
                    new SqlParameter("@HeDieuHanh", txtHeDieuHanh.Text.Trim()),
                    new SqlParameter("@TrangThai", cboTrangThai.SelectedIndex == 0 ? 1 : 0),
                    new SqlParameter("@DanhMuc", cboDanhMucSP.Text)
                });

                if (result == -99) return;
                if (result == -1) MessageBox.Show("Mã máy đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    MessageBox.Show($"Thêm Dòng máy mới thành công!\nMã định danh tự động cấp là: {maMayTuDong}", "Thành công");
                    txtMaMay.Text = maMayTuDong;
                    LoadData_DongMay();
                }
            }
            catch (Exception ex) { MessageBox.Show("Sự cố thêm máy: " + ex.Message); }
        }

        private void dgvDienThoai_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvDienThoai.Rows[e.RowIndex];

                    txtMaMay.Text = row.Cells["MaMay"].Value?.ToString() ?? "";
                    txtTenMay.Text = row.Cells["TenMay"].Value?.ToString() ?? "";
                    cboHang.Text = row.Cells["TenHang"].Value?.ToString() ?? "";
                    cboDanhMucSP.Text = row.Cells["DanhMuc"].Value?.ToString() ?? "Smartphone";
                    txtCPU.Text = row.Cells["CPU"].Value?.ToString() ?? "";
                    txtRAM.Text = row.Cells["RAM"].Value?.ToString() ?? "";
                    txtCamera.Text = row.Cells["Camera"].Value?.ToString() ?? "";
                    txtPin.Text = row.Cells["Pin"].Value?.ToString() ?? "";
                    txtHeDieuHanh.Text = row.Cells["HeDieuHanh"].Value?.ToString() ?? "";
                    cboTrangThai.Text = row.Cells["TrangThai"].Value?.ToString() ?? "Đang bán";

                    LoadData_PhienBan(txtMaMay.Text);
                    txtMaChiTiet.Text = txtMaMay.Text + "-";
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi chọn dòng máy: " + ex.Message); }
        }

        private void dgvChiTietDienThoai_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvChiTietDienThoai.Rows[e.RowIndex];

                    txtMaChiTiet.Text = row.Cells["Mã Phiên Bản"].Value?.ToString() ?? "";
                    cboDungLuong.Text = row.Cells["Dung Lượng"].Value?.ToString() ?? "";
                    cboMauSac.Text = row.Cells["Màu Sắc"].Value?.ToString() ?? "";
                    txtGiaNhap.Text = row.Cells["Giá Vốn"].Value?.ToString().Replace(",", "") ?? "0";
                    txtGiaBan.Text = row.Cells["Giá Bán"].Value?.ToString().Replace(",", "") ?? "0";
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi chọn phiên bản: " + ex.Message); }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaMay.Text)) { MessageBox.Show("Vui lòng chọn dòng máy từ bảng!"); return; }

                var maHang = cboHang.SelectedValue?.ToString();
                int result = DatabaseHelper.ExecuteStoredProcedure("sp_SuaDienThoai", new[] {
                    new SqlParameter("@MaMay", txtMaMay.Text.Trim()),
                    new SqlParameter("@TenMay", txtTenMay.Text.Trim()),
                    new SqlParameter("@MaHang", maHang ?? (object)DBNull.Value),
                    new SqlParameter("@CPU", txtCPU.Text.Trim()),
                    new SqlParameter("@RAM", txtRAM.Text.Trim()),
                    new SqlParameter("@Camera", txtCamera.Text.Trim()),
                    new SqlParameter("@Pin", txtPin.Text.Trim()),
                    new SqlParameter("@HeDieuHanh", txtHeDieuHanh.Text.Trim()),
                    new SqlParameter("@TrangThai", cboTrangThai.SelectedIndex == 0 ? 1 : 0),
                    new SqlParameter("@DanhMuc", cboDanhMucSP.Text)
                });

                if (result != -99) { MessageBox.Show("Cập nhật Dòng máy thành công!"); LoadData_DongMay(); }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi cập nhật máy: " + ex.Message); }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaMay.Text)) return;

                if (MessageBox.Show("Ngừng bán máy này và tất cả các phiên bản cấu hình của nó?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int result = DatabaseHelper.ExecuteStoredProcedure("sp_XoaDienThoai", new[] { new SqlParameter("@MaMay", txtMaMay.Text.Trim()) });
                    if (result != -99) { MessageBox.Show("Đã ngừng kinh doanh!"); LoadData_DongMay(); dgvChiTietDienThoai.DataSource = null; }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMaMay.Clear(); txtTenMay.Clear(); txtCPU.Clear(); txtRAM.Clear();
            txtCamera.Clear(); txtPin.Clear(); txtHeDieuHanh.Clear();
            txtMaChiTiet.Clear(); txtGiaNhap.Clear(); txtGiaBan.Clear();
            if (cboDanhMucSP.Items.Count > 0) cboDanhMucSP.SelectedIndex = 0;
            txtMaMay.Focus();
        }

        private void btnLuuPhienBan_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaMay.Text)) { MessageBox.Show("Phải chọn Dòng máy trước!"); return; }
                if (string.IsNullOrWhiteSpace(txtMaChiTiet.Text)) { MessageBox.Show("Nhập Mã phiên bản chi tiết!"); return; }

                decimal giaNhap = decimal.TryParse(txtGiaNhap.Text, out var gn) ? gn : 0;
                decimal giaBan = decimal.TryParse(txtGiaBan.Text, out var gb) ? gb : 0;

                int result = DatabaseHelper.ExecuteStoredProcedure("sp_LuuChiTietDienThoai", new[] {
                    new SqlParameter("@MaChiTiet", txtMaChiTiet.Text.Trim()),
                    new SqlParameter("@MaMay", txtMaMay.Text.Trim()),
                    new SqlParameter("@DungLuong", cboDungLuong.Text),
                    new SqlParameter("@MauSac", cboMauSac.Text),
                    new SqlParameter("@GiaNhap", giaNhap),
                    new SqlParameter("@GiaBan", giaBan)
                });

                if (result != -99) { MessageBox.Show("Lưu Cấu hình thành công!"); LoadData_PhienBan(txtMaMay.Text); }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi lưu phiên bản: " + ex.Message); }
        }

        private void btnXoaPhienBan_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaChiTiet.Text)) return;
                if (MessageBox.Show("Xóa phiên bản chi tiết này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int result = DatabaseHelper.ExecuteStoredProcedure("sp_XoaChiTietDienThoai", new[] { new SqlParameter("@MaChiTiet", txtMaChiTiet.Text.Trim()) });
                    if (result != -99) { LoadData_PhienBan(txtMaMay.Text); }
                }
            }
            catch { }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDienThoai.Rows.Count == 0) { MessageBox.Show("Không có dữ liệu!"); return; }
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel (*.xls)|*.xls", FileName = "DanhSachDienThoai.xls" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.AppendLine("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
                        sb.AppendLine("<table border='1' style='font-family:Arial;'><tr><td colspan='10' align='center'><b>BÁO CÁO</b></td></tr>");
                        sb.AppendLine("<tr style='background-color:#2980b9; color:white;'>");
                        foreach (DataGridViewColumn col in dgvDienThoai.Columns) if (col.Visible) sb.AppendLine($"<td>{col.HeaderText}</td>");
                        sb.AppendLine("</tr>");

                        foreach (DataGridViewRow row in dgvDienThoai.Rows)
                        {
                            if (row.IsNewRow) continue;
                            sb.AppendLine("<tr>");
                            foreach (DataGridViewCell cell in row.Cells)
                                if (cell.OwningColumn.Visible) sb.AppendLine($"<td x:str>{cell.Value?.ToString()}</td>");
                            sb.AppendLine("</tr>");
                        }
                        sb.AppendLine("</table>");
                        System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi xuất Excel: {ex.Message}"); }
        }
        private void grpDongMay_Click(object sender, EventArgs e) { }
        private void ucDienThoai_Load(object sender, EventArgs e) { }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            LoadData_DongMay(txtTimKiem.Text.Trim());

        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {

        }

    }
}