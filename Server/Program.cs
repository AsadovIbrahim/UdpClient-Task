using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;


UdpClient server= new UdpClient(12345);
var remoteEp = new IPEndPoint(IPAddress.Any, 0);

while (true)
{
    var result = await server.ReceiveAsync();
    new Task(async () =>
    {
        remoteEp=result.RemoteEndPoint;
        while (true)
        {
            var screen = TakeScreenShot();
            var imgByte=ImageToByte(screen);
            var chunks=imgByte.Chunk(ushort.MaxValue-29);

            foreach(var chunk in chunks)
            {
                await server.SendAsync(chunk,chunk.Length,remoteEp);
            }
        }
    }).Start();
}

Image TakeScreenShot()
{
    var width=Screen.PrimaryScreen.Bounds.Width;
    var height=Screen.PrimaryScreen.Bounds.Height;

    Bitmap bitmap = new Bitmap(width, height);

    using(Graphics g = Graphics.FromImage(bitmap))
    {
        g.CopyFromScreen(0,0,0,0,bitmap.Size);
    }
    return bitmap;
}

byte[] ImageToByte(Image image)
{
    using(MemoryStream stream=new MemoryStream())
    {
        image.Save(stream,ImageFormat.Jpeg);
        return stream.ToArray();
    }
}