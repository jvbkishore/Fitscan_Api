namespace Fitscan_Api.Helpers
{
    public class DateFormatHelper
    {
        public static DateTime? ConvertToUtc(DateTime? dateTime)
        {
            if (dateTime == null)
                return null;

            return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
        }
    }
}
