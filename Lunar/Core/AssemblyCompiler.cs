using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;
using Lunar.IO;

namespace Lunar.Compiler
{
    public static class AssemblyCompiler
    {
        public static readonly string[] refPaths =
            {
                typeof(object).GetTypeInfo().Assembly.Location,
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.Console.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.Runtime.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.Collections.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\System.Linq.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\netstandard.dll",
                Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location) + "\\mscorlib.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\Lunar.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\SDL2-CS.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\OpenGL.Net.dll",
                AppDomain.CurrentDomain.BaseDirectory + "\\OpenGL.Net.Math.dll"
            };

        public static async Task<Assembly> CompileScripts()
        {
            Task<List<SyntaxTree>> syntaxTrees = Task.Run(LoadScripts);

            List<MetadataReference> references = new List<MetadataReference>();
            foreach (string refPath in refPaths)
            {
                try { references.Add(MetadataReference.CreateFromFile(refPath)); }
                catch { Console.WriteLine("Could not load dll: " + refPath); }
            }

            syntaxTrees.Wait();

            CSharpCompilation compilation = await Task.Run(() => CSharpCompilation.Create(Path.GetRandomFileName(), syntaxTrees.Result, 
                references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true)));

            return await Task.Run(() => CompileAssembly(compilation));
        }

        public static List<SyntaxTree> LoadScripts()
        {
            string path = FileManager.Path + "Scripts" + FileManager.Seperator;
            if (!Directory.Exists(path)) return null;

            string[] scripts = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();

            for (int i = 0; i < scripts.Length; i++)
            {
                using (var stream = File.OpenRead(path + scripts[i].Split(FileManager.Seperator)[^1]))
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path));
            }

            return syntaxTrees;
        }

        public static System.Reflection.Assembly CompileAssembly(CSharpCompilation compilation)
        {
            Console.WriteLine("Scripts compile start...");

            using MemoryStream ms = new MemoryStream();
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                Console.WriteLine("Compilation failed!");

                List<Diagnostic> failures = new List<Diagnostic>();

                foreach (Diagnostic diagnostic in result.Diagnostics) {
                    if (diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error) 
                        failures.Add(diagnostic);
                }


                foreach (Diagnostic diagnostic in failures) {
                    Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                }

                return null;
            }

            Console.WriteLine("Scripts Compiled!");

            ms.Seek(0, SeekOrigin.Begin);
            return AssemblyLoadContext.Default.LoadFromStream(ms);
        }
    }
}
