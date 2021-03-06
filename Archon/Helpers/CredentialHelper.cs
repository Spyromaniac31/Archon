using Archon.Services;
using System;
using Windows.Security.Credentials;

namespace Archon.Helpers
{
    public static class CredentialHelper
    {
        public static void UpdateServerCredentials(string username, string password)
        {
            var vault = new PasswordVault();
            try
            {
                var credentialsList = vault.FindAllByResource("Archon");
                foreach (var credentials in credentialsList)
                {
                    vault.Remove(credentials);
                }
            }
            catch (Exception ex) 
            {
                ErrorReporterService.ReportError("Error removing old credentials", ex.ToString(), "Warning");
            }
            vault.Add(new PasswordCredential("Archon", username, password));
        }

        public static PasswordCredential RetrieveServerCredentials()
        {
            var vault = new PasswordVault();
            var credentials = vault.FindAllByResource("Archon")[0];
            credentials.RetrievePassword();
            return credentials;
        }
    }
}
