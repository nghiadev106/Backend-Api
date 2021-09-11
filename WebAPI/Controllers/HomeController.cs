using AutoMapper;
using Models.Models;
using Models.ViewModels;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Infrastructure.Core;
using WebAPI.Infrastructure.Extensions;
using WebAPI.Models.Product;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/client")]
    public class HomeController : ApiControllerBase
    {
        #region Initializ
        private IProductService _productService;
        private IOrderService _orderService;
        private IProductCategoryService _productCategoryService;

        public HomeController(IErrorService errorService, 
            IProductService productService, 
            IProductCategoryService productCategoryService,
            IOrderService orderService)
            : base(errorService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _orderService = orderService;
        }

        #endregion Initialize
        [Route("search")]
        [HttpGet]
        public HttpResponseMessage Search(HttpRequestMessage request, string keyword, int page, string sort, int pageSize = 20)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                var model = _productService.Search(keyword,page,pageSize,sort,out totalRow);

                totalRow = model.Count();
                var query = model.OrderByDescending(x => x.CreatedDate).Skip(page - 1 * pageSize).Take(pageSize).ToList();

                var responseData = Mapper.Map<List<Product>, List<ProductViewModel>>(query);

                var paginationSet = new PaginationSet<ProductViewModel>()
                {
                    Items = responseData,
                    PageIndex = page,
                    TotalRows = totalRow,
                    PageSize = pageSize
                };
                var response = request.CreateResponse(HttpStatusCode.OK, paginationSet);
                return response;
            });
        }

        [Route("detail/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var data = _productService.GetById(id);
                var responseData = Mapper.Map<Product, ProductViewModel>(data);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("get-shop")]
        [HttpGet]
        public HttpResponseMessage GetShop(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var data = _productService.GetAll().Take(6).ToList();
                var responseData = Mapper.Map<List<Product>, List<ProductViewModel>>(data);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("get-lastest")]
        [HttpGet]
        public HttpResponseMessage GetLastest(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var data = _productService.GetLastest(8).ToList();
                var responseData = Mapper.Map<List<Product>, List<ProductViewModel>>(data);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("get-hot-product")]
        [HttpGet]
        public HttpResponseMessage GetHotProduct(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var data = _productService.GetHotProduct(8).ToList();
                var responseData = Mapper.Map<List<Product>, List<ProductViewModel>>(data);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("get-reated-products/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetReatedProducts(HttpRequestMessage request,int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var data = _productService.GetReatedProducts(3,id).ToList();
                var responseData = Mapper.Map<List<Product>, List<ProductViewModel>>(data);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("get-by-categoryId/{categoryId:int}")]
        [HttpGet]
        public HttpResponseMessage GetProductByCategoryId(HttpRequestMessage request, int categoryId)
        {
            return CreateHttpResponse(request, () =>
            {
                var data = _productService.GetProductByCategoryId(categoryId).ToList();
                var responseData = Mapper.Map<List<Product>, List<ProductViewModel>>(data);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("get-category")]
        [HttpGet]
        public HttpResponseMessage GetCategory(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var data = _productCategoryService.GetAll().Take(10).ToList();
                var responseData = Mapper.Map<List<ProductCategory>, List<ProductCategoryViewModel>>(data);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [HttpPost]
        [Route("create-order")]
        public HttpResponseMessage Create(HttpRequestMessage request, AddOrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                try { 
                    var result = _orderService.Add(orderViewModel);
                    return request.CreateResponse(HttpStatusCode.OK, result);
                }
                catch (Exception ex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                }
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

    }
}
