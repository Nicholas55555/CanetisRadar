using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using IniParser;
using IniParser.Model;

namespace CanetisRadar
{
    public partial class Overlay : Form
    {
        private MMDeviceEnumerator _enumerator;
        private MMDevice _device;

        private int _multiplier = 500;
        private int _updateRate = 50;

        private readonly Bitmap _radar = new Bitmap(150, 150);

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

            _multiplier = int.Parse(data["basic"]["multiplier"]);
            _updateRate = int.Parse(data["basic"]["updateRate"]);

            var t = new Thread(Loop);
            t.Start();
        }

        private void Loop()
        {
            _enumerator = new MMDeviceEnumerator();
            _device = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

            if (_device.AudioMeterInformation.PeakValues.Count < 8)
            {
                MessageBox.Show(@"You are not using 7.1 audio device! Please look again at setup guide.",
                    @"No 7.1 audio detected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            while (true)
            {
                float leftTop = _device.AudioMeterInformation.PeakValues[0];
                float rightTop = _device.AudioMeterInformation.PeakValues[1];
                float leftBottom = _device.AudioMeterInformation.PeakValues[4];
                float rightBottom = _device.AudioMeterInformation.PeakValues[5];

                float tempOne = leftTop * _multiplier;
                float tempTwo = rightTop * _multiplier;

                float tempThree = leftBottom * _multiplier;
                float tempFour = rightBottom * _multiplier;

                float x = 75 - tempOne + tempTwo;
                float y = 75 - tempOne - tempTwo;

                x = x - tempThree + tempFour;
                y = y + tempThree + tempFour;

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

                CreateRadar((int) x, (int) y);

                Thread.Sleep(_updateRate);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private void CreateRadar(int x, int y)
        {
            Graphics grp = Graphics.FromImage(_radar);
            grp.FillRectangle(Brushes.Black, 0, 0, _radar.Width, _radar.Height);
            grp.FillRectangle(Brushes.Red, x - 5, y - 5, 10, 10);

            RadarBox.Invoke((MethodInvoker) delegate { RadarBox.Image = _radar; });
        }
    }
}