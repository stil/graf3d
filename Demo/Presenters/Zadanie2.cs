using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using graf3d.Demo.Utils;
using graf3d.Demo.Views;
using graf3d.Engine.Abstractions;
using graf3d.Engine.Komponenty;
using graf3d.Engine.Oświetlenie;
using graf3d.Engine.Struktury;
using Color = System.Drawing.Color;

namespace graf3d.Demo.Presenters
{
    internal class WrappedColor : IColor
    {
        private readonly Color _color;

        public WrappedColor(Color color)
        {
            _color = color;
        }

        public byte R => _color.R;
        public byte G => _color.G;
        public byte B => _color.B;
    }

    internal class WrappedBitmap : IReadOnlyBitmap
    {
        private readonly Bitmap _bmp;

        public WrappedBitmap(Bitmap bmp)
        {
            _bmp = bmp;
        }

        public int Width => _bmp.Width;
        public int Height => _bmp.Height;

        public IColor GetPixel(int x, int y)
        {
            return new WrappedColor(_bmp.GetPixel(x, y));
        }
    }

    internal class Zadanie2
    {
        private readonly Dictionary<IlluminationObject, Scene> _scenes = SetupScenes();
        private readonly IZadanie2 _view;
        private IlluminationObject _currentObject = IlluminationObject.Sphere;
        private PixelShader _shader;

        public Zadanie2(IZadanie2 view)
        {
            _view = view;
            _view.Loaded += OnLoaded;
            SetupScenes();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateDebugInfo();

            var bmp = new BufferedBitmap(300, 300);
            _view.Render2.Source = bmp.BitmapSource;
            _shader = new PixelShader(bmp);

            _view.RefreshButton.Click += (o, args) => RenderScene();
            _view.IlluminationObjectChanged += o =>
            {
                _currentObject = o;
                UpdateDebugInfo();
            };


            _view.KeyDown += (o, args) =>
            {
                var move = Vector3.Zero;
                var step = 0.25f;
                switch (args.Key)
                {
                    case Key.W:
                        move = new Vector3(0, step, 0);
                        break;
                    case Key.S:
                        move = new Vector3(0, -step, 0);
                        break;
                    case Key.A:
                        move = new Vector3(-step, 0, 0);
                        break;
                    case Key.D:
                        move = new Vector3(step, 0, 0);
                        break;
                    case Key.E:
                        move = new Vector3(0, 0, step);
                        break;
                    case Key.C:
                        move = new Vector3(0, 0, -step);
                        break;
                    case Key.I:
                        _scenes[_currentObject].Lights[0].AttenuationCutoff += 1;
                        RenderScene();
                        break;
                    case Key.K:
                        _scenes[_currentObject].Lights[0].AttenuationCutoff -= 1;
                        RenderScene();
                        break;
                    case Key.O:
                        _scenes[_currentObject].Lights[0].Radius += 1;
                        RenderScene();
                        break;
                    case Key.L:
                        _scenes[_currentObject].Lights[0].Radius -= 1;
                        RenderScene();
                        break;
                }

                if (move.Length() > 0)
                {
                    _scenes[_currentObject].Lights[0].Position += move;
                    RenderScene();
                    UpdateDebugInfo();
                }
            };

            RenderScene();
        }

        private void RenderScene()
        {
            var scene = _scenes[_currentObject];
            scene.Material.Shininess = _view.SpecularPower;
            scene.Material.BumpFactor = _view.BumpFactor;
            _shader.Illuminate(scene);
        }

        private void UpdateDebugInfo()
        {
            _view.SpecularPower = _scenes[_currentObject].Material.Shininess;
            _view.IlluminationObject = _currentObject;
            _view.BumpFactor = _scenes[_currentObject].Material.BumpFactor;
            _view.SetLightPosition(_scenes[_currentObject].Lights[0].Position);
        }

        private static Dictionary<IlluminationObject, Scene> SetupScenes()
        {
            var scenes = new Dictionary<IlluminationObject, Scene>();

            // SPHERE
            var scene1 = new Scene();
            scenes.Add(IlluminationObject.Sphere, scene1);
            scene1.SurfaceShader = new Sphere();
            scene1.Lights.Add(new Light
            {
                Color = new Engine.Struktury.Color(1f, 1f, 1f, 1f),
                Position = new Vector3(-1.25f, 5.25f, 13)
            });
            scene1.Camera = new Camera
            {
                Position = new Vector3(0, 1.6f, -2f),
                LookDirection = Vector3.UnitZ
            };

            var bitmap = new WrappedBitmap(new Bitmap(Path.Combine("Resources", "rock.png")));
            var map = new NormalMap(bitmap);

            scene1.Material = new Material
            {
                Ambient = new Engine.Struktury.Color(0.1f, 0, 0, 1),
                Diffuse = 1f * new Engine.Struktury.Color(0.68f, 0.85f, 0.9f, 1f),
                Emissive = new Engine.Struktury.Color(0, 0, 0, 0),
                Specular = 3f * new Engine.Struktury.Color(0.15f, 0.15f, 0.15f, 1),
                Shininess = 200.0f,
                BumpFactor = 0.1f,
                NormalMap = map
            };

            // RECTANGLE
            var scene2 = new Scene();
            scenes.Add(IlluminationObject.Rectangle, scene2);
            scene2.SurfaceShader = new Plane();
            scene2.Lights.Add(new Light
            {
                Color = new Engine.Struktury.Color(1, 1, 1, 1),
                Position = new Vector3(-1, 0, 0),
                AttenuationOn = true,
                Radius = 3,
                AttenuationCutoff = 0
            });
            scene2.Camera = new Camera
            {
                Position = new Vector3(0, 0, -1),
                LookDirection = Vector3.UnitZ
            };
            scene2.Material = new Material
            {
                Ambient = new Engine.Struktury.Color(0.2f, 0, 0, 1),
                Diffuse = 1f * new Engine.Struktury.Color(0.68f, 0.85f, 0.9f, 1f),
                Emissive = new Engine.Struktury.Color(0, 0, 0, 1),
                Specular = 3f * new Engine.Struktury.Color(0.15f, 0.15f, 0.15f, 1),
                Shininess = 200.0f,
                BumpFactor = 0.4f,
                NormalMap = map
            };

            return scenes;
        }
    }
}