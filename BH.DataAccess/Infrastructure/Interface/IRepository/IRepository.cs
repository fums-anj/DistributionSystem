using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
    }
}
