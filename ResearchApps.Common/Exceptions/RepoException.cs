namespace ResearchApps.Common.Exceptions;

public class RepoException : Exception
{
    // create a custom exception for repository errors without input parameters
    public RepoException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class RepoException<T> : RepoException
{
    // create a custom exception for repository errors with capability to capture the input parameters
    public RepoException(string message, T? parameters, Exception? innerException = null)
        : base($"{message} Input Parameters: {parameters}", innerException)
    {
    }
}