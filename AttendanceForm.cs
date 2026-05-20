using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class AttendanceForm : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";

        // ═══════════════════════════════════════════
        // -1 = Admin/HR mode (sab employees)
        // >0 = Employee mode (sirf apni attendance)
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

        ComboBox cbEmployee, cbStatus;
        DateTimePicker dtpDate;
        DataGridView dgvAttendance;

        // ── Admin/HR constructor ──
        public AttendanceForm()
        {
            InitializeComponent();
            restrictedEmployeeID = -1;
            CreateUI();
            LoadEmployees();
            LoadAttendance();
        }

        // ── Employee constructor — EmployeeDashboard se yahi call hota hai ──
        // sirf employeeID pass hota hai, role ki zaroorat nahi
        public AttendanceForm(int employeeID)
        {
            InitializeComponent();
            restrictedEmployeeID = employeeID;
            CreateUI();
            LoadEmployees();
            LoadAttendance();
        }

        private void CreateUI()
        {
            this.Text = isEmployeeMode ? "My Attendance" : "Attendance Management";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = bgColor;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            // ═══ TOP BAR ═══
            Panel topBar = new Panel();
            topBar.Dock = DockStyle.Top;
            topBar.Height = 60;
            topBar.BackColor = primary;

            Label lblTitle = new Label();
            lblTitle.Text = isEmployeeMode ? "📅  My Attendance" : "📅  Attendance Management";
            lblTitle.ForeColor = white;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Size = new Size(400, 30);
            topBar.Controls.Add(lblTitle);
            this.Controls.Add(topBar);

            // ═══ LEFT PANEL ═══
            Panel leftPanel = new Panel();
            leftPanel.Size = new Size(350, 580);
            leftPanel.Location = new Point(20, 75);
            leftPanel.BackColor = white;

            Panel formHeader = new Panel();
            formHeader.Size = new Size(350, 45);
            formHeader.Location = new Point(0, 0);
            formHeader.BackColor = isEmployeeMode ? success : accent;

            Label lblFormTitle = new Label();
            lblFormTitle.Text = isEmployeeMode ? "✅  Mark My Attendance" : "Mark Attendance";
            lblFormTitle.ForeColor = white;
            lblFormTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblFormTitle.Dock = DockStyle.Fill;
            lblFormTitle.TextAlign = ContentAlignment.MiddleCenter;
            formHeader.Controls.Add(lblFormTitle);
            leftPanel.Controls.Add(formHeader);

            int y = 60;

            // ═══════════════════════════════════════════
            // Employee ComboBox:
            // Employee mode: disabled — sirf apna naam
            // Admin mode: sab employees
            // ═══════════════════════════════════════════
            AddLabel(leftPanel, "Employee *", y);
            cbEmployee = new ComboBox();
            cbEmployee.Location = new Point(20, y + 20);
            cbEmployee.Size = new Size(310, 30);
            cbEmployee.Font = new Font("Segoe UI", 10);
            cbEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEmployee.BackColor = white;
            if (isEmployeeMode)
            {
                cbEmployee.Enabled = false; // Employee apna naam change na kar sake
                cbEmployee.BackColor = bgColor;
            }
            leftPanel.Controls.Add(cbEmployee);
            y += 65;

            // Date picker
            AddLabel(leftPanel, "Date *", y);
            dtpDate = new DateTimePicker();
            dtpDate.Location = new Point(20, y + 20);
            dtpDate.Size = new Size(310, 30);
            dtpDate.Font = new Font("Segoe UI", 10);
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Value = DateTime.Today;
            // ── Employee sirf aaj ki attendance mark kare ──
            if (isEmployeeMode)
            {
                dtpDate.Enabled = false; // Pichli date set na ho sake
                dtpDate.Value = DateTime.Today;
            }
            leftPanel.Controls.Add(dtpDate);
            y += 65;

            // Status
            AddLabel(leftPanel, "Status *", y);
            cbStatus = new ComboBox();
            cbStatus.Location = new Point(20, y + 20);
            cbStatus.Size = new Size(310, 30);
            cbStatus.Font = new Font("Segoe UI", 10);
            cbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            // ═══════════════════════════════════════════
            // Employee sirf Present/Leave mark kar sakta hai
            // Absent/Late — HR/Admin set karta hai
            // ═══════════════════════════════════════════
            if (isEmployeeMode)
                cbStatus.Items.AddRange(new string[] { "Present", "Leave" });
            else
                cbStatus.Items.AddRange(new string[] { "Present", "Absent", "Late", "Leave" });
            cbStatus.SelectedIndex = 0;
            cbStatus.BackColor = white;
            leftPanel.Controls.Add(cbStatus);
            y += 65;

            // Save Button
            Button btnSave = new Button();
            btnSave.Text = isEmployeeMode ? "✅ Mark My Attendance" : "✅ Save Attendance";
            btnSave.Location = new Point(20, y);
            btnSave.Size = new Size(310, 45);
            btnSave.BackColor = success;
            btnSave.ForeColor = white;
            btnSave.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;
            leftPanel.Controls.Add(btnSave);
            y += 55;

            // Clear Button
            Button btnClear = new Button();
            btnClear.Text = "🔄 Clear";
            btnClear.Location = new Point(20, y);
            btnClear.Size = new Size(310, 35);
            btnClear.BackColor = bgColor;
            btnClear.ForeColor = textDark;
            btnClear.Font = new Font("Segoe UI", 9);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnClear.Click += (s, e) => ClearForm();
            leftPanel.Controls.Add(btnClear);
            y += 45;

            // ── Employee mode: info notice ──
            if (isEmployeeMode)
            {
                Panel noticePanel = new Panel();
                noticePanel.Location = new Point(20, y);
                noticePanel.Size = new Size(310, 55);
                noticePanel.BackColor = Color.FromArgb(240, 253, 244);

                Label lblNotice = new Label();
                lblNotice.Text = "ℹ️ You can only mark today's attendance.\nAbsent/Late is set by HR.";
                lblNotice.ForeColor = Color.FromArgb(5, 120, 85);
                lblNotice.Font = new Font("Segoe UI", 8);
                lblNotice.Dock = DockStyle.Fill;
                lblNotice.TextAlign = ContentAlignment.MiddleCenter;
                noticePanel.Controls.Add(lblNotice);
                leftPanel.Controls.Add(noticePanel);
            }

            this.Controls.Add(leftPanel);

            // ═══ RIGHT PANEL ═══
            Panel rightPanel = new Panel();
            rightPanel.Size = new Size(700, 580);
            rightPanel.Location = new Point(385, 75);
            rightPanel.BackColor = white;

            // Filter Panel
            Panel filterPanel = new Panel();
            filterPanel.Size = new Size(700, 55);
            filterPanel.Location = new Point(0, 0);
            filterPanel.BackColor = white;

            Label lblFilter = new Label();
            lblFilter.Text = "🔍 Filter by Date:";
            lblFilter.ForeColor = textGray;
            lblFilter.Font = new Font("Segoe UI", 9);
            lblFilter.Location = new Point(15, 18);
            lblFilter.Size = new Size(110, 20);
            filterPanel.Controls.Add(lblFilter);

            DateTimePicker dtpFilter = new DateTimePicker();
            dtpFilter.Location = new Point(125, 13);
            dtpFilter.Size = new Size(130, 30);
            dtpFilter.Font = new Font("Segoe UI", 9);
            dtpFilter.Format = DateTimePickerFormat.Short;
            dtpFilter.Value = DateTime.Today;
            filterPanel.Controls.Add(dtpFilter);

            Button btnFilter = new Button();
            btnFilter.Text = "🔍 Filter";
            btnFilter.Location = new Point(265, 12);
            btnFilter.Size = new Size(90, 32);
            btnFilter.BackColor = accent;
            btnFilter.ForeColor = white;
            btnFilter.Font = new Font("Segoe UI", 9);
            btnFilter.FlatStyle = FlatStyle.Flat;
            btnFilter.FlatAppearance.BorderSize = 0;
            btnFilter.Click += (s, e) => LoadAttendanceByDate(dtpFilter.Value.Date);
            filterPanel.Controls.Add(btnFilter);

            Button btnAll = new Button();
            btnAll.Text = "📋 All";
            btnAll.Location = new Point(365, 12);
            btnAll.Size = new Size(80, 32);
            btnAll.BackColor = bgColor;
            btnAll.ForeColor = textDark;
            btnAll.Font = new Font("Segoe UI", 9);
            btnAll.FlatStyle = FlatStyle.Flat;
            btnAll.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnAll.Click += (s, e) => LoadAttendance();
            filterPanel.Controls.Add(btnAll);

            rightPanel.Controls.Add(filterPanel);

            // DataGridView
            dgvAttendance = new DataGridView();
            dgvAttendance.Location = new Point(0, 55);
            dgvAttendance.Size = new Size(700, 525);
            dgvAttendance.BackgroundColor = white;
            dgvAttendance.BorderStyle = BorderStyle.None;
            dgvAttendance.RowHeadersVisible = false;
            dgvAttendance.AllowUserToAddRows = false;
            dgvAttendance.ReadOnly = true;
            dgvAttendance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAttendance.Font = new Font("Segoe UI", 9);
            dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAttendance.ColumnHeadersDefaultCellStyle.BackColor = primary;
            dgvAttendance.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvAttendance.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvAttendance.ColumnHeadersHeight = 38;
            dgvAttendance.RowTemplate.Height = 35;
            dgvAttendance.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgvAttendance.GridColor = Color.FromArgb(229, 231, 235);

            // Row colors by status
            dgvAttendance.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var statusCell = dgvAttendance.Rows[e.RowIndex].Cells["Status"];
                if (statusCell == null) return;
                string status = statusCell.Value?.ToString();
                switch (status)
                {
                    case "Present": dgvAttendance.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244); break;
                    case "Absent": dgvAttendance.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242); break;
                    case "Late": dgvAttendance.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235); break;
                    case "Leave": dgvAttendance.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(239, 246, 255); break;
                }
            };

            rightPanel.Controls.Add(dgvAttendance);
            this.Controls.Add(rightPanel);
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
                        // ── Sirf apna naam — EmployeeID se direct match ──
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
            catch (Exception ex) { MessageBox.Show("Employee Load Error: " + ex.Message); }
        }

        private void LoadAttendance()
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
                        // ── Sirf apni attendance — employeeID se ──
                        query = @"SELECT a.AttendanceID,
                                  e.FirstName + ' ' + e.LastName AS Employee,
                                  a.AttendanceDate AS Date,
                                  a.Status
                                  FROM Attendance a
                                  JOIN Employees e ON a.EmployeeID = e.EmployeeID
                                  WHERE a.EmployeeID = @id
                                  ORDER BY a.AttendanceDate DESC";
                        da = new SqlDataAdapter(query, con);
                        da.SelectCommand.Parameters.AddWithValue("@id", restrictedEmployeeID);
                    }
                    else
                    {
                        query = @"SELECT a.AttendanceID,
                                  e.FirstName + ' ' + e.LastName AS Employee,
                                  a.AttendanceDate AS Date,
                                  a.Status
                                  FROM Attendance a
                                  JOIN Employees e ON a.EmployeeID = e.EmployeeID
                                  ORDER BY a.AttendanceDate DESC";
                        da = new SqlDataAdapter(query, con);
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvAttendance.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        private void LoadAttendanceByDate(DateTime date)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query;
                    SqlCommand cmd;

                    if (isEmployeeMode)
                    {
                        // ── Sirf apni filtered attendance ──
                        query = @"SELECT a.AttendanceID,
                                  e.FirstName + ' ' + e.LastName AS Employee,
                                  a.AttendanceDate AS Date,
                                  a.Status
                                  FROM Attendance a
                                  JOIN Employees e ON a.EmployeeID = e.EmployeeID
                                  WHERE a.EmployeeID = @id AND a.AttendanceDate = @date
                                  ORDER BY a.AttendanceDate DESC";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", restrictedEmployeeID);
                        cmd.Parameters.AddWithValue("@date", date);
                    }
                    else
                    {
                        query = @"SELECT a.AttendanceID,
                                  e.FirstName + ' ' + e.LastName AS Employee,
                                  a.AttendanceDate AS Date,
                                  a.Status
                                  FROM Attendance a
                                  JOIN Employees e ON a.EmployeeID = e.EmployeeID
                                  WHERE a.AttendanceDate = @date
                                  ORDER BY a.AttendanceDate DESC";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@date", date);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvAttendance.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Filter Error: " + ex.Message); }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cbEmployee.SelectedValue == null)
            {
                MessageBox.Show("Employee select karo!", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── Employee mode: sirf apna ID use ho, koi aur ID nahi ──
            int empID = isEmployeeMode
                ? restrictedEmployeeID
                : Convert.ToInt32(cbEmployee.SelectedValue);

            // ── Employee sirf aaj ki date mark kar sakta hai ──
            DateTime selectedDate = isEmployeeMode ? DateTime.Today : dtpDate.Value.Date;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Duplicate check — ek din mein sirf ek record
                    SqlCommand chk = new SqlCommand(
                        "SELECT COUNT(*) FROM Attendance WHERE EmployeeID=@eid AND AttendanceDate=@date", con);
                    chk.Parameters.AddWithValue("@eid", empID);
                    chk.Parameters.AddWithValue("@date", selectedDate);
                    int exists = (int)chk.ExecuteScalar();

                    if (exists > 0)
                    {
                        MessageBox.Show(
                            isEmployeeMode
                                ? "✋ You have already marked your attendance for today!"
                                : "This employee's attendance for this date already exists!",
                            "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = @"INSERT INTO Attendance (EmployeeID, AttendanceDate, Status)
                                     VALUES (@eid, @date, @status)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@eid", empID);
                    cmd.Parameters.AddWithValue("@date", selectedDate);
                    cmd.Parameters.AddWithValue("@status", cbStatus.SelectedItem.ToString());
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("✅ Attendance Saved Successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadAttendance();
                }
            }
            catch (Exception ex) { MessageBox.Show("Save Error: " + ex.Message); }
        }

        private void ClearForm()
        {
            if (cbEmployee.Items.Count > 0) cbEmployee.SelectedIndex = 0;
            if (!isEmployeeMode) dtpDate.Value = DateTime.Today;
            cbStatus.SelectedIndex = 0;
        }

        private void AttendanceForm_Load(object sender, EventArgs e) { }
    }
}