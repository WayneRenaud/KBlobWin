using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace KBlobWin
{
    //KblobWin - ported May 2014 Wayne Renaud
    //Original C++ by Tiaan Wessels, 1997

    public class MainWindow
    {
        //Window state information
        private int Width, Height;
        private GameWindow Window;
        private GraphicsMode Mode;

        //Texture
        private Bitmap Buffer;
        private int TextureId;
        UInt32[,] NewPixels;

        private ScreenSaverContext Context;
        private DateTime LastCycle;
        private TimeSpan SecondsPerCycle;

        public void StartLoop()
        {
            //Perform basic OpenGL initialization            
            Mode = new GraphicsMode();
            Window = new GameWindow(800, 600, Mode, "KBlobWin", GameWindowFlags.Default);
            Window.VSync = VSyncMode.Off;

            Window.RenderFrame += Window_RenderFrame;
            Window.Resize += mainWindow_Resize;
            Window.KeyDown += mainWindow_KeyDown;

            InitGL();

            //initialize screensaver cycling time
            LastCycle = DateTime.Now;
            SecondsPerCycle = new TimeSpan(0, 0, int.Parse(ConfigurationManager.AppSettings["cycletime"]));

            //updates per second, frames per second
            int fps = int.Parse(ConfigurationManager.AppSettings["speed"]);
            Window.Run(fps, fps);
        }

        private void InitGL()
        {
            //Clear to black, enable 2d textures.
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.Texture2D);
        }
        
        private void mainWindow_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case OpenTK.Input.Key.F:
                    if (Window.WindowState == WindowState.Fullscreen)
                    {
                        Window.WindowState = WindowState.Normal;
                        Window.WindowBorder = WindowBorder.Resizable;
                    }
                    else
                    {
                        Window.WindowState = WindowState.Fullscreen;
                        Window.WindowBorder = WindowBorder.Hidden;
                    }
                    break;
                case OpenTK.Input.Key.Left:
                    Context.PrevScreenSaver();
                    InitBuffer();
                    break;
                case OpenTK.Input.Key.Right:
                    Context.NextScreenSaver();
                    InitBuffer();
                    break;
                case OpenTK.Input.Key.Up:
                    Context.Reset(Width, Height);
                    InitBuffer();
                    break;
                case OpenTK.Input.Key.Down:
                    Context.Reset(Width, Height);
                    InitBuffer();
                    break;
                case OpenTK.Input.Key.Escape:
                    Window.Close();
                    break;
                default:
                    break;
            }
        }

        private void mainWindow_Resize(object sender, EventArgs e)
        {
            //This causes a complete reinit and restart.
            //It's also triggered when the window starts up, so this is our entry point into the app.
            Width = Window.Width;
            Height = Window.Height;

            //Initialize the screen savers
            if (Context == null)
                Context = new ScreenSaverContext(Width, Height, int.Parse(ConfigurationManager.AppSettings["screensaver"]));
            else
                Context.Reset(Width, Height);

            //Rebuild the projection
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, 0, 1);
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Modelview);

            //Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit);

            InitBuffer();
        }

        private void InitBuffer()
        {
            //One bitmap to hold them all, 
            Buffer = new Bitmap(Width, Height);

            //One array to find them
            NewPixels = new UInt32[Height / 70 + 1, Height / 70 + 1];

            //One command to bring them all
            TextureId = GL.GenTexture();

            //And to the texture bind them            
            //In the GPU RAM where the pixels lie
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest); //Nearest neighbor stops this from being blurry  
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            //Set texture settings
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            //Lock bitmap into memory
            System.Drawing.Imaging.BitmapData bitmapPointer = Buffer.LockBits(new Rectangle(0, 0, Width, Height),
                                                                            System.Drawing.Imaging.ImageLockMode.ReadOnly, 
                                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //Load the pixels in
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte, bitmapPointer.Scan0);
            
            //Clean up
            Buffer.UnlockBits(bitmapPointer);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            //Boxes per frame option gives the user to drastically speed up the screen saver
            for (int boxesPerFrame = 0; boxesPerFrame < int.Parse(ConfigurationManager.AppSettings["boxesperframe"]); boxesPerFrame++)
            {
                //Find out if enough time has passed to cycle to the next screensaver
                if (DateTime.Now - LastCycle > SecondsPerCycle)
                {
                    LastCycle = DateTime.Now;
                    Context.NextScreenSaver();
                    InitBuffer();
                }

                //Get the next location of the box from the screen saver
                Rectangle r = Context.GetNextRect();

                //Create an array of pixel data to update the on-GPU texture with
                //Rather than read the texture back into memory each time, the Buffer bitmap is also
                //going to be updated each time. Double-account pixelkeeping.
                for (int i = 0; i < r.Width; i++)
                    for (int j = 0; j < r.Height; j++)
                    {
                        //Determine new colour of this pixel
                        Color p = Buffer.GetPixel(i + r.X, j + r.Y);
                        int red = (p.R + int.Parse(ConfigurationManager.AppSettings["brightness"])) % 255;
                        Color newCol = Color.FromArgb(255, red, 0, 0);

                        /*
                        Original calc is: p += (colorInc << 18)
                        Which gives:
                        AAAAAAAARRRRRRRRGGGGGGGGBBBBBBBB
                        00000000000000000000000000000011
                        00000000000011000000000000000000
                        Which is red 12.
                        */

                        //Reassign it back to the texture and buffer
                        Buffer.SetPixel(i + r.X, j + r.Y, newCol);

                        //Array has to be flipped before being applied... I guess because of endianness?
                        NewPixels[j, i] = (uint)Buffer.GetPixel(i + r.X, j + r.Y).ToArgb();
                    }

                //Clear and draw the screen texture
                GL.Clear(ClearBufferMask.ColorBufferBit);

                //Update the texture to be drawn
                GL.BindTexture(TextureTarget.Texture2D, TextureId);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, r.X, r.Y, r.Width, r.Height, PixelFormat.Bgra, PixelType.UnsignedByte, ref NewPixels[0, 0]);
            }

            //Draw the texture from GPU memory
            GL.Begin(PrimitiveType.Quads);
                GL.Color4(Color.White);
                GL.TexCoord2(0, 0);
                GL.Vertex2(0, 0);
                GL.TexCoord2(0, 1);
                GL.Vertex2(0, Height);
                GL.TexCoord2(1, 1);
                GL.Vertex2(Width, Height);
                GL.TexCoord2(1, 0);
                GL.Vertex2(Width, 0);
            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, 0);            
                        
            //Flip the buffer
            Window.SwapBuffers();
        }
    }
}
