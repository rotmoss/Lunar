using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using Silk.NET.SDL;
using Lunar.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using Lunar.ECS;

namespace Lunar
{
    static class Engine
    {
        public static readonly string LineTerminator = Environment.NewLine;
        public static readonly string Seperator = System.IO.Path.DirectorySeparatorChar.ToString();
        public static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "GameData" + Seperator;

        public static Sdl SDLContext { get => _sdlContext; }
        private static Sdl _sdlContext;

        public static GL GL { get => _glContext; }
        private static GL _glContext;

        public static Random Random { get => _random; }
        private static Random _random;

        public static IWindow Window { get => _window; }
        private static IWindow _window;

        public static void Initialize() 
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 800);
            options.Title = "LearnOpenGL with Silk.NET";
            options.VSync = false;
            _window = Silk.NET.Windowing.Window.Create(options);

            _sdlContext = Sdl.GetApi();

            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Update += OnUpdate;
            _window.Closing += OnClose;
            _window.Resize += SetViewport;

            _shaderStorage = new Dictionary<string, IShaderStorageBuffer>();
            _random = new Random();
        }

        public static void Run() => _window.Run();

        public static void OnLoad() 
        {
            _glContext = GL.GetApi(_window);
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1f);

            GL.CullFace(CullFaceMode.Front);
            GL.FrontFace(FrontFaceDirection.CW);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendEquation(BlendEquationModeEXT.FuncAdd);
            GL.Enable(EnableCap.Blend);
    
            _shaderStorage.Add("projection", new ShaderStorageBuffer<Matrix4X4<float>>(0, default));
            _shaderStorage.Add("aspectRatio", new ShaderStorageBuffer<float>(1, 0));
            SetViewport(_window.Size);

            //Scene.LoadScene("start.xml");

            Scene.Collection.Add(new Scene("start"));
        }

        public static void OnUpdate(double obj) 
        {
            for (int i = 0; i < 16; i++) 
            {
                Gameobject g = new Gameobject("");
                Gameobject.Collection.Add(g, Scene.Collection["start"]);
                Transform.Collection.Add(new Transform((float)(Random.NextDouble() - 0.5) * 200, (float)(Random.NextDouble() - 0.5) * 200, 0, 0.1f, 0.1f, 1), g);
                Sprite.Collection.Add(new Sprite(Material.CreateMaterial(ShaderProgram.CreateShaderProgram("default.glsl"), OpenGL.Texture.LoadImage("sprite_" + i + ".png"))), g);
            }

            for (int i = 0; i < Transform.Collection.Count; i++) 
            {
                Transform transform = Transform.Collection[i];

                transform.Position += new Vector3D<float>(transform.Position.X * 0.02f, transform.Position.Y * 0.02f, 0);
                if (MathF.Abs(transform.Position.X) > _width || MathF.Abs(transform.Position.Y) > _height) 
                {
                    Gameobject.Collection.Remove(Gameobject.Collection[transform.Id]);
                    i--;
                } 
            }
        }

        public static void OnRender(double obj) 
        {
            GL.Clear((uint)ClearBufferMask.ColorBufferBit);

            List<ShaderProgram> shaderPrograms =  ShaderProgram.ShaderPrograms;

            for (int i = 0; i < shaderPrograms.Count; i++) 
            {
                VertexArrayObject VAO = VertexArrayObject.GetVAOByShader(shaderPrograms[i]);

                GL.UseProgram(shaderPrograms[i].Id);
                GL.BindVertexArray(VAO.Id);

                List<Material[]> materials = Material.GetMaterials(shaderPrograms[i]);

                for (int j = 0; j < materials.Count; j++)
                {
                    RenderData.GetVertices(RenderData.GetDataFromMaterials(materials[j]), out float[] vertices, out uint[] indices);

                    for (int k = 0; k < materials[j].Length; k++)
                        materials[j][k].BindTexture();

                    VAO.Draw(vertices, indices);
                }
            }
        }

        public static void OnClose() 
        {
            Scene.Collection.Dispose();
            OpenGL.Texture.Dispose();
            OpenGL.ShaderProgram.Dispose();

            foreach(IShaderStorageBuffer s in _shaderStorage.Values) 
                s.Dispose();
        }

        private static float _width;
        private static float _height;
        private static float _ratio = 16.0f / 9.0f;
        private static bool _stretch = false;

        public static void SetViewport(Vector2D<int> size)
        {
            GL.Viewport(size);
            double newRatio = (double)size.X / (double)size.Y;

            SetShaderStorage("projection", _stretch ? CreateMatrix4X4(0, 0, 1 / 1280.0, 1 / 720.0) : CreateMatrix4X4(0, 0, 1 / (1280.0 / (_ratio / newRatio)), 1 / 720.0));
            SetShaderStorage("aspectRatio", (float)newRatio);

            _width = size.X;
            _height = size.Y;
        }

        public static Dictionary<string, IShaderStorageBuffer> _shaderStorage;
        public static void SetShaderStorage<T>(string name, T data) => _shaderStorage[name].Data = data; 
        public static Matrix4X4<float> CreateMatrix4X4(double x, double y, double w, double h) => new Matrix4X4<float>((float)w, 0, 0, 0, 0, (float)h, 0, 0, 0, 0, 1, 0, (float)x, (float)y, 0, 1);
    }
}