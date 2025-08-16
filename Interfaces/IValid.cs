namespace mpc_dotnetc_user_server.Interfaces
{
    public interface IValid
    {
        bool Email(string email);
        bool Password(string password);
        bool Language_Code(string language_code);
        bool Region_Code(string region_code);
    }
}
