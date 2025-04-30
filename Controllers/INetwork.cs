namespace mpc_dotnetc_user_server.Controllers
{
    public interface INetwork
    {
        Task<string> Get_Client_Remote_Internet_Protocol_Address();

        Task<int> Get_Client_Remote_Internet_Protocol_Port();

        Task<string> Get_Client_Internet_Protocol_Address();

        Task<int> Get_Client_Internet_Protocol_Port();
    }
}