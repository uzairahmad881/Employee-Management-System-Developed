using EmployeeManagementSystem;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class HRDashboard : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";
        string loggedInUser = "";
        Panel contentPanel, sidebarPanel, topBar;
        Button activeBtn = null;
        bool isDarkMode = true;

        // Dark Mode Colors
        Color darkBg = Color.FromArgb(15, 23, 42);
        Color darkSidebar = Color.FromArgb(20, 30, 55);
        Color darkCard = Color.FromArgb(30, 41, 70);
        Color darkTopBar = Color.FromArgb(20, 30, 55);
        Color darkText = Color.White;
        Color darkSubText = Color.FromArgb(148, 163, 184);

        // Light Mode Colors
        Color lightBg = Color.FromArgb(243, 244, 246);
        Color lightSidebar = Color.FromArgb(5, 96, 76);
        Color lightCard = Color.White;
        Color lightTopBar = Color.White;
        Color lightText = Color.FromArgb(17, 24, 39);
        Color lightSubText = Color.FromArgb(107, 114, 128);

        // Accent Colors
        Color accentGreen = Color.FromArgb(16, 185, 129);
        Color accentBlue = Color.FromArgb(59, 130, 246);
        Color accentOrange = Color.FromArgb(245, 158, 11);
        Color accentPurple = Color.FromArgb(139, 92, 246);
        Color accentRed = Color.FromArgb(239, 68, 68);

        public HRDashboard(string username)
        {
            InitializeComponent();
            loggedInUser = username;
            BuildUI();
            LoadStats();
        }

        private void BuildUI()
        {
            this.Text = "HR Dashboard - EMS";
            this.Size = new Size(1200, 720);
            this.MinimumSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = isDarkMode ? darkBg : lightBg;

            // ═══ SIDEBAR ═══
            sidebarPanel = new Panel();
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.Width = 240;
            sidebarPanel.BackColor = isDarkMode ? darkSidebar : lightSidebar;

            // Logo Panel
            Panel logoPanel = new Panel();
            logoPanel.Size = new Size(240, 100);
            logoPanel.Location = new Point(0, 0);
            logoPanel.BackColor = isDarkMode ? Color.FromArgb(10, 18, 40) : Color.FromArgb(2, 70, 55);

            Label lblLogo = new Label();
            lblLogo.Text = "🏥 HR Portal";
            lblLogo.ForeColor = Color.White;
            lblLogo.Font = new Font("Segoe UI", 17, FontStyle.Bold);
            lblLogo.Dock = DockStyle.Fill;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            logoPanel.Controls.Add(lblLogo);

            // User Info
            Panel userPanel = new Panel();
            userPanel.Size = new Size(240, 85);
            userPanel.Location = new Point(0, 100);
            userPanel.BackColor = isDarkMode ? Color.FromArgb(10, 18, 40) : Color.FromArgb(2, 70, 55);

            Panel avatarCircle = new Panel();
            avatarCircle.Size = new Size(48, 48);
            avatarCircle.Location = new Point(18, 18);
            avatarCircle.BackColor = accentGreen;
            avatarCircle.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(Brushes.White, 0, 0, 47, 47);
                e.Graphics.DrawString(loggedInUser.Substring(0, 1).ToUpper(),
                    new Font("Segoe UI", 20, FontStyle.Bold),
                    new SolidBrush(Color.FromArgb(5, 96, 76)), 9, 7);
            };

            Label lblUserName = new Label();
            lblUserName.Text = loggedInUser;
            lblUserName.ForeColor = Color.White;
            lblUserName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblUserName.Location = new Point(76, 20);
            lblUserName.Size = new Size(155, 22);

            Label lblUserRole = new Label();
            lblUserRole.Text = "HR Manager";
            lblUserRole.ForeColor = Color.FromArgb(167, 243, 208);
            lblUserRole.Font = new Font("Segoe UI", 8);
            lblUserRole.Location = new Point(76, 44);
            lblUserRole.Size = new Size(155, 18);

            userPanel.Controls.AddRange(new Control[] { avatarCircle, lblUserName, lblUserRole });

            // Menu Label
            Label lblMenuTitle = new Label();
            lblMenuTitle.Text = "  NAVIGATION";
            lblMenuTitle.ForeColor = Color.FromArgb(100, 160, 130);
            lblMenuTitle.Font = new Font("Segoe UI", 7, FontStyle.Bold);
            lblMenuTitle.Location = new Point(0, 200);
            lblMenuTitle.Size = new Size(240, 22);

            // Menu Items
            var menus = new (string emoji, string label)[]
            {
                ("🏠", "Dashboard"),
                ("👨‍💼", "Employees"),
                ("📅", "Attendance"),
                ("⭐", "Performance"),
                ("📩", "Requests"),
                ("💰", "Salary")
            };

            int menuY = 225;
            foreach (var m in menus)
            {
                Button btn = new Button();
                btn.Name = "menu_" + m.label;
                btn.Text = $"   {m.emoji}    {m.label}";
                btn.Size = new Size(240, 48);
                btn.Location = new Point(0, menuY);
                btn.BackColor = isDarkMode ? darkSidebar : lightSidebar;
                btn.ForeColor = Color.FromArgb(200, 240, 220);
                btn.Font = new Font("Segoe UI", 10);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Cursor = Cursors.Hand;

                btn.MouseEnter += (s, e) =>
                {
                    if ((Button)s != activeBtn)
                        ((Button)s).BackColor = isDarkMode ? Color.FromArgb(40, 55, 90) : Color.FromArgb(4, 120, 95);
                };
                btn.MouseLeave += (s, e) =>
                {
                    if ((Button)s != activeBtn)
                        ((Button)s).BackColor = isDarkMode ? darkSidebar : lightSidebar;
                };
                btn.Click += (s, e) =>
                {
                    if (activeBtn != null)
                    {
                        activeBtn.BackColor = isDarkMode ? darkSidebar : lightSidebar;
                        activeBtn.ForeColor = Color.FromArgb(200, 240, 220);
                    }
                    activeBtn = (Button)s;
                    activeBtn.BackColor = accentGreen;
                    activeBtn.ForeColor = Color.White;
                    HandleMenu(m.label);
                };

                sidebarPanel.Controls.Add(btn);
                menuY += 49;
            }

            // Logout
            Button btnLogout = new Button();
            btnLogout.Text = "   🚪    Logout";
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Height = 52;
            btnLogout.BackColor = accentRed;
            btnLogout.ForeColor = Color.White;
            btnLogout.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Are you sure you want to logout?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                { this.Hide(); new LoginForm().Show(); }
            };

            sidebarPanel.Controls.AddRange(new Control[] { logoPanel, userPanel, lblMenuTitle, btnLogout });

            // ═══ CONTENT ═══
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = isDarkMode ? darkBg : lightBg;
            contentPanel.AutoScroll = true;

            // Top Bar
            topBar = new Panel();
            topBar.Name = "topBar";
            topBar.Dock = DockStyle.Top;
            topBar.Height = 68;
            topBar.BackColor = isDarkMode ? darkTopBar : lightTopBar;
            topBar.Padding = new Padding(25, 0, 25, 0);

            Panel topShadow = new Panel();
            topShadow.Dock = DockStyle.Bottom;
            topShadow.Height = 1;
            topShadow.BackColor = isDarkMode ? Color.FromArgb(40, 55, 90) : Color.FromArgb(229, 231, 235);
            topBar.Controls.Add(topShadow);

            Label lblPageTitle = new Label();
            lblPageTitle.Name = "lblPageTitle";
            lblPageTitle.Text = "HR Dashboard";
            lblPageTitle.ForeColor = isDarkMode ? darkText : lightText;
            lblPageTitle.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            lblPageTitle.Location = new Point(25, 16);
            lblPageTitle.Size = new Size(400, 36);
            topBar.Controls.Add(lblPageTitle);

            // Date
            Label lblDate = new Label();
            lblDate.Text = "📅  " + DateTime.Now.ToString("ddd, dd MMM yyyy");
            lblDate.ForeColor = isDarkMode ? darkSubText : lightSubText;
            lblDate.Font = new Font("Segoe UI", 9);
            lblDate.Location = new Point(700, 24);
            lblDate.Size = new Size(220, 22);
            topBar.Controls.Add(lblDate);

            // 🌙 Dark/Light Toggle Button
            Button btnToggle = new Button();
            btnToggle.Name = "btnToggle";
            btnToggle.Text = isDarkMode ? "☀️  Light Mode" : "🌙  Dark Mode";
            btnToggle.Size = new Size(130, 36);
            btnToggle.Location = new Point(930, 16);
            btnToggle.BackColor = isDarkMode ? Color.FromArgb(40, 55, 90) : Color.FromArgb(229, 231, 235);
            btnToggle.ForeColor = isDarkMode ? Color.White : Color.FromArgb(17, 24, 39);
            btnToggle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnToggle.FlatStyle = FlatStyle.Flat;
            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.Cursor = Cursors.Hand;
            btnToggle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnToggle.Click += (s, e) =>
            {
                isDarkMode = !isDarkMode;
                this.Controls.Clear();
                BuildUI();
                LoadStats();
            };
            topBar.Controls.Add(btnToggle);

            contentPanel.Controls.Add(topBar);

            // ═══ WELCOME BANNER ═══
            Panel banner = new Panel();
            banner.Name = "banner";
            banner.Size = new Size(900, 80);
            banner.Location = new Point(25, 85);
            banner.BackColor = isDarkMode ? Color.FromArgb(5, 96, 76) : accentGreen;

            Label lblWelcome = new Label();
            lblWelcome.Text = $"👋  Welcome back, {loggedInUser}!";
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblWelcome.Location = new Point(22, 12);
            lblWelcome.Size = new Size(600, 28);
            banner.Controls.Add(lblWelcome);

            Label lblSub = new Label();
            lblSub.Text = "Manage your team efficiently — employees, attendance, performance & requests.";
            lblSub.ForeColor = Color.FromArgb(210, 255, 235);
            lblSub.Font = new Font("Segoe UI", 9);
            lblSub.Location = new Point(22, 44);
            lblSub.Size = new Size(700, 22);
            banner.Controls.Add(lblSub);

            contentPanel.Controls.Add(banner);

            // ═══ STATS LABEL ═══
            Label lblStatsTitle = new Label();
            lblStatsTitle.Text = "📊  Overview";
            lblStatsTitle.ForeColor = isDarkMode ? darkText : lightText;
            lblStatsTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblStatsTitle.Location = new Point(25, 185);
            lblStatsTitle.Size = new Size(200, 28);
            contentPanel.Controls.Add(lblStatsTitle);

            // ═══ STAT CARDS ═══
            CreateStatCard("lblEmp", "Total Employees", "0", "👥", 25, 220, accentBlue);
            CreateStatCard("lblPresent", "Present Today", "0", "✅", 255, 220, accentGreen);
            CreateStatCard("lblReq", "Pending Requests", "0", "📩", 485, 220, accentOrange);
            CreateStatCard("lblPerf", "Avg Performance", "0.0", "⭐", 715, 220, accentPurple);

            // ═══ QUICK ACTIONS LABEL ═══
            Label lblActTitle = new Label();
            lblActTitle.Text = "⚡  Quick Actions";
            lblActTitle.ForeColor = isDarkMode ? darkText : lightText;
            lblActTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblActTitle.Location = new Point(25, 400);
            lblActTitle.Size = new Size(220, 28);
            contentPanel.Controls.Add(lblActTitle);

            // ═══ ACTION BUTTONS ═══
            CreateActionBtn("👨‍💼  Employees", 25, 438, accentBlue, () => new EmployeeForm().ShowDialog());
            CreateActionBtn("📅  Attendance", 255, 438, accentGreen, () => new AttendanceForm().ShowDialog());
            CreateActionBtn("⭐  Performance", 485, 438, accentOrange, () => new PerformanceForm().ShowDialog());
            CreateActionBtn("📩  Requests", 715, 438, accentPurple, () => new RequestsForm().ShowDialog());

            // ═══ STATUS PANEL ═══
            Label lblStatusTitle = new Label();
            lblStatusTitle.Text = "🖥️  System Status";
            lblStatusTitle.ForeColor = isDarkMode ? darkText : lightText;
            lblStatusTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblStatusTitle.Location = new Point(25, 555);
            lblStatusTitle.Size = new Size(220, 28);
            contentPanel.Controls.Add(lblStatusTitle);

            Panel statusPanel = new Panel();
            statusPanel.Size = new Size(900, 70);
            statusPanel.Location = new Point(25, 590);
            statusPanel.BackColor = isDarkMode ? darkCard : Color.White;

            string[] statuses = {
                "✅  DB Connected",
                "✅  Role: HR",
                "✅  System Active",
                "🕐  " + DateTime.Now.ToString("hh:mm tt")
            };

            int sx = 20;
            foreach (string st in statuses)
            {
                Label l = new Label();
                l.Text = st;
                l.ForeColor = accentGreen;
                l.Font = new Font("Segoe UI", 9);
                l.Location = new Point(sx, 22);
                l.Size = new Size(200, 25);
                statusPanel.Controls.Add(l);
                sx += 215;
            }
            contentPanel.Controls.Add(statusPanel);

            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebarPanel);
        }

        private void CreateStatCard(string labelName, string title, string value, string icon, int x, int y, Color color)
        {
            Panel card = new Panel();
            card.Size = new Size(210, 155);
            card.Location = new Point(x, y);
            card.BackColor = isDarkMode ? darkCard : Color.White;
            card.Cursor = Cursors.Hand;

            // Top color bar
            Panel topColor = new Panel();
            topColor.Size = new Size(210, 5);
            topColor.Location = new Point(0, 0);
            topColor.BackColor = color;
            card.Controls.Add(topColor);

            // Icon circle
            Panel iconCircle = new Panel();
            iconCircle.Size = new Size(50, 50);
            iconCircle.Location = new Point(145, 20);
            iconCircle.BackColor = Color.FromArgb(isDarkMode ? 40 : 230,
                color.R / 4, color.G / 4, color.B / 4);
            iconCircle.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(30, color)), 0, 0, 49, 49);
                e.Graphics.DrawString(icon, new Font("Segoe UI", 18),
                    new SolidBrush(color), 5, 8);
            };
            card.Controls.Add(iconCircle);

            // Value
            Label lblVal = new Label();
            lblVal.Name = labelName;
            lblVal.Text = value;
            lblVal.ForeColor = isDarkMode ? Color.White : Color.FromArgb(17, 24, 39);
            lblVal.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblVal.Location = new Point(15, 20);
            lblVal.Size = new Size(130, 55);
            card.Controls.Add(lblVal);

            // Title
            Label lblT = new Label();
            lblT.Text = title;
            lblT.ForeColor = isDarkMode ? darkSubText : lightSubText;
            lblT.Font = new Font("Segoe UI", 9);
            lblT.Location = new Point(15, 80);
            lblT.Size = new Size(185, 22);
            card.Controls.Add(lblT);

            // Divider
            Panel divider = new Panel();
            divider.Size = new Size(210, 1);
            divider.Location = new Point(0, 112);
            divider.BackColor = isDarkMode ? Color.FromArgb(40, 55, 90) : Color.FromArgb(229, 231, 235);
            card.Controls.Add(divider);

            // Bottom tag
            Label lblTag = new Label();
            lblTag.Text = "● Live Data";
            lblTag.ForeColor = color;
            lblTag.Font = new Font("Segoe UI", 8);
            lblTag.Location = new Point(15, 125);
            lblTag.Size = new Size(150, 20);
            card.Controls.Add(lblTag);

            // Bottom color bar
            Panel bottomBar = new Panel();
            bottomBar.Size = new Size(210, 3);
            bottomBar.Location = new Point(0, 152);
            bottomBar.BackColor = color;
            card.Controls.Add(bottomBar);

            contentPanel.Controls.Add(card);
        }

        private void CreateActionBtn(string text, int x, int y, Color color, Action onClick)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(210, 80);
            btn.BackColor = isDarkMode ? darkCard : Color.White;
            btn.ForeColor = isDarkMode ? Color.White : Color.FromArgb(17, 24, 39);
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = color;
            btn.FlatAppearance.BorderSize = 2;
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => { ((Button)s).BackColor = color; ((Button)s).ForeColor = Color.White; };
            btn.MouseLeave += (s, e) =>
            {
                ((Button)s).BackColor = isDarkMode ? darkCard : Color.White;
                ((Button)s).ForeColor = isDarkMode ? Color.White : Color.FromArgb(17, 24, 39);
            };
            btn.Click += (s, e) => onClick?.Invoke();
            contentPanel.Controls.Add(btn);
        }

        private void HandleMenu(string menu)
        {
            Control[] titleControls = contentPanel.Controls.Find("lblPageTitle", true);
            if (titleControls.Length > 0) titleControls[0].Text = menu;
            if (menu == "Dashboard") LoadStats();
            else
            {
                switch (menu)
                {
                    case "Employees": new EmployeeForm().ShowDialog(); break;
                    case "Attendance": new AttendanceForm().ShowDialog(); break;
                    case "Performance": new PerformanceForm().ShowDialog(); break;
                    case "Requests": new RequestsForm().ShowDialog(); break;
                    case "Salary": new SalaryForm().ShowDialog(); break;
                }
            }
        }

        private void LoadStats()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand c1 = new SqlCommand("SELECT COUNT(*) FROM Employees WHERE IsActive=1", con);
                    UpdateLabel("lblEmp", c1.ExecuteScalar().ToString());

                    SqlCommand c2 = new SqlCommand("SELECT COUNT(*) FROM Attendance WHERE AttendanceDate=CAST(GETDATE() AS DATE) AND Status='Present'", con);
                    UpdateLabel("lblPresent", c2.ExecuteScalar().ToString());

                    SqlCommand c3 = new SqlCommand("SELECT COUNT(*) FROM Requests WHERE Status='Pending'", con);
                    UpdateLabel("lblReq", c3.ExecuteScalar().ToString());

                    SqlCommand c4 = new SqlCommand("SELECT ISNULL(AVG(CAST(Rating AS FLOAT)),0) FROM Performance", con);
                    double avg = Convert.ToDouble(c4.ExecuteScalar());
                    UpdateLabel("lblPerf", avg.ToString("F1"));
                }
            }
            catch (Exception ex) { MessageBox.Show("Stats Error: " + ex.Message); }
        }

        private void UpdateLabel(string name, string value)
        {
            Control[] c = this.Controls.Find(name, true);
            if (c.Length > 0) c[0].Text = value;
        }

        private void HRDashboard_Load(object sender, EventArgs e)
        {

        }
    }
}