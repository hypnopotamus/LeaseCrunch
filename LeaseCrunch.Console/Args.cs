using CommandLine;
using LeaseCrunch.Domain;

namespace LeaseCrunch.Console;

public class Args
{
    [Option('f', "file", Required = true, HelpText = "Input file to read.")]
    public string File { get; set; }

    [Option('s', "sort", Required = false, HelpText = $"Sort on lease {nameof(ILease.Start)} or lease {nameof(ILease.End)}", Default = nameof(ILease.Start))]
    public string Sort { get; set; } = nameof(ILease.Start);

    [Option('d', "direction", Required = false, HelpText = "Sort asc(ending) or desc(ending)", Default = "asc")]
    public string Direction { get; set; } = "asc";
}