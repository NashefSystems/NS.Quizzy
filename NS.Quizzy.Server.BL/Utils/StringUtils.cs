
namespace NS.Quizzy.Server.BL.Utils
{
    internal static class StringUtils
    {
        public static string? FirstNotNullOrWhiteSpace(params string?[] values)
        {
            if (values?.Length > 0)
            {
                foreach (var item in values)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        internal static string FirstNotNullOrWhiteSpace(object platform, string v)
        {
            throw new NotImplementedException();
        }
    }
}
