using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lunar
{
    public struct ShaderObject
    {
        public string VertexShader;
        public string FragmentShader;
        public uint ShaderProgram;
        public Dictionary<string, int> Uniforms;
    }
    partial class GraphicsController
    {
        public List<ShaderObject> _shaderPrograms;
        public Dictionary<string, uint> _vertexShaders;
        public Dictionary<string, uint> _fragmentShaders;

        public uint CreateShader(string vertexShader, string fragmentShader)
        {
            vertexShader = vertexShader == null ? "" : vertexShader;
            fragmentShader = fragmentShader == null ? "" : fragmentShader;

            foreach (ShaderObject shader in _shaderPrograms) { if (shader.VertexShader == vertexShader && shader.FragmentShader == fragmentShader) { return shader.ShaderProgram; } }

            uint vs, fs;

            if (_vertexShaders.ContainsKey(vertexShader)) { vs = _vertexShaders[vertexShader]; }
            else { vs = CompileShader(LoadShader(vertexShader, ShaderType.VertexShader), ShaderType.VertexShader); ; if (vs != 0) _vertexShaders.Add(vertexShader, vs); }
            if (_fragmentShaders.ContainsKey(fragmentShader)) { fs = _fragmentShaders[fragmentShader]; }
            else { fs = CompileShader(LoadShader(fragmentShader, ShaderType.FragmentShader), ShaderType.FragmentShader); if(fs != 0) _fragmentShaders.Add(fragmentShader, fs); }

            uint shaderProgram = Gl.CreateProgram();
            if(!AttachShader(vs, fs, shaderProgram)) Gl.DeleteProgram(shaderProgram);

            _shaderPrograms.Add(new ShaderObject { VertexShader = vertexShader, FragmentShader = fragmentShader, ShaderProgram = shaderProgram, Uniforms = new Dictionary<string, int>() });
            return shaderProgram;
        }

        internal string[] LoadShader(string file, ShaderType type)
        {
            string[] shaderSource;
            if (!FileManager.ReadLines(file, "Shaders", out shaderSource))
            {
                Console.WriteLine("Could not find shader " + file);
                shaderSource = type == ShaderType.VertexShader ? _vsDefault : _fsDefault;
            }

            Array.ForEach(shaderSource, x => x = Regex.Unescape(x));
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
            if (compiled == 0) { Console.WriteLine(GetShaderInfo(id)); Gl.DeleteShader(id); return 0; }
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

        public void SetUniform<T>(ShaderObject shaderObject, T data, string uniformName) where T : struct
        {
            Gl.UseProgram(shaderObject.ShaderProgram);
            if (!shaderObject.Uniforms.ContainsKey(uniformName)) {
                shaderObject.Uniforms.Add(uniformName, Gl.GetUniformLocation(shaderObject.ShaderProgram, uniformName));
            }

            if (data.GetType() == typeof(Matrix4x4f)) { Gl.UniformMatrix4f(shaderObject.Uniforms[uniformName], 1, false, data); }
            else { throw new Exception(); }

            Gl.UseProgram(0);
        }

        public void DisposeShaders()
        {
            _shaderPrograms.ForEach(x => Gl.DeleteProgram(x.ShaderProgram));
            _vertexShaders.Values.ToList().ForEach(x => Gl.DeleteShader(x));
            _fragmentShaders.Values.ToList().ForEach(x => Gl.DeleteShader(x));
        }

        public void ForEachShader(Action<ShaderObject> actions) { foreach(ShaderObject shaderObject in _shaderPrograms) actions(shaderObject); }

        readonly string[] _vsDefault =
        {
            "#version 330 core\n",
            "layout(location = 0) in vec3 aPos;\n",
            "layout(location = 1) in vec2 aTexCoord;\n",
            "uniform mat4 uProjection;\n",
            "uniform mat4 uCameraView;\n",
            "out vec2 TexCoord;\n",
            "\n",
            "void main()\n",
            "{\n",
            "   gl_Position = uProjection * uCameraView * vec4(aPos, 1.0);\n",
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
