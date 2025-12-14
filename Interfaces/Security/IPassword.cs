namespace mpc_dotnetc_user_server.Interfaces
{
    public interface IPassword
    {
        byte[] Create_Password_Salted_Hash_Bytes(byte[] original_string_as_bytes, byte[] salted_string_as_bytes);
        bool Compare_Password_Byte_Arrays(byte[] array_containing_bytes_1, byte[] array_containing_bytes_2);
    }
}
