using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

class FiguraU
{
    private int _vao, _vbo, _ebo;
    private int _shaderProgram;
    private int _modelLocation;

    private readonly float[] _vertices =
    {
        // Cara frontal
        -0.6f,  0.5f,  0.1f,  -0.5f,  0.5f,  0.1f,  -0.5f, -0.5f,  0.1f,
        -0.6f, -0.5f,  0.1f,   0.5f,  0.5f,  0.1f,   0.6f,  0.5f,  0.1f,
         0.6f, -0.5f,  0.1f,   0.5f, -0.5f,  0.1f,  -0.5f, -0.5f,  0.1f,
         0.5f, -0.5f,  0.1f,   0.5f, -0.6f,  0.1f,  -0.5f, -0.6f,  0.1f,
        // Cara trasera
        -0.6f,  0.5f, -0.1f,  -0.5f,  0.5f, -0.1f,  -0.5f, -0.5f, -0.1f,
        -0.6f, -0.5f, -0.1f,   0.5f,  0.5f, -0.1f,   0.6f,  0.5f, -0.1f,
         0.6f, -0.5f, -0.1f,   0.5f, -0.5f, -0.1f,  -0.5f, -0.5f, -0.1f,
         0.5f, -0.5f, -0.1f,   0.5f, -0.6f, -0.1f,  -0.5f, -0.6f, -0.1f,
    };

    private readonly uint[] _indices =
    {
        // Frente
        0, 1, 2, 2, 3, 0, 4, 5, 6, 6, 7, 4, 8, 9,10,10,11, 8,
        // Atrás
        12,13,14, 14,15,12, 16,17,18, 18,19,16, 20,21,22, 22,23,20,
        // Lados
        0,12,13, 13,1,0, 1,13,14, 14,2,1, 2,14,15, 15,3,2,
        4,16,17, 17,5,4, 5,17,18, 18,6,5, 6,18,19, 19,7,6,
        8,20,21, 21,9,8, 9,21,22, 22,10,9, 10,22,23, 23,11,10,
    };

    public FiguraU(int shaderProgram)
    {
        _shaderProgram = shaderProgram;
        _modelLocation = GL.GetUniformLocation(_shaderProgram, "model");

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    public void DibujarU(float x, float y, float z)
    {
        GL.UseProgram(_shaderProgram);

        // Solo aplicamos traslación (sin rotación)
        Matrix4 model = Matrix4.CreateTranslation(x, y, z);
        GL.UniformMatrix4(_modelLocation, false, ref model);

        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}
