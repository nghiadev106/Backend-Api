using System.Web.Mvc;

namespace WebAPI.Controllers
{
    public class APIController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("/Help");
        }
    }
}
