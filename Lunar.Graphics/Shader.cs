using OpenGL;
using System;
using System.Text;

namespace Lunar.Graphics
{
    public class Shader
    {
        public uint id;
        public string name;

        internal bool CompileShader(string[] shaderSource, ShaderType type)
        {
            id = Gl.CreateShader(type);
            Gl.ShaderSource(id, shaderSource);
            Gl.CompileShader(id);
            Gl.GetShader(id, ShaderParameterName.CompileStatus, out int compiled);
            if (compiled == 0) { PrintShaderInfo(); Gl.DeleteShader(id); return false; }
            return true;
        }

        internal void PrintShaderInfo()
        {
            StringBuilder infolog = new StringBuilder(1024);
            Gl.GetShaderInfoLog(id, 1024, out _, infolog);
            Console.WriteLine(infolog.ToString());
        }
    }
}
