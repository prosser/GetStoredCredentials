using System;

namespace PeterRosser.GetStoredCredentials
{
    public class Credential
    {
        public Credential(CredentialType credentialType, string applicationName, string userName, string password)
        {
            ApplicationName = applicationName;
            UserName = userName;
            Password = password;
            CredentialType = credentialType;
        }

        public CredentialType CredentialType { get; }

        public string ApplicationName { get; }

        public string UserName { get; }

        public string Password { get; }

        public override string ToString()
        {
            return
                FormattableString.Invariant(
                    $"CredentialType: {CredentialType}, ApplicationName: {ApplicationName}, UserName: {UserName}, Password: {Password}");
        }
    }
}