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
    private int index = 1;

    /// <summary>
    /// 页大小
    /// </summary>
    public int Size { get; set; } = 10;
    /// <summary>
    /// 当前页
    /// </summary>
    public int Index
    {
        get => index;
        set
        {
            index = value;
            if (index < 1)
                index = 1;
        }
    }
    /// <summary>
    /// 排序字段
    /// </summary>
    public string Fields { get; set; } = "id";
    /// <summary>
    /// 排序方案(aes,desc)
    /// </summary>
    public string Orders { get; set; } = "desc";
}
