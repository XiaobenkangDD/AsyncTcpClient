namespace LTKJ.RTU
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.pnlTop = new System.Windows.Forms.Panel();
            this.tsMenu = new System.Windows.Forms.ToolStrip();
            this.tsbtnClose = new System.Windows.Forms.ToolStripButton();
            this.onLineLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tvArea = new System.Windows.Forms.TreeView();
            this.imgListTree = new System.Windows.Forms.ImageList(this.components);
            this.icnSys = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsSys = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsbtnExit = new System.Windows.Forms.ToolStripMenuItem();
            this.panLeft = new System.Windows.Forms.Panel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.portText = new System.Windows.Forms.TextBox();
            this.ServerSwitchButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textReceive = new System.Windows.Forms.TextBox();
            this.textSend = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SendButton = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            this.tsMenu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.cmsSys.SuspendLayout();
            this.panLeft.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.Controls.Add(this.tsMenu);
            this.pnlTop.Name = "pnlTop";
            // 
            // tsMenu
            // 
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnClose});
            resources.ApplyResources(this.tsMenu, "tsMenu");
            this.tsMenu.Name = "tsMenu";
            // 
            // tsbtnClose
            // 
            this.tsbtnClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.tsbtnClose, "tsbtnClose");
            this.tsbtnClose.Name = "tsbtnClose";
            // 
            // onLineLabel
            // 
            resources.ApplyResources(this.onLineLabel, "onLineLabel");
            this.onLineLabel.Name = "onLineLabel";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tvArea);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tvArea
            // 
            resources.ApplyResources(this.tvArea, "tvArea");
            this.tvArea.ImageList = this.imgListTree;
            this.tvArea.Name = "tvArea";
            // 
            // imgListTree
            // 
            this.imgListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListTree.ImageStream")));
            this.imgListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListTree.Images.SetKeyName(0, "shape_square_go.png");
            this.imgListTree.Images.SetKeyName(1, "shape_square_delete.png");
            this.imgListTree.Images.SetKeyName(2, "shape_square_error.png");
            // 
            // icnSys
            // 
            this.icnSys.ContextMenuStrip = this.cmsSys;
            resources.ApplyResources(this.icnSys, "icnSys");
            // 
            // cmsSys
            // 
            this.cmsSys.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsSys.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnExit});
            this.cmsSys.Name = "cmsSys";
            resources.ApplyResources(this.cmsSys, "cmsSys");
            // 
            // tsbtnExit
            // 
            this.tsbtnExit.Name = "tsbtnExit";
            resources.ApplyResources(this.tsbtnExit, "tsbtnExit");
            // 
            // panLeft
            // 
            this.panLeft.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.panLeft, "panLeft");
            this.panLeft.Name = "panLeft";
            // 
            // pnlCenter
            // 
            this.pnlCenter.Controls.Add(this.pnlGrid);
            resources.ApplyResources(this.pnlCenter, "pnlCenter");
            this.pnlCenter.Name = "pnlCenter";
            // 
            // pnlGrid
            // 
            this.pnlGrid.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.pnlGrid, "pnlGrid");
            this.pnlGrid.Name = "pnlGrid";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.portText);
            this.groupBox2.Controls.Add(this.ServerSwitchButton);
            this.groupBox2.Controls.Add(this.onLineLabel);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // portText
            // 
            resources.ApplyResources(this.portText, "portText");
            this.portText.Name = "portText";
            // 
            // ServerSwitchButton
            // 
            resources.ApplyResources(this.ServerSwitchButton, "ServerSwitchButton");
            this.ServerSwitchButton.Name = "ServerSwitchButton";
            this.ServerSwitchButton.UseVisualStyleBackColor = true;           
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textReceive);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Name = "comboBox1";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textSend);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // textReceive
            // 
            resources.ApplyResources(this.textReceive, "textReceive");
            this.textReceive.Name = "textReceive";
            // 
            // textSend
            // 
            resources.ApplyResources(this.textSend, "textSend");
            this.textSend.Name = "textSend";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.SendButton);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.comboBox1);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // SendButton
            // 
            resources.ApplyResources(this.SendButton, "SendButton");
            this.SendButton.Name = "SendButton";
            this.SendButton.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.panLeft);
            this.Controls.Add(this.pnlTop);
            this.Name = "FrmMain";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.cmsSys.ResumeLayout(false);
            this.panLeft.ResumeLayout(false);
            this.pnlCenter.ResumeLayout(false);
            this.pnlGrid.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.NotifyIcon icnSys;
        private System.Windows.Forms.ContextMenuStrip cmsSys;
        private System.Windows.Forms.ToolStripMenuItem tsbtnExit;
        private System.Windows.Forms.ToolStrip tsMenu;
        private System.Windows.Forms.TreeView tvArea;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panLeft;
        private System.Windows.Forms.Panel pnlCenter;
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ImageList imgListTree;
        private System.Windows.Forms.ToolStripButton tsbtnClose;
        private System.Windows.Forms.Label onLineLabel;
        private System.Windows.Forms.TextBox portText;
        private System.Windows.Forms.Button ServerSwitchButton;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textSend;
        private System.Windows.Forms.TextBox textReceive;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SendButton;
    }
}

