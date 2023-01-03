# LeaseCrunch

## Tools

### Required

1. dotnet 7 sdk

### Recommended

1. Visual Studio
1. Resharper
1. dotCover

## Build

`dotnet build` from the solution folder

or

visual studio build command

## Run

`dotnet run -- -f {full path to a csv file}`

or

`cd ./LeaseCrunch.Console/bin/{Debug|Release}/net7.0`
`LeaseCrunch.Console -f {full path to a csv file}`

### CLI options

these are printed if the arguments fail to be parsed or with --help
1. -f, --file: the path to the csv file to be imported from
1. -s, --sort: either Start or End, default Start
1. -d, --direction: either asc or desc, default asc

### CSV format

The first row will be discarded as a header row
Content rows should have columns of
1. Name
1. Start Date (MM/dd/yyyy)
1. End Date (MM/dd/yyyy)
1. Payment Amount
   - decimal
   - between -1,000,000,000 and 1,000,000,000
1. Number of Payments
   - unsigned integer
   - between 1 and the month difference of Start Date to End Date
1. Interest Rate
   - decimal
   - between 0 and 9.9999 inclusive

## Testing

`dotnet test`

or

test from inside visual studio with either the resharper test runner or visual studio test runner

## Outstanding Questions

1. date format: the example csv has dates formatted as MM?/dd?/yy (e.g. 1/1/20) rather than MM/dd/yyyy (e.g. 01/01/2020)
1. halt on first error:
   - stop on the first error rather than running the whole file and reporting all errors?
   - reprocessing a large file with only some errors should reprocess the entire file again rather than, maybe, staging the successful rows and leaving a memento of unsuccessul rows?
      - i.e. resumable / interruptable batch processing patterns
1. sorting:
   - requirements doc says both that leases are default sorted by Start Date and that users want to see them sorted by End Date
   - ascending or descending?
1. what output format is actually useful for consumers of this application?

## Assumptions

1. lease names are unique
   - this is probably not true in the real world
   - this assumption was made to make the primary key constraint easy to deal with for a PoC coding challenge solution
1. the intended date format is MM/dd/yyyy from the requirements doc, not the format of the example (which is ambiguous)
1. halt on first error: yes, as an MVP with room for improvement
1. sorting:
   - allow either sort direction, ascending or descending
   - default: Start, Ascending
1. output: CLI is acceptable for a coding challenge MVP, printing the lease data back out into the console directly with little formatting or manipulation
   - I suspect this wouldn't be what a real user of this product would want, either in format or in delivery
      - mention of which months have payments when the term has more months than the lease has payments implies some need to see which months have payments and which don't... that math is currently left to the user
      - non-technical consumers tend to get nervous when they have to use the console for anything and a console UI tends to be less interactive than desirable for most scenarios
1. persistence: fine for an MVP for data to be "imported into the system" only for the duration the executable is running
   - easy to switch the database provider of entity framework for a real database (or, even, SQLite)
   - easy to separate import from view so that it's not required to import a file to see previously imported leases... assuming the data persists longer than the executable runs for

## Time Log

- ~2 hours spent on 12/28/2022
- ~2.5 hours spent on 12/31/2022
- ~3 hours spent on 1/2/2023

total: ~7.5 hours