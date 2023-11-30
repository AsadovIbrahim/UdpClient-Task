using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    public partial class MainWindow : Window
    {
        UdpClient client;
        IPEndPoint remoteEp;
        bool isPlaying = false;

        public MainWindow()
        {
            InitializeComponent();
            client=new UdpClient();
            remoteEp = new IPEndPoint(IPAddress.Parse("192.168.0.105"), 12345);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                var buffer = new byte[ushort.MaxValue - 29];
                await client.SendAsync(buffer, buffer.Length, remoteEp);
                var list=new List<byte>();
                var maxlen = buffer.Length;
                var len = 0;
                while (true)
                {
                    do
                    {
                        try
                        {
                            var result = await client.ReceiveAsync();
                            buffer = result.Buffer;
                            len = buffer.Length;
                            list.AddRange(buffer);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    } while (len == maxlen);
                    var image = LoadImage(list.ToArray());
                    if(image != null)
                    {
                        imageScreen.Source = image;
                    }
                    list.Clear();
                }
            }
        }
        private static BitmapImage? LoadImage(byte[] imageData)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(imageData);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }
    }
}