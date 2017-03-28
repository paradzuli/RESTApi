using System.Web;
using System.Web.Mvc;
using RestWebApi.Filters;

namespace RestWebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new AuthorizedFilterAttribute());
            filters.Add(new HandleErrorAttribute());

        }
    }
}
