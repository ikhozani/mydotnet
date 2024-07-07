namespace FileUploadSvc.model;

public class PagedResponse<T>
{
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public List<T> Items { get; set; }
}
