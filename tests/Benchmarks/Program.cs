using BenchmarkDotNet.Running;
using Benchmarks;

BenchmarkRunner.Run<GeneralControllerGeneratorBenchmarks>();
// BenchmarkRunner.Run<ALotOfControllersGeneratorBenchmark>();
// BenchmarkRunner.Run<SingleBloatedControllerGeneratorBenchmark>();