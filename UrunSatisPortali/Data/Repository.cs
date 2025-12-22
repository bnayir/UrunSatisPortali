using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Collections.Generic;
using System;
using System.Linq;

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
            _db.SaveChanges(); // Kayıt işlemini garantiler
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
            _db.SaveChanges(); // Silme işlemini garantiler
        }

        public IEnumerable<T> GetAll(string? includeProps = null)
        {
            IQueryable<T> query = dbSet;

            if (!string.IsNullOrEmpty(includeProps))
            {
                foreach (var includeProp in includeProps.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.ToList();
        }

        public T GetById(int id)
        {
            return dbSet.Find(id);
        }

        // --- DÜZELTİLEN GÜNCELLEME METODU ---
        public void Update(T entity)
        {
            // Değişken isimleri sınıfın üst kısmındaki tanımlarla eşitlendi
            dbSet.Update(entity);
            _db.SaveChanges(); // Veritabanına fiziksel kaydı yapan kritik satır
        }
    }
}