
namespace OpenDive.BCS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Numerics;
    using System.Reflection;
    public class Deserialization
    {
        int MAX_U8 = (int)(Math.Pow(2, 8) - 1);
        int MAX_U16 = (int)(Math.Pow(2, 16) - 1);
        uint MAX_U32 = (uint)(Math.Pow(2, 32) - 1);
        UInt64 MAX_U64 = (UInt64)(Math.Pow(2, 64) - 1);
        BigInteger MAX_U128 = (BigInteger)(Math.Pow(2, 128) - 1); // System.UInt128
        BigInteger MAX_U256 = (BigInteger)(Math.Pow(2, 256) - 1);

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
            SortedDictionary<string, ISerializable> sortedMap = new SortedDictionary<string, ISerializable>();

            while (sortedMap.Count < length)
            {
                string key = DeserializeString();

                // Calls the "Deserialize" method from the give BCS type
                MethodInfo method = valueType.GetMethod("Deserialize", new Type[] { typeof(Deserialization) });
                ISerializable val = (ISerializable)method.Invoke(null, new[] { this });

                sortedMap.Add(key, val);
            }

            Dictionary<BString, ISerializable> value = new Dictionary<BString, ISerializable>();
            foreach (KeyValuePair<string, ISerializable> entry in sortedMap)
            {
                value.Add(new BString(entry.Key), entry.Value);
            }

            return new BCSMap(value);
        }

        /// <summary>
        /// Deserializes a list of objects of same type
        /// </summary>
        /// <param name="valueDecoderType"></param>
        /// <returns></returns>
        public ISerializable[] DeserializeSequence(Type valueDecoderType)
        {
            int length = DeserializeUleb128();
            List<ISerializable> values = new List<ISerializable>();

            while (values.Count < length)
            {
                MethodInfo method = valueDecoderType.GetMethod("Deserialize", new Type[] { typeof(Deserialization) });
                ISerializable val = (ISerializable)method.Invoke(null, new[] { this });
                values.Add(val);
            }

            return values.ToArray();
        }

        public TagSequence DeserializeTagSequence()
        {
            int length = DeserializeUleb128();
            List<ISerializableTag> values = new List<ISerializableTag>();

            while (values.Count < length)
            {
                ISerializableTag val = ISerializableTag.DeserializeTag(this);
                values.Add(val);
            }

            return new TagSequence(values.ToArray());
        }

        // TODO: CLEAN THIS
        //public Sequence DeserializeScriptArgSequence()
        //{
        //    int length = DeserializeUleb128();
        //    List<ScriptArgument> values = new List<ScriptArgument>();

        //    while (values.Count < length)
        //    {
        //        ScriptArgument val = ScriptArgument.Deserialize(this);
        //        values.Add(val);
        //    }

        //    return new Sequence(values.ToArray());
        //}

        public TagSequence DeserializeArgSequence(Type valueDecoderType)
        {
            int length = DeserializeUleb128();
            List<ISerializableTag> values = new List<ISerializableTag>();

            while (values.Count < length)
            {
                MethodInfo method = valueDecoderType.GetMethod("Deserialize", new Type[] { typeof(Deserialization) });
                ISerializableTag val = (ISerializableTag)method.Invoke(null, new[] { this });
                values.Add(val);
            }

            return new TagSequence(values.ToArray());
        }

        public string DeserializeString()
        {
            return BString.Deserialize(this.Read(this.DeserializeUleb128()));
        }

        //public ISerializable Struct(ISerializable structObj )
        public ISerializable Struct(Type structType)
        {
            //Type structType = structObj.GetType();
            MethodInfo method = structType.GetMethod("Deserialize", new Type[] { typeof(Deserialization) });
            ISerializableTag val = (ISerializableTag)method.Invoke(null, new[] { this });

            return val;
        }

        public byte DeserializeU8()
        {
            return (byte)this.ReadInt(1);
        }

        public ushort DeserializeU16()
        {
            return BCS.U16.Deserialize(this.Read(2));
        }

        public uint DeserializeU32()
        {
            return BCS.U32.Deserialize(this.Read(4));
        }

        public ulong DeserializeU64()
        {
            return BCS.U64.Deserialize(this.Read(8));
        }

        public BigInteger DeserializeU128()
        {
            return BCS.U128.Deserialize(this.Read(16));
        }

        public BigInteger DeserializeU256()
        {
            return BCS.U256.Deserialize(this.Read(32));
        }

        public int DeserializeUleb128()
        {
            int value = 0;
            int shift = 0;

            while (value <= MAX_U32)
            {
                byte byteRead = this.ReadInt(1);
                value |= (byteRead & 0x7F) << shift;

                if ((byteRead & 0x80) == 0)
                    break;
                shift += 7;
            }

            if (value > MAX_U128)
            {
                throw new Exception("Unexpectedly large uleb128 value");
            }

            return value;
        }

        public byte[] Read(int length)
        {
            byte[] value = new byte[length];
            int totalRead = this.input.Read(value, 0, length);

            if (totalRead == 0 || totalRead < length)
            {
                throw new Exception("Unexpected end of input. Requested: " + length + ", found: " + totalRead);
            }
            return value;
        }

        public byte ReadInt(int length)
        {
            return this.Read(length)[0];
        }

        #endregion
    }
}
