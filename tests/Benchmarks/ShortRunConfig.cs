using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Benchmarks;

public sealed class ShortRunConfig : ManualConfig
{
    public ShortRunConfig() => AddJob(Job.ShortRun);
}