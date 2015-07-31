using System.ComponentModel;
using System.Windows.Forms;

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
            this.globalHotkeysCheckBox.Location = new System.Drawing.Point(13, 13);
            this.globalHotkeysCheckBox.Name = "globalHotkeysCheckBox";
            this.globalHotkeysCheckBox.Size = new System.Drawing.Size(98, 17);
            this.globalHotkeysCheckBox.TabIndex = 0;
            this.globalHotkeysCheckBox.Text = "Global Hotkeys";
            this.globalHotkeysCheckBox.UseVisualStyleBackColor = true;
            this.globalHotkeysCheckBox.CheckedChanged += new System.EventHandler(this.globalHotkeysCheckBox_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
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
    }
}

