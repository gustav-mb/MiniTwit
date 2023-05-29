using MiniTwit.Core.Responses;

namespace MiniTwit.Core.IRepositories;

public interface IFollowerRepository
{
    Task<DBResult> CreateAsync(string sourceId, string targetUsername);
    Task<DBResult> DeleteAsync(string sourceId, string targetUsername);
}