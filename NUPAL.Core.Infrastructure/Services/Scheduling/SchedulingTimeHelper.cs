namespace NUPAL.Core.Infrastructure.Services.Scheduling
{
    internal static class SchedulingTimeHelper
    {
        internal const int SlotStartMin = 7 * 60;    // 07:00
        internal const int SlotEndMin   = 21 * 60;   // 21:00
        internal const int SlotSizeMin  = 30;
        internal const int NumSlots     = (SlotEndMin - SlotStartMin) / SlotSizeMin; // 28
        internal static readonly string[] DaysOrder =
            ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

        internal static readonly Dictionary<string, int> DayToIdx =
            DaysOrder.Select((d, i) => (d, i)).ToDictionary(x => x.d, x => x.i);

       
        internal static int? ParseTime(string t)
        {
            var s = (t ?? "").Trim();
            if (string.IsNullOrEmpty(s)) return null;

            var parts = s.Split(':');
            if (parts.Length < 2) return null;
            if (!int.TryParse(parts[0], out int h) || !int.TryParse(parts[1], out int m)) return null;

            return h * 60 + m;
        }

        
        internal static string FormatTime(string t)
        {
            var p = ParseTime(t);
            return p.HasValue ? $"{p.Value / 60:D2}:{p.Value % 60:D2}" : t;
        }

        
        internal static List<int> TimeSlots(string startStr, string endStr)
        {
            var s = ParseTime(startStr);
            var e = ParseTime(endStr);
            if (!s.HasValue || !e.HasValue || e <= s) return [];

            var slots = new List<int>();
            for (int i = 0; i < NumSlots; i++)
            {
                int slotStart = SlotStartMin + i * SlotSizeMin;
                int slotEnd   = slotStart + SlotSizeMin;
                if (s < slotEnd && e > slotStart) slots.Add(i);
            }
            return slots;
        }

        internal static string TitleCase(string s) =>
            string.Join(" ", s.Split(' ')
                .Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w[1..] : w));
    }
}
