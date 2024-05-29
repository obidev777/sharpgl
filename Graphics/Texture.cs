using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGL.Graphics
{
    public class Texture
    {
        public IntPtr Handle { get; private set; }
        public uint ID { get; internal set; }
        public int W { get; internal set; }
        public int H { get; internal set; }

        internal Texture(uint textureID, int w, int h)
        {
            ID = textureID;
            W = w;
            H = h;
        }
        public static Texture CreateFrom(IntPtr ptrPixels,int w,int h, PixelType pixelType=PixelType.UnsignedByte)
        {
            uint id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, id);


            GL.TexImage2D(TextureTarget.Texture2D, 0,
                (uint)PixelInternalFormat.Rgba, w,h
                , 0, (uint)PixelFormat.Bgra, (uint)pixelType, ptrPixels);

            GL.TexParameter(TextureTarget.Texture2D,
                 (uint)TextureParameterName.TextureWrapS, (int)
                 (uint)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D,
              (uint)TextureParameterName.TextureWrapT, (int)
             (uint)TextureWrapMode.Clamp);

            GL.TexParameter(TextureTarget.Texture2D,
           (uint)TextureParameterName.TextureMinFilter, (int)
           (uint)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
            (uint)TextureParameterName.TextureMagFilter, (int)
           (uint)TextureMagFilter.Linear);

            Texture tex = new Texture(id, w, h);
            tex.Handle = ptrPixels;
            return tex;
        }
        public static Texture CreateFrom(Bitmap bmp, PixelType pixelType = PixelType.UnsignedByte)
        {
            uint id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, id);

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Texture tex = CreateFrom(data.Scan0,bmp.Width,bmp.Height, pixelType);

            bmp.UnlockBits(data);

            return tex;
        }
        public static Texture Create(int w, int h) => CreateFrom(new Bitmap(w, h));
        public static Texture CreateFromFile(string file) => CreateFrom(new Bitmap(file));
    }
}
