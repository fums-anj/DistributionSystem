using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace BH.DataAccess.Infrastructure.Interface.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T - Any model class
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        void Add(T entity);
        public T AddAndReturn(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        void AddRange(IEnumerable<T> entity);
        IEnumerable<SelectListItem> GetSelectList(Func<T, string> text, Func<T, string> value, Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
    }
}
