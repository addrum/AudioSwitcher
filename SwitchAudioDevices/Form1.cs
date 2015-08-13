using System;
using System.Windows.Forms;
using SwitchAudioDevices.Properties;

namespace SwitchAudioDevices
{
    public partial class Form1 : Form
    {
        private readonly ContextMenu _menu;
        public bool DoubleClickToCycle { get; set; }
        public bool GlobalHotkeys { get; set; }

        // http://stackoverflow.com/a/27309185/1860436
        readonly KeyboardHook _hook = new KeyboardHook();

        public Form1()
        {
            InitializeComponent();
            _menu = new ContextMenu();
            NotifyIcon.ContextMenu = _menu;
            Program.PopulateDeviceList(_menu);
            AddPreferencesAndExit();
            PresetValuesOfSettings();

            // reigster the event that is fired after the key press
            _hook.KeyPressed += hook_KeyPressed;
            // register the control + alt + F12 combination as hot key
            _hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Alt, Keys.F12);
        }

        private void AddPreferencesAndExit()
        {
            // Add preferences
            var preferencesItem = new MenuItem { Text = Resources.Preferences };
            preferencesItem.Click += OpenPreferences;
            _menu.MenuItems.Add("-");
            _menu.MenuItems.Add(preferencesItem);

            // Add an exit button
            var exitItem = new MenuItem { Text = Resources.Exit };
            exitItem.Click += OnExit;
            _menu.MenuItems.Add("-");
            _menu.MenuItems.Add(exitItem);
        }

        private void PresetValuesOfSettings()
        {
            doubleClickCheckBox.Checked = Settings.Default.DoubleClickToCycle;
            globalHotkeysCheckBox.Checked = Settings.Default.GlobalHotkeys;
        }

        private void OpenPreferences(object sender, EventArgs e)
        {
            Visible = true;

            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;

            Activate();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Icon = null;
            Dispose(true);
            Application.Exit();
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!DoubleClickToCycle) return;
            Program.SelectDevice(Program.NextId());
            ShowBalloonTip();
        }

        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                Visible = false;
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
             if (!GlobalHotkeys) return;
             Program.SelectDevice(Program.NextId());
             ShowBalloonTip();
        }

        private void doubleClickCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DoubleClickToCycle = doubleClickCheckBox.Checked;
            Settings.Default.DoubleClickToCycle = doubleClickCheckBox.Checked;
            Settings.Default.Save();
        }

        private void globalHotkeysCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GlobalHotkeys = globalHotkeysCheckBox.Checked;
            Settings.Default.GlobalHotkeys = globalHotkeysCheckBox.Checked;
            Settings.Default.Save();
        }

        private void ShowBalloonTip()
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Visible = true;
            NotifyIcon.ShowBalloonTip(1000, "Audio Device Changed", "Device changed to: " + Program.GetCurrentPlaybackDevice(), ToolTipIcon.None);
        }
    }
}
