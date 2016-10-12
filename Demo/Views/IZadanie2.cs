using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using graf3d.Engine.Oświetlenie;
using graf3d.Engine.Struktury;

namespace graf3d.Demo.Views
{
    internal interface IZadanie2
    {
        Image Render2 { get; }
        float SpecularPower { get; set; }
        Button RefreshButton { get; }
        IlluminationObject IlluminationObject { set; }
        float BumpFactor { get; set; }
        event RoutedEventHandler Loaded;
        event Action<IlluminationObject> IlluminationObjectChanged;
        event KeyEventHandler KeyDown;
        void SetLightPosition(Vector3 position);
    }
}