using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;

class Ventana : GameWindow
{
    private FiguraU figuraU;
    private int _shaderProgram;
    private float _rotationAngle = 0f;

    // Variables para la cámara
    private Matrix4 _view;
    private Matrix4 _projection;
    private Vector3 _cameraPosition = new Vector3(0f, 0f, 3f);
    private Vector3 _cameraFront = new Vector3(0f, 0f, -1f);
    private Vector3 _cameraUp = Vector3.UnitY;
    private float _cameraSpeed = 2.5f;

    public Ventana() : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        Size = new Vector2i(800, 600),
        Title = "Dibujando una U en 3D con coordenadas de centro de masa",
    })
    { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(Color4.Black);
        GL.Enable(EnableCap.DepthTest);

        string vertexShaderSource = @"#version 330 core
            layout(location = 0) in vec3 aPosition;
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            void main()
            {
                gl_Position = projection * view * model * vec4(aPosition, 1.0);
            }";

        string fragmentShaderSource = @"#version 330 core
            out vec4 FragColor;
            void main()
            {
                FragColor = vec4(1.0, 1.0, 1.0, 1.0);
            }";

        _shaderProgram = CreateShaderProgram(vertexShaderSource, fragmentShaderSource);

        // Configurar matrices de vista y proyección
        _view = Matrix4.LookAt(_cameraPosition, _cameraPosition + _cameraFront, _cameraUp);
        _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)Size.X / Size.Y, 0.1f, 100f);

        this.figuraU = new FiguraU(  // <-- Ahora se asigna al campo de la clase
            _shaderProgram,
            "vertices.json",
            "indices.json"
        );
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _rotationAngle += 1.0f * (float)args.Time; // Rotación en cada frame

        // Actualizar matriz de vista (por si la cámara se mueve)
        _view = Matrix4.LookAt(_cameraPosition, _cameraPosition + _cameraFront, _cameraUp);

        // Usar el shader program
        GL.UseProgram(_shaderProgram);

        // Pasar las matrices de vista y proyección al shader
        int viewLocation = GL.GetUniformLocation(_shaderProgram, "view");
        GL.UniformMatrix4(viewLocation, false, ref _view);

        int projectionLocation = GL.GetUniformLocation(_shaderProgram, "projection");
        GL.UniformMatrix4(projectionLocation, false, ref _projection);

        figuraU.DibujarU(0f, 0f, -1.5f);  // Centro (solo traslación)
        figuraU.DibujarU(-1.5f,3.0f, -1.5f); // Izquierda
        figuraU.DibujarU(1.5f, -3.0f, -1.5f);  // Derecha

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        // Control de la cámara con teclado
        var input = KeyboardState;

        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            _cameraPosition += _cameraFront * _cameraSpeed * (float)args.Time;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            _cameraPosition -= _cameraFront * _cameraSpeed * (float)args.Time;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
            _cameraPosition -= Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * _cameraSpeed * (float)args.Time;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            _cameraPosition += Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * _cameraSpeed * (float)args.Time;
    }

    private int CreateShaderProgram(string vertexShaderSource, string fragmentShaderSource)
    {
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);

        GL.DetachShader(shaderProgram, vertexShader);
        GL.DetachShader(shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return shaderProgram;
    }

    public static void Main()
    {
        using (var ventana = new Ventana())
        {
            ventana.Run();
        }
    }
}