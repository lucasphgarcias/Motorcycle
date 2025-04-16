using Motorcycle.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Motorcycle.Domain.ValueObjects;

public class Cnpj : IEquatable<Cnpj>
{
    public string Value { get; }

    private Cnpj(string value)
    {
        Value = value;
    }

    public static Cnpj Create(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new DomainException("O CNPJ não pode ser vazio.");

        // Remove caracteres não numéricos
        var numericCnpj = Regex.Replace(cnpj, @"[^\d]", "");

        if (!IsValidCnpj(numericCnpj))
            throw new DomainException("CNPJ inválido.");

        // Formata o CNPJ para o padrão XX.XXX.XXX/XXXX-XX
        var formattedCnpj = FormatCnpj(numericCnpj);

        return new Cnpj(formattedCnpj);
    }

    private static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Length != 14)
            return false;

        // Verifica se todos os dígitos são iguais
        if (new string(cnpj[0], 14) == cnpj)
            return false;

        // Cálculo do primeiro dígito verificador
        int sum = 0;
        int[] weight1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        for (int i = 0; i < 12; i++)
            sum += int.Parse(cnpj[i].ToString()) * weight1[i];

        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        // Cálculo do segundo dígito verificador
        sum = 0;
        int[] weight2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        for (int i = 0; i < 12; i++)
            sum += int.Parse(cnpj[i].ToString()) * weight2[i];

        sum += digit1 * weight2[12];
        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cnpj[12].ToString()) == digit1 && int.Parse(cnpj[13].ToString()) == digit2;
    }

    private static string FormatCnpj(string cnpj)
    {
        return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
    }

    public bool Equals(Cnpj? other)
    {
        if (other is null)
            return false;

        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Cnpj other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static bool operator ==(Cnpj? left, Cnpj? right)
    {
        if (left is null && right is null)
            return true;
        
        if (left is null || right is null)
            return false;
        
        return left.Equals(right);
    }

    public static bool operator !=(Cnpj? left, Cnpj? right)
    {
        return !(left == right);
    }
}