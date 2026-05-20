using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class SalaryForm : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";

        // ═══════════════════════════════════════════
        // -1 = Admin/HR mode (sab employees)
        // >0 = Employee mode (sirf apni salary)
        // ═══════════════════════════════════════════
        int restrictedEmployeeID = -1;
        bool isEmployeeMode => restrictedEmployeeID > 0;

        Color primary = Color.FromArgb(30, 58, 138);
        Color accent = Color.FromArgb(59, 130, 246);
        Color bgColor = Color.FromArgb(243, 244, 246);
        Color white = Color.White;
        Color textDark = Color.FromArgb(17, 24, 39);
        Color textGray = Color.FromArgb(107, 114, 128);
        Color success = Color.FromArgb(16, 185, 129);
        Color danger = Color.FromArgb(220, 38, 38);
        Color warning = Color.FromArgb(245, 158, 11);

        ComboBox cbEmployee;
        TextBox txtBaseSalary, txtBonus, txtDeduction, txtFinalSalary;
        Label lblAttendance, lblPerformance;
        DataGridView dgvSalary;

        // ── Admin/HR constructor ──
        public SalaryForm()
        {
            InitializeComponent();
            restrictedEmployeeID = -1;
            CreateUI();
            LoadEmployees();
        }

        // ── Employee constructor — sirf apna employeeID pass hoga ──
        public SalaryForm(int employeeID)
        {
            InitializeComponent();
            restrictedEmployeeID = employeeID;
            CreateUI();
            LoadEmployees();
        }

        private void CreateUI()
        {
            this.Text = isEmployeeMode ? "My Salary" : "Salary Management";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = bgColor;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            // TOP BAR
            Panel topBar = new Panel();
            topBar.Dock = DockStyle.Top;
            topBar.Height = 60;
            topBar.BackColor = primary;

            Label lblTitle = new Label();
            lblTitle.Text = isEmployeeMode ? "💰  My Salary" : "💰  Salary Management";
            lblTitle.ForeColor = white;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Size = new Size(400, 30);
            topBar.Controls.Add(lblTitle);
            this.Controls.Add(topBar);

            // LEFT PANEL
            Panel leftPanel = new Panel();
            leftPanel.Size = new Size(350, 580);
            leftPanel.Location = new Point(20, 75);
            leftPanel.BackColor = white;

            Panel formHeader = new Panel();
            formHeader.Size = new Size(350, 45);
            formHeader.Location = new Point(0, 0);
            formHeader.BackColor = isEmployeeMode ? Color.FromArgb(16, 185, 129) : accent;

            Label lblFormTitle = new Label();
            lblFormTitle.Text = isEmployeeMode ? "💰  My Salary Details" : "Calculate Salary";
            lblFormTitle.ForeColor = white;
            lblFormTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblFormTitle.Dock = DockStyle.Fill;
            lblFormTitle.TextAlign = ContentAlignment.MiddleCenter;
            formHeader.Controls.Add(lblFormTitle);
            leftPanel.Controls.Add(formHeader);

            int y = 60;

            // Employee ComboBox
            AddLabel(leftPanel, "Employee *", y);
            cbEmployee = new ComboBox();
            cbEmployee.Location = new Point(20, y + 20);
            cbEmployee.Size = new Size(310, 30);
            cbEmployee.Font = new Font("Segoe UI", 10);
            cbEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEmployee.SelectedIndexChanged += CbEmployee_SelectedIndexChanged;
            // Employee mode: disable — apna naam change na ho sake
            if (isEmployeeMode) { cbEmployee.Enabled = false; cbEmployee.BackColor = bgColor; }
            leftPanel.Controls.Add(cbEmployee);
            y += 65;

            // Base Salary — always read-only (employee change nahi kar sakta)
            AddLabel(leftPanel, "Base Salary (Rs.)", y);
            txtBaseSalary = new TextBox();
            txtBaseSalary.Location = new Point(20, y + 20);
            txtBaseSalary.Size = new Size(310, 30);
            txtBaseSalary.Font = new Font("Segoe UI", 10);
            txtBaseSalary.ReadOnly = true;
            txtBaseSalary.BackColor = bgColor;
            leftPanel.Controls.Add(txtBaseSalary);
            y += 65;

            // Attendance Info
            Panel attPanel = new Panel();
            attPanel.Location = new Point(20, y);
            attPanel.Size = new Size(310, 40);
            attPanel.BackColor = Color.FromArgb(239, 246, 255);
            lblAttendance = new Label();
            lblAttendance.Text = "📅 Attendance: -- days present";
            lblAttendance.ForeColor = accent;
            lblAttendance.Font = new Font("Segoe UI", 9);
            lblAttendance.Dock = DockStyle.Fill;
            lblAttendance.TextAlign = ContentAlignment.MiddleCenter;
            attPanel.Controls.Add(lblAttendance);
            leftPanel.Controls.Add(attPanel);
            y += 50;

            // Performance Info
            Panel perfPanel = new Panel();
            perfPanel.Location = new Point(20, y);
            perfPanel.Size = new Size(310, 40);
            perfPanel.BackColor = Color.FromArgb(240, 253, 244);
            lblPerformance = new Label();
            lblPerformance.Text = "⭐ Performance: -- avg rating";
            lblPerformance.ForeColor = success;
            lblPerformance.Font = new Font("Segoe UI", 9);
            lblPerformance.Dock = DockStyle.Fill;
            lblPerformance.TextAlign = ContentAlignment.MiddleCenter;
            perfPanel.Controls.Add(lblPerformance);
            leftPanel.Controls.Add(perfPanel);
            y += 50;

            // Bonus — employee sirf dekh sakta hai
            AddLabel(leftPanel, "Bonus (Rs.)", y);
            txtBonus = new TextBox();
            txtBonus.Location = new Point(20, y + 20);
            txtBonus.Size = new Size(310, 30);
            txtBonus.Font = new Font("Segoe UI", 10);
            txtBonus.Text = "0";
            if (isEmployeeMode) { txtBonus.ReadOnly = true; txtBonus.BackColor = bgColor; }
            else txtBonus.TextChanged += RecalculateSalary;
            leftPanel.Controls.Add(txtBonus);
            y += 65;

            // Deduction — employee sirf dekh sakta hai
            AddLabel(leftPanel, "Deduction (Rs.)", y);
            txtDeduction = new TextBox();
            txtDeduction.Location = new Point(20, y + 20);
            txtDeduction.Size = new Size(310, 30);
            txtDeduction.Font = new Font("Segoe UI", 10);
            txtDeduction.Text = "0";
            if (isEmployeeMode) { txtDeduction.ReadOnly = true; txtDeduction.BackColor = bgColor; }
            else txtDeduction.TextChanged += RecalculateSalary;
            leftPanel.Controls.Add(txtDeduction);
            y += 65;

            // Final Salary
            Panel finalPanel = new Panel();
            finalPanel.Location = new Point(20, y);
            finalPanel.Size = new Size(310, 55);
            finalPanel.BackColor = Color.FromArgb(16, 185, 129);
            AddLabel(finalPanel, "Final Salary (Rs.)", 3);
            txtFinalSalary = new TextBox();
            txtFinalSalary.Location = new Point(10, 22);
            txtFinalSalary.Size = new Size(290, 28);
            txtFinalSalary.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            txtFinalSalary.ReadOnly = true;
            txtFinalSalary.BackColor = Color.FromArgb(16, 185, 129);
            txtFinalSalary.ForeColor = white;
            txtFinalSalary.BorderStyle = BorderStyle.None;
            txtFinalSalary.TextAlign = HorizontalAlignment.Center;
            finalPanel.Controls.Add(txtFinalSalary);
            leftPanel.Controls.Add(finalPanel);
            y += 65;

            // ═══════════════════════════════════════════
            // Calculate/Save buttons — sirf Admin/HR ke liye
            // Employee mode mein yeh buttons nahi dikhenge
            // ═══════════════════════════════════════════
            if (!isEmployeeMode)
            {
                Button btnCalculate = new Button();
                btnCalculate.Text = "🔄  Auto Calculate";
                btnCalculate.Location = new Point(20, y);
                btnCalculate.Size = new Size(148, 38);
                btnCalculate.BackColor = warning;
                btnCalculate.ForeColor = white;
                btnCalculate.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                btnCalculate.FlatStyle = FlatStyle.Flat;
                btnCalculate.FlatAppearance.BorderSize = 0;
                btnCalculate.Cursor = Cursors.Hand;
                btnCalculate.Click += BtnCalculate_Click;
                leftPanel.Controls.Add(btnCalculate);

                Button btnSave = new Button();
                btnSave.Text = "💾  Save Record";
                btnSave.Location = new Point(182, y);
                btnSave.Size = new Size(148, 38);
                btnSave.BackColor = success;
                btnSave.ForeColor = white;
                btnSave.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                btnSave.FlatStyle = FlatStyle.Flat;
                btnSave.FlatAppearance.BorderSize = 0;
                btnSave.Cursor = Cursors.Hand;
                btnSave.Click += BtnSave_Click;
                leftPanel.Controls.Add(btnSave);
            }
            else
            {
                // ── Employee ke liye info notice ──
                Panel noticePanel = new Panel();
                noticePanel.Location = new Point(20, y);
                noticePanel.Size = new Size(310, 45);
                noticePanel.BackColor = Color.FromArgb(239, 246, 255);

                Label lblNotice = new Label();
                lblNotice.Text = "ℹ️ Salary is calculated by HR.\nContact HR for any queries.";
                lblNotice.ForeColor = accent;
                lblNotice.Font = new Font("Segoe UI", 8);
                lblNotice.Dock = DockStyle.Fill;
                lblNotice.TextAlign = ContentAlignment.MiddleCenter;
                noticePanel.Controls.Add(lblNotice);
                leftPanel.Controls.Add(noticePanel);
            }

            this.Controls.Add(leftPanel);

            // RIGHT PANEL
            Panel rightPanel = new Panel();
            rightPanel.Size = new Size(700, 580);
            rightPanel.Location = new Point(385, 75);
            rightPanel.BackColor = white;

            Panel headerRight = new Panel();
            headerRight.Size = new Size(700, 50);
            headerRight.Location = new Point(0, 0);
            headerRight.BackColor = Color.FromArgb(249, 250, 251);

            Label lblGridTitle = new Label();
            lblGridTitle.Text = isEmployeeMode ? "💰 My Salary Records" : "💰 All Salary Records";
            lblGridTitle.ForeColor = textDark;
            lblGridTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblGridTitle.Location = new Point(15, 13);
            lblGridTitle.Size = new Size(300, 25);
            headerRight.Controls.Add(lblGridTitle);

            Button btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Location = new Point(590, 12);
            btnRefresh.Size = new Size(95, 28);
            btnRefresh.BackColor = accent;
            btnRefresh.ForeColor = white;
            btnRefresh.Font = new Font("Segoe UI", 9);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadSalaryRecords();
            headerRight.Controls.Add(btnRefresh);
            rightPanel.Controls.Add(headerRight);

            dgvSalary = new DataGridView();
            dgvSalary.Location = new Point(0, 50);
            dgvSalary.Size = new Size(700, 530);
            dgvSalary.BackgroundColor = white;
            dgvSalary.BorderStyle = BorderStyle.None;
            dgvSalary.RowHeadersVisible = false;
            dgvSalary.AllowUserToAddRows = false;
            dgvSalary.ReadOnly = true;
            dgvSalary.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSalary.Font = new Font("Segoe UI", 9);
            dgvSalary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSalary.ColumnHeadersDefaultCellStyle.BackColor = primary;
            dgvSalary.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvSalary.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSalary.ColumnHeadersHeight = 38;
            dgvSalary.RowTemplate.Height = 35;
            dgvSalary.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgvSalary.GridColor = Color.FromArgb(229, 231, 235);
            rightPanel.Controls.Add(dgvSalary);

            this.Controls.Add(rightPanel);
            LoadSalaryRecords();
        }

        private void AddLabel(Panel parent, string text, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.ForeColor = textGray;
            lbl.Font = new Font("Segoe UI", 8);
            lbl.Location = new Point(20, y);
            lbl.Size = new Size(310, 18);
            parent.Controls.Add(lbl);
        }

        private void LoadEmployees()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query;
                    SqlDataAdapter da;

                    if (isEmployeeMode)
                    {
                        // ── Sirf apna naam ──
                        query = "SELECT EmployeeID, FirstName + ' ' + LastName AS FullName FROM Employees WHERE EmployeeID=@id AND IsActive=1";
                        da = new SqlDataAdapter(query, con);
                        da.SelectCommand.Parameters.AddWithValue("@id", restrictedEmployeeID);
                    }
                    else
                    {
                        query = "SELECT EmployeeID, FirstName + ' ' + LastName AS FullName FROM Employees WHERE IsActive=1";
                        da = new SqlDataAdapter(query, con);
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cbEmployee.DataSource = dt;
                    cbEmployee.DisplayMember = "FullName";
                    cbEmployee.ValueMember = "EmployeeID";
                }
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        private void CbEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbEmployee.SelectedValue == null) return;
            int empID = Convert.ToInt32(cbEmployee.SelectedValue);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmd1 = new SqlCommand("SELECT BaseSalary FROM Employees WHERE EmployeeID=@id", con);
                    cmd1.Parameters.AddWithValue("@id", empID);
                    object sal = cmd1.ExecuteScalar();
                    txtBaseSalary.Text = sal == DBNull.Value ? "0" : sal.ToString();

                    SqlCommand cmd2 = new SqlCommand(@"SELECT COUNT(*) FROM Attendance
                        WHERE EmployeeID=@id AND Status='Present'
                        AND MONTH(AttendanceDate)=MONTH(GETDATE())
                        AND YEAR(AttendanceDate)=YEAR(GETDATE())", con);
                    cmd2.Parameters.AddWithValue("@id", empID);
                    int present = (int)cmd2.ExecuteScalar();
                    lblAttendance.Text = $"📅 This Month: {present} days present";

                    SqlCommand cmd3 = new SqlCommand("SELECT AVG(CAST(Rating AS FLOAT)) FROM Performance WHERE EmployeeID=@id", con);
                    cmd3.Parameters.AddWithValue("@id", empID);
                    object rating = cmd3.ExecuteScalar();
                    double avgRating = rating == DBNull.Value ? 0 : Convert.ToDouble(rating);
                    lblPerformance.Text = $"⭐ Avg Rating: {avgRating:F1}/5.0";

                    RecalculateSalary(null, null);
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void RecalculateSalary(object sender, EventArgs e)
        {
            try
            {
                decimal base_sal = decimal.TryParse(txtBaseSalary.Text, out decimal b) ? b : 0;
                decimal bonus = decimal.TryParse(txtBonus.Text, out decimal bon) ? bon : 0;
                decimal deduction = decimal.TryParse(txtDeduction.Text, out decimal ded) ? ded : 0;
                txtFinalSalary.Text = "Rs. " + (base_sal + bonus - deduction).ToString("N0");
            }
            catch { }
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (isEmployeeMode) return; // Extra safety
            if (cbEmployee.SelectedValue == null) return;
            int empID = Convert.ToInt32(cbEmployee.SelectedValue);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT AVG(CAST(Rating AS FLOAT)) FROM Performance WHERE EmployeeID=@id", con);
                    cmd.Parameters.AddWithValue("@id", empID);
                    object result = cmd.ExecuteScalar();
                    double avgRating = result == DBNull.Value ? 0 : Convert.ToDouble(result);

                    decimal baseSal = decimal.TryParse(txtBaseSalary.Text, out decimal b) ? b : 0;
                    decimal autoBonus = 0;
                    decimal autoDeduction = 0;

                    if (avgRating >= 4.5) autoBonus = baseSal * 0.20m;
                    else if (avgRating >= 3.5) autoBonus = baseSal * 0.10m;
                    else if (avgRating < 2.0) autoDeduction = baseSal * 0.05m;

                    SqlCommand cmd2 = new SqlCommand(@"SELECT COUNT(*) FROM Attendance
                        WHERE EmployeeID=@id AND Status='Absent'
                        AND MONTH(AttendanceDate)=MONTH(GETDATE())
                        AND YEAR(AttendanceDate)=YEAR(GETDATE())", con);
                    cmd2.Parameters.AddWithValue("@id", empID);
                    int absents = (int)cmd2.ExecuteScalar();
                    autoDeduction += absents * (baseSal / 26);

                    txtBonus.Text = autoBonus.ToString("N0");
                    txtDeduction.Text = autoDeduction.ToString("N0");
                    RecalculateSalary(null, null);

                    MessageBox.Show($"✅ Auto calculation complete!\n\nRating: {avgRating:F1}/5\nBonus: Rs.{autoBonus:N0}\nDeduction: Rs.{autoDeduction:N0}",
                        "Calculation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show("Calc Error: " + ex.Message); }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (isEmployeeMode) return; // Extra safety
            if (cbEmployee.SelectedValue == null)
            { MessageBox.Show("Employee select karo!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            MessageBox.Show("✅ Salary record saved!\n\n" + txtFinalSalary.Text,
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadSalaryRecords();
        }

        private void LoadSalaryRecords()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query;
                    SqlDataAdapter da;

                    if (isEmployeeMode)
                    {
                        // ── Sirf apni salary record ──
                        query = @"SELECT e.EmployeeID,
                                  e.FirstName + ' ' + e.LastName AS Employee,
                                  d.DepartmentName AS Department,
                                  e.BaseSalary,
                                  ISNULL((SELECT AVG(CAST(Rating AS FLOAT))
                                          FROM Performance p WHERE p.EmployeeID = e.EmployeeID), 0) AS AvgRating,
                                  ISNULL((SELECT COUNT(*) FROM Attendance a
                                          WHERE a.EmployeeID = e.EmployeeID
                                          AND Status='Present'
                                          AND MONTH(AttendanceDate)=MONTH(GETDATE())), 0) AS PresentDays
                                  FROM Employees e
                                  LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                                  WHERE e.EmployeeID = @id AND e.IsActive=1";
                        da = new SqlDataAdapter(query, con);
                        da.SelectCommand.Parameters.AddWithValue("@id", restrictedEmployeeID);
                    }
                    else
                    {
                        // ── Admin: sab employees ki salary ──
                        query = @"SELECT e.EmployeeID,
                                  e.FirstName + ' ' + e.LastName AS Employee,
                                  d.DepartmentName AS Department,
                                  e.BaseSalary,
                                  ISNULL((SELECT AVG(CAST(Rating AS FLOAT))
                                          FROM Performance p WHERE p.EmployeeID = e.EmployeeID), 0) AS AvgRating,
                                  ISNULL((SELECT COUNT(*) FROM Attendance a
                                          WHERE a.EmployeeID = e.EmployeeID
                                          AND Status='Present'
                                          AND MONTH(AttendanceDate)=MONTH(GETDATE())), 0) AS PresentDays
                                  FROM Employees e
                                  LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                                  WHERE e.IsActive=1
                                  ORDER BY e.EmployeeID";
                        da = new SqlDataAdapter(query, con);
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvSalary.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        private void SalaryForm_Load(object sender, EventArgs e) { }
    }
}