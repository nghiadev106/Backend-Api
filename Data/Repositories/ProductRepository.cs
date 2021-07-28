using Data.Infrastructure;
using Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetListProductByTag(string tagId, int page, int pageSize, out int totalRow);
        IEnumerable<Product> GetListProduct(string keyword);
    }

    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<Product> GetListProductByTag(string tagId, int page, int pageSize, out int totalRow)
        {
            var query = from p in DbContext.Products
                        join pt in DbContext.ProductTags
                        on p.ID equals pt.ProductID
                        where pt.TagID == tagId
                        select p;
            totalRow = query.Count();

            return query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IEnumerable<Product> GetListProduct(string keyword)
        {
            var query = from p in DbContext.Products
                        select p;
            if (!string.IsNullOrEmpty(keyword))
            {
                query.Where(x => x.Name.Contains(keyword)).ToList();
            }
            return query;
        }
    }
}