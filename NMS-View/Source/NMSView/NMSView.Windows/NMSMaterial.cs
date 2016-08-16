using SiliconStudio.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSView
{
    public class NMSMaterial
    {
        public const uint OffsMatName = 0x60;
        public const uint OffsMatType = 0xE0;
        public const uint OffsMatShader = 0x186;
        public const uint OffsMatVarTable = 0x210;

        public string MaterialName { get; set; }
        public string MaterialType { get; set; }
        public string ShaderFilename { get; set; }
        public NMSMatVarTable ShaderVars { get; set; }

        public static NMSMaterial Read(string fname)
        {
            var mat = new NMSMaterial();

            using (var fs = File.OpenRead(fname))
            {
                using (var handle = new BinaryReader(fs))
                {
                    mat.Read(handle);
                }
            }

            return mat;
        }

        public void Read(BinaryReader handle)
        {
            MaterialName = Common.ReadStringBlockAt(handle, OffsMatName, 0x20);
            MaterialType = Common.ReadStringBlockAt(handle, OffsMatType, 0x20);
            ShaderFilename = Common.ReadStringBlockAt(handle, OffsMatShader, 0x20);
            ReadVarTable(handle);
        }

        public void ReadVarTable(BinaryReader handle)
        {
            handle.BaseStream.Position = OffsMatVarTable;
            ShaderVars = new NMSMatVarTable(handle);
        }


    }

    public class NMSMatVarTable
    {
        private MetaInfo[] Header;

        public uint[] SomeUnits;
        public NMSMatVarVec4[] VarsVec4;
        public NMSMatVarMap[] VarsMap;

        public NMSMatVarTable(BinaryReader handle)
        {
            Header = new MetaInfo[3];

            for (int i = 0; i < Header.Length; i++)
            {
                Header[i] = new MetaInfo(handle);
            }

            // Most likely wrong LOL
            uint numRows = (Header[0].Unk1 >> 0x4) - 2;

            SomeUnits = new uint[numRows * 4];

            for (uint i = 0; i < SomeUnits.Length; i++)
            {
                SomeUnits[i] = handle.ReadUInt32();
            }

            VarsVec4 = ReadVarArray<NMSMatVarVec4>(handle, Header[1].Count);
            VarsMap = ReadVarArray<NMSMatVarMap>(handle, Header[2].Count);
        }

        public T[] ReadVarArray<T>(BinaryReader handle, uint count) where T : NMSMatVarBase, new()
        {
            var array = new T[count];

            for (uint i = 0; i < count; i++)
            {
                array[i] = new T();
                array[i].Read(handle);
            }

            return array;
        }

        public class MetaInfo
        {
            public uint Count;
            public uint Unk0;
            public uint Unk1;
            public uint Unk2;

            public MetaInfo(BinaryReader handle)
            {
                Count = handle.ReadUInt32();
                Unk0 = handle.ReadUInt32();
                Unk1 = handle.ReadUInt32();
                Unk2 = handle.ReadUInt32();
            }
        }
    }

    public abstract class NMSMatVarBase
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public void Read(BinaryReader handle)
        {
            Name = Common.ReadStringBlock(handle, 0x20);
            ReadInternal(handle);
        }

        protected abstract void ReadInternal(BinaryReader handle);
    }

    public class NMSMatVarVec4 : NMSMatVarBase
    {
        public Vector4 Data;

        protected override void ReadInternal(BinaryReader handle)
        {
            Data = new Vector4(handle.ReadSingle(),
                handle.ReadSingle(),
                handle.ReadSingle(),
                handle.ReadSingle());

            handle.BaseStream.Seek(0x10, SeekOrigin.Current); // todo...
        }
    }

    public class NMSMatVarMap : NMSMatVarBase
    {
        public string FileName;

        protected override void ReadInternal(BinaryReader handle)
        {
            FileName = Common.ReadStringBlock(handle, 0x80);
            handle.BaseStream.Seek(0x28, SeekOrigin.Current); // todo...
        }
    }
}
