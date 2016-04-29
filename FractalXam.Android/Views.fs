module FractalXam.Android.Views

open System
open Android.Opengl
open Javax.Microedition.Khronos.Opengles;
open OpenTK.Graphics
open OpenTK.Graphics.ES30
open FractalXam.Core

[<Literal>]
let LineVertex =
    """#version 310 es

    in vec2 in_position;

    void main ()
    {
        gl_Position = vec4 (in_position, 0.0, 1.0);
    }"""

[<Literal>]
let LineFragment =
    """#version 310 es

    precision highp float;

    out vec4 out_color;

    void main()
    {
        out_color = vec4(1.0, 1.0, 1.0, 1.0);
    }"""

type FractalRenderer () =
    inherit Java.Lang.Object ()

    let mutable lineProgramId = 0
    let mutable positionAttribId = 0
    let mutable lineBufferId = 0
    let mutable vaoId = 0

    member val Lines = Array.zeroCreate<Fractal.Line> 0 with get, set

    member this.DrawLines () =
        if this.Lines.Length > 0 then
            GL.BindBuffer (BufferTarget.ArrayBuffer, lineBufferId)
            GL.BufferData (BufferTarget.ArrayBuffer, nativeint (this.Lines.Length * sizeof<Fractal.Line>), this.Lines, BufferUsage.StaticDraw);
            GL.BindBuffer (BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer (BufferTarget.ArrayBuffer, lineBufferId)
            GL.VertexAttribPointer (positionAttribId, 2, VertexAttribPointerType.Float, false, sizeof<OpenTK.Vector2>, 0);

            GL.DrawArrays (BeginMode.Lines, 0, this.Lines.Length * sizeof<Fractal.Line>);

            GL.BindBuffer (BufferTarget.ArrayBuffer, 0);

    member __.CheckError () =
        let err = GL.GetErrorCode ()
        if err <> ErrorCode.NoError then
            failwith <| err.ToString ()

    member this.LoadShaders () =
        let vertexShaderId = GL.CreateShader (ShaderType.VertexShader)
        GL.ShaderSource (vertexShaderId, LineVertex)
        GL.CompileShader (vertexShaderId)

        let mutable vertexErr = 0
        GL.GetShader (vertexShaderId, ShaderParameter.CompileStatus, &vertexErr)
        if vertexErr = 0 then
            let log = GL.GetShaderInfoLog (vertexShaderId)
            failwith log

        let fragmentShaderId = GL.CreateShader (ShaderType.FragmentShader)
        GL.ShaderSource (fragmentShaderId, LineFragment)
        GL.CompileShader (fragmentShaderId)

        let mutable fragmentErr = 0
        GL.GetShader(vertexShaderId, ShaderParameter.CompileStatus, &fragmentErr)
        if fragmentErr = 0 then
            let log = GL.GetShaderInfoLog (fragmentShaderId)
            failwith log

        lineProgramId <- GL.CreateProgram()

        GL.AttachShader (lineProgramId, vertexShaderId)
        GL.AttachShader (lineProgramId, fragmentShaderId)

        GL.LinkProgram (lineProgramId);

        let mutable status = 0
        GL.GetProgram (lineProgramId, ProgramParameter.LinkStatus, &status)
        if status = 0 then
            let log = GL.GetProgramInfoLog (lineProgramId)
            failwithf "Error linking program: %s" log

    interface GLSurfaceView.IRenderer with

        member this.OnDrawFrame _ =
            GL.Clear (ClearBufferMask.ColorBufferBit)

            this.DrawLines ()
            this.CheckError ()

        member this.OnSurfaceChanged (gl, width, height) =
            GL.Viewport (0, 0, width, height)

        member this.OnSurfaceCreated (gl, config) =
            GL.ClearColor (Color4.CornflowerBlue)

            this.LoadShaders ()

            GL.GenVertexArrays (1, &vaoId)
            GL.BindVertexArray (vaoId)

            GL.GenBuffers (1, &lineBufferId)

            GL.UseProgram (lineProgramId)
            positionAttribId <- GL.GetAttribLocation (lineProgramId, "in_position")
            GL.EnableVertexAttribArray (positionAttribId)

type FractalView (context) as this =
    inherit GLSurfaceView (context)

    let renderer = new FractalRenderer ()

    let mutable config : Fractal.Config = { Degrees = 0.f; Length = 0.f; FirstLength = 0.f }

    do
        this.SetEGLContextClientVersion (3)
        this.SetEGLConfigChooser (8, 8, 8, 8, 16, 0)
        this.SetRenderer (renderer)

    member this.SetFractalLength length =
        config <- { config with Length = length }
        renderer.Lines <- Fractal.create config

    member this.SetFractalDegrees degrees =
        config <- { config with Degrees = degrees }
        renderer.Lines <- Fractal.create config

    member this.SetFractalFirstLength length =
        config <- { config with FirstLength = length }
        renderer.Lines <- Fractal.create config