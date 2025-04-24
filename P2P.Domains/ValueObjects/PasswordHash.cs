namespace P2P.Domains.ValueObjects;


    public record PasswordHash( byte[] Hash, byte[] Salt);
