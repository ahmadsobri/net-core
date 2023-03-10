using Net.Core.Authentication.Entities;
using System.Threading.Tasks;

namespace Net.Core.Authentication.Logs
{
    interface ILogService
    {
        Task save(Log log);
    }
}
