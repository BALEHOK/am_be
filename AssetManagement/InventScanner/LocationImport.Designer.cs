namespace InventScanner
{
    partial class LocationImport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocationImport));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.selectedListBox = new System.Windows.Forms.ListBox();
            this.aLabel = new System.Windows.Forms.Label();
            this.existingListBox = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.moveSelToRight = new System.Windows.Forms.Button();
            this.moveAllToRight = new System.Windows.Forms.Button();
            this.moveSelToLeft = new System.Windows.Forms.Button();
            this.moveAllToLeft = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.selectedListBox, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.aLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.existingListBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // selectedListBox
            // 
            this.selectedListBox.DisplayMember = "Name";
            resources.ApplyResources(this.selectedListBox, "selectedListBox");
            this.selectedListBox.FormattingEnabled = true;
            this.selectedListBox.Name = "selectedListBox";
            this.selectedListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.selectedListBox.ValueMember = "UID";
            // 
            // aLabel
            // 
            resources.ApplyResources(this.aLabel, "aLabel");
            this.aLabel.Name = "aLabel";
            // 
            // existingListBox
            // 
            this.existingListBox.DisplayMember = "Name";
            resources.ApplyResources(this.existingListBox, "existingListBox");
            this.existingListBox.FormattingEnabled = true;
            this.existingListBox.Name = "existingListBox";
            this.existingListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.existingListBox.ValueMember = "UID";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.moveSelToRight);
            this.flowLayoutPanel1.Controls.Add(this.moveAllToRight);
            this.flowLayoutPanel1.Controls.Add(this.moveSelToLeft);
            this.flowLayoutPanel1.Controls.Add(this.moveAllToLeft);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // moveSelToRight
            // 
            resources.ApplyResources(this.moveSelToRight, "moveSelToRight");
            this.moveSelToRight.Name = "moveSelToRight";
            this.moveSelToRight.UseVisualStyleBackColor = true;
            this.moveSelToRight.Click += new System.EventHandler(this.moveSelToRight_Click);
            // 
            // moveAllToRight
            // 
            resources.ApplyResources(this.moveAllToRight, "moveAllToRight");
            this.moveAllToRight.Name = "moveAllToRight";
            this.moveAllToRight.UseVisualStyleBackColor = true;
            this.moveAllToRight.Click += new System.EventHandler(this.moveAllToRight_Click);
            // 
            // moveSelToLeft
            // 
            resources.ApplyResources(this.moveSelToLeft, "moveSelToLeft");
            this.moveSelToLeft.Name = "moveSelToLeft";
            this.moveSelToLeft.UseVisualStyleBackColor = true;
            this.moveSelToLeft.Click += new System.EventHandler(this.moveSelToLeft_Click);
            // 
            // moveAllToLeft
            // 
            resources.ApplyResources(this.moveAllToLeft, "moveAllToLeft");
            this.moveAllToLeft.Name = "moveAllToLeft";
            this.moveAllToLeft.UseVisualStyleBackColor = true;
            this.moveAllToLeft.Click += new System.EventHandler(this.button5_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.button1);
            this.flowLayoutPanel2.Controls.Add(this.button3);
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // LocationImport
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "LocationImport";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LocationImport_FormClosing);
            this.Load += new System.EventHandler(this.LocationImport_Load);
            this.Shown += new System.EventHandler(this.LocationImport_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label aLabel;
        private System.Windows.Forms.ListBox selectedListBox;
        private System.Windows.Forms.ListBox existingListBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button moveSelToRight;
        private System.Windows.Forms.Button moveAllToRight;
        private System.Windows.Forms.Button moveSelToLeft;
        private System.Windows.Forms.Button moveAllToLeft;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
    }
}