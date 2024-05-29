using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL.Graphics;
using SharpGL.Natives;


namespace SharpGL.Desktop
{
    public class RenderWindow : IDisposable
    {
        public static RenderWindow Current { get; private set; }

        public Form Window { get; private set; }
        private Panel ctl;
        private GLCreateResult gl;
        private RenderLoop loop;


        private string title = "RenderWindow";
        public string Title { get { return title; } set { title = value; } }
        public int Width { get { return Window.Width; } set { Window.Width = value; } }
        public int Height { get { return Window.Height; } set { Window.Height = value; } }
        public int ControlWidth => ctl.Width;
        public int ControlHeight => ctl.Height;

        public InputDevice InputDevice { get; internal set; }


        public bool EnableFps { get; set; } = true;
        public bool FullScreen { 
            get
            {
                if (Window.FormBorderStyle == FormBorderStyle.None && Window.WindowState == FormWindowState.Minimized)
                    return true;
                return false;
            }
            set
            {
                if(value)
                {
                    Window.FormBorderStyle = FormBorderStyle.None;
                    Window.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    Window.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                    Window.WindowState = FormWindowState.Normal;
                }
            }
        }


        public RenderWindow(string title="RenderWindow",int w = 800,int h = 600)
        {
            Current = this;
            Window = new Form();
            Window.StartPosition = FormStartPosition.CenterScreen;
            Window.MinimumSize = new System.Drawing.Size(400, 300);
            this.title = title;
            Width = w;
            Height = h;
        }


        public void Run()
        {
            ctl = new Panel();
            ctl.Dock = DockStyle.Fill;
            Window.Controls.Add(ctl);

            gl = GL.Create(ctl.Handle);

            Window.Resize += (s, e) => OnResize(ControlWidth, ControlHeight);

            InputDevice = new InputDevice(ctl);
            InputDevice.SetKeyEvents(Window);

            OnResize(ControlWidth, ControlHeight);
            if (gl.IsCreated)
            {
                OnLoad();
                loop = new RenderLoop(InternalUpdate);
                loop.Run();
                Application.Run(Window);
            }
        }

        private void InternalUpdate(RenderLoop loop, float fps, float deltaTime)
        {
            InputDevice.Update();

            System.Threading.Tasks.Parallel.Invoke(ParalellOnUpdate);
            OnUpdate(loop);
            System.Threading.Tasks.Parallel.Invoke(ParalellOnRender);
            OnRender();
            SwapBuffers();


            InputDevice.Clear();

            if (EnableFps)
                Window.Text = $"{title} Fps:{fps}";
            else
                Window.Text = $"{title}";

            
        }



        public virtual void OnLoad()
        {

        }
        public virtual void OnUpdate(RenderLoop loop)
        {

        }
        public virtual void OnRender()
        {

        }
        public virtual void ParalellOnRender()
        {

        }
        public virtual void ParalellOnUpdate()
        {

        }

        public virtual void OnResize(int w,int h)
        {

        }


        public void SwapBuffers() => GL.SwapBuffers(gl);

        public void Dispose()
        {
            gl.Dispose();    
        }
    }
}
