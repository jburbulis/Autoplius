namespace Autoplius.Repository.Repositories
{
    public interface ISearchesRepository : IGenericRepository<Searches>
    {
    }

    public class SearchesRepository : GenericRepository<Searches>, ISearchesRepository
    {
        public SearchesRepository(AutopliusDatabase context) : base(context)
        {
        }
    }
}
