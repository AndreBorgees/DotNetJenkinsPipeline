using System.Collections.Generic;
using System.Linq;

namespace DotNetJenkinsPipeline.API.Domain.ValueObjects
{
    public sealed class Cpf
    {
        public string Valor { get; }

        private Cpf(string valor)
        {
            Valor = valor;
        }

        internal static CpfResult Criar(string cpfEntrada)
        {
            if (string.IsNullOrWhiteSpace(cpfEntrada))
                return CpfResult.Invalido("Cpf está vazio.");

            var digitosCpf = cpfEntrada.Where(char.IsDigit).Select(c => c - '0').ToList();

            if (digitosCpf.Count != 11)
                return CpfResult.Invalido("Cpf deve conter 11 dígitos.");

            if (!DigitosVerificadoresConferem(digitosCpf))
                return CpfResult.Invalido("Dígitos verificadores inválidos.");

            var cpfCompleto = string.Concat(digitosCpf);
            return CpfResult.Valido(new Cpf(cpfCompleto));
        }

        private static bool DigitosVerificadoresConferem(List<int> numerosCpf)
        {
            var primeiroDigitoVerificador = CalcularDigitosVerificadores(numerosCpf, 10);
            var segundoDigitoVerificador = CalcularDigitosVerificadores(numerosCpf.Take(9).Append(primeiroDigitoVerificador).ToList(), 11);

            return numerosCpf[9] == primeiroDigitoVerificador && numerosCpf[10] == segundoDigitoVerificador;   
        }

        private static int CalcularDigitosVerificadores(List<int> numerosCpf, int posicaoInicial)
        {
            int soma = 0;

            for (int i = 0; i < posicaoInicial - 1; i++)
            {
                soma += (posicaoInicial - i) * numerosCpf[i];
            }

            int digitoVerificador, resto = soma % 11;

            return digitoVerificador = resto < 2 ? 0 : 11 - resto;
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj is null) return false;
        //    if (ReferenceEquals(obj, this)) return true;
        //    if(obj.GetType() != GetType()) return false;
        //    var other = (Cpf)obj;
        //    return Valor == other.Valor;
        //}

        public override bool Equals(object obj) =>
            obj is Cpf other && Valor == other.Valor;

        public override int GetHashCode() => Valor != null ? Valor.GetHashCode() : 0;

        public override string ToString() => Valor;
    }

    internal record CpfResult(bool Sucesso, string Erro, Cpf Cpf)
    {
        public static CpfResult Valido(Cpf cpf) => new(true, null, cpf);
        public static CpfResult Invalido(string erro) => new(false, erro, null);
    }
}
