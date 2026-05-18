using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers;
using System;
using System.Windows.Forms;

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class Trang_Chủcs : Form
    {
        public Trang_Chủcs()
        {
            InitializeComponent();
            lblUserInfo.Text = $"Xin chào, {Session.HoTen} ({Session.ChucVu})!";
            ThietLapPhanQuyen();
        }
        private void ThietLapPhanQuyen()
        {
            try
            {
                // 1. Lấy chức vụ và IN HOA TOÀN BỘ để tránh lỗi gõ sai chữ hoa/chữ thường
                string role = Session.ChucVu != null ? Session.ChucVu.Trim().ToUpper() : "";

                // MẸO: Tạm thời bật thông báo này lên để xem C# nhận được chữ gì từ SQL
                // MessageBox.Show("Hệ thống nhận diện bạn có quyền: [" + role + "]", "Test Phân Quyền");

                // 2. ĐÓNG SẬP CỬA: Khóa TẤT CẢ các nút trên Sidebar
                // (Lưu ý: Bạn hãy tự sửa lại tên mấy cái btn... dưới đây cho khớp với tên trên máy bạn nhé)
                btnDienThoai.Visible = false;
                btnHangCungCap.Visible = false;
                btnKhachHang.Visible = false;
                btnLapHoaDon.Visible = false;
                btnHoaDon.Visible = false;     // Quản lý hóa đơn lịch sử
                btnPhieuNhap.Visible = false;   // Nhập kho
                btnBaoTri.Visible = false;
                btnBaoCao.Visible = false;
                btnNhanVien.Visible = false;

                // 3. XÉT DUYỆT VÀ MỞ CỬA TỪNG PHÒNG BAN
                switch (role)
                {
                    case "ADMIN":
                        // Boss tổng: Mở full không trượt phát nào
                        btnDienThoai.Visible = true;
                        btnHangCungCap.Visible = true;
                        btnKhachHang.Visible = true;
                        btnLapHoaDon.Visible = true;
                        btnHoaDon.Visible = true;
                        btnPhieuNhap.Visible = true;
                        btnBaoTri.Visible = true;
                        btnBaoCao.Visible = true;
                        btnNhanVien.Visible = true;
                        break;

                    case "KẾ TOÁN":
                        // Kế toán: Quản lý danh mục, chứng từ, kho và xem tiền (Không được bán máy)
                        btnDienThoai.Visible = true;
                        btnHangCungCap.Visible = true;
                        btnKhachHang.Visible = true;
                        btnHoaDon.Visible = true;
                        btnPhieuNhap.Visible = true;
                        btnBaoCao.Visible = true;
                        break;

                    case "BÁN HÀNG":
                        // Thu ngân: Chỉ giao tiếp khách, tra cứu máy và lập phiếu tính tiền
                        btnDienThoai.Visible = true;
                        btnKhachHang.Visible = true;
                        btnLapHoaDon.Visible = true;
                        btnHoaDon.Visible = true;
                        break;

                    case "KỸ THUẬT":
                        btnDienThoai.Visible = true;
                        btnKhachHang.Visible = true;
                        btnBaoTri.Visible = true;
                        break;

                    default:
                        MessageBox.Show("Tài khoản của bạn chưa được cấp quyền truy cập tính năng!", "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load giao diện: " + ex.Message);
            }
        }

        private void BtnDienThoai_Click(object sender, EventArgs e)
        {
            // 1. Dọn dẹp Panel chứa
            pnlContent.Controls.Clear();

            // 2. Khởi tạo màn hình Điện thoại
            ucDienThoai ucDT = new ucDienThoai();

            // 3. 🔴 TUYỆT ĐỐI KHÔNG DÙNG DOCK = FILL! THAY BẰNG DOCK = TOP
            ucDT.Dock = DockStyle.Top;

            // 4. 🔴 BẬT ÉP THANH CUỘN CHO PANEL TẠI ĐÂY
            pnlContent.AutoScroll = true;

            // 5. Thêm vào Panel
            pnlContent.Controls.Add(ucDT);
            ucDT.BringToFront();
        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {

                Session.MaNV = "";
                Session.HoTen = "";
                Session.ChucVu = "";

                Application.Restart();
                Form1 frmLogin = new Form1();
                frmLogin.Show();
            }
        }

        private void btnHangCungCap_Click(object sender, EventArgs e)
        {
            ucHangCungCap hcc = new ucHangCungCap();
            hcc.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(hcc);
            hcc.BringToFront();
        }

        private void btnKhachHang_Click(object sender, EventArgs e)
        {
            ucKhachHang khachHang = new ucKhachHang();
            khachHang.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(khachHang);
            khachHang.BringToFront();
        }

        private void btnHoaDon_Click(object sender, EventArgs e)
        {
            ucHoaDon hcc = new ucHoaDon();
            hcc.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(hcc);
            hcc.BringToFront();
        }

        private void btnPhieuNhap_Click(object sender, EventArgs e)
        {
            ucPhieuNhap pn = new ucPhieuNhap();
            pn.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(pn);
            pn.BringToFront();
        }

        private void btnBaoTri_Click(object sender, EventArgs e)
        {
            ucBaoTri bt = new ucBaoTri();
            bt.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(bt);
            bt.BringToFront();
        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            ucBaoCao bc = new ucBaoCao();
            bc.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(bc);
            bc.BringToFront(); ;
        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            ucNhanVien nv = new ucNhanVien();
            nv.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(nv);
            nv.BringToFront();
        }

        private void pnlSidebar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblAppName_Click(object sender, EventArgs e)
        {

        }

        private void guna2Separator1_Click(object sender, EventArgs e)
        {

        }

        private void btnLapHoaDon_Click(object sender, EventArgs e)
        {
            ucLapHoaDon lhd = new ucLapHoaDon();
            lhd.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(lhd);
            lhd.BringToFront();
        }
    }
}