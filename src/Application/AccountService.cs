using Ebanx.Challenge.Domain;

namespace Ebanx.Challenge.Application;

public sealed class AccountService(IAccountStore store) : IAccountService
{
    public int? GetBalance(string accountId)
    {
        var account = store.Get(accountId);
        return account?.Balance;
    }

    public (Account? Destination, bool Created) Deposit(string accountId, int amount)
    {
        var account = store.Get(accountId);
        if (account == null)
        {
            return (null, false);
        }

        account.Balance += amount;
        store.Upsert(account);
        return (account, true);
    }

    public (Account? Origin, bool Ok)? Withdraw(string originId, int amount)
    {
        var origin = store.Get(originId);
        if (origin == null || !origin.TryWithdraw(amount))
        {
            return null;
        }

        store.Upsert(origin);
        return (origin, true);
    }

    public (Account? Origin, Account? Destination, bool Ok)? Transfer(string originId, string destinationId, int amount)
    {
        var origin = store.Get(originId);
        var destination = store.Get(destinationId);

        if (origin == null || destination == null || !origin.TryWithdraw(amount))
        {
            return null;
        }

        destination.Balance += amount;
        store.Upsert(origin);
        store.Upsert(destination);

        return (origin, destination, true);
    }

    public void Reset() => store.Clear();
}