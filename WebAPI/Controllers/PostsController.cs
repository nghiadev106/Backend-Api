using AutoMapper;
using Models.Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using WebAPI.Infrastructure.Core;
using WebAPI.Infrastructure.Extensions;
using WebAPI.Models.Common;
using WebAPI.Models.Post;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/post")]
    [Authorize]
    public class PostsController : ApiControllerBase
    {
        #region Initialize

        private IPostService _postService;

        public PostsController(IErrorService errorService, IPostService postService)
            : base(errorService)
        {
            this._postService = postService;
        }

        #endregion Initialize

        [Route("getallparents")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            Func<HttpResponseMessage> func = () =>
            {
                var model = _postService.GetAll();

                var responseData = Mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            };
            return CreateHttpResponse(request, func);
        }
        [Route("gettags")]
        [HttpGet]
        public HttpResponseMessage GetTags(HttpRequestMessage request, string text)
        {
            Func<HttpResponseMessage> func = () =>
            {
                var model = _postService.GetListPostTag(text);

                var responseData = Mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            };
            return CreateHttpResponse(request, func);
        }
        [Route("detail/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _postService.GetById(id);

                var responseData = Mapper.Map<Post, PostViewModel>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);

                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, int? categoryId, string keyword, int page, int pageSize = 20)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                var model = _postService.GetAll(categoryId, keyword);

                totalRow = model.Count();
                var query = model.OrderByDescending(x => x.CreatedDate).Skip(page - 1 * pageSize).Take(pageSize).ToList();

                var responseData = Mapper.Map<List<Post>, List<PostViewModel>>(query);

                var paginationSet = new PaginationSet<PostViewModel>()
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

        [Route("add")]
        [HttpPost]
        public HttpResponseMessage Create(HttpRequestMessage request, PostViewModel postCategoryVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var newPost = new Post();
                    newPost.UpdatePost(postCategoryVm);
                    newPost.CreatedDate = DateTime.Now;
                    newPost.CreatedBy = User.Identity.Name;
                    _postService.Add(newPost);
                    _postService.SaveChanges();

                    var responseData = Mapper.Map<Post, PostViewModel>(newPost);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        public HttpResponseMessage Update(HttpRequestMessage request, PostViewModel postVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var dbPost = _postService.GetById(postVm.ID);

                    dbPost.UpdatePost(postVm);
                    dbPost.UpdatedDate = DateTime.Now;
                    dbPost.UpdatedBy = User.Identity.Name;
                    _postService.Update(dbPost);
                    _postService.SaveChanges();

                    var responseData = Mapper.Map<Post, PostViewModel>(dbPost);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("delete")]
        [HttpDelete]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var oldPostCategory = _postService.Delete(id);
                    _postService.SaveChanges();

                    var responseData = Mapper.Map<Post, PostViewModel>(oldPostCategory);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string checkedPosts)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var listPostCategory = new JavaScriptSerializer().Deserialize<List<int>>(checkedPosts);
                    foreach (var item in listPostCategory)
                    {
                        _postService.Delete(item);
                    }

                    _postService.SaveChanges();

                    response = request.CreateResponse(HttpStatusCode.OK, listPostCategory.Count);
                }

                return response;
            });
        }

        
    }
}
