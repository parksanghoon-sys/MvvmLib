using DocumentFormat.OpenXml.ExtendedProperties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Domain.Services;

namespace wpfCodeCheck.Main.Local.Servies.CheckSumService
{
    public class FileCheckSumCRC32 : IFileCheckSum
    {
        private readonly uint[] Crc32Table;

        public FileCheckSumCRC32()
        {
            Crc32Table = Enumerable.Range(0, 256).Select(i =>
            {
                uint crc = (uint)i;
                for (int j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                    {
                        crc = (crc >> 1) ^ 0xEDB88320u;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
                return crc;
            }).ToArray();
        }
        public uint ComputeChecksum(byte[] bytes)
        {            
            uint crc = 0xFFFFFFFF;
            foreach (byte b in bytes)
            {
                byte tableIndex = (byte)(((crc) & 0xFF) ^ b);
                crc = Crc32Table[tableIndex] ^ (crc >> 8);
            }
            return ~crc;
        }
        private uint ComputeChecksum(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                uint crc = 0xFFFFFFFF;
                int byteValue;
                while ((byteValue = stream.ReadByte()) != -1)
                {
                    byte tableIndex = (byte)(((crc) & 0xFF) ^ (byte)byteValue);
                    crc = Crc32Table[tableIndex] ^ (crc >> 8);
                }
                return ~crc;
            }
        }
    }
}
