namespace UniversalKeepTheRhythm.Services
{
#if (P)
    public class ListOfValues : CircularBuffer<double>
    {
        public double sumInv;

        public ListOfValues(int L)
            : base(L)
        {
            sumInv = 0;
        }

        public new double Add(double a)
        {
            int n = getCount();
            double old = base.Add(a);

            if (n == getN())
            {
                if (old > 0)
                    sumInv -= 1 / old;
            }

            if (a > 0)
                sumInv += 1 / a;

            return old;
        }

        public bool isDecreasing()
        {
            int n = getCount();
            if (n >= 3)
            {
                double first = getFirst();
                double last = getLast();

                return first > last;
            }
            else
            {
                return false;
            }
        }

        public bool isIncreasing()
        {
            int n = getCount();
            if (n >= 3)
            {
                double first = getFirst();
                double last = getLast();

                return first < last;
            }
            else
            {
                return false;
            }
        }
    }
#endif
#if (Q)
    public class ListOfValues : CircularBuffer<double>
    {
        public double max, min;
        public double sumInv;

        public ListOfValues(int L)
            : base(L)
        {
            max = Double.MinValue;
            min = Double.MaxValue;
            sumInv = 0;
        }

        public new double Add(double a)
        {
            int n = getCount();
            double old = base.Add(a);
            if (max < a) max = a;
            if (min > a) min = a;

            if (n == getN())
            {
                if (old > 0)
                    sumInv -= 1 / old;
            }

            if (a > 0)
                sumInv += 1 / a;

            return old;
        }

        public bool isDecreasing()
        {
            double avgTmp = 0;
            double minTmp = Double.MaxValue;
            double maxTmp = Double.MinValue;
            double c = 0;

            bool isFirst = true;
            double first = 0;
            double last = 0;
            foreach (double tmp in this)
            {
                if (isFirst)
                {
                    isFirst = false;
                    first = tmp;
                }
                last = tmp;
                if (tmp > maxTmp) max = tmp;
                if (tmp < minTmp) min = tmp;
                if (tmp > 0) avgTmp += 1 / tmp;
                c++;
            }

            return first > last && (first > avgTmp && avgTmp > last);
        }

        public bool isIncreasing()
        {
            double avgTmp = 0;
            double minTmp = Double.MaxValue;
            double maxTmp = Double.MinValue;
            double c = 0;

            bool isFirst = true;
            double first = 0;
            double last = 0;
            foreach (double tmp in this)
            {
                if (isFirst)
                {
                    isFirst = false;
                    first = tmp;
                }
                last = tmp;
                if (tmp > maxTmp) max = tmp;
                if (tmp < minTmp) min = tmp;
                if (tmp > 0) avgTmp += 1 / tmp;
                c++;
            }

            return first < last && (first < avgTmp && avgTmp < last);
        }
    }
#endif

    public class ListOfValues : CircularBuffer<double>
    {
        private int resP = 0;
        private int resN = 0;

        public ListOfValues(int L)
            : base(L)
        {
            resP = 0;
            resN = 0;
        }

        public new double Add(double a)
        {
            int n = getCount();

            if (n > 1)
            {
                double last = getLast();

                if (last < a) resP++;
                if (last > a) resN++;

                if (n == getN())
                {
                    double old = base.Add(a);
                    double first = getFirst();
                    if (old < first) resP--;
                    if (old > first) resN--;

                    return old;
                }
                else
                {
                    return base.Add(a);
                }
            }
            else
            {
                return base.Add(a);
            }
        }

        public bool isDecreasing()
        {
            return resN > 0 && resP == 0;
        }

        public bool isIncreasing()
        {
            return resP > 0 && resN == 0;
        }

        public bool isTourn()
        {
            return resP > 0 && resN > 0 && resP == resN;
        }
    }

}


