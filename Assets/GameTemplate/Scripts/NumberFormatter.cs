using System.Collections.Generic;
using System.Text;

public static class NumberFormatter
{
    // Birim kısaltmaları listesi
    private static readonly List<string> Units = new List<string> { "", "K", "M", "B", "A", "B", "C", "D" };
    // Hariç tutulacak karakterler
    private static readonly HashSet<char> ExcludedChars = new HashSet<char> { 'x', 'y', 'z' };

    // Sayı formatlama methodu
    public static string Convert(object number)
    {
        double adjustedNumber;

        if (number is float)
        {
            adjustedNumber = System.Math.Round((float)number);
        }
        else if (number is double)
        {
            adjustedNumber = (double)number;
        }
        else if (number is int)
        {
            adjustedNumber = (int)number;
        }
        else if (number is long)
        {
            adjustedNumber = (long)number;
        }
        else
        {
            throw new System.ArgumentException("Unsupported number type");
        }

        int unitIndex = 0;

        // Birim kısaltmaları ile formatlama
        while (adjustedNumber >= 1000 && unitIndex < Units.Count - 1)
        {
            adjustedNumber /= 1000;
            unitIndex++;
        }

        // Büyük birim kısaltmaları ile formatlama
        if (unitIndex == Units.Count - 1)
        {
            return $"{adjustedNumber:0.#}{GetLargeUnit(adjustedNumber)}";
        }
        else
        {
            return $"{adjustedNumber:0.#}{Units[unitIndex]}";
        }
    }

    // Büyük birim kısaltmaları üretme methodu
    private static string GetLargeUnit(double number)
    {
        int largeUnitIndex = 0;
        while (number >= 1000)
        {
            number /= 1000;
            largeUnitIndex++;
        }

        return GenerateLargeUnit(largeUnitIndex);
    }

    // Prosedürel büyük birim kısaltmaları üretme methodu
    private static string GenerateLargeUnit(int index)
    {
        StringBuilder sb = new StringBuilder();
        int alphabetLength = 26;

        while (index >= 0)
        {
            char nextChar = (char)('a' + (index % alphabetLength));
            if (ExcludedChars.Contains(nextChar) || Units.Contains(nextChar.ToString()))
            {
                index++;
                continue;
            }
            sb.Insert(0, nextChar);
            index = index / alphabetLength - 1;
        }

        return sb.ToString();
    }
}