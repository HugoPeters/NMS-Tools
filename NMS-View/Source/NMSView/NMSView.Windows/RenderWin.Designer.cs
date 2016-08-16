namespace NMSView
{
    partial class RenderWin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGeometryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFloorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenderPanel = new System.Windows.Forms.Panel();
            this.rotateXYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1584, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openGeometryToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openGeometryToolStripMenuItem
            // 
            this.openGeometryToolStripMenuItem.Name = "openGeometryToolStripMenuItem";
            this.openGeometryToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.openGeometryToolStripMenuItem.Text = "Open Geometry Bin...";
            this.openGeometryToolStripMenuItem.Click += new System.EventHandler(this.openGeometryToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeFloorToolStripMenuItem,
            this.resetCameraToolStripMenuItem,
            this.rotateXYToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // removeFloorToolStripMenuItem
            // 
            this.removeFloorToolStripMenuItem.Name = "removeFloorToolStripMenuItem";
            this.removeFloorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.removeFloorToolStripMenuItem.Text = "Remove Floor";
            this.removeFloorToolStripMenuItem.Click += new System.EventHandler(this.removeFloorToolStripMenuItem_Click);
            // 
            // resetCameraToolStripMenuItem
            // 
            this.resetCameraToolStripMenuItem.Name = "resetCameraToolStripMenuItem";
            this.resetCameraToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.resetCameraToolStripMenuItem.Text = "Reset Camera";
            this.resetCameraToolStripMenuItem.Click += new System.EventHandler(this.resetCameraToolStripMenuItem_Click);
            // 
            // RenderPanel
            // 
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 24);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(1584, 837);
            this.RenderPanel.TabIndex = 1;
            // 
            // rotateXYToolStripMenuItem
            // 
            this.rotateXYToolStripMenuItem.Name = "rotateXYToolStripMenuItem";
            this.rotateXYToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rotateXYToolStripMenuItem.Text = "Rotate X + Y";
            this.rotateXYToolStripMenuItem.Click += new System.EventHandler(this.rotateXYToolStripMenuItem_Click);
            // 
            // RenderWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1584, 861);
            this.Controls.Add(this.RenderPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RenderWin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RenderWin";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGeometryToolStripMenuItem;
        public System.Windows.Forms.Panel RenderPanel;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeFloorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateXYToolStripMenuItem;
    }
}