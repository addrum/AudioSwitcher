using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using AudioSwitcher.Properties;

namespace AudioSwitcher
{
    public class SwitchAudioDevices : Form
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SwitchAudioDevices());
        }

        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenu _trayMenu;
        private readonly int _deviceCount;
        private Button button1;
        private int _currentDeviceId;
        

        public SwitchAudioDevices()
        {
            InitializeComponent();
            // Set form properties
        
            ClientSize = new Size(292, 266);

            // Create a simple tray menu
            _trayMenu = new ContextMenu();

            // Create a tray icon
            _trayIcon = new NotifyIcon
            {
                Text = "Switch Audio Devices",
                Icon = new Icon(Resources.speaker, 40, 40),
                ContextMenu = _trayMenu,
                Visible = true
            };

            // Add menu to tray icon and show it

            // Count sound-devices
            foreach (var tuple in GetDevices())
            {
                _deviceCount += 1;
            }

            // Populate device list when menu is opened
            _trayIcon.ContextMenu.Popup += PopulateDeviceList;

            // Register MEH on trayicon leftclick
            _trayIcon.MouseUp += TrayIcon_LeftClick;
        }

        // Selects next device in list when trayicon is left-clicked
        private void TrayIcon_LeftClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SelectDevice(NextId());
            }
        }

        //Gets the ID of the next sound device in the list
        private int NextId()
        {
            if (_currentDeviceId == _deviceCount){
                _currentDeviceId = 1;
            } else {
                _currentDeviceId += 1;
            }
            return _currentDeviceId;
        }

        

        #region Tray events

        private void PopulateDeviceList(object sender, EventArgs e)
        {
            // Empty menu to prevent stuff to pile up
            _trayMenu.MenuItems.Clear();

            // All all active devices
            foreach (var tuple in GetDevices())
            {
                var id = tuple.Item1;
                var deviceName = tuple.Item2;
                var isInUse = tuple.Item3;

                var item = new MenuItem {Checked = isInUse, Text = deviceName};
                item.Click += (s, a) => SelectDevice(id);

                _trayMenu.MenuItems.Add(item);
            }

            // Add preferences
            var preferencesItem = new MenuItem {Text = "Preferences"};
            preferencesItem.Click += OpenPreferences;
            _trayMenu.MenuItems.Add("-");
            _trayMenu.MenuItems.Add(preferencesItem);

            // Add an exit button
            var exitItem = new MenuItem {Text = "Exit"};
            exitItem.Click += OnExit;
            _trayMenu.MenuItems.Add("-");
            _trayMenu.MenuItems.Add(exitItem);
        }

        #endregion

        #region EndPointController.exe interaction

        private static IEnumerable<Tuple<int, string, bool>> GetDevices()
        {
            var p = new Process
                        {
                            StartInfo =
                                {
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true,
                                    FileName = "EndPointController.exe",
                                    Arguments = "-f \"%d|%ws|%d|%d\""
                                }
                        };
            p.Start();
            p.WaitForExit();
            var stdout = p.StandardOutput.ReadToEnd().Trim();

            var devices = new List<Tuple<int, string, bool>>();

            foreach (var line in stdout.Split('\n'))
            {
                var elems = line.Trim().Split('|');
                var deviceInfo = new Tuple<int, string, bool>(int.Parse(elems[0]), elems[1], elems[3].Equals("1"));
                devices.Add(deviceInfo);
            }

            return devices;
        }

        private static void SelectDevice(int id)
        {
            var p = new Process
                        {
                            StartInfo =
                                {
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true,
                                    FileName = "EndPointController.exe",
                                    Arguments = id.ToString(CultureInfo.InvariantCulture)
                                }
                        };
            p.Start();
            p.WaitForExit();
        }

        #endregion

        #region Main app methods

        private void OpenPreferences(object Sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            
            Activate();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Dispose(true);
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                _trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        #endregion

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SwitchAudioDevices));
            button1 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // SwitchAudioDevices
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(button1);
            Name = "SwitchAudioDevices";
            ResumeLayout(false);

        }
    }
}