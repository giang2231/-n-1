using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using Microsoft.Data.SqlClient;
using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers; // Chú ý đổi namespace cho khớp project của bác

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class ucBaoCao : UserControl
    {
        public ucBaoCao()
        {
            InitializeComponent();

            // TỰ ĐỘNG NỐI SỰ KIỆN CLICK NÚT IN BÁO CÁO RA EXCEL
            btnIn.Click += btnIn_Click;
        }

        private void UcBaoCao_Load(object sender, EventArgs e)
        {
            // Thiết lập mặc định: Lọc từ đầu tháng đến ngày hiện tại
            dtpTu.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpDen.Value = DateTime.Now;

            // Chọn mặc định báo cáo đầu tiên
            if (cboBaoCao.Items.Count > 0)
            {
                cboBaoCao.SelectedIndex = 0;
            }
        }

        // ====================================================================
        // NÚT XEM BÁO CÁO - KÍCH HOẠT TRUY VẤN
        // ====================================================================
        private void btnXem_Click(object sender, EventArgs e)
        {
            if (dtpTu.Value > dtpDen.Value)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Lỗi chọn ngày", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gọi hàm tính tổng Doanh thu - Chi phí - Lợi nhuận trước
            TinhTongKetDoanhThu();

            // Tùy theo lựa chọn trong ComboBox để chạy câu SQL tương ứng
            int loaiBaoCao = cboBaoCao.SelectedIndex;
            LoadDuLieuBaoCao(loaiBaoCao);
        }

        // ====================================================================
        // HÀM XỬ LÝ LƯỚI DATA GRID VIEW (4 LOẠI BÁO CÁO)
        // ====================================================================
        private void LoadDuLieuBaoCao(int loai)
        {
            try
            {
                string sql = "";
                SqlParameter[] p = new SqlParameter[]
                {
                    new SqlParameter("@TuNgay", dtpTu.Value.Date),
                    new SqlParameter("@DenNgay", dtpDen.Value.Date.AddDays(1).AddTicks(-1))
                };

                switch (loai)
                {
                    case 0: // 1. DOANH THU THEO THÁNG
                        sql = @"
                            SELECT FORMAT(NgayLap, 'MM/yyyy') AS [Tháng/Năm], 
                                   COUNT(MaHD) AS [Số Lượng Hóa Đơn], 
                                   SUM(TongTien) AS [Tổng Tiền Hàng], 
                                   SUM(GiamGia) AS [Đã Giảm Giá], 
                                   SUM(ThanhTien) AS [Doanh Thu Thực Tế]
                            FROM HoaDon
                            WHERE NgayLap >= @TuNgay AND NgayLap <= @DenNgay AND TrangThai = N'Đã thanh toán'
                            GROUP BY FORMAT(NgayLap, 'MM/yyyy')
                            ORDER BY [Tháng/Năm] DESC";
                        break;

                    case 1: // 2. SẢN PHẨM BÁN CHẠY
                        sql = @"
                            SELECT (d.TenMay + ' ' + ct.DungLuong) AS [Tên Sản Phẩm], 
                                   COUNT(cthd.SoIMEI) AS [Số Lượng Đã Bán], 
                                   SUM(cthd.DonGia) AS [Tổng Thu Về]
                            FROM ChiTietHoaDon cthd
                            JOIN HoaDon hd ON cthd.MaHD = hd.MaHD
                            JOIN ChiTietDienThoai ct ON cthd.MaChiTietSP = ct.MaChiTiet
                            JOIN DienThoai d ON ct.MaMay = d.MaMay
                            WHERE hd.NgayLap >= @TuNgay AND hd.NgayLap <= @DenNgay AND hd.TrangThai = N'Đã thanh toán'
                            GROUP BY d.TenMay, ct.DungLuong
                            ORDER BY [Số Lượng Đã Bán] DESC";
                        break;

                    case 2: // 3. TỒN KHO THEO HÃNG
                        sql = @"
                            SELECT hcc.TenHang AS [Hãng Sản Xuất], 
                                   COUNT(pn.SoIMEI) AS [Số Lượng Tồn Kho], 
                                   SUM(ctdt.GiaBan) AS [Giá Trị Tồn Ước Tính]
                            FROM ChiTietPhieuNhap pn
                            JOIN ChiTietDienThoai ctdt ON pn.MaChiTietSP = ctdt.MaChiTiet
                            JOIN DienThoai d ON ctdt.MaMay = d.MaMay
                            JOIN HangCungCap hcc ON d.MaHang = hcc.MaHang
                            WHERE pn.TrangThaiIMEI = N'Trong kho'
                            GROUP BY hcc.TenHang
                            ORDER BY [Số Lượng Tồn Kho] DESC";
                        break;

                    case 3: // 4. DOANH SỐ THEO NHÂN VIÊN
                        sql = @"
                            SELECT nv.MaNV AS [Mã NV], 
                                   nv.HoTen AS [Tên Nhân Viên], 
                                   COUNT(hd.MaHD) AS [Số Hóa Đơn Đã Lập], 
                                   SUM(hd.ThanhTien) AS [Doanh Số Mang Về]
                            FROM HoaDon hd
                            JOIN NhanVien nv ON hd.MaNV = nv.MaNV
                            WHERE hd.NgayLap >= @TuNgay AND hd.NgayLap <= @DenNgay AND hd.TrangThai = N'Đã thanh toán'
                            GROUP BY nv.MaNV, nv.HoTen
                            ORDER BY [Doanh Số Mang Về] DESC";
                        break;
                }

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, p);
                dgvBaoCao.DataSource = dt;

                FormatDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lập báo cáo: " + ex.Message, "Lỗi truy vấn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================================================================
        // HÀM SỰ KIỆN: XUẤT DỮ LIỆU LƯỚI RA FILE EXCEL (SIÊU TỐC & AN TOÀN)
        // ====================================================================
        private void btnIn_Click(object sender, EventArgs e)
        {
            if (dgvBaoCao.Rows.Count == 0)
            {
                MessageBox.Show("Hiện tại trên lưới không có dữ liệu để xuất Excel! Vui lòng chọn loại báo cáo và bấm [Xem BC] trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            // Tự động đặt tên file gợi ý theo tên loại báo cáo và mốc thời gian xuất file
            string tenBaoCaoFormated = cboBaoCao.Text.Replace(" ", "_");
            saveFileDialog.FileName = $"BaoCao_{tenBaoCaoFormated}_{DateTime.Now:yyyyMMdd_HHmmss}";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Dùng StreamWriter mở file với bảng mã Unicode để không bao giờ bị lỗi font tiếng Việt
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.Unicode))
                    {
                        // Khai báo thẻ HTML chuẩn cấu trúc Excel
                        sw.WriteLine("<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");
                        sw.WriteLine("<head><meta http-equiv=\"content-type\" content=\"application/vnd.ms-excel; charset=UTF-8\"></head>");
                        sw.WriteLine("<body>");

                        // Tiêu đề lớn nằm trên cùng file Excel
                        sw.WriteLine($"<h3>BÁO CÁO: {cboBaoCao.Text.ToUpper()}</h3>");
                        sw.WriteLine($"<p>Thời gian lọc: Từ {dtpTu.Value:dd/MM/yyyy} đến {dtpDen.Value:dd/MM/yyyy}</p>");

                        // Tạo bảng Excel có Style bắt sáng viền mượt mà
                        sw.WriteLine("<table border='1' style='border-collapse:collapse; font-family:Segoe UI, Arial; font-size:11pt;'>");

                        // 1. Ghi tiêu đề cột (Màu xanh đậm hoàng gia chuyên nghiệp)
                        sw.WriteLine("<tr style='background-color:#2980b9; color:white; font-weight:bold; height:30px;'>");
                        foreach (DataGridViewColumn col in dgvBaoCao.Columns)
                        {
                            sw.WriteLine($"<th style='padding:5px;'>{col.HeaderText}</th>");
                        }
                        sw.WriteLine("</tr>");

                        // 2. Ghi toàn bộ dữ liệu dòng từ DataGridView xuống
                        foreach (DataGridViewRow row in dgvBaoCao.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                sw.WriteLine("<tr style='height:25px;'>");
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    string giaTriCell = cell.Value?.ToString() ?? "";

                                    // Kiểm tra nếu ô này là dạng số/tiền thì căn lề phải cho chuẩn quy tắc kế toán
                                    if (decimal.TryParse(giaTriCell, out decimal numerical))
                                    {
                                        // Ghi số vào excel, để excel tự format
                                        sw.WriteLine($"<td align='right' style='padding:5px;'>{numerical}</td>");
                                    }
                                    else
                                    {
                                        sw.WriteLine($"<td align='left' style='padding:5px;'>{giaTriCell}</td>");
                                    }
                                }
                                sw.WriteLine("</tr>");
                            }
                        }

                        // 3. Khúc bổ sung: Nếu là báo cáo Doanh Thu, in thêm 1 dòng tổng kết tài chính ở đáy bảng Excel
                        if (cboBaoCao.SelectedIndex == 0 || cboBaoCao.SelectedIndex == 1 || cboBaoCao.SelectedIndex == 3)
                        {
                            sw.WriteLine("<tr style='background-color:#f1f2f6; font-weight:bold; height:28px;'>");
                            sw.WriteLine($"<td colspan='{dgvBaoCao.Columns.Count}' style='padding:5px; color:#2e7d32;'>");
                            sw.WriteLine($"* {lblTongDoanhThu.Text} | {lblTongChiPhi.Text} | {lblLoiNhuan.Text}");
                            sw.WriteLine("</td>");
                            sw.WriteLine("</tr>");
                        }

                        sw.WriteLine("</table></body></html>");
                    }

                    MessageBox.Show("Đã xuất báo cáo và tạo bảng dữ liệu Excel thành công rực rỡ!", "Tuyệt vời", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ====================================================================
        // HÀM TÍNH LỢI NHUẬN THỰC TẾ
        // ====================================================================
        private void TinhTongKetDoanhThu()
        {
            try
            {
                SqlParameter[] p = new SqlParameter[]
                {
                    new SqlParameter("@TuNgay", dtpTu.Value.Date),
                    new SqlParameter("@DenNgay", dtpDen.Value.Date.AddDays(1).AddTicks(-1))
                };

                string sqlDoanhThu = "SELECT ISNULL(SUM(ThanhTien), 0) FROM HoaDon WHERE NgayLap >= @TuNgay AND NgayLap <= @DenNgay AND TrangThai = N'Đã thanh toán'";
                decimal doanhThu = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(sqlDoanhThu, p));

                string sqlChiPhi = @"
                    SELECT ISNULL(SUM(pn.DonGiaNhap), 0)
                    FROM ChiTietHoaDon cthd
                    JOIN HoaDon hd ON cthd.MaHD = hd.MaHD
                    JOIN ChiTietPhieuNhap pn ON cthd.SoIMEI = pn.SoIMEI
                    WHERE hd.NgayLap >= @TuNgay AND hd.NgayLap <= @DenNgay AND hd.TrangThai = N'Đã thanh toán'";

                SqlParameter[] p2 = new SqlParameter[] {
                    new SqlParameter("@TuNgay", dtpTu.Value.Date),
                    new SqlParameter("@DenNgay", dtpDen.Value.Date.AddDays(1).AddTicks(-1))
                };
                decimal chiPhi = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(sqlChiPhi, p2));

                decimal loiNhuan = doanhThu - chiPhi;

                lblTongDoanhThu.Text = $"Doanh thu: {doanhThu:N0} VNĐ";
                lblTongChiPhi.Text = $"Chi phí gốc: {chiPhi:N0} VNĐ";
                lblLoiNhuan.Text = $"Lợi nhuận: {loiNhuan:N0} VNĐ";
            }
            catch { }
        }

        // ====================================================================
        // HÀM ĐỊNH DẠNG LƯỚI CHO ĐẸP
        // ====================================================================
        private void FormatDataGrid()
        {
            if (dgvBaoCao.Columns.Count > 0)
            {
                dgvBaoCao.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                foreach (DataGridViewColumn col in dgvBaoCao.Columns)
                {
                    if (col.Name.Contains("Tiền") || col.Name.Contains("Thu") || col.Name.Contains("Giá") || col.Name.Contains("Doanh"))
                    {
                        col.DefaultCellStyle.Format = "N0";
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    else if (col.Name.Contains("Số Lượng"))
                    {
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }
            }
        }
    }
}