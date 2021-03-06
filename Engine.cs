using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using Silk.NET.SDL;
using Lunar.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Lunar.ECS;

namespace Lunar
{
    static class Engine
    {
        public static readonly string Seperator = System.IO.Path.DirectorySeparatorChar.ToString();
        public static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "GameData" + Seperator;

        public static Sdl SDLContext { get => _sdlContext; }
        private static Sdl _sdlContext;

        public static GL GL { get => _glContext; }
        private static GL _glContext;

        public static Random rnd;

        private static IWindow _window;

        public static void Initialize() 
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 800);
            options.Title = "LearnOpenGL with Silk.NET";
            options.VSync = false;
            _window = Silk.NET.Windowing.Window.Create(options);
            _sdlContext = Sdl.GetApi();
            _shaderStorage = new Dictionary<string, IShaderStorageBuffer>();

            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Update += OnUpdate;
            _window.Closing += OnClose;
        }

        public static void Run() => _window.Run();

        public static void OnLoad() 
        {
            _glContext = GL.GetApi(_window);
            _glContext.ClearColor(0.2f, 0.2f, 0.2f, 1f);

            _glContext.CullFace(CullFaceMode.Front);
            _glContext.FrontFace(FrontFaceDirection.CW);

            _glContext.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _glContext.BlendEquation(BlendEquationModeEXT.FuncAdd);
            _glContext.Enable(EnableCap.Blend);

            rnd = new Random();

            Scene.LoadScene("start.xml");

            /*
            Gameobject.Collection.Add(new Gameobject("player"), new Scene("start"));
            Transform.Collection.Add(new Transform(0, 0, 0, 1, 1, 1), Gameobject.Collection["player"]);
            Sprite.Collection.Add(new Sprite(Material.CreateMaterial(ShaderProgram.CreateShaderProgram("default"), OpenGL.Texture.LoadImage("sprite_1.png"))), Gameobject.Collection["player"]);
            */
        }

        public static void OnUpdate(double obj) 
        {

        }

        public static void OnRender(double obj) 
        {
            GL.Clear((uint)ClearBufferMask.ColorBufferBit);

            List<ShaderProgram> shaderPrograms =  ShaderProgram.ShaderPrograms;

            for (int i = 0; i < shaderPrograms.Count; i++) 
            {
                VertexArrayObject VAO = VertexArrayObject.GetVAOByShader(shaderPrograms[i]);
                GL.UseProgram(shaderPrograms[i].Id);
                VAO.Bind();

                List<Material[]> materials = Material.GetMaterials(shaderPrograms[i]);

                for (int j = 0; j < materials.Count; j++)
                {
                    RenderData.GetVertices(materials[j], out List<float> vertices, out List<uint> indices);

                    for (int k = 0; k < materials[j].Length; k++)
                        materials[j][k].BindTexture();

                    VAO.Draw(vertices.ToArray(), indices.ToArray());
                }
            }
        }

        public static void OnClose() 
        {

        }

        private static float _ratio = 16.0f / 9.0f;
        private static bool _stretch = false;

        public static void SetViewport(int w, int h)
        {
            double newRatio = w / h;

            SetShaderStorage("projection", _stretch ? CreateMatrix4X4(0, 0, 1 / 1280.0, 1 / 720.0) : CreateMatrix4X4(0, 0, 1 / (1280.0 / (_ratio / newRatio)), 1 / 720.0));
            SetShaderStorage("aspectRatio", (float)newRatio);
        }

        public static Dictionary<string, IShaderStorageBuffer> _shaderStorage;
        public static void SetShaderStorage<T>(string name, T data) => _shaderStorage[name].Data = data; 
        public static Matrix4X4<float> CreateMatrix4X4(double x, double y, double w, double h) => new Matrix4X4<float>((float)w, 0, 0, 0, 0, (float)h, 0, 0, 0, 0, 1, 0, (float)x, (float)y, 0, 1);

        private static string[] GetDirectories(string directory, out bool error)
        {
            try
            {
                error = false;
                return Directory.GetFiles(Path + directory, "*.*", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                error = true;
                return null;
            }
        }

        private static string FindFile(string file, string directory)
        {
            if (file == null) { return null; }

            string[] directories = GetDirectories(directory, out bool dir_error);

            if (!dir_error)
            {
                foreach (string temp in directories)
                {
                    string[] tokens = temp.Split(Seperator);

                    if (file.ToLower() == tokens[^1].ToLower())
                    {
                        return temp;
                    }
                }
            }
            return "";
        }
        public static string[] ReadLines(string file, string directory, out bool error)
        {
            try
            {
                error = false;
                return File.ReadAllLines(FindFile(file, directory));
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                error = true;
                return null;
            }
            catch
            {
                error = true;
                return null;
            }
        }
        public static string ReadText(string file, string directory, out bool error)
        {
            try
            {
                error = false;
                return File.ReadAllText(FindFile(file, directory));
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                error = true;
                return null;
            }
        }
        private static void WriteLines(string file, string directory, string[] lines, out bool error)
        {
            try
            {
                error = false;
                File.WriteAllLines(FindFile(file, directory), lines);
                return;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                error = true;
                return;
            }
        }
    }
}