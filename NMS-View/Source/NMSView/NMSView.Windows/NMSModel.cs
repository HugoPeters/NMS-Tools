using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Graphics;

namespace NMSView
{
    public class NMSModel
    {
        public bool Indices16Bit;
        public NMSModelHeader SecHeader;
        public NMSSectionUintList SecData4;
        public NMSSectionUintList SecVtxPartsStart;
        public NMSSectionUintList SecVtxPartsEnd;
        public NMSSectionUintList SecData7;
        public NMSSectionQuatList SecData8;
        public NMSSectionQuatList SecData9;
        // Unk sec 10
        public NMSSectionStreamDescriptor VertexStreamDesc1;
        // Unk sec 12
        public NMSSectionStreamDescriptor VertexStreamDesc2;

        public List<int> Indices;
        public List<NMSVertex> Vertices1;
        public List<NMSVertex> Vertices2;

        public static NMSModel Read(string fname)
        {
            var model = new NMSModel();

            using (var fs = File.OpenRead(fname))
            {
                using (var handle = new BinaryReader(fs))
                {
                    model.Read(handle);
                }
            }

            return model;
        }

        public void Read(BinaryReader handle)
        {
            ReadHeader(handle);
            SecData4 = ReadSection<NMSSectionUintList>(4, handle);
            SecVtxPartsStart = ReadSection<NMSSectionUintList>(5, handle);
            SecVtxPartsEnd = ReadSection<NMSSectionUintList>(6, handle);
            SecData7 = ReadSection<NMSSectionUintList>(7, handle);
            SecData8 = ReadSection<NMSSectionQuatList>(8, handle);
            SecData9 = ReadSection<NMSSectionQuatList>(9, handle);

            VertexStreamDesc1 = ReadSection<NMSSectionStreamDescriptor>(11, handle);
            VertexStreamDesc2 = ReadSection<NMSSectionStreamDescriptor>(13, handle);

            ReadIndices(handle);

            Vertices1 = ReadVertices(handle, 15, VertexStreamDesc1);
            Vertices2 = ReadVertices(handle, 16, VertexStreamDesc2);
        }

        public void PrintStats()
        {
            WriteStat("Index Format", Indices16Bit ? "16bit" : "32bit");
            WriteStat("Index Count", Indices.Count);
            WriteStat("Vertex Buffer 1 count", Vertices1.Count);
            WriteStat("Vertex Stream Desc 1 num attributes", VertexStreamDesc1 == null ? "NULL" : VertexStreamDesc1.Attributes.Length.ToString());
            WriteStat("Vertex Buffer 2 count", Vertices2.Count);
            WriteStat("Vertex Stream Desc 2 num attributes", VertexStreamDesc2 == null ? "NULL" : VertexStreamDesc2.Attributes.Length.ToString());
        }
        private void WriteStat(string name, object value)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("- ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(": ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(value.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
        }


        private void ReadHeader(BinaryReader handle)
        {
            handle.BaseStream.Position = 0x68;
            Indices16Bit = handle.ReadUInt32() == 1; // ???

            handle.BaseStream.Position = 0x70;
            SecHeader = new NMSModelHeader(handle);
        }
        private List<NMSVertex> ReadVertices(BinaryReader handle, int SecId, NMSSectionStreamDescriptor StreamDesc)
        {
            var array = new List<NMSVertex>();
            var meta = SecHeader.HeaderOffsetSections[SecId];

            handle.BaseStream.Position = meta.AbsOffset;
            long maxPos = meta.AbsOffset + meta.Count;

            array = new List<NMSVertex>();
                
            while (handle.BaseStream.Position < maxPos)
            {
                array.Add(new NMSVertex(handle, StreamDesc));
            }

            return array;
        }
        private void ReadIndices(BinaryReader handle)
        {
            var meta = SecHeader.HeaderOffsetSections[14];

            handle.BaseStream.Position = meta.AbsOffset;
            uint IdxCount = meta.Count * 2; // ????

            Indices = new List<int>();

            if (Indices16Bit)
            {
                for (uint i = 0; i < IdxCount; i++)
                {
                    Indices.Add((int)handle.ReadUInt16());
                }
            }
            else
            {
                for (uint i = 0; i < IdxCount; i++)
                {
                    Indices.Add((int)handle.ReadUInt32());
                }
            }
        }

        private T ReadSection<T>(int SecId, BinaryReader handle) where T : NMSSectionBase, new()
        {
            var instance = new T();

            var Meta = SecHeader.HeaderOffsetSections[SecId];

            if (Meta.Count == 0 || Meta.Offset == 0)
                return default(T);

            instance._Meta = Meta;
            instance.Read(Meta, handle);

            return instance;
        }

        public VertexPositionNormalTexture[] ToXenkoVertices(int PartId = -1)
        {
            uint partOffset = 0, partCount = (uint)Vertices1.Count;

            if (PartId != -1)
            {
                partOffset = SecVtxPartsStart.Uints[PartId];
                partCount = SecVtxPartsEnd.Uints[PartId];
            }

            var array = new VertexPositionNormalTexture[partCount];

            for (uint i = 0; i < partCount; i++)
            {
                var data = Vertices1[(int)(partOffset + i)];

                var pos = data.GetPosition();
                var norm = data.GetNormal();
                var uv = data.GetTexCoord();

                array[i] = new VertexPositionNormalTexture(
                    pos.HasValue ? pos.Value : Vector3.Zero, 
                    norm.HasValue ? norm.Value : Vector3.Zero, 
                    uv.HasValue ? uv.Value : Vector2.Zero);
            }

            return array;
        }
    }

    public abstract class NMSSectionBase
    {
        public NMSModelHeader.SectionData _Meta; // used for debugging

        public void Read(NMSModelHeader.SectionData Meta, BinaryReader handle)
        {
            handle.BaseStream.Position = Meta.AbsOffset;
            ReadInternal(Meta, handle);
        }

        protected abstract void ReadInternal(NMSModelHeader.SectionData Meta, BinaryReader handle);
    }

    public class NMSSectionQuatList : NMSSectionBase
    {
        public Quaternion[] Quats;

        protected override void ReadInternal(NMSModelHeader.SectionData Meta, BinaryReader handle)
        {
            var array = new Quaternion[Meta.Count];

            for (uint i = 0; i < Meta.Count; i++)
            {
                array[i] = new Quaternion(handle.ReadSingle(), handle.ReadSingle(), handle.ReadSingle(), handle.ReadSingle());
            }

            Quats = array;
        }
    }

    public class NMSSectionUintList : NMSSectionBase
    {
        public uint[] Uints;

        protected override void ReadInternal(NMSModelHeader.SectionData Meta, BinaryReader handle)
        {
            var array = new uint[Meta.Count];

            for (uint i = 0; i < Meta.Count; i++)
            {
                array[i] = handle.ReadUInt32();
            }

            Uints = array;
        }
    }

    public class NMSSectionStreamDescriptor : NMSSectionBase
    {
        public struct AttributeDesc
        {
            public uint AttribId;
            public uint ElementCount;
            public uint DataType;
            public uint Offset;
            public uint Unk4;
            public uint Unk5;
            public uint Unk6;
            public uint Unk7;

            public AttributeDesc(BinaryReader handle)
            {
                AttribId = handle.ReadUInt32();
                ElementCount = handle.ReadUInt32();
                DataType = handle.ReadUInt32();
                Offset = handle.ReadUInt32();
                Unk4 = handle.ReadUInt32();
                Unk5 = handle.ReadUInt32();
                Unk6 = handle.ReadUInt32();
                Unk7 = handle.ReadUInt32();
            }
        }

        public AttributeDesc[] Attributes;

        protected override void ReadInternal(NMSModelHeader.SectionData Meta, BinaryReader handle)
        {
            Attributes = new AttributeDesc[Meta.Count];

            for (int i = 0; i < Meta.Count; i++)
            {
                Attributes[i] = new AttributeDesc(handle);
            }
        }
    }

    public class NMSModelHeader
    {
        public const uint NumHeaderSections = 17;

        public class SectionData
        {
            public uint BaseOffset;
            public uint Offset;
            public uint Unk0;
            public uint Count;
            public uint Unk2;

            public uint AbsOffset
            {
                get
                {
                    return BaseOffset + Offset;
                }
            }

            public SectionData(BinaryReader handle)
            {
                BaseOffset = (uint)handle.BaseStream.Position;
                Offset = handle.ReadUInt32();
                Unk0 = handle.ReadUInt32();
                Count = handle.ReadUInt32();
                Unk2 = handle.ReadUInt32();
            }

            public override string ToString()
            {
                return string.Format("Offset=0x{0:X}, Count=0x{1:X}", AbsOffset, Count);
            }
        }

        public SectionData[] HeaderOffsetSections;

        public NMSModelHeader(BinaryReader handle)
        {
            HeaderOffsetSections = new SectionData[NumHeaderSections];

            for (int i = 0; i < NumHeaderSections; i++)
            {
                HeaderOffsetSections[i] = new SectionData(handle);
            }
        }
    }

    public struct NMSVertex
    {
        private List<float[]> _data;

        public NMSVertex(BinaryReader handle, NMSSectionStreamDescriptor desc)
        {
            _data = new List<float[]>();

            foreach (var att in desc.Attributes)
            {
                switch (att.DataType)
                {
                    case 0x140b: _data.Add(ReadCompressedFloats(handle, 4)); break;
                    case 0x1401: _data.Add(ReadCompressedFloats(handle, 2)); break;
                }
            }
        }

        public Vector3? GetPosition()
        {
            if (_data.Count < 1)
                return null;

            return new Vector3(_data[0][0], _data[0][1], _data[0][2]);
        }

        public Vector2? GetTexCoord()
        {
            if (_data.Count < 2)
                return null;

            return new Vector2(_data[1][0], _data[1][1]);
        }

        public Vector3? GetNormal()
        {
            if (_data.Count < 3)
                return null;

            return new Vector3(_data[2][0], _data[2][1], _data[2][2]);
        }

        public static float[] ReadCompressedFloats(BinaryReader handle, int count)
        {
            float[] array = new float[count];

            for (int i = 0; i < count; i++)
            {
                array[i] = Extensions.ReadHalfLittle(handle);
            }

            return array;
        }
        public static Vector3 ReadCompressedVec3(BinaryReader handle)
        {
            return new Vector3(ReadCompressedFloats(handle, 3));
        }
        public static Vector4 ReadCompressedVec4(BinaryReader handle)
        {
            return new Vector4(ReadCompressedFloats(handle, 4));
        }
        public static Vector2 ReadCompressedVec2(BinaryReader handle)
        {
            return new Vector2(ReadCompressedFloats(handle, 2));
        }
        public static float ReadCompressedFloat(BinaryReader handle)
        {
            return Extensions.ReadHalfLittle(handle);
        }

        public override string ToString()
        {
            return string.Format("Pos={0}, UV={1}, Norm={2}", GetPosition(), GetTexCoord(), GetNormal());
        }
    }

}
