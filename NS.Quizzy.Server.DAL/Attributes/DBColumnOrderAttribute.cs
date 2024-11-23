namespace NS.Quizzy.Server.DAL.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class DBColumnOrderAttribute : Attribute
    {
        public int Order { get; }

        public DBColumnOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
