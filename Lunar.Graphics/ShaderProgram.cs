using System;
using System.Collections.Generic;
using OpenGL;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Lunar.Graphics
{
    public class ShaderProgram
    {
        public uint id;
        public Shader vs;
        public Shader fs;
        public Dictionary<string, int> uniforms;

        private static List<ShaderProgram> _shaderPrograms = new List<ShaderProgram>();
        private static List<Shader> _shaders = new List<Shader>();

        readonly static string[] _vsDefault =
{
            "#version 330 core",
            "layout(location = 0) in vec3 aPos;",
            "layout(location = 1) in vec2 aTexCoord;",
            "uniform mat4 uProjection;",
            "uniform mat4 uCameraView;",
            "out vec2 TexCoord;",
            "",
            "void main()",
            "{",
            "   gl_Position = uProjection * uCameraView * vec4(aPos, 1.0);",
            "   TexCoord = aTexCoord;",
            "}"
        };

        readonly static string[] _fsDefault =
        {
            "#version 330 core",
            "out vec4 FragColor;",
            "in vec2 TexCoord;",
            "uniform sampler2D aTexture;",
            "void main()",
            "{",
            "   vec4 color = texture(aTexture, TexCoord);",
            "   FragColor = color;",
            "}",
        };

        public ShaderProgram(Shader vs, Shader fs)
        {
            id = 0;
            this.vs = vs;
            this.fs = fs;
            uniforms = new Dictionary<string, int>();
        }

        public static bool CreateShader(string vertexShader, string fragmentShader, out ShaderProgram shaderProgram)
        {
            Shader vs = new Shader { name = vertexShader == null ? "" : vertexShader, id = 0 };
            Shader fs = new Shader { name = fragmentShader == null ? "" : fragmentShader, id = 0 };

            foreach (Shader shader in _shaders)
            {
                if (shader.name == vertexShader)
                    vs.id = shader.id;
                if (shader.name == fragmentShader) 
                    fs.id = shader.id;
            }

            if (vs.id == 0) {
                if (!vs.CompileShader(LoadShaderSource(vertexShader, ShaderType.VertexShader), ShaderType.VertexShader)) {
                    Gl.DeleteShader(vs.id); Gl.DeleteShader(fs.id); shaderProgram = null; return false;
                }
            }

            if (fs.id == 0) {
                if (!fs.CompileShader(LoadShaderSource(fragmentShader, ShaderType.FragmentShader), ShaderType.FragmentShader)) {
                    Gl.DeleteShader(vs.id); Gl.DeleteShader(fs.id); shaderProgram = null; return false;
                }
            }

            shaderProgram = new ShaderProgram(vs, fs);

            if (!shaderProgram.CompileProgram()) { Gl.DeleteShader(vs.id); Gl.DeleteShader(fs.id); Gl.DeleteProgram(shaderProgram.id); return false; }

            _shaderPrograms.Add(shaderProgram);
            return true;
        }

        private static string[] LoadShaderSource(string file, ShaderType type)
        {
            if (!IO.FileManager.ReadLines(file, "shaders", out string[] shaderSource))
            {
                Console.WriteLine("Could not find shader " + file);
                shaderSource = type == ShaderType.VertexShader ? _vsDefault : _fsDefault;
            }

            for(int i = 0; i < shaderSource.Length; i++)
            {
                shaderSource[i] += "\n";
            }
            return shaderSource;
        }

        public static void ForEachShader(Action<ShaderProgram> actions) { foreach (ShaderProgram shaderObject in _shaderPrograms) actions(shaderObject); }

        internal bool CompileProgram()
        {
            id = Gl.CreateProgram();
            Gl.AttachShader(id, vs.id); Gl.AttachShader(id, fs.id);
            Gl.LinkProgram(id); Gl.ValidateProgram(id);
            Gl.GetProgram(id, ProgramProperty.LinkStatus, out int linked);
            if (linked == 0) { PrintProgramInfo(); return false; }
            return true;
        }

        internal void PrintProgramInfo()
        {
            StringBuilder infolog = new StringBuilder(1024);
            Gl.GetProgramInfoLog(id, 1024, out _, infolog);
            Console.WriteLine(infolog.ToString());
        }

        public void SetUniform<T>(T data, string uniformName) where T : struct
        {
            Gl.UseProgram(id);

            if (!uniforms.ContainsKey(uniformName))
            {
                uniforms.Add(uniformName, Gl.GetUniformLocation(id, uniformName));
            }

            if (data.GetType() == typeof(Matrix4x4f))
            {
                Gl.UniformMatrix4f(uniforms[uniformName], 1, false, data);
            }
            else { throw new Exception(); }

            Gl.UseProgram(0);
        }

        public static Matrix4x4f ToMatrix4x4f(Vector2 pos, Vector2 scale) => new Matrix4x4f(
            scale.X, 0, 0, 0,
            0, scale.Y, 0, 0,
            0, 0, 0, 0,
            pos.X, pos.Y, 0, 1
        );

        public static void DisposeShaders()
        {
            foreach (Shader shader in _shaders) {
                Gl.DeleteShader(shader.id);
            }

            foreach (ShaderProgram shader in _shaderPrograms) {
                Gl.DeleteProgram(shader.id);
            }
        }
    }
}
