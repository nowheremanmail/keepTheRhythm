namespace UniversalKeepTheRhythm.Services
{
    public class ListOfMovements : CircularBuffer<int>
    {
        private int stop = 0;
        private int move = 0;
        private int unkw = 0;

        public ListOfMovements(int L)
            : base(L)
        {
            stop = 0;
            move = 0;
            unkw = 0;
        }

        public new void clear()
        {
            stop = 0;
            move = 0;
            unkw = 0;

            base.clear();
        }

        public int getRunning()
        {
            return move;
        }

        public int getStopping()
        {
            return stop;
        }

        public int getUnknow()
        {
            return unkw;
        }



        public new int Add(int a)
        {
            bool wasFull = isFull();

            if (a == 0)
            {
                stop++;
            }
            else if (a > 0)
            {
                move++;
            }
            else
            {
                unkw++;
            }

            int old = base.Add(a);

            if (wasFull)
            {
                if (old == 0)
                {
                    stop--;
                }
                else if (old > 0)
                {
                    move--;
                }
                else
                {
                    unkw--;
                }
            }

            return old;
        }
    }

}


