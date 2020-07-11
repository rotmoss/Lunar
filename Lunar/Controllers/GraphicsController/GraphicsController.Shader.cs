using System;
using System.Collections.Generic;
using OpenGL;
using System.Text;
using System.Linq;

namespace Lunar
{
    partial class GraphicsController
    {
        private Dictionary<uint, uint> _shader;

        public uint CreateShader(string vertexShader, string fragmentShader)
        {
            string[] vertexSource = LoadShader(vertexShader, ShaderType.VertexShader);
            string[] fragmentSource = LoadShader(fragmentShader, ShaderType.FragmentShader);

            uint vs = CompileShader(vertexSource, ShaderType.VertexShader);
            uint fs = CompileShader(fragmentSource, ShaderType.FragmentShader);

            uint shaderprogram = Gl.CreateProgram();

            if(!AttachShader(vs, fs, shaderprogram)) Gl.DeleteProgram(shaderprogram);

            Gl.DeleteShader(vs);
            Gl.DeleteShader(fs);

            return shaderprogram;
        }

        public string[] LoadShader(string file, ShaderType type)
        {
            if (!FileManager.ReadLines(file, "Shaders", out string[] shaderSource))
            {
                Console.WriteLine("Could not find shader " + file);
                return type == ShaderType.VertexShader ? _vsDefault : _fsDefault;
            }
            return shaderSource;
        }

        public bool AttachShader(uint vs, uint fs, uint shader)
        {
            Gl.AttachShader(shader, vs); Gl.AttachShader(shader, fs);
            Gl.LinkProgram(shader); Gl.ValidateProgram(shader);
            Gl.GetProgram(shader, ProgramProperty.LinkStatus, out int linked);
            if (linked == 0) { Console.WriteLine(GetProgramInfo(shader)); return false; }
            return true;
        }

        uint CompileShader(string[] source, ShaderType type)
        {
            uint id = Gl.CreateShader(type);
            Gl.ShaderSource(id, source);
            Gl.CompileShader(id);
            Gl.GetShader(id, ShaderParameterName.CompileStatus, out int compiled);
            if (compiled == 0) { Console.WriteLine(GetShaderInfo(id)); return 0; }
            return id;
        }

        public string GetShaderInfo(uint id)
        {
            StringBuilder infolog = new StringBuilder(1024);
            Gl.GetShaderInfoLog(id, 1024, out int infologLength, infolog);
            return infolog.ToString();
        }

        public string GetProgramInfo(uint id)
        {
            StringBuilder infolog = new StringBuilder(1024);
            Gl.GetProgramInfoLog(id, 1024, out int infologLength, infolog);
            return infolog.ToString();
        }

        public void SetUniform<T>(uint id, T data, string uniformName) where T : struct
        {
            Gl.UseProgram(_shader[id]);

            if (data.GetType() == typeof(Matrix4x4f)) { Gl.UniformMatrix4f(Gl.GetUniformLocation(_shader[id], uniformName), 1, false, data); }
            else { throw new Exception(); }

            Gl.UseProgram(0);
        }

        public void UnBindShader() => Gl.UseProgram(0);
        public void BindShader(uint id) => Gl.UseProgram(_shader[id]);
        public void DeleteShader(uint id) => Gl.DeleteProgram(_shader[id]);

        public void ForeachShader(Action<uint> actions) => _shader.Keys.ToList().ForEach(actions);

        string[] _vsDefault =
        {
            "#version 330 core\n",
            "layout(location = 0) in vec3 aPos;\n",
            "layout(location = 1) in vec2 aTexCoord;\n",
            "uniform mat4 uProjection;\n",
            "uniform mat4 uModelView;\n",
            "out vec2 TexCoord;\n",
            "\n",
            "int roundUp(int numToRound, int multiple)\n",
            "{\n",
            "   if (multiple == 0) {\n",
            "       return numToRound;\n",
            "   }\n",
            "\n",
            "   int remainder = numToRound % multiple;\n",
            "   if (remainder == 0) {\n",
            "       return numToRound;\n",
            "   }\n",
            "\n",
            "   if (numToRound < 0) {\n",
            "       return -(abs(numToRound) - remainder);\n",
            "   }\n",
            "   else {\n",
            "       return numToRound + multiple - remainder;\n",
            "   }\n",
            "}",
            "\n",
            "void main()\n",
            "{\n",
            "   vec3 newPos = vec3(roundUp(int(round(aPos.x)) * 4, 8), roundUp(int(round(aPos.y)) * 4, 8), aPos.z);\n",
            "   gl_Position = uProjection * vec4(newPos, 1.0);\n",
            "   TexCoord = aTexCoord;\n",
            "}\n"
        };
        string[] _fsDefault =
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
