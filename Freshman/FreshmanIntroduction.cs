using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Freshman;

public class FreshmanIntroduction
{
    public static void Main()
    {
        string name = "dundun";
        Console.WriteLine($"Hello World from {name}");
    }

    public static string HowStringWorks(string str)
    {
        string temp;
        temp = str.Replace("-", "");
        temp = str.Trim();
        temp = str.TrimStart();
        bool hasCaonima = str.Contains("caonima");
        hasCaonima = str.StartsWith("caonima");
        hasCaonima = str.EndsWith("caonima");

        return temp;
    }

    public static void HowNumbersWork()
    {
        int a = 2;
        decimal b = 140;
        short c = short.MaxValue;
        float d = 1.5;
        double f = double.MinValue;

    }
    //record default: record class ~ record struct
    public record struct Point(int X, int Y)
    {
        public float Slope() => X / Y;
    }
    public static void HowTupleWorks()
    {
        var point = (X: 1, Y: 2);
        Point pt = new Point(1, 2);
        float slope = pt.Slope();
        point.X += 5;
        //copy of point1 modifying Y
        var point2 = point with { Y = 14 };
        var point3 = (A: 14, B: 12);
        point = point3;
        slope = point.X / point.Y;

    }

    
    
}
