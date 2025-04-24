namespace P2P.Domains.ValueObjects;

public record ConsentSettings
{
    public bool TermsAccepted { get; private set; }
    public bool MarketingOptIn { get; private set; }

    public void AcceptTerms() => TermsAccepted = true;
    public void OptInForMarketing() => MarketingOptIn = true;
};