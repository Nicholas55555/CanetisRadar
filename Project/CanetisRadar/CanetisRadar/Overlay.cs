using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.CoreAudioApi;
using IniParser;
using IniParser.Model;

namespace CanetisRadar
{
    public partial class Overlay : Form
    {
        // -------------------------------------------------------
        // Variables
        // -------------------------------------------------------
        private MMDeviceEnumerator _enumerator;
        private MMDevice _device;

        private int _multiplier = 100;

        // -------------------------------------------------------
        // Dll Imports
        // -------------------------------------------------------
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public Overlay()
        {
            InitializeComponent();
        }

        private void Overlay_Load(object sender, EventArgs e)
        {
            TransparencyKey = Color.Turquoise;
            BackColor = Color.Turquoise;

            int initialStyle = GetWindowLong(Handle, -20);
            SetWindowLong(Handle, -20, initialStyle | 0x80000 | 0x20);

            WindowState = FormWindowState.Maximized;
            Opacity = 0.5;

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(AppDomain.CurrentDomain.BaseDirectory + "settings.ini");
            string m = data["basic"]["multiplier"];
            _multiplier = int.Parse(m);

            var t = new Thread(Loop);
            t.Start();
        }

        // -------------------------------------------------------
        // Main Loop
        // -------------------------------------------------------
        private void Loop()
        {
            _enumerator = new MMDeviceEnumerator();
            _device = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

            if (_device.AudioMeterInformation.PeakValues.Count < 8)
            {
                MessageBox.Show("You are not using 7.1 audio device! Please look again at setup guide.", "No 7.1 audio detected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            while (true)
            {
                float lefttop = _device.AudioMeterInformation.PeakValues[0];
                float righttop = _device.AudioMeterInformation.PeakValues[1];
                float leftbottom = _device.AudioMeterInformation.PeakValues[4];
                float rightbottom = _device.AudioMeterInformation.PeakValues[5];

                float tempone = lefttop * _multiplier;
                float temptwo = righttop * _multiplier;

                float tempthree = leftbottom * _multiplier;
                float tempfour = rightbottom * _multiplier;

                float x = 75 - tempone + temptwo;
                float y = 75 - tempone - temptwo;

                x = x - tempthree + tempfour;
                y = y + tempthree + tempfour;

                if (y < 10)
                {
                    y = 10;
                }
                if (x < 10)
                {
                    x = 10;
                }

                if (y > 140)
                {
                    y = 140;
                }
                if (x > 140)
                {
                    x = 140;
                }

                var infotext = "";
                for (var i = 0; i < _device.AudioMeterInformation.PeakValues.Count; i++)
                {
                    infotext += i + " -> " + _device.AudioMeterInformation.PeakValues[i] + "\n";
                }
                label2.Invoke((MethodInvoker)delegate {
                    label2.Text = infotext;
                });

                CreateRadar((int)x, (int)y);

                Thread.Sleep(10);
            }
        }

        private void CreateRadar(int x, int y)
        {
            var radar = new Bitmap(150, 150);
            Graphics grp = Graphics.FromImage(radar);
            grp.FillRectangle(Brushes.Black, 0, 0, radar.Width, radar.Height);

            grp.FillRectangle(Brushes.Red, x - 5, y - 5, 10, 10);

            pictureBox1.Invoke((MethodInvoker)delegate {
                pictureBox1.Image = radar;
            });
        }
    }
}
