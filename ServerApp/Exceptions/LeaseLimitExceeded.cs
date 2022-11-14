namespace VehicleLeasing.Exceptions
{
    public class LeasesLimitExceeded: Exception
    {
        public LeasesLimitExceeded(): base("Driver reached leases limit")
        {
        }
    }
}
