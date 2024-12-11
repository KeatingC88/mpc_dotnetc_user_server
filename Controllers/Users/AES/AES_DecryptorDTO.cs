namespace mpc_dotnetc_user_server.Controllers
{
    public class AES_DecryptorDTO
    {
        public string Encrypted_text { get; set; } = string.Empty;
        public string Secret_key { get; set; } = string.Empty;
    }
}
