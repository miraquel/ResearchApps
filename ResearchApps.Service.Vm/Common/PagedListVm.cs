using Newtonsoft.Json;

namespace ResearchApps.Service.Vm.Common;

public class PagedListVm<T>
{
    public PagedListVm(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount, int totalFilteredCount = 0)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalFilteredCount = totalFilteredCount == 0 ? totalCount : totalFilteredCount;
    }

    public IEnumerable<T> Items { get; init; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public int PageNumber { get; init; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public int PageSize { get; init; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public int TotalCount { get; init; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public int TotalFilteredCount { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public bool HasPreviousPage => PageNumber > 1 && PageNumber <= TotalPages;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public bool HasNextPage => PageNumber < TotalPages;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public List<string> Columns => typeof(T).GetProperties().Select(p => p.Name).ToList();
}