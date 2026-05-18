using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers;

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class ucPhieuNhap : UserControl
    {
        // Tạo một DataTable ảo (RAM) để làm "Giỏ hàng chờ nhập"
        private DataTable dtTam;
        private decimal tongTienPhieu = 0;

        public ucPhieuNhap()
        {
            InitializeComponent();

            // Đăng ký sự kiện Enter cho TextBox IMEI để hỗ trợ máy quét mã vạch
            this.txtIMEI.KeyDown += TxtIMEI_KeyDown;
        }

        private void UcPhieuNhap_Load(object sender, EventArgs e)
        {
            LoadDanhSachPhieuNhap();
            LoadComboBoxHang();
            KhoiTaoGioHangTam();
            ResetFormNhap();

            btnHuyPhieu.Enabled = false;
        }

        #region KHỞI TẠO & LOAD DỮ LIỆU CƠ BẢN

        private void KhoiTaoGioHangTam()
        {
            dtTam = new DataTable();
            dtTam.Columns.Add("MaChiTiet", typeof(string));
            dtTam.Columns.Add("TenSP", typeof(string));
            dtTam.Columns.Add("SoIMEI", typeof(string));
            dtTam.Columns.Add("DonGiaNhap", typeof(decimal));

            dgvTam.DataSource = dtTam;

            // Định dạng tiền tệ cho lưới tạm
            if (dgvTam.Columns.Contains("DonGiaNhap"))
                dgvTam.Columns["DonGiaNhap"].DefaultCellStyle.Format = "N0";

            dgvTam.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadDanhSachPhieuNhap()
        {
            try
            {
                string sql = @"
                    SELECT p.MaPhieu AS [Mã Phiếu], h.TenHang AS [Hãng NCC], 
                           nv.HoTen AS [Người Lập], p.NgayNhap AS [Ngày Nhập], 
                           p.TongTien AS [Tổng Tiền], p.TrangThai AS [Trạng Thái]
                    FROM PhieuNhap p
                    LEFT JOIN HangCungCap h ON p.MaHang = h.MaHang
                    LEFT JOIN NhanVien nv ON p.MaNV = nv.MaNV
                    ORDER BY p.NgayNhap DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvPhieu.DataSource = dt;

                if (dgvPhieu.Columns.Contains("Tổng Tiền"))
                    dgvPhieu.Columns["Tổng Tiền"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải lịch sử: " + ex.Message); }
        }

        private void LoadComboBoxHang()
        {
            try
            {
                string sql = "SELECT MaHang, TenHang FROM HangCungCap";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                cboHang.DataSource = dt;
                cboHang.DisplayMember = "TenHang";
                cboHang.ValueMember = "MaHang";
                cboHang.SelectedIndex = -1; // Để trống lúc đầu
            }
            catch { }
        }

        private void ResetFormNhap()
        {
            try { txtMaPhieu.Text = DatabaseHelper.GenerateCode("PN", "PhieuNhap", "MaPhieu"); }
            catch { txtMaPhieu.Text = "PN001"; }

            cboHang.SelectedIndex = -1;
            cboMaMay.DataSource = null;
            txtGiaNhap.Text = "";
            txtIMEI.Text = "";

            dtTam.Clear();
            CachNhatTongTien();
        }

        #endregion

        #region SỰ KIỆN COMBOBOX (CHỌN HÃNG -> HIỆN ĐIỆN THOẠI)

        private void CboHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboHang.SelectedIndex == -1 || cboHang.SelectedValue == null) return;

            try
            {
                // Load danh sách điện thoại chi tiết thuộc Hãng này
                string maHang = cboHang.SelectedValue.ToString();
                string sql = @"
                    SELECT pb.MaChiTiet, (dt.TenMay + ' - ' + pb.DungLuong + ' ' + pb.MauSac) AS TenSP 
                    FROM ChiTietDienThoai pb 
                    JOIN DienThoai dt ON pb.MaMay = dt.MaMay 
                    WHERE dt.MaHang = @MaHang";

                SqlParameter[] param = { new SqlParameter("@MaHang", maHang) };
                DataTable dtMay = DatabaseHelper.ExecuteQuery(sql, param);

                cboMaMay.DataSource = dtMay;
                cboMaMay.DisplayMember = "TenSP";
                cboMaMay.ValueMember = "MaChiTiet";
                cboMaMay.SelectedIndex = -1;
            }
            catch { }
        }

        private void CboMaMay_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Để trống, hoặc có thể code tự động gợi ý Giá Nhập cũ nếu cần
        }

        #endregion

        #region XỬ LÝ THÊM / XÓA IMEI VÀO GIỎ HÀNG NHẬP

        // Hỗ trợ máy quét mã vạch: Quét xong máy sẽ tự gửi phím Enter
        private void TxtIMEI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Ngăn tiếng "ting" của Windows
                BtnThemCT_Click(sender, e);
            }
        }

        private void BtnThemCT_Click(object sender, EventArgs e)
        {
            if (cboMaMay.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn dòng máy muốn nhập!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string imei = txtIMEI.Text.Trim();
            if (string.IsNullOrEmpty(imei))
            {
                MessageBox.Show("Vui lòng quét hoặc nhập số IMEI!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal DonGiaNhap = 0;
            if (!decimal.TryParse(txtGiaNhap.Text.Trim(), out DonGiaNhap) || DonGiaNhap <= 0)
            {
                MessageBox.Show("Giá nhập không hợp lệ!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra IMEI trùng trong giỏ hàng tạm
            foreach (DataRow r in dtTam.Rows)
            {
                if (r["SoIMEI"].ToString() == imei)
                {
                    MessageBox.Show("Số IMEI này đã được quét trong danh sách!", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtIMEI.SelectAll();
                    return;
                }
            }

            // Thêm vào giỏ hàng
            DataRow row = dtTam.NewRow();
            row["MaChiTiet"] = cboMaMay.SelectedValue.ToString();
            row["TenSP"] = cboMaMay.Text;
            row["SoIMEI"] = imei;
            row["DonGiaNhap"] = DonGiaNhap;
            dtTam.Rows.Add(cboMaMay.SelectedValue.ToString(), cboMaMay.Text, imei, DonGiaNhap);

            CachNhatTongTien();

            // Xóa rỗng ô IMEI để quét tiếp máy sau
            txtIMEI.Text = "";
            txtIMEI.Focus();
        }

        private void BtnXoaSP_Click(object sender, EventArgs e)
        {
            if (dgvTam.CurrentRow != null)
            {
                dgvTam.Rows.RemoveAt(dgvTam.CurrentRow.Index);
                CachNhatTongTien();
            }
        }

        private void CachNhatTongTien()
        {
            tongTienPhieu = 0;
            foreach (DataRow row in dtTam.Rows)
            {
                tongTienPhieu += Convert.ToDecimal(row["DonGiaNhap"]);
            }
            lblTongTien.Text = $"TỔNG TIỀN NHẬP: {tongTienPhieu:N0} VND";
        }

        #endregion

        #region CHỐT LƯU PHIẾU NHẬP KHO (TRANSACTION)

        private void BtnTaoPhieu_Click(object sender, EventArgs e)
        {
            if (dtTam.Rows.Count == 0)
            {
                MessageBox.Show("Danh sách nhập đang trống! Không thể tạo phiếu.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maHang = cboHang.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(maHang))
            {
                MessageBox.Show("Vui lòng chọn Hãng cung cấp cho phiếu nhập này!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Xác nhận chốt lưu phiếu nhập kho này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Lấy User đăng nhập (Nghiệp vụ session)
                string maNV = Session.MaNV ?? "NV001";
                string maPhieu = txtMaPhieu.Text.Trim();

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. LƯU BẢNG PHIẾU NHẬP HÀNG
                            string sqlPhieu = @"
                                INSERT INTO PhieuNhap (MaPhieu, MaHang, MaNV, NgayNhap, TongTien, TrangThai, GhiChu)
                                VALUES (@MaPhieu, @MaHang, @MaNV, GETDATE(), @TongTien, N'Hoàn thành', @GhiChu)";

                            using (SqlCommand cmdPhieu = new SqlCommand(sqlPhieu, conn, trans))
                            {
                                cmdPhieu.Parameters.AddWithValue("@MaPhieu", maPhieu);
                                cmdPhieu.Parameters.AddWithValue("@MaHang", maHang);
                                cmdPhieu.Parameters.AddWithValue("@MaNV", maNV);
                                cmdPhieu.Parameters.AddWithValue("@TongTien", tongTienPhieu);
                                cmdPhieu.Parameters.AddWithValue("@GhiChu", "Nhập hàng mới");
                                cmdPhieu.ExecuteNonQuery();
                            }

                            // 2. LƯU TỪNG MÁY VÀO CHI TIẾT PHIẾU NHẬP (TRẠNG THÁI 'TRONG KHO')
                            string sqlChiTiet = @"
                                INSERT INTO ChiTietPhieuNhap (MaPhieu, SoIMEI, MaChiTiet, DonGiaNhap, TrangThaiIMEI)
                                VALUES (@MaPhieu, @SoIMEI, @MaChiTiet, @DonGiaNhap, N'Trong kho')";

                            foreach (DataRow row in dtTam.Rows)
                            {
                                using (SqlCommand cmdCT = new SqlCommand(sqlChiTiet, conn, trans))
                                {
                                    cmdCT.Parameters.AddWithValue("@MaPhieu", maPhieu);
                                    cmdCT.Parameters.AddWithValue("@SoIMEI", row["SoIMEI"].ToString());
                                    cmdCT.Parameters.AddWithValue("@MaChiTiet", row["MaChiTiet"].ToString());
                                    cmdCT.Parameters.AddWithValue("@DonGiaNhap", Convert.ToDecimal(row["DonGiaNhap"]));
                                    cmdCT.ExecuteNonQuery();
                                }
                            }

                            // 3. COMMIT NẾU THÀNH CÔNG
                            trans.Commit();
                            MessageBox.Show("Lưu phiếu nhập và cất điện thoại vào kho thành công!", "Tuyệt vời", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadDanhSachPhieuNhap();
                            ResetFormNhap();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            MessageBox.Show("Giao dịch lỗi! Đã hoàn tác để bảo vệ database.\nChi tiết: " + ex.Message, "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        #endregion

        #region XEM LỊCH SỬ & HỦY PHIẾU NHẬP CŨ

        private void DgvPhieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPhieu.Rows[e.RowIndex];
                string maPhieu = row.Cells["Mã Phiếu"].Value.ToString();
                string trangThai = row.Cells["Trạng Thái"].Value?.ToString();

                // Nạp chi tiết các máy IMEI của phiếu này lên dgvChiTiet
                try
                {
                    string sql = @"
                    SELECT ct.SoIMEI AS [Số IMEI], 
                           (dt.TenMay + ' ' + pb.DungLuong) AS [Sản Phẩm],
                           ct.DonGiaNhap AS [Giá Nhập], ct.TrangThaiIMEI AS [Trạng Thái]
                    FROM ChiTietPhieuNhap ct
                    -- NỐI BẢNG QUA CỘT MaChiTietSP (vì nó chứa chuỗi CT01)
                    JOIN ChiTietDienThoai pb ON ct.MaChiTietSP = pb.MaChiTiet
                    JOIN DienThoai dt ON pb.MaMay = dt.MaMay
                    WHERE ct.MaPhieu = @MaPhieu";

                    SqlParameter[] param = { new SqlParameter("@MaPhieu", maPhieu) };
                    DataTable dtChiTiet = DatabaseHelper.ExecuteQuery(sql, param);

                    dgvChiTiet.DataSource = dtChiTiet;

                    if (dgvChiTiet.Columns.Contains("Giá Nhập"))
                        dgvChiTiet.Columns["Giá Nhập"].DefaultCellStyle.Format = "N0";
                    dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                catch { }

                // Logic bật tắt nút Hủy an toàn
                if (trangThai == "Đã hủy")
                {
                    btnHuyPhieu.Enabled = false;
                    btnHuyPhieu.FillColor = System.Drawing.Color.Gray;
                    btnHuyPhieu.Text = "Phiếu Này Đã Bị Hủy";
                }
                else
                {
                    btnHuyPhieu.Enabled = true;
                    btnHuyPhieu.FillColor = System.Drawing.Color.FromArgb(231, 76, 60);
                    btnHuyPhieu.Text = "Hủy Phiếu Chọn";
                }
            }
        }

        private void BtnHuyPhieu_Click(object sender, EventArgs e)
        {
            if (dgvPhieu.CurrentRow == null) return;

            string maPhieu = dgvPhieu.CurrentRow.Cells["Mã Phiếu"].Value.ToString();

            if (MessageBox.Show($"Bạn có thực sự muốn HỦY phiếu nhập kho [{maPhieu}] không?\n\nChú ý: Toàn bộ máy IMEI thuộc phiếu này sẽ bị khóa khỏi hệ thống bán hàng!", "Cảnh báo bảo mật", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                try
                {
                    // Gọi Stored Procedure bọc thép sp_HuyPhieuNhap mà chúng ta đã làm
                    SqlParameter[] p = { new SqlParameter("@MaPhieu", maPhieu) };
                    int res = DatabaseHelper.ExecuteStoredProcedure("sp_HuyPhieuNhap", p);

                    if (res != -99)
                    {
                        MessageBox.Show("Hủy phiếu nhập hàng thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDanhSachPhieuNhap();
                        dgvChiTiet.DataSource = null; // Xóa trắng lưới chi tiết
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion
    }
}