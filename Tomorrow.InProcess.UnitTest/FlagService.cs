namespace Tomorrow.InProcess.UnitTest
{
    public class FlagService
    {
        public bool Called { get; protected set; } = false;
        public int Calls { get; protected set; } = 0;
        public void Call()
        {
            Called = true;
            Calls++;
        }
    }
}
