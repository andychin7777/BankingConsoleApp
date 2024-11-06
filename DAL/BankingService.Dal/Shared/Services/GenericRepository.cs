using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BankingService.Dal.Shared.Interfaces;

namespace BankingService.Dal.Shared.Services
{
    /// <summary>
    /// generic repository to be inherited
    /// </summary>
    /// <typeparam name="T">Database class object</typeparam>
    /// <typeparam name="T2">key object</typeparam>
    /// <typeparam name="T3">repository type</typeparam>
    public abstract class GenericRepository<T, T2, T3> : IGenericRepository<T, T2> where T : class 
        where T3 : class
    {
        internal readonly DbContext _context;
        internal readonly DbSet<T> _dbSet;

        protected GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> All(bool withTracking = true)
        {
            return withTracking ? await _dbSet.ToListAsync() : await _dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<T?> GetById(T2 id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            return true;
        }

        public async Task<bool> AddRange(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return true;
        }

        public async Task<bool> Delete(T2 id)
        {
            //find by id
            var entity = await GetById(id);

            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            return true;
        }

        public virtual async Task<IEnumerable<T?>> Find(Expression<Func<T, bool>> predicate, bool withTracking = true)
        {
            return withTracking
                ? await _dbSet.Where(predicate).ToListAsync()
                : await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="getId"></param>
        /// <param name="setValuesAction">action to apply to existingEntity with new Entity</param>
        /// <returns></returns>
        public virtual async Task<T> Upsert(T entity, Func<T, T2> getId, Action<T, T>? setValuesAction = null)
        {
            var existingEntity = await GetById(getId(entity));

            if (existingEntity != null)
            {
                var entry = _context.Entry(existingEntity);
                if (setValuesAction == null)
                {
                    entry.CurrentValues.SetValues(entity);
                }
                else
                {
                    setValuesAction(existingEntity, entity);
                }
            }
            else
            {
                await Add(entity);
            }

            return entity;
        }
    }
}
