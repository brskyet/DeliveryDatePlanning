using Points.Protocol.Models;

namespace DeliveryDatePlanning.UnitTest.Common.TestDataProviders;

public static class Points
{
    public static Point PointFactory(List<DateTime> scheduleExceptions)
    {
        return new ()
        {
            ServiceDateExceptions = scheduleExceptions, 
            ServiceSchedule = new Schedule()
            {
                Days = new Dictionary<DayOfWeek, DaySchedule>()
                {
                    { DayOfWeek.Monday , new DaySchedule()
                    {
                        From = new TimeSpan(09,00,00), To = new TimeSpan(19,00,00)
                    }}
                }
            }

        };
    }
}