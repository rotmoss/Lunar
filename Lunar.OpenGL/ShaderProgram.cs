using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunar.OpenGL
{
    public class ShaderProgram
    {
        public uint Id { get => _id; }
        private uint _id;

        public VertexFormat VertexFormat { get => _vertexFormat; }
        private VertexFormat _vertexFormat;

        public static List<ShaderProgram> ShaderPrograms { get => _shaderPrograms; }
        private static List<ShaderProgram> _shaderPrograms = new List<ShaderProgram>();

        private static Dictionary<string, ShaderProgram> _shaderProgramsByName = new Dictionary<string, ShaderProgram>();
        
        private static string _path = Engine.Path + Engine.Seperator + "Shaders" + Engine.Seperator;

        private ShaderProgram(uint id)
        {
            _id = id;
            _vertexFormat = GetVertexFormat(id);
        }

        public static ShaderProgram CreateShaderProgram(string name) 
        {
            ShaderProgram program;

            if (_shaderProgramsByName.ContainsKey(name)) { return _shaderProgramsByName[name]; }
            
            uint id = LoadShaderProgram(name);
            if(id == 0) return null;

            program = new ShaderProgram(id);

            _shaderPrograms.Add(program);
            _shaderProgramsByName.Add(name, program);
            VertexArrayObject.CreateVertexArray(program);

            return program;
        }

        private static uint LoadShaderProgram(string name) 
        {
            uint program = 0;
            string source;

            try { source = System.IO.File.ReadAllText(_path + name); }
            catch { return program; }

            if(!CompileShader(source, out uint vs, ShaderType.VertexShader)) return program;
            if(!CompileShader(source, out uint fs, ShaderType.FragmentShader)) return program;
            if(!CompileProgram(vs, fs, out program)) return program;

            int[] samplers = new int[32];
            for(int i = 0; i < samplers.Length; i++) samplers[i] = i;

            Engine.GL.UseProgram(program);
            Engine.GL.Uniform1(Engine.GL.GetUniformLocation(program, "textures"), 32, samplers);

            return program;
        }

        public static unsafe bool CompileShader(string shaderSource, out uint id, ShaderType type) 
        {
            string define = "#version 450" + Environment.NewLine;
            define += (type == ShaderType.VertexShader ? "#define VERTEX" : "#define FRAGMENT") + Environment.NewLine;
            string[] source = { define, shaderSource };
            int[] length = { define.Length, shaderSource.Length };

            id = Engine.GL.CreateShader(type);

            fixed (int* lengthPointer = &length[0]) {
                Engine.GL.ShaderSource(id, 2, source, lengthPointer);
            }

            Engine.GL.CompileShader(id);
            Engine.GL.GetShader(id, ShaderParameterName.CompileStatus, out int compiled);

            if(compiled == 0) { 
                Console.WriteLine(Engine.GL.GetShaderInfoLog(id));
                Engine.GL.DeleteShader(id); 
                return false; 
            }

            return true;
        }

        public static bool CompileProgram(uint vs, uint fs, out uint program) 
        {
            program = Engine.GL.CreateProgram();
            Engine.GL.AttachShader(program, vs); 
            Engine.GL.AttachShader(program, fs);
            Engine.GL.LinkProgram(program);

            Engine.GL.GetProgram(program, ProgramPropertyARB.LinkStatus, out int linked);
            
            if (linked == 0) { 
                Console.WriteLine(Engine.GL.GetProgramInfoLog(program)); 
                Engine.GL.DeleteShader(vs); 
                Engine.GL.DeleteShader(fs); 
                Engine.GL.DeleteProgram(program); 
                return false; 
            }

            Engine.GL.ValidateProgram(program);
            Engine.GL.DeleteShader(vs); 
            Engine.GL.DeleteShader(fs);

            return true;
        }

        public static void Dispose() 
        {
            foreach (ShaderProgram shaderProgram in _shaderPrograms) {
                Engine.GL.DeleteProgram(shaderProgram.Id);
            }

            _shaderPrograms.Clear();
            _shaderProgramsByName.Clear();
        }

        private static unsafe VertexFormat GetVertexFormat(uint program)
        {
            int attrbutes = 0;
            Engine.GL.GetProgramInterface(program, ProgramInterface.ProgramInput, GLEnum.ActiveResources, &attrbutes);

            int[] results = new int[2];
            GLEnum[] properties = new GLEnum[] { GLEnum.Location, GLEnum.Type };
            VecInfo[] Vecs = new VecInfo[attrbutes];

            fixed (GLEnum* property = &properties[0])
            {
                fixed (int* result = &results[0])
                {
                    for (uint i = 0; i < attrbutes; i++)
                    {
                        Engine.GL.GetProgramResource(program, GLEnum.ProgramInput, i, 2, &property[0], 2, null, result);

                        int length;
                        if (result[1] == (int)GLEnum.Float) length = 1;
                        else length = result[1] - (int)GLEnum.FloatVec2 + 2;

                        Vecs[i] = new VecInfo(result[0], length);
                    }
                }
            }

            Vecs = Vecs.OrderBy(x => x.AttribLocation).ToArray();

            for (int i = 0; i < Vecs.Length; i++)
            {
                int offsetBytes = 0;
                int offsetLength = 0;

                for (int j = i; j > 0; j--)
                {
                    offsetBytes += Vecs[j - 1].Size;
                    offsetLength += Vecs[j - 1].Length;
                }

                Vecs[i].SetOffsetBytes(offsetBytes);
                Vecs[i].SetOffsetLength(offsetLength);
            }

            return new VertexFormat(Vecs);
        }


        public override int GetHashCode() => (int)Id;
        public override bool Equals(object obj) => Equals(obj as ShaderProgram);
        public bool Equals(ShaderProgram obj) => obj != null && obj.Id == Id;
    }
}