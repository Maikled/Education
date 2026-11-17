using Core.Interfaces;

namespace Core.Departments.GetDepartments
{
    public class GetDepartmentsCommand : ICommand
    {
        public string? Search { get; }
        public string? SortField { get; }
        public string SortDirection { get; }
        public int Page { get; }
        public int PageSize { get; }

        public GetDepartmentsCommand(string? search, string? sortField, string sortDirection, int page = 1, int pageSize = 20)
        {
            Search = search;
            SortField = sortField;
            SortDirection = sortDirection;
            Page = page;
            PageSize = pageSize;
        }
    }
}
