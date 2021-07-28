using Common.Constants;
using Data.Infrastructure;
using Data.Repositories;
using Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public interface IPostService
    {
        void Add(Post post);

        void Update(Post post);

        Post Delete(int id);

        IEnumerable<Post> GetAll();
        IEnumerable<Post> GetAll(int? categoryId, string keyword);

        IEnumerable<Post> GetAllPaging(int page, int pageSize, out int totalRow);

        IEnumerable<Post> GetAllByCategoryPaging(int categoryId, int page, int pageSize, out int totalRow);

        Post GetById(int id);

        IEnumerable<Post> GetAllByTagPaging(string tag, int page, int pageSize, out int totalRow);

        void SaveChanges();
        Tag GetTag(string tagId);
        IEnumerable<Tag> GetListPostTag(string searchText);
    }

    public class PostService : IPostService
    {
        private IPostRepository _postRepository;
        private IUnitOfWork _unitOfWork;
        private ITagRepository _tagRepository;

        public PostService(IPostRepository postRepository, IUnitOfWork unitOfWork, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
        }

        public void Add(Post post)
        {
            _postRepository.Add(post);
        }

        public Post Delete(int id)
        {
            return _postRepository.Delete(id);
        }

        public IEnumerable<Post> GetAll()
        {
            return _postRepository.GetAll(new string[] { "PostCategory" });
        }

        public IEnumerable<Post> GetAll(int? categoryId, string keyword)
        {
            var query = _postRepository.GetAll(new string[] { "PostCategory", "ProductTags" });
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));

            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryID == categoryId.Value);

            return query;
        }

        public IEnumerable<Post> GetAllByCategoryPaging(int categoryId, int page, int pageSize, out int totalRow)
        {
            return _postRepository.GetMultiPaging(x => x.Status && x.CategoryID == categoryId, out totalRow, page, pageSize, new string[] { "PostCategory" });
        }

        public IEnumerable<Post> GetAllByTagPaging(string tag, int page, int pageSize, out int totalRow)
        {
            //TODO: Select all post by tag
            return _postRepository.GetAllByTag(tag, page, pageSize, out totalRow);
        }

        public IEnumerable<Post> GetAllPaging(int page, int pageSize, out int totalRow)
        {
            return _postRepository.GetMultiPaging(x => x.Status, out totalRow, page, pageSize);
        }

        public Post GetById(int id)
        {
            return _postRepository.GetSingleById(id);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void Update(Post post)
        {
            _postRepository.Update(post);
        }

        public Tag GetTag(string tagId)
        {
            return _tagRepository.GetSingleByCondition(x => x.ID == tagId);
        }

        public IEnumerable<Tag> GetListPostTag(string searchText)
        {
            return _tagRepository.GetMulti(x => x.Type == CommonConstants.PostTag && searchText.Contains(x.Name));
        }

    }
}