namespace MapCore.Services
{
    public interface IMapProvider
    {
        string GetTileUrl(int z, int x, int y);
        bool RequiresAuthentication { get; }
        string GetAuthenticationHeader();
        string GetSearchParamName();
        string GetLimitParamName();
    }
} 