using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class DepartmentsForm : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";

        Color primary = Color.FromArgb(30, 58, 138);
        Color accent = Color.FromArgb(59, 130, 246);
        Color bgColor = Color.FromArgb(243, 244, 246);
        Color white = Color.White;
        Color textDark = Color.FromArgb(17, 24, 39);
        Color textGray = Color.FromArgb(107, 114, 128);
        Color success = Color.FromArgb(16, 185, 129);
        Color danger = Color.FromArgb(239, 68, 68);

        TextBox txtDeptName, txtDescription;
        DataGridView dgvDepartments;
        int selectedDeptID = -1;

        public DepartmentsForm()
        {
            InitializeComponent();
            CreateUI();
            LoadDepartments();
        }

        private void CreateUI()
        {
            this.Text = "Department Management";
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
            lblTitle.Text = "🏢  Department Management";
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
            lblFormTitle.Text = "Department Details";
            lblFormTitle.ForeColor = white;
            lblFormTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblFormTitle.Dock = DockStyle.Fill;
            lblFormTitle.TextAlign = ContentAlignment.MiddleCenter;
            formHeader.Controls.Add(lblFormTitle);
            leftPanel.Controls.Add(formHeader);

            int y = 60;

            // Department Name
            AddLabel(leftPanel, "Department Name *", y);
            txtDeptName = new TextBox();
            txtDeptName.Location = new Point(20, y + 20);
            txtDeptName.Size = new Size(310, 30);
            txtDeptName.Font = new Font("Segoe UI", 10);
            leftPanel.Controls.Add(txtDeptName);
            y += 65;

            // Description
            AddLabel(leftPanel, "Description", y);
            txtDescription = new TextBox();
            txtDescription.Location = new Point(20, y + 20);
            txtDescription.Size = new Size(310, 100);
            txtDescription.Font = new Font("Segoe UI", 10);
            txtDescription.Multiline = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;
            leftPanel.Controls.Add(txtDescription);
            y += 130;

            // Buttons
            Button btnAdd = CreateButton("➕ Add", 20, y, success);
            Button btnUpdate = CreateButton("✏️ Update", 125, y, accent);
            Button btnDelete = CreateButton("🗑️ Delete", 230, y, danger);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            leftPanel.Controls.Add(btnAdd);
            leftPanel.Controls.Add(btnUpdate);
            leftPanel.Controls.Add(btnDelete);
            y += 45;

            // Clear Button
            Button btnClear = new Button();
            btnClear.Text = "🔄 Clear";
            btnClear.Size = new Size(310, 35);
            btnClear.Location = new Point(20, y);
            btnClear.BackColor = bgColor;
            btnClear.ForeColor = textDark;
            btnClear.Font = new Font("Segoe UI", 9);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnClear.Click += (s, e) => ClearForm();
            leftPanel.Controls.Add(btnClear);

            this.Controls.Add(leftPanel);

            // RIGHT PANEL
            Panel rightPanel = new Panel();
            rightPanel.Size = new Size(700, 580);
            rightPanel.Location = new Point(385, 75);
            rightPanel.BackColor = white;

            // Search bar
            Panel searchPanel = new Panel();
            searchPanel.Size = new Size(700, 55);
            searchPanel.Location = new Point(0, 0);
            searchPanel.BackColor = white;

            TextBox txtSearch = new TextBox();
            txtSearch.Location = new Point(15, 13);
            txtSearch.Size = new Size(450, 30);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.ForeColor = textGray;
            txtSearch.Text = "🔍  Search department...";
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text.StartsWith("🔍")) txtSearch.Text = ""; txtSearch.ForeColor = textDark; };
            txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "🔍  Search department..."; txtSearch.ForeColor = textGray; } };
            txtSearch.TextChanged += (s, e) => SearchDepartments(txtSearch.Text);
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
            btnRefresh.Click += (s, e) => LoadDepartments();
            searchPanel.Controls.Add(btnRefresh);

            rightPanel.Controls.Add(searchPanel);

            // DataGridView
            dgvDepartments = new DataGridView();
            dgvDepartments.Location = new Point(0, 55);
            dgvDepartments.Size = new Size(700, 525);
            dgvDepartments.BackgroundColor = white;
            dgvDepartments.BorderStyle = BorderStyle.None;
            dgvDepartments.RowHeadersVisible = false;
            dgvDepartments.AllowUserToAddRows = false;
            dgvDepartments.ReadOnly = true;
            dgvDepartments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDepartments.Font = new Font("Segoe UI", 9);
            dgvDepartments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDepartments.ColumnHeadersDefaultCellStyle.BackColor = primary;
            dgvDepartments.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvDepartments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvDepartments.ColumnHeadersHeight = 38;
            dgvDepartments.RowTemplate.Height = 35;
            dgvDepartments.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgvDepartments.GridColor = Color.FromArgb(229, 231, 235);
            dgvDepartments.CellClick += DgvDepartments_CellClick;

            rightPanel.Controls.Add(dgvDepartments);
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

        private Button CreateButton(string text, int x, int y, Color color)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(95, 35);
            btn.BackColor = color;
            btn.ForeColor = white;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT DepartmentID, DepartmentName, Description FROM Departments ORDER BY DepartmentID DESC";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvDepartments.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        private void SearchDepartments(string keyword)
        {
            if (keyword.StartsWith("🔍") || keyword.Length < 2) { LoadDepartments(); return; }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT DepartmentID, DepartmentName, Description FROM Departments WHERE DepartmentName LIKE @k ORDER BY DepartmentID DESC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@k", "%" + keyword + "%");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvDepartments.DataSource = dt;
                }
            }
            catch { }
        }

        private void DgvDepartments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvDepartments.Rows[e.RowIndex];
            selectedDeptID = Convert.ToInt32(row.Cells["DepartmentID"].Value);
            txtDeptName.Text = row.Cells["DepartmentName"].Value?.ToString();
            txtDescription.Text = row.Cells["Description"].Value?.ToString();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDeptName.Text))
            { MessageBox.Show("Department Name likho!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "INSERT INTO Departments (DepartmentName, Description) VALUES (@name, @desc)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", txtDeptName.Text);
                    cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("✅ Department add ho gaya!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadDepartments();
                }
            }
            catch (Exception ex) { MessageBox.Show("Add Error: " + ex.Message); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedDeptID == -1) { MessageBox.Show("Pehle department select karo!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "UPDATE Departments SET DepartmentName=@name, Description=@desc WHERE DepartmentID=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", txtDeptName.Text);
                    cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text);
                    cmd.Parameters.AddWithValue("@id", selectedDeptID);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("✅ Department update ho gaya!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadDepartments();
                }
            }
            catch (Exception ex) { MessageBox.Show("Update Error: " + ex.Message); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedDeptID == -1) { MessageBox.Show("Pehle department select karo!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            if (MessageBox.Show("Kya aap is department ko delete karna chahte hain?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Departments WHERE DepartmentID=@id", con);
                        cmd.Parameters.AddWithValue("@id", selectedDeptID);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("✅ Department delete ho gaya!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadDepartments();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Delete Error: " + ex.Message); }
            }
        }

        private void ClearForm()
        {
            txtDeptName.Text = "";
            txtDescription.Text = "";
            selectedDeptID = -1;
        }
    }
}