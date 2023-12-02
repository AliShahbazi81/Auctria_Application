namespace AuctriaApplication.Services.Store.Dto;

public record ProductFilterDto
{
    public string? ProductName = null;
    public string? CategoryName = null;
    public double MinPrice = 0;
    public double MaxPrice = 0;
    public int PageNumber = 1;
    public int PageSize = 20;
    public bool IsDeleted = false;
}