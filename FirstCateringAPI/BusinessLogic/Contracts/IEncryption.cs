namespace FirstCateringAPI.BusinessLogic.Contracts
{
    public interface IEncryption
    {
        string EncryptString(string text, string keyString);

        string DecryptString(string cipherText, string keyString);
    }
}