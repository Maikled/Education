using Core.Interfaces;

namespace Core.Locations.GetLocations
{
    public class GetLocationsCommand : ICommand
    {
        public string? Search { get; }
        public int? MinDepartments { get; }
        public string? SortField { get; }
        public string SortDirection { get; }
        public int Page { get; }
        public int PageSize { get; }

        public GetLocationsCommand(string? search, int? minDepartments, string? sortField, string sortDirection, int page = 1, int pageSize = 20)
        {
            Search = search;
            MinDepartments = minDepartments;
            SortField = sortField;
            SortDirection = sortDirection;
            Page = page;
            PageSize = pageSize;
        }
    }
}
