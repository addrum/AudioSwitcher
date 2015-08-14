using System;
using System.Windows.Forms;
using SwitchAudioDevices.Properties;

namespace SwitchAudioDevices
{
    public partial class Form1 : Form 
    {
        private readonly ContextMenu _menu;
        KeysConverter converter = new KeysConverter();
        private bool DoubleClickToCycle { get; set; }
        private bool GlobalHotkeys { get; set; }

        // http://stackoverflow.com/a/27309185/1860436
        private KeyboardHook _hook = new KeyboardHook();

        public Form1()
        {
            InitializeComponent();
            _menu = new ContextMenu();
            NotifyIcon.ContextMenu = _menu;
            PopulateDeviceList(_menu);
            AddPreferencesAndExit();
            PresetValuesOfSettings();

            // reigster the event that is fired after the key press
            _hook.KeyPressed += hook_KeyPressed;
            // register the control + alt + F12 combination as hot key
            var hotkeys = Settings.Default.Hotkey.Split(',');
            ModifierKeys modifiers = 0;
            Keys keys = Keys.None;
                foreach (var hotkey in hotkeys)
                {
                    switch (hotkey)
                    {
                        case "CTRL":
                            modifiers |= global::ModifierKeys.Control;
                            break;
                        case "ALT":
                            modifiers |= global::ModifierKeys.Alt;
                            break;
                        case "SHIFT":
                            modifiers |= global::ModifierKeys.Shift;
                            break;
                        case "WIN":
                            modifiers |= global::ModifierKeys.Win;
                            break;
                        default:
                            keys += Convert.ToChar(hotkey);
                            break;
                    }
                }
                _hook.RegisterHotKey(modifiers, keys);
        }

        private static void PopulateDeviceList(ContextMenu menu)
        {

            // All all active devices
            foreach (var device in Program.GetDevices())
            {
                var id = device.Item1;
                var deviceName = device.Item2;
                var isInUse = device.Item3;

                var item = new MenuItem { Checked = isInUse, Text = deviceName };
                item.Click += (s, a) => Program.SelectDevice(id);

                menu.MenuItems.Add(item);
            }
        }

        private void DeviceClick(object sender, EventArgs e, int id)
        {
            Program.SelectDevice(id);
            ResetDeviceList();
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
            var hotkeys = Settings.Default.Hotkey.Replace(",", " + ");
            hotkeysTextBox.Text = hotkeys;
        }

        private void ResetDeviceList()
        {
            _menu.MenuItems.Clear();
            PopulateDeviceList(_menu);
            AddPreferencesAndExit();
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
            ResetDeviceList();
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
             ResetDeviceList();
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

        // ReSharper disable LocalizableElement
        private void hotkeysTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Control || e.KeyCode == Keys.ControlKey) && !hotkeysTextBox.Text.Contains("CTRL"))
            {
                AppendToHotkeysTextBox("CTRL");
                SaveHotkeysSetting("CTRL");
            }
            else if (e.Alt && !hotkeysTextBox.Text.Contains("ALT"))
            {
                AppendToHotkeysTextBox("ALT");
                SaveHotkeysSetting("ALT");
            }
            else if ((e.Shift || e.KeyCode == Keys.ShiftKey) && !hotkeysTextBox.Text.Contains("SHIFT"))
            {
                AppendToHotkeysTextBox("SHIFT");
                SaveHotkeysSetting("SHIFT");
            }
            else if (e.KeyCode == Keys.Space && !hotkeysTextBox.Text.Contains("SPACE"))
            {
                AppendToHotkeysTextBox("SPACE");
                SaveHotkeysSetting(e.KeyValue.ToString());
            }
            else if (!hotkeysTextBox.Text.Contains(e.KeyCode.ToString()))
            {
                AppendToHotkeysTextBox(e.KeyCode.ToString());
                SaveHotkeysSetting(e.KeyCode.ToString());
            }

            /*if (hotkeysTextBox.Text.Length > 1 && !hotkeysTextBox.Text.EndsWith(" "))
            {
                hotkeysTextBox.Text += " + ";
            }*/
        }

        private void AppendToHotkeysTextBox(string value)
        {
            if (hotkeysTextBox.Text.Length > 0)
            {
                hotkeysTextBox.Text += " + ";
            }
            hotkeysTextBox.Text += value;
        }

        private void SaveHotkeysSetting(string key)
        {
            if (Settings.Default.Hotkey.Contains(key)) return;
            if (string.IsNullOrEmpty(Settings.Default.Hotkey))
            {
                Settings.Default.Hotkey += key;
            }
            else
            {
                Settings.Default.Hotkey += "," + key;
            }
            Settings.Default.Save();
        }

        private void hotkeysTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            hotkeysTextBox.Text = "";
            Settings.Default.Hotkey = "";
        }
    }
}
