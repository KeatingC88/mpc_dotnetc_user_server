using mpc_dotnetc_user_server.Models.Report;

namespace mpc_dotnetc_user_server.Interfaces
{
    public interface ISystem_Tampering
    {
        Task<bool> Validate_Client_With_Server_Authorization(Report_Failed_Authorization_History dto);
    }
}
