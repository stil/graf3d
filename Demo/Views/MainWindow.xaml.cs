using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using graf3d.Demo.Presenters;
using graf3d.Engine.Oświetlenie;
using graf3d.Engine.Struktury;

namespace graf3d.Demo.Views
{
    public partial class MainWindow : IZadanie1, IZadanie2
    {
        public MainWindow()
        {
            InitializeComponent();
            new Zadanie1(this);
            new Zadanie2(this);
        }

        public Image Render => render;

        public bool IsCameraVisible => render.IsVisible;

        public void SetFps(int current, int low, int high)
        {
            pFPS.Text = current.ToString();
            pFPSlow.Text = low.ToString();
            pFPShigh.Text = high.ToString();
        }

        public void SetCameraParams(Vector3 position, Quaternion rotation, float fov)
        {
            SetCoordinates(position, pCamPosX, pCamPosY, pCamPosZ);
            SetCoordinates(rotation, pCamRotX, pCamRotY, pCamRotZ);
            pZoom.Text = Math.Round(fov).ToString("0.0", CultureInfo.InvariantCulture);
        }

        event KeyEventHandler IZadanie1.KeyDown
        {
            add
            {
                KeyDown += (sender, args) =>
                {
                    if (cameraTab.IsSelected)
                    {
                        value.Invoke(sender, args);
                    }
                };
            }
            remove { }
        }

        event KeyEventHandler IZadanie2.KeyDown
        {
            add
            {
                KeyDown += (sender, args) =>
                {
                    if (specularTab.IsSelected)
                    {
                        value.Invoke(sender, args);
                    }
                };
            }
            remove { }
        }

        public event Action<IlluminationObject> IlluminationObjectChanged;

        public void SetLightPosition(Vector3 position)
        {
            SetCoordinates(position, pLightPosX, pLightPosY, pLightPosZ);
        }

        public Image Render2 => render2;
        public Button RefreshButton => refreshButton;

        public IlluminationObject IlluminationObject
        {
            set
            {
                switch (value)
                {
                    case IlluminationObject.Sphere:
                        radioSphere.IsChecked = true;
                        radioRectangle.IsChecked = false;
                        break;
                    case IlluminationObject.Rectangle:
                        radioSphere.IsChecked = false;
                        radioRectangle.IsChecked = true;
                        break;
                }
            }
        }

        public float BumpFactor
        {
            get
            {
                float factor;
                return float.TryParse(bumpFactor.Text, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out factor)
                    ? factor/100f
                    : 0;
            }
            set { bumpFactor.Text = (value*100).ToString("0", CultureInfo.InvariantCulture); }
        }

        public float SpecularPower
        {
            get
            {
                float power;
                return float.TryParse(specularPower.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                    out power)
                    ? power
                    : 0;
            }
            set { specularPower.Text = value.ToString("0.0", CultureInfo.InvariantCulture); }
        }

        private static void SetCoordinates(Vector3 vector, TextBlock x, TextBlock y, TextBlock z)
        {
            x.Text = vector.X.ToString("0.000", CultureInfo.InvariantCulture);
            y.Text = vector.Y.ToString("0.000", CultureInfo.InvariantCulture);
            z.Text = vector.Z.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private static void SetCoordinates(Quaternion vector, TextBlock x, TextBlock y, TextBlock z)
        {
            x.Text = RadianToDegree(vector.Yaw).ToString("0.000", CultureInfo.InvariantCulture);
            y.Text = RadianToDegree(vector.Pitch).ToString("0.000", CultureInfo.InvariantCulture);
            z.Text = RadianToDegree(vector.Roll).ToString("0.000", CultureInfo.InvariantCulture);
        }

        private static double RadianToDegree(double angle)
        {
            return Math.Round(angle*(180.0f/Math.PI));
        }

        private void RadioSphere_OnChecked(object sender, RoutedEventArgs e)
        {
            IlluminationObjectChanged?.Invoke(IlluminationObject.Sphere);
        }

        private void RadioRectangle_OnChecked(object sender, RoutedEventArgs e)
        {
            IlluminationObjectChanged?.Invoke(IlluminationObject.Rectangle);
        }
    }
}