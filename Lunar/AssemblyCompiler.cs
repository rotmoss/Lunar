﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Lunar.IO;

namespace Lunar
{
    public class AssemblyCompiler
    {
        private static AssemblyCompiler instance;
        public static AssemblyCompiler Instance { get { instance ??= new AssemblyCompiler(); return instance; } }

        public static readonly string[] refPaths =
            {
                typeof(object).GetTypeInfo().Assembly.Location,
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.Console.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.Runtime.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.Collections.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.Linq.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.Numerics.Vectors.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\netstandard.dll", 
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\mscorlib.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\Lunar.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\Lunar.Scripts.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\Lunar.Input.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\Lunar.Graphics.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\Lunar.Scene.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\Lunar.Physics.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\Lunar.StopWatch.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\Lunar.Math.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\Lunar.Audio.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\SDL2-CS.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\OpenGL.Net.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\OpenGL.Net.Math.dll"
            };

    public async Task<Assembly> CompileScripts()
        {
           
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

            SyntaxTree[] syntaxTrees = await Task.Run(LoadScripts);
            CSharpCompilation compilation = await Task.Run(() => CSharpCompilation.Create(Path.GetRandomFileName(),
                syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)));
            return await Task.Run(() => CompileAssembly(compilation));
        }

        public SyntaxTree[] LoadScripts()
        {
            string path = FileManager.Path + "Scripts" + FileManager.Seperator;
            if (!Directory.Exists(path)) return null;

            string[] scripts = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();

            for (int i = 0; i < scripts.Length; i++)
            {
                string[] temp = scripts[i].Split(FileManager.Seperator);

                string text = FileManager.ReadText(temp[^1], "Scripts" + FileManager.Seperator,
                    out bool error);
                if (!error && temp[^1].Split('.')[1] == "cs")
                {
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(text));
                }
            }

            return syntaxTrees.ToArray();
        }

        public Assembly CompileAssembly(CSharpCompilation compilation)
        {
            using MemoryStream ms = new MemoryStream();
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                Console.WriteLine("Compilation failed!");

                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Diagnostic diagnostic in failures) 
                    Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());

                return null;
            }

            ms.Seek(0, SeekOrigin.Begin);
            return AssemblyLoadContext.Default.LoadFromStream(ms);
        }
    }
}
