using System;
using System.Windows.Forms;

namespace CanetisRadar
{
    partial class Overlay
    {
        public IntPtr ParentHandle;
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox RadarBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(Overlay));
            this.RadarBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize) (this.RadarBox)).BeginInit();
            this.SuspendLayout();
            
            // Bruh hardcoded screen 2, I'm tired today oof
            var yPosRadar = Screen.AllScreens[2].WorkingArea.Height / 2 - 150 / 2;
            this.RadarBox.Location = new System.Drawing.Point(12, yPosRadar);
            this.RadarBox.Name = "RadarBox";
            this.RadarBox.Size = new System.Drawing.Size(150, 150);
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 512);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Overlay";
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Screen.AllScreens[2].WorkingArea.Location;
            this.Text = "CanetisRadar Overlay";
            this.TopMost = true;
            
            this.Controls.Add(this.RadarBox);
            
            this.Load += new System.EventHandler(this.Overlay_Load);
            ((System.ComponentModel.ISupportInitialize) (this.RadarBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}