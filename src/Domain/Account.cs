namespace Ebanx.Challenge.Domain;

public sealed class Account
{
    public string Id { get; }
    public int Balance { get; set; }

    public Account(string id, int initialBalance)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Account ID cannot be null or empty.");
        if (initialBalance < 0) throw new ArgumentOutOfRangeException(nameof(initialBalance));
        Id = id;
        Balance = initialBalance;
    }

    public void Deposit(int amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be positive.");
        Balance += amount;
    }

    public bool TryWithdraw(int amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal amount must be positive.");
        if (Balance < amount) return false;
        Balance -= amount;
        return true;
    }

}