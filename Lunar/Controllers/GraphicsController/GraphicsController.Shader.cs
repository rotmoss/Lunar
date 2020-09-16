using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lunar
{
    partial class GraphicsController
    {
        private Dictionary<uint, uint> _shaders;
        private Dictionary<uint, Dictionary<string, int>> _uniforms;
        public uint CreateShader(string vertexShader, string fragmentShader)
        {
            string[] vertexSource = LoadShader(vertexShader, ShaderType.VertexShader);
            string[] fragmentSource = LoadShader(fragmentShader, ShaderType.FragmentShader);

            for (int i = 0; i < vertexSource.Length; i++)
                vertexSource[i] = Regex.Unescape(vertexSource[i]);
            for (int i = 0; i < fragmentSource.Length; i++)
                fragmentSource[i] = Regex.Unescape(fragmentSource[i]);

            uint vs = CompileShader(vertexSource, ShaderType.VertexShader);
            uint fs = CompileShader(fragmentSource, ShaderType.FragmentShader);

            uint shaderProgram = Gl.CreateProgram();

            if(!AttachShader(vs, fs, shaderProgram)) Gl.DeleteProgram(shaderProgram);

            Gl.DeleteShader(vs);
            Gl.DeleteShader(fs);

            return shaderProgram;
        }

        internal string[] LoadShader(string file, ShaderType type)
        {
            if (!FileManager.ReadLines(file, "Shaders", out string[] shaderSource))
            {
                Console.WriteLine("Could not find shader " + file);
                return type == ShaderType.VertexShader ? _vsDefault : _fsDefault;
            }
            return shaderSource;
        }

        internal bool AttachShader(uint vs, uint fs, uint shader)
        {
            Gl.AttachShader(shader, vs); Gl.AttachShader(shader, fs);
            Gl.LinkProgram(shader); Gl.ValidateProgram(shader);
            Gl.GetProgram(shader, ProgramProperty.LinkStatus, out int linked);
            if (linked == 0) { Console.WriteLine(GetProgramInfo(shader)); return false; }
            return true;
        }

        internal uint CompileShader(string[] source, ShaderType type)
        {
            uint id = Gl.CreateShader(type);
            Gl.ShaderSource(id, source);
            Gl.CompileShader(id);
            Gl.GetShader(id, ShaderParameterName.CompileStatus, out int compiled);
            if (compiled == 0) { Console.WriteLine(GetShaderInfo(id)); return 0; }
            return id;
        }

        internal string GetShaderInfo(uint id)
        {
            StringBuilder infolog = new StringBuilder(1024);
            Gl.GetShaderInfoLog(id, 1024, out _, infolog);
            return infolog.ToString();
        }

        internal string GetProgramInfo(uint id)
        {
            StringBuilder infolog = new StringBuilder(1024);
            Gl.GetProgramInfoLog(id, 1024, out _, infolog);
            return infolog.ToString();
        }

        public void SetUniform<T>(uint id, T data, string uniformName) where T : struct
        {
            Gl.UseProgram(_shaders[id]);

            if (!_uniforms.ContainsKey(id)) _uniforms.Add(id, new Dictionary<string, int>());
            if (!_uniforms[id].ContainsKey(uniformName)) _uniforms[id].Add(uniformName, Gl.GetUniformLocation(_shaders[id], uniformName));

            if (data.GetType() == typeof(Matrix4x4f)) { Gl.UniformMatrix4f(_uniforms[id][uniformName], 1, false, data); }
            else { throw new Exception(); }

            Gl.UseProgram(0);
        }

        internal void UnBindShader() => Gl.UseProgram(0);
        internal void BindShader(uint id) => Gl.UseProgram(_shaders[id]);
        internal void DeleteShader(uint id) => Gl.DeleteProgram(_shaders[id]);
        public void ForeachShader(Action<uint> actions) => _shaders.Keys.ToList().ForEach(actions);

        readonly string[] _vsDefault =
        {
            "#version 330 core\n",
            "layout(location = 0) in vec3 aPos;\n",
            "layout(location = 1) in vec2 aTexCoord;\n",
            "uniform mat4 uProjection;\n",
            "uniform mat4 uModelView;\n",
            "uniform mat4 uCameraView;\n",
            "out vec2 TexCoord;\n",
            "\n",
            "void main()\n",
            "{\n",
            "   gl_Position = uProjection * uCameraView * uModelView * vec4(aPos, 1.0);\n",
            "   TexCoord = aTexCoord;\n",
            "}\n"
        };

        readonly string[] _fsDefault =
        {
            "#version 330 core\n",
            "out vec4 FragColor;\n",
            "in vec2 TexCoord;\n",
            "uniform sampler2D aTexture;\n",
            "void main()\n",
            "{\n",
            "   vec4 color = texture(aTexture, TexCoord);\n",
            "   FragColor = color; \n",
            "}\n",
        };
    }
}
