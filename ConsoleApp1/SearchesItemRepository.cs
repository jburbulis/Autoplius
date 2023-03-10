namespace Autoplius.Repository.Repositories
{
    public interface ISearchesItemRepository : IGenericRepository<SearchesItem>
    {
    }

    public class SearchesItemRepository : GenericRepository<SearchesItem>, ISearchesItemRepository
    {
        public SearchesItemRepository(AutopliusDatabase context) : base(context)
        {
        }
    }
}
