using EmployeeManagementSystem1;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class LoginForm : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";

        TextBox txtUsername;
        TextBox txtPassword;
        Button btnLogin;
        Label lblStatus;
        System.Windows.Forms.Timer animTimer;
        int animStep = 0;

        Panel leftPanel;
        Panel rightPanel;
        Panel centerBox;

        public LoginForm()
        {
            InitializeComponent();
            BuildUI();
            StartAnimation();
        }

        // ══════════════════════════════════════════════════════
        //  MAIN UI BUILDER
        // ══════════════════════════════════════════════════════
        private void BuildUI()
        {
            this.Text = "EMS — Employee Management System";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(900, 560);
            this.DoubleBuffered = true;

            // ── LEFT PANEL ────────────────────────────────────
            leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = this.Width / 2;
            leftPanel.BackColor = Color.Transparent;
            leftPanel.Paint += LeftPanel_Paint;

            this.SizeChanged += (s, e) =>
            {
                leftPanel.Width = this.ClientSize.Width * 45 / 100;
                PositionLeftContent();
                PositionRightContent();
            };

            // EMS big title
            Label lblBadge = new Label();
            lblBadge.Name = "lblBadge";
            lblBadge.Text = "EMS";
            lblBadge.ForeColor = Color.White;
            lblBadge.Font = new Font("Segoe UI Black", 58, FontStyle.Bold);
            lblBadge.AutoSize = true;
            lblBadge.BackColor = Color.Transparent;

            Label lblSub = new Label();
            lblSub.Name = "lblSub";
            lblSub.Text = "Employee Management System";
            lblSub.ForeColor = Color.FromArgb(220, 235, 255);
            lblSub.Font = new Font("Segoe UI", 12);
            lblSub.AutoSize = true;
            lblSub.BackColor = Color.Transparent;

            Label lblDivider = new Label();
            lblDivider.Name = "lblDivider";
            lblDivider.Text = "━━━━━━━━━━━━━━━━━━━━━━━";
            lblDivider.ForeColor = Color.FromArgb(180, 210, 255);
            lblDivider.Font = new Font("Segoe UI", 9);
            lblDivider.AutoSize = true;
            lblDivider.BackColor = Color.Transparent;

            Label lblWho = new Label();
            lblWho.Name = "lblWho";
            lblWho.Text = "Who can login?";
            lblWho.ForeColor = Color.White;
            lblWho.Font = new Font("Segoe UI Semibold", 13, FontStyle.Bold);
            lblWho.AutoSize = true;
            lblWho.BackColor = Color.Transparent;

            // Role cards — light style
            Panel cardAdmin = MakeRoleCard("👑", "Admin",
                "Full system access. Manage users,\ndepartments, salaries & all reports.",
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(230, 242, 255),
                Color.FromArgb(0, 102, 204));
            cardAdmin.Name = "cardAdmin";

            Panel cardHR = MakeRoleCard("🧑‍💼", "HR Manager",
                "Manage employees, attendance,\nleave requests & performance reviews.",
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(230, 248, 238),
                Color.FromArgb(0, 160, 80));
            cardHR.Name = "cardHR";

            Panel cardEmp = MakeRoleCard("👤", "Employee",
                "View own profile, apply for leave\nand check salary slips.",
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(255, 245, 225),
                Color.FromArgb(210, 120, 0));
            cardEmp.Name = "cardEmp";

            leftPanel.Controls.Add(lblBadge);
            leftPanel.Controls.Add(lblSub);
            leftPanel.Controls.Add(lblDivider);
            leftPanel.Controls.Add(lblWho);
            leftPanel.Controls.Add(cardAdmin);
            leftPanel.Controls.Add(cardHR);
            leftPanel.Controls.Add(cardEmp);

            // ── RIGHT PANEL ───────────────────────────────────
            rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.FromArgb(250, 251, 255);

            // Left shadow line
            Panel shadowBar = new Panel();
            shadowBar.Dock = DockStyle.Left;
            shadowBar.Width = 2;
            shadowBar.BackColor = Color.FromArgb(210, 225, 245);
            rightPanel.Controls.Add(shadowBar);

            // ── CENTER LOGIN CARD ──────────────────────────────
            centerBox = new Panel();
            centerBox.Size = new Size(430, 460);
            centerBox.BackColor = Color.White;

            // Card shadow effect (outer border)
            centerBox.Paint += (s, e) =>
            {
                Rectangle rect = new Rectangle(0, 0, centerBox.Width - 1, centerBox.Height - 1);
                using (Pen pen = new Pen(Color.FromArgb(220, 228, 245), 1.5f))
                    e.Graphics.DrawRectangle(pen, rect);
            };

            // Top blue accent bar inside card
            Panel cardTopBar = new Panel();
            cardTopBar.Dock = DockStyle.Top;
            cardTopBar.Height = 5;
            cardTopBar.BackColor = Color.FromArgb(0, 102, 204);
            centerBox.Controls.Add(cardTopBar);

            // Lock icon + heading
            Label lblLock = new Label();
            lblLock.Text = "🔐";
            lblLock.Font = new Font("Segoe UI Emoji", 22);
            lblLock.Location = new Point(30, 28);
            lblLock.Size = new Size(46, 46);
            lblLock.TextAlign = ContentAlignment.MiddleCenter;
            lblLock.BackColor = Color.Transparent;

            Label lblWelcome = new Label();
            lblWelcome.Text = "Welcome Back";
            lblWelcome.ForeColor = Color.FromArgb(20, 40, 80);
            lblWelcome.Font = new Font("Segoe UI Light", 26);
            lblWelcome.Location = new Point(82, 28);
            lblWelcome.Size = new Size(320, 48);
            lblWelcome.BackColor = Color.Transparent;

            Label lblSignIn = new Label();
            lblSignIn.Text = "Sign in to your account to continue";
            lblSignIn.ForeColor = Color.FromArgb(130, 145, 175);
            lblSignIn.Font = new Font("Segoe UI", 9.5f);
            lblSignIn.Location = new Point(30, 78);
            lblSignIn.Size = new Size(370, 20);
            lblSignIn.BackColor = Color.Transparent;

            // Divider under heading
            Panel headDiv = new Panel();
            headDiv.Size = new Size(370, 1);
            headDiv.Location = new Point(30, 106);
            headDiv.BackColor = Color.FromArgb(230, 235, 245);

            // Username
            Label lblU = MakeFieldLabel("USERNAME", 30, 122);
            Panel userBox = MakeInputBox(30, 144, out txtUsername);
            txtUsername.PlaceholderText = "Enter your username";

            // Password
            Label lblP = MakeFieldLabel("PASSWORD", 30, 210);
            Panel passBox = MakeInputBox(30, 232, out txtPassword);
            txtPassword.PlaceholderText = "Enter your password";
            txtPassword.PasswordChar = '●';

            // Eye toggle
            Button btnToggle = new Button();
            btnToggle.Text = "👁";
            btnToggle.Size = new Size(34, 34);
            btnToggle.Location = new Point(334, 6);
            btnToggle.FlatStyle = FlatStyle.Flat;
            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.BackColor = Color.Transparent;
            btnToggle.ForeColor = Color.FromArgb(160, 175, 200);
            btnToggle.Cursor = Cursors.Hand;
            btnToggle.Font = new Font("Segoe UI Emoji", 13);
            bool showing = false;
            btnToggle.Click += (s, e) =>
            {
                showing = !showing;
                txtPassword.PasswordChar = showing ? '\0' : '●';
                btnToggle.ForeColor = showing
                    ? Color.FromArgb(0, 102, 204)
                    : Color.FromArgb(160, 175, 200);
            };
            passBox.Controls.Add(btnToggle);

            // Status
            lblStatus = new Label();
            lblStatus.Text = "";
            lblStatus.ForeColor = Color.FromArgb(200, 60, 60);
            lblStatus.Font = new Font("Segoe UI", 9);
            lblStatus.Location = new Point(30, 300);
            lblStatus.Size = new Size(370, 22);
            lblStatus.BackColor = Color.Transparent;

            // Sign In button
            btnLogin = new Button();
            btnLogin.Text = "SIGN IN  →";
            btnLogin.Location = new Point(30, 328);
            btnLogin.Size = new Size(370, 52);
            btnLogin.BackColor = Color.FromArgb(0, 102, 204);
            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(0, 80, 180);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(0, 102, 204);
            btnLogin.Click += (s, e) => DoLogin();

            // Keyboard
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) DoLogin(); };

            // Footer
            Label lblFooter = new Label();
            lblFooter.Text = "© 2026 Employee Management System  •  All rights reserved";
            lblFooter.ForeColor = Color.FromArgb(180, 190, 210);
            lblFooter.Font = new Font("Segoe UI", 8);
            lblFooter.Location = new Point(30, 418);
            lblFooter.Size = new Size(370, 20);
            lblFooter.BackColor = Color.Transparent;

            centerBox.Controls.Add(lblLock);
            centerBox.Controls.Add(lblWelcome);
            centerBox.Controls.Add(lblSignIn);
            centerBox.Controls.Add(headDiv);
            centerBox.Controls.Add(lblU);
            centerBox.Controls.Add(userBox);
            centerBox.Controls.Add(lblP);
            centerBox.Controls.Add(passBox);
            centerBox.Controls.Add(lblStatus);
            centerBox.Controls.Add(btnLogin);
            centerBox.Controls.Add(lblFooter);

            rightPanel.Controls.Add(centerBox);

            // Add panels — Fill first, Left after
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);

            PositionLeftContent();
            PositionRightContent();

            rightPanel.SizeChanged += (s, e) => PositionRightContent();
        }

        // ══════════════════════════════════════════════════════
        //  POSITION LEFT CONTENT
        // ══════════════════════════════════════════════════════
        private void PositionLeftContent()
        {
            if (leftPanel == null) return;
            int margin = 55;
            int w = leftPanel.Width - margin * 2;
            int y = 65;

            SetLeft("lblBadge", margin, y, w, 88); y += 96;
            SetLeft("lblSub", margin, y, w, 28); y += 36;
            SetLeft("lblDivider", margin, y, w, 22); y += 30;
            SetLeft("lblWho", margin, y, w, 30); y += 46;
            SetLeft("cardAdmin", margin, y, w, 82); y += 94;
            SetLeft("cardHR", margin, y, w, 82); y += 94;
            SetLeft("cardEmp", margin, y, w, 82);
        }

        private void SetLeft(string name, int x, int y, int w, int h)
        {
            Control[] c = leftPanel.Controls.Find(name, false);
            if (c.Length > 0) { c[0].Location = new Point(x, y); c[0].Size = new Size(w, h); }
        }

        // ══════════════════════════════════════════════════════
        //  POSITION RIGHT CONTENT — center the card
        // ══════════════════════════════════════════════════════
        private void PositionRightContent()
        {
            if (rightPanel == null || centerBox == null) return;
            int cx = (rightPanel.Width - centerBox.Width) / 2 + 8;
            int cy = (rightPanel.Height - centerBox.Height) / 2;
            if (cx < 20) cx = 20;
            if (cy < 20) cy = 20;
            centerBox.Location = new Point(cx, cy);
        }

        // ══════════════════════════════════════════════════════
        //  ROLE CARD — Light Style
        // ══════════════════════════════════════════════════════
        private Panel MakeRoleCard(string icon, string role, string desc,
                                   Color textColor, Color bgColor, Color accentColor)
        {
            Panel card = new Panel();
            card.BackColor = bgColor;

            // Left accent strip
            Panel strip = new Panel();
            strip.Dock = DockStyle.Left;
            strip.Width = 5;
            strip.BackColor = accentColor;
            card.Controls.Add(strip);

            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI Emoji", 20);
            lblIcon.Size = new Size(48, 48);
            lblIcon.Location = new Point(14, 16);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;
            lblIcon.BackColor = Color.Transparent;
            card.Controls.Add(lblIcon);

            Label lblName = new Label();
            lblName.Text = role;
            lblName.ForeColor = Color.FromArgb(25, 40, 80);
            lblName.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            lblName.Location = new Point(70, 14);
            lblName.Size = new Size(260, 24);
            lblName.BackColor = Color.Transparent;
            card.Controls.Add(lblName);

            Label lblDesc = new Label();
            lblDesc.Text = desc;
            lblDesc.ForeColor = Color.FromArgb(100, 120, 155);
            lblDesc.Font = new Font("Segoe UI", 8.5f);
            lblDesc.Location = new Point(70, 38);
            lblDesc.Size = new Size(260, 36);
            lblDesc.BackColor = Color.Transparent;
            card.Controls.Add(lblDesc);

            return card;
        }

        // ══════════════════════════════════════════════════════
        //  FIELD LABEL
        // ══════════════════════════════════════════════════════
        private Label MakeFieldLabel(string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.ForeColor = Color.FromArgb(100, 120, 165);
            lbl.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(370, 18);
            lbl.BackColor = Color.Transparent;
            return lbl;
        }

        // ══════════════════════════════════════════════════════
        //  INPUT BOX — Light Style
        // ══════════════════════════════════════════════════════
        private Panel MakeInputBox(int x, int y, out TextBox txt)
        {
            Panel box = new Panel();
            box.Size = new Size(370, 48);
            box.Location = new Point(x, y);
            box.BackColor = Color.FromArgb(245, 247, 252);

            // Border
            box.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(215, 222, 240), 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, box.Width - 1, box.Height - 1);
            };

            // Bottom focus line
            Panel line = new Panel();
            line.Size = new Size(370, 2);
            line.Location = new Point(0, 46);
            line.BackColor = Color.Transparent;

            txt = new TextBox();
            txt.BorderStyle = BorderStyle.None;
            txt.BackColor = Color.FromArgb(245, 247, 252);
            txt.ForeColor = Color.FromArgb(25, 40, 80);
            txt.Font = new Font("Segoe UI", 12);
            txt.Location = new Point(12, 12);
            txt.Size = new Size(320, 26);

            txt.GotFocus += (s, e) => { line.BackColor = Color.FromArgb(0, 102, 204); box.Invalidate(); };
            txt.LostFocus += (s, e) => { line.BackColor = Color.Transparent; box.Invalidate(); };

            box.Controls.Add(txt);
            box.Controls.Add(line);
            return box;
        }

        // ══════════════════════════════════════════════════════
        //  LEFT PANEL PAINT — Blue gradient
        // ══════════════════════════════════════════════════════
        private void LeftPanel_Paint(object sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Main blue gradient
            using (LinearGradientBrush br = new LinearGradientBrush(
                new Rectangle(0, 0, p.Width, p.Height),
                Color.FromArgb(0, 102, 204),
                Color.FromArgb(0, 55, 140),
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(br, 0, 0, p.Width, p.Height);
            }

            // Subtle light circles
            using (Pen pen1 = new Pen(Color.FromArgb(30, 255, 255, 255), 1.5f))
            using (Pen pen2 = new Pen(Color.FromArgb(18, 255, 255, 255), 1f))
            {
                e.Graphics.DrawEllipse(pen1, -90, -90, 300, 300);
                e.Graphics.DrawEllipse(pen2, p.Width - 160, p.Height - 180, 260, 260);
                e.Graphics.DrawEllipse(pen1, p.Width / 2 - 50, p.Height / 2 - 50, 100, 100);
            }

            // Right edge soft shadow
            using (LinearGradientBrush shadow = new LinearGradientBrush(
                new Rectangle(p.Width - 12, 0, 12, p.Height),
                Color.FromArgb(40, 0, 0, 0),
                Color.Transparent,
                LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(shadow, p.Width - 12, 0, 12, p.Height);
            }
        }

        // ══════════════════════════════════════════════════════
        //  ANIMATION
        // ══════════════════════════════════════════════════════
        private void StartAnimation()
        {
            animTimer = new System.Windows.Forms.Timer();
            animTimer.Interval = 40;
            animTimer.Tick += (s, e) =>
            {
                animStep = (animStep + 1) % 100;
                int blue = 180 + (int)(30 * Math.Sin(animStep * Math.PI / 50));
                if (btnLogin != null && btnLogin.Enabled)
                    btnLogin.BackColor = Color.FromArgb(0, blue < 102 ? 102 : blue, 220);
            };
            animTimer.Start();
        }

        // ══════════════════════════════════════════════════════
        //  LOGIN LOGIC
        // ══════════════════════════════════════════════════════
        private void DoLogin()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowStatus("⚠  Please enter both username and password.", Color.FromArgb(200, 130, 0));
                return;
            }

            btnLogin.Text = "Signing in...";
            btnLogin.Enabled = false;
            lblStatus.Text = "";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"SELECT Role FROM Users
                                     WHERE Username = @u
                                       AND Password  = @p
                                       AND IsActive  = 1";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);

                    string role = cmd.ExecuteScalar()?.ToString();

                    if (role == null)
                    {
                        ShowStatus("✗  Invalid username or password. Please try again.", Color.FromArgb(200, 50, 50));
                        ResetButton();
                        return;
                    }

                    animTimer.Stop();

                    if (role == "Admin")
                    {
                        AdminDashboard ad = new AdminDashboard(username);
                        ad.Show();
                        this.Hide();
                    }
                    else if (role == "HR")
                    {
                        HRDashboard hr = new HRDashboard(username);
                        hr.Show();
                        this.Hide();
                    }
                    else if (role == "Employee")
                    {
                        // Get UserID first
                        SqlCommand cmdID = new SqlCommand("SELECT UserID FROM Users WHERE Username=@u", con);
                        cmdID.Parameters.AddWithValue("@u", username);
                        int userID = Convert.ToInt32(cmdID.ExecuteScalar());

                        EmployeeDashboard ed = new EmployeeDashboard(username, userID);
                        ed.Show();
                        this.Hide();
                    }
                    else
                    {
                        ShowStatus("✗  Role not recognized. Please contact administrator.", Color.FromArgb(200, 50, 50));
                        ResetButton();
                        animTimer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatus("✗  Database Error: " + ex.Message, Color.FromArgb(200, 50, 50));
                ResetButton();
            }
        }

        private void ResetButton()
        {
            btnLogin.Text = "SIGN IN  →";
            btnLogin.Enabled = true;
        }

        private void ShowStatus(string msg, Color color)
        {
            lblStatus.Text = msg;
            lblStatus.ForeColor = color;
        }

        private void LoginForm_Load(object sender, EventArgs e) { }
    }
}