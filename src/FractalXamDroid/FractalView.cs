using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Opengl;
using Javax.Microedition.Khronos.Egl;
using Javax.Microedition.Khronos.Opengles;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using FractalXam.Core;

namespace FractalXamDroid
{
    public class FractalView : GLSurfaceView
    {
        readonly FractalRenderer m_renderer;

        public FractalView(Context context) : base(context)
        {
            m_renderer = new FractalRenderer();

            SetEGLContextClientVersion(3);
            SetEGLConfigChooser(8, 8, 8, 8, 16, 0); // We need this.
            SetRenderer(m_renderer);
        }

        public void SetFractalLength(float length)
        {
            m_renderer.Lines = Fractal.Create(length);
        }
    }
}