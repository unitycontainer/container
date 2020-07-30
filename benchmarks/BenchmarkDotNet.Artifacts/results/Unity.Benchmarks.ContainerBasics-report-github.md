``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.959 (1909/November2018Update/19H2)
AMD Ryzen Threadripper 2970WX, 1 CPU, 48 logical and 24 physical cores
.NET Core SDK=5.0.100-preview.8.20363.2
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.36102, CoreFX 5.0.20.36102), X64 RyuJIT
  Job-PPFHMS : .NET Core 5.0.0 (CoreCLR 5.0.20.36102, CoreFX 5.0.20.36102), X64 RyuJIT

Runtime=.NET Core 5.0  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
|             Method |     Mean |      Error |    StdDev |
|------------------- |---------:|-----------:|----------:|
|         Register() | 7.632 μs |  1.0376 μs | 0.0569 μs |
| RegisterBaseline() | 7.343 μs |  0.3218 μs | 0.0176 μs |
|     RegisterType() | 7.651 μs |  1.3150 μs | 0.0721 μs |
| RegisterInstance() | 8.031 μs | 10.5654 μs | 0.5791 μs |
|  RegisterFactory() | 7.849 μs |  3.5032 μs | 0.1920 μs |
