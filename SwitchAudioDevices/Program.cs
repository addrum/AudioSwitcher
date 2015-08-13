using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace SwitchAudioDevices
{
    public class Program
    {
        private static int _deviceCount;
        private static int _currentDeviceId;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        
        #region Tray events

        public static void PopulateDeviceList(ContextMenu menu)
        {

            // All all active devices
            foreach (var tuple in GetDevices())
            {
                var id = tuple.Item1;
                var deviceName = tuple.Item2;
                var isInUse = tuple.Item3;

                var item = new MenuItem { Checked = isInUse, Text = deviceName };
                item.Click += (s, a) => SelectDevice(id);

                menu.MenuItems.Add(item);
            }
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
                _deviceCount += 1;
            }

            return devices;
        }

        public static void SelectDevice(int id)
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

        //Gets the ID of the next sound device in the list
        public static int NextId()
        {
            if (_currentDeviceId == _deviceCount)
            {
                _currentDeviceId = 1;
            }
            else
            {
                _currentDeviceId += 1;
            }
            return _currentDeviceId;
        }

        #endregion
    }
}
