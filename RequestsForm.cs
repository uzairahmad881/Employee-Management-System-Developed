using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class RequestsForm : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";

        Color primary = Color.FromArgb(30, 58, 138);
        Color accent = Color.FromArgb(59, 130, 246);
        Color bgColor = Color.FromArgb(243, 244, 246);
        Color white = Color.White;
        Color textDark = Color.FromArgb(17, 24, 39);
        Color textGray = Color.FromArgb(107, 114, 128);
        Color success = Color.FromArgb(16, 185, 129);
        Color danger = Color.FromArgb(220, 38, 38);
        Color warning = Color.FromArgb(245, 158, 11);

        // ═══════════════════════════════════════════
        // -1 = Admin/HR mode (sab employees dikhao)
        // >0 = Employee mode (sirf apni requests)
        // ═══════════════════════════════════════════
        int restrictedEmployeeID = -1;
        bool isEmployeeMode => restrictedEmployeeID > 0;

        ComboBox cbEmployee, cbRequestType;
        TextBox txtDescription;
        DataGridView dgvRequests;
        int selectedRequestID = -1;

        // ── Admin/HR constructor ──
        public RequestsForm()
        {
            InitializeComponent();
            restrictedEmployeeID = -1;
            CreateUI();
            LoadEmployees();
            LoadRequests();
        }

        // ── Employee constructor — sirf apna employeeID pass hoga ──
        public RequestsForm(int employeeID)
        {
            InitializeComponent();
            restrictedEmployeeID = employeeID;
            CreateUI();
            LoadEmployees();
            LoadRequests();
        }

        private void CreateUI()
        {
            this.Text = isEmployeeMode ? "My Requests" : "Requests Management";
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
            lblTitle.Text = isEmployeeMode ? "📩  My Requests" : "📩  Requests Management";
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
            formHeader.BackColor = accent;

            Label lblFormTitle = new Label();
            lblFormTitle.Text = "New Request";
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
            if (isEmployeeMode)
            {
                cbEmployee.Enabled = false;
                cbEmployee.BackColor = bgColor;
            }
            leftPanel.Controls.Add(cbEmployee);
            y += 65;

            // Request Type
            AddLabel(leftPanel, "Request Type *", y);
            cbRequestType = new ComboBox();
            cbRequestType.Location = new Point(20, y + 20);
            cbRequestType.Size = new Size(310, 30);
            cbRequestType.Font = new Font("Segoe UI", 10);
            cbRequestType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRequestType.Items.AddRange(new string[] { "Leave", "Promotion" });
            cbRequestType.SelectedIndex = 0;
            leftPanel.Controls.Add(cbRequestType);
            y += 65;

            // Description
            AddLabel(leftPanel, "Description / Reason", y);
            txtDescription = new TextBox();
            txtDescription.Location = new Point(20, y + 20);
            txtDescription.Size = new Size(310, 120);
            txtDescription.Font = new Font("Segoe UI", 10);
            txtDescription.Multiline = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;
            leftPanel.Controls.Add(txtDescription);
            y += 145;

            // Submit Button
            Button btnSubmit = new Button();
            btnSubmit.Text = "📩  Submit Request";
            btnSubmit.Location = new Point(20, y);
            btnSubmit.Size = new Size(310, 42);
            btnSubmit.BackColor = accent;
            btnSubmit.ForeColor = white;
            btnSubmit.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.Click += BtnSubmit_Click;
            leftPanel.Controls.Add(btnSubmit);
            y += 55;

            // Approve/Reject — sirf Admin/HR
            if (!isEmployeeMode)
            {
                Label lblAction = new Label();
                lblAction.Text = "── Selected Request Action ──";
                lblAction.ForeColor = textGray;
                lblAction.Font = new Font("Segoe UI", 8);
                lblAction.Location = new Point(20, y);
                lblAction.Size = new Size(310, 20);
                lblAction.TextAlign = ContentAlignment.MiddleCenter;
                leftPanel.Controls.Add(lblAction);
                y += 25;

                Button btnApprove = new Button();
                btnApprove.Text = "✅ Approve";
                btnApprove.Location = new Point(20, y);
                btnApprove.Size = new Size(148, 38);
                btnApprove.BackColor = success;
                btnApprove.ForeColor = white;
                btnApprove.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnApprove.FlatStyle = FlatStyle.Flat;
                btnApprove.FlatAppearance.BorderSize = 0;
                btnApprove.Cursor = Cursors.Hand;
                btnApprove.Click += (s, e) => UpdateRequestStatus("Approved");
                leftPanel.Controls.Add(btnApprove);

                Button btnReject = new Button();
                btnReject.Text = "❌ Reject";
                btnReject.Location = new Point(182, y);
                btnReject.Size = new Size(148, 38);
                btnReject.BackColor = danger;
                btnReject.ForeColor = white;
                btnReject.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnReject.FlatStyle = FlatStyle.Flat;
                btnReject.FlatAppearance.BorderSize = 0;
                btnReject.Cursor = Cursors.Hand;
                btnReject.Click += (s, e) => UpdateRequestStatus("Rejected");
                leftPanel.Controls.Add(btnReject);
            }

            this.Controls.Add(leftPanel);

            // RIGHT PANEL
            Panel rightPanel = new Panel();
            rightPanel.Size = new Size(700, 580);
            rightPanel.Location = new Point(385, 75);
            rightPanel.BackColor = white;

            // Filter Bar
            Panel filterPanel = new Panel();
            filterPanel.Size = new Size(700, 55);
            filterPanel.Location = new Point(0, 0);
            filterPanel.BackColor = Color.FromArgb(249, 250, 251);

            Label lblFilter = new Label();
            lblFilter.Text = "Filter:";
            lblFilter.ForeColor = textGray;
            lblFilter.Font = new Font("Segoe UI", 9);
            lblFilter.Location = new Point(15, 17);
            lblFilter.Size = new Size(50, 22);
            filterPanel.Controls.Add(lblFilter);

            string[] filters = { "All", "Pending", "Approved", "Rejected" };
            int fx = 65;
            foreach (string f in filters)
            {
                Button btnF = new Button();
                btnF.Text = f;
                btnF.Location = new Point(fx, 12);
                btnF.Size = new Size(80, 30);
                btnF.BackColor = f == "All" ? accent : bgColor;
                btnF.ForeColor = f == "All" ? white : textDark;
                btnF.Font = new Font("Segoe UI", 9);
                btnF.FlatStyle = FlatStyle.Flat;
                btnF.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
                btnF.FlatAppearance.BorderSize = 1;
                btnF.Tag = f;
                btnF.Click += (s, e) =>
                {
                    string status = ((Button)s).Tag.ToString();
                    if (status == "All") LoadRequests();
                    else LoadRequestsByStatus(status);
                };
                filterPanel.Controls.Add(btnF);
                fx += 85;
            }

            rightPanel.Controls.Add(filterPanel);

            // Grid
            dgvRequests = new DataGridView();
            dgvRequests.Location = new Point(0, 55);
            dgvRequests.Size = new Size(700, 525);
            dgvRequests.BackgroundColor = white;
            dgvRequests.BorderStyle = BorderStyle.None;
            dgvRequests.RowHeadersVisible = false;
            dgvRequests.AllowUserToAddRows = false;
            dgvRequests.ReadOnly = true;
            dgvRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRequests.Font = new Font("Segoe UI", 9);
            dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRequests.ColumnHeadersDefaultCellStyle.BackColor = primary;
            dgvRequests.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvRequests.ColumnHeadersHeight = 38;
            dgvRequests.RowTemplate.Height = 35;
            dgvRequests.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgvRequests.GridColor = Color.FromArgb(229, 231, 235);
            dgvRequests.CellClick += DgvRequests_CellClick;

            dgvRequests.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex >= 0 && dgvRequests.Rows[e.RowIndex].Cells["Status"] != null)
                {
                    string status = dgvRequests.Rows[e.RowIndex].Cells["Status"].Value?.ToString();
                    if (status == "Approved")
                        dgvRequests.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244);
                    else if (status == "Rejected")
                        dgvRequests.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
                    else
                        dgvRequests.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235);
                }
            };

            rightPanel.Controls.Add(dgvRequests);
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

        private void LoadRequests()
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
                        query = @"SELECT r.RequestID,
                                  e.FirstName + ' ' + e.LastName AS Employee,
                                  r.RequestType, r.RequestDate,
                                  r.Status, r.Description
                                  FROM Requests r
                                  JOIN Employees e ON r.EmployeeID = e.EmployeeID
                                  WHERE r.EmployeeID = @id
                                  ORDER BY r.RequestDate DESC";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", restrictedEmployeeID);
                    }
                    else
                    {
                        query = @"SELECT r.RequestID,
                                  e.FirstName + ' ' + e.LastName AS Employee,
                                  r.RequestType, r.RequestDate,
                                  r.Status, r.Description
                                  FROM Requests r
                                  JOIN Employees e ON r.EmployeeID = e.EmployeeID
                                  ORDER BY r.RequestDate DESC";
                        cmd = new SqlCommand(query, con);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRequests.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        private void LoadRequestsByStatus(string status)
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
                        query = @"SELECT r.RequestID,
                                  e.FirstName + ' ' + e.LastName AS Employee,
                                  r.RequestType, r.RequestDate,
                                  r.Status, r.Description
                                  FROM Requests r
                                  JOIN Employees e ON r.EmployeeID = e.EmployeeID
                                  WHERE r.EmployeeID = @id AND r.Status = @status
                                  ORDER BY r.RequestDate DESC";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", restrictedEmployeeID);
                        cmd.Parameters.AddWithValue("@status", status);
                    }
                    else
                    {
                        query = @"SELECT r.RequestID,
                                  e.FirstName + ' ' + e.LastName AS Employee,
                                  r.RequestType, r.RequestDate,
                                  r.Status, r.Description
                                  FROM Requests r
                                  JOIN Employees e ON r.EmployeeID = e.EmployeeID
                                  WHERE r.Status = @status
                                  ORDER BY r.RequestDate DESC";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@status", status);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRequests.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Filter Error: " + ex.Message); }
        }

        private void DgvRequests_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            selectedRequestID = Convert.ToInt32(dgvRequests.Rows[e.RowIndex].Cells["RequestID"].Value);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (cbEmployee.SelectedValue == null)
            { MessageBox.Show("Employee select karo!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int empID = isEmployeeMode ? restrictedEmployeeID : Convert.ToInt32(cbEmployee.SelectedValue);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"INSERT INTO Requests (EmployeeID, RequestType, Description)
                                     VALUES (@eid, @type, @desc)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@eid", empID);
                    cmd.Parameters.AddWithValue("@type", cbRequestType.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(txtDescription.Text)
                        ? (object)DBNull.Value : txtDescription.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("✅ Request Submitted Successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtDescription.Text = "";
                    LoadRequests();
                }
            }
            catch (Exception ex) { MessageBox.Show("Submit Error: " + ex.Message); }
        }

        // ✅ FIX: ProcessedBy=1 (hardcoded) ki jagah
        // pehle valid UserID dhundho ya NULL rakho
        private void UpdateRequestStatus(string status)
        {
            if (selectedRequestID == -1)
            {
                MessageBox.Show("Please select a request from the grid first!", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // ✅ Pehle Users table se pehla valid UserID lo
                    SqlCommand cmdGetUser = new SqlCommand(
                        "SELECT TOP 1 UserID FROM Users WHERE IsActive=1 ORDER BY UserID", con);
                    object result = cmdGetUser.ExecuteScalar();

                    // ✅ Agar koi user mila toh uska ID use karo, warna NULL rakho
                    object processedBy = (result != null) ? result : (object)DBNull.Value;

                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Requests SET Status=@status, ProcessedDate=GETDATE(), ProcessedBy=@processedBy WHERE RequestID=@id", con);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@processedBy", processedBy);
                    cmd.Parameters.AddWithValue("@id", selectedRequestID);
                    cmd.ExecuteNonQuery();

                    string emoji = status == "Approved" ? "✅" : "❌";
                    MessageBox.Show($"{emoji} Request {status}!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    selectedRequestID = -1;
                    LoadRequests();
                }
            }
            catch (Exception ex) { MessageBox.Show("Update Error: " + ex.Message); }
        }

        private void RequestsForm_Load(object sender, EventArgs e) { }
    }
}