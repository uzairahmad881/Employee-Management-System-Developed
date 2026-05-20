using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class SplashScreen : Form
    {
        private System.Windows.Forms.Timer splashTimer;
        private int countdown = 30; // 3 seconds (30 × 100ms)
        private ProgressBar progressBar;
        private Label lblBuiltBy;
        private Label lblLoading;
        private int animStep = 0;
        private System.Windows.Forms.Timer animTimer;

        public SplashScreen()
        {
            BuildSplash();
            StartTimers();
        }

        private void BuildSplash()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(640, 360);
            this.BackColor = Color.FromArgb(10, 22, 40); // #0A1628
            this.DoubleBuffered = true;

            // ── LOGO PANEL (SVG drawn via GDI) ────────────────
            Panel logoPanel = new Panel();
            logoPanel.Size = new Size(480, 160);
            logoPanel.Location = new Point(80, 60);
            logoPanel.BackColor = Color.Transparent;
            logoPanel.Paint += LogoPanel_Paint;
            this.Controls.Add(logoPanel);

            // ── "Built by" label ──────────────────────────────
            lblBuiltBy = new Label();
            lblBuiltBy.Text = "This Project build by  Uzair Ahmad";
            lblBuiltBy.ForeColor = Color.FromArgb(168, 192, 214); // #A8C0D6
            lblBuiltBy.Font = new Font("Trebuchet MS", 10f, FontStyle.Italic);
            lblBuiltBy.TextAlign = ContentAlignment.MiddleCenter;
            lblBuiltBy.Size = new Size(480, 24);
            lblBuiltBy.Location = new Point(80, 232);
            lblBuiltBy.BackColor = Color.Transparent;
            this.Controls.Add(lblBuiltBy);

            // ── Loading label ─────────────────────────────────
            lblLoading = new Label();
            lblLoading.Text = "Loading...";
            lblLoading.ForeColor = Color.FromArgb(0, 201, 167); // #00C9A7
            lblLoading.Font = new Font("Segoe UI", 9f);
            lblLoading.TextAlign = ContentAlignment.MiddleCenter;
            lblLoading.Size = new Size(480, 20);
            lblLoading.Location = new Point(80, 262);
            lblLoading.BackColor = Color.Transparent;
            this.Controls.Add(lblLoading);

            // ── Progress bar ──────────────────────────────────
            progressBar = new ProgressBar();
            progressBar.Size = new Size(480, 6);
            progressBar.Location = new Point(80, 288);
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.ForeColor = Color.FromArgb(0, 201, 167);
            progressBar.BackColor = Color.FromArgb(17, 34, 64);
            this.Controls.Add(progressBar);

            //// ── Version / copyright ───────────────────────────
            //Label lblVer = new Label();
            //lblVer.Text = "v1.0   •   © 2026 EMS";
            //lblVer.ForeColor = Color.FromArgb(60, 100, 140);
            //lblVer.Font = new Font("Segoe UI", 8f);
            //lblVer.TextAlign = ContentAlignment.MiddleCenter;
            //lblVer.Size = new Size(480, 18);
            //lblVer.Location = new Point(80, 330);
            //lblVer.BackColor = Color.Transparent;
            //this.Controls.Add(lblVer);
        }

        // ══════════════════════════════════════════════════════
        //  LOGO PAINT — replicates the SVG design via GDI+
        // ══════════════════════════════════════════════════════
        private void LogoPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            Panel p = (Panel)sender;
            Rectangle bounds = new Rectangle(0, 0, p.Width, p.Height);

            // Background pill
            using (GraphicsPath path = RoundRect(bounds, 18))
            using (LinearGradientBrush bg = new LinearGradientBrush(
                bounds,
                Color.FromArgb(10, 22, 40),
                Color.FromArgb(17, 34, 64),
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillPath(bg, path);
                using (Pen border = new Pen(Color.FromArgb(0, 201, 167), 1f))
                    g.DrawPath(border, path);
            }

            // Left accent bar
            using (LinearGradientBrush accent = new LinearGradientBrush(
                new Rectangle(0, 0, 5, p.Height),
                Color.FromArgb(0, 201, 167),
                Color.FromArgb(0, 132, 255),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(accent, 0, 0, 5, p.Height);
            }

            // ── People icon ───────────────────────────────────
            Color teal = Color.FromArgb(0, 201, 167);
            Color blue = Color.FromArgb(0, 132, 255);

            // Center person
            using (LinearGradientBrush iconBr = new LinearGradientBrush(
                new Rectangle(52, 22, 56, 56), teal, blue, LinearGradientMode.ForwardDiagonal))
            {
                g.FillEllipse(iconBr, 71, 22, 18, 18);
                using (GraphicsPath arc = new GraphicsPath())
                {
                    arc.AddArc(62, 40, 36, 36, 180, 180);
                    arc.CloseFigure();
                    g.FillPath(iconBr, arc);
                }
            }

            // Left person
            using (SolidBrush tealBr = new SolidBrush(Color.FromArgb(178, 0, 201, 167)))
            {
                g.FillEllipse(tealBr, 50, 29, 13, 13);
                GraphicsPath lp = new GraphicsPath();
                lp.AddArc(44, 44, 26, 26, 180, 180);
                lp.CloseFigure();
                g.FillPath(tealBr, lp);
            }

            // Right person
            using (SolidBrush blueBr = new SolidBrush(Color.FromArgb(178, 0, 132, 255)))
            {
                g.FillEllipse(blueBr, 97, 29, 13, 13);
                GraphicsPath rp = new GraphicsPath();
                rp.AddArc(90, 44, 26, 26, 180, 180);
                rp.CloseFigure();
                g.FillPath(blueBr, rp);
            }

            // Org chart lines
            using (Pen linePen = new Pen(Color.FromArgb(120, 0, 201, 167), 1.2f))
            {
                linePen.DashStyle = DashStyle.Dash;
                g.DrawLine(linePen, 80, 42, 80, 52);
                g.DrawLine(linePen, 57, 52, 103, 52);
                g.DrawLine(linePen, 57, 52, 57, 42);
                g.DrawLine(linePen, 103, 52, 103, 42);
            }

            // ── EMS text ──────────────────────────────────────
            using (LinearGradientBrush txtBr = new LinearGradientBrush(
                new Rectangle(130, 10, 290, 70),
                Color.FromArgb(0, 201, 167),
                Color.FromArgb(0, 132, 255),
                LinearGradientMode.Horizontal))
            using (Font emsFont = new Font("Georgia", 56, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                g.DrawString("EMS", emsFont, txtBr, new PointF(130, 10));
            }

            // Divider line
            using (LinearGradientBrush divBr = new LinearGradientBrush(
                new Rectangle(130, 82, 290, 2),
                Color.FromArgb(80, 0, 201, 167),
                Color.FromArgb(0, 0, 132, 255),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(divBr, 130, 82, 290, 1);
            }

            // Subtitle
            using (Font subFont = new Font("Trebuchet MS", 11, FontStyle.Regular, GraphicsUnit.Pixel))
            using (SolidBrush subBr = new SolidBrush(Color.FromArgb(168, 192, 214)))
            {
                g.DrawString("EMPLOYEE MANAGEMENT", subFont, subBr, new PointF(132, 88));
            }

            // System label
            using (Font sysFont = new Font("Trebuchet MS", 9, FontStyle.Regular, GraphicsUnit.Pixel))
            using (SolidBrush sysBr = new SolidBrush(Color.FromArgb(120, 0, 201, 167)))
            {
                g.DrawString("SYSTEM", sysFont, sysBr, new PointF(133, 106));
            }

            // Corner glow
            using (GraphicsPath glow = new GraphicsPath())
            {
                glow.AddEllipse(420, -10, 60, 60);
                using (PathGradientBrush glowBr = new PathGradientBrush(glow))
                {
                    glowBr.CenterColor = Color.FromArgb(18, 0, 132, 255);
                    glowBr.SurroundColors = new[] { Color.Transparent };
                    g.FillPath(glowBr, glow);
                }
            }
        }

        private GraphicsPath RoundRect(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        // ══════════════════════════════════════════════════════
        //  TIMERS
        // ══════════════════════════════════════════════════════
        private void StartTimers()
        {
            // Splash close timer (3 seconds)
            splashTimer = new System.Windows.Forms.Timer();
            splashTimer.Interval = 100;
            splashTimer.Tick += SplashTimer_Tick;
            splashTimer.Start();

            // Loading text animation
            animTimer = new System.Windows.Forms.Timer();
            animTimer.Interval = 400;
            animTimer.Tick += (s, e) =>
            {
                animStep = (animStep + 1) % 4;
                string dots = new string('.', animStep);
                lblLoading.Text = "Loading" + dots.PadRight(3);
            };
            animTimer.Start();
        }

        private void SplashTimer_Tick(object sender, EventArgs e)
        {
            countdown--;
            int progress = (int)(((30 - countdown) / 30.0) * 100);
            if (progressBar.Value < progress) progressBar.Value = progress;

            if (countdown <= 0)
            {
                splashTimer.Stop();
                animTimer.Stop();

                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Hide();  // ✅ Close ki jagah Hide karo
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Outer border glow
            using (Pen pen = new Pen(Color.FromArgb(40, 0, 201, 167), 2f))
                e.Graphics.DrawRectangle(pen, 1, 1, this.Width - 3, this.Height - 3);
        }
    }
}