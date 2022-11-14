namespace VehicleLeasing.Exceptions
{
    public class VehicleInLeaseException : Exception
    {
        public VehicleInLeaseException(): base("Vehicle already in lease")
        {
        }
    }
}
