namespace PrimeWitnessesLiarsFinder;

public class Program
{
    private static readonly Dictionary<long, long> Liars = new();
    //TODO try to parallelize this
    private static void Main()
    {
        const long maxValue = 10000L;

        var n = 0L;
        var startTime = DateTime.Now;
        var timer = new Timer(StillAliveMsg, null, 10000, 10000);

        for (n = 3; n <= maxValue; n++)
        {
            FindLiars(n);
        }

        timer.Dispose();
        var elapsedTime = DateTime.Now - startTime;
        
        using var fs = new FileStream("liars.csv", FileMode.OpenOrCreate);
        using var sw = new StreamWriter(fs);
        foreach (var (key, value) in Liars.Select(kvp => (Key: kvp.Key, Value: kvp.Value)).OrderBy(v => v.Value))
        {
            sw.WriteLine($"{key},{value}");
            Console.WriteLine($"{key}: {value}");
        }
        sw.Close();
        fs.Close();
        
        Console.WriteLine($"Done in {elapsedTime}");

        void StillAliveMsg(object? state)
        {
            Console.WriteLine($"[{DateTime.Now}] Hello, I am still alive! I have processed {n}/{maxValue} numbers so far. I'll keep you updated in 5 seconds intervals.");
        }
    }
    
    private static void FindLiars(long numberToTest)
    {
        var tempLiars = new List<long>();

        var isPrime = true;
        var cachedValue = (-1L, -1L);
        for (long w = 2; w < (numberToTest >> 1); w++)
        {
            if (VerifyPrimeWithOneWitness(numberToTest, w, ref cachedValue))
            {
                if (isPrime)
                    tempLiars.Add(w);
                else AddLiar(w);
                
                continue;
            }
            
            isPrime = false;
        }

        if (isPrime) return;
        foreach (var liar in tempLiars) AddLiar(liar);

        void AddLiar(long liar)
        {
            if (Liars.ContainsKey(liar))
                Liars[liar]++;
            else
                Liars.Add(liar, 1);
        }
    }
    
    private static bool VerifyPrimeWithOneWitness(long numberToTest, long witness, ref (long, long) cachedFactoredNum)
    {
        //if (numberToTest == 2) // This function shall not be called with numberToTest = 2
        //return true;

        if (IsDivisibleByTwo(numberToTest))
            return false;

        var factoredNum = numberToTest - 1;
        var s = 0L;
        if (cachedFactoredNum == (-1, -1))
        {
            while (IsDivisibleByTwo(factoredNum))
            {
                factoredNum >>= 1;
                s++;
            }
            
            cachedFactoredNum = (factoredNum, s);
        }

        (factoredNum, s) = cachedFactoredNum;
        
        var result = ModSquareAndMultiply(witness, factoredNum, numberToTest);

        if (result == 1 || result == numberToTest - 1L) return true;
        
        for (var i = 0; i < s; i++)
        {
            result = (result * result) % numberToTest;
            
            if (result == numberToTest - 1L) return true;
        }
        
        return false;

        bool IsDivisibleByTwo(long n) => (n & 1) == 0; // this allocated 1GB of memory, so maybe its best to use % 2 instead
        //bool IsDivisibleByTwo(long n) => n % Two == 0;
    }

    private static long ModSquareAndMultiply(long baseNumber, long exponent, long modulus)
    {
        var value = baseNumber;

        var foundAOne = false;
        for (var i = 63; i >= 0; i--)
        {
            switch (foundAOne)
            {
                case false when (exponent & (1 << i)) == 1 << i:
                    foundAOne = true;
                    continue;
                case true:
                    value = (exponent & (1 << i)) == 1 << i ? SquareAndMultiply(value) : Square(value);
                    break;
            }
        }

        return value;

        long Square(long a) => (a * a) % modulus;
        long SquareAndMultiply(long a) => (Square(a) * baseNumber) % modulus; // or a * a * baseNumber % modulus not sure which one is faster though it will overflow earlier
    }
    
    /* BigInteger Version
private static BigInteger ModSquareAndMultiply(BigInteger baseNumber, BigInteger exponent, BigInteger modulus)
{
    Span<byte> bytes = stackalloc byte[exponent.GetByteCount()]; // This is for using BigInteger
    exponent.TryWriteBytes(bytes, out _);

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
*/
}