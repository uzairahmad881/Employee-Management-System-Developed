using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class UsersForm : Form
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

        TextBox txtUsername, txtPassword, txtConfirmPass;
        ComboBox cbRole;
        DataGridView dgvUsers;
        int selectedUserID = -1;

        public UsersForm()
        {
            InitializeComponent();
            CreateUI();
            LoadUsers();
        }

        private void CreateUI()
        {
            this.Text = "User Management";
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
            lblTitle.Text = "👥  User Management";
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
            lblFormTitle.Text = "User Details";
            lblFormTitle.ForeColor = white;
            lblFormTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblFormTitle.Dock = DockStyle.Fill;
            lblFormTitle.TextAlign = ContentAlignment.MiddleCenter;
            formHeader.Controls.Add(lblFormTitle);
            leftPanel.Controls.Add(formHeader);

            int y = 60;

            // Username
            AddLabel(leftPanel, "Username *", y);
            txtUsername = new TextBox();
            txtUsername.Location = new Point(20, y + 20);
            txtUsername.Size = new Size(310, 30);
            txtUsername.Font = new Font("Segoe UI", 10);
            leftPanel.Controls.Add(txtUsername);
            y += 65;

            // Password
            AddLabel(leftPanel, "Password *", y);
            txtPassword = new TextBox();
            txtPassword.Location = new Point(20, y + 20);
            txtPassword.Size = new Size(310, 30);
            txtPassword.Font = new Font("Segoe UI", 10);
            txtPassword.PasswordChar = '*';
            leftPanel.Controls.Add(txtPassword);
            y += 65;

            // Confirm Password
            AddLabel(leftPanel, "Confirm Password *", y);
            txtConfirmPass = new TextBox();
            txtConfirmPass.Location = new Point(20, y + 20);
            txtConfirmPass.Size = new Size(310, 30);
            txtConfirmPass.Font = new Font("Segoe UI", 10);
            txtConfirmPass.PasswordChar = '*';
            leftPanel.Controls.Add(txtConfirmPass);
            y += 65;

            // Role
            AddLabel(leftPanel, "Role *", y);
            cbRole = new ComboBox();
            cbRole.Location = new Point(20, y + 20);
            cbRole.Size = new Size(310, 30);
            cbRole.Font = new Font("Segoe UI", 10);
            cbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRole.Items.AddRange(new string[] { "Admin", "HR", "Employee" });
            cbRole.SelectedIndex = 2;
            leftPanel.Controls.Add(cbRole);
            y += 65;

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

            // Clear
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

            // Search
            Panel searchPanel = new Panel();
            searchPanel.Size = new Size(700, 55);
            searchPanel.Location = new Point(0, 0);
            searchPanel.BackColor = white;

            TextBox txtSearch = new TextBox();
            txtSearch.Location = new Point(15, 13);
            txtSearch.Size = new Size(450, 30);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.ForeColor = textGray;
            txtSearch.Text = "🔍  Search username...";
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text.StartsWith("🔍")) txtSearch.Text = ""; txtSearch.ForeColor = textDark; };
            txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "🔍  Search username..."; txtSearch.ForeColor = textGray; } };
            txtSearch.TextChanged += (s, e) => SearchUsers(txtSearch.Text);
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
            btnRefresh.Click += (s, e) => LoadUsers();
            searchPanel.Controls.Add(btnRefresh);

            rightPanel.Controls.Add(searchPanel);

            // Grid
            dgvUsers = new DataGridView();
            dgvUsers.Location = new Point(0, 55);
            dgvUsers.Size = new Size(700, 525);
            dgvUsers.BackgroundColor = white;
            dgvUsers.BorderStyle = BorderStyle.None;
            dgvUsers.RowHeadersVisible = false;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.ReadOnly = true;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.Font = new Font("Segoe UI", 9);
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = primary;
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvUsers.ColumnHeadersHeight = 38;
            dgvUsers.RowTemplate.Height = 35;
            dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgvUsers.GridColor = Color.FromArgb(229, 231, 235);
            dgvUsers.CellClick += DgvUsers_CellClick;

            rightPanel.Controls.Add(dgvUsers);
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

        private void LoadUsers()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT UserID, Username, Role, IsActive, CreatedAt FROM Users ORDER BY UserID DESC";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvUsers.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        private void SearchUsers(string keyword)
        {
            if (keyword.StartsWith("🔍") || keyword.Length < 2) { LoadUsers(); return; }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT UserID, Username, Role, IsActive, CreatedAt FROM Users WHERE Username LIKE @k ORDER BY UserID DESC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@k", "%" + keyword + "%");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvUsers.DataSource = dt;
                }
            }
            catch { }
        }

        private void DgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvUsers.Rows[e.RowIndex];
            selectedUserID = Convert.ToInt32(row.Cells["UserID"].Value);
            txtUsername.Text = row.Cells["Username"].Value?.ToString();
            txtPassword.Text = "***hidden***";
            txtConfirmPass.Text = "***hidden***";
            cbRole.SelectedItem = row.Cells["Role"].Value?.ToString();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            { MessageBox.Show("Username aur Password likho!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (txtPassword.Text != txtConfirmPass.Text)
            { MessageBox.Show("Passwords match nahi kar rahe!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "INSERT INTO Users (Username, Password, Role) VALUES (@user, @pass, @role)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@role", cbRole.SelectedItem.ToString());
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("✅ User add ho gaya!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadUsers();
                }
            }
            catch (Exception ex) { MessageBox.Show("Add Error: " + ex.Message); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedUserID == -1) { MessageBox.Show("Pehle user select karo!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            if (txtPassword.Text == "***hidden***" || string.IsNullOrEmpty(txtPassword.Text))
            { MessageBox.Show("Naya password likho!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "UPDATE Users SET Username=@user, Password=@pass, Role=@role WHERE UserID=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@role", cbRole.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@id", selectedUserID);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("✅ User update ho gaya!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadUsers();
                }
            }
            catch (Exception ex) { MessageBox.Show("Update Error: " + ex.Message); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedUserID == -1) { MessageBox.Show("Pehle user select karo!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            if (MessageBox.Show("Kya aap is user ko delete karna chahte hain?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserID=@id", con);
                        cmd.Parameters.AddWithValue("@id", selectedUserID);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("✅ User delete ho gaya!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadUsers();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Delete Error: " + ex.Message); }
            }
        }

        private void ClearForm()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtConfirmPass.Text = "";
            cbRole.SelectedIndex = 2;
            selectedUserID = -1;
        }
    }
}