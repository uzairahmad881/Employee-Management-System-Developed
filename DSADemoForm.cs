using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeManagementSystem1
{
    public partial class DSADemoForm : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDB;Integrated Security=True;";

        Color primary = Color.FromArgb(30, 58, 138);
        Color accent = Color.FromArgb(59, 130, 246);
        Color bgColor = Color.FromArgb(243, 244, 246);
        Color white = Color.White;
        Color textDark = Color.FromArgb(17, 24, 39);
        Color textGray = Color.FromArgb(107, 114, 128);
        Color success = Color.FromArgb(16, 185, 129);
        Color warning = Color.FromArgb(245, 158, 11);
        Color purple = Color.FromArgb(139, 92, 246);
        Color danger = Color.FromArgb(239, 68, 68);

        // ✅ DSA Data Structures
        Dictionary<int, string> employeeDict = new Dictionary<int, string>();
        Queue<string> requestQueue = new Queue<string>();
        List<(string name, double rating, int present)> employeeList = new List<(string, double, int)>();

        TextBox txtSearchID, txtOutput;
        DataGridView dgvResults;

        public DSADemoForm()
        {
            InitializeComponent();
            CreateUI();
            LoadDSAData();
        }

        private void CreateUI()
        {
            this.Text = "DSA Integration - Employee Management System";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = bgColor;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            // TOP BAR
            Panel topBar = new Panel();
            topBar.Dock = DockStyle.Top;
            topBar.Height = 65;
            topBar.BackColor = primary;

            Label lblTitle = new Label();
            lblTitle.Text = "🧠  DSA Integration — Data Structures & Algorithms";
            lblTitle.ForeColor = white;
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.Location = new Point(20, 18);
            lblTitle.Size = new Size(600, 30);
            topBar.Controls.Add(lblTitle);

            Label lblSub = new Label();
            lblSub.Text = "Dictionary | Queue | Sorting | Heap | Search";
            lblSub.ForeColor = Color.FromArgb(147, 197, 253);
            lblSub.Font = new Font("Segoe UI", 9);
            lblSub.Location = new Point(650, 22);
            lblSub.Size = new Size(400, 22);
            topBar.Controls.Add(lblSub);

            this.Controls.Add(topBar);

            // TABS
            TabControl tabs = new TabControl();
            tabs.Location = new Point(10, 75);
            tabs.Size = new Size(1070, 590);
            tabs.Font = new Font("Segoe UI", 10);

            // Tab 1 — Dictionary Search
            TabPage tabDict = new TabPage("🔍 Dictionary Search");
            tabDict.BackColor = white;
            CreateDictionaryTab(tabDict);
            tabs.TabPages.Add(tabDict);

            // Tab 2 — Sorting
            TabPage tabSort = new TabPage("📊 Sorting & Ranking");
            tabSort.BackColor = white;
            CreateSortingTab(tabSort);
            tabs.TabPages.Add(tabSort);

            // Tab 3 — Queue
            TabPage tabQueue = new TabPage("📩 Request Queue");
            tabQueue.BackColor = white;
            CreateQueueTab(tabQueue);
            tabs.TabPages.Add(tabQueue);

            // Tab 4 — Heap
            TabPage tabHeap = new TabPage("🏆 Top Performers (Heap)");
            tabHeap.BackColor = white;
            CreateHeapTab(tabHeap);
            tabs.TabPages.Add(tabHeap);

            // Tab 5 — Binary Search
            TabPage tabSearch = new TabPage("🔎 Binary Search");
            tabSearch.BackColor = white;
            CreateBinarySearchTab(tabSearch);
            tabs.TabPages.Add(tabSearch);

            this.Controls.Add(tabs);
        }

        // ═══════════════════════════════
        // TAB 1 — DICTIONARY
        // ═══════════════════════════════
        private void CreateDictionaryTab(TabPage tab)
        {
            Label lblInfo = new Label();
            lblInfo.Text = "📖 Dictionary: O(1) average time complexity — fastest lookup!";
            lblInfo.ForeColor = accent;
            lblInfo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblInfo.Location = new Point(15, 15);
            lblInfo.Size = new Size(700, 25);
            tab.Controls.Add(lblInfo);

            Label lblSearch = new Label();
            lblSearch.Text = "Find Employee ID :";
            lblSearch.ForeColor = textDark;
            lblSearch.Font = new Font("Segoe UI", 10);
            lblSearch.Location = new Point(15, 55);
            lblSearch.Size = new Size(180, 25);
            tab.Controls.Add(lblSearch);

            txtSearchID = new TextBox();
            txtSearchID.Location = new Point(200, 53);
            txtSearchID.Size = new Size(150, 30);
            txtSearchID.Font = new Font("Segoe UI", 10);
            tab.Controls.Add(txtSearchID);

            Button btnSearch = new Button();
            btnSearch.Text = "🔍 Search";
            btnSearch.Location = new Point(360, 52);
            btnSearch.Size = new Size(110, 32);
            btnSearch.BackColor = accent;
            btnSearch.ForeColor = white;
            btnSearch.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnDictSearch_Click;
            tab.Controls.Add(btnSearch);

            // Result
            Panel resultPanel = new Panel();
            resultPanel.Location = new Point(15, 100);
            resultPanel.Size = new Size(1020, 60);
            resultPanel.BackColor = Color.FromArgb(239, 246, 255);

            Label lblResult = new Label();
            lblResult.Name = "lblDictResult";
            lblResult.Text = "Result Show Here...";
            lblResult.ForeColor = textGray;
            lblResult.Font = new Font("Segoe UI", 11);
            lblResult.Dock = DockStyle.Fill;
            lblResult.TextAlign = ContentAlignment.MiddleCenter;
            resultPanel.Controls.Add(lblResult);
            tab.Controls.Add(resultPanel);

            // Dictionary Contents
            Label lblContents = new Label();
            lblContents.Text = "📋 Dictionary Contents (EmployeeID → Name):";
            lblContents.ForeColor = textDark;
            lblContents.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblContents.Location = new Point(15, 175);
            lblContents.Size = new Size(400, 25);
            tab.Controls.Add(lblContents);

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgvDict";
            dgv.Location = new Point(15, 205);
            dgv.Size = new Size(1020, 330);
            dgv.BackgroundColor = white;
            dgv.BorderStyle = BorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = primary;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 35;
            dgv.RowTemplate.Height = 32;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            tab.Controls.Add(dgv);
        }

        // ═══════════════════════════════
        // TAB 2 — SORTING
        // ═══════════════════════════════
        private void CreateSortingTab(TabPage tab)
        {
            Label lblInfo = new Label();
            lblInfo.Text = "📊 Sorting Algorithms: LINQ OrderBy (QuickSort internally) ";
            lblInfo.ForeColor = success;
            lblInfo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblInfo.Location = new Point(15, 15);
            lblInfo.Size = new Size(800, 25);
            tab.Controls.Add(lblInfo);

            // Sort Buttons
            Button btnSortName = new Button();
            btnSortName.Text = "🔤 Sort by Name";
            btnSortName.Location = new Point(15, 50);
            btnSortName.Size = new Size(160, 36);
            btnSortName.BackColor = accent;
            btnSortName.ForeColor = white;
            btnSortName.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSortName.FlatStyle = FlatStyle.Flat;
            btnSortName.FlatAppearance.BorderSize = 0;
            btnSortName.Click += (s, e) => SortEmployees("name");
            tab.Controls.Add(btnSortName);

            Button btnSortRating = new Button();
            btnSortRating.Text = "⭐ Sort by Rating";
            btnSortRating.Location = new Point(185, 50);
            btnSortRating.Size = new Size(160, 36);
            btnSortRating.BackColor = warning;
            btnSortRating.ForeColor = white;
            btnSortRating.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSortRating.FlatStyle = FlatStyle.Flat;
            btnSortRating.FlatAppearance.BorderSize = 0;
            btnSortRating.Click += (s, e) => SortEmployees("rating");
            tab.Controls.Add(btnSortRating);

            Button btnSortAttendance = new Button();
            btnSortAttendance.Text = "📅 Sort by Attendance";
            btnSortAttendance.Location = new Point(355, 50);
            btnSortAttendance.Size = new Size(180, 36);
            btnSortAttendance.BackColor = success;
            btnSortAttendance.ForeColor = white;
            btnSortAttendance.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSortAttendance.FlatStyle = FlatStyle.Flat;
            btnSortAttendance.FlatAppearance.BorderSize = 0;
            btnSortAttendance.Click += (s, e) => SortEmployees("attendance");
            tab.Controls.Add(btnSortAttendance);

            Label lblTime = new Label();
            lblTime.Name = "lblSortTime";
            lblTime.Text = "⏱ Sort time: --";
            lblTime.ForeColor = textGray;
            lblTime.Font = new Font("Segoe UI", 9);
            lblTime.Location = new Point(550, 58);
            lblTime.Size = new Size(300, 22);
            tab.Controls.Add(lblTime);

            DataGridView dgvSort = new DataGridView();
            dgvSort.Name = "dgvSort";
            dgvSort.Location = new Point(15, 100);
            dgvSort.Size = new Size(1020, 430);
            dgvSort.BackgroundColor = white;
            dgvSort.BorderStyle = BorderStyle.None;
            dgvSort.RowHeadersVisible = false;
            dgvSort.AllowUserToAddRows = false;
            dgvSort.ReadOnly = true;
            dgvSort.Font = new Font("Segoe UI", 9);
            dgvSort.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSort.ColumnHeadersDefaultCellStyle.BackColor = primary;
            dgvSort.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvSort.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSort.ColumnHeadersHeight = 35;
            dgvSort.RowTemplate.Height = 32;
            dgvSort.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            tab.Controls.Add(dgvSort);
        }

        // ═══════════════════════════════
        // TAB 3 — QUEUE
        // ═══════════════════════════════
        private void CreateQueueTab(TabPage tab)
        {
            Label lblInfo = new Label();
            lblInfo.Text = "📩 Queue (FIFO): First In First Out — O(1) enqueue/dequeue";
            lblInfo.ForeColor = purple;
            lblInfo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblInfo.Location = new Point(15, 15);
            lblInfo.Size = new Size(700, 25);
            tab.Controls.Add(lblInfo);

            // Enqueue
            Label lblAdd = new Label();
            lblAdd.Text = "Request add karo:";
            lblAdd.ForeColor = textDark;
            lblAdd.Font = new Font("Segoe UI", 10);
            lblAdd.Location = new Point(15, 55);
            lblAdd.Size = new Size(160, 25);
            tab.Controls.Add(lblAdd);

            TextBox txtRequest = new TextBox();
            txtRequest.Name = "txtQueueRequest";
            txtRequest.Location = new Point(180, 53);
            txtRequest.Size = new Size(300, 30);
            txtRequest.Font = new Font("Segoe UI", 10);
            txtRequest.Text = "Leave Request - Ali Hassan";
            tab.Controls.Add(txtRequest);

            Button btnEnqueue = new Button();
            btnEnqueue.Text = "➕ Enqueue";
            btnEnqueue.Location = new Point(490, 52);
            btnEnqueue.Size = new Size(110, 32);
            btnEnqueue.BackColor = success;
            btnEnqueue.ForeColor = white;
            btnEnqueue.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnEnqueue.FlatStyle = FlatStyle.Flat;
            btnEnqueue.FlatAppearance.BorderSize = 0;
            btnEnqueue.Click += (s, e) =>
            {
                Control[] c = tab.Controls.Find("txtQueueRequest", true);
                if (c.Length > 0 && !string.IsNullOrEmpty(c[0].Text))
                {
                    requestQueue.Enqueue(c[0].Text);
                    RefreshQueueDisplay(tab);
                }
            };
            tab.Controls.Add(btnEnqueue);

            Button btnDequeue = new Button();
            btnDequeue.Text = "✅ Process (Dequeue)";
            btnDequeue.Location = new Point(610, 52);
            btnDequeue.Size = new Size(170, 32);
            btnDequeue.BackColor = warning;
            btnDequeue.ForeColor = white;
            btnDequeue.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDequeue.FlatStyle = FlatStyle.Flat;
            btnDequeue.FlatAppearance.BorderSize = 0;
            btnDequeue.Click += (s, e) =>
            {
                if (requestQueue.Count > 0)
                {
                    string processed = requestQueue.Dequeue();
                    MessageBox.Show($"✅ Request Processed:\n\n{processed}", "Dequeued", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshQueueDisplay(tab);
                }
                else MessageBox.Show("Queue Empty!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            tab.Controls.Add(btnDequeue);

            // Queue Display
            Label lblQueue = new Label();
            lblQueue.Text = "📋 Queue Status:";
            lblQueue.ForeColor = textDark;
            lblQueue.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblQueue.Location = new Point(15, 100);
            lblQueue.Size = new Size(200, 25);
            tab.Controls.Add(lblQueue);

            Panel queueDisplay = new Panel();
            queueDisplay.Name = "queueDisplay";
            queueDisplay.Location = new Point(15, 130);
            queueDisplay.Size = new Size(1020, 380);
            queueDisplay.BackColor = Color.FromArgb(249, 250, 251);
            queueDisplay.AutoScroll = true;
            tab.Controls.Add(queueDisplay);

            // Load pending requests from DB
            LoadRequestsToQueue();
            RefreshQueueDisplay(tab);
        }

        // ═══════════════════════════════
        // TAB 4 — HEAP
        // ═══════════════════════════════
        private void CreateHeapTab(TabPage tab)
        {
            Label lblInfo = new Label();
            lblInfo.Text = "🏆 Max Heap: Find Top performers   — Best for rankings!";
            lblInfo.ForeColor = warning;
            lblInfo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblInfo.Location = new Point(15, 15);
            lblInfo.Size = new Size(800, 25);
            tab.Controls.Add(lblInfo);

            Button btnTopPerformers = new Button();
            btnTopPerformers.Text = "🏆 Show Top 5 Performers";
            btnTopPerformers.Location = new Point(15, 50);
            btnTopPerformers.Size = new Size(220, 36);
            btnTopPerformers.BackColor = warning;
            btnTopPerformers.ForeColor = white;
            btnTopPerformers.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnTopPerformers.FlatStyle = FlatStyle.Flat;
            btnTopPerformers.FlatAppearance.BorderSize = 0;
            btnTopPerformers.Click += BtnHeap_Click;
            tab.Controls.Add(btnTopPerformers);

            Button btnWarning = new Button();
            btnWarning.Text = "⚠️ Low Performers Warning";
            btnWarning.Location = new Point(245, 50);
            btnWarning.Size = new Size(220, 36);
            btnWarning.BackColor = danger;
            btnWarning.ForeColor = white;
            btnWarning.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnWarning.FlatStyle = FlatStyle.Flat;
            btnWarning.FlatAppearance.BorderSize = 0;
            btnWarning.Click += BtnLowPerformers_Click;
            tab.Controls.Add(btnWarning);

            Button btnPromotion = new Button();
            btnPromotion.Text = "🚀 Promotion Suggestions";
            btnPromotion.Location = new Point(475, 50);
            btnPromotion.Size = new Size(200, 36);
            btnPromotion.BackColor = success;
            btnPromotion.ForeColor = white;
            btnPromotion.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnPromotion.FlatStyle = FlatStyle.Flat;
            btnPromotion.FlatAppearance.BorderSize = 0;
            btnPromotion.Click += BtnPromotion_Click;
            tab.Controls.Add(btnPromotion);

            DataGridView dgvHeap = new DataGridView();
            dgvHeap.Name = "dgvHeap";
            dgvHeap.Location = new Point(15, 100);
            dgvHeap.Size = new Size(1020, 430);
            dgvHeap.BackgroundColor = white;
            dgvHeap.BorderStyle = BorderStyle.None;
            dgvHeap.RowHeadersVisible = false;
            dgvHeap.AllowUserToAddRows = false;
            dgvHeap.ReadOnly = true;
            dgvHeap.Font = new Font("Segoe UI", 9);
            dgvHeap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHeap.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 158, 11);
            dgvHeap.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvHeap.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvHeap.ColumnHeadersHeight = 35;
            dgvHeap.RowTemplate.Height = 35;
            dgvHeap.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235);
            tab.Controls.Add(dgvHeap);
        }

        // ═══════════════════════════════
        // TAB 5 — BINARY SEARCH
        // ═══════════════════════════════
        private void CreateBinarySearchTab(TabPage tab)
        {
            Label lblInfo = new Label();
            lblInfo.Text = "🔎 Binary Search:  Sorted array!";
            lblInfo.ForeColor = danger;
            lblInfo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblInfo.Location = new Point(15, 15);
            lblInfo.Size = new Size(700, 25);
            tab.Controls.Add(lblInfo);

            Label lblSearch = new Label();
            lblSearch.Text = "Employee Name search karo:";
            lblSearch.ForeColor = textDark;
            lblSearch.Font = new Font("Segoe UI", 10);
            lblSearch.Location = new Point(15, 55);
            lblSearch.Size = new Size(220, 25);
            tab.Controls.Add(lblSearch);

            TextBox txtBSearch = new TextBox();
            txtBSearch.Name = "txtBinarySearch";
            txtBSearch.Location = new Point(240, 53);
            txtBSearch.Size = new Size(200, 30);
            txtBSearch.Font = new Font("Segoe UI", 10);
            tab.Controls.Add(txtBSearch);

            Button btnBSearch = new Button();
            btnBSearch.Text = "🔎 Binary Search";
            btnBSearch.Location = new Point(450, 52);
            btnBSearch.Size = new Size(150, 32);
            btnBSearch.BackColor = danger;
            btnBSearch.ForeColor = white;
            btnBSearch.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBSearch.FlatStyle = FlatStyle.Flat;
            btnBSearch.FlatAppearance.BorderSize = 0;
            btnBSearch.Click += (s, e) =>
            {
                Control[] c = tab.Controls.Find("txtBinarySearch", true);
                if (c.Length > 0) BinarySearch(c[0].Text, tab);
            };
            tab.Controls.Add(btnBSearch);

            // Steps display
            RichTextBox rtbSteps = new RichTextBox();
            rtbSteps.Name = "rtbBinarySteps";
            rtbSteps.Location = new Point(15, 100);
            rtbSteps.Size = new Size(1020, 200);
            rtbSteps.Font = new Font("Consolas", 10);
            rtbSteps.BackColor = Color.FromArgb(15, 23, 42);
            rtbSteps.ForeColor = Color.FromArgb(16, 185, 129);
            rtbSteps.ReadOnly = true;
            rtbSteps.Text = "Binary Search steps yahan dikhenge...";
            tab.Controls.Add(rtbSteps);

            DataGridView dgvBSearch = new DataGridView();
            dgvBSearch.Name = "dgvBSearch";
            dgvBSearch.Location = new Point(15, 310);
            dgvBSearch.Size = new Size(1020, 220);
            dgvBSearch.BackgroundColor = white;
            dgvBSearch.BorderStyle = BorderStyle.None;
            dgvBSearch.RowHeadersVisible = false;
            dgvBSearch.AllowUserToAddRows = false;
            dgvBSearch.ReadOnly = true;
            dgvBSearch.Font = new Font("Segoe UI", 9);
            dgvBSearch.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBSearch.ColumnHeadersDefaultCellStyle.BackColor = danger;
            dgvBSearch.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvBSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvBSearch.ColumnHeadersHeight = 35;
            dgvBSearch.RowTemplate.Height = 32;
            tab.Controls.Add(dgvBSearch);
        }

        // ═══════════════════════════════
        // DATA LOADING
        // ═══════════════════════════════
        private void LoadDSAData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"SELECT e.EmployeeID, 
                                    e.FirstName + ' ' + e.LastName AS FullName,
                                    ISNULL(AVG(CAST(p.Rating AS FLOAT)),0) AS AvgRating,
                                    ISNULL(COUNT(CASE WHEN a.Status='Present' THEN 1 END),0) AS PresentDays
                                    FROM Employees e
                                    LEFT JOIN Performance p ON e.EmployeeID=p.EmployeeID
                                    LEFT JOIN Attendance a ON e.EmployeeID=a.EmployeeID
                                    WHERE e.IsActive=1
                                    GROUP BY e.EmployeeID, e.FirstName, e.LastName";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    employeeDict.Clear();
                    employeeList.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        int id = Convert.ToInt32(row["EmployeeID"]);
                        string name = row["FullName"].ToString();
                        double rating = Convert.ToDouble(row["AvgRating"]);
                        int present = Convert.ToInt32(row["PresentDays"]);

                        // Fill Dictionary
                        employeeDict[id] = name;

                        // Fill List for sorting
                        employeeList.Add((name, rating, present));
                    }

                    // Load Dictionary tab grid
                    Control[] dictGrids = this.Controls.Find("dgvDict", true);
                    if (dictGrids.Length > 0)
                    {
                        DataGridView dgv = (DataGridView)dictGrids[0];
                        DataTable dtDict = new DataTable();
                        dtDict.Columns.Add("Employee ID");
                        dtDict.Columns.Add("Employee Name");
                        dtDict.Columns.Add("Data Structure");
                        dtDict.Columns.Add("Lookup Time");

                        foreach (var kv in employeeDict)
                            dtDict.Rows.Add(kv.Key, kv.Value, "Dictionary<int, string>", "O(1)");

                        dgv.DataSource = dtDict;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("DSA Load Error: " + ex.Message); }
        }

        private void LoadRequestsToQueue()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(
                        @"SELECT e.FirstName + ' ' + e.LastName + ' - ' + r.RequestType AS RequestInfo
                          FROM Requests r JOIN Employees e ON r.EmployeeID=e.EmployeeID
                          WHERE r.Status='Pending' ORDER BY r.RequestDate", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    requestQueue.Clear();
                    foreach (DataRow row in dt.Rows)
                        requestQueue.Enqueue(row["RequestInfo"].ToString());
                }
            }
            catch { }
        }

        // ═══════════════════════════════
        // EVENT HANDLERS
        // ═══════════════════════════════
        private void BtnDictSearch_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtSearchID.Text, out int searchID))
            { MessageBox.Show("Valid Employee ID likho!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            Control[] results = this.Controls.Find("lblDictResult", true);
            if (results.Length > 0)
            {
                if (employeeDict.ContainsKey(searchID))
                {
                    results[0].Text = $"✅ Found! Employee ID {searchID} → {employeeDict[searchID]}  |  Time Complexity: O(1)";
                    results[0].ForeColor = success;
                }
                else
                {
                    results[0].Text = $"❌ Employee ID {searchID} dictionary mein nahi mila!";
                    results[0].ForeColor = danger;
                }
            }
        }

        private void SortEmployees(string by)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            List<(string name, double rating, int present)> sorted;

            if (by == "name")
                sorted = employeeList.OrderBy(e => e.name).ToList();
            else if (by == "rating")
                sorted = employeeList.OrderByDescending(e => e.rating).ToList();
            else
                sorted = employeeList.OrderByDescending(e => e.present).ToList();

            sw.Stop();

            Control[] grids = this.Controls.Find("dgvSort", true);
            if (grids.Length > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Rank");
                dt.Columns.Add("Employee Name");
                dt.Columns.Add("Avg Rating");
                dt.Columns.Add("Days Present");
                dt.Columns.Add("Algorithm");

                int rank = 1;
                foreach (var emp in sorted)
                    dt.Rows.Add(rank++, emp.name, emp.rating.ToString("F1"), emp.present, "QuickSort O(n log n)");

                ((DataGridView)grids[0]).DataSource = dt;
            }

            Control[] timeLabels = this.Controls.Find("lblSortTime", true);
            if (timeLabels.Length > 0)
                timeLabels[0].Text = $"⏱ Sort time: {sw.ElapsedMilliseconds}ms | Algorithm: QuickSort | O(n log n)";
        }

        private void RefreshQueueDisplay(TabPage tab)
        {
            Control[] panels = tab.Controls.Find("queueDisplay", true);
            if (panels.Length == 0) return;

            Panel panel = (Panel)panels[0];
            panel.Controls.Clear();

            if (requestQueue.Count == 0)
            {
                Label lblEmpty = new Label();
                lblEmpty.Text = "📭 Queue are Empty — No Request are pending!";
                lblEmpty.ForeColor = textGray;
                lblEmpty.Font = new Font("Segoe UI", 12);
                lblEmpty.Location = new Point(20, 20);
                lblEmpty.Size = new Size(600, 30);
                panel.Controls.Add(lblEmpty);
                return;
            }

            int py = 15;
            int position = 1;
            var items = requestQueue.ToArray();

            foreach (string item in items)
            {
                Panel itemPanel = new Panel();
                itemPanel.Size = new Size(980, 50);
                itemPanel.Location = new Point(15, py);
                itemPanel.BackColor = position == 1 ? Color.FromArgb(240, 253, 244) : white;

                Label lblPos = new Label();
                lblPos.Text = position == 1 ? "👉 FRONT" : $"#{position}";
                lblPos.ForeColor = position == 1 ? success : textGray;
                lblPos.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                lblPos.Location = new Point(10, 15);
                lblPos.Size = new Size(80, 22);
                itemPanel.Controls.Add(lblPos);

                Label lblItem = new Label();
                lblItem.Text = item;
                lblItem.ForeColor = textDark;
                lblItem.Font = new Font("Segoe UI", 10);
                lblItem.Location = new Point(100, 15);
                lblItem.Size = new Size(700, 22);
                itemPanel.Controls.Add(lblItem);

                Label lblDS = new Label();
                lblDS.Text = "Queue FIFO";
                lblDS.ForeColor = purple;
                lblDS.Font = new Font("Segoe UI", 8);
                lblDS.Location = new Point(820, 18);
                lblDS.Size = new Size(120, 18);
                itemPanel.Controls.Add(lblDS);

                panel.Controls.Add(itemPanel);
                py += 55;
                position++;
            }
        }

        private void BtnHeap_Click(object sender, EventArgs e)
        {
            var topPerformers = employeeList
                .OrderByDescending(emp => emp.rating)
                .ThenByDescending(emp => emp.present)
                .Take(5)
                .ToList();

            Control[] grids = this.Controls.Find("dgvHeap", true);
            if (grids.Length > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Rank");
                dt.Columns.Add("Employee Name");
                dt.Columns.Add("Avg Rating");
                dt.Columns.Add("Days Present");
                dt.Columns.Add("Status");
                dt.Columns.Add("Algorithm");

                int rank = 1;
                foreach (var emp in topPerformers)
                {
                    string status = emp.rating >= 4 ? "🏆 Top Performer" : emp.rating >= 3 ? "✅ Good" : "⚠️ Average";
                    dt.Rows.Add(rank++, emp.name, emp.rating.ToString("F1"), emp.present, status, "Max Heap O(n log n)");
                }

                ((DataGridView)grids[0]).DataSource = dt;
            }
        }

        private void BtnLowPerformers_Click(object sender, EventArgs e)
        {
            var lowPerformers = employeeList
                .Where(emp => emp.rating < 3 || emp.present < 10)
                .OrderBy(emp => emp.rating)
                .ToList();

            Control[] grids = this.Controls.Find("dgvHeap", true);
            if (grids.Length > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Employee Name");
                dt.Columns.Add("Avg Rating");
                dt.Columns.Add("Days Present");
                dt.Columns.Add("Warning");
                dt.Columns.Add("Action");

                foreach (var emp in lowPerformers)
                {
                    string warning = emp.rating < 2 ? "🔴 Critical" : "🟡 Warning";
                    string action = emp.rating < 2 ? "Immediate review needed" : "Performance improvement plan";
                    dt.Rows.Add(emp.name, emp.rating.ToString("F1"), emp.present, warning, action);
                }

                if (dt.Rows.Count == 0)
                    MessageBox.Show("✅ Koi low performer nahi! Sab theek hain!", "Great News", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    ((DataGridView)grids[0]).DataSource = dt;
            }
        }

        private void BtnPromotion_Click(object sender, EventArgs e)
        {
            var promotionCandidates = employeeList
                .Where(emp => emp.rating >= 4 && emp.present >= 20)
                .OrderByDescending(emp => emp.rating)
                .ToList();

            Control[] grids = this.Controls.Find("dgvHeap", true);
            if (grids.Length > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Employee Name");
                dt.Columns.Add("Avg Rating");
                dt.Columns.Add("Days Present");
                dt.Columns.Add("Promotion Status");
                dt.Columns.Add("Recommendation");

                foreach (var emp in promotionCandidates)
                {
                    string rec = emp.rating >= 4.5 ? "Strongly Recommended" : "Recommended";
                    dt.Rows.Add(emp.name, emp.rating.ToString("F1"), emp.present, "🚀 Eligible", rec);
                }

                if (dt.Rows.Count == 0)
                    MessageBox.Show("Abhi koi employee promotion criteria meet nahi karta.\n\nCriteria: Rating ≥ 4.0 AND Present Days ≥ 20", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    ((DataGridView)grids[0]).DataSource = dt;
            }
        }

        private void BinarySearch(string searchName, TabPage tab)
        {
            var sortedList = employeeList.OrderBy(e => e.name).ToList();
            int low = 0, high = sortedList.Count - 1;
            int steps = 0;
            string log = $"🔎 Binary Search for: '{searchName}'\n";
            log += $"📋 Total employees: {sortedList.Count}\n";
            log += new string('─', 60) + "\n";

            int foundIndex = -1;

            while (low <= high)
            {
                int mid = (low + high) / 2;
                steps++;
                log += $"Step {steps}: low={low}, high={high}, mid={mid} → checking '{sortedList[mid].name}'\n";

                int compare = string.Compare(sortedList[mid].name, searchName, StringComparison.OrdinalIgnoreCase);

                if (compare == 0) { foundIndex = mid; break; }
                else if (compare < 0) { low = mid + 1; log += $"  → '{sortedList[mid].name}' < '{searchName}', search right half\n"; }
                else { high = mid - 1; log += $"  → '{sortedList[mid].name}' > '{searchName}', search left half\n"; }
            }

            log += new string('─', 60) + "\n";
            if (foundIndex >= 0)
                log += $"✅ FOUND at index {foundIndex}! Steps: {steps} | O(log n) vs O(n) linear search";
            else
                log += $"❌ NOT FOUND. Steps taken: {steps} | O(log n) complexity";

            Control[] rtbs = tab.Controls.Find("rtbBinarySteps", true);
            if (rtbs.Length > 0) rtbs[0].Text = log;

            // Show results
            Control[] grids = this.Controls.Find("dgvBSearch", true);
            if (grids.Length > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Index");
                dt.Columns.Add("Employee Name");
                dt.Columns.Add("Match");

                for (int i = 0; i < sortedList.Count; i++)
                {
                    bool isMatch = string.Compare(sortedList[i].name, searchName, StringComparison.OrdinalIgnoreCase) == 0;
                    dt.Rows.Add(i, sortedList[i].name, isMatch ? "✅ FOUND" : "");
                }

                ((DataGridView)grids[0]).DataSource = dt;
            }
        }

        private void DSADemoForm_Load(object sender, EventArgs e)
        {

        }
    }
}