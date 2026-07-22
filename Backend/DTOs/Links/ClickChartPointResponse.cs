namespace Backend.DTOs.Links;

public class ClickChartPointResponse
{
    public string Date { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    public long Clicks { get; set; }
}
