using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using SwitchAudioDevices;
using SwitchAudioDevices.Properties;

namespace SwitchAudioDevices
{
    public partial class Form1 : Form 
    {
        private readonly ContextMenu _menu;
        private bool DoubleClickToCycle { get; set; }
        private bool GlobalHotkeys { get; set; }
        private bool ChangingHotkeys { get; set; }
        private bool MinimiseToTray { get; set; }

        // http://stackoverflow.com/a/27309185/1860436
        private static readonly KeyboardHook Hook = new KeyboardHook();

        #region disable keys

        // Structure contain information about low-level keyboard input event
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }
        
        //System level functions to be used for hook and unhook keyboard input
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);
        
        
        //Declaring Global objects
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess; 

        #endregion

        public Form1()
        {
            ProcessModule objCurModule = Process.GetCurrentProcess().MainModule;
            objKeyboardProcess = CaptureKey;
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurModule.ModuleName), 0);

            InitializeComponent();
            _menu = new ContextMenu();
            NotifyIcon.ContextMenu = _menu;

            PopulateDeviceList(_menu);
            AddPreferencesAndExit();
            PresetValuesOfSettings();

            // reigster the event that is fired after the key press
            Hook.KeyPressed += hook_KeyPressed;
            Program.RegisterHotkeys(Hook);
        }

        private IntPtr CaptureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                if (ChangingHotkeys)
                {
                    if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin) // Disabling Windows keys
                    {
                        AppendToHotkeysTextBox(objKeyInfo.key.ToString());
                        return (IntPtr) 1;
                    }
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
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
            DoubleClickToCycle = Settings.Default.DoubleClickToCycle;

            globalHotkeysCheckBox.Checked = Settings.Default.GlobalHotkeys;
            GlobalHotkeys = Settings.Default.GlobalHotkeys;

            var modifierKeys = Settings.Default.ModifierKeys.Replace(",", " + ");
            var keys = Settings.Default.Keys.ToString();
            var hotkeys =  keys.Replace(",", " + ");
            var finalKeys = modifierKeys + " + " + hotkeys;
            hotkeysTextBox.Text = finalKeys;

            minimiseCheckBox.Checked = Settings.Default.MinimiseToTray;
            MinimiseToTray = Settings.Default.MinimiseToTray;
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

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
             if (!GlobalHotkeys || ChangingHotkeys) return;
             Program.SelectDevice(Program.NextId());
             ResetDeviceList();
             ShowBalloonTip();
        }

        private void ShowBalloonTip()
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Visible = true;
            NotifyIcon.ShowBalloonTip(1000, "Audio Device Changed", "Device changed to: " + Program.GetCurrentPlaybackDevice(), ToolTipIcon.None);
        }

        private void AppendToHotkeysTextBox(string value)
        {
            var keys = hotkeysTextBox.Text.Replace(" + ", ",");
            var hotkeys = keys.Split(',');
            if (hotkeys.Contains(value)) return;
            if (hotkeysTextBox.Text.Length > 0)
            {
                hotkeysTextBox.Text += " + ";
            }
            hotkeysTextBox.Text += value;
        }

        private static void SaveModifierKeysSetting(string key)
        {
            if (Settings.Default.ModifierKeys.Contains(key)) return;
            if (string.IsNullOrEmpty(Settings.Default.ModifierKeys))
            {
                Settings.Default.ModifierKeys += key;
            }
            else
            {
                Settings.Default.ModifierKeys += "," + key;
            }
        }

        private static void SaveHotkeySettings(Keys key)
        {
            if (Settings.Default.Keys == key) return;
            Settings.Default.Keys = key;
        }

        #region form elements

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

        // ReSharper disable LocalizableElement
        private void hotkeysTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Control || e.KeyCode == Keys.ControlKey || e.KeyCode.ToString().Equals("ControlKey")) && !hotkeysTextBox.Text.Contains("CTRL"))
            {
                AppendToHotkeysTextBox("CTRL");
                SaveModifierKeysSetting("CTRL");
            }
            else if (e.Alt && !hotkeysTextBox.Text.Contains("ALT"))
            {
                AppendToHotkeysTextBox("ALT");
                SaveModifierKeysSetting("ALT");
            }
            else if ((e.Shift || e.KeyCode == Keys.ShiftKey) && !hotkeysTextBox.Text.Contains("SHIFT"))
            {
                AppendToHotkeysTextBox("SHIFT");
                SaveModifierKeysSetting("SHIFT");
            }
            else if (e.KeyCode == Keys.LWin && !hotkeysTextBox.Text.Contains("WIN"))
            {
                e.Handled = true;
                AppendToHotkeysTextBox("WIN");
                SaveModifierKeysSetting("WIN");
            }
            else if (e.KeyCode == Keys.Space && !hotkeysTextBox.Text.Contains("SPACE"))
            {
                AppendToHotkeysTextBox("SPACE");
                SaveHotkeySettings(e.KeyCode);
            }
            else if (e.KeyCode == Keys.Escape)
            {

            }
            else 
            {
                AppendToHotkeysTextBox(e.KeyCode.ToString());
                SaveHotkeySettings(e.KeyCode);
            }
            e.Handled = true;

            /*if (hotkeysTextBox.Text.Length > 1 && !hotkeysTextBox.Text.EndsWith(" "))
            {
                hotkeysTextBox.Text += " + ";
            }*/
        }

        private void hotkeysTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            ChangingHotkeys = true;
            hotkeysTextBox.Text = "";
            Settings.Default.ModifierKeys = "";
            saveButton.Enabled = true;
        }

        private void hotkeysTextBox_Leave(object sender, EventArgs e)
        {
            ChangingHotkeys = false;
        }

        private void minimiseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MinimiseToTray = minimiseCheckBox.Checked;
            Settings.Default.MinimiseToTray = minimiseCheckBox.Checked;
            Settings.Default.Save();
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

        private void saveButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            Program.RegisterHotkeys(Hook);
            saveButton.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!MinimiseToTray) return;
            e.Cancel = true;
            Hide();
        }

        #endregion

    }
}
