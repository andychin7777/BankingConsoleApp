using System.Linq.Expressions;

namespace BankingService.Dal.Shared.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Dbset object</typeparam>
    /// <typeparam name="T2">key</typeparam>
    public interface IGenericRepository<T, T2> where T : class
    {
        Task<IEnumerable<T>> All(bool withTracking = true);
        Task<T?> GetById(T2 id);
        Task<bool> Add(T entity);
        Task<bool> AddRange(IEnumerable<T> entities);
        Task<bool> Delete(T2 id);
        Task<IEnumerable<T?>> Find(Expression<Func<T, bool>> predicate, bool withTracking = true);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="getId"></param>
        /// <param name="setValuesAction">action on existing values T with pass in entity T</param>
        /// <returns></returns>
        Task<T> Upsert(T entity, Func<T, T2> getId, Action<T, T>? setValuesAction = null);
    }
}
