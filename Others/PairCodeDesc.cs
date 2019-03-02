using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nowhereman
{
    public class PairCodeDesc : IComparable<PairCodeDesc>, IEquatable<PairCodeDesc>, IEqualityComparer<PairCodeDesc>, IEquatable<string> //, IComparer<PairCodeDesc>
    {
        public PairCodeDesc(string a, string b)
        {
            code = a;
            description = b;
        }
        public PairCodeDesc()
        {

        }
        public string code { get; set; }
        public string description { get; set; }

        public int CompareTo(PairCodeDesc other)
        {
            return code.CompareTo(other.code);
        }

        public bool Equals(PairCodeDesc other)
        {
            return code.Equals(other.code);
        }

        public bool Equals(PairCodeDesc x, PairCodeDesc y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(PairCodeDesc obj)
        {
            return code.GetHashCode();
        }

        public bool Equals(string other)
        {
            return code.Equals(other);
        }
    }
}
