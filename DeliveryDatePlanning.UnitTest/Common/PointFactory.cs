using Points.Protocol.Models;

namespace DeliveryDatePlanning.UnitTest.Common;

public class PointFactory
{
    public static DaySchedule DefaultSchedule => new DaySchedule
    {
        From = new TimeSpan(10, 0, 0),
        To = new TimeSpan(18, 0, 0)
    };
    
    public static Point CreateDefaultPoint()
    {
        return new Point
        {
            ServiceSchedule = new Schedule
            {
                Days = new Dictionary<DayOfWeek, DaySchedule>
                {
                    {
                        DayOfWeek.Monday, DefaultSchedule
                    },
                    {
                        DayOfWeek.Tuesday, DefaultSchedule
                    },
                    {
                        DayOfWeek.Wednesday, DefaultSchedule
                    },
                    {
                        DayOfWeek.Thursday, DefaultSchedule
                    },
                    {
                        DayOfWeek.Friday, DefaultSchedule
                    },
                    {
                        DayOfWeek.Saturday, DefaultSchedule
                    }
                }
            }
        };
    }
}