using mpc_dotnetc_user_server.Models.Report;

namespace mpc_dotnetc_user_server.Interfaces.IUsers_Respository
{
    public interface IUsers_Repository
    {
        Task<bool> Validate_Client_With_Server_Authorization(Report_Failed_Authorization_History dto);
    }
}
