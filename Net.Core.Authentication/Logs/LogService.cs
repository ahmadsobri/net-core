using Microsoft.EntityFrameworkCore;
using Net.Core.Authentication.Entities;
using Net.Core.Authentication.Entities.Contexts;
using System.Threading.Tasks;

namespace Net.Core.Authentication.Logs
{
    public class LogService : ILogService
    {
        private readonly AuthContext dbContext;
        public LogService(AuthContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task save(Log log)
        {
            dbContext.Entry(log).State = EntityState.Added;
            await dbContext.SaveChangesAsync();
        }
    }
}
