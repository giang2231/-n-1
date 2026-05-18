using Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers;
using System.Data;
using Microsoft.Data.SqlClient;
namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = @"SELECT MaNV, HoTen, ChucVu 
                           FROM NhanVien 
                           WHERE TenDangNhap = @user AND MatKhau = @pass AND TrangThai = 1";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@pass", pass)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                Session.MaNV = dt.Rows[0]["MaNV"].ToString();
                Session.HoTen = dt.Rows[0]["HoTen"].ToString();
                Session.ChucVu = dt.Rows[0]["ChucVu"].ToString();

                this.Hide();

                Trang_Chủcs dashboard = new Trang_Chủcs();
                dashboard.Show();

            }
            else
            {
                // Đăng nhập thất bại
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!\nHoặc tài khoản đã bị khóa.",
                    "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtPass.Clear();
                txtPass.Focus();
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void chkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.PasswordChar = chkShowPass.Checked ? '\0' : '●';
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
