using Data.Infrastructure;
using Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetListProductByTag(string tagId, int page, int pageSize, out int totalRow);
        IEnumerable<Product> Search(string keyword);
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

        public IEnumerable<Product> Search(string keyword)
        {
            string queryString = string.Format("SELECT * FROM Products WHERE dbo.fuConvertToUnsign(Name) LIKE N'%' + dbo.fuConvertToUnsign(N'{0}') + '%'", keyword);
            var query = DbContext.Products.SqlQuery(queryString).ToList();         
            return query;
        }
    }
}