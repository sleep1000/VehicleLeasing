namespace VehicleLeasing.Util
{
    public interface IServerTime
    {
        DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }

    public class DefaultServerTime : IServerTime
    {
    }
}
