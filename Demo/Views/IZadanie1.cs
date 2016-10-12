using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using graf3d.Engine.Struktury;

namespace graf3d.Demo.Views
{
    internal interface IZadanie1
    {
        Image Render { get; }
        bool IsCameraVisible { get; }
        void SetFps(int current, int low, int high);
        void SetCameraParams(Vector3 position, Quaternion rotation, float fov);
        event RoutedEventHandler Loaded;
        event MouseWheelEventHandler MouseWheel;
        event KeyEventHandler KeyDown;
    }
}