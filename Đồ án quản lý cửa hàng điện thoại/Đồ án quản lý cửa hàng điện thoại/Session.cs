using System;
using System.Collections.Generic;
using System.Text;

namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại.Helpers
{
   
    public static class Session
    {
        public static string MaNV { get; set; }
        public static string HoTen { get; set; }
        public static string ChucVu { get; set; }  

      
        public static bool IsAdmin => ChucVu == "QuanLy";
        public static bool IsBanHang => ChucVu == "BanHang";
        public static bool IsKyThuat => ChucVu == "KyThuat";
        public static bool IsKeToan => ChucVu == "KeToan";
        public static void Clear()
        {
            MaNV = null;
            HoTen = null;
            ChucVu = null;
        }

        
    }
}
