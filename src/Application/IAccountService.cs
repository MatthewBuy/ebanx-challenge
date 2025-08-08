using Ebanx.Challenge.Domain;

namespace Ebanx.Challenge.Application;

public interface IAccountService
{
    int? GetBalance(string accountId);
    (Account? Destination, bool Created) Deposit(string accountId, int amount);
    (Account? Origin, bool Ok)? Withdraw(string originId, int amount);
    (Account? Origin, Account? Destination, bool Ok)? Transfer(string originId, string destinationId, int amount);
    void Reset();
}