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

        Fractal.Config m_config;

        public FractalView(Context context) : base(context)
        {
            m_renderer = new FractalRenderer();

            SetEGLContextClientVersion(3);
            SetEGLConfigChooser(8, 8, 8, 8, 16, 0); // We need this.
            SetRenderer(m_renderer);

            m_config = new Fractal.Config(0f, 0f, 0f);
        }

        public void SetFractalLength(float length)
        {
            m_config = new Fractal.Config(m_config.Degrees, length, m_config.FirstLength);
            m_renderer.Lines = Fractal.Create(m_config);
        }

        public void SetFractalDegrees(float degrees)
        {
            m_config = new Fractal.Config(degrees, m_config.Length, m_config.FirstLength);
            m_renderer.Lines = Fractal.Create(m_config);
        }

        public void SetFractalFirstLength(float length)
        {
            m_config = new Fractal.Config(m_config.Degrees, m_config.Length, length);
            m_renderer.Lines = Fractal.Create(m_config);
        }
    }
}