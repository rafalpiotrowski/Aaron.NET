namespace Aaron.Contracts;

public sealed record PersistenceId
{
    public required string Prefix { get; init; }
    public required string EntityId { get; init; }
    public string Delimiter { get; init; } = UnderscoreDelimiter;
    
    public string Value => $"{Prefix}{Delimiter}{EntityId}";
    
    public const string UnderscoreDelimiter = "_";
}