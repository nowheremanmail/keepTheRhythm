namespace UniversalKeepTheRhythm.Services
{
    public class ListOfInmediatePoints : CircularBuffer<InmediatePoint>
    {
        public ListOfInmediatePoints(int N)
            : base(N)
        {

        }
    }

}


