using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace FCA.Data.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets all <see cref="TEntity"/>.
        /// </summary>
        /// <returns>IQueryable of <see cref="TEntity"/>.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Finds by Predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>IQueryable of <see cref="TEntity"/> which satisfies the predicate.</returns>
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Adds <see cref="TEntity"/>.
        /// </summary>
        /// <param name="entity">An instance of <see cref="TEntity"/>.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Deletes <see cref="TEntity"/>.
        /// </summary>
        /// <param name="entity">An instance of <see cref="TEntity"/>.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Modifies <see cref="TEntity"/>.
        /// </summary>
        /// <param name="entity">An instance of <see cref="TEntity"/> to modify.</param>
        void Edit(TEntity entity);

        /// <summary>
        /// SaveChanges made on <see cref="FCA.Data.DbContext"/>.
        /// </summary>
        void Save();
    }

    /// <summary>
    /// Abstract representation of CRUD Repository.
    /// </summary>
    /// <typeparam name="Ctx">DataContext.</typeparam>
    /// <typeparam name="TEntity">Entity class.</typeparam>
    public abstract class GenericRepository<Ctx, TEntity> :
        IGenericRepository<TEntity>
        where TEntity : class
        where Ctx : Microsoft.EntityFrameworkCore.DbContext, new()
    {
        private Ctx _entities = new Ctx();

        #region Constructor

        protected GenericRepository(Ctx context)
        {
            _entities = context;
        }

        #endregion

        #region Class Methods

        // TODO: Check the limit
        public IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = _entities.Set<TEntity>().AsNoTracking();
            return query;
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = _entities.Set<TEntity>().Where(predicate);
            return query;
        }

        public virtual void Add(TEntity entity)
        {
            _entities.Set<TEntity>().Add(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _entities.Set<TEntity>().Remove(entity);
        }

        public virtual void Edit(TEntity entity)
        {
            _entities.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Save()
        {
            _entities.SaveChanges();
        }
        #endregion
    }
}
