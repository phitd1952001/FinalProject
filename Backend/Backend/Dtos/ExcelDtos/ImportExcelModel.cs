namespace Backend.Dtos.ExcelDtos;

public class ImportExcelModel
{
    public IFormFile file { get; set; }
    public string mapping { get; set; }
}