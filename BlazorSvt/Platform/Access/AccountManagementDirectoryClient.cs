using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace BlazorSvt.Platform.Access;

/// <summary>
/// LDAP через <c>UserPrincipal.GetGroups()</c>; имя группы — <c>Sid.Translate(NTAccount)</c>.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class AccountManagementDirectoryClient : IActiveDirectoryClient
{
    public Task<DirectoryUser> GetUserAsync(string login, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!OperatingSystem.IsWindows())
        {
            throw new DirectoryUnavailableException("Active Directory is only available on Windows.");
        }

        try
        {
            return Task.FromResult(GetUserCore(login));
        }
        catch (DirectoryUnavailableException)
        {
            throw;
        }
        catch (Exception ex) when (ex is PrincipalServerDownException
                                     or PrincipalOperationException
                                     or COMException)
        {
            throw new DirectoryUnavailableException("Failed to query Active Directory.", ex);
        }
    }

    private static DirectoryUser GetUserCore(string login)
    {
        var (domain, sam) = SplitLogin(login);

        using var context = string.IsNullOrEmpty(domain)
            ? new PrincipalContext(ContextType.Domain)
            : new PrincipalContext(ContextType.Domain, domain);

        using var principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, sam);
        if (principal is null)
        {
            return new DirectoryUser(sam, Email: null, Groups: []);
        }

        var groups = new List<string>();
        using var groupPrincipals = principal.GetGroups();
        foreach (var group in groupPrincipals)
        {
            try
            {
                if (group.Sid is null)
                {
                    continue;
                }

                var account = (NTAccount)group.Sid.Translate(typeof(NTAccount));
                if (!string.IsNullOrWhiteSpace(account.Value))
                {
                    groups.Add(account.Value);
                }
            }
            catch (IdentityNotMappedException)
            {
                // SID without a resolvable name — skip, do not fail the whole lookup.
            }
            catch (SystemException)
            {
                // Translate can throw ArgumentException / COMException for some SIDs.
            }
            finally
            {
                group.Dispose();
            }
        }

        var displayName = string.IsNullOrWhiteSpace(principal.DisplayName)
            ? principal.SamAccountName
            : principal.DisplayName;
        var email = string.IsNullOrWhiteSpace(principal.EmailAddress)
            ? null
            : principal.EmailAddress.Trim();

        return new DirectoryUser(displayName, email, groups);
    }

    private static (string? Domain, string Sam) SplitLogin(string login)
    {
        var separator = login.IndexOf('\\');
        if (separator <= 0 || separator == login.Length - 1)
        {
            return (null, login);
        }

        return (login[..separator], login[(separator + 1)..]);
    }
}
