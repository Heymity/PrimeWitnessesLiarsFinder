// See https://aka.ms/new-console-template for more information

using System.Numerics;

public class Program
{

    private static void Main()
    {
        Console.WriteLine(VerifyPrimeWithOneWitness(91, 11));
        Console.WriteLine(ModSquareAndMultiply(23, 373, 747));
        Console.WriteLine(ModSquareAndMultiply(3, 45, 7));
    }
    
    private static readonly BigInteger two = new(2); // Avoid unnecessary BigInteger creations
    private static bool VerifyPrimeWithOneWitness(BigInteger numberToTest, BigInteger witness)
    {
        if (numberToTest == two)
            return true;
    
        if (IsDivisibleByTwo(numberToTest))
            return false;

        var factoredNum = numberToTest - 1;
        var s = 0;
        while (IsDivisibleByTwo(factoredNum))
        {
            factoredNum >>= 1;
            s++;
        }

        Console.WriteLine(factoredNum);
        var result = ModSquareAndMultiply(witness, factoredNum, numberToTest);

        if (result == 1 || result == numberToTest - BigInteger.One) return true;
        
        for (var i = 0; i < s; i++)
        {
            result = (result * result) % numberToTest;
            
            if (result == numberToTest - BigInteger.One) return true;
        }
        
        return false;

        bool IsDivisibleByTwo(BigInteger n) => (n.ToByteArray()[0] & 1) == 0;
    }

    private static BigInteger ModSquareAndMultiply(BigInteger baseNumber, BigInteger exponent, BigInteger modulus)
    {
        var bytes = exponent.ToByteArray();

        var value = baseNumber;
        
        for (var byteIndex = bytes.Length - 1; byteIndex >= 0; byteIndex--)
        {
            var byteLenght = 7;
            var exponentByte = bytes[byteIndex];
            if (byteIndex == bytes.Length - 1)
            {
                int i; 
                for (i = byteLenght; i >= 0; i--)
                {
                    if ((exponentByte & (1 << i)) == 1 << i) break;
                }

                byteLenght = i - 1;
                if (byteLenght < 0) continue;
            }
            for (var i = byteLenght; i >= 0; i--)
            {
                //Console.Write($"[{i}] bLenght: {byteLenght} byte: {exponentByte}, bit {((exponentByte & (1 << i)) == (1 << i) ? "1" : "0")} ");
                //Console.WriteLine((exponentByte & (1 << i)) == 1 << i ? "SM" : "S");
                value = (exponentByte & (1 << i)) == 1 << i ? SquareAndMultiply(value) : Square(value);
            }
        }

        return value;

        BigInteger Square(BigInteger a) => (a * a) % modulus;
        BigInteger SquareAndMultiply(BigInteger a) => (Square(a) * baseNumber) % modulus; // or a * a * baseNumber % modulus not sure which one is faster
    }
}
