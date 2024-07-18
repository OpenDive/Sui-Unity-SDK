using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace OpenDive.BCS
{
    public class Deserialization
    {
        readonly int MAX_U8 = (int)(Math.Pow(2, 8) - 1);
        readonly int MAX_U16 = (int)(Math.Pow(2, 16) - 1);
        readonly uint MAX_U32 = (uint)(Math.Pow(2, 32) - 1);
        readonly ulong MAX_U64 = (ulong)(Math.Pow(2, 64) - 1);
        readonly BigInteger MAX_U128 = (BigInteger)(Math.Pow(2, 128) - 1);
        readonly BigInteger MAX_U256 = (BigInteger)(Math.Pow(2, 256) - 1);

        #region Deserialization

        protected MemoryStream input;
        private long length;

        public Deserialization(byte[] data)
        {
            this.length = data.Length;
            this.input = new MemoryStream(data);
        }

        public long Remaining()
        {
            return length - input.Position;
        }

        public bool DeserializeBool()
        {
            byte[] read = Read(1);
            bool value = BitConverter.ToBoolean(read);
            return value;
        }

        public byte[] ToBytes()
        {
            return this.Read(this.DeserializeUleb128());
        }

        public byte[] FixedBytes(int length)
        {
            return this.Read(length);
        }

        public BCSMap DeserializeMap(Type keyType, Type valueType)
        {
            int length = DeserializeUleb128();
            Dictionary<BString, ISerializable> sorted_map = new Dictionary<BString, ISerializable>();

            while (sorted_map.Count < length)
            {
                BString key = DeserializeString();

                // Calls the "Deserialize" method from the give BCS type
                MethodInfo method = valueType.GetMethod("Deserialize", new Type[] { typeof(Deserialization) });
                ISerializable val = (ISerializable)method.Invoke(null, new[] { this });

                sorted_map.Add(key, val);
            }

            return new BCSMap(sorted_map);
        }

        /// <summary>
        /// Deserializes a list of objects of same type
        /// </summary>
        /// <param name="valueDecoderType"></param>
        /// <returns></returns>
        public Sequence DeserializeSequence(Type valueDecoderType)
        {
            int length = DeserializeUleb128();
            List<ISerializable> values = new List<ISerializable>();

            while (values.Count < length)
            {
                MethodInfo method = valueDecoderType.GetMethod("Deserialize", new Type[] { typeof(Deserialization) });
                ISerializable val = (ISerializable)method.Invoke(null, new[] { this });
                values.Add(val);
            }

            return new Sequence(values.ToArray());
        }

        public BString DeserializeString() => new BString(Encoding.UTF8.GetString(this.Read(this.DeserializeUleb128())));

        public ISerializable DeserializeOptional(Type valueDecoderType)
        {
            int is_null = this.PeekByte();

            if (is_null != 0)
            {
                MethodInfo method = valueDecoderType.GetMethod("Deserialize", new Type[] { typeof(Deserialization) });
                return (ISerializable)method.Invoke(null, new[] { this });
            }

            return null;
        }

        internal int PeekByte()
        {
            long originalPosition = this.input.Position;

            // Read the next byte
            int nextByte = this.input.ReadByte();

            // Restore the original position
            this.input.Position = originalPosition;

            return nextByte;
        }

        public ISerializable DeserializeStruct(Type structType)
        {
            MethodInfo method = structType.GetMethod("Deserialize", new Type[] { typeof(Deserialization) });
            ISerializable val = (ISerializable)method.Invoke(null, new[] { this });

            return val;
        }

        public U8 DeserializeU8() => new U8((byte)this.ReadInt(1)[0]);

        public U16 DeserializeU16() => new U16(BitConverter.ToUInt16(this.ReadInt(2)));

        public U32 DeserializeU32() => new U32(BitConverter.ToUInt32(this.Read(4)));

        public U64 DeserializeU64() => new U64(BitConverter.ToUInt64(this.Read(8)));

        public U128 DeserializeU128() => new U128(new BigInteger(this.Read(16)));

        public U256 DeserializeU256() => new U256(new BigInteger(this.Read(32)));

        public int DeserializeUleb128()
        {
            uint value = 0;
            int shift = 0;

            while (value <= MAX_U32)
            {
                byte byteRead = this.ReadInt(1)[0];
                value |= ((uint)byteRead & 0x7F) << shift;

                if ((byteRead & 0x80) == 0)
                    break;
                shift += 7;
            }

            if (value > MAX_U128)
            {
                throw new Exception("Unexpectedly large uleb128 value");
            }

            return (int)value;
        }

        public byte[] Read(int length)
        {
            byte[] value = new byte[length];
            int totalRead = this.input.Read(value, 0, length);

            if (totalRead < length)
                throw new Exception("Unexpected end of input. Requested: " + length + ", found: " + totalRead);

            return value;
        }

        public byte[] ReadInt(int length)
        {
            return this.Read(length);
        }

        #endregion
    }
}
