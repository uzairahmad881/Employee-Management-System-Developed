namespace EmployeeManagementSystem1
{
    partial class AttendanceForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // AttendanceForm
            // 
            ClientSize = new Size(278, 244);
            Name = "AttendanceForm";
            Load += AttendanceForm_Load;
            ResumeLayout(false);
        }
    }
}