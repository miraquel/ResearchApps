using System.Collections.Generic;
using ResearchApps.Domain;

namespace ResearchApps.Web.Areas.Admin.Models.Roles
{
    public class RoleIndexViewModel
    {
        public IList<AppIdentityRole> Roles { get; set; } = new List<AppIdentityRole>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
