using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

class FiguraU
{
    private int _vao, _vbo, _ebo;
    private int _shaderProgram;
    private int _modelLocation;

    private float[] _vertices;
    private uint[] _indices;

    public FiguraU(int shaderProgram, string verticesPath, string indicesPath)
    {
        _shaderProgram = shaderProgram;
        _modelLocation = GL.GetUniformLocation(_shaderProgram, "model");

        // Cargar vértices e índices desde los archivos JSON
        _vertices = JsonLoader.LoadFromJson<float[]>(verticesPath);
        _indices = JsonLoader.LoadFromJson<uint[]>(indicesPath);

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

        Matrix4 model = Matrix4.CreateTranslation(x, y, z);
        GL.UniformMatrix4(_modelLocation, false, ref model);

        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}