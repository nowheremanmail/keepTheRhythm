using System;

namespace UniversalKeepTheRhythm
{
    public class CircularBuffer<T>
    {
        readonly object _locker = new object();

        T[] list;
        int N;
        int last = -1;
        int first = -1;
        int C = 0;

        public void clear()
        {
            last = -1;
            first = -1;
            C = 0;
        }

        public int getFirstIndex()
        {
            lock (_locker)
            {
                return first;
            }
        }

        public int getNLast()
        {
            lock (_locker)
            {
                return last;
            }
        }

        public T getItem(int L, int i)
        {
            lock (_locker)
            {

                int P = L - i;
                if (P < 0) P += N;
                return list[P];
            }
        }

        public T getLast()
        {
            lock (_locker)
            {
                return list[last];
            }
        }

        public T getFirst()
        {
            lock (_locker)
            {
                return list[first];
            }
        }

        public T dequeue()
        {
            lock (_locker)
            {
                if (C > 0)
                {
                    C--;
                    int oldFirst = first;
                    first = (first + 1) % N;

                    if (C == 0)
                    {
                        last = -1;
                        first = -1;
                    }

                    return list[oldFirst];
                }
                else
                {
                    throw new Exception("EMpty");
                }
            }
        }

        public T getLast(int L)
        {
            lock (_locker)
            {
                int P = last - L;
                if (P < 0) P += N;
                return list[P];
            }
        }

        public int getCount()
        {
            lock (_locker)
            {
                return C;
            }
        }

        public int getN()
        {
            return N;
        }
        public CircularBuffer(int L)
        {
            N = L;
            list = new T[N];
            last = -1;
            first = -1;
            C = 0;
        }

        public T Add(T o)
        {
            lock (_locker)
            {
                T res = o;

                if (last == -1)
                {
                    last = 0;
                    first = 0;
                    C++;
                }
                else
                {
                    last = (last + 1) % N;
                    if (first == last)
                    {
                        res = list[first];
                        first = (first + 1) % N;
                    }
                    else
                    {
                        C++;
                    }
                }
                list[last] = o;

                return res;
            }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            for (int i = 0; i < C; i++)
            {
                yield return list[(i + first) % N];
            }
        }

        public System.Collections.IEnumerator GetEnumerator(int newFirst, int _C)
        {
            for (int i = 0; i < C && i < _C; i++)
            {
                yield return list[(i + newFirst) % N];
            }
        }

        public bool isFull()
        {
            lock (_locker)
            {
                return C == N;
            }
        }
    }

}
