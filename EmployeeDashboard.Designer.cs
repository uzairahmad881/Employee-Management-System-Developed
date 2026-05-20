namespace EmployeeManagementSystem1
{
    partial class EmployeeDashboard
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
            // EmployeeDashboard
            // 
            ClientSize = new Size(278, 244);
            Name = "EmployeeDashboard";
            Load += EmployeeDashboard_Load;
            ResumeLayout(false);
        }
    }
}