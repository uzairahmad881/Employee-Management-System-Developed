using EmployeeManagementSystem;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class AdminDashboard : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";
        string loggedInUser = "";
        Panel contentPanel;
        Panel sidebarPanel;
        Button activeBtn = null;

        public AdminDashboard(string username)
        {
            InitializeComponent();
            loggedInUser = username;
            CreateAdminUI();
            LoadStats();
        }

        private void CreateAdminUI()
        {
            this.Text = "Employee Management System - Admin";
            this.Size = new Size(1400, 750);
            this.MinimumSize = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(18, 18, 30);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;

            // ═══════ SIDEBAR ═══════
            sidebarPanel = new Panel();
            sidebarPanel.Size = new Size(220, this.Height);
            sidebarPanel.Location = new Point(0, 0);
            sidebarPanel.BackColor = Color.FromArgb(25, 25, 45);
            sidebarPanel.Dock = DockStyle.Left;

            // Logo Area
            Panel logoPanel = new Panel();
            logoPanel.Size = new Size(220, 80);
            logoPanel.Location = new Point(0, 0);
            logoPanel.BackColor = Color.FromArgb(0, 100, 200);

            Label lblLogo = new Label();
            lblLogo.Text = "⚡ EMS Admin";
            lblLogo.ForeColor = Color.White;
            lblLogo.Font = new Font("Arial", 18, FontStyle.Bold);
            lblLogo.Location = new Point(0, 0);
            lblLogo.Size = new Size(220, 80);
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;

            logoPanel.Controls.Add(lblLogo);

            // Admin Info
            Label lblAdminName = new Label();
            lblAdminName.Text = "👑 " + loggedInUser;
            lblAdminName.ForeColor = Color.LightBlue;
            lblAdminName.Font = new Font("Arial", 10, FontStyle.Bold);
            lblAdminName.Location = new Point(0, 90);
            lblAdminName.Size = new Size(220, 30);
            lblAdminName.TextAlign = ContentAlignment.MiddleCenter;

            Label lblRole = new Label();
            lblRole.Text = "Administrator";
            lblRole.ForeColor = Color.Gray;
            lblRole.Font = new Font("Arial", 8);
            lblRole.Location = new Point(0, 118);
            lblRole.Size = new Size(220, 20);
            lblRole.TextAlign = ContentAlignment.MiddleCenter;

            Panel divider = new Panel();
            divider.Size = new Size(180, 1);
            divider.Location = new Point(20, 145);
            divider.BackColor = Color.FromArgb(60, 60, 90);

            string[] menuItems =
            {
                "🏠  Dashboard",
                "👥  Users",
                "🏢  Departments",
                "👨‍💼  Employees",
                "📅  Attendance",
                "⭐  Performance",
                "💰  Salary",
                "📩  Requests",
                "📊  Reports"
            };

            int btnY = 160;

            foreach (string item in menuItems)
            {
                Button btn = new Button();
                btn.Text = item;
                btn.Size = new Size(220, 45);
                btn.Location = new Point(0, btnY);
                btn.BackColor = Color.FromArgb(25, 25, 45);
                btn.ForeColor = Color.White;
                btn.Font = new Font("Arial", 10);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(15, 0, 0, 0);

                btn.MouseEnter += (s, e) =>
                {
                    Button b = (Button)s;

                    if (b != activeBtn)
                        b.BackColor = Color.FromArgb(40, 40, 70);
                };

                btn.MouseLeave += (s, e) =>
                {
                    Button b = (Button)s;

                    if (b != activeBtn)
                        b.BackColor = Color.FromArgb(25, 25, 45);
                };

                string capturedItem = item;

                btn.Click += (s, e) =>
                {
                    if (activeBtn != null)
                    {
                        activeBtn.BackColor = Color.FromArgb(25, 25, 45);
                        activeBtn.ForeColor = Color.White;
                    }

                    activeBtn = (Button)s;
                    activeBtn.BackColor = Color.FromArgb(0, 100, 200);

                    HandleSidebarClick(capturedItem.Trim());
                };

                sidebarPanel.Controls.Add(btn);

                btnY += 46;
            }

            // Logout Button
            Button btnLogout = new Button();
            btnLogout.Text = "🚪  Logout";
            btnLogout.Size = new Size(220, 45);
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.BackColor = Color.FromArgb(180, 30, 30);
            btnLogout.ForeColor = Color.White;
            btnLogout.Font = new Font("Arial", 10, FontStyle.Bold);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;

            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show(
                    "Do you want to Logout?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Hide();
                    new LoginForm().Show();
                }
            };

            sidebarPanel.Controls.Add(btnLogout);
            sidebarPanel.Controls.Add(logoPanel);
            sidebarPanel.Controls.Add(lblAdminName);
            sidebarPanel.Controls.Add(lblRole);
            sidebarPanel.Controls.Add(divider);

            // ═══════ CONTENT AREA ═══════
            contentPanel = new Panel();
            contentPanel.BackColor = Color.FromArgb(18, 18, 30);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.AutoScroll = true;

            // ═══════ TOP BAR ═══════
            Panel topBar = new Panel();
            topBar.Name = "topBar";
            topBar.Height = 60;
            topBar.Dock = DockStyle.Top;
            topBar.BackColor = Color.FromArgb(25, 25, 45);

            Label lblPageTitle = new Label();
            lblPageTitle.Name = "lblPageTitle";
            lblPageTitle.Text = "Dashboard Overview";
            lblPageTitle.ForeColor = Color.White;
            lblPageTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblPageTitle.Location = new Point(20, 15);
            lblPageTitle.Size = new Size(400, 30);

            Label lblDate = new Label();
            lblDate.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            lblDate.ForeColor = Color.Gray;
            lblDate.Font = new Font("Arial", 9);
            lblDate.Location = new Point(850, 20);
            lblDate.Size = new Size(300, 20);

            topBar.Controls.Add(lblPageTitle);
            topBar.Controls.Add(lblDate);

            contentPanel.Controls.Add(topBar);

            // ═══════ STAT CARDS ═══════
            CreateStatCard(
                contentPanel,
                "👥 Total Employees",
                "0",
                20,
                90,
                Color.FromArgb(0, 120, 215),
                "lblEmp");

            CreateStatCard(
                contentPanel,
                "🏢 Departments",
                "0",
                320,
                90,
                Color.FromArgb(16, 150, 16),
                "lblDept");

            CreateStatCard(
                contentPanel,
                "📩 Pending Requests",
                "0",
                620,
                90,
                Color.FromArgb(200, 120, 0),
                "lblReq");

            CreateStatCard(
                contentPanel,
                "👤 Total Users",
                "0",
                920,
                90,
                Color.FromArgb(130, 0, 220),
                "lblUsers");

            // ═══════ QUICK ACTIONS ═══════
            Label lblQuick = new Label();
            lblQuick.Text = "Quick Actions";
            lblQuick.ForeColor = Color.White;
            lblQuick.Font = new Font("Arial", 12, FontStyle.Bold);
            lblQuick.Location = new Point(20, 250);
            lblQuick.Size = new Size(200, 30);

            contentPanel.Controls.Add(lblQuick);

            CreateActionButton(
                contentPanel,
                "➕ Add Employee",
                20,
                290,
                Color.FromArgb(0, 120, 215),
                () => OpenForm("Employees"));

            CreateActionButton(
                contentPanel,
                "📅 Mark Attendance",
                320,
                290,
                Color.FromArgb(16, 150, 16),
                () => OpenForm("Attendance"));

            CreateActionButton(
                contentPanel,
                "⭐ Add Performance",
                620,
                290,
                Color.FromArgb(200, 120, 0),
                () => OpenForm("Performance"));

            CreateActionButton(
                contentPanel,
                "💰 Calculate Salary",
                920,
                290,
                Color.FromArgb(130, 0, 220),
                () => OpenForm("Salary"));

            // ═══════ SYSTEM INFO ═══════
            Label lblRecent = new Label();
            lblRecent.Text = "System Info";
            lblRecent.ForeColor = Color.White;
            lblRecent.Font = new Font("Arial", 12, FontStyle.Bold);
            lblRecent.Location = new Point(20, 410);
            lblRecent.Size = new Size(200, 30);

            contentPanel.Controls.Add(lblRecent);

            Panel infoPanel = new Panel();
            infoPanel.Size = new Size(1180, 200);
            infoPanel.Location = new Point(20, 445);
            infoPanel.BackColor = Color.FromArgb(25, 25, 45);

            Label lblInfo = new Label();
            lblInfo.Text =
                "✅  Database Connected\n\n" +
                "✅  Role-Based Access Active\n\n" +
                "✅  Employee Management System Running\n\n" +
                "📅  Last Login: " +
                DateTime.Now.ToString("hh:mm tt");

            lblInfo.ForeColor = Color.LightGreen;
            lblInfo.Font = new Font("Arial", 10);
            lblInfo.Location = new Point(20, 20);
            lblInfo.Size = new Size(1000, 160);

            infoPanel.Controls.Add(lblInfo);

            contentPanel.Controls.Add(infoPanel);

            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebarPanel);
        }

        // ═══════ CREATE STAT CARD ═══════
        private void CreateStatCard(
            Panel parent,
            string title,
            string value,
            int x,
            int y,
            Color color,
            string labelName)
        {
            Panel card = new Panel();

            // CARD SIZE BARI
            card.Size = new Size(280, 130);

            card.Location = new Point(x, y);
            card.BackColor = color;

            Panel glow = new Panel();
            glow.Size = new Size(280, 5);
            glow.Location = new Point(0, 0);
            glow.BackColor = Color.White;

            card.Controls.Add(glow);

            Label lblT = new Label();
            lblT.Text = title;
            lblT.ForeColor = Color.White;
            lblT.Font = new Font("Arial", 9, FontStyle.Bold);
            lblT.Location = new Point(10, 15);

            // WIDTH BARI
            lblT.Size = new Size(260, 25);

            lblT.TextAlign = ContentAlignment.MiddleCenter;

            Label lblV = new Label();
            lblV.Name = labelName;
            lblV.Text = value;
            lblV.ForeColor = Color.White;
            lblV.Font = new Font("Arial", 32, FontStyle.Bold);
            lblV.Location = new Point(10, 45);

            // WIDTH BARI
            lblV.Size = new Size(260, 65);

            lblV.TextAlign = ContentAlignment.MiddleCenter;

            card.Controls.Add(lblT);
            card.Controls.Add(lblV);

            parent.Controls.Add(card);
        }

        // ═══════ ACTION BUTTON ═══════
        private void CreateActionButton(
            Panel parent,
            string text,
            int x,
            int y,
            Color color,
            Action onClick)
        {
            Button btn = new Button();

            btn.Text = text;
            btn.Location = new Point(x, y);

            // BUTTON SIZE BARI
            btn.Size = new Size(280, 85);

            btn.BackColor = Color.FromArgb(25, 25, 45);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Arial", 10, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;

            btn.FlatAppearance.BorderColor = color;
            btn.FlatAppearance.BorderSize = 2;

            btn.MouseEnter += (s, e) =>
            {
                ((Button)s).BackColor = color;
            };

            btn.MouseLeave += (s, e) =>
            {
                ((Button)s).BackColor = Color.FromArgb(25, 25, 45);
            };

            btn.Click += (s, e) => onClick?.Invoke();

            parent.Controls.Add(btn);
        }

        private void HandleSidebarClick(string menuItem)
        {
            Control[] titleControls =
                contentPanel.Controls.Find("lblPageTitle", true);

            if (titleControls.Length > 0)
            {
                titleControls[0].Text =
                    menuItem.Substring(menuItem.IndexOf(' ') + 1).Trim();
            }

            if (menuItem.Contains("Dashboard"))
                LoadStats();
            else
                OpenForm(GetFormName(menuItem));
        }

        private string GetFormName(string menuItem)
        {
            if (menuItem.Contains("Users")) return "Users";
            if (menuItem.Contains("Departments")) return "Departments";
            if (menuItem.Contains("Employees")) return "Employees";
            if (menuItem.Contains("Attendance")) return "Attendance";
            if (menuItem.Contains("Performance")) return "Performance";
            if (menuItem.Contains("Salary")) return "Salary";
            if (menuItem.Contains("Requests")) return "Requests";
            if (menuItem.Contains("Reports")) return "Reports";

            return "";
        }

        private void OpenForm(string formName)
        {
            try
            {
                switch (formName)
                {
                    case "Employees":
                        new EmployeeForm().ShowDialog();
                        break;

                    case "Users":
                        new UsersForm().ShowDialog();
                        break;

                    case "Departments":
                        new DepartmentsForm().ShowDialog();
                        break;

                    case "Attendance":
                        new AttendanceForm().ShowDialog();
                        break;

                    case "Performance":
                        new PerformanceForm().ShowDialog();
                        break;

                    case "Salary":
                        new SalaryForm().ShowDialog();
                        break;

                    case "Requests":
                        new RequestsForm().ShowDialog();
                        break;

                    case "Reports":
                        new DSADemoForm().ShowDialog();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Form Error: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadStats()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmd1 =
                        new SqlCommand(
                            "SELECT COUNT(*) FROM Employees WHERE IsActive=1",
                            con);

                    UpdateStatLabel(
                        "lblEmp",
                        cmd1.ExecuteScalar().ToString());

                    SqlCommand cmd2 =
                        new SqlCommand(
                            "SELECT COUNT(*) FROM Departments",
                            con);

                    UpdateStatLabel(
                        "lblDept",
                        cmd2.ExecuteScalar().ToString());

                    SqlCommand cmd3 =
                        new SqlCommand(
                            "SELECT COUNT(*) FROM Requests WHERE Status='Pending'",
                            con);

                    UpdateStatLabel(
                        "lblReq",
                        cmd3.ExecuteScalar().ToString());

                    SqlCommand cmd4 =
                        new SqlCommand(
                            "SELECT COUNT(*) FROM Users",
                            con);

                    UpdateStatLabel(
                        "lblUsers",
                        cmd4.ExecuteScalar().ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stats Error: " + ex.Message);
            }
        }

        private void UpdateStatLabel(
            string labelName,
            string value)
        {
            Control[] controls =
                this.Controls.Find(labelName, true);

            if (controls.Length > 0)
                controls[0].Text = value;
        }
    }
}