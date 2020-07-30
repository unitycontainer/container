``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.959 (1909/November2018Update/19H2)
AMD Ryzen Threadripper 2970WX, 1 CPU, 48 logical and 24 physical cores
.NET Core SDK=5.0.100-preview.8.20363.2
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.36102, CoreFX 5.0.20.36102), X64 RyuJIT
  Job-SENGTX : .NET Core 5.0.0 (CoreCLR 5.0.20.36102, CoreFX 5.0.20.36102), X64 RyuJIT

Runtime=.NET Core 5.0  InvocationCount=1  IterationCount=3  
LaunchCount=1  UnrollFactor=1  WarmupCount=3  

```
|             Method |       Mean |      Error |    StdDev |   Median |
|------------------- |-----------:|-----------:|----------:|---------:|
|         Register() |   588.9 ns | 5,450.5 ns | 298.76 ns | 433.3 ns |
|     RegisterType() |   977.8 ns |   928.9 ns |  50.92 ns | 966.7 ns |
| RegisterInstance() | 1,088.9 ns | 6,559.1 ns | 359.53 ns | 933.3 ns |
|  RegisterFactory() | 1,122.2 ns | 9,128.6 ns | 500.37 ns | 833.3 ns |
