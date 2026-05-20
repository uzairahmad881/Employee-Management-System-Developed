namespace EmployeeManagementSystem
{
    partial class LoginForm
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
            // LoginForm
            // 
            ClientSize = new Size(867, 556);
            Name = "LoginForm";
            Load += LoginForm_Load;
            ResumeLayout(false);
        }
    }
}