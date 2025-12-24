using System.Linq.Expressions;

namespace UrunSatisPortali.Data 
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProps = null);

        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}