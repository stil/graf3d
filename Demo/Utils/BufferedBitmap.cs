using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using graf3d.Engine.Struktury;

namespace graf3d.Demo.Utils
{
    /// <summary>
    ///     Klasa implementująca buforowany dostęp do bitmapy.
    /// </summary>
    public class BufferedBitmap : IBufferedBitmap
    {
        /// <summary>
        ///     Bufor, w którym zapisywane są piksele, które będą wyświetlone w następnej klatce animacji.
        /// </summary>
        private readonly byte[] _backBuffer;

        /// <summary>
        ///     Właściwa bitmapa, która jest wyświetlana użytkownikowi.
        /// </summary>
        public WriteableBitmap BitmapSource { get; }

        /// <summary>
        ///     Stosunek szerokości do wysokości bitmapy.
        /// </summary>
        public float AspectRatio { get; }

        /// <summary>
        ///     Szerokość bitmapy w piskelach.
        /// </summary>
        public int PixelWidth { get; }

        /// <summary>
        ///     Wysokość bitmapy w piskelach.
        /// </summary>
        public int PixelHeight { get; }

        /// <summary>
        ///     Tworzy buforowaną bitmapę o danej szerokości i wyskości w pikselach.
        /// </summary>
        public BufferedBitmap(int pixelWidth, int pixelHeight)
        {
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            AspectRatio = PixelWidth/(float) PixelHeight;
            BitmapSource = new WriteableBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Bgra32, null);

            // Rozmiar bufora równy jest ilości rysowanych pikseli.
            // Każdy piksel ma rozmiar 4 bajtów, po jednym bajcie na każdy kanał (BGRA).
            _backBuffer = new byte[PixelWidth*PixelHeight*4];
        }

        /// <summary>
        ///     Rysuje piksel w buforowanej ramce, uprzednio sprawdzając czy mieści się w granicach.
        /// </summary>
        public void DrawPoint(int x, int y, Color32 color)
        {
            if (x < 0 || y < 0 || x >= PixelWidth || y >= PixelHeight) return;

            var offset = (x + y*PixelWidth)*4;
            _backBuffer[offset] = color.B;
            _backBuffer[offset + 1] = color.G;
            _backBuffer[offset + 2] = color.R;
            _backBuffer[offset + 3] = color.A;
        }

        /// <summary>
        ///     Wypełnia cały bufor jednolitym kolorem.
        /// </summary>
        public void Clear(Color32 color)
        {
            for (var offset = 0; offset < _backBuffer.Length; offset += 4)
            {
                _backBuffer[offset] = color.B;
                _backBuffer[offset + 1] = color.G;
                _backBuffer[offset + 2] = color.R;
                _backBuffer[offset + 3] = color.A;
            }
        }

        /// <summary>
        ///     Kiedy bufor jest już przygotowany, zostanie wypchnięty do bitmapy,
        ///     aby mógł klatka animacji mogła zostać wyświetlona na ekranie.
        /// </summary>
        public void Present()
        {
            var rect = new Int32Rect(0, 0, PixelWidth, PixelHeight);
            BitmapSource.WritePixels(rect, _backBuffer, BitmapSource.BackBufferStride, 0);
        }
    }
}