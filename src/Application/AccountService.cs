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
            // Cria a conta com o saldo inicial
            var newAccount = new Domain.Account(accountId, amount);
            store.Upsert(newAccount);
            return (newAccount, true);
        }

        account.Balance += amount;
        store.Upsert(account);
        return (account, false);
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
        if (origin == null || !origin.TryWithdraw(amount))
        {
            return null;
        }

        var destination = store.Get(destinationId);
        if (destination == null)
        {
            destination = new Domain.Account(destinationId, 0);
        }
        destination.Balance += amount;
        store.Upsert(origin);
        store.Upsert(destination);

        return (origin, destination, true);
    }

    public void Reset() => store.Clear();
}