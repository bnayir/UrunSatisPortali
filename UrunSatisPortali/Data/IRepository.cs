using System.Linq.Expressions;

namespace UrunSatisPortali.Data // Namespace ismine dikkat et, sende farklıysa düzelt
{
    public interface IRepository<T> where T : class
    {
        // ARTIK PARANTEZ İÇİNDE STRING PARAMETRE ALABİLİYOR
        IEnumerable<T> GetAll(string? includeProps = null);

        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}