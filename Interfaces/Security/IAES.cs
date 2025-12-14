namespace mpc_dotnetc_user_server.Interfaces
{
    public interface IAES
    {
        string Process_Decryption(string decrypt_me);
        string Process_Encryption(string encrypt_me);
    }
}