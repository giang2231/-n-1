namespace Đồ_án_quản_lý_cửa_hàng_điện_thoại
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlMain = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtUser = new Guna.UI2.WinForms.Guna2TextBox();
            txtPass = new Guna.UI2.WinForms.Guna2TextBox();
            chkShowPass = new Guna.UI2.WinForms.Guna2CheckBox();
            btnDangNhap = new Guna.UI2.WinForms.Guna2Button();
            btnThoat = new Guna.UI2.WinForms.Guna2Button();
            guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(components);
            guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(components);
            pnlMain.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.BackColor = Color.Transparent;
            pnlMain.BorderRadius = 25;
            pnlMain.Controls.Add(lblTitle);
            pnlMain.Controls.Add(txtUser);
            pnlMain.Controls.Add(txtPass);
            pnlMain.Controls.Add(chkShowPass);
            pnlMain.Controls.Add(btnDangNhap);
            pnlMain.Controls.Add(btnThoat);
            pnlMain.CustomizableEdges = customizableEdges9;
            pnlMain.FillColor = Color.FromArgb(180, 255, 255, 255);
            pnlMain.Location = new Point(35, 30);
            pnlMain.Name = "pnlMain";
            pnlMain.ShadowDecoration.CustomizableEdges = customizableEdges10;
            pnlMain.Size = new Size(350, 360);
            pnlMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(41, 128, 185);
            lblTitle.Location = new Point(85, 30);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(132, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "ĐĂNG NHẬP";
            // 
            // txtUser
            // 
            txtUser.BorderRadius = 15;
            txtUser.CustomizableEdges = customizableEdges1;
            txtUser.DefaultText = "";
            txtUser.Font = new Font("Segoe UI", 11F);
            txtUser.Location = new Point(25, 100);
            txtUser.Name = "txtUser";
            txtUser.PlaceholderText = "Tên đăng nhập";
            txtUser.SelectedText = "";
            txtUser.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtUser.Size = new Size(300, 40);
            txtUser.TabIndex = 1;
            // 
            // txtPass
            // 
            txtPass.BorderRadius = 15;
            txtPass.CustomizableEdges = customizableEdges3;
            txtPass.DefaultText = "";
            txtPass.Font = new Font("Segoe UI", 11F);
            txtPass.Location = new Point(25, 175);
            txtPass.Name = "txtPass";
            txtPass.PasswordChar = '●';
            txtPass.PlaceholderText = "Mật khẩu";
            txtPass.SelectedText = "";
            txtPass.ShadowDecoration.CustomizableEdges = customizableEdges4;
            txtPass.Size = new Size(300, 40);
            txtPass.TabIndex = 2;
            // 
            // chkShowPass
            // 
            chkShowPass.AutoSize = true;
            chkShowPass.CheckedState.BorderRadius = 0;
            chkShowPass.CheckedState.BorderThickness = 0;
            chkShowPass.Location = new Point(25, 225);
            chkShowPass.Name = "chkShowPass";
            chkShowPass.Size = new Size(104, 19);
            chkShowPass.TabIndex = 3;
            chkShowPass.Text = "Hiện mật khẩu";
            chkShowPass.UncheckedState.BorderRadius = 0;
            chkShowPass.UncheckedState.BorderThickness = 0;
            chkShowPass.CheckedChanged += chkShowPass_CheckedChanged;
            // 
            // btnDangNhap
            // 
            btnDangNhap.BorderRadius = 15;
            btnDangNhap.CustomizableEdges = customizableEdges5;
            btnDangNhap.FillColor = Color.FromArgb(41, 128, 185);
            btnDangNhap.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnDangNhap.ForeColor = Color.White;
            btnDangNhap.Location = new Point(25, 260);
            btnDangNhap.Name = "btnDangNhap";
            btnDangNhap.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnDangNhap.Size = new Size(300, 40);
            btnDangNhap.TabIndex = 3;
            btnDangNhap.Text = "ĐĂNG NHẬP";
            btnDangNhap.Click += btnDangNhap_Click;
            // 
            // btnThoat
            // 
            btnThoat.BorderRadius = 15;
            btnThoat.CustomizableEdges = customizableEdges7;
            btnThoat.FillColor = Color.FromArgb(231, 76, 60);
            btnThoat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnThoat.ForeColor = Color.White;
            btnThoat.Location = new Point(135, 315);
            btnThoat.Name = "btnThoat";
            btnThoat.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnThoat.Size = new Size(80, 30);
            btnThoat.TabIndex = 4;
            btnThoat.Text = "THOÁT";
            btnThoat.Click += btnThoat_Click;
            // 
            // guna2Elipse1
            // 
            guna2Elipse1.BorderRadius = 30;
            guna2Elipse1.TargetControl = this;
            // 
            // guna2DragControl1
            // 
            guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            guna2DragControl1.TargetControl = this;
            guna2DragControl1.UseTransparentDrag = true;
            // 
            // Form1
            // 
            ClientSize = new Size(420, 420);
            Controls.Add(pnlMain);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Load += Form1_Load;
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlMain;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtUser;
        private Guna.UI2.WinForms.Guna2TextBox txtPass;
        private Guna.UI2.WinForms.Guna2CheckBox chkShowPass;
        private Guna.UI2.WinForms.Guna2Button btnDangNhap;
        private Guna.UI2.WinForms.Guna2Button btnThoat;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
    }
}