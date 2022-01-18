using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Interpreter
{
    public class A
    {
        public A()
        {
            var b = new B();
            b.Ints.Add(1);
            C(b);
            System.Console.WriteLine(b.Ints.Count);
        }

        private void C(B b)
        {
            b.Ints.Add(2);
        }
    }

    public class B
    {
        public List<int> Ints = new();
    }
    class Program
    {
        static void Main(string[] args)
        {
            var a = new A();
        }
    }
}