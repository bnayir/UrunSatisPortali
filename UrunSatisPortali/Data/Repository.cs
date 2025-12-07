using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UrunSatisPortali.Data; // Kendi namespace'ini kontrol et

namespace UrunSatisPortali.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
            _db.SaveChanges();
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
            _db.SaveChanges();
        }

        // --- GÜNCELLENEN KISIM BURASI ---
        public IEnumerable<T> GetAll(string? includeProps = null)
        {
            IQueryable<T> query = dbSet;

            // Eğer "Category" gibi bir istek geldiyse, onu sorguya dahil et
            if (!string.IsNullOrEmpty(includeProps))
            {
                foreach (var includeProp in includeProps.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.ToList();
        }
        // --------------------------------

        public T GetById(int id)
        {
            return dbSet.Find(id);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
            _db.SaveChanges();
        }
    }
}