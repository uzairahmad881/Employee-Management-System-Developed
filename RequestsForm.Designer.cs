namespace EmployeeManagementSystem1
{
    partial class RequestsForm
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
            // RequestsForm
            // 
            ClientSize = new Size(278, 244);
            Name = "RequestsForm";
            Load += RequestsForm_Load;
            ResumeLayout(false);
        }
    }
}