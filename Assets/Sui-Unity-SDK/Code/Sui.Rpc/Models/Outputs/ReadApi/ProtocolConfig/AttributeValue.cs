using System.Numerics;
using Newtonsoft.Json;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    [JsonConverter(typeof(AttributeValueConverter))]
    public class AttributeValue : ReturnBase
    {
        public AttributeValueType Type { get; internal set; }

        public IAttributeValue Attribute { get; internal set; }

        internal AttributeValue
        (
            AttributeValueType type,
            IAttributeValue attribute
        )
        {
            this.Type = type;
            this.Attribute = attribute;
        }

        internal AttributeValue(SuiError error)
        {
            this.Error = error;
        }

        public BigInteger? GetValue()
        {
            return this.Type switch
            {
                AttributeValueType.F64 => new BigInteger(((U64AttributeValue)this.Attribute).Value),
                AttributeValueType.U32 => new BigInteger(((U64AttributeValue)this.Attribute).Value),
                AttributeValueType.U64 => new BigInteger(((U64AttributeValue)this.Attribute).Value),
                AttributeValueType.U16 => new BigInteger(((U64AttributeValue)this.Attribute).Value),
                _ => this.SetError<BigInteger?, SuiError>(null, "Unable to convert to a BigInteger")
            };
        }
    }
}