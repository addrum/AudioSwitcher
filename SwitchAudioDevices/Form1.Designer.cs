using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SwitchAudioDevices
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.globalHotkeysCheckBox = new System.Windows.Forms.CheckBox();
            this.doubleClickCheckBox = new System.Windows.Forms.CheckBox();
            this.globalEventProvider = new Gma.UserActivityMonitor.GlobalEventProvider();
            this.hotkeysLabel = new System.Windows.Forms.Label();
            this.hotkeysTextBox = new System.Windows.Forms.TextBox();
            this.startupCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "Switch Audio Devices";
            this.NotifyIcon.Visible = true;
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // globalHotkeysCheckBox
            // 
            this.globalHotkeysCheckBox.AutoSize = true;
            this.globalHotkeysCheckBox.Location = new System.Drawing.Point(12, 58);
            this.globalHotkeysCheckBox.Name = "globalHotkeysCheckBox";
            this.globalHotkeysCheckBox.Size = new System.Drawing.Size(98, 17);
            this.globalHotkeysCheckBox.TabIndex = 0;
            this.globalHotkeysCheckBox.Text = "Global Hotkeys";
            this.globalHotkeysCheckBox.UseVisualStyleBackColor = true;
            this.globalHotkeysCheckBox.CheckedChanged += new System.EventHandler(this.globalHotkeysCheckBox_CheckedChanged);
            // 
            // doubleClickCheckBox
            // 
            this.doubleClickCheckBox.AutoSize = true;
            this.doubleClickCheckBox.Location = new System.Drawing.Point(12, 35);
            this.doubleClickCheckBox.Name = "doubleClickCheckBox";
            this.doubleClickCheckBox.Size = new System.Drawing.Size(194, 17);
            this.doubleClickCheckBox.TabIndex = 1;
            this.doubleClickCheckBox.Text = "Double click to cycle audio devices";
            this.doubleClickCheckBox.UseVisualStyleBackColor = true;
            this.doubleClickCheckBox.CheckedChanged += new System.EventHandler(this.doubleClickCheckBox_CheckedChanged);
            // 
            // hotkeysLabel
            // 
            this.hotkeysLabel.AutoSize = true;
            this.hotkeysLabel.Location = new System.Drawing.Point(9, 84);
            this.hotkeysLabel.Name = "hotkeysLabel";
            this.hotkeysLabel.Size = new System.Drawing.Size(49, 13);
            this.hotkeysLabel.TabIndex = 2;
            this.hotkeysLabel.Text = "Hotkeys:";
            // 
            // hotkeysTextBox
            // 
            this.hotkeysTextBox.Location = new System.Drawing.Point(67, 81);
            this.hotkeysTextBox.Name = "hotkeysTextBox";
            this.hotkeysTextBox.ReadOnly = true;
            this.hotkeysTextBox.Size = new System.Drawing.Size(139, 20);
            this.hotkeysTextBox.TabIndex = 3;
            this.hotkeysTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.hotkeysTextBox_MouseClick);
            this.hotkeysTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hotkeysTextBox_KeyUp);
            // 
            // startupCheckBox
            // 
            this.startupCheckBox.AutoSize = true;
            this.startupCheckBox.Location = new System.Drawing.Point(12, 12);
            this.startupCheckBox.Name = "startupCheckBox";
            this.startupCheckBox.Size = new System.Drawing.Size(93, 17);
            this.startupCheckBox.TabIndex = 4;
            this.startupCheckBox.Text = "Run at startup";
            this.startupCheckBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.startupCheckBox);
            this.Controls.Add(this.hotkeysTextBox);
            this.Controls.Add(this.hotkeysLabel);
            this.Controls.Add(this.doubleClickCheckBox);
            this.Controls.Add(this.globalHotkeysCheckBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ClientSizeChanged += new System.EventHandler(this.Form1_ClientSizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NotifyIcon NotifyIcon;
        private CheckBox globalHotkeysCheckBox;
        private CheckBox doubleClickCheckBox;
        private Gma.UserActivityMonitor.GlobalEventProvider globalEventProvider;
        private Label hotkeysLabel;
        private TextBox hotkeysTextBox;
        private CheckBox startupCheckBox;
    }
}

