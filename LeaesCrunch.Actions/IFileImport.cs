namespace LeaseCrunch.Actions;

public interface IFileImport
{
    public Task Import(string filePath);
}