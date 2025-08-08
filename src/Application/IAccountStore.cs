using Ebanx.Challenge.Domain;

namespace Ebanx.Challenge.Application;
public interface IAccountStore
{
    Account? Get(string id);
    void Upsert(Account account);
    void Clear();
}