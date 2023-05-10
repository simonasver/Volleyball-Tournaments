namespace Backend.Data.Entities.Utils;

public class SearchParameters
{
    private readonly int _pageSize;
    private const int MaxPageSize = 50;
    
    public int PageNumber { get; init; } = 1;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
    
    public string? SearchInput { get; init; } = "";
}