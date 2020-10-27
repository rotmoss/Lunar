using System;
using System.Collections.Generic;
using System.Numerics;
using OpenGL;

namespace Lunar.Graphics
{
    partial class ShaderProgram
    {
        public void SetUniform(float data, string uniformName)
        {
            Gl.UseProgram(id);

            if (!uniforms.ContainsKey(uniformName))
                uniforms.Add(uniformName, Gl.GetUniformLocation(id, uniformName));

            Gl.Uniform1f(uniforms[uniformName], 1, data);

            Gl.UseProgram(0);
        }
        public void SetUniform(Vertex2f data, string uniformName)
        {
            Gl.UseProgram(id);

            if (!uniforms.ContainsKey(uniformName))
                uniforms.Add(uniformName, Gl.GetUniformLocation(id, uniformName));

            Gl.Uniform2f(uniforms[uniformName], 1, data);

            Gl.UseProgram(0);
        }
        public void SetUniform(Vertex3f data, string uniformName)
        {
            Gl.UseProgram(id);

            if (!uniforms.ContainsKey(uniformName))
                uniforms.Add(uniformName, Gl.GetUniformLocation(id, uniformName));

            Gl.Uniform3f(uniforms[uniformName], 1, data);

            Gl.UseProgram(0);
        }
        public void SetUniform(Vertex4f data, string uniformName)
        {
            Gl.UseProgram(id);

            if (!uniforms.ContainsKey(uniformName))
                uniforms.Add(uniformName, Gl.GetUniformLocation(id, uniformName));

            Gl.Uniform4f(uniforms[uniformName], 1, data);

            Gl.UseProgram(0);
        }

        public void SetUniform(Vertex2f[] data, string uniformName)
        {
            Gl.UseProgram(id);

            if (!uniforms.ContainsKey(uniformName))
                uniforms.Add(uniformName, Gl.GetUniformLocation(id, uniformName));

            unsafe { fixed (Vertex2f* p_v = data) Gl.Uniform2(uniforms[uniformName], data.Length, (float*)p_v); }

            Gl.UseProgram(0);
        }
        public void SetUniform(Vertex3f[] data, string uniformName)
        {
            Gl.UseProgram(id);

            if (!uniforms.ContainsKey(uniformName))
                uniforms.Add(uniformName, Gl.GetUniformLocation(id, uniformName));

            unsafe { fixed (Vertex3f* p_v = data) Gl.Uniform3(uniforms[uniformName], data.Length, (float*)p_v); }

            Gl.UseProgram(0);
        }
        public void SetUniform(Vertex4f[] data, string uniformName)
        {
            Gl.UseProgram(id);

            if (!uniforms.ContainsKey(uniformName))
                uniforms.Add(uniformName, Gl.GetUniformLocation(id, uniformName));

            unsafe { fixed (Vertex4f* p_v = data) Gl.Uniform4(uniforms[uniformName], data.Length, (float*)p_v); }

            Gl.UseProgram(0);
        }

        public void SetUniformMatrix(Matrix4x4f data, string uniformName)
        {
            Gl.UseProgram(id);

            if (!uniforms.ContainsKey(uniformName))
                uniforms.Add(uniformName, Gl.GetUniformLocation(id, uniformName));

            Gl.UniformMatrix4f(uniforms[uniformName], 1, false, data);

            Gl.UseProgram(0);
        }
        public void SetUniformMatrix(Matrix3x3f data, string uniformName)
        {
            Gl.UseProgram(id);

            if (!uniforms.ContainsKey(uniformName))
                uniforms.Add(uniformName, Gl.GetUniformLocation(id, uniformName));

            Gl.UniformMatrix3f(uniforms[uniformName], 1, false, data);

            Gl.UseProgram(0);
        }
    }
}
