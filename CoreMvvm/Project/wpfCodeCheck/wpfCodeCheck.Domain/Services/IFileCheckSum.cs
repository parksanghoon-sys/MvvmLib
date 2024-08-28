namespace wpfCodeCheck.Domain.Services
{
    public interface IFileCheckSum
    {
        uint ComputeChecksum(byte[] input);
    }
}
