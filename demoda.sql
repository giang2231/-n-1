 
USE master;
GO
 
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyDienThoai')
    DROP DATABASE QuanLyDienThoai;
GO
 
CREATE DATABASE QuanLyDienThoai;
GO
 
USE QuanLyDienThoai;
GO
 
CREATE TABLE HangCungCap (
    MaHang      NVARCHAR(20)  PRIMARY KEY,
    TenHang     NVARCHAR(100) NOT NULL,
    QuocGia     NVARCHAR(50),
    DiaChi      NVARCHAR(200),
    SoDienThoai NVARCHAR(20),
    Email       NVARCHAR(100),
    TrangThai   BIT NOT NULL DEFAULT 1  
);
GO
 
CREATE TABLE DienThoai (
    MaMay       NVARCHAR(20)  PRIMARY KEY,
    TenMay      NVARCHAR(150) NOT NULL,
    MaHang      NVARCHAR(20)  REFERENCES HangCungCap(MaHang),
    CPU         NVARCHAR(100),
    RAM         NVARCHAR(50),
    ROM         NVARCHAR(50),
    ManHinh     NVARCHAR(50),
    Camera      NVARCHAR(100),
    Pin         NVARCHAR(50),
    HeDieuHanh  NVARCHAR(50),
    MoTa        NTEXT,
    GiaNhap     DECIMAL(18,0) NOT NULL DEFAULT 0,
    GiaBan      DECIMAL(18,0) NOT NULL DEFAULT 0,
    HinhAnh     NVARCHAR(500),
    TrangThai   BIT           NOT NULL DEFAULT 1  
);
GO

CREATE TABLE ChiTietDienThoai (
    MaChiTiet NVARCHAR(50) PRIMARY KEY,  
    MaMay NVARCHAR(20) NOT NULL FOREIGN KEY REFERENCES DienThoai(MaMay),
    DungLuong NVARCHAR(20) NOT NULL,     
    MauSac NVARCHAR(50) NOT NULL,        
    GiaNhap DECIMAL(18,0) NOT NULL,    
    GiaBan DECIMAL(18,0) NOT NULL,       
    TrangThai BIT DEFAULT 1
);
GO
 

CREATE TABLE NhanVien (
    MaNV        NVARCHAR(20)  PRIMARY KEY,
    HoTen       NVARCHAR(100) NOT NULL,
    GioiTinh    NVARCHAR(10),
    NgaySinh    DATE,
    CMND        NVARCHAR(20),
    SoDienThoai NVARCHAR(20),
    Email       NVARCHAR(100),
    DiaChi      NVARCHAR(200),
    ChucVu      NVARCHAR(50)  NOT NULL,  
    LuongCoBan  DECIMAL(18,0) DEFAULT 0,
    TenDangNhap NVARCHAR(50)  NOT NULL UNIQUE,
    MatKhau     NVARCHAR(100) NOT NULL,
    TrangThai   BIT           NOT NULL DEFAULT 1  
);
GO

CREATE TABLE KhachHang (
    MaKH        NVARCHAR(20)  PRIMARY KEY,
    HoTen       NVARCHAR(100) NOT NULL,
    GioiTinh    NVARCHAR(10),
    NgaySinh    DATE,
    SoDienThoai NVARCHAR(20)  NOT NULL UNIQUE,
    DiaChi      NVARCHAR(200),
    LoaiKhach   NVARCHAR(20)  DEFAULT N'Thường',  
    TongChiTieu DECIMAL(18,0) DEFAULT 0,
    NgayTao     DATE          DEFAULT GETDATE()
);
GO
 
CREATE TABLE HoaDon (
    MaHD        NVARCHAR(20)  PRIMARY KEY,
    MaKH        NVARCHAR(20)  REFERENCES KhachHang(MaKH),
    MaNV        NVARCHAR(20)  REFERENCES NhanVien(MaNV),
    NgayLap     DATETIME      NOT NULL DEFAULT GETDATE(),
    TongTien    DECIMAL(18,0) NOT NULL DEFAULT 0,
    GiamGia     DECIMAL(18,0) DEFAULT 0,
    ThanhTien   DECIMAL(18,0) NOT NULL DEFAULT 0,
    TrangThai   NVARCHAR(20)  DEFAULT N'Đã thanh toán',  -- Đã thanh toán / Đã hủy
    GhiChu      NVARCHAR(500)
);
GO
 

CREATE TABLE ChiTietHoaDon (
    MaChiTiet   INT           IDENTITY(1,1) PRIMARY KEY,
    MaHD        NVARCHAR(20)  NOT NULL REFERENCES HoaDon(MaHD),
    MaMay       NVARCHAR(20)  NOT NULL REFERENCES DienThoai(MaMay),
    SoIMEI      NVARCHAR(50)  NOT NULL,
    DonGia      DECIMAL(18,0) NOT NULL,
    SoLuong     INT           NOT NULL DEFAULT 1
);
GO
 

CREATE TABLE PhieuNhap (
    MaPhieu     NVARCHAR(20)  PRIMARY KEY,
    MaHang      NVARCHAR(20)  REFERENCES HangCungCap(MaHang),
    MaNV        NVARCHAR(20)  REFERENCES NhanVien(MaNV),
    NgayNhap    DATETIME      NOT NULL DEFAULT GETDATE(),
    TongTien    DECIMAL(18,0) NOT NULL DEFAULT 0,
    TrangThai   NVARCHAR(20)  DEFAULT N'Đã nhập',  
    GhiChu      NVARCHAR(500)
);
GO
 
CREATE TABLE ChiTietPhieuNhap (
    MaChiTiet   INT           IDENTITY(1,1) PRIMARY KEY,
    MaPhieu     NVARCHAR(20)  NOT NULL REFERENCES PhieuNhap(MaPhieu),
    MaMay       NVARCHAR(20)  NOT NULL REFERENCES DienThoai(MaMay),
    SoIMEI      NVARCHAR(50)  NOT NULL UNIQUE,
    DonGiaNhap  DECIMAL(18,0) NOT NULL,
    SoLuong     INT           NOT NULL DEFAULT 1,
    TrangThaiIMEI NVARCHAR(20) DEFAULT N'Trong kho'  
);
GO
 
CREATE TABLE PhieuBaoTri (
    MaPhieuBT   NVARCHAR(20)  PRIMARY KEY,
    MaKH        NVARCHAR(20)  REFERENCES KhachHang(MaKH),
    MaNV        NVARCHAR(20)  REFERENCES NhanVien(MaNV),
    MaMay       NVARCHAR(20)  REFERENCES DienThoai(MaMay),
    SoIMEI      NVARCHAR(50),
    NgayTiepNhan DATETIME     NOT NULL DEFAULT GETDATE(),
    NgayHenTra  DATE,
    NgayTraThuc DATE,
    TinhTrangNhan NTEXT,
    NoiDungSua  NTEXT,
    ChiPhiSua   DECIMAL(18,0) DEFAULT 0,
    TrangThai   NVARCHAR(30)  DEFAULT N'Đang kiểm tra',
 
    GhiChu      NVARCHAR(500)
);
GO
 

USE QuanLyDienThoai;
GO

DELETE FROM ChiTietHoaDon;
DELETE FROM HoaDon;
DELETE FROM ChiTietPhieuNhap;
DELETE FROM PhieuNhap;
DELETE FROM PhieuBaoTri;
DELETE FROM ChiTietDienThoai;
DELETE FROM DienThoai;

DELETE FROM HangCungCap;
GO

INSERT INTO HangCungCap (MaHang, TenHang, QuocGia, DiaChi, SoDienThoai, Email, TrangThai) VALUES
('NCC01', N'Apple', N'Mỹ', N'One Apple Park Way, Cupertino, California', '18001192', 'contact@apple.com', 1),
('NCC02', N'Samsung', N'Hàn Quốc', N'Suwon, Gyeonggi-do, South Korea', '1800588889', 'support@samsung.com', 1),
('NCC03', N'Xiaomi', N'Trung Quốc', N'Haidian District, Beijing, China', '1900561558', 'service.global@xiaomi.com', 1),
('NCC04', N'Oppo', N'Trung Quốc', N'Dongguan, Guangdong, China', '1800577776', 'support@oppo.com', 1),
('NCC05', N'Vivo', N'Trung Quốc', N'Dongguan, Guangdong, China', '18006101', 'service@vivo.com', 1),
('NCC06', N'Realme', N'Trung Quốc', N'Shenzhen, Guangdong, China', '18006067', 'service@realme.com', 1),
('NCC07', N'Nokia', N'Phần Lan', N'Karakaari 7, Espoo, Finland', '18001516', 'support@nokia.com', 1),
('NCC08', N'Sony', N'Nhật Bản', N'Minato, Tokyo, Japan', '1800588885', 'info@sony.com.vn', 1),
('NCC09', N'Asus', N'Đài Loan', N'Beitou District, Taipei, Taiwan', '18006588', 'support@asus.com', 1),
('NCC10', N'Google', N'Mỹ', N'1600 Amphitheatre Pkwy, Mountain View, California', '18001234', 'hardware-support@google.com', 1),
('NCC11', N'OnePlus', N'Trung Quốc', N'Shenzhen, Guangdong, China', '18005555', 'support@oneplus.com', 1),
('NCC12', N'Huawei', N'Trung Quốc', N'Longgang District, Shenzhen, China', '18001085', 'huawei.support@huawei.com', 1),
('NCC13', N'Motorola', N'Mỹ', N'Merchandise Mart, Chicago, Illinois', '18002222', 'support@motorola.com', 1),
('NCC14', N'HTC', N'Đài Loan', N'Taoyuan District, Taoyuan, Taiwan', '18003344', 'contact@htc.com', 1),
('NCC15', N'LG', N'Hàn Quốc', N'Yeouido-dong, Yeongdeungpo-gu, Seoul', '18001503', 'support@lg.com', 1),
('NCC16', N'ZTE', N'Trung Quốc', N'Nanshan District, Shenzhen, China', '18004444', 'support@zte.com.cn', 1),
('NCC17', N'Meizu', N'Trung Quốc', N'Zhuhai, Guangdong, China', '18007788', 'service@meizu.com', 1),
('NCC18', N'Tecno', N'Trung Quốc', N'Futian District, Shenzhen, China', '18009999', 'service@tecno.com', 1),
('NCC19', N'Infinix', N'Trung Quốc', N'Futian District, Shenzhen, China', '18008888', 'support@infinix.com', 1),
('NCC20', N'Itel', N'Trung Quốc', N'Nanshan District, Shenzhen, China', '18006666', 'service@itel.com', 1);
GO


INSERT INTO DienThoai (MaMay, TenMay, MaHang, CPU, RAM, ROM, ManHinh, Camera, Pin, HeDieuHanh, MoTa, TrangThai, DanhMuc) VALUES
('MM01', N'iPhone 15 Pro Max', 'NCC01', N'Apple A17 Pro', N'8GB', N'256GB', N'6.7 inch', N'48MP', N'4422 mAh', N'iOS 17', N'Siêu phẩm vỏ Titan đỉnh cao 2023', 1, N'Flagship'),
('MM02', N'iPhone 15 Pro', 'NCC01', N'Apple A17 Pro', N'8GB', N'128GB', N'6.1 inch', N'48MP', N'3274 mAh', N'iOS 17', N'Cấu hình Pro trong thân máy nhỏ gọn', 1, N'Flagship'),
('MM03', N'iPhone 15 Plus', 'NCC01', N'Apple A16 Bionic', N'6GB', N'128GB', N'6.7 inch', N'48MP', N'4383 mAh', N'iOS 17', N'Màn hình lớn pin siêu trâu phân khúc', 1, N'Tầm trung'),
('MM04', N'iPhone 15', 'NCC01', N'Apple A16 Bionic', N'6GB', N'128GB', N'6.1 inch', N'48MP', N'3349 mAh', N'iOS 17', N'Phiên bản tiêu chuẩn màu sắc trẻ trung', 1, N'Tầm trung'),
('MM05', N'iPhone 14 Pro Max', 'NCC01', N'Apple A16 Bionic', N'6GB', N'128GB', N'6.7 inch', N'48MP', N'4323 mAh', N'iOS 16', N'Huyền thoại màn hình Dynamic Island đầu tiên', 1, N'Cận cao cấp'),
('MM06', N'iPhone 14 Pro', 'NCC01', N'Apple A16 Bionic', N'6GB', N'128GB', N'6.1 inch', N'48MP', N'3200 mAh', N'iOS 16', N'Flagship nhỏ gọn mượt mà', 1, N'Cận cao cấp'),
('MM07', N'iPhone 14 Plus', 'NCC01', N'Apple A15 Bionic', N'6GB', N'128GB', N'6.7 inch', N'12MP', N'4325 mAh', N'iOS 16', N'Dòng Plus pin tốt màn hình lớn', 1, N'Tầm trung'),
('MM08', N'iPhone 14', 'NCC01', N'Apple A15 Bionic', N'6GB', N'128GB', N'6.1 inch', N'12MP', N'3279 mAh', N'iOS 16', N'Máy phụ hoàn hảo mượt mà ổn định', 1, N'Tầm trung'),
('MM09', N'iPhone 13 Pro Max', 'NCC01', N'Apple A15 Bionic', N'6GB', N'128GB', N'6.7 inch', N'12MP', N'4352 mAh', N'iOS 15', N'Màn hình 120Hz pin trâu nhất lịch sử', 1, N'Cận cao cấp'),
('MM10', N'iPhone 13', 'NCC01', N'Apple A15 Bionic', N'4GB', N'128GB', N'6.1 inch', N'12MP', N'3240 mAh', N'iOS 15', N'Điện thoại iPhone quốc dân phân khúc cũ', 1, N'Tầm trung'),
('MM11', N'Galaxy S24 Ultra', 'NCC02', N'Snapdragon 8 Gen 3', N'12GB', N'256GB', N'6.8 inch', N'200MP', N'5000 mAh', N'Android 14', N'Flagship tích hợp quyền năng Galaxy AI', 1, N'Flagship'),
('MM12', N'Galaxy S24+', 'NCC02', N'Exynos 2400', N'12GB', N'256GB', N'6.7 inch', N'50MP', N'4900 mAh', N'Android 14', N'Màn hình 2K siêu nét tích hợp AI', 1, N'Flagship'),
('MM13', N'Galaxy S24', 'NCC02', N'Exynos 2400', N'8GB', N'128GB', N'6.2 inch', N'50MP', N'4000 mAh', N'Android 14', N'Nhỏ gọn cao cấp đầy đủ tính năng AI', 1, N'Tầm trung'),
('MM14', N'Galaxy Z Fold 5', 'NCC02', N'Snapdragon 8 Gen 2', N'12GB', N'256GB', N'7.6 inch', N'50MP', N'4400 mAh', N'Android 13', N'Điện thoại gập ngang đa nhiệm đỉnh cao', 1, N'Flagship'),
('MM15', N'Galaxy Z Flip 5', 'NCC02', N'Snapdragon 8 Gen 2', N'8GB', N'256GB', N'6.7 inch', N'12MP', N'3700 mAh', N'Android 13', N'Màn hình gập hộp phấn thời trang cá tính', 1, N'Cận cao cấp'),
('MM16', N'Galaxy S23 Ultra', 'NCC02', N'Snapdragon 8 Gen 2', N'8GB', N'256GB', N'6.8 inch', N'200MP', N'5000 mAh', N'Android 13', N'Quái vật camera zoom 100x kèm bút S-Pen', 1, N'Cận cao cấp'),
('MM17', N'Galaxy S23 FE', 'NCC02', N'Exynos 2200', N'8GB', N'128GB', N'6.4 inch', N'50MP', N'4500 mAh', N'Android 13', N'Phiên bản cấu hình cao giá tốt cho Samfan', 1, N'Tầm trung'),
('MM18', N'Galaxy A55 5G', 'NCC02', N'Exynos 1480', N'8GB', N'128GB', N'6.6 inch', N'50MP', N'5000 mAh', N'Android 14', N'Tầm trung quốc dân khung nhôm kính cao cấp', 1, N'Tầm trung'),
('MM19', N'Galaxy A35 5G', 'NCC02', N'Exynos 1380', N'8GB', N'128GB', N'6.6 inch', N'50MP', N'5000 mAh', N'Android 14', N'Điện thoại thiết kế hiện đại giá sinh viên', 1, N'Giá rẻ'),
('MM20', N'Galaxy M54 5G', 'NCC02', N'Exynos 1380', N'8GB', N'256GB', N'6.7 inch', N'108MP', N'6000 mAh', N'Android 13', N'Dòng M pin khủng long dùng 2 ngày', 1, N'Tầm trung'),
('MM21', N'Xiaomi 14 Ultra', 'NCC03', N'Snapdragon 8 Gen 3', N'16GB', N'512GB', N'6.73 inch', N'50MP Leica', N'5000 mAh', N'HyperOS', N'Đỉnh cao nhiếp ảnh ống kính vương miện Leica', 1, N'Flagship'),
('MM22', N'Xiaomi 14', 'NCC03', N'Snapdragon 8 Gen 3', N'12GB', N'256GB', N'6.36 inch', N'50MP Leica', N'4610 mAh', N'HyperOS', N'Flagship nhỏ gọn cấu hình mạnh nhất phân khúc', 1, N'Flagship'),
('MM23', N'Xiaomi 13T Pro', 'NCC03', N'Dimensity 9200+', N'12GB', N'256GB', N'6.67 inch', N'50MP Leica', N'5000 mAh', N'MIUI 14', N'Cấu hình quái vật sạc siêu nhanh 120W', 1, N'Cận cao cấp'),
('MM24', N'Xiaomi 13T', 'NCC03', N'Dimensity 8200 Ultra', N'8GB', N'256GB', N'6.67 inch', N'50MP Leica', N'5000 mAh', N'MIUI 14', N'Trải nghiệm camera Leica cao cấp giá mềm', 1, N'Tầm trung'),
('MM25', N'Redmi Note 13 Pro+', 'NCC03', N'Dimensity 7200 Ultra', N'8GB', N'256GB', N'6.67 inch', N'200MP', N'5000 mAh', N'MIUI 14', N'Màn hình cong tràn cạnh chống nước IP68', 1, N'Tầm trung'),
('MM26', N'Redmi Note 13 Pro', 'NCC03', N'Helio G99 Ultra', N'8GB', N'128GB', N'6.67 inch', N'200MP', N'5000 mAh', N'MIUI 14', N'Camera siêu nét viền siêu mỏng', 1, N'Tầm trung'),
('MM27', N'Redmi Note 13', 'NCC03', N'Snapdragon 685', N'6GB', N'128GB', N'6.67 inch', N'108MP', N'5000 mAh', N'MIUI 14', N'Vua phân khúc giá rẻ màn hình 120Hz', 1, N'Giá rẻ'),
('MM28', N'POCO X6 Pro', 'NCC03', N'Dimensity 8300 Ultra', N'8GB', N'256GB', N'6.67 inch', N'64MP', N'5000 mAh', N'HyperOS', N'Hủy diệt hiệu năng chuyên cày game nặng', 1, N'Gaming Phone'),
('MM29', N'POCO F5 Pro', 'NCC03', N'Snapdragon 8+ Gen 1', N'12GB', N'256GB', N'6.67 inch', N'64MP', N'5160 mAh', N'MIUI 14', N'Màn hình 2K siêu mượt giá hời', 1, N'Cận cao cấp'),
('MM30', N'Redmi 13C', 'NCC03', N'Dimensity 6100+', N'4GB', N'128GB', N'6.74 inch', N'50MP', N'5000 mAh', N'MIUI 14', N'Máy phụ pin trâu màn hình lớn giá hời', 1, N'Giá rẻ'),
('MM31', N'Oppo Find X7 Ultra', 'NCC04', N'Snapdragon 8 Gen 3', N'16GB', N'256GB', N'6.82 inch', N'50MP Hasselblad', N'5000 mAh', N'ColorOS 14', N'Camera Hasselblad zoom tàu ngầm kép', 1, N'Flagship'),
('MM32', N'Oppo Find N3', 'NCC04', N'Snapdragon 8 Gen 2', N'12GB', N'512GB', N'7.82 inch', N'48MP', N'4805 mAh', N'ColorOS 13', N'Màn hình gập mỏng nhất phân cấp', 1, N'Flagship'),
('MM33', N'Oppo Find N3 Flip', 'NCC04', N'Dimensity 9200', N'12GB', N'256GB', N'6.8 inch', N'50MP', N'4300 mAh', N'ColorOS 13', N'Màn hình gập dọc 3 camera đỉnh cao', 1, N'Cận cao cấp'),
('MM34', N'Oppo Reno 11 Pro', 'NCC04', N'Dimensity 8200', N'12GB', N'512GB', N'6.7 inch', N'50MP', N'4600 mAh', N'ColorOS 14', N'Chuyên gia nhiếp ảnh chân dung thế hệ mới', 1, N'Cận cao cấp'),
('MM35', N'Oppo Reno 11', 'NCC04', N'Dimensity 7050', N'8GB', N'256GB', N'6.7 inch', N'50MP', N'5000 mAh', N'ColorOS 14', N'Thiết kế mặt lưng sóng biển thời trang', 1, N'Tầm trung'),
('MM36', N'Oppo A79 5G', 'NCC04', N'Dimensity 6020', N'8GB', N'256GB', N'6.72 inch', N'50MP', N'5000 mAh', N'ColorOS 13', N'Âm thanh loa kép kết nối 5G tốc độ', 1, N'Giá rẻ'),
('MM37', N'Vivo X100 Pro', 'NCC05', N'Dimensity 9300', N'16GB', N'512GB', N'6.78 inch', N'50MP Zeiss', N'5400 mAh', N'Funtouch 14', N'Ống kính nhiếp ảnh cao cấp hợp tác Zeiss', 1, N'Flagship'),
('MM38', N'Vivo X100', 'NCC05', N'Dimensity 9300', N'12GB', N'256GB', N'6.78 inch', N'50MP Zeiss', N'5000 mAh', N'Funtouch 14', N'Cấu hình quái vật camera thấu kính Zeiss', 1, N'Flagship'),
('MM39', N'Vivo V30 Pro', 'NCC05', N'Dimensity 8200', N'12GB', N'512GB', N'6.78 inch', N'50MP Aura', N'5000 mAh', N'Funtouch 14', N'Đèn Aura Light thế hệ mới chụp đêm đỉnh cao', 1, N'Cận cao cấp'),
('MM40', N'Vivo V30', 'NCC05', N'Snapdragon 7 Gen 3', N'8GB', N'256GB', N'6.78 inch', N'50MP Aura', N'5000 mAh', N'Funtouch 14', N'Thân máy siêu mỏng màn hình cong cuốn hút', 1, N'Tầm trung'),
('MM41', N'Vivo Y100', 'NCC05', N'Snapdragon 685', N'8GB', N'256GB', N'6.67 inch', N'50MP', N'5000 mAh', N'Funtouch 14', N'Sạc siêu tốc 80W mặt lưng đổi màu', 1, N'Giá rẻ'),
('MM42', N'Sony Xperia 1 V', 'NCC08', N'Snapdragon 8 Gen 2', N'12GB', N'256GB', N'6.5 inch', N'48MP', N'5000 mAh', N'Android 13', N'Màn hình 4K tỷ lệ điện ảnh 21:9', 1, N'Flagship'),
('MM43', N'Sony Xperia 5 V', 'NCC08', N'Snapdragon 8 Gen 2', N'8GB', N'128GB', N'6.1 inch', N'48MP', N'5000 mAh', N'Android 13', N'Thiết kế compact đẳng cấp âm thanh Sony', 1, N'Cận cao cấp'),
('MM44', N'Asus ROG Phone 8 Pro', 'NCC09', N'Snapdragon 8 Gen 3', N'16GB', N'512GB', N'6.78 inch', N'50MP', N'5500 mAh', N'Android 14', N'Vua gaming phone màn hình 165Hz tản nhiệt khủng', 1, N'Gaming Phone'),
('MM45', N'Asus Zenfone 11 Ultra', 'NCC09', N'Snapdragon 8 Gen 3', N'12GB', N'256GB', N'6.78 inch', N'50MP', N'5500 mAh', N'Android 14', N'Flagship màn hình lớn pin cực khủng', 1, N'Flagship'),
('MM46', N'Google Pixel 8 Pro', 'NCC10', N'Google Tensor G3', N'12GB', N'128GB', N'6.7 inch', N'50MP', N'5050 mAh', N'Android 14', N'Điện thoại AI thông minh nhất từ Google', 1, N'Flagship'),
('MM47', N'Google Pixel 8', 'NCC10', N'Google Tensor G3', N'8GB', N'128GB', N'6.2 inch', N'50MP', N'4575 mAh', N'Android 14', N'Trải nghiệm mượt mà cập nhật Android đầu tiên', 1, N'Tầm trung'),
('MM48', N'Google Pixel 7a', 'NCC10', N'Google Tensor G2', N'8GB', N'128GB', N'6.1 inch', N'64MP', N'4385 mAh', N'Android 13', N'Chụp ảnh đỉnh cao thuật toán Google giá hời', 1, N'Giá rẻ'),
('MM49', N'OnePlus 12', 'NCC11', N'Snapdragon 8 Gen 3', N'16GB', N'512GB', N'6.82 inch', N'50MP', N'5400 mAh', N'OxygenOS 14', N'Flagship Killer cấu hình đè bẹp giá tiền', 1, N'Flagship'),
('MM50', N'OnePlus 12R', 'NCC11', N'Snapdragon 8 Gen 2', N'8GB', N'128GB', N'6.78 inch', N'50MP', N'5500 mAh', N'OxygenOS 14', N'Hiệu năng cực mạnh giá sinh viên chuyên game', 1, N'Tầm trung');
INSERT INTO ChiTietDienThoai (MaChiTiet, MaMay, DungLuong, MauSac, GiaNhap, GiaBan, TrangThai) VALUES
('CT01', 'MM01', '256GB', N'Titan Sa Mạc', 27500000, 31990000, 1),
('CT02', 'MM01', '512GB', N'Titan Tự Nhiên', 32000000, 36990000, 1),
('CT03', 'MM01', '1TB', N'Titan Đen Nhám', 37500000, 42990000, 1),
('CT04', 'MM02', '128GB', N'Titan Trắng', 23000000, 26490000, 1),
('CT05', 'MM02', '256GB', N'Titan Sa Mạc', 25500000, 29190000, 1),
('CT06', 'MM03', '128GB', N'Xanh Dương Pastel', 20000000, 23490000, 1),
('CT07', 'MM03', '256GB', N'Hồng Trendy', 22500000, 25990000, 1),
('CT08', 'MM04', '128GB', N'Xanh Lá Mạ', 17500000, 19990000, 1),
('CT09', 'MM04', '256GB', N'Đen Tuyền', 20000000, 22990000, 1),
('CT10', 'MM05', '128GB', N'Tím Đậm (Deep Purple)', 22000000, 24990000, 1),
('CT11', 'MM05', '256GB', N'Vàng Gold', 24500000, 27490000, 1),
('CT12', 'MM11', '256GB', N'Xám Titan', 25000000, 28990000, 1),
('CT13', 'MM11', '512GB', N'Tím Titan', 28000000, 32490000, 1),
('CT14', 'MM11', '1TB', N'Đen Vũ Trụ', 33500000, 37990000, 1),
('CT15', 'MM12', '256GB', N'Vàng Amber', 20000000, 22990000, 1),
('CT16', 'MM12', '512GB', N'Cobalt Tím', 22500000, 25990000, 1),
('CT17', 'MM14', '256GB', N'Xanh Icy', 31000000, 34990000, 1),
('CT18', 'MM14', '512GB', N'Đen Phantom', 33500000, 37990000, 1),
('CT19', 'MM15', '256GB', N'Xanh Mint', 16500000, 18990000, 1),
('CT20', 'MM15', '512GB', N'Tím Combo', 18500000, 21490000, 1),
('CT21', 'MM18', '128GB', N'Xanh Đen Navy', 8000000, 9690000, 1),
('CT22', 'MM18', '256GB', N'Tím Lilac', 9000000, 10690000, 1),
('CT23', 'MM21', '512GB', N'Trắng Gốm Tinh Khiết', 24500000, 28990000, 1),
('CT24', 'MM21', '512GB', N'Đen Da Vân Cổ Điển', 24500000, 28990000, 1),
('CT25', 'MM22', '256GB', N'Xanh Cẩm Thạch', 17000000, 19990000, 1),
('CT26', 'MM22', '512GB', N'Đen Nhám', 19000000, 22490000, 1),
('CT27', 'MM28', '256GB', N'Vàng Đậm Gaming', 7400000, 8990000, 1),
('CT28', 'MM28', '512GB', N'Xám Xi Măng', 8500000, 9990000, 1),
('CT29', 'MM31', '256GB', N'Nâu Da Bò Sang Trọng', 21000000, 24990000, 1),
('CT30', 'MM31', '512GB', N'Xanh Đại Dương', 23500000, 27490000, 1),
('CT31', 'MM34', '512GB', N'Trắng Ngọc Trai', 13000000, 15490000, 1),
('CT32', 'MM37', '512GB', N'Xanh Tinh Vân', 20500000, 23990000, 1),
('CT33', 'MM37', '512GB', N'Cam Hoàng Hôn', 20500000, 23990000, 1),
('CT34', 'MM44', '512GB', N'Đen Phantom Led', 25500000, 29990000, 1),
('CT35', 'MM46', '128GB', N'Xanh Bay (Sky Blue)', 19000000, 21990000, 1),
('CT36', 'MM46', '256GB', N'Trắng Sứ Porcelain', 21000000, 23990000, 1);
GO
INSERT INTO NhanVien (MaNV, HoTen, GioiTinh, NgaySinh, CMND, SoDienThoai, ChucVu, TenDangNhap, MatKhau, TrangThai) VALUES
('NV001', N'Vũ Trường Giang', N'Nam', '2002-05-15', '034202001234', '0987654321', N'Quản Lý', 'admin', '123456', 1),
('NV002', N'Nguyễn Thu Ngân', N'Nữ', '2003-10-20', '034203005678', '0912345678', N'Bán Hàng', 'thungan', '123456', 1),
('NV003', N'Trần Kỹ Thuật', N'Nam', '1998-02-28', '034198009999', '0909998887', N'Kỹ Thuật', 'kythuat', '123456', 1),
('NV004', N'Lê Thị Kế Toán', N'Nữ', '1995-08-20', '034195007788', '0988111222', N'Kế toán', 'ketoan', '123456', 1);
INSERT INTO KhachHang (MaKH, HoTen, GioiTinh, SoDienThoai, LoaiKhach) VALUES
('KH000', N'Khách vãng lai', N'Khác', '0000000000', N'Thường'),
('KH001', N'Phạm Nhật Vượng', N'Nam', '0901234567', N'VIP'),
('KH002', N'Mai Kiều Liên', N'Nữ', '0988777666', N'VIP'),
('KH003', N'Trần Đình Long', N'Nam', '0933222111', N'Thường');
GO
INSERT INTO PhieuNhap (MaPhieu, MaHang, MaNV, NgayNhap, TongTien, TrangThai) VALUES
('PN240501', 'NCC01', 'NV001', GETDATE(), 187000000, N'Đã nhập');
INSERT INTO ChiTietPhieuNhap (MaPhieu, MaChiTietSP, SoIMEI, DonGiaNhap, SoLuong, TrangThaiIMEI) VALUES
('PN240501', 'CT01', '358000111222333', 28000000, 1, N'Trong kho'),
('PN240501', 'CT01', '358000111222444', 28000000, 1, N'Trong kho'),
('PN240501', 'CT01', '358000111222555', 28000000, 1, N'Trong kho'),
('PN240501', 'CT02', '358000111222666', 32000000, 1, N'Trong kho'),
('PN240501', 'CT02', '358000111222777', 32000000, 1, N'Trong kho'),
('PN240501', 'CT04', '861000333444555', 25000000, 1, N'Trong kho'),
('PN240501', 'CT04', '861000333444666', 25000000, 1, N'Trong kho');
GO

INSERT INTO PhieuBaoTri (MaPhieuBT, MaKH, MaNV, MaMay, SoIMEI, NgayTiepNhan, NgayHenTra, NgayTraThuc, TinhTrangNhan, NoiDungSua, ChiPhiSua, TrangThai, GhiChu) VALUES
('PBT01', 'KH001', 'NV003', 'MM01', '358000111222333', '2025-01-10', '2025-01-12', '2025-01-12', N'Bể màn hình do rơi rớt', N'Thay cụm màn hình iPhone 15 Pro Max', 4500000, N'Đã trả khách', N'Linh kiện chính hãng bóc máy'),
('PBT02', 'KH002', 'NV003', 'MM11', '861000333444555', '2025-01-15', '2025-01-16', '2025-01-16', N'Mất nguồn hoàn toàn', N'Sửa nguồn ic nguồn trên mainboard', 1800000, N'Đã trả khách', N'Bảo hành ic nguồn 3 tháng'),
('PBT03', 'KH003', 'NV003', 'MM18', '861000333444999', '2025-01-20', '2025-01-21', '2025-01-21', N'Pin chai nặng, sập nguồn liên tục', N'Thay pin dung lượng cao Pisen', 650000, N'Đã trả khách', N'Tặng cường lực cho khách'),
('PBT04', 'KH001', 'NV003', 'MM02', '358000111222888', '2025-02-02', '2025-02-04', '2025-02-04', N'Camera sau bị mờ, không lấy nét được', N'Thay camera sau zin tháo máy', 2200000, N'Đã trả khách', N'Khách kiểm tra hài lòng'),
('PBT05', 'KH002', 'NV003', 'MM21', '861000555666111', '2025-02-10', '2025-02-13', '2025-02-12', N'Rớt nước, mất đèn màn hình', N'Sấy khô, vệ sinh cáp cáp nối màn hình', 400000, N'Đã trả khách', N'Vệ sinh máy miễn phí'),
('PBT06', 'KH003', 'NV003', 'MM05', '358000111222005', '2025-03-01', '2025-03-01', '2025-03-01', N'Hư chân sạc, sạc lúc được lúc không', N'Thay bo đuôi sạc mới', 450000, N'Đã trả khách', N'Linh kiện zin loại 1'),
('PBT07', 'KH001', 'NV003', 'MM12', '861000333444012', '2025-03-05', '2025-03-05', '2025-03-05', N'Liệt phím âm lượng và phím nguồn', N'Thay dây cáp nguồn âm lượng', 350000, N'Đã trả khách', N'Khách quen giảm 10%'),
('PBT08', 'KH002', 'NV003', 'MM22', '861000555666022', '2025-03-12', '2025-03-14', '2025-03-13', N'Kính lưng sau bị vỡ nát', N'Thay kính lưng sau bằng công nghệ Laser', 800000, N'Đã trả khách', N'Khung sườn hơi trầy nhẹ'),
('PBT09', 'KH003', 'NV003', 'MM03', '358000111222003', '2025-03-20', '2025-03-22', '2025-03-22', N'Mất sóng di động, báo không có SIM', N'Sửa ổ SIM và thay IC sóng', 1200000, N'Đã trả khách', N'Bảo hành sóng 6 tháng'),
('PBT10', 'KH001', 'NV003', 'MM14', '861000333444014', '2025-04-02', '2025-04-05', '2025-04-05', N'Màn hình gập bị đốm đen, sọc xanh', N'Thay màn hình trong Galaxy Z Fold 5', 7500000, N'Đã trả khách', N'Ca khó, linh kiện chính hãng hãng cung cấp'),
('PBT11', 'KH002', 'NV003', 'MM25', '861000555666025', '2025-05-10', '2025-05-11', '2025-05-11', N'Hư loa trong, nghe đàm thoại rè', N'Thay loa trong chính hãng', 300000, N'Đã trả khách', N'Đã test âm thanh ok'),
('PBT12', 'KH003', 'NV003', 'MM04', '358000111222004', '2025-05-18', '2025-05-20', '2025-05-19', N'Màn hình xanh, chớp nháy liên tục', N'Fix cổ cáp màn hình bằng máy ép', 1500000, N'Đã trả khách', N'Tiết kiệm chi phí thay màn'),
('PBT13', 'KH001', 'NV003', 'MM15', '861000333444015', '2025-06-01', '2025-06-02', '2025-06-02', N'Cáp gập bị đứt, mất cảm ứng màn phụ', N'Thay dây cáp gập bản lề', 1200000, N'Đã trả khách', N'Gập mở mượt mà'),
('PBT14', 'KH002', 'NV003', 'MM34', '861000777888034', '2025-06-15', '2025-06-16', '2025-06-16', N'Hư micro đàm thoại, nói bên kia không nghe', N'Thay cụm mic dưới', 380000, N'Đã trả khách', N'Bảo hành mic 1 tháng'),
('PBT15', 'KH003', 'NV003', 'MM44', '861000999000044', '2025-07-04', '2025-07-06', '2025-07-06', N'Nóng máy bất thường, sạc không vào điện', N'Thay ic sạc USB-C chân main', 1400000, N'Đã trả khách', N'Đặc chủng máy gaming ROG Phone'),
('PBT16', 'KH001', 'NV003', 'MM06', '358000111222006', '2025-07-20', '2025-07-21', '2025-07-21', N'Vỡ kính camera sau, ảnh hưởng thấu kính', N'Thay kính camera và lau bụi cảm biến', 400000, N'Đã trả khách', N'Ảnh chụp nét lại'),
('PBT17', 'KH002', 'NV003', 'MM16', '861000333444016', '2025-08-05', '2025-08-07', '2025-08-07', N'Hư cảm biến tiệm cận, áp tai không tắt màn', N'Thay dây cảm biến tiệm cận', 550000, N'Đã trả khách', N'Đã fix lỗi triệt để'),
('PBT18', 'KH003', 'NV003', 'MM28', '861000555666028', '2025-08-19', '2025-08-20', '2025-08-20', N'Lỗi rung liên tục hoặc mất rung hoàn toàn', N'Thay cục rung tuyến tính mới', 250000, N'Đã trả khách', N'Rung êm như zin'),
('PBT19', 'KH001', 'NV003', 'MM37', '861000777888037', '2025-09-02', '2025-09-04', '2025-09-04', N'Màn hình bị lưu ảnh (Burn-in) bóng mờ', N'Thay màn hình OLED Zeiss mới', 3800000, N'Đã trả khách', N'Hàng linh kiện cao cấp'),
('PBT20', 'KH002', 'NV003', 'MM46', '861000999000046', '2025-09-15', '2025-09-18', '2025-09-17', N'Treo logo Google không khởi động được vào trong', N'Chạy lại phần mềm gốc Firmware Android', 200000, N'Đã trả khách', N'Đã cứu máy thành công'),
('PBT21', 'KH003', 'NV003', 'MM07', '358000111222007', '2025-10-01', '2025-10-02', '2025-10-02', N'Hư hỏng loa ngoài (loa chuông báo thức)', N'Thay loa ngoài chính hãng Apple', 500000, N'Đã trả khách', N'Âm thanh to rõ'),
('PBT22', 'KH001', 'NV003', 'MM19', '861000333444019', '2025-10-10', '2025-10-12', '2025-10-12', N'Vỏ sườn bị cong vênh nghiêm trọng', N'Thay bộ vỏ sườn khung máy mới', 950000, N'Đã trả khách', N'Độ khít 99%'),
('PBT23', 'KH002', 'NV003', 'MM23', '861000555666023', '2025-11-05', '2025-11-06', '2025-11-06', N'Lỗi kết nối Wifi yếu, không dò được mạng', N'Thay dây anten thu phát sóng Wifi', 350000, N'Đã trả khách', N'Bắt wifi căng đét'),
('PBT24', 'KH003', 'NV003', 'MM31', '861000777888031', '2025-11-20', '2025-11-22', '2025-11-22', N'Mất vân tay hoặc nhận diện khuôn mặt lỗi', N'Sửa cụm cáp vân tay dán chân main', 600000, N'Đã trả khách', N'Nhạy lại bình thường'),
('PBT25', 'KH001', 'NV003', 'MM08', '358000111222008', '2025-12-01', '2025-12-02', '2025-12-02', N'Pin hỏng phồng rộp làm đẩy màn hình lên', N'Thay pin chính hãng tháo máy', 900000, N'Đã trả khách', N'Hỗ trợ cố định lại ron màn hình'),
('PBT26', 'KH002', 'NV003', 'MM13', '861000333444013', '2025-12-12', '2025-12-14', '2025-12-14', N'Màn hình nhấp nháy sọc trắng nửa màn', N'Ép lại cáp màn hình hiển thị', 1200000, N'Đã trả khách', N'Bảo hành ép kính 3 tháng'),
('PBT27', 'KH003', 'NV003', 'MM26', '861000555666026', '2025-12-25', '2025-12-25', '2025-12-25', N'Quên mật khẩu màn hình, khóa tài khoản', N'Bẻ khóa mật khẩu, cài lại Rom', 300000, N'Đã trả khách', N'Xóa sạch dữ liệu cũ'),
('PBT28', 'KH001', 'NV003', 'MM09', '358000111222009', '2026-01-05', '2026-01-06', '2026-01-06', N'Hư camera trước, chụp ảnh màn sương', N'Thay cụm camera selfie trước', 1100000, N'Đã trả khách', N'FaceID hoạt động ổn định'),
('PBT29', 'KH002', 'NV003', 'MM17', '861000333444017', '2026-01-15', '2026-01-16', '2026-01-16', N'Cắm sạc báo thiết bị quá nhiệt, không vào pin', N'Thay ic nhiệt độ phòng đuôi sạc', 450000, N'Đã trả khách', N'Hết báo lỗi'),
('PBT30', 'KH003', 'NV003', 'MM24', '861000555666024', '2026-02-02', '2026-02-03', '2026-02-03', N'Vỡ nát mặt kính ngoài, cảm ứng tốt', N'Cạo kính cũ, ép mặt kính cường lực mới', 600000, N'Đã trả khách', N'Kính cường lực Gorilla Glass'),
('PBT31', 'KH001', 'NV003', 'MM35', '861000777888035', '2026-02-18', '2026-02-19', '2026-02-19', N'Đèn flash sau không sáng khi chụp hình', N'Thay bóng led đuôi cáp đèn flash', 250000, N'Đã trả khách', N'Đèn sáng tốt'),
('PBT32', 'KH002', 'NV003', 'MM42', '861000999000042', '2026-03-01', '2026-03-03', '2026-03-03', N'Jack cắm tai nghe 3.5mm bị gãy kẹt lõi', N'Gắp lõi gãy, thay cụm jack âm thanh', 300000, N'Đã trả khách', N'Nghe nhạc lại bình thường'),
('PBT33', 'KH003', 'NV003', 'MM10', '358000111222010', '2026-03-10', '2026-03-12', '2026-03-12', N'Màn hình trầy xước quá nặng', N'Đánh bóng kính màn hình bằng dung dịch', 400000, N'Đã trả khách', N'Màn láng mịn như mới'),
('PBT34', 'KH001', 'NV003', 'MM20', '861000333444020', '2026-03-20', '2026-03-21', '2026-03-21', N'Khách báo pin tụt siêu nhanh qua đêm', N'Thay ic nguồn phụ hao nguồn dòng', 1200000, N'Đã trả khách', N'Đã đo dòng ổn định'),
('PBT35', 'KH002', 'NV003', 'MM27', '861000555666027', '2026-04-01', '2026-04-03', '2026-04-03', N'Xe cán qua nát bét máy, gãy đôi mainboard', N'Không thể cứu chữa, trả lại linh kiện xác', 0, N'Không sửa được', N'Trả máy lại cho khách vớt vác linh kiện rác'),
('PBT36', 'KH003', 'NV003', 'MM32', '861000777888032', '2026-04-05', '2026-04-08', '2026-04-07', N'Rớt biển ngâm nước mặn 3 ngày hoen rỉ', N'Trả máy do rỉ sét mạch toàn bộ main', 0, N'Không sửa được', N'Không thu phí kiểm tra máy của khách'),
('PBT37', 'KH001', 'NV003', 'MM33', '861000777888033', '2026-05-01', '2026-05-03', NULL, N'Bản lề gập kêu cọc cọc, kẹt khớp', N'Vệ sinh trục bản lề, tra mỡ silicon máy gập', 800000, N'Đã sửa xong', N'Máy đang để tủ chờ khách ra nhận'),
('PBT38', 'KH002', 'NV003', 'MM36', '861000777888036', '2026-05-05', '2026-05-06', NULL, N'Loa ngoài bị tịt không nghe tiếng chuông', N'Thay cụm loa chuông zin Oppo', 400000, N'Đã sửa xong', N'Đóng gói niêm phong chờ lấy'),
('PBT39', 'KH003', 'NV003', 'MM38', '861000777888038', '2026-05-08', '2026-05-09', NULL, N'Cảm ứng bị nhảy loạn loạn (ghost touch)', N'Thay mặt kính ép cảm ứng tích hợp', 750000, N'Đã sửa xong', N'Đã bọc keo dán bảo vệ'),
('PBT40', 'KH001', 'NV003', 'MM39', '861000777888039', '2026-05-10', '2026-05-11', NULL, N'Camera trước bị lọt bụi vào thấu kính', N'Tháo máy vệ sinh buồng camera trước', 250000, N'Đã sửa xong', N'Chụp selfie sáng rõ lại'),
('PBT41', 'KH002', 'NV003', 'MM40', '861000777888040', '2026-05-12', '2026-05-14', NULL, N'Vỡ màn hình chảy mực góc trái', N'Đang chờ linh kiện màn hình Vivo gửi về để thay', 2400000, N'Đang sửa chữa', N'Màn hình khan hàng hiếm'),
('PBT42', 'KH003', 'NV003', 'MM41', '861000777888041', '2026-05-13', '2026-05-15', NULL, N'Cắm tai nghe lúc được lúc không', N'Đang hàn lại chân socket tiếp xúc âm thanh', 300000, N'Đang sửa chữa', N'Khách gọi giục liên tục'),
('PBT43', 'KH001', 'NV003', 'MM43', '861000999000043', '2026-05-14', '2026-05-16', NULL, N'Hao nguồn ăn dòng sụt pin rất nhanh', N'Đang đo dạc tìm tụ chập rò điện trên main', 800000, N'Đang sửa chữa', N'Ca khó, mainboard Sony nhiều lớp'),
('PBT44', 'KH002', 'NV003', 'MM45', '861000999000045', '2026-05-14', '2026-05-16', NULL, N'Nứt kính nắp lưng sau camera', N'Đang bóc mặt kính lưng sau vỡ', 500000, N'Đang sửa chữa', N'Hẹn chiều trả máy'),
('PBT45', 'KH003', 'NV001', 'MM47', '861000999000047', '2026-05-15', '2026-05-17', NULL, N'Khách báo micro video quay phim không thu tiếng', N'Nhận quầy, chờ chuyển tổ kỹ thuật', 400000, N'Đã tiếp nhận', N'Ngoại hình máy đẹp không móp méo'),
('PBT46', 'KH001', 'NV001', 'MM48', '861000999000048', '2026-05-15', '2026-05-16', NULL, N'Chai pin sưng to, đẩy hở viền nhựa', N'Nhận quầy, chờ thay pin Pixel 7a', 650000, N'Đã tiếp nhận', N'Cần xử lý gấp tránh nổ pin'),
('PBT47', 'KH002', 'NV001', 'MM49', '861000999000049', '2026-05-16', '2026-05-18', NULL, N'Sọc màn hình trắng hồng dọc máy', N'Nhận quầy, chờ báo giá thay màn OnePlus 12', 4200000, N'Đã tiếp nhận', N'Khách ký tên lên linh kiện main'),
('PBT48', 'KH003', 'NV001', 'MM50', '861000999000050', '2026-05-16', '2026-05-17', NULL, N'Lỗi Face ID / Vân tay chập chờn lúc nhận lúc không', N'Nhận quầy, chuyển phòng kỹ thuật chạy test', 500000, N'Đã tiếp nhận', N'Khách để máy lại qua đêm'),
('PBT49', 'KH001', 'NV001', 'MM11', '861000333444666', '2026-05-16', '2026-05-19', NULL, N'Kính chắn camera zoom 5x bị nứt', N'Nhận quầy, chờ bóc thay thấu kính bảo vệ ngoài', 600000, N'Đã tiếp nhận', N'Ống kính trong không xước'),
('PBT50', 'KH002', 'NV001', 'MM21', '861000555666111', '2026-05-16', '2026-05-17', NULL, N'Loa ngoài nhỏ do bám đầy bụi bẩn, sơ vải', N'Nhận quầy, chuyển kỹ thuật vệ sinh màng loa', 100000, N'Đã tiếp nhận', N'Làm lấy ngay sau 15 phút');
GO
INSERT INTO HoaDon (MaHD, MaKH, MaNV, NgayLap, TongTien, GiamGia, ThanhTien, TrangThai, GhiChu) VALUES
('HD01', 'KH001', 'NV002', '2024-01-15', 31990000, 500000, 31490000, N'Đã thanh toán', N'Khách VIP mua iPhone 15 Pro Max'),
('HD02', 'KH000', 'NV002', '2024-02-18', 26990000, 0, 26990000, N'Đã thanh toán', N'Khách vãng lai mua Galaxy S24 Ultra'),
('HD03', 'KH002', 'NV002', '2024-03-05', 19990000, 200000, 19790000, N'Đã thanh toán', N'Khách mua Xiaomi 14 trả góp'),
('HD04', 'KH003', 'NV002', '2024-04-12', 9690000, 0, 9690000, N'Đã thanh toán', N'Mua Galaxy A55 làm máy phụ'),
('HD05', 'KH000', 'NV002', '2024-05-20', 42990000, 1000000, 41990000, N'Đã thanh toán', N'Khách mua combo 2 máy iPhone'),
('HD06', 'KH001', 'NV002', '2024-06-02', 28990000, 300000, 28690000, N'Đã thanh toán', N'Mua Xiaomi 14 Ultra chụp ảnh Leica'),
('HD07', 'KH002', 'NV002', '2024-07-22', 34990000, 500000, 34490000, N'Đã thanh toán', N'Mua Galaxy Z Fold 5 thời thượng'),
('HD08', 'KH000', 'NV002', '2024-08-11', 8990000, 0, 8990000, N'Đã thanh toán', N'Khách sinh viên mua Poco X6 Pro cày game'),
('HD09', 'KH003', 'NV002', '2024-09-09', 26490000, 200000, 26290000, N'Đã thanh toán', N'Mua iPhone 15 Pro màu Titan Trắng'),
('HD10', 'KH000', 'NV002', '2024-09-25', 18990000, 0, 18990000, N'Đã thanh toán', N'Khách mua Galaxy Z Flip 5 tặng vợ'),
('HD11', 'KH001', 'NV002', '2024-10-05', 24990000, 400000, 24590000, N'Đã thanh toán', N'Mua Oppo Find X7 Ultra xách tay'),
('HD12', 'KH002', 'NV002', '2024-11-14', 23990000, 300000, 23690000, N'Đã thanh toán', N'Mua Vivo X100 Pro trải nghiệm ống kính Zeiss'),
('HD13', 'KH000', 'NV002', '2024-12-01', 29990000, 0, 29990000, N'Đã thanh toán', N'Khách mua quái vật Asus ROG Phone 8 Pro'),
('HD14', 'KH003', 'NV002', '2024-12-20', 21990000, 200000, 21790000, N'Đã thanh toán', N'Khách mua Google Pixel 8 Pro làm lập trình'),
('HD15', 'KH000', 'NV002', '2024-12-28', 15490000, 100000, 15390000, N'Đã thanh toán', N'Mua Oppo Reno 11 Pro làm máy chụp ảnh chân dung'),
('HD16', 'KH001', 'NV002', '2025-01-05', 31990000, 600000, 31390000, N'Đã thanh toán', N'Mở hàng đầu năm thuận lợi'),
('HD17', 'KH000', 'NV002', '2025-01-18', 26990000, 0, 26990000, N'Đã thanh toán', N'Khách mua sắm Tết Nguyên Đán'),
('HD18', 'KH002', 'NV002', '2025-02-10', 36990000, 500000, 36490000, N'Đã thanh toán', N'Mua iPhone 15 Pro Max bản 512GB'),
('HD19', 'KH003', 'NV002', '2025-02-25', 10690000, 0, 10690000, N'Đã thanh toán', N'Lên đời Galaxy A55 bản 256GB'),
('HD20', 'KH000', 'NV002', '2025-03-08', 21490000, 500000, 20990000, N'Đã thanh toán', N'Quà tặng ngày Quốc tế Phụ nữ 8/3'),
('HD21', 'KH001', 'NV002', '2025-03-22', 28990000, 400000, 28590000, N'Đã thanh toán', N'Khách chốt thêm 1 con Samsung cao cấp'),
('HD22', 'KH000', 'NV002', '2025-04-04', 19990000, 0, 19990000, N'Đã thanh toán', N'Khách mua trả thẳng tại quầy'),
('HD23', 'KH002', 'NV002', '2025-04-29', 42990000, 1000000, 41990000, N'Đã thanh toán', N'Đại gia mua iPhone bản 1TB kịch độc'),
('HD24', 'KH003', 'NV002', '2025-05-15', 22990000, 200000, 22790000, N'Đã thanh toán', N'Mua iPhone 15 thường bản 256GB'),
('HD25', 'KH000', 'NV002', '2025-05-30', 9990000, 0, 9990000, N'Đã thanh toán', N'Khách mua Poco X6 Pro bản 512GB lưu trữ game'),
('HD26', 'KH001', 'NV002', '2025-06-12', 32490000, 500000, 31990000, N'Đã thanh toán', N'Chốt Samsung S24 Ultra bản 512GB'),
('HD27', 'KH000', 'NV002', '2025-06-28', 25990000, 0, 25990000, N'Đã thanh toán', N'Khách vãng lai mua sắm ngày hè'),
('HD28', 'KH002', 'NV002', '2025-07-10', 37990000, 800000, 37190000, N'Đã thanh toán', N'Mua Galaxy Z Fold 5 512GB làm việc doanh nhân'),
('HD29', 'KH003', 'NV002', '2025-07-26', 18990000, 100000, 18890000, N'Đã thanh toán', N'Mua Galaxy Z Flip 5 dạo phố thời trang'),
('HD30', 'KH000', 'NV002', '2025-08-08', 23490000, 0, 23490000, N'Đã thanh toán', N'Mua iPhone 15 Plus xem phim giải trí'),
('HD31', 'KH001', 'NV002', '2025-09-02', 29190000, 300000, 28890000, N'Đã thanh toán', N'Khách VIP mua sắm ngày Quốc khánh 2/9'),
('HD32', 'KH000', 'NV002', '2025-10-15', 24990000, 0, 24990000, N'Đã thanh toán', N'Khách mua Oppo Find X7 Ultra'),
('HD33', 'KH002', 'NV002', '2025-11-20', 23990000, 400000, 23590000, N'Đã thanh toán', N'Mua quà ngày Nhà giáo Việt Nam 20/11'),
('HD34', 'KH003', 'NV002', '2025-12-12', 21990000, 200000, 21790000, N'Đã thanh toán', N'Mua Google Pixel 8 Pro chụp ảnh phong cảnh'),
('HD35', 'KH000', 'NV002', '2025-12-24', 27990000, 500000, 27490000, N'Đã thanh toán', N'Khách mua sắm mùa Noel Giáng Sinh'),
('HD36', 'KH001', 'NV002', '2026-01-08', 31990000, 500000, 31490000, N'Đã thanh toán', N'Hóa đơn đầu năm 2026 của sếp Vượng'),
('HD37', 'KH000', 'NV002', '2026-01-25', 26990000, 0, 26990000, N'Đã thanh toán', N'Khách mua máy ăn Tết 2026'),
('HD38', 'KH002', 'NV002', '2026-02-14', 18990000, 300000, 18690000, N'Đã thanh toán', N'Mua tặng người yêu ngày Valentine 14/2'),
('HD39', 'KH003', 'NV002', '2026-03-02', 9690000, 0, 9690000, N'Đã thanh toán', N'Khách quen mua thêm thiết bị tầm trung'),
('HD40', 'KH000', 'NV002', '2026-03-20', 22990000, 100000, 22890000, N'Đã thanh toán', N'Mua Galaxy S24 phiên bản lưu trữ chuẩn'),
('HD41', 'KH001', 'NV002', '2026-04-05', 36990000, 500000, 36490000, N'Đã thanh toán', N'Đổi trả nâng cấp lên bản 512GB'),
('HD42', 'KH000', 'NV002', '2026-04-18', 25990000, 0, 25990000, N'Đã thanh toán', N'Khách vãng lai mua Galaxy S24+'),
('HD43', 'KH002', 'NV002', '2026-04-30', 42990000, 1000000, 41990000, N'Đã thanh toán', N'Săn sale ngày giải phóng miền Nam 30/4'),
('HD44', 'KH003', 'NV002', '2026-05-01', 29190000, 200000, 28990000, N'Đã thanh toán', N'Mua iPhone 15 Pro ngày Quốc tế Lao động 1/5'),
('HD45', 'KH000', 'NV002', '2026-05-10', 15490000, 0, 15490000, N'Đã thanh toán', N'Mua Oppo Reno 11 Pro tặng mẹ'),
('HD46', 'KH001', 'NV002', '2026-05-15', 31990000, 500000, 31490000, N'Chờ xử lý', N'Khách đặt cọc giữ hàng chờ máy về kho'),
('HD47', 'KH000', 'NV002', '2026-05-15', 26990000, 0, 26990000, N'Chờ xử lý', N'Đơn hàng thanh toán quẹt thẻ lỗi mạng'),
('HD48', 'KH002', 'NV002', '2026-05-16', 36990000, 0, 36990000, N'Chờ xử lý', N'Khách đặt giao hàng tận nơi Ship COD'),
('HD49', 'KH003', 'NV002', '2026-05-16', 19990000, 200000, 19790000, N'Đã hủy', N'Hủy đơn do khách đổi ý sang mua dòng Ultra'),
('HD50', 'KH000', 'NV002', '2026-05-16', 22990000, 0, 22990000, N'Đã hủy', N'Khách không đủ điều kiện xét duyệt duyệt trả góp');
GO

DBCC CHECKIDENT ('ChiTietHoaDon', RESEED, 0);
GO

INSERT INTO ChiTietHoaDon (MaHD, SoIMEI, DonGia, SoLuong, MaChiTietSP) VALUES
('HD01', '358000111222101', 31990000, 1, 'CT01'), -- iPhone 15 Pro Max 256GB
('HD02', '861000333444102', 26990000, 1, 'CT12'), -- Galaxy S24 Ultra 256GB
('HD03', '861000555666103', 19990000, 1, 'CT25'), -- Xiaomi 14 256GB
('HD04', '861000333444104', 9690000, 1, 'CT21'),  -- Galaxy A55 128GB
('HD05', '358000111222105', 42990000, 1, 'CT03'), -- iPhone 15 Pro Max 1TB
('HD06', '861000555666106', 28990000, 1, 'CT23'), -- Xiaomi 14 Ultra
('HD07', '861000333444107', 34990000, 1, 'CT17'), -- Galaxy Z Fold 5
('HD08', '861000555666108', 8990000, 1, 'CT27'),  -- Poco X6 Pro
('HD09', '358000111222109', 26490000, 1, 'CT04'), -- iPhone 15 Pro
('HD10', '861000333444110', 18990000, 1, 'CT19'), -- Galaxy Z Flip 5
('HD11', '861000777888111', 24990000, 1, 'CT29'), -- Oppo Find X7 Ultra
('HD12', '861000777888112', 23990000, 1, 'CT32'), -- Vivo X100 Pro
('HD13', '861000999000113', 29990000, 1, 'CT34'), -- ROG Phone 8 Pro
('HD14', '861000999000114', 21990000, 1, 'CT35'), -- Pixel 8 Pro
('HD15', '861000777888115', 15490000, 1, 'CT31'), -- Oppo Reno 11 Pro
('HD16', '358000111222116', 31990000, 1, 'CT01'), -- iPhone 15 Pro Max
('HD17', '861000333444117', 26990000, 1, 'CT12'), -- Galaxy S24 Ultra
('HD18', '358000111222118', 36990000, 1, 'CT02'), -- iPhone 15 Pro Max 512GB
('HD19', '861000333444119', 10690000, 1, 'CT22'), -- Galaxy A55 256GB
('HD20', '861000999000120', 21490000, 1, 'CT35'), -- Pixel 8 Pro
('HD21', '861000333444121', 28990000, 1, 'CT12'), -- S24 Ultra
('HD22', '358000111222122', 19990000, 1, 'CT08'), -- iPhone 15 128GB
('HD23', '358000111222123', 42990000, 1, 'CT03'), -- iPhone 15 Pro Max 1TB
('HD24', '358000111222124', 22990000, 1, 'CT09'), -- iPhone 15 256GB
('HD25', '861000555666125', 9990000, 1, 'CT28'),  -- Poco X6 Pro 512GB
('HD26', '861000333444126', 32490000, 1, 'CT13'), -- S24 Ultra 512GB
('HD27', '358000111222127', 25990000, 1, 'CT07'), -- iPhone 15 Plus 256GB
('HD28', '861000333444128', 37990000, 1, 'CT18'), -- Z Fold 5 512GB
('HD29', '861000333444129', 18990000, 1, 'CT19'), -- Z Flip 5 256GB
('HD30', '358000111222130', 23490000, 1, 'CT06'), -- iPhone 15 Plus 128GB
('HD31', '358000111222131', 29190000, 1, 'CT05'), -- iPhone 15 Pro 256GB
('HD32', '861000777888132', 24990000, 1, 'CT29'), -- Oppo Find X7
('HD33', '861000777888133', 23990000, 1, 'CT33'), -- Vivo X100 Pro Cam
('HD34', '861000999000134', 21990000, 1, 'CT35'), -- Pixel 8 Pro
('HD35', '861000777888135', 27490000, 1, 'CT30'), -- Oppo Find X7 Ultra 512GB
('HD36', '358000111222136', 31990000, 1, 'CT01'), -- iPhone 15 Pro Max
('HD37', '861000333444137', 26990000, 1, 'CT12'), -- S24 Ultra
('HD38', '861000333444138', 18990000, 1, 'CT19'), -- Z Flip 5
('HD39', '861000333444139', 9690000, 1, 'CT21'),  -- Galaxy A55
('HD40', '861000333444140', 22990000, 1, 'CT15'), -- S24+
('HD41', '358000111222141', 36990000, 1, 'CT02'), -- iPhone 15 Pro Max 512
('HD42', '861000333444142', 25990000, 1, 'CT16'), -- S24+ 512GB
('HD43', '358000111222143', 42990000, 1, 'CT03'), -- iPhone 15 PM 1TB
('HD44', '358000111222144', 29190000, 1, 'CT05'), -- iPhone 15 Pro 256GB
('HD45', '861000777888145', 15490000, 1, 'CT31'), -- Oppo Reno 11 Pro
('HD46', '358000111222146', 31990000, 1, 'CT01'),
('HD47', '861000333444147', 26990000, 1, 'CT12'),
('HD48', '358000111222148', 36990000, 1, 'CT02'),
('HD49', '861000555666149', 19990000, 1, 'CT25'),
('HD50', '861000333444150', 22990000, 1, 'CT15');
GO
INSERT INTO PhieuNhap (MaPhieu, MaHang, MaNV, NgayNhap, TongTien, TrangThai, GhiChu) VALUES
('PN01', 'NCC01', 'NV004', '2024-01-05', 850000000, N'Đã nhập', N'Nhập lô iPhone 15 series phục vụ đầu năm'),
('PN02', 'NCC02', 'NV004', '2024-01-20', 540000000, N'Đã nhập', N'Nhập Samsung Galaxy S24 Ultra ra mắt'),
('PN03', 'NCC03', 'NV004', '2024-02-15', 320000000, N'Đã nhập', N'Nhập Xiaomi 14 series bản nội địa'),
('PN04', 'NCC04', 'NV004', '2024-03-10', 210000000, N'Đã nhập', N'Lô hàng Oppo Reno 11 series'),
('PN05', 'NCC01', 'NV001', '2024-04-05', 450000000, N'Đã nhập', N'Bổ sung iPhone 14 Pro Max tồn kho'),
('PN06', 'NCC05', 'NV004', '2024-05-12', 150000000, N'Đã nhập', N'Nhập Vivo V30 tầm trung'),
('PN07', 'NCC08', 'NV004', '2024-06-01', 120000000, N'Đã nhập', N'Nhập Sony Xperia 1 V kén khách'),
('PN08', 'NCC02', 'NV001', '2024-07-20', 680000000, N'Đã nhập', N'Nhập lô Galaxy Z Fold 5 và Flip 5'),
('PN09', 'NCC09', 'NV004', '2024-08-15', 250000000, N'Đã nhập', N'Asus ROG Phone 8 Pro cho game thủ'),
('PN10', 'NCC03', 'NV004', '2024-09-05', 180000000, N'Đã nhập', N'Lô Poco X6 Pro bán tựu trường'),
('PN11', 'NCC01', 'NV004', '2024-10-10', 950000000, N'Đã nhập', N'Nhập số lượng lớn iPhone chuẩn bị cuối năm'),
('PN12', 'NCC10', 'NV004', '2024-11-02', 300000000, N'Đã nhập', N'Nhập Google Pixel 8 Pro hàng xách tay'),
('PN13', 'NCC04', 'NV004', '2024-11-20', 220000000, N'Đã nhập', N'Bổ sung Oppo Find X7 Ultra'),
('PN14', 'NCC02', 'NV001', '2024-12-05', 400000000, N'Đã nhập', N'Nhập bổ sung dòng A series (A35, A55)'),
('PN15', 'NCC11', 'NV004', '2024-12-25', 180000000, N'Đã nhập', N'Nhập OnePlus 12 đón Tết dương lịch'),
00000, N'Đã nhập', N'Lô iPhone 15 Pro Max số lượng lớn đón Tết'),
('PN17', 'NCC02', 'NV004', '2025-02-05', 600000000, N'Đã nhập', N'Nhập thêm S24 Ultra và S24 Plus'),
('PN18', 'NCC03', 'NV004', '2025-03-01', 350000000, N'Đã nhập', N'Nhập Xiaomi 14 Ultra chụp ảnh'),
('PN19', 'NCC05', 'NV004', '2025-03-20', 200000000, N'Đã nhập', N'Lô Vivo X100 Pro mới nhất'),
('PN20', 'NCC04', 'NV001', '2025-04-15', 280000000, N'Đã nhập', N'Nhập Oppo gập Find N3'),
('PN21', 'NCC01', 'NV004', '2025-05-10', 500000000, N'Đã nhập', N'Nhập các dòng iPhone Plus và thường'),
('PN22', 'NCC02', 'NV004', '2025-05-25', 300000000, N'Đã nhập', N'Bổ sung Samsung A series giá rẻ'),
('PN23', 'NCC09', 'NV004', '2025-06-15', 200000000, N'Đã nhập', N'Nhập thêm thiết bị Gaming Asus Zenfone'),
('PN24', 'NCC10', 'NV004', '2025-07-05', 150000000, N'Đã nhập', N'Nhập Pixel 7a giá rẻ'),
('PN25', 'NCC03', 'NV004', '2025-07-20', 400000000, N'Đã nhập', N'Nhập dòng Redmi Note 13 series bán chạy'),
('PN26', 'NCC01', 'NV004', '2025-08-10', 800000000, N'Đã nhập', N'Lô hàng iPhone 15 Pro các màu Titan'),
('PN27', 'NCC02', 'NV004', '2025-08-25', 750000000, N'Đã nhập', N'Samsung Z Fold 5 và phụ kiện'),
('PN28', 'NCC04', 'NV001', '2025-09-12', 250000000, N'Đã nhập', N'Oppo Reno 11 Pro đợt 2'),
('PN29', 'NCC05', 'NV004', '2025-09-28', 180000000, N'Đã nhập', N'Vivo Y100 học sinh sinh viên'),
('PN30', 'NCC08', 'NV004', '2025-10-15', 100000000, N'Đã nhập', N'Nhập Sony Xperia 5 V theo đơn đặt trước'),
('PN31', 'NCC11', 'NV004', '2025-11-05', 220000000, N'Đã nhập', N'OnePlus 12R giá tốt'),
('PN32', 'NCC01', 'NV004', '2025-11-25', 900000000, N'Đã nhập', N'Gom hàng Apple bán Giáng Sinh'),
('PN33', 'NCC03', 'NV001', '2025-12-10', 450000000, N'Đã nhập', N'Lô Xiaomi 13T Pro clear kho cuối năm'),
('PN34', 'NCC02', 'NV004', '2025-12-20', 500000000, N'Đã nhập', N'Chốt lô Samsung cuối năm báo cáo thuế'),
('PN35', 'NCC10', 'NV004', '2025-12-28', 150000000, N'Đã nhập', N'Hàng tồn Pixel 8 chốt sổ'),
('PN36', 'NCC01', 'NV004', '2026-01-10', 700000000, N'Đã nhập', N'Lô hàng Apple khai xuân 2026'),
('PN37', 'NCC02', 'NV004', '2026-02-05', 650000000, N'Đã nhập', N'Nhập S24 Ultra bổ sung sau Tết'),
('PN38', 'NCC03', 'NV004', '2026-03-01', 300000000, N'Đã nhập', N'Nhập Redmi 13C phục vụ khách bình dân'),
('PN39', 'NCC04', 'NV001', '2026-03-15', 240000000, N'Đã nhập', N'Oppo Find N3 Flip màn gập'),
('PN40', 'NCC05', 'NV004', '2026-04-10', 160000000, N'Đã nhập', N'Vivo V30 series mùa hè'),
('PN41', 'NCC09', 'NV004', '2026-04-20', 250000000, N'Đã nhập', N'Asus ROG Phone 8 Pro giải đấu Esport'),
('PN42', 'NCC01', 'NV004', '2026-05-02', 850000000, N'Đã nhập', N'Nhập iPhone 15 Pro Max chuẩn bị lễ lớn'),
('PN43', 'NCC02', 'NV004', '2026-05-08', 400000000, N'Đã nhập', N'Samsung Galaxy A35, A55 nhập kho'),
('PN44', 'NCC10', 'NV004', '2026-05-12', 180000000, N'Đã nhập', N'Google Pixel nhập theo yêu cầu'),
('PN45', 'NCC03', 'NV004', '2026-05-14', 320000000, N'Đã nhập', N'Xiaomi 14 siêu phẩm Leica'),
('PN46', 'NCC01', 'NV004', '2026-05-15', 600000000, N'Đang giao', N'Lô hàng Apple đang kẹt ở hải quan sân bay'),
('PN47', 'NCC02', 'NV004', '2026-05-16', 450000000, N'Đang giao', N'Nhà phân phối báo chiều nay xe tải tới kho'),
('PN48', 'NCC04', 'NV001', '2026-05-16', 150000000, N'Đang giao', N'Đơn hàng Oppo chờ duyệt công nợ'),
('PN49', 'NCC03', 'NV004', '2026-05-10', 300000000, N'Đã hủy', N'Hủy do sai giá trên hợp đồng VAT'),
('PN50', 'NCC08', 'NV004', '2026-05-12', 120000000, N'Đã hủy', N'Nhà cung cấp Sony báo hết hàng toàn cầu');
GO
INSERT INTO ChiTietPhieuNhap (MaPhieu, SoIMEI, DonGiaNhap, SoLuong, TrangThaiIMEI, MaChiTietSP) VALUES
('PN01', '358111000222001', 27500000, 1, N'Trong kho', 'CT01'),
('PN01', '358111000222002', 27500000, 1, N'Trong kho', 'CT01'),
('PN01', '358111000222003', 27500000, 1, N'Trong kho', 'CT01'),
('PN01', '358111000222004', 32000000, 1, N'Trong kho', 'CT02'),
('PN01', '358111000222005', 32000000, 1, N'Đã bán', 'CT02'), 
('PN02', '861222000333001', 25000000, 1, N'Trong kho', 'CT12'),
('PN02', '861222000333002', 25000000, 1, N'Trong kho', 'CT12'),
('PN02', '861222000333003', 25000000, 1, N'Trong kho', 'CT12'),
('PN02', '861222000333004', 28000000, 1, N'Trong kho', 'CT13'),
('PN02', '861222000333005', 28000000, 1, N'Đã bán', 'CT13'),
('PN03', '861333000444001', 24500000, 1, N'Trong kho', 'CT23'),
('PN03', '861333000444002', 24500000, 1, N'Trong kho', 'CT23'),
('PN03', '861333000444003', 24500000, 1, N'Trong kho', 'CT23'),
('PN03', '861333000444004', 17000000, 1, N'Trong kho', 'CT25'),
('PN03', '861333000444005', 17000000, 1, N'Trong kho', 'CT25'),
('PN08', '861888000999001', 31000000, 1, N'Trong kho', 'CT17'),
('PN08', '861888000999002', 31000000, 1, N'Trong kho', 'CT17'),
('PN08', '861888000999003', 16500000, 1, N'Trong kho', 'CT19'),
('PN08', '861888000999004', 16500000, 1, N'Trong kho', 'CT19'),
('PN08', '861888000999005', 16500000, 1, N'Đã bán', 'CT19'),
('PN14', '861444000555001', 8000000, 1, N'Trong kho', 'CT21'),
('PN14', '861444000555002', 8000000, 1, N'Trong kho', 'CT21'),
('PN14', '861444000555003', 8000000, 1, N'Trong kho', 'CT21'),
('PN14', '861444000555004', 8000000, 1, N'Trong kho', 'CT21'),
('PN14', '861444000555005', 8000000, 1, N'Trong kho', 'CT21'),
('PN21', '358222000333001', 17500000, 1, N'Trong kho', 'CT08'),
('PN21', '358222000333002', 17500000, 1, N'Trong kho', 'CT08'),
('PN21', '358222000333003', 17500000, 1, N'Trong kho', 'CT08'),
('PN21', '358222000333004', 17500000, 1, N'Trong kho', 'CT08'),
('PN21', '358222000333005', 17500000, 1, N'Đã bán', 'CT08'),
('PN10', '861555000666001', 7400000, 1, N'Trong kho', 'CT27'),
('PN10', '861555000666002', 7400000, 1, N'Trong kho', 'CT27'),
('PN10', '861555000666003', 7400000, 1, N'Trong kho', 'CT27'),
('PN10', '861555000666004', 7400000, 1, N'Trong kho', 'CT27'),
('PN10', '861555000666005', 7400000, 1, N'Trong kho', 'CT27'),
('PN04', '861666000777001', 13000000, 1, N'Trong kho', 'CT31'),
('PN04', '861666000777002', 13000000, 1, N'Trong kho', 'CT31'),
('PN04', '861666000777003', 13000000, 1, N'Trong kho', 'CT31'),
('PN04', '861666000777004', 13000000, 1, N'Trong kho', 'CT31'),
('PN04', '861666000777005', 13000000, 1, N'Đã bán', 'CT31'),
('PN06', '861777000888001', 20500000, 1, N'Trong kho', 'CT32'),
('PN06', '861777000888002', 20500000, 1, N'Trong kho', 'CT32'),
('PN06', '861777000888003', 20500000, 1, N'Trong kho', 'CT32'),
('PN06', '861777000888004', 20500000, 1, N'Trong kho', 'CT32'),
('PN06', '861777000888005', 20500000, 1, N'Trong kho', 'CT32'),
('PN09', '861999000000001', 25500000, 1, N'Trong kho', 'CT34'),
('PN09', '861999000000002', 25500000, 1, N'Trong kho', 'CT34'),
('PN09', '861999000000003', 25500000, 1, N'Trong kho', 'CT34'),
('PN09', '861999000000004', 25500000, 1, N'Trong kho', 'CT34'),
('PN09', '861999000000005', 25500000, 1, N'Đã bán', 'CT34');
GO


--điện thoại
CREATE OR ALTER PROCEDURE sp_ThemDienThoai
    @MaMay NVARCHAR(20),
    @TenMay NVARCHAR(150),
    @MaHang NVARCHAR(20) = NULL,
    @CPU NVARCHAR(100) = NULL,
    @RAM NVARCHAR(50) = NULL,
    @Camera NVARCHAR(100) = NULL,
    @Pin NVARCHAR(50) = NULL,
    @HeDieuHanh NVARCHAR(50) = NULL,
    @TrangThai BIT = 1,
    @DanhMuc NVARCHAR(50) = NULL 
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM DienThoai WHERE MaMay = @MaMay) RETURN -1;
    
    INSERT INTO DienThoai (MaMay, TenMay, MaHang, CPU, RAM, Camera, Pin, HeDieuHanh, TrangThai, DanhMuc)
    VALUES (@MaMay, @TenMay, @MaHang, @CPU, @RAM, @Camera, @Pin, @HeDieuHanh, @TrangThai, @DanhMuc);
    RETURN 1;
END
GO


CREATE OR ALTER PROCEDURE sp_SuaDienThoai
    @MaMay NVARCHAR(20),
    @TenMay NVARCHAR(150),
    @MaHang NVARCHAR(20) = NULL,
    @CPU NVARCHAR(100) = NULL,
    @RAM NVARCHAR(50) = NULL,
    @Camera NVARCHAR(100) = NULL,
    @Pin NVARCHAR(50) = NULL,
    @HeDieuHanh NVARCHAR(50) = NULL,
    @TrangThai BIT = 1,
    @DanhMuc NVARCHAR(50) = NULL 
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE DienThoai 
    SET TenMay = @TenMay, 
        MaHang = @MaHang, 
        CPU = @CPU, 
        RAM = @RAM, 
        Camera = @Camera, 
        Pin = @Pin, 
        HeDieuHanh = @HeDieuHanh, 
        TrangThai = @TrangThai,
        DanhMuc = @DanhMuc 
    WHERE MaMay = @MaMay;
    RETURN 1;
END
GO
 
CREATE OR ALTER PROCEDURE sp_XoaDienThoai
    @MaMay NVARCHAR(20)
AS
BEGIN
    UPDATE DienThoai SET TrangThai = 0 WHERE MaMay = @MaMay;
    UPDATE ChiTietDienThoai SET TrangThai = 0 WHERE MaMay = @MaMay;
END
GO

CREATE OR ALTER PROCEDURE sp_LuuChiTietDienThoai
    @MaChiTiet NVARCHAR(50),
    @MaMay NVARCHAR(20),
    @DungLuong NVARCHAR(20),
    @MauSac NVARCHAR(50),
    @GiaNhap DECIMAL(18,0),
    @GiaBan DECIMAL(18,0)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM ChiTietDienThoai WHERE MaChiTiet = @MaChiTiet)
    BEGIN
        UPDATE ChiTietDienThoai
        SET DungLuong = @DungLuong,
            MauSac = @MauSac,
            GiaNhap = @GiaNhap,
            GiaBan = @GiaBan,
            TrangThai = 1 
        WHERE MaChiTiet = @MaChiTiet;
    END
    ELSE
    BEGIN
        INSERT INTO ChiTietDienThoai (MaChiTiet, MaMay, DungLuong, MauSac, GiaNhap, GiaBan, TrangThai)
        VALUES (@MaChiTiet, @MaMay, @DungLuong, @MauSac, @GiaNhap, @GiaBan, 1);
    END
END
GO
     
CREATE OR ALTER PROCEDURE sp_XoaChiTietDienThoai
    @MaChiTiet NVARCHAR(50)
AS
BEGIN
    UPDATE ChiTietDienThoai 
    SET TrangThai = 0 
    WHERE MaChiTiet = @MaChiTiet;
END
GO


CREATE OR ALTER PROCEDURE sp_LayThongTinIMEI
    @IMEI NVARCHAR(50)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ChiTietPhieuNhap WHERE SoIMEI = @IMEI AND TrangThaiIMEI = N'Trong kho')
        THROW 50001, N'LỖI: Mã IMEI không tồn tại hoặc thiết bị này đã được xuất bán!', 1;

    SELECT pn.SoIMEI, dt.TenMay + ' - ' + pb.DungLuong + ' ' + pb.MauSac AS TenSP, pb.GiaBan
    FROM ChiTietPhieuNhap pn
    JOIN ChiTietDienThoai pb ON pn.MaChiTiet = pb.MaChiTiet
    JOIN DienThoai dt ON pb.MaMay = dt.MaMay
    WHERE pn.SoIMEI = @IMEI;
END
GO

CREATE OR ALTER PROCEDURE sp_ChotHoaDon
    @SoDienThoai_KH NVARCHAR(20),
    @MaNV NVARCHAR(20),
    @GiamGia DECIMAL(18,2),
    @GhiChu NVARCHAR(250),
    @DanhSachIMEI NVARCHAR(MAX) 
AS
BEGIN
    BEGIN TRY
        BEGIN TRAN; 
        IF (@DanhSachIMEI IS NULL OR @DanhSachIMEI = '')
            THROW 50002, N'LỖI: Chứng từ trống! Chưa có sản phẩm nào để xuất bán.', 1;
        DECLARE @MaKH NVARCHAR(20) = 'KH000';
        IF (@SoDienThoai_KH <> '')
        BEGIN
            SELECT @MaKH = MaKH FROM KhachHang WHERE @SoDienThoai_KH = @SoDienThoai_KH;
            IF (@MaKH IS NULL)
                THROW 50003, N'LỖI: Số điện thoại chưa được đăng ký trong hệ thống!', 1;
        END
        DECLARE @MaHD NVARCHAR(20) = 'HD' + FORMAT(GETDATE(), 'yyMMddHHmmss');
        DECLARE @TongTien DECIMAL(18,2) = 0;
        SELECT @TongTien = SUM(pb.GiaBan)
        FROM STRING_SPLIT(@DanhSachIMEI, ',') s
        JOIN ChiTietPhieuNhap pn ON pn.SoIMEI = s.value
        JOIN ChiTietDienThoai pb ON pb.MaChiTiet = pn.MaChiTiet;

        DECLARE @ThanhTien DECIMAL(18,2) = @TongTien - @GiamGia;
        IF (@ThanhTien < 0) SET @ThanhTien = 0;

        INSERT INTO HoaDon (MaHD, MaNV, MaKH, NgayLap, TongTien, GiamGia, ThanhTien, GhiChu, TrangThai)
        VALUES (@MaHD, @MaNV, @MaKH, GETDATE(), @TongTien, @GiamGia, @ThanhTien, @GhiChu, 1);

        INSERT INTO ChiTietHoaDon (MaHD, SoIMEI, DonGia)
        SELECT @MaHD, s.value, pb.GiaBan
        FROM STRING_SPLIT(@DanhSachIMEI, ',') s
        JOIN ChiTietPhieuNhap pn ON pn.SoIMEI = s.value
        JOIN ChiTietDienThoai pb ON pb.MaChiTiet = pn.MaChiTiet;

        UPDATE ChiTietPhieuNhap
        SET TrangThaiIMEI = N'Đã bán'
        WHERE SoIMEI IN (SELECT value FROM STRING_SPLIT(@DanhSachIMEI, ','));

        COMMIT TRAN; 
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN; 
        THROW; 
    END CATCH
END
GO




 ---hãng cung cấp
 
CREATE PROCEDURE sp_InsertHangCungCap
    @MaHang VARCHAR(20),
    @TenHang NVARCHAR(100),
    @QuocGia NVARCHAR(50),
    @DiaChi NVARCHAR(200),
    @SoDienThoai VARCHAR(15),
    @Email VARCHAR(100),
    @TrangThai INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
       
        IF EXISTS (SELECT 1 FROM HangCungCap WHERE MaHang = @MaHang)
        BEGIN
            RAISERROR(N'Mã hãng sản xuất này đã tồn tại trên hệ thống!', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM HangCungCap WHERE TenHang = @TenHang)
        BEGIN
            RAISERROR(N'Tên hãng sản xuất này đã tồn tại trên hệ thống!', 16, 1);
            RETURN;
        END
        INSERT INTO HangCungCap(MaHang, TenHang, QuocGia, DiaChi, SoDienThoai, Email, TrangThai)
        VALUES (@MaHang, @TenHang, @QuocGia, @DiaChi, @SoDienThoai, @Email, @TrangThai);
        
        PRINT N'Thêm mới hãng sản xuất thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE sp_UpdateHangCungCap
    @MaHang VARCHAR(20),
    @TenHang NVARCHAR(100),
    @QuocGia NVARCHAR(50),
    @DiaChi NVARCHAR(200),
    @SoDienThoai VARCHAR(15),
    @Email VARCHAR(100),
    @TrangThai INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM HangCungCap WHERE MaHang = @MaHang)
        BEGIN
            RAISERROR(N'Không tìm thấy hãng sản xuất cần cập nhật thông tin!', 16, 1);
            RETURN;
        END
        IF EXISTS (SELECT 1 FROM HangCungCap WHERE TenHang = @TenHang AND MaHang <> @MaHang)
        BEGIN
            RAISERROR(N'Tên hãng sản xuất mới bị trùng với một hãng khác đã có!', 16, 1);
            RETURN;
        END
        UPDATE HangCungCap
        SET TenHang = @TenHang,
            QuocGia = @QuocGia,
            DiaChi = @DiaChi,
            SoDienThoai = @SoDienThoai,
            Email = @Email,
            TrangThai = @TrangThai
        WHERE MaHang = @MaHang;

        PRINT N'Cập nhật thông tin hãng sản xuất thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE sp_DeleteHangCungCap
    @MaHang VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM HangCungCap WHERE MaHang = @MaHang)
        BEGIN
            RAISERROR(N'Không tìm thấy hãng sản xuất cần xóa!', 16, 1);
            RETURN;
        END
        UPDATE HangCungCap
        SET TrangThai = 0
        WHERE MaHang = @MaHang;

        PRINT N'Xóa hãng sản xuất thành công (Đã cập nhật trạng thái ngừng hoạt động)!';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

--khách hàng
CREATE or alter PROCEDURE sp_InsertKhachHang
  @MaKH VARCHAR(20),
    @HoTen NVARCHAR(100),
    @GioiTinh NVARCHAR(10),
    @NgaySinh DATE,
    @SoDienThoai VARCHAR(15),
    @DiaChi NVARCHAR(200),
    @LoaiKhach NVARCHAR(50),
    @TongChiTieu DECIMAL(18,2) = 0,   
    @NgayTao DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
        BEGIN
            RAISERROR(N'Mã khách hàng này đã tồn tại trên hệ thống!', 16, 1);
            RETURN;
        END
        IF EXISTS (SELECT 1 FROM KhachHang WHERE SoDienThoai = @SoDienThoai)
        BEGIN
            RAISERROR(N'Số điện thoại này đã được đăng ký cho một khách hàng khác!', 16, 1);
            RETURN;
        END
       IF @NgayTao IS NULL
            SET @NgayTao = GETDATE();

        INSERT INTO KhachHang (MaKH, HoTen, GioiTinh, NgaySinh, SoDienThoai, DiaChi, LoaiKhach, TongChiTieu, NgayTao)
        VALUES (@MaKH, @HoTen, @GioiTinh, @NgaySinh, @SoDienThoai, @DiaChi, @LoaiKhach, @TongChiTieu, @NgayTao);
        
        PRINT N'Thêm mới khách hàng thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO


CREATE PROCEDURE sp_UpdateKhachHang
    @MaKH VARCHAR(20),
    @HoTen NVARCHAR(100),
    @GioiTinh NVARCHAR(10),
    @NgaySinh DATE,
    @SoDienThoai VARCHAR(15),
    @DiaChi NVARCHAR(200),
    @LoaiKhach NVARCHAR(50),
    @TongChiTieu DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
        BEGIN
            RAISERROR(N'Không tìm thấy dữ liệu khách hàng cần cập nhật!', 16, 1);
            RETURN;
        END
        IF EXISTS (SELECT 1 FROM KhachHang WHERE SoDienThoai = @SoDienThoai AND MaKH <> @MaKH)
        BEGIN
            RAISERROR(N'Số điện thoại mới bị trùng với thông tin của một khách hàng khác!', 16, 1);
            RETURN;
        END
        UPDATE KhachHang
        SET HoTen = @HoTen,
            GioiTinh = @GioiTinh,
            NgaySinh = @NgaySinh,
            SoDienThoai = @SoDienThoai,
            DiaChi = @DiaChi,
            LoaiKhach = @LoaiKhach,
            TongChiTieu = @TongChiTieu
        WHERE MaKH = @MaKH;

        PRINT N'Cập nhật thông tin khách hàng thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE sp_DeleteKhachHang
    @MaKH VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = @MaKH)
        BEGIN
            RAISERROR(N'Không tìm thấy khách hàng cần xóa!', 16, 1);
            RETURN;
        END
        DELETE FROM KhachHang WHERE MaKH = @MaKH;

        PRINT N'Xóa thông tin khách hàng thành công!';
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 547
        BEGIN
            RAISERROR(N'Không thể xóa khách hàng này vì lịch sử giao dịch (Hóa đơn/Phiếu bảo trì) của họ đang tồn tại trên hệ thống!', 16, 1);
        END
        ELSE
        BEGIN
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
            RAISERROR(@ErrorMessage, 16, 1);
        END
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_LayDanhSachHoaDon
    @TuNgay DATE,
    @DenNgay DATE,
    @TuKhoa NVARCHAR(50)
AS
BEGIN
    SELECT hd.MaHD AS [Mã HĐ], hd.NgayLap AS [Ngày Lập], 
           kh.HoTen AS [Khách Hàng], kh.SoDienThoai AS [SĐT], nv.HoTen AS [Thu Ngân],
           hd.TongTien, hd.GiamGia, hd.ThanhTien, hd.GhiChu,
           CASE WHEN hd.TrangThai = N'Đã thanh toán' OR hd.TrangThai = N'Hoàn thành' THEN N'Hoàn thành' ELSE N'Đã Hủy' END AS [Trạng Thái]
    FROM HoaDon hd
    LEFT JOIN KhachHang kh ON hd.MaKH = kh.MaKH
    LEFT JOIN NhanVien nv ON hd.MaNV = nv.MaNV
    WHERE (hd.MaHD LIKE '%' + @TuKhoa + '%' OR kh.SoDienThoai LIKE '%' + @TuKhoa + '%')
      AND CAST(hd.NgayLap AS DATE) BETWEEN @TuNgay AND @DenNgay
    ORDER BY hd.NgayLap DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_HuyHoaDon
    @MaHD NVARCHAR(20)
AS
BEGIN
    BEGIN TRY
        BEGIN TRAN;
        
        IF (@MaHD IS NULL OR @MaHD = '')
            THROW 51000, N'Hệ thống báo lỗi: Bạn chưa chọn hóa đơn nào để hủy!', 1;
        IF EXISTS (SELECT 1 FROM HoaDon WHERE MaHD = @MaHD AND (TrangThai = N'Đã Hủy' OR TrangThai = N'Đã hủy'))
            THROW 51001, N'Hệ thống báo lỗi: Hóa đơn này đã bị hủy trước đó rồi, không thể hủy thêm!', 1;
        UPDATE HoaDon SET TrangThai = N'Đã Hủy' WHERE MaHD = @MaHD;

        UPDATE ChiTietPhieuNhap
        SET TrangThaiIMEI = N'Trong kho'
        WHERE SoIMEI IN (SELECT SoIMEI FROM ChiTietHoaDon WHERE MaHD = @MaHD);

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        THROW; 
    END CATCH
END
GO 
CREATE OR ALTER PROCEDURE sp_UpdateHoaDon
    @MaHD VARCHAR(20),
    @MaKH VARCHAR(20),
    @MaNV VARCHAR(20),
    @TongTien DECIMAL(18,2),
    @GiamGia DECIMAL(18,2),
    @TrangThai NVARCHAR(50), 
    @GhiChu NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @TongTien IS NULL SET @TongTien = 0;
        IF @GiamGia IS NULL SET @GiamGia = 0;
        
        DECLARE @ThanhTien DECIMAL(18,2) = @TongTien - @GiamGia;
        IF @ThanhTien < 0 SET @ThanhTien = 0;

        UPDATE HoaDon
        SET MaKH = @MaKH, MaNV = @MaNV, TongTien = @TongTien,
            GiamGia = @GiamGia, ThanhTien = @ThanhTien,
            TrangThai = @TrangThai, GhiChu = @GhiChu
        WHERE MaHD = @MaHD;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

--chi tiết hóa đơn

CREATE PROCEDURE sp_InsertChiTietHoaDon
    @MaHD VARCHAR(20),
    @SoIMEI VARCHAR(50),
    @DonGia DECIMAL(18,2),
    @SoLuong INT,
    @MaChiTietSP VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRAN;

        -- 1. Kiểm tra xem hóa đơn gốc có tồn tại không
        IF NOT EXISTS (SELECT 1 FROM HoaDon WHERE MaHD = @MaHD)
        BEGIN
            RAISERROR(N'Lỗi: Mã hóa đơn không tồn tại trên hệ thống!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        IF @SoLuong IS NULL OR @SoLuong <= 0 
            SET @SoLuong = 1;

        INSERT INTO ChiTietHoaDon (MaHD, SoIMEI, DonGia, SoLuong, MaChiTietSP)
        VALUES (@MaHD, @SoIMEI, @DonGia, @SoLuong, @MaChiTietSP);

        UPDATE ChiTietPhieuNhap 
        SET TrangThaiIMEI = N'Đã bán' 
        WHERE SoIMEI = @SoIMEI;

        COMMIT TRAN;
        PRINT N'Thêm chi tiết hóa đơn và cập nhật kho thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO


CREATE PROCEDURE sp_UpdateChiTietHoaDon
    @MaHD VARCHAR(20),
    @SoIMEI VARCHAR(50),
    @DonGia DECIMAL(18,2),
    @SoLuong INT,
    @MaChiTietSP VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM ChiTietHoaDon WHERE MaHD = @MaHD AND SoIMEI = @SoIMEI)
        BEGIN
            RAISERROR(N'Không tìm thấy dòng chi tiết hóa đơn cần cập nhật!', 16, 1);
            RETURN;
        END

        UPDATE ChiTietHoaDon
        SET DonGia = @DonGia,
            SoLuong = @SoLuong,
            MaChiTietSP = @MaChiTietSP
        WHERE MaHD = @MaHD AND SoIMEI = @SoIMEI;

        PRINT N'Cập nhật chi tiết hóa đơn thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO


CREATE PROCEDURE sp_DeleteChiTietHoaDon
    @MaHD VARCHAR(20),
    @SoIMEI VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM ChiTietHoaDon WHERE MaHD = @MaHD AND SoIMEI = @SoIMEI)
        BEGIN
            RAISERROR(N'Không tìm thấy chi tiết hóa đơn cần xóa!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        DELETE FROM ChiTietHoaDon WHERE MaHD = @MaHD AND SoIMEI = @SoIMEI;

        UPDATE ChiTietPhieuNhap 
        SET TrangThaiIMEI = N'Trong kho' 
        WHERE SoIMEI = @SoIMEI;

        COMMIT TRAN;
        PRINT N'Xóa chi tiết hóa đơn và hoàn kho thiết bị thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

--phieunhap
CREATE OR ALTER PROCEDURE sp_InsertPhieuNhap
    @MaPhieu VARCHAR(20),
    @MaHang VARCHAR(20),
    @MaNV VARCHAR(20),
    @NgayNhap DATETIME,
    @TongTien DECIMAL(18,2),
    @TrangThai NVARCHAR(50),
    @GhiChu NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM PhieuNhapHang WHERE MaPhieu = @MaPhieu)
            THROW 51000, N'Hệ thống báo lỗi: Mã phiếu nhập này đã tồn tại!', 1;

        IF @NgayNhap IS NULL 
            SET @NgayNhap = GETDATE();

        IF @TongTien IS NULL SET @TongTien = 0;
        IF @TrangThai IS NULL OR @TrangThai = '' SET @TrangThai = N'Hoàn thành';

        INSERT INTO PhieuNhapHang (MaPhieu, MaHang, MaNV, NgayNhap, TongTien, TrangThai, GhiChu)
        VALUES (@MaPhieu, @MaHang, @MaNV, @NgayNhap, @TongTien, @TrangThai, @GhiChu);
        
    END TRY
    BEGIN CATCH
        THROW; 
    END CATCH
END
GO

=
CREATE OR ALTER PROCEDURE sp_UpdatePhieuNhap
    @MaPhieu VARCHAR(20),
    @MaHang VARCHAR(20),
    @MaNV VARCHAR(20),
    @TongTien DECIMAL(18,2),
    @TrangThai NVARCHAR(50),
    @GhiChu NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM PhieuNhapHang WHERE MaPhieu = @MaPhieu)
            THROW 51001, N'Hệ thống báo lỗi: Không tìm thấy phiếu nhập cần cập nhật!', 1;

        IF @TongTien IS NULL SET @TongTien = 0;

        UPDATE PhieuNhapHang
        SET MaHang = @MaHang,
            MaNV = @MaNV,
            TongTien = @TongTien,
            TrangThai = @TrangThai,
            GhiChu = @GhiChu
        WHERE MaPhieu = @MaPhieu;

    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_HuyPhieuNhap
    @MaPhieu VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM PhieuNhapHang WHERE MaPhieu = @MaPhieu)
            THROW 51002, N'Hệ thống báo lỗi: Không tìm thấy phiếu nhập cần hủy!', 1;

        IF EXISTS (SELECT 1 FROM PhieuNhapHang WHERE MaPhieu = @MaPhieu AND TrangThai = N'Đã hủy')
            THROW 51003, N'Hệ thống báo lỗi: Phiếu nhập hàng này đã bị hủy từ trước, không thể thao tác thêm!', 1;

        UPDATE PhieuNhapHang
        SET TrangThai = N'Đã hủy',
            GhiChu = ISNULL(GhiChu, '') + N' [Hệ thống ghi nhận: Phiếu nhập đã bị hủy]'
        WHERE MaPhieu = @MaPhieu;

        UPDATE ChiTietPhieuNhap
        SET TrangThaiIMEI = N'Lỗi/Đã hủy'
        WHERE MaPhieu = @MaPhieu AND TrangThaiIMEI = N'Trong kho';

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
     
--nhân viên
CREATE PROCEDURE sp_InsertNhanVien
    @MaNV VARCHAR(20),
    @HoTen NVARCHAR(100),
    @GioiTinh NVARCHAR(10),
    @NgaySinh DATE,
    @CMND VARCHAR(20),
    @SoDienThoai VARCHAR(15),
    @Email VARCHAR(100) = NULL,         
    @DiaChi NVARCHAR(200),
    @ChucVu NVARCHAR(50),
    @LuongCoBan DECIMAL(18,2) = 0,       
    @TenDangNhap VARCHAR(50),
    @MatKhau VARCHAR(255),
   @TrangThai BIT = 1 
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = @MaNV)
        BEGIN
            RAISERROR(N'Mã nhân viên này đã tồn tại trên hệ thống!', 16, 1);
            RETURN;
        END
        IF EXISTS (SELECT 1 FROM NhanVien WHERE TenDangNhap = @TenDangNhap)
        BEGIN
            RAISERROR(N'Tên đăng nhập này đã có người sử dụng. Vui lòng chọn tên khác!', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM NhanVien WHERE CMND = @CMND)
        BEGIN
            RAISERROR(N'Số CMND/CCCD này đã được đăng ký cho nhân viên khác!', 16, 1);
            RETURN;
        END

        INSERT INTO NhanVien (MaNV, HoTen, GioiTinh, NgaySinh, CMND, SoDienThoai, Email, DiaChi, ChucVu, LuongCoBan, TenDangNhap, MatKhau, TrangThai)
        VALUES (@MaNV, @HoTen, @GioiTinh, @NgaySinh, @CMND, @SoDienThoai, @Email, @DiaChi, @ChucVu, @LuongCoBan, @TenDangNhap, @MatKhau, @TrangThai);
        
        PRINT N'Thêm mới nhân viên thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE sp_UpdateNhanVien
    @MaNV VARCHAR(20),
    @HoTen NVARCHAR(100),
    @GioiTinh NVARCHAR(10),
    @NgaySinh DATE,
    @CMND VARCHAR(20),
    @SoDienThoai VARCHAR(15),
    @Email VARCHAR(100) = NULL,
    @DiaChi NVARCHAR(200),
    @ChucVu NVARCHAR(50),
    @LuongCoBan DECIMAL(18,2),
    @TenDangNhap VARCHAR(50),
   @TrangThai BIT

AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = @MaNV)
        BEGIN
            RAISERROR(N'Không tìm thấy dữ liệu nhân viên cần cập nhật!', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM NhanVien WHERE TenDangNhap = @TenDangNhap AND MaNV <> @MaNV)
        BEGIN
            RAISERROR(N'Tên đăng nhập mới bị trùng với một nhân viên khác!', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM NhanVien WHERE CMND = @CMND AND MaNV <> @MaNV)
        BEGIN
            RAISERROR(N'Số CMND mới bị trùng với một nhân viên khác!', 16, 1);
            RETURN;
        END

        UPDATE NhanVien
        SET HoTen = @HoTen,
            GioiTinh = @GioiTinh,
            NgaySinh = @NgaySinh,
            CMND = @CMND,
            SoDienThoai = @SoDienThoai,
            Email = @Email,
            DiaChi = @DiaChi,
            ChucVu = @ChucVu,
            LuongCoBan = @LuongCoBan,
            TenDangNhap = @TenDangNhap,
            TrangThai = @TrangThai
        WHERE MaNV = @MaNV;

        PRINT N'Cập nhật thông tin nhân viên thành công!';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE sp_DeleteNhanVien
    @MaNV VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = @MaNV)
        BEGIN
            RAISERROR(N'Không tìm thấy nhân viên cần xóa!', 16, 1);
            RETURN;
        END

        -- Rất rủi ro: Nếu nhân viên là Admin (Chủ cửa hàng) thì cấm xóa
        IF EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = @MaNV AND ChucVu = 'Admin')
        BEGIN
            RAISERROR(N'Hệ thống từ chối xóa tài khoản Admin (Quản trị viên)!', 16, 1);
            RETURN;
        END

        DELETE FROM NhanVien WHERE MaNV = @MaNV;

        PRINT N'Xóa nhân viên thành công!';
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 547
        BEGIN
            RAISERROR(N'KHÔNG THỂ XÓA! Nhân viên này đã từng lập hóa đơn hoặc nhập hàng. Vui lòng chuyển trạng thái thành "Nghỉ việc" thay vì xóa dữ liệu!', 16, 1);
        END
        ELSE
        BEGIN
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
            RAISERROR(@ErrorMessage, 16, 1);
        END
    END CATCH
END
GO


--FUNCTION
CREATE FUNCTION fn_TinhTongChiTieuKhachHang (@MaKH VARCHAR(20))
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TongChiTieu DECIMAL(18,2);

    -- Tính tổng: ThanhTien - GiamGia từ bảng HoaDon theo MaKH
    SELECT @TongChiTieu = ISNULL(SUM(ThanhTien - GiamGia), 0)
    FROM HoaDon
    WHERE MaKH = @MaKH;

    RETURN @TongChiTieu;
END
GO

