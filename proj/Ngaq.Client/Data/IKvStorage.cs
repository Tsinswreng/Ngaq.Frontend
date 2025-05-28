namespace Ngaq.Client.Data;

public interface IKvStorage{
	public Task<str?> Get(str key);
	public Task<nil> Set(str key, str value);
	public Task<nil> Delete(str key);
}
