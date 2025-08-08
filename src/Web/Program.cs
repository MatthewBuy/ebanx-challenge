using Ebanx.Challenge.Application;
using Ebanx.Challenge.Infrastructure;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAccountStore, InMemoryAccountStore>();
builder.Services.AddSingleton<IAccountService, AccountService>();

var app = builder.Build();

app.MapPost("/reset", (IAccountService svc) =>
{
    svc.Reset();
    return Results.Ok();
});

app.MapGet("/balance", ([FromQuery(Name = "account_id")] string account_id, IAccountService svc) =>
{
    var bal = svc.GetBalance(account_id);
    return bal is null ? Results.NotFound("0") : Results.Text(bal.Value.ToString());
});

app.MapPost("/event", async (HttpContext ctx, IAccountService svc) =>
{
    var req = await ctx.Request.ReadFromJsonAsync<EventRequest>();
    if (req is null) return Results.BadRequest();

    switch (req.Type)
    {
        case "deposit":
            if (string.IsNullOrWhiteSpace(req.Destination) || req.Amount is null)
                return Results.BadRequest();

            var (destAccount, _) = svc.Deposit(req.Destination, req.Amount.Value);
            return Results.Created("/balance", new
            {
                destination = new { id = destAccount.Id, balance = destAccount.Balance }
            });

        case "withdraw":
            if (string.IsNullOrWhiteSpace(req.Origin) || req.Amount is null)
                return Results.BadRequest();

            var withdrawResult = svc.Withdraw(req.Origin, req.Amount.Value);
            if (withdrawResult is null)
                return Results.NotFound("0");

            var (originAccount, _) = withdrawResult.Value;
            return Results.Created("/balance", new
            {
                origin = new { id = originAccount.Id, balance = originAccount.Balance }
            });

        case "transfer":
            if (string.IsNullOrWhiteSpace(req.Origin) || string.IsNullOrWhiteSpace(req.Destination) || req.Amount is null)
                return Results.BadRequest();

            var transferResult = svc.Transfer(req.Origin, req.Destination, req.Amount.Value);
            if (transferResult is null)
                return Results.NotFound("0");

            var (fromAcc, toAcc, _) = transferResult.Value;
            return Results.Created("/balance", new
            {
                origin = new { id = fromAcc.Id, balance = fromAcc.Balance },
                destination = new { id = toAcc.Id, balance = toAcc.Balance }
            });

        default:
            return Results.BadRequest();
    }
});

app.Run();

record EventRequest
{
    public string Type { get; init; } = null!;
    public string? Origin { get; init; }
    public string? Destination { get; init; }
    public int? Amount { get; init; }
}