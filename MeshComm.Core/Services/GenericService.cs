using FluentValidation;
using MeshComm.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MeshComm.Core.Services
{
    public abstract class GenericService<T, U> : BaseDataService, IGenericService<T> where T : class where U : AbstractValidator<T>
    {
        private readonly DbContext _dbContext;
        private readonly U _entityValidator;

        protected GenericService(
            DbContext dbContext,
            U entityValidator) : base(dbContext)
        {
            _dbContext = dbContext;
            _entityValidator = entityValidator;
        }

        public async Task<bool> AddAsync(T entity)
        {
            if (entity is not null)
            {
                await _entityValidator.ValidateAndThrowAsync(entity);
                await _dbContext.Set<T>().AddAsync(entity);

                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                    return true;
                else
                    return false;
            }

            return false;
        }

        public async Task<bool> DeleteAsync(int entityId)
        {
            var entity = await _dbContext.Set<T>().FindAsync(entityId);

            if (entity is not null)
            {
                _dbContext.Set<T>().Remove(entity);

                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                    return true;
                else
                    return false;
            }

            return false;
        }

        public IQueryable<T> GetAll()
        {
            var entities = _dbContext.Set<T>().AsQueryable();

            return entities;
        }

        public async Task<T?> GetByIdAsync(Guid entityId)
        {
            var entity = await _dbContext.Set<T>().FindAsync(entityId);

            if (entity is not null)
            {
                return entity;
            }

            return null;
        }

        public async Task<T?> GetByIdAsync(int entityId)
        {
            if (entityId > 0)
            {
                var entity = await _dbContext.Set<T>().FindAsync(entityId);

                if (entity is not null)
                {
                    return entity;
                }
            }

            return null;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity is not null)
            {
                Type entityType = entity.GetType();
                PropertyInfo? propertyInfo = entityType.GetProperty($"{entityType.Name}Id");

                if (propertyInfo is null)
                    propertyInfo = entityType.GetProperty($"Id");

                if (propertyInfo is not null)
                {
                    object entityId = propertyInfo.GetValue(entity)!;

                    var existingEntity = await _dbContext.Set<T>().FindAsync(entityId);

                    if (existingEntity is not null)
                    {
                        await _entityValidator.ValidateAndThrowAsync(entity);
                        _dbContext.Set<T>().Update(entity);

                        var result = await _dbContext.SaveChangesAsync();

                        if (result > 0)
                            return true;
                        else
                            return false;
                    }
                }
            }

            return false;
        }
    }
}
