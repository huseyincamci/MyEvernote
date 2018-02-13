namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class RepositoryBase
    {
        protected static DatabaseContext _context;
        private static object _lockSync = new object();

        protected RepositoryBase()
        {
             CreateContext();
        }

        private static void CreateContext()
        {
            if (_context == null)
            {
                lock (_lockSync)
                {
                    if (_context == null)
                    {
                        _context = new DatabaseContext();
                    }
                }
            }
        }
    }
}
