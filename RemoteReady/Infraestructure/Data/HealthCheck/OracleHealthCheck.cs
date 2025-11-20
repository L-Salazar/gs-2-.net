using RemoteReady.Data.AppData;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace RemoteReady.Infraestructure.Data.HealthCheck
{
    public class OracleHealthCheck : IHealthCheck
    {
        private readonly ApplicationContext _context;

        public OracleHealthCheck(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var userExists = await _context.Usuarios
                                    .Where(u => u.Id == 1) 
                                    .FirstOrDefaultAsync(cancellationToken);

                return userExists != null
                    ? HealthCheckResult.Healthy("Banco de dados Respondeu!!")
                    : HealthCheckResult.Unhealthy("Banco de dados não Respondeu!!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro" + e.ToString());
                return HealthCheckResult.Unhealthy("Banco de dados nao Respondeu!!");
            }
        }
    }
}
