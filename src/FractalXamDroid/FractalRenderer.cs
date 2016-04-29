using System;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using FractalXam.Core;

namespace FractalXamDroid
{
    public class FractalRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
    {
        const string LineVertex = @"
#version 310 es

in vec2 in_position;

void main ()
{
    gl_Position = vec4 (in_position, 0.0, 1.0);
}
";

        const string LineFragment = @"
#version 310 es

precision highp float;

out vec4 out_color;

void main()
{
    out_color = vec4(1.0, 1.0, 1.0, 1.0);
}
";

        int m_lineProgramId = 0;
        int m_positionAttribId = 0;
        int m_lineBufferId = 0;
        int m_vaoId = 0;

        public FractalRenderer()
        {
            Lines = new Fractal.Line[0];
        }

        public void OnDrawFrame(IGL10 gl)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            DrawLines();
            CheckError();
        }

        public void OnSurfaceChanged(IGL10 gl, int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }

        public void OnSurfaceCreated(IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
        {
            GL.ClearColor(Color4.CornflowerBlue);

            LoadShaders();

            GL.GenVertexArrays(1, out m_vaoId);
            GL.BindVertexArray(m_vaoId);

            GL.GenBuffers(1, out m_lineBufferId);

            GL.UseProgram(m_lineProgramId);
            m_positionAttribId = GL.GetAttribLocation(m_lineProgramId, "in_position");
            GL.EnableVertexAttribArray(m_positionAttribId);
        }

        public Fractal.Line[] Lines { get; set; }

        void LoadShaders()
        {
            var vertexShaderId = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderId, LineVertex);
            GL.CompileShader(vertexShaderId);

            var vertexErr = 0;
            GL.GetShader(vertexShaderId, ShaderParameter.CompileStatus, out vertexErr);
            if (vertexErr == 0)
            {
                var log = GL.GetShaderInfoLog(vertexShaderId);
                throw new Exception(log);
            }

            var fragmentShaderId = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderId, LineFragment);
            GL.CompileShader(fragmentShaderId);

            var fragmentErr = 0;
            GL.GetShader(vertexShaderId, ShaderParameter.CompileStatus, out fragmentErr);
            if (fragmentErr == 0)
            {
                var log = GL.GetShaderInfoLog(fragmentShaderId);
                throw new Exception(log);
            }

            m_lineProgramId = GL.CreateProgram();

            GL.AttachShader(m_lineProgramId, vertexShaderId);
            GL.AttachShader(m_lineProgramId, fragmentShaderId);

            GL.LinkProgram(m_lineProgramId);

            int status;
            GL.GetProgram(m_lineProgramId, ProgramParameter.LinkStatus, out status);
            if (status == 0)
            {
                var log = GL.GetProgramInfoLog(m_lineProgramId);
                throw new Exception(String.Format("Error linking program: {0}", log));
            }
        }

        unsafe void DrawLines()
        {
            if (Lines.Length > 0)
            {
                var lineSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Fractal.Line));
                var vector2Size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(OpenTK.Vector2));

                GL.BindBuffer(BufferTarget.ArrayBuffer, m_lineBufferId);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr (Lines.Length * lineSize), Lines, BufferUsage.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, m_lineBufferId);
                GL.VertexAttribPointer(m_positionAttribId, 2, VertexAttribPointerType.Float, false, vector2Size, 0);

                GL.DrawArrays(BeginMode.Lines, 0, Lines.Length * lineSize);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
        }

        void CheckError()
        {
            var err = GL.GetErrorCode();
            if (err != ErrorCode.NoError)
            {
                throw new Exception(err.ToString());
            }
        }
    }
}