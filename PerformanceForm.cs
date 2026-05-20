using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class PerformanceForm : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";

        // ═══════════════════════════════════════════
        // -1   = Admin/HR mode
        // >0   = Employee mode (sirf apna data)
        // isReadOnly = true → employee add/edit na kar sake
        // ═══════════════════════════════════════════
        int restrictedEmployeeID = -1;
        bool isReadOnly = false;
        bool isEmployeeMode => restrictedEmployeeID > 0;

        Color primary = Color.FromArgb(30, 58, 138);
        Color accent = Color.FromArgb(59, 130, 246);
        Color bgColor = Color.FromArgb(243, 244, 246);
        Color white = Color.White;
        Color textDark = Color.FromArgb(17, 24, 39);
        Color textGray = Color.FromArgb(107, 114, 128);
        Color success = Color.FromArgb(16, 185, 129);

        ComboBox cbEmployee;
        ComboBox cbRating;
        TextBox txtReview;
        DataGridView dgvPerformance;

        // ── Admin/HR constructor ──
        public PerformanceForm()
        {
            InitializeComponent();
            restrictedEmployeeID = -1;
            isReadOnly = false;
            CreateUI();
            LoadEmployees();
            LoadPerformance();
        }

        // ── Employee constructor ──
        // employeeID: sirf apna data
        // isReadOnly: true = sirf dekh sakta hai, add nahi kar sakta
        public PerformanceForm(int employeeID, bool isReadOnly = false)
        {
            InitializeComponent();
            restrictedEmployeeID = employeeID;
            this.isReadOnly = isReadOnly;
            CreateUI();
            LoadEmployees();
            LoadPerformance();
        }

        private void CreateUI()
        {
            this.Text = isEmployeeMode ? "My Performance" : "Performance Management";
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
            lblTitle.Text = isEmployeeMode ? "⭐  My Performance" : "⭐  Performance Management";
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
            // Header color: read-only mein gray, writable mein accent
            formHeader.BackColor = isReadOnly ? Color.FromArgb(107, 114, 128) : accent;

            Label lblFormTitle = new Label();
            lblFormTitle.Text = isReadOnly ? "👁️  View Only — My Performance" : "Add Performance Review";
            lblFormTitle.ForeColor = white;
            lblFormTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
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
            cbEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEmployee.Font = new Font("Segoe UI", 10);
            cbEmployee.BackColor = white;
            // Employee mode: disable karo taake change na ho sake
            if (isEmployeeMode) { cbEmployee.Enabled = false; cbEmployee.BackColor = bgColor; }
            leftPanel.Controls.Add(cbEmployee);
            y += 65;

            // Rating
            AddLabel(leftPanel, "Rating (1-5) *", y);
            cbRating = new ComboBox();
            cbRating.Location = new Point(20, y + 20);
            cbRating.Size = new Size(310, 30);
            cbRating.Font = new Font("Segoe UI", 10);
            cbRating.Items.AddRange(new string[] { "1 - Poor", "2 - Fair", "3 - Good", "4 - Very Good", "5 - Excellent" });
            cbRating.SelectedIndex = 2;
            cbRating.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRating.BackColor = white;
            // Read-only mode: disable karo
            if (isReadOnly) { cbRating.Enabled = false; cbRating.BackColor = bgColor; }
            leftPanel.Controls.Add(cbRating);
            y += 65;

            // Review TextBox
            AddLabel(leftPanel, "Review", y);
            txtReview = new TextBox();
            txtReview.Location = new Point(20, y + 20);
            txtReview.Size = new Size(310, 120);
            txtReview.Font = new Font("Segoe UI", 10);
            txtReview.Multiline = true;
            txtReview.ScrollBars = ScrollBars.Vertical;
            // Read-only mode: edit nahi ho sakta
            if (isReadOnly) { txtReview.ReadOnly = true; txtReview.BackColor = bgColor; }
            leftPanel.Controls.Add(txtReview);
            y += 150;

            // ═══════════════════════════════════════════
            // Add/Clear buttons — sirf Admin/HR ke liye
            // Employee (isReadOnly) mein yeh buttons nahi dikhenge
            // ═══════════════════════════════════════════
            if (!isReadOnly)
            {
                Button btnAdd = new Button();
                btnAdd.Text = "✅ Add Review";
                btnAdd.Location = new Point(20, y);
                btnAdd.Size = new Size(310, 40);
                btnAdd.BackColor = success;
                btnAdd.ForeColor = white;
                btnAdd.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnAdd.FlatStyle = FlatStyle.Flat;
                btnAdd.FlatAppearance.BorderSize = 0;
                btnAdd.Click += BtnAdd_Click;
                leftPanel.Controls.Add(btnAdd);

                Button btnClear = new Button();
                btnClear.Text = "🔄 Clear";
                btnClear.Location = new Point(20, y + 50);
                btnClear.Size = new Size(310, 35);
                btnClear.BackColor = bgColor;
                btnClear.ForeColor = textDark;
                btnClear.Font = new Font("Segoe UI", 9);
                btnClear.FlatStyle = FlatStyle.Flat;
                btnClear.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
                btnClear.Click += (s, e) => ClearForm();
                leftPanel.Controls.Add(btnClear);
            }
            else
            {
                // ── Read-only notice label ──
                Panel noticePanel = new Panel();
                noticePanel.Location = new Point(20, y);
                noticePanel.Size = new Size(310, 50);
                noticePanel.BackColor = Color.FromArgb(255, 251, 235);

                Label lblNotice = new Label();
                lblNotice.Text = "ℹ️ You can only view your performance records.\nContact HR for any queries.";
                lblNotice.ForeColor = Color.FromArgb(180, 120, 0);
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

            // Search bar — Admin ke liye hi useful, employee ke liye bhi rakho (sirf apna data filter)
            Panel searchPanel = new Panel();
            searchPanel.Size = new Size(700, 55);
            searchPanel.Location = new Point(0, 0);
            searchPanel.BackColor = white;

            TextBox txtSearch = new TextBox();
            txtSearch.Location = new Point(15, 13);
            txtSearch.Size = new Size(450, 30);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.ForeColor = textGray;
            txtSearch.Text = "🔍  Search...";
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text.StartsWith("🔍")) txtSearch.Text = ""; txtSearch.ForeColor = textDark; };
            txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "🔍  Search..."; txtSearch.ForeColor = textGray; } };
            txtSearch.TextChanged += (s, e) => SearchPerformance(txtSearch.Text);
            searchPanel.Controls.Add(txtSearch);

            Button btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Location = new Point(480, 12);
            btnRefresh.Size = new Size(100, 32);
            btnRefresh.BackColor = accent;
            btnRefresh.ForeColor = white;
            btnRefresh.Font = new Font("Segoe UI", 9);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadPerformance();
            searchPanel.Controls.Add(btnRefresh);

            rightPanel.Controls.Add(searchPanel);

            // DataGridView
            dgvPerformance = new DataGridView();
            dgvPerformance.Location = new Point(0, 55);
            dgvPerformance.Size = new Size(700, 525);
            dgvPerformance.BackgroundColor = white;
            dgvPerformance.BorderStyle = BorderStyle.None;
            dgvPerformance.RowHeadersVisible = false;
            dgvPerformance.AllowUserToAddRows = false;
            dgvPerformance.ReadOnly = true;
            dgvPerformance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPerformance.Font = new Font("Segoe UI", 9);
            dgvPerformance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPerformance.ColumnHeadersDefaultCellStyle.BackColor = primary;
            dgvPerformance.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvPerformance.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvPerformance.ColumnHeadersHeight = 38;
            dgvPerformance.RowTemplate.Height = 35;
            dgvPerformance.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgvPerformance.GridColor = Color.FromArgb(229, 231, 235);

            rightPanel.Controls.Add(dgvPerformance);
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
                        // ── Sirf apna naam ──
                        query = "SELECT EmployeeID, (FirstName + ' ' + LastName) AS Name FROM Employees WHERE EmployeeID=@id AND IsActive=1";
                        da = new SqlDataAdapter(query, con);
                        da.SelectCommand.Parameters.AddWithValue("@id", restrictedEmployeeID);
                    }
                    else
                    {
                        query = "SELECT EmployeeID, (FirstName + ' ' + LastName) AS Name FROM Employees WHERE IsActive=1";
                        da = new SqlDataAdapter(query, con);
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cbEmployee.DataSource = dt;
                    cbEmployee.DisplayMember = "Name";
                    cbEmployee.ValueMember = "EmployeeID";
                }
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        private void LoadPerformance()
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
                        // ── Sirf apni performance ──
                        query = @"SELECT e.FirstName + ' ' + e.LastName AS EmployeeName,
                                  p.Rating, p.Review, p.ReviewDate
                                  FROM Performance p
                                  JOIN Employees e ON p.EmployeeID = e.EmployeeID
                                  WHERE p.EmployeeID = @id
                                  ORDER BY p.ReviewDate DESC";
                        da = new SqlDataAdapter(query, con);
                        da.SelectCommand.Parameters.AddWithValue("@id", restrictedEmployeeID);
                    }
                    else
                    {
                        query = @"SELECT e.FirstName + ' ' + e.LastName AS EmployeeName,
                                  p.Rating, p.Review, p.ReviewDate
                                  FROM Performance p
                                  JOIN Employees e ON p.EmployeeID = e.EmployeeID
                                  ORDER BY p.ReviewDate DESC";
                        da = new SqlDataAdapter(query, con);
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvPerformance.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        private void SearchPerformance(string keyword)
        {
            if (keyword.StartsWith("🔍") || keyword.Length < 2) { LoadPerformance(); return; }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query;
                    SqlCommand cmd;

                    if (isEmployeeMode)
                    {
                        query = @"SELECT e.FirstName + ' ' + e.LastName AS EmployeeName,
                                  p.Rating, p.Review, p.ReviewDate
                                  FROM Performance p
                                  JOIN Employees e ON p.EmployeeID = e.EmployeeID
                                  WHERE p.EmployeeID = @id AND (e.FirstName LIKE @k OR e.LastName LIKE @k)";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", restrictedEmployeeID);
                        cmd.Parameters.AddWithValue("@k", "%" + keyword + "%");
                    }
                    else
                    {
                        query = @"SELECT e.FirstName + ' ' + e.LastName AS EmployeeName,
                                  p.Rating, p.Review, p.ReviewDate
                                  FROM Performance p
                                  JOIN Employees e ON p.EmployeeID = e.EmployeeID
                                  WHERE (e.FirstName LIKE @k OR e.LastName LIKE @k)";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@k", "%" + keyword + "%");
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvPerformance.DataSource = dt;
                }
            }
            catch { }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // ── Extra safety: employee kabhi bhi add na kar sake ──
            if (isReadOnly)
            {
                MessageBox.Show("You are not allowed to add performance records.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    int empID = (int)cbEmployee.SelectedValue;
                    int rating = cbRating.SelectedIndex + 1;

                    SqlCommand getReviewedBy = new SqlCommand(
                        "SELECT UserID FROM Employees WHERE EmployeeID=@empID", con);
                    getReviewedBy.Parameters.AddWithValue("@empID", empID);
                    object reviewedByObj = getReviewedBy.ExecuteScalar();
                    int reviewedBy = (reviewedByObj != null && reviewedByObj != DBNull.Value)
                        ? Convert.ToInt32(reviewedByObj) : 1;

                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Performance (EmployeeID, Rating, Review, ReviewedBy) VALUES (@emp, @rating, @review, @by)", con);
                    cmd.Parameters.AddWithValue("@emp", empID);
                    cmd.Parameters.AddWithValue("@rating", rating);
                    cmd.Parameters.AddWithValue("@review", txtReview.Text);
                    cmd.Parameters.AddWithValue("@by", reviewedBy);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("✅ Performance review added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadPerformance();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ClearForm()
        {
            if (cbEmployee.Items.Count > 0) cbEmployee.SelectedIndex = 0;
            cbRating.SelectedIndex = 2;
            txtReview.Text = "";
        }
    }
}