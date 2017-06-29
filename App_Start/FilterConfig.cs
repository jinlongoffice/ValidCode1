//using EnterpriseBackSystem.App_Start;
using System.Web;
using System.Web.Mvc;

namespace EnterpriseBackSystem
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new ErrorAllAttribute());
            //filters.Add(new UseSituationAttribute());
        }
    }
}