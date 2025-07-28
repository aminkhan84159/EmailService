using Microsoft.EntityFrameworkCore;

namespace MeshComm.Core.Services
{
    public abstract class BaseDataService
    {
        private readonly DbContext _dbContext;

        public BaseDataService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
    }
}
