using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Questao5.Domain.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Description("Transaction type")]
    public enum TransactionType
    {
        [EnumMember(Value = "C")]
        [Description("Credit")]
        Credit,
        [EnumMember(Value = "D")]
        [Description("Debit")]
        Debit
    }
}
