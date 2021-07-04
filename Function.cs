using System;
using System.Collections.Generic;

namespace Interpretator
{
    public interface IFunction
    {
        public int Apply(List<int> args);
    }

    public class Min : IFunction
    {
        public int Apply(List<int> args)
        {
            if (args.Count == 0)         
                throw new Exception("No arguments for function min");
           
            int min = args[0];

            foreach (int val in args)          
                if (val < min)              
                    min = val;
                        
            return min;
        }
    }

    public class Rand : IFunction
    {
        private const int RANDOMIZER = 256;
        private readonly Random random;

        public int Apply(List<int> args)
        {
            if (args.Count != 0)          
                throw new Exception("Wrong argument count for function rand");         

            return (int)(random.NextDouble() * RANDOMIZER);
        }

        public Rand()
        {
            random = new Random();
        }
    }

    public class Pow : IFunction
    {
        public int Apply(List<int> args)
        {
            if (args.Count != 2)          
                throw new Exception("Wrong argument count for function pow: " + args.Count);
           
            return (int)Math.Pow(args[0], args[1]);
        }
    }

    public class Avg : IFunction
    {
        public int Apply(List<int> args)
        {
            if (args.Count == 0)
                return 0;

            int sum = 0;

            for (int i = 0; i < args.Count; i++)        
                sum += args[i];
           
            return sum / args.Count;
        }
    }
}
