using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=QuanLyDienThoai;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // 1. Hàm ExecuteQuery chuẩn (Đã loại bỏ trùng lặp và bọc bảo vệ)
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Lỗi kết nối hoặc truy vấn SQL:\n{ex.Message}", "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi ứng dụng hệ thống:\n{ex.Message}", "Lỗi App", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        // 2. Hàm thực thi Stored Procedure (Thêm/Sửa/Xóa)
        public static int ExecuteStoredProcedure(string procName, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(procName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null) cmd.Parameters.AddRange(parameters);

                        SqlParameter returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return (int)returnParameter.Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    MessageBox.Show("Lỗi vi phạm ràng buộc dữ liệu!\n(Gợi ý: Bạn phải chọn hoặc Lưu dòng máy trước thì mới thêm được phiên bản)", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Mã này đã tồn tại trong hệ thống, không được nhập trùng!", "Trùng dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Lỗi thực thi lệnh SQL:\n{ex.Message}", "Lỗi SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return -99;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống ứng dụng:\n{ex.Message}", "Lỗi phần mềm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -99;
            }
        }

        // 3. Hàm ExecuteNonQuery dùng cho các câu lệnh SQL trực tiếp (INSERT, UPDATE, DELETE dạng chuỗi)
        public static int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                using (var conn = GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thực thi SQL NonQuery:\n{ex.Message}", "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        // 4. Hàm ExecuteScalar lấy 1 giá trị đơn lẻ
        public static object ExecuteScalar(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                using (var conn = GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi truy vấn scalar:\n{ex.Message}", "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static string GenerateCode(string prefix, string tableName, string idColumn)
        {
            string newCode = prefix + "001"; // Mặc định nếu bảng chưa có dữ liệu nào
            try
            {
                // Câu lệnh này TÌM CÁI MÃ LỚN NHẤT HIỆN TẠI (Ví dụ: HD051)
                // Dùng ORDER BY DESC để lấy thằng to nhất đẩy lên đầu
                string sql = $"SELECT TOP 1 {idColumn} FROM {tableName} ORDER BY LEN({idColumn}) DESC, {idColumn} DESC";

                DataTable dt = ExecuteQuery(sql); // Giả sử bác có hàm ExecuteQuery trả về DataTable

                if (dt != null && dt.Rows.Count > 0)
                {
                    string lastCode = dt.Rows[0][0].ToString(); // Lấy được chữ "HD051"

                    // Tách lấy phần số (051)
                    string numberPart = lastCode.Substring(prefix.Length);

                    // Ép sang số và cộng 1 (Thành 52)
                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        lastNumber++;

                        // Ghép lại thành HD052 (Giữ nguyên định dạng 3 số 0 ở đầu nếu có)
                        newCode = prefix + lastNumber.ToString(new string('0', numberPart.Length));
                    }
                }
            }
            catch
            {
                // Nếu có lỗi mạng/SQL thì cứ sinh mã kèm thời gian thực để chống trùng tuyệt đối
                newCode = prefix + DateTime.Now.ToString("HHmmss");
            }

            return newCode;
        }
    }
}