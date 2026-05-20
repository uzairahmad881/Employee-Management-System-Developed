namespace EmployeeManagementSystem1
{
    partial class HRDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // HRDashboard
            // 
            ClientSize = new Size(278, 244);
            Name = "HRDashboard";
            Load += HRDashboard_Load;
            ResumeLayout(false);
        }
    }
}