using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Interface.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BH.DataAccess.Infrastructure.Concrete.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly BHContext _db;
        internal DbSet<T> dbSet;

        public Repository(BHContext db)
        {
            _db = db;
            //_db.ShoppingCarts.Include(u => u.Product).Include(u=>u.CoverType);
            dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }
        public T AddAndReturn(T entity)
        {
            return dbSet.Add(entity).Entity;
        }
        //includeProp - "Category,CoverType"
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public IEnumerable<SelectListItem> GetSelectList(Func<T, string> text, Func<T, string> value, Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            var items = GetAll(filter, includeProperties);
            return items.Select(i => new SelectListItem { Text = text(i), Value = value(i) });
        }

        //public IEnumerable<SelectListItem> GetSelectList<T>(IEnumerable<T> items, Func<T, string> text, Func<T, string> value) =>
        //   items.Select(i => new SelectListItem { Text = text(i), Value = value(i) });

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
        {
            if (tracked)
            {
                IQueryable<T> query = dbSet;

                query = query.Where(filter);
                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return query.FirstOrDefault();
            }
            else
            {
                IQueryable<T> query = dbSet.AsNoTracking();

                query = query.Where(filter);
                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return query.FirstOrDefault();
            }

        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
        public void AddRange(IEnumerable<T> entity)
        {
            dbSet.AddRange(entity);
        }
    }
}
