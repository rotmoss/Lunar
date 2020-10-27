﻿using OpenGL;
using SDL2;
using System;
using Lunar.Scenes;
using Lunar.Physics;
using Lunar.Graphics;
using Lunar.Input;
using Lunar.Scripts;
using Lunar.Stopwatch;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Lunar.Audio;
using System.Collections.Generic;

namespace Lunar
{ 
    public class Lunar
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static void LoadScene()
        {
            var assemblyAwaiter = AssemblyCompiler.CompileScripts();

            InputController.Init();

            InputController.OnWindowClose += OnWindowClose;
            InputController.OnWindowSizeChanged += OnWindowSizeChanged;
            InputController.OnKeyDown += OnKeyDown;

            Scene.LoadScene("start.xml", out List<ScriptInfo> scripts);

            assemblyAwaiter.Wait();
            Script.Assembly = assemblyAwaiter.Result;
            Script.AddScripts(scripts);

            assemblyAwaiter.Dispose();
        }


        static void OnKeyDown(object sender, KeyboardState eventArgs)
        {
            if (eventArgs.RawKeyStates[SDL.SDL_Keycode.SDLK_LALT] && eventArgs.RawKeyStates[SDL.SDL_Keycode.SDLK_RETURN])
            {
                Window.ToggleFullscreen();
                Window.UpdateWindowSize();
            }
        }

        static void OnWindowClose(object sender, EventArgs eventArgs)
        {
            Mixer.Dispose();
            Window.Close();
            Environment.Exit(0);
        }

        static void OnWindowSizeChanged(object sender, EventArgs eventArgs)
        {
            Window.UpdateWindowSize();
        }

        static void Main(string[] args)
        {
            Task task = Task.Run(() => LoadScene());
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0 || SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) < 0 || SDL_ttf.TTF_Init() < 0) //Init SDL
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }
            if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) < 0)
            { Console.WriteLine("Couldn't initialize SDL: %s\n" + SDL.SDL_GetError()); SDL.SDL_Quit(); }

            Window.Init(1280, 720, false);

            Window.AddLayers("Background", "Sprite", "Foreground", "Collider");
            Window.SetLineWidth(2);
            Window.CreateFramebuffer();

            Mixer.Init();

            task.Wait();
            GC.Collect();

            Script.InitScripts();
            Script.LateInitScripts();

            while (true)
            {
                Time.StartFrameTimer();

                //Invoke Input-Events
                InputController.PollInputs();

                //Update all scripts
                Script.UpdateScripts();

                //Update Transforms
                Force.ApplyForces();

                //Check Colissions
                Collider.CheckColissions();

                //Update all scripts again
                Script.LateUpdateScripts();

                //Use Transfroms to Translate Graphics Data
                Window.TranslateBuffers("aPos");

                AnimatedSprite.Animate(Time.FrameTime);

                //Render Graphics
                Window.Render();

                //Update all scripts again
                Script.PostRenderUpdateScripts();

                //Present FrameBuffer
                Window.SwapBuffer();

                Time.StopFrameTimer();
            }
        }
    }
}
