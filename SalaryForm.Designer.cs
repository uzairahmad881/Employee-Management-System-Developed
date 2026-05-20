namespace EmployeeManagementSystem1
{
    partial class SalaryForm
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
            // SalaryForm
            // 
            ClientSize = new Size(278, 244);
            Name = "SalaryForm";
            Load += SalaryForm_Load;
            ResumeLayout(false);
        }
    }
}