using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class EmployeeForm : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";
        int loggedInUserID = -1;
        string userRole = "Admin";

        Color primary = Color.FromArgb(30, 58, 138);
        Color accent = Color.FromArgb(59, 130, 246);
        Color bgColor = Color.FromArgb(243, 244, 246);
        Color white = Color.White;
        Color textDark = Color.FromArgb(17, 24, 39);
        Color textGray = Color.FromArgb(107, 114, 128);
        Color success = Color.FromArgb(16, 185, 129);
        Color danger = Color.FromArgb(220, 38, 38);

        TextBox txtEmpID, txtFirstName, txtLastName, txtEmail, txtPhone, txtSalary;
        ComboBox cbDepartment, cbPosition;
        DataGridView dgvEmployees;
        int selectedEmployeeID = -1;

        public EmployeeForm(int userID = -1, string role = "Admin")
        {
            InitializeComponent();
            loggedInUserID = userID;
            userRole = role;
            CreateUI();
            LoadDepartments();
            LoadEmployees();
        }

        private void CreateUI()
        {
            this.Text = "Employee Management";
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
            lblTitle.Text = "👨‍💼  Employee Management";
            lblTitle.ForeColor = white;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Size = new Size(400, 30);
            topBar.Controls.Add(lblTitle);
            this.Controls.Add(topBar);

            // ═══ LEFT PANEL (Form) ═══
            Panel leftPanel = new Panel();
            leftPanel.Size = new Size(350, 580);
            leftPanel.Location = new Point(20, 75);
            leftPanel.BackColor = white;
            leftPanel.AutoScroll = true;

            // Form Header
            Panel formHeader = new Panel();
            formHeader.Size = new Size(350, 45);
            formHeader.Location = new Point(0, 0);
            formHeader.BackColor = accent;

            Label lblFormTitle = new Label();
            lblFormTitle.Text = userRole == "Employee" ? "My Profile" : "Employee Details";
            lblFormTitle.ForeColor = white;
            lblFormTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblFormTitle.Dock = DockStyle.Fill;
            lblFormTitle.TextAlign = ContentAlignment.MiddleCenter;
            formHeader.Controls.Add(lblFormTitle);
            leftPanel.Controls.Add(formHeader);

            int fieldY = 60;

            // Employee ID
            txtEmpID = CreateField(leftPanel, "Employee ID (Auto)", fieldY, true);
            fieldY += 65;

            // First Name
            txtFirstName = CreateField(leftPanel, "First Name *", fieldY);
            fieldY += 65;

            // Last Name
            txtLastName = CreateField(leftPanel, "Last Name *", fieldY);
            fieldY += 65;

            // Email
            txtEmail = CreateField(leftPanel, "Email", fieldY);
            fieldY += 65;

            // Phone
            txtPhone = CreateField(leftPanel, "Phone", fieldY);
            fieldY += 65;

            // Department
            Label lblDept = new Label();
            lblDept.Text = "Department";
            lblDept.ForeColor = textGray;
            lblDept.Font = new Font("Segoe UI", 8);
            lblDept.Location = new Point(20, fieldY);
            lblDept.Size = new Size(310, 18);
            leftPanel.Controls.Add(lblDept);

            cbDepartment = new ComboBox();
            cbDepartment.Location = new Point(20, fieldY + 20);
            cbDepartment.Size = new Size(310, 30);
            cbDepartment.Font = new Font("Segoe UI", 10);
            cbDepartment.DropDownStyle = ComboBoxStyle.DropDownList;
            cbDepartment.BackColor = white;
            leftPanel.Controls.Add(cbDepartment);
            fieldY += 65;

            // Position
            Label lblPos = new Label();
            lblPos.Text = "Position";
            lblPos.ForeColor = textGray;
            lblPos.Font = new Font("Segoe UI", 8);
            lblPos.Location = new Point(20, fieldY);
            lblPos.Size = new Size(310, 18);
            leftPanel.Controls.Add(lblPos);

            cbPosition = new ComboBox();
            cbPosition.Location = new Point(20, fieldY + 20);
            cbPosition.Size = new Size(310, 30);
            cbPosition.Font = new Font("Segoe UI", 10);
            cbPosition.DropDownStyle = ComboBoxStyle.DropDown;
            cbPosition.Items.AddRange(new string[] { "Developer", "Designer", "Manager", "HR", "Finance", "Sales" });
            cbPosition.BackColor = white;
            leftPanel.Controls.Add(cbPosition);
            fieldY += 65;

            // Base Salary
            txtSalary = CreateField(leftPanel, "Base Salary", fieldY);
            fieldY += 65;

            // ✅ BUTTONS (Only for Admin)
            if (userRole == "Admin")
            {
                Button btnAdd = new Button();
                btnAdd.Text = "➕ Add";
                btnAdd.Location = new Point(20, fieldY);
                btnAdd.Size = new Size(95, 35);
                btnAdd.BackColor = success;
                btnAdd.ForeColor = white;
                btnAdd.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                btnAdd.FlatStyle = FlatStyle.Flat;
                btnAdd.FlatAppearance.BorderSize = 0;
                btnAdd.Cursor = Cursors.Hand;
                btnAdd.Click += BtnAdd_Click;
                leftPanel.Controls.Add(btnAdd);

                Button btnUpdate = new Button();
                btnUpdate.Text = "✏️ Update";
                btnUpdate.Location = new Point(125, fieldY);
                btnUpdate.Size = new Size(95, 35);
                btnUpdate.BackColor = accent;
                btnUpdate.ForeColor = white;
                btnUpdate.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                btnUpdate.FlatStyle = FlatStyle.Flat;
                btnUpdate.FlatAppearance.BorderSize = 0;
                btnUpdate.Cursor = Cursors.Hand;
                btnUpdate.Click += BtnUpdate_Click;
                leftPanel.Controls.Add(btnUpdate);

                Button btnDelete = new Button();
                btnDelete.Text = "🗑️ Delete";
                btnDelete.Location = new Point(230, fieldY);
                btnDelete.Size = new Size(95, 35);
                btnDelete.BackColor = danger;
                btnDelete.ForeColor = white;
                btnDelete.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                btnDelete.FlatStyle = FlatStyle.Flat;
                btnDelete.FlatAppearance.BorderSize = 0;
                btnDelete.Cursor = Cursors.Hand;
                btnDelete.Click += BtnDelete_Click;
                leftPanel.Controls.Add(btnDelete);

                fieldY += 45;

                Button btnClear = new Button();
                btnClear.Text = "🔄 Clear";
                btnClear.Size = new Size(310, 35);
                btnClear.Location = new Point(20, fieldY);
                btnClear.BackColor = bgColor;
                btnClear.ForeColor = textDark;
                btnClear.Font = new Font("Segoe UI", 9);
                btnClear.FlatStyle = FlatStyle.Flat;
                btnClear.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
                btnClear.Click += (s, e) => ClearForm();
                leftPanel.Controls.Add(btnClear);
            }

            this.Controls.Add(leftPanel);

            // ═══ RIGHT PANEL (Grid) ═══
            Panel rightPanel = new Panel();
            rightPanel.Size = new Size(700, 580);
            rightPanel.Location = new Point(385, 75);
            rightPanel.BackColor = white;

            // Search Panel
            Panel searchPanel = new Panel();
            searchPanel.Size = new Size(700, 55);
            searchPanel.Location = new Point(0, 0);
            searchPanel.BackColor = white;

            TextBox txtSearch = new TextBox();
            txtSearch.Location = new Point(15, 13);
            txtSearch.Size = new Size(450, 30);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.ForeColor = textGray;
            txtSearch.Text = "🔍  Search by name or email...";
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text.StartsWith("🔍")) txtSearch.Text = ""; txtSearch.ForeColor = textDark; };
            txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "🔍  Search by name or email..."; txtSearch.ForeColor = textGray; } };
            txtSearch.TextChanged += (s, e) => SearchEmployees(txtSearch.Text);
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
            btnRefresh.Click += (s, e) => LoadEmployees();
            searchPanel.Controls.Add(btnRefresh);

            rightPanel.Controls.Add(searchPanel);

            // DataGridView
            dgvEmployees = new DataGridView();
            dgvEmployees.Location = new Point(0, 55);
            dgvEmployees.Size = new Size(700, 525);
            dgvEmployees.BackgroundColor = white;
            dgvEmployees.BorderStyle = BorderStyle.None;
            dgvEmployees.RowHeadersVisible = false;
            dgvEmployees.AllowUserToAddRows = false;
            dgvEmployees.ReadOnly = true;
            dgvEmployees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmployees.Font = new Font("Segoe UI", 9);
            dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEmployees.ColumnHeadersDefaultCellStyle.BackColor = primary;
            dgvEmployees.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvEmployees.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvEmployees.ColumnHeadersHeight = 38;
            dgvEmployees.RowTemplate.Height = 35;
            dgvEmployees.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251); dgvEmployees.GridColor = Color.FromArgb(229, 231, 235);
            dgvEmployees.CellClick += DgvEmployees_CellClick;

            rightPanel.Controls.Add(dgvEmployees);
            this.Controls.Add(rightPanel);
        }

        private TextBox CreateField(Panel parent, string label, int y, bool readOnly = false)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.ForeColor = textGray;
            lbl.Font = new Font("Segoe UI", 8);
            lbl.Location = new Point(20, y);
            lbl.Size = new Size(310, 18);
            parent.Controls.Add(lbl);

            TextBox txt = new TextBox();
            txt.Location = new Point(20, y + 20);
            txt.Size = new Size(310, 30);
            txt.Font = new Font("Segoe UI", 10);
            txt.ReadOnly = readOnly;
            txt.BackColor = readOnly ? bgColor : white;
            parent.Controls.Add(txt);

            return txt;
        }

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT DepartmentID, DepartmentName FROM Departments", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cbDepartment.DataSource = dt;
                    cbDepartment.DisplayMember = "DepartmentName";
                    cbDepartment.ValueMember = "DepartmentID";
                }
            }
            catch (Exception ex) { MessageBox.Show("Dept Load Error: " + ex.Message); }
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

                    if (userRole == "Employee" && loggedInUserID != -1)
                    {
                        query = @"SELECT e.EmployeeID, e.FirstName, e.LastName, 
                                 e.Email, e.Phone, d.DepartmentName, 
                                 e.Position, e.BaseSalary
                                 FROM Employees e
                                 LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                                 WHERE e.IsActive = 1 AND e.UserID = @uid
                                 ORDER BY e.EmployeeID DESC";
                        da = new SqlDataAdapter(query, con);
                        da.SelectCommand.Parameters.AddWithValue("@uid", loggedInUserID);
                    }
                    else
                    {
                        query = @"SELECT e.EmployeeID, e.FirstName, e.LastName, 
                                 e.Email, e.Phone, d.DepartmentName, 
                                 e.Position, e.BaseSalary
                                 FROM Employees e
                                 LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                                 WHERE e.IsActive = 1
                                 ORDER BY e.EmployeeID DESC";
                        da = new SqlDataAdapter(query, con);
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEmployees.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        private void SearchEmployees(string keyword)
        {
            if (keyword.StartsWith("🔍") || keyword.Length < 2) { LoadEmployees(); return; }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"SELECT e.EmployeeID, e.FirstName, e.LastName, 
                                    e.Email, e.Phone, d.DepartmentName, e.Position, e.BaseSalary
                                    FROM Employees e
                                    LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                                    WHERE e.IsActive=1 AND 
                                    (e.FirstName LIKE @k OR e.LastName LIKE @k OR e.Email LIKE @k)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@k", "%" + keyword + "%");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEmployees.DataSource = dt;
                }
            }
            catch { }
        }

        private void DgvEmployees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvEmployees.Rows[e.RowIndex];
            selectedEmployeeID = Convert.ToInt32(row.Cells["EmployeeID"].Value);
            txtEmpID.Text = selectedEmployeeID.ToString();
            txtFirstName.Text = row.Cells["FirstName"].Value?.ToString();
            txtLastName.Text = row.Cells["LastName"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();
            txtPhone.Text = row.Cells["Phone"].Value?.ToString();
            txtSalary.Text = row.Cells["BaseSalary"].Value?.ToString();
            cbPosition.Text = row.Cells["Position"].Value?.ToString();

            string deptName = row.Cells["DepartmentName"].Value?.ToString();
            foreach (DataRowView item in cbDepartment.Items)
                if (item["DepartmentName"].ToString() == deptName)
                { cbDepartment.SelectedItem = item; break; }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text))
            { MessageBox.Show("First Name aur Last Name zaroor bharo!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string baseUsername = (txtFirstName.Text.Substring(0, 1) + txtLastName.Text).ToLower();
                    string autoUsername = baseUsername;
                    int counter = 1;

                    while (true)
                    {
                        SqlCommand chkUser = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@user", con);
                        chkUser.Parameters.AddWithValue("@user", autoUsername);
                        int userExists = (int)chkUser.ExecuteScalar();

                        if (userExists == 0) break;
                        autoUsername = baseUsername + counter;
                        counter++;
                    }

                    string autoPassword = DateTime.Now.ToString("ddMMyyyy");

                    SqlCommand cmdUser = new SqlCommand(
                        "INSERT INTO Users (Username, Password, Role) VALUES (@user, @pass, @role)", con);
                    cmdUser.Parameters.AddWithValue("@user", autoUsername);
                    cmdUser.Parameters.AddWithValue("@pass", autoPassword);
                    cmdUser.Parameters.AddWithValue("@role", "Employee");
                    cmdUser.ExecuteNonQuery();

                    SqlCommand getUID = new SqlCommand("SELECT UserID FROM Users WHERE Username=@user", con);
                    getUID.Parameters.AddWithValue("@user", autoUsername);
                    int newUserID = (int)getUID.ExecuteScalar();

                    string query = @"INSERT INTO Employees (UserID, FirstName, LastName, Email, Phone, DepartmentID, Position, BaseSalary)
                                    VALUES (@uid, @fn, @ln, @em, @ph, @did, @pos, @sal)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@uid", newUserID);
                    cmd.Parameters.AddWithValue("@fn", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@ln", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@em", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@ph", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@did", cbDepartment.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@pos", string.IsNullOrEmpty(cbPosition.Text) ? (object)DBNull.Value : cbPosition.Text);
                    cmd.Parameters.AddWithValue("@sal", string.IsNullOrEmpty(txtSalary.Text) ? (object)DBNull.Value : decimal.Parse(txtSalary.Text));
                    cmd.ExecuteNonQuery();

                    string credentials = $"✅ Employee Successfully Added!\n\n" +
                                        $"👤 Name: {txtFirstName.Text} {txtLastName.Text}\n" +
                                        $"📧 Email: {txtEmail.Text}\n\n" +
                                        $"🔐 Login Credentials:\n" +
                                        $"━━━━━━━━━━━━━━━━━━━━━━\n" +
                                        $"Username: {autoUsername}\n" +
                                        $"Password: {autoPassword}\n" +
                                        $"━━━━━━━━━━━━━━━━━━━━━━";

                    MessageBox.Show(credentials, "Employee Account Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadEmployees();
                }
            }
            catch (Exception ex) { MessageBox.Show("Add Error: " + ex.Message); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedEmployeeID == -1) { MessageBox.Show("Pehle koi employee select karo!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"UPDATE Employees SET FirstName=@fn, LastName=@ln, 
                                    Email=@em, Phone=@ph, DepartmentID=@did, Position=@pos, BaseSalary=@sal
                                    WHERE EmployeeID=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@fn", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@ln", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@em", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@ph", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@did", cbDepartment.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@pos", string.IsNullOrEmpty(cbPosition.Text) ? (object)DBNull.Value : cbPosition.Text);
                    cmd.Parameters.AddWithValue("@sal", string.IsNullOrEmpty(txtSalary.Text) ? (object)DBNull.Value : decimal.Parse(txtSalary.Text));
                    cmd.Parameters.AddWithValue("@id", selectedEmployeeID);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("✅ Employee update ho gaya!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadEmployees();
                }
            }
            catch (Exception ex) { MessageBox.Show("Update Error: " + ex.Message); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedEmployeeID == -1) { MessageBox.Show("Pehle koi employee select karo!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            if (MessageBox.Show("Kya aap is employee ko delete karna chahte hain?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("UPDATE Employees SET IsActive=0 WHERE EmployeeID=@id", con);
                        cmd.Parameters.AddWithValue("@id", selectedEmployeeID);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("✅ Employee delete ho gaya!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadEmployees();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Delete Error: " + ex.Message); }
            }
        }

        private void ClearForm()
        {
            txtEmpID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtSalary.Text = "";
            cbPosition.Text = "";
            selectedEmployeeID = -1;
            if (cbDepartment.Items.Count > 0) cbDepartment.SelectedIndex = 0;
        }
    }
}