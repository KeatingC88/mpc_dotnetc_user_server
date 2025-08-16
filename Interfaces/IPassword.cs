namespace mpc_dotnetc_user_server.Interfaces
{
    public interface IPassword
    {
        Task<byte[]> Process_Password_Salted_Hash_Bytes(byte[] original_string_as_bytes, byte[] salted_string_as_bytes);
        Task<bool> Process_Comparison_Between_Password_Salted_Hash_Bytes(byte[] array_containing_bytes_1, byte[] array_containing_bytes_2);
    }
}
