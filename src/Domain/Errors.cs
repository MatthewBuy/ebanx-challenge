namespace Ebanx.Challenge.Domain;

public enum ErrorCode { NotFound, InvalidAmount, InsufficientFunds }
public record Error(ErrorCode Code, string Message);