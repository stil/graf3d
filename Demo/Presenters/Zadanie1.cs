using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using graf3d.Demo.Utils;
using graf3d.Demo.Views;
using graf3d.Engine.Algorytmy;
using graf3d.Engine.Komponenty;
using graf3d.Engine.Struktury;
using Colors = graf3d.Engine.Struktury.Colors;

namespace graf3d.Demo.Presenters
{
    internal class Zadanie1
    {
        /// <summary>
        ///     Buforowana bitmapa, przechowująca obraz widziany na ekranie.
        /// </summary>
        private readonly BufferedBitmap _bmp;

        /// <summary>
        ///     Klasa renderująca scenę.
        /// </summary>
        private readonly Device _device;

        /// <summary>
        ///     Licznik klatek na sekundę.
        /// </summary>
        private readonly FpsCounter _fpsCounter = new FpsCounter();

        /// <summary>
        ///     Scena zawierająca obiekty i kamerę.
        /// </summary>
        private readonly Scene _scene;

        /// <summary>
        ///     Klasa widoku, pozwalająca na komunikację z GUI.
        /// </summary>
        private readonly IZadanie1 _view;

        /// <summary>
        ///     Przygotowuje bitmapę oraz scenę.
        /// </summary>
        /// <param name="view"></param>
        public Zadanie1(IZadanie1 view)
        {
            _view = view;
            _view.Loaded += OnLoaded;

            _bmp = new BufferedBitmap(750, 450);

            _device = new Device(_bmp,
                new Bresenham(),
                new LiangBarskyClipping());

            _scene = new Scene
            {
                Camera = new Camera
                {
                    Position = Vector3.Zero,
                    LookDirection = Vector3.UnitZ,
                    FieldOfView = 60
                }
            };

            _scene = SceneImporter.LoadJsonFile(Path.Combine("Resources", "scene.unity.babylon"));
            // Przypisujemy płaszczyźnie kolor szary, aby mieć punkt odniesienia do "podłogi".
            _scene.Meshes.First(m => m.Name == "Plane").Color = Colors.DarkGrey;

            _fpsCounter.StatsChanged += UpdateDebugInfo;
        }

        private Camera Camera => _scene.Camera;

        /// <summary>
        ///     Wywoływane kiedy widok jest gotowy.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateDebugInfo();
            _view.Render.Source = _bmp.BitmapSource;
            _view.MouseWheel += OnMouseWheel;
            _view.KeyDown += OnKeyDown;
            CompositionTarget.Rendering += CompositionTargetOnRendering;
        }

        /// <summary>
        ///     Obsługuje naciśnięcie klawisza.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            const float step = 0.2f;
            const float rot = 1f;

            switch (keyEventArgs.Key)
            {
                case Key.W:
                    Camera.Move(new Vector3(0, 0, step));
                    break;
                case Key.S:
                    Camera.Move(new Vector3(0, 0, -step));
                    break;
                case Key.A:
                    Camera.Move(new Vector3(-step, 0, 0));
                    break;
                case Key.D:
                    Camera.Move(new Vector3(step, 0, 0));
                    break;
                case Key.E:
                    Camera.Move(new Vector3(0, step, 0));
                    break;
                case Key.C:
                    Camera.Move(new Vector3(0, -step, 0));
                    break;
                case Key.K:
                    Camera.Rotate(Axis.Y, -rot); // Sterowanie osią Y jest wygodniejsze w RelativeSpace.World
                    break;
                case Key.OemSemicolon:
                    Camera.Rotate(Axis.Y, rot); // Sterowanie osią Y jest wygodniejsze w RelativeSpace.World
                    break;
                case Key.I:
                    Camera.Rotate(Axis.Z, rot);
                    break;
                case Key.P:
                    Camera.Rotate(Axis.Z, -rot);
                    break;
                case Key.O:
                    Camera.Rotate(Axis.X, rot);
                    break;
                case Key.L:
                    Camera.Rotate(Axis.X, -rot);
                    break;
            }
            UpdateDebugInfo();
        }

        /// <summary>
        ///     Obsługuje poruszenie rolką myszki.
        /// </summary>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var steps = e.Delta/120f;
            var angleDelta = steps*5;
            Camera.FieldOfView -= angleDelta;
        }

        /// <summary>
        ///     Obsługuje zdarzenie na chwilę przed wyrenderowaniem okna.
        ///     W tym miejscu będziemy renderować klatki z kamery 3D.
        /// </summary>
        private void CompositionTargetOnRendering(object sender, object e)
        {
            if (!_view.IsCameraVisible) return; // Nie renderuj gdy okno jest schowane.

            _bmp.Clear(Colors.Black.ToColor32()); // Ustawia czarne tło.
            _device.Render(_scene); // Renderuje klatkę sceny 3D.
            _bmp.Present(); // Wypycha wyrenderowaną bitmapę do GUI.
            _fpsCounter.Tick(); // Aktualizuje licznik FPS.
        }

        /// <summary>
        ///     Aktualizuje pozycję kamery i jej rotację w GUI.
        /// </summary>
        private void UpdateDebugInfo()
        {
            _view.SetCameraParams(Camera.Position, Camera.Rotation, Camera.FieldOfView);
            _view.SetFps(_fpsCounter.Average, _fpsCounter.Low, _fpsCounter.High);
        }
    }
}