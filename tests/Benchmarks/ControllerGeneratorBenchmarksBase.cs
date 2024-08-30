using AutoApiGen.Generators;
using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Benchmarks;

//[Config(typeof(ShortRunConfig))]
[MemoryDiagnoser]
public abstract class ControllerGeneratorBenchmarksBase<TCodeProvider> 
    where TCodeProvider : ICodeProvider
{
    private readonly CSharpGeneratorDriver _driver = CSharpGeneratorDriver.Create(new ControllersGenerator());

    private readonly Compilation _compilation = CSharpCompilation.Create(
        "assemblyName",
        syntaxTrees: [CSharpSyntaxTree.ParseText(TCodeProvider.Code)],
        references: AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Concat([MetadataReference.CreateFromFile(typeof(ControllersGenerator).Assembly.Location)]),
        options: new(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true)
    );

    [Benchmark]
    public Compilation RunGeneratorsAndUpdateCompilation()
    {
        _driver.RunGeneratorsAndUpdateCompilation(_compilation, out var outputCompilation, out _);

        return outputCompilation;
    }
}
