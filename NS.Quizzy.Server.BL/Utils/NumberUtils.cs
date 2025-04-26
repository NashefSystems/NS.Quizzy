namespace NS.Quizzy.Server.BL.Utils
{
    internal static class NumberUtils
    {
        public static double? GetPercentage(int? value, int? total)
        {
            if (value == null || total == null)
            {
                return null;
            }

            if (value == 0 || total == 0)
            {
                return 0;
            }

            var val = ((double)value) * 100.0 / (double)total;
            return Math.Round(val);
        }
    }
}
