namespace EmployeeManagementSystem1
{
    partial class DSADemoForm
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
            // DSADemoForm
            // 
            ClientSize = new Size(278, 244);
            Name = "DSADemoForm";
            Load += DSADemoForm_Load;
            ResumeLayout(false);
        }
    }
}