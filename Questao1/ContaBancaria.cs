using System.Globalization;

namespace Questao1
{
    public class ContaBancaria
    {
        public ContaBancaria(int numeroConta, string titular, double saldo)
        {
            this.NumeroConta = numeroConta;
            this.Titular = titular;
            this.Saldo = saldo;
        }

        public ContaBancaria(int numeroConta, string titular) : this(numeroConta, titular, 0.0) { }

        public int NumeroConta { get; }
        public string Titular { get; set; }
        public double Saldo { get; private set; }

        public void Deposito(double quantiaDeposito)
        {
            Saldo += quantiaDeposito;
        }

        public void Saque(double quantiaSaque)
        {
            Saldo -= quantiaSaque + 3.5;
        }

        public override string ToString()
        {
            return $"Conta {NumeroConta}, Titular: {Titular}, Saldo: $ {Saldo.ToString("F2", CultureInfo.InvariantCulture)}";
        }
    }
}
