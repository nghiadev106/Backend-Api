using Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Infrastructure.Core;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/statistic")]
    [Authorize]
    public class StatisticsController : ApiControllerBase
    {
        private IStatisticService _statisticService;

        public StatisticsController(IErrorService errorService, IStatisticService statisticService) : base(errorService)
        {
            _statisticService = statisticService;
        }

        [Route("getrevenue")]
        [HttpGet]
        public HttpResponseMessage GetRevenueStatistic(HttpRequestMessage request, string fromDate, string toDate)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _statisticService.GetRevenueStatisticByDate(fromDate, toDate);
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, model);
                return response;
            });
        }

        [Route("getrevenue-month")]
        [HttpGet]
        public HttpResponseMessage GetRevenueStatisticByMonth(HttpRequestMessage request, string fromDate, string toDate)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _statisticService.GetRevenueStatisticByMonth(fromDate, toDate);
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, model);
                return response;
            });
        }

        [Route("getrevenue-year")]
        [HttpGet]
        public HttpResponseMessage GetRevenueStatisticByYear(HttpRequestMessage request, string fromDate, string toDate)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _statisticService.GetRevenueStatisticByYear(fromDate, toDate);
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, model);
                return response;
            });
        }
    }
}
