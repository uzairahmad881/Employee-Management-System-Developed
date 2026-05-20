using EmployeeManagementSystem;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class EmployeeDashboard : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";
        string loggedInUser = "";
        int loggedInUserID = 0;
        int employeeID = 0;
        bool isDarkMode = false;

        // Dark Colors
        Color darkBg = Color.FromArgb(15, 23, 42);
        Color darkSidebar = Color.FromArgb(20, 30, 55);
        Color darkCard = Color.FromArgb(30, 41, 70);
        Color darkTopBar = Color.FromArgb(20, 30, 55);
        Color darkText = Color.White;
        Color darkSubText = Color.FromArgb(148, 163, 184);

        // Light Colors
        Color lightBg = Color.FromArgb(243, 244, 246);
        Color lightSidebar = Color.FromArgb(30, 58, 138);
        Color lightCard = Color.White;
        Color lightTopBar = Color.White;
        Color lightText = Color.FromArgb(17, 24, 39);
        Color lightSubText = Color.FromArgb(107, 114, 128);

        // Accents
        Color accentBlue = Color.FromArgb(59, 130, 246);
        Color accentGreen = Color.FromArgb(16, 185, 129);
        Color accentOrange = Color.FromArgb(245, 158, 11);
        Color accentPurple = Color.FromArgb(139, 92, 246);
        Color accentRed = Color.FromArgb(239, 68, 68);
        Color accentTeal = Color.FromArgb(20, 184, 166);

        Button activeBtn = null;
        Panel contentPanel, sidebarPanel;

        public EmployeeDashboard(string username, int userID)
        {
            InitializeComponent();
            loggedInUser = username;
            loggedInUserID = userID;
            GetEmployeeID();
            BuildUI();
            LoadStats();
        }

        // ═══════════════════════════════════════════════
        // Employee ID fetch — sirf apna ID milta hai
        // ═══════════════════════════════════════════════
        private void GetEmployeeID()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    // IsActive=1 check — inactive employee login na kar sake
                    SqlCommand cmd = new SqlCommand(
                        "SELECT EmployeeID FROM Employees WHERE UserID=@uid AND IsActive=1", con);
                    cmd.Parameters.AddWithValue("@uid", loggedInUserID);
                    object result = cmd.ExecuteScalar();
                    employeeID = result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch { }
        }

        private void BuildUI()
        {
            this.Controls.Clear();
            this.Text = "Employee Portal - EMS";
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

            // Logo
            Panel logoPanel = new Panel();
            logoPanel.Size = new Size(240, 100);
            logoPanel.Location = new Point(0, 0);
            logoPanel.BackColor = isDarkMode ? Color.FromArgb(10, 18, 40) : Color.FromArgb(23, 45, 115);

            Label lblLogo = new Label();
            lblLogo.Text = "👨‍🔧 My Portal";
            lblLogo.ForeColor = Color.White;
            lblLogo.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblLogo.Dock = DockStyle.Fill;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            logoPanel.Controls.Add(lblLogo);

            // User Info
            Panel userPanel = new Panel();
            userPanel.Size = new Size(240, 90);
            userPanel.Location = new Point(0, 100);
            userPanel.BackColor = isDarkMode ? Color.FromArgb(10, 18, 40) : Color.FromArgb(23, 45, 115);

            Panel avatarCircle = new Panel();
            avatarCircle.Size = new Size(50, 50);
            avatarCircle.Location = new Point(15, 18);
            avatarCircle.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(accentBlue), 0, 0, 49, 49);
                e.Graphics.DrawString(loggedInUser.Substring(0, 1).ToUpper(),
                    new Font("Segoe UI", 22, FontStyle.Bold), Brushes.White, 10, 7);
            };

            Label lblUserName = new Label();
            lblUserName.Text = loggedInUser;
            lblUserName.ForeColor = Color.White;
            lblUserName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblUserName.Location = new Point(75, 20);
            lblUserName.Size = new Size(155, 22);

            Label lblUserRole = new Label();
            lblUserRole.Text = "Employee";
            lblUserRole.ForeColor = Color.FromArgb(147, 197, 253);
            lblUserRole.Font = new Font("Segoe UI", 8);
            lblUserRole.Location = new Point(75, 44);
            lblUserRole.Size = new Size(155, 18);

            userPanel.Controls.AddRange(new Control[] { avatarCircle, lblUserName, lblUserRole });

            // Menu Label
            Label lblMenuTitle = new Label();
            lblMenuTitle.Text = "  MY MENU";
            lblMenuTitle.ForeColor = Color.FromArgb(100, 140, 200);
            lblMenuTitle.Font = new Font("Segoe UI", 7, FontStyle.Bold);
            lblMenuTitle.Location = new Point(0, 205);
            lblMenuTitle.Size = new Size(240, 22);

            // ═══════════════════════════════════════════════
            // SIRF EMPLOYEE MENUS — Admin/HR menus nahi hain
            // ═══════════════════════════════════════════════
            var menus = new (string emoji, string label)[]
            {
                ("🏠", "Dashboard"),
                ("👤", "My Profile"),
                ("📅", "My Attendance"),   // Sirf apni attendance add/dekh sakta hai
                ("⭐", "My Performance"),  // Sirf dekh sakta hai (read-only)
                ("📩", "My Requests"),     // Sirf apni requests submit kar sakta hai
                ("💰", "My Salary"),       // Sirf apni salary dekh sakta hai
                ("🔔", "Notifications")
            };

            int menuY = 228;
            foreach (var m in menus)
            {
                Button btn = new Button();
                btn.Name = "menu_" + m.label;
                btn.Text = $"   {m.emoji}    {m.label}";
                btn.Size = new Size(240, 46);
                btn.Location = new Point(0, menuY);
                btn.BackColor = isDarkMode ? darkSidebar : lightSidebar;
                btn.ForeColor = Color.FromArgb(200, 220, 255);
                btn.Font = new Font("Segoe UI", 10);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Cursor = Cursors.Hand;

                btn.MouseEnter += (s, e) =>
                {
                    if ((Button)s != activeBtn)
                        ((Button)s).BackColor = isDarkMode ? Color.FromArgb(40, 55, 90) : Color.FromArgb(49, 82, 170);
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
                        activeBtn.ForeColor = Color.FromArgb(200, 220, 255);
                    }
                    activeBtn = (Button)s;
                    activeBtn.BackColor = accentBlue;
                    activeBtn.ForeColor = Color.White;
                    HandleMenu(m.label);
                };

                sidebarPanel.Controls.Add(btn);
                menuY += 47;
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
                {
                    this.Hide();
                    new LoginForm().Show();
                }
            };

            sidebarPanel.Controls.AddRange(new Control[] { logoPanel, userPanel, lblMenuTitle, btnLogout });

            // ═══ CONTENT ═══
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = isDarkMode ? darkBg : lightBg;
            contentPanel.AutoScroll = true;

            // Top Bar
            Panel topBarPanel = new Panel();
            topBarPanel.Dock = DockStyle.Top;
            topBarPanel.Height = 65;
            topBarPanel.BackColor = isDarkMode ? darkTopBar : lightTopBar;

            Panel topShadow = new Panel();
            topShadow.Dock = DockStyle.Bottom;
            topShadow.Height = 1;
            topShadow.BackColor = isDarkMode ? Color.FromArgb(40, 55, 90) : Color.FromArgb(229, 231, 235);
            topBarPanel.Controls.Add(topShadow);

            Label lblPageTitle = new Label();
            lblPageTitle.Name = "lblPageTitle";
            lblPageTitle.Text = "My Dashboard";
            lblPageTitle.ForeColor = isDarkMode ? darkText : lightText;
            lblPageTitle.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            lblPageTitle.Location = new Point(25, 15);
            lblPageTitle.Size = new Size(400, 36);
            topBarPanel.Controls.Add(lblPageTitle);

            Label lblDate = new Label();
            lblDate.Text = "📅  " + DateTime.Now.ToString("dddd, dd MMM yyyy");
            lblDate.ForeColor = isDarkMode ? darkSubText : lightSubText;
            lblDate.Font = new Font("Segoe UI", 9);
            lblDate.Location = new Point(600, 22);
            lblDate.Size = new Size(280, 22);
            topBarPanel.Controls.Add(lblDate);

            // Toggle Button
            Button btnToggle = new Button();
            btnToggle.Text = isDarkMode ? "☀️  Light Mode" : "🌙  Dark Mode";
            btnToggle.Size = new Size(130, 36);
            btnToggle.Location = new Point(940, 15);
            btnToggle.BackColor = isDarkMode ? Color.FromArgb(40, 55, 90) : Color.FromArgb(229, 231, 235);
            btnToggle.ForeColor = isDarkMode ? Color.White : lightText;
            btnToggle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnToggle.FlatStyle = FlatStyle.Flat;
            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.Cursor = Cursors.Hand;
            btnToggle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnToggle.Click += (s, e) => { isDarkMode = !isDarkMode; BuildUI(); LoadStats(); };
            topBarPanel.Controls.Add(btnToggle);

            contentPanel.Controls.Add(topBarPanel);

            // ═══ WELCOME BANNER ═══
            Panel banner = new Panel();
            banner.Size = new Size(950, 85);
            banner.Location = new Point(25, 80);
            banner.BackColor = isDarkMode ? Color.FromArgb(23, 45, 115) : accentBlue;

            Label lblWelcome = new Label();
            lblWelcome.Text = $"👋  Welcome, {loggedInUser}!";
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblWelcome.Location = new Point(22, 12);
            lblWelcome.Size = new Size(600, 30);
            banner.Controls.Add(lblWelcome);

            Label lblSub = new Label();
            lblSub.Text = "View your attendance, performance, salary and submit requests here.";
            lblSub.ForeColor = Color.FromArgb(210, 230, 255);
            lblSub.Font = new Font("Segoe UI", 9);
            lblSub.Location = new Point(22, 48);
            lblSub.Size = new Size(700, 22);
            banner.Controls.Add(lblSub);

            // Employee ID badge
            Panel badge = new Panel();
            badge.Size = new Size(140, 50);
            badge.Location = new Point(790, 18);
            badge.BackColor = Color.FromArgb(255, 255, 255, 30);

            Label lblEmpBadge = new Label();
            lblEmpBadge.Text = $"EMP-{employeeID:D4}";
            lblEmpBadge.ForeColor = Color.White;
            lblEmpBadge.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblEmpBadge.Dock = DockStyle.Fill;
            lblEmpBadge.TextAlign = ContentAlignment.MiddleCenter;
            badge.Controls.Add(lblEmpBadge);
            banner.Controls.Add(badge);

            contentPanel.Controls.Add(banner);

            // ═══ STATS SECTION ═══
            Label lblStatsTitle = new Label();
            lblStatsTitle.Text = "📊  My Overview";
            lblStatsTitle.ForeColor = isDarkMode ? darkText : lightText;
            lblStatsTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblStatsTitle.Location = new Point(25, 185);
            lblStatsTitle.Size = new Size(250, 28);
            contentPanel.Controls.Add(lblStatsTitle);

            // ═══ STAT CARDS ═══
            CreateStatCard("lblMyAttendance", "Days Present", "0", "📅", 25, 220, accentBlue);
            CreateStatCard("lblMyRating", "Avg Rating", "0.0", "⭐", 270, 220, accentOrange);
            CreateStatCard("lblMyRequests", "My Requests", "0", "📩", 515, 220, accentPurple);
            CreateStatCard("lblMySalary", "Base Salary", "Rs.0", "💰", 760, 220, accentGreen);

            // ═══ QUICK ACTIONS ═══
            Label lblActTitle = new Label();
            lblActTitle.Text = "⚡  Quick Actions";
            lblActTitle.ForeColor = isDarkMode ? darkText : lightText;
            lblActTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblActTitle.Location = new Point(25, 405);
            lblActTitle.Size = new Size(220, 28);
            contentPanel.Controls.Add(lblActTitle);

            // ═══════════════════════════════════════════════
            // Quick actions — sirf employee ke allowed kaam
            // ═══════════════════════════════════════════════
            CreateActionBtn("📩  Submit Request", 25, 442, accentBlue, () => ShowRequestForm());
            CreateActionBtn("📅  My Attendance", 270, 442, accentGreen, () => ShowMyAttendance());
            CreateActionBtn("⭐  My Performance", 515, 442, accentOrange, () => ShowMyPerformance());
            CreateActionBtn("👤  My Profile", 760, 442, accentPurple, () => ShowMyProfile());

            // ═══ NOTICE BOARD ═══
            Label lblNotice = new Label();
            lblNotice.Text = "📋  Notice Board";
            lblNotice.ForeColor = isDarkMode ? darkText : lightText;
            lblNotice.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblNotice.Location = new Point(25, 550);
            lblNotice.Size = new Size(220, 28);
            contentPanel.Controls.Add(lblNotice);

            Panel noticePanel = new Panel();
            noticePanel.Size = new Size(950, 80);
            noticePanel.Location = new Point(25, 585);
            noticePanel.BackColor = isDarkMode ? darkCard : Color.White;

            string[] notices = {
                "📢  Office timing: 9AM - 5PM",
                "🎉  Salary disbursed on 1st of every month",
                "📝  Performance reviews every quarter",
                "🏖️  Leave requests 3 days in advance"
            };

            int nx = 15;
            foreach (string n in notices)
            {
                Label l = new Label();
                l.Text = n;
                l.ForeColor = isDarkMode ? darkSubText : lightSubText;
                l.Font = new Font("Segoe UI", 8);
                l.Location = new Point(nx, 15);
                l.Size = new Size(220, 50);
                noticePanel.Controls.Add(l);
                nx += 230;
            }
            contentPanel.Controls.Add(noticePanel);

            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebarPanel);
        }

        private void CreateStatCard(string labelName, string title, string value, string icon, int x, int y, Color color)
        {
            Panel card = new Panel();
            card.Size = new Size(220, 160);
            card.Location = new Point(x, y);
            card.BackColor = isDarkMode ? darkCard : lightCard;

            Panel topBar = new Panel();
            topBar.Size = new Size(220, 6);
            topBar.Location = new Point(0, 0);
            topBar.BackColor = color;
            card.Controls.Add(topBar);

            Panel iconBg = new Panel();
            iconBg.Size = new Size(52, 52);
            iconBg.Location = new Point(152, 18);
            iconBg.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(35, color)), 0, 0, 51, 51);
                e.Graphics.DrawString(icon, new Font("Segoe UI", 20),
                    new SolidBrush(color), 5, 8);
            };
            card.Controls.Add(iconBg);

            Label lblVal = new Label();
            lblVal.Name = labelName;
            lblVal.Text = value;
            lblVal.ForeColor = isDarkMode ? darkText : lightText;
            lblVal.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            lblVal.Location = new Point(15, 18);
            lblVal.Size = new Size(140, 52);
            card.Controls.Add(lblVal);

            Label lblT = new Label();
            lblT.Text = title;
            lblT.ForeColor = isDarkMode ? darkSubText : lightSubText;
            lblT.Font = new Font("Segoe UI", 9);
            lblT.Location = new Point(15, 78);
            lblT.Size = new Size(195, 22);
            card.Controls.Add(lblT);

            Panel divider = new Panel();
            divider.Size = new Size(220, 1);
            divider.Location = new Point(0, 110);
            divider.BackColor = isDarkMode ? Color.FromArgb(40, 55, 90) : Color.FromArgb(229, 231, 235);
            card.Controls.Add(divider);

            Label lblLive = new Label();
            lblLive.Text = "● Live";
            lblLive.ForeColor = color;
            lblLive.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblLive.Location = new Point(15, 125);
            lblLive.Size = new Size(80, 20);
            card.Controls.Add(lblLive);

            Panel bottomBar = new Panel();
            bottomBar.Size = new Size(220, 4);
            bottomBar.Location = new Point(0, 156);
            bottomBar.BackColor = color;
            card.Controls.Add(bottomBar);

            contentPanel.Controls.Add(card);
        }

        private void CreateActionBtn(string text, int x, int y, Color color, Action onClick)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(220, 85);
            btn.BackColor = isDarkMode ? darkCard : lightCard;
            btn.ForeColor = isDarkMode ? darkText : lightText;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = color;
            btn.FlatAppearance.BorderSize = 2;
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => { ((Button)s).BackColor = color; ((Button)s).ForeColor = Color.White; };
            btn.MouseLeave += (s, e) => { ((Button)s).BackColor = isDarkMode ? darkCard : lightCard; ((Button)s).ForeColor = isDarkMode ? darkText : lightText; };
            btn.Click += (s, e) => onClick?.Invoke();
            contentPanel.Controls.Add(btn);
        }

        private void HandleMenu(string menu)
        {
            Control[] t = contentPanel.Controls.Find("lblPageTitle", true);
            if (t.Length > 0) t[0].Text = menu;

            switch (menu)
            {
                case "Dashboard": LoadStats(); break;
                case "My Profile": ShowMyProfile(); break;
                case "My Attendance": ShowMyAttendance(); break;
                case "My Performance": ShowMyPerformance(); break;
                case "My Requests": ShowRequestForm(); break;
                case "My Salary": ShowMySalary(); break;
                case "Notifications": ShowNotifications(); break;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // EMPLOYEE CHECK — har form open hone se pehle employeeID check
        // ═══════════════════════════════════════════════════════════════
        private bool CheckEmployeeLinked()
        {
            if (employeeID == 0)
            {
                MessageBox.Show(
                    "Your Employee Profile is not linked!\nPlease contact HR to set up your profile.",
                    "Profile Not Linked",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // ═══════════════════════════════════════════════════════════════
        // MY ATTENDANCE — employeeID pass karo taake sirf apni add kare
        // ═══════════════════════════════════════════════════════════════
        private void ShowMyAttendance()
        {
            if (!CheckEmployeeLinked()) return;
            // EmployeeID pass karo — AttendanceForm mein sirf is ID ka data dikhe
            new AttendanceForm(employeeID).ShowDialog();
        }

        // ═══════════════════════════════════════════════════════════════
        // MY PERFORMANCE — read-only, employeeID pass karo
        // ═══════════════════════════════════════════════════════════════
        private void ShowMyPerformance()
        {
            if (!CheckEmployeeLinked()) return;
            // isReadOnly = true — employee edit/add na kar sake
            new PerformanceForm(employeeID, isReadOnly: true).ShowDialog();
        }

        // ═══════════════════════════════════════════════════════════════
        // MY REQUESTS — employeeID pass karo
        // ═══════════════════════════════════════════════════════════════
        private void ShowRequestForm()
        {
            if (!CheckEmployeeLinked()) return;
            new RequestsForm(employeeID).ShowDialog();
        }

        // ═══════════════════════════════════════════════════════════════
        // MY SALARY — sirf apni salary dekhe, employeeID pass karo
        // ═══════════════════════════════════════════════════════════════
        private void ShowMySalary()
        {
            if (!CheckEmployeeLinked()) return;
            new SalaryForm(employeeID).ShowDialog();
        }

        // ═══════════════════════════════════════════════════════════════
        // MY PROFILE — sirf apna data dikhao (DB se direct)
        // ═══════════════════════════════════════════════════════════════
        private void ShowMyProfile()
        {
            if (!CheckEmployeeLinked()) return;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    // Sirf apna employeeID use karo — kisi aur ka data access nahi
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT e.FirstName, e.LastName, e.Email,
                               e.Phone, d.DepartmentName, e.Position,
                               e.BaseSalary, e.JoinDate
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                        WHERE e.EmployeeID = @id", con);
                    cmd.Parameters.AddWithValue("@id", employeeID);

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        string info =
                            $"👤  Name:          {dr["FirstName"]} {dr["LastName"]}\n\n" +
                            $"📧  Email:          {dr["Email"]}\n\n" +
                            $"📱  Phone:         {dr["Phone"]}\n\n" +
                            $"🏢  Department:  {dr["DepartmentName"]}\n\n" +
                            $"💼  Position:       {dr["Position"]}\n\n" +
                            $"💰  Salary:          Rs. {dr["BaseSalary"]}\n\n" +
                            $"📅  Join Date:     {Convert.ToDateTime(dr["JoinDate"]).ToString("dd MMM yyyy")}";

                        MessageBox.Show(info, "👤 My Profile",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Profile data not found!", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowNotifications()
        {
            string msg =
                "🔔 Notifications:\n\n" +
                "✅  Attendance marked for today\n\n" +
                "📩  Your last request status: Check Requests\n\n" +
                "⭐  Performance review pending\n\n" +
                "💰  Next salary: 1st of next month";
            MessageBox.Show(msg, "Notifications", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ═══════════════════════════════════════════════════════════════
        // STATS — sirf apne employeeID ka data load ho
        // ═══════════════════════════════════════════════════════════════
        private void LoadStats()
        {
            if (employeeID == 0) return;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Attendance — sirf apni Present days
                    SqlCommand c1 = new SqlCommand(
                        "SELECT COUNT(*) FROM Attendance WHERE EmployeeID=@id AND Status='Present'", con);
                    c1.Parameters.AddWithValue("@id", employeeID);
                    UpdateLabel("lblMyAttendance", c1.ExecuteScalar().ToString());

                    // Performance — sirf apni average rating
                    SqlCommand c2 = new SqlCommand(
                        "SELECT ISNULL(AVG(CAST(Rating AS FLOAT)), 0) FROM Performance WHERE EmployeeID=@id", con);
                    c2.Parameters.AddWithValue("@id", employeeID);
                    double avg = Convert.ToDouble(c2.ExecuteScalar());
                    UpdateLabel("lblMyRating", avg.ToString("F1"));

                    // Requests — sirf apni requests count
                    SqlCommand c3 = new SqlCommand(
                        "SELECT COUNT(*) FROM Requests WHERE EmployeeID=@id", con);
                    c3.Parameters.AddWithValue("@id", employeeID);
                    UpdateLabel("lblMyRequests", c3.ExecuteScalar().ToString());

                    // Salary — sirf apni base salary
                    SqlCommand c4 = new SqlCommand(
                        "SELECT ISNULL(BaseSalary, 0) FROM Employees WHERE EmployeeID=@id", con);
                    c4.Parameters.AddWithValue("@id", employeeID);
                    UpdateLabel("lblMySalary", "Rs." + c4.ExecuteScalar().ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stats Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateLabel(string name, string value)
        {
            Control[] c = this.Controls.Find(name, true);
            if (c.Length > 0) c[0].Text = value;
        }

        private void EmployeeDashboard_Load(object sender, EventArgs e)
        {

        }
    }
}