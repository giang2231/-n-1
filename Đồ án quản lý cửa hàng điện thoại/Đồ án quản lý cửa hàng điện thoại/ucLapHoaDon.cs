using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers;

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class ucLapHoaDon : UserControl
    {
        private DataTable dtLapHoaDon = new DataTable();
        private string maKhachHangHienTai = "KH000";
        private decimal tongTienHang = 0;

        public ucLapHoaDon()
        {
            InitializeComponent();
        }

        private void UcLapHoaDon_Load(object sender, EventArgs e)
        {
            PhanQuyenUI();
            KhoiTaoDanhSachChoXuat();
            LoadDanhSachXuatBan();
            ResetForm();

            if (cboTrangThai.Items.Count > 0)
            {
                cboTrangThai.SelectedIndex = 0;
            }
        }

        // ==============================================================
        // 1. PHÂN QUYỀN GIAO DIỆN
        // ==============================================================
        private void PhanQuyenUI()
        {
            try
            {
                string role = Session.ChucVu != null ? Session.ChucVu.Trim().ToUpper() : "";

                if (role != "ADMIN" && role != "BÁN HÀNG")
                {
                    btnChotHoaDon.Visible = false;
                    btnTimKiemKH.Visible = false;
                    btnBoMayChon.Visible = false;
                    txtBanMaIMEI.Enabled = false;
                    txtMucGiamGia.Enabled = false;
                    MessageBox.Show("Bạn không có quyền lập hóa đơn. Chỉ được xem giao diện!", "Phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch { }
        }

        // ==============================================================
        // 2. KHỞI TẠO BẢNG CHỜ XUẤT (TRÊN RAM)
        // ==============================================================
        private void KhoiTaoDanhSachChoXuat()
        {
            dtLapHoaDon = new DataTable();
            dtLapHoaDon.Columns.Add("MaChiTietSP", typeof(string));
            dtLapHoaDon.Columns.Add("TenMay", typeof(string));
            dtLapHoaDon.Columns.Add("SoIMEI", typeof(string));
            dtLapHoaDon.Columns.Add("DonGia", typeof(decimal));

            dgvLapHoaDon.DataSource = dtLapHoaDon;
            dgvLapHoaDon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ==============================================================
        // 3. TẢI LỊCH SỬ HÓA ĐƠN ĐÃ BÁN
        // ==============================================================
        private void LoadDanhSachXuatBan()
        {
            try
            {
                string sql = @"
                    SELECT hd.MaHD AS [Mã Hóa Đơn], 
                           kh.HoTen AS [Tên Khách Hàng], 
                           hd.NgayLap AS [Ngày Lập], 
                           hd.ThanhTien AS [Thực Thu], 
                           hd.TrangThai AS [Trạng Thái], 
                           hd.GhiChu AS [Ghi Chú]
                    FROM HoaDon hd
                    LEFT JOIN KhachHang kh ON hd.MaKH = kh.MaKH
                    ORDER BY hd.NgayLap DESC";

                DataTable dtHD = DatabaseHelper.ExecuteQuery(sql);
                dgvDanhSachXuatBan.DataSource = dtHD;

                if (dgvDanhSachXuatBan.Columns.Count > 0)
                {
                    dgvDanhSachXuatBan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvDanhSachXuatBan.Columns["Thực Thu"].DefaultCellStyle.Format = "N0";
                    dgvDanhSachXuatBan.Columns["Thực Thu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách hóa đơn: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==============================================================
        // 4. QUÉT MÃ IMEI VÀO GIỎ HÀNG
        // ==============================================================
        private void txtBanMaIMEI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                string imeiQuet = txtBanMaIMEI.Text.Trim();
                if (string.IsNullOrEmpty(imeiQuet)) return;

                foreach (DataRow row in dtLapHoaDon.Rows)
                {
                    if (row["SoIMEI"].ToString() == imeiQuet)
                    {
                        MessageBox.Show("Mã IMEI này đã nằm trong danh sách chờ xuất hóa đơn!", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBanMaIMEI.Clear();
                        return;
                    }
                }

                try
                {
                    string sql = @"
                        SELECT pn.SoIMEI, 
                               (d.TenMay + ' ' + ct.DungLuong + ' ' + ct.MauSac) AS TenDayDu, 
                               pn.MaChiTietSP,
                               ct.GiaBan AS [GiaBan]   
                        FROM ChiTietPhieuNhap pn
                        JOIN ChiTietDienThoai ct ON pn.MaChiTietSP = ct.MaChiTiet
                        JOIN DienThoai d ON ct.MaMay = d.MaMay
                        WHERE pn.SoIMEI = @IMEI AND pn.TrangThaiIMEI = N'Trong kho'";

                    DataTable dtMay = DatabaseHelper.ExecuteQuery(sql, new[] { new SqlParameter("@IMEI", imeiQuet) });

                    if (dtMay != null && dtMay.Rows.Count > 0)
                    {
                        DataRow m = dtMay.Rows[0];
                        string maChiTietSP = m["MaChiTietSP"].ToString();
                        string tenMay = m["TenDayDu"].ToString();
                        string imei = m["SoIMEI"].ToString();
                        decimal donGia = m["GiaBan"] != DBNull.Value ? Convert.ToDecimal(m["GiaBan"]) : 0;

                        dtLapHoaDon.Rows.Add(maChiTietSP, tenMay, imei, donGia);

                        TinhTongTien();
                        txtBanMaIMEI.Clear();
                        txtBanMaIMEI.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy IMEI này trong kho, hoặc máy đã bị bán!", "Lỗi quét mã", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtBanMaIMEI.SelectAll();
                        txtBanMaIMEI.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==============================================================
        // 5. TÌM KHÁCH HÀNG THÀNH VIÊN
        // ==============================================================
        private void btnTimKiemKH_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSoDienThoaiKH.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại khách hàng cần tìm!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoDienThoaiKH.Focus();
                return;
            }

            string sql = "SELECT MaKH, HoTen, LoaiKhach, SoDienThoai FROM KhachHang WHERE SoDienThoai = @SoDienThoai";
            SqlParameter[] p = new SqlParameter[] { new SqlParameter("@SoDienThoai", txtSoDienThoaiKH.Text.Trim()) };
            DataTable dt = DatabaseHelper.ExecuteQuery(sql, p);

            if (dt != null && dt.Rows.Count > 0)
            {
                maKhachHangHienTai = dt.Rows[0]["MaKH"].ToString();
                string ten = dt.Rows[0]["HoTen"].ToString();
                string loai = dt.Rows[0]["LoaiKhach"].ToString();

                lblStatusKH.Text = $"→ Đã nhận diện: {ten} ({loai})";
                lblStatusKH.ForeColor = Color.Blue;
            }
            else
            {
                MessageBox.Show("Chưa có dữ liệu khách hàng này. Hệ thống ghi nhận Khách Vãng Lai.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                maKhachHangHienTai = "KH000";
                lblStatusKH.Text = "→ Hệ thống ghi nhận: Khách vãng lai";
                lblStatusKH.ForeColor = Color.Gray;
            }
        }

        // ==============================================================
        // 6. XÓA MÁY KHỎI GIỎ HÀNG
        // ==============================================================
        private void BtnBoMayChon_Click(object sender, EventArgs e)
        {
            if (dgvLapHoaDon.CurrentRow != null && !dgvLapHoaDon.CurrentRow.IsNewRow)
            {
                int index = dgvLapHoaDon.CurrentRow.Index;
                dtLapHoaDon.Rows.RemoveAt(index);
                TinhTongTien();

                // Mẹo nhỏ: Kích hoạt lại hàm tính tiền thực thu phòng khi có giảm giá
                TxtMucGiamGia_TextChanged(null, null);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một máy trong Giỏ hàng để hủy!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ==============================================================
        // 7. TÍNH TOÁN TIỀN TỆ
        // ==============================================================
        private void TinhTongTien()
        {
            decimal tongTien = 0;
            foreach (DataRow row in dtLapHoaDon.Rows)
            {
                tongTien += Convert.ToDecimal(row["DonGia"]);
            }
            tongTienHang = tongTien;
            lblCongTienHang.Text = "Tổng giá trị: " + tongTien.ToString("N0") + " VNĐ";
        }

        private void TxtMucGiamGia_TextChanged(object sender, EventArgs e)
        {
            decimal giamGia = 0;
            if (decimal.TryParse(txtMucGiamGia.Text.Trim().Replace(",", ""), out decimal result))
            {
                giamGia = result;
            }

            decimal thucThu = tongTienHang - giamGia;
            if (thucThu < 0) thucThu = 0;

            lblThucThuGiaoDich.Text = $"THỰC THU: {thucThu:#,##0} VNĐ";
        }

        // ==============================================================
        // 8. CHỐT HÓA ĐƠN (ĐÃ BỌC THÉP CHỐNG LỖI)
        // ==============================================================
        private void btnChotHoaDon_Click(object sender, EventArgs e)
        {
            if (dtLapHoaDon.Rows.Count == 0)
            {
                MessageBox.Show("Danh sách máy chờ xuất đang trống! Vui lòng quét IMEI trước.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Xác nhận lập bán hóa đơn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // KHÓA NÚT CHỐNG DOUBLE-CLICK (Chống lỗi HD051)
                btnChotHoaDon.Enabled = false;

                try
                {
                    // Lấy giảm giá và ép kiểu chuẩn
                    decimal giamGia = 0;
                    decimal.TryParse(txtMucGiamGia.Text.Trim().Replace(",", ""), out giamGia);

                    // CHỐT CHẶN: Chống nhập giảm giá lố tay
                    if (giamGia > tongTienHang)
                    {
                        MessageBox.Show("Mức giảm giá không được lớn hơn tổng tiền hàng!", "Cảnh báo xài tiền sếp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        btnChotHoaDon.Enabled = true; // Mở lại nút cho thao tác lại
                        return;
                    }

                    decimal thucThu = tongTienHang - giamGia;
                    string maNV = Session.MaNV ?? "NV001";
                    string maHDMoi = DatabaseHelper.GenerateCode("HD", "HoaDon", "MaHD");

                    txtMaHD.Text = maHDMoi; // Hiển thị mã

                    // THAO TÁC 1: LƯU HÓA ĐƠN CHA
                    string sqlTaoHD = @"
                        INSERT INTO HoaDon (MaHD, MaKH, MaNV, NgayLap, TongTien, GiamGia, ThanhTien, TrangThai, GhiChu)
                        VALUES (@MaHD, @MaKH, @MaNV, GETDATE(), @TongTien, @GiamGia, @ThanhTien, @TrangThai, @GhiChu)";

                    SqlParameter[] pHD = {
                        new SqlParameter("@MaHD", maHDMoi),
                        new SqlParameter("@MaKH", maKhachHangHienTai),
                        new SqlParameter("@MaNV", maNV),
                        new SqlParameter("@TongTien", tongTienHang),
                        new SqlParameter("@GiamGia", giamGia),
                        new SqlParameter("@ThanhTien", thucThu),
                        new SqlParameter("@TrangThai", cboTrangThai.Text.Trim()),
                        new SqlParameter("@GhiChu", txtGhiChuHD.Text.Trim())
                    };

                    DatabaseHelper.ExecuteNonQuery(sqlTaoHD, pHD);

                    // THAO TÁC 2: LƯU CHI TIẾT VÀ TRỪ KHO
                    foreach (DataRow row in dtLapHoaDon.Rows)
                    {
                        string imei = row["SoIMEI"].ToString();
                        decimal donGia = Convert.ToDecimal(row["DonGia"]);
                        string maCTSP = row["MaChiTietSP"].ToString();

                        string sqlChiTiet = @"
                            INSERT INTO ChiTietHoaDon (MaHD, SoIMEI, DonGia, SoLuong, MaChiTietSP)
                            VALUES (@MaHD, @SoIMEI, @DonGia, 1, @MaCTSP);
                            
                            UPDATE ChiTietPhieuNhap 
                            SET TrangThaiIMEI = N'Đã bán' 
                            WHERE SoIMEI = @SoIMEI;";

                        DatabaseHelper.ExecuteNonQuery(sqlChiTiet, new SqlParameter[] {
                            new SqlParameter("@MaHD", maHDMoi),
                            new SqlParameter("@SoIMEI", imei),
                            new SqlParameter("@DonGia", donGia),
                            new SqlParameter("@MaCTSP", maCTSP)
                        });
                    }

                    MessageBox.Show($"Thanh toán thành công! Mã hóa đơn của bác là: {maHDMoi}", "Tuyệt vời", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ResetForm();
                    LoadDanhSachXuatBan(); // CẬP NHẬT LƯỚI LỊCH SỬ NGAY LẬP TỨC
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất hóa đơn: " + ex.Message, "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // MỞ LẠI NÚT SAU KHI XỬ LÝ XONG (Dù thành công hay bị lỗi cũng phải mở)
                    btnChotHoaDon.Enabled = true;
                }
            }
        }

        // ==============================================================
        // 9. LÀM SẠCH FORM ĐÓN KHÁCH MỚI
        // ==============================================================
        private void ResetForm()
        {
            dtLapHoaDon.Clear();
            txtSoDienThoaiKH.Clear();
            txtMucGiamGia.Text = "0";
            txtGhiChuHD.Clear();
            txtBanMaIMEI.Clear();
            txtMaHD.Clear(); // Xóa mã hóa đơn cũ

            maKhachHangHienTai = "KH000";
            lblStatusKH.Text = "→ Hệ thống ghi nhận: Khách vãng lai";
            lblStatusKH.ForeColor = Color.Gray;

            TinhTongTien();
            TxtMucGiamGia_TextChanged(null, null); // Reset lại nhãn Thực Thu

            txtNhanVienLap.Text = Session.HoTen ?? "Admin";
        }

        private void txtBanMaIMEI_TextChanged(object sender, EventArgs e)
        {
            // Để trống nếu không dùng
        }
    }
}