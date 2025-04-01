using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Questao5.Application.Queries.Responses
{
    public class GetCheckingAccountBalanceResponse
    {
        [Required]
        [StringLength(100)]
        [Description("Checking account id")]
        public string CheckingAccountId { get; private set; }
        [Required]
        [StringLength(37)]
        [Description("Checking account responsible")]
        public string CheckingAccountResponsible { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH\\:mm\\:ss.fffZ}")]
        [Description("Response date of the balance")]
        public DateTime ResponseDate { get; set; }
        [Required]
        [Description("Account balance")]
        public double AccountBalance { get; set; }

        public GetCheckingAccountBalanceResponse(string checkingAccountId, string checkingAccountResponsible, DateTime responseDate, double accountBalance)
        {
            this.CheckingAccountId = checkingAccountId;
            this.CheckingAccountResponsible = checkingAccountResponsible;
            this.ResponseDate = responseDate;
            this.AccountBalance = accountBalance;
        }
    }
}
