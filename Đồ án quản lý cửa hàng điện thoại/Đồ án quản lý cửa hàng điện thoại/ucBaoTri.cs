using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers; // Nhớ đổi đúng namespace của bạn

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class ucBaoTri : UserControl
    {
        public ucBaoTri()
        {
            InitializeComponent();

            // Tự động đấu nối các sự kiện
            this.Load += UcBaoTri_Load;
            dgvBaoTri.CellClick += DgvBaoTri_CellClick;
            btnTiepNhan.Click += BtnTiepNhan_Click;
            btnCapNhat.Click += BtnCapNhat_Click;
            btnClear.Click += BtnClear_Click;
        }

        private void UcBaoTri_Load(object sender, EventArgs e)
        {
            // 1. Phân quyền người dùng ngay khi mở Form
            PhanQuyenUI();

            // 2. Load dữ liệu lên lưới
            LoadData();

            // 3. Xóa trắng form và sinh mã tự động
            BtnClear_Click(null, null);
        }

        // ==============================================================
        // 1. HÀM PHÂN QUYỀN GIAO DIỆN (CHỈ KỸ THUẬT VÀ ADMIN ĐƯỢC LÀM)
        // ==============================================================
        private void PhanQuyenUI()
        {
            try
            {
                string role = Session.ChucVu != null ? Session.ChucVu.Trim().ToUpper() : "";

                // Nếu là Nhân viên Bán hàng hoặc Kế toán -> Chỉ được xem, không được sửa
                if (role != "ADMIN" && role != "KỸ THUẬT")
                {
                    btnTiepNhan.Visible = false;
                    btnCapNhat.Visible = false;
                    txtChiPhi.Enabled = false; // Không cho phép sửa giá tiền
                    cboTrangThai.Enabled = false; // Không cho đổi trạng thái
                }
                else
                {
                    btnTiepNhan.Visible = true;
                    btnCapNhat.Visible = true;
                    txtChiPhi.Enabled = true;
                    cboTrangThai.Enabled = true;
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi phân quyền UI: " + ex.Message); }
        }

        // ==============================================================
        // 2. HÀM TẢI DỮ LIỆU DANH SÁCH BẢO TRÌ TỪ SQL LÊN GRID
        // ==============================================================
        private void LoadData()
        {
            try
            {
                string sql = @"
                    SELECT 
                        MaPhieuBT AS [Mã Phiếu], 
                        SoIMEI AS [Số IMEI], 
                        FORMAT(NgayTiepNhan, 'dd/MM/yyyy') AS [Ngày Nhận], 
                        FORMAT(NgayHenTra, 'dd/MM/yyyy') AS [Ngày Hẹn Trả], 
                        TinhTrangNhan AS [Mô Tả Lỗi], 
                        FORMAT(ChiPhiSua, '#,##0') AS [Chi Phí], 
                        TrangThai AS [Trạng Thái]
                    FROM PhieuBaoTri 
                    ORDER BY MaPhieuBT DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvBaoTri.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách bảo trì: " + ex.Message); }
        }

        // ==============================================================
        // 3. SỰ KIỆN CLICK VÀO LƯỚI ĐỂ HIỂN THỊ CHI TIẾT SANG BÊN PHẢI
        // ==============================================================
        private void DgvBaoTri_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvBaoTri.Rows[e.RowIndex];

                    txtMaPhieu.Text = row.Cells["Mã Phiếu"].Value?.ToString();
                    txtIMEI.Text = row.Cells["Số IMEI"].Value?.ToString();
                    txtMoTa.Text = row.Cells["Mô Tả Lỗi"].Value?.ToString();

                    string chiPhi = row.Cells["Chi Phí"].Value?.ToString();
                    txtChiPhi.Text = chiPhi != "" ? chiPhi.Replace(",", "") : "0";

                    cboTrangThai.Text = row.Cells["Trạng Thái"].Value?.ToString();

                    if (row.Cells["Ngày Hẹn Trả"].Value != DBNull.Value && row.Cells["Ngày Hẹn Trả"].Value.ToString() != "")
                    {
                        dtpHenTra.Value = DateTime.ParseExact(row.Cells["Ngày Hẹn Trả"].Value.ToString(), "dd/MM/yyyy", null);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi chọn phiếu: " + ex.Message); }
        }

        // ==============================================================
        // 4. NÚT TIẾP NHẬN MỚI (INSERT)
        // ==============================================================
        private void BtnTiepNhan_Click(object sender, EventArgs e)
        {
            if (txtIMEI.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng quét hoặc nhập số IMEI của máy khách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Mặc định khách vãng lai (KH000) và Máy chưa xác định (MM00) nếu chỉ quét IMEI bảo hành ngoài
                string maNV = Session.MaNV ?? "NV001";

                string sql = @"
                    INSERT INTO PhieuBaoTri (MaPhieuBT, MaKH, MaNV, MaMay, SoIMEI, NgayTiepNhan, NgayHenTra, TinhTrangNhan, ChiPhiSua, TrangThai) 
                    VALUES (@MaPhieu, 'KH000', @MaNV, 'MM01', @IMEI, GETDATE(), @HenTra, @MoTa, @ChiPhi, @TrangThai)";

                SqlParameter[] p = new SqlParameter[]
                {
                    new SqlParameter("@MaPhieu", txtMaPhieu.Text),
                    new SqlParameter("@MaNV", maNV),
                    new SqlParameter("@IMEI", txtIMEI.Text.Trim()),
                    new SqlParameter("@HenTra", dtpHenTra.Value),
                    new SqlParameter("@MoTa", txtMoTa.Text.Trim()),
                    new SqlParameter("@ChiPhi", txtChiPhi.Text == "" ? 0 : Convert.ToDecimal(txtChiPhi.Text)),
                    new SqlParameter("@TrangThai", cboTrangThai.Text)
                };

                int result = DatabaseHelper.ExecuteNonQuery(sql, p);
                if (result > 0)
                {
                    MessageBox.Show("Tiếp nhận bảo trì thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    BtnClear_Click(null, null);
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tiếp nhận: " + ex.Message); }
        }

        // ==============================================================
        // 5. NÚT CẬP NHẬT TRẠNG THÁI & CHI PHÍ (UPDATE)
        // ==============================================================
        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            if (txtMaPhieu.Text == "")
            {
                MessageBox.Show("Vui lòng chọn một phiếu trên lưới để cập nhật!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string sql = @"
                    UPDATE PhieuBaoTri 
                    SET NgayHenTra = @HenTra, TinhTrangNhan = @MoTa, ChiPhiSua = @ChiPhi, TrangThai = @TrangThai
                    WHERE MaPhieuBT = @MaPhieu";

                SqlParameter[] p = new SqlParameter[]
                {
                    new SqlParameter("@MaPhieu", txtMaPhieu.Text),
                    new SqlParameter("@HenTra", dtpHenTra.Value),
                    new SqlParameter("@MoTa", txtMoTa.Text.Trim()),
                    new SqlParameter("@ChiPhi", txtChiPhi.Text == "" ? 0 : Convert.ToDecimal(txtChiPhi.Text)),
                    new SqlParameter("@TrangThai", cboTrangThai.Text)
                };

                int result = DatabaseHelper.ExecuteNonQuery(sql, p);
                if (result > 0)
                {
                    MessageBox.Show("Cập nhật tiến độ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi cập nhật: " + ex.Message); }
        }

        // ==============================================================
        // 6. XÓA TRẮNG FORM & TỰ ĐỘNG SINH MÃ MỚI
        // ==============================================================
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtIMEI.Clear();
            txtMoTa.Clear();
            txtChiPhi.Text = "0";
            dtpHenTra.Value = DateTime.Now.AddDays(3); // Mặc định hẹn trả sau 3 ngày
            if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0; // Mặc định: Đang kiểm tra

            // Thuật toán sinh mã phiếu mới nhất (PBT01, PBT50, PBT51...)
            try
            {
                string sql = "SELECT TOP 1 MaPhieuBT FROM PhieuBaoTri ORDER BY MaPhieuBT DESC";
                object result = DatabaseHelper.ExecuteScalar(sql);

                if (result != null && result.ToString() != "")
                {
                    string maCu = result.ToString();
                    int so = int.Parse(maCu.Substring(3)) + 1;
                    txtMaPhieu.Text = "PBT" + so.ToString("D2"); // Formats thành PBT01, PBT02...
                }
                else
                {
                    txtMaPhieu.Text = "PBT01";
                }
            }
            catch { txtMaPhieu.Text = "PBT01"; }
        }
    }
}