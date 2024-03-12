namespace Heatwave.Application.Models;
public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public long TotalCount { get; }

    public PaginatedList(List<T> items, long count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
        //var count = await source.CountAsync();
        //var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        //return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}

public class PaginatedInputBase
{
    /// <summary>
    /// 页大小
    /// </summary>
    public int Size { get; set; }
    /// <summary>
    /// 当前页
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// 排序字段
    /// </summary>
    public string Fields { get; set; }
    /// <summary>
    /// 排序方案(aes,desc)
    /// </summary>
    public string Orders { get; set; }
}
