using System.Threading.Tasks;

public interface IDataService
{
    Task<PlayerDataSO> LoadPlayerDataAsync();
    Task<bool> SavePlayerDataAsync(PlayerDataSO data);
    Task<bool> InitializeNewPlayerAsync();
}