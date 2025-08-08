using System.Collections.Concurrent;
using Ebanx.Challenge.Application;
using Ebanx.Challenge.Domain;

namespace Ebanx.Challenge.Infrastructure;

public sealed class InMemoryAccountStore : IAccountStore
{
    private readonly ConcurrentDictionary<string, Account> _db = new();

    public Account? Get(string id) => _db.TryGetValue(id, out var acc) ? acc : null;

    public void Upsert(Account account) => _db[account.Id] = account;

    public void Clear() => _db.Clear();
}