using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSView
{
    public static class Common
    {
        public static string ReadStringBlock(BinaryReader handle, int BlockSize)
        {
            var data = handle.ReadBytes(BlockSize);
            return Encoding.ASCII.GetString(data).TrimEnd((char)0);
        }
        public static string ReadStringBlockAt(BinaryReader handle, uint offset, int BlockSize)
        {
            handle.BaseStream.Position = offset;
            return ReadStringBlock(handle, BlockSize);
        }
    }
}
