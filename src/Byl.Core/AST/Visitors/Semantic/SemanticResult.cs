namespace Byl.Core.AST.Visitors.Semantic;

public class SemanticResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    private SemanticResult(bool isValid, string error = "")
    {
        IsValid = isValid;
        ErrorMessage = error;
    }

    public static SemanticResult Success() => new(true);
    public static SemanticResult Error(string error) => new(false, error);
}
