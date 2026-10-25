class Command
{
    public Func<long, int, Task> Process { get; init; }
    public Access Access { get; init; }

    public Command(Func<long, int, Task> process, Access access)
    {
        Process = process;
        Access = access;
    }
}