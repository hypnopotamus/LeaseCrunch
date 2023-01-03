using Autofac;
using Autofac.Extensions.DependencyInjection;
using CommandLine;
using LeaseCrunch.Actions;
using LeaseCrunch.Console;
using LeaseCrunch.Console.Output;
using LeaseCrunch.CSVInput;
using LeaseCrunch.Domain;
using LeaseCrunch.Domain.Errors;
using LeaseCrunch.Storage;
using Microsoft.Extensions.DependencyInjection;

var parsedArgs = Parser.Default.ParseArguments<Args>(args)
    .WithNotParsed(Console.Write)
    .WithParsed(
        a =>
        {
            if (a.Direction is not "asc" or "desc")
            {
                throw new ArgumentException($"{nameof(Args.Direction)} must be asc or desc");
            }

            if (a.Sort is not nameof(ILease.Start) or nameof(ILease.End))
            {
                throw new ArgumentException($"{nameof(Args.Sort)} must be {nameof(ILease.Start)} or {nameof(ILease.End)}");
            }
        });
if (parsedArgs.Errors.Any()) return;

var sort = new Sort
{
    Direction = parsedArgs.Value.Direction switch
    {
        "asc" => SortDirection.Ascending,
        "desc" => SortDirection.Descending,
        _ => SortDirection.Ascending,
    },
    Property = parsedArgs.Value.Sort switch
    {
        nameof(ILease.Start) => SortableProperties.StartDate,
        nameof(ILease.End) => SortableProperties.EndDate,
        _ => SortableProperties.StartDate,
    }
};

try
{
    var (importer, repository) = ImportFactory();

    Console.WriteLine("Processing");
    await importer.Import(parsedArgs.Value.File);

    Console.WriteLine("Imported:");
    foreach (var lease in await repository.GetLeases(sort))
    {
        Console.WriteLine(lease.Format());
    }
}
catch (InvalidRowFormatException e)
{
    Console.Error.WriteLine(e.Format());
}
catch (InvalidLeaseException e)
{
    Console.Error.WriteLine(e.Format());
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
}

(IFileImport, ILeaseRepository) ImportFactory()
{
    var services = new ServiceCollection();
    var containerBuilder = new ContainerBuilder();

    services.AddDbContext<LeaseDbContext>();
    containerBuilder.Populate(services);
    containerBuilder
        .RegisterAssemblyTypes
        (
            typeof(ILease).Assembly,
            typeof(IFileImport).Assembly,
            typeof(CSVReader).Assembly,
            typeof(ILeaseRepository).Assembly
        )
        .AsImplementedInterfaces()
        .AsSelf();
    containerBuilder.Register<FileReaderFactory>
    (
        provider =>
        {
            var context = provider.Resolve<IComponentContext>();

            return path => path.Split(".").Last() switch
            {
                "csv" => context.Resolve<CSVReader>(),
                _ => throw new NotImplementedException()
            };
        }
    );

    var container = containerBuilder.Build();
    return (container.Resolve<IFileImport>(), container.Resolve<ILeaseRepository>());
}