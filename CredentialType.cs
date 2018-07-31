namespace PeterRosser.GetStoredCredentials
{
    public enum CredentialType
    {
        None = 0,
        Generic = 1,
        DomainPassword,
        DomainCertificate,
        DomainVisiblePassword,
        GenericCertificate,
        DomainExtended,
        Maximum,
        Maximum2 = Maximum + 1000
    }
}