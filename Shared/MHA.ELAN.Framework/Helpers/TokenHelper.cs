using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.JSONConstants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.SharePoint.Client;
using PnP.Core.Auth;
using PnP.Core.Services;
using PnP.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MHA.ELAN.Framework.Helpers
{
    public class TokenHelper
    {
        private readonly IPnPContextFactory _pnpContextFactory;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly JSONAppSettings appSettings;

        private readonly string[] scopes = new string[] { "https://mha.sharepoint.com/.default" };

        public TokenHelper(IPnPContextFactory pnpContextFactory, ITokenAcquisition tokenAcquisition, IWebHostEnvironment webHostEnvironment)
        {
            _pnpContextFactory = pnpContextFactory;
            _tokenAcquisition = tokenAcquisition;
            _webHostEnvironment = webHostEnvironment;
            appSettings = ConfigurationManager.GetAppSetting();
        }

        public async Task<PnPContext> CreateSiteContext(string spHostUrl)
        {
            var siteUrl = new Uri(spHostUrl);
            var scopes = new[] { "https://mha.sharepoint.com/.default" };
            return await _pnpContextFactory.CreateAsync(siteUrl,
                            new ExternalAuthenticationProvider((resourceUri, scopes) =>
                            {
                                return _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                            }
                            ));
        }

        public async Task<string> GetUserAccessToken()
        {
            return await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
        }

        public async Task<string> GetAccessTokenFromHybridApp(string SPHostURL)
        {
            string accessToken = string.Empty;

            string certificatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Certs", appSettings.CertName);

            X509Certificate2 certificate2 = new X509Certificate2(certificatePath, appSettings.CertPassword);

            var am_1 = AuthenticationManager.CreateWithCertificate(appSettings.ClientId, certificate2, appSettings.TenantId);
            accessToken = await Task.Run(() => am_1.GetAccessToken(SPHostURL));

            return accessToken;
        }

        public static bool CheckValidAccessToken(string accessToken, string spHostUrl)
        {
            bool IsValid = false;
            try
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var authManager = AuthenticationManager.CreateWithAccessToken(ConvertAccessTokenToSecureString(accessToken));

                    using (ClientContext clientContext = authManager.GetContext(spHostUrl))
                    {
                        clientContext.ExecuteQuery();
                        IsValid = true;
                    }
                }
                else
                {
                    throw new Exception("No access token retrieved.");
                }
            }
            catch (System.Net.WebException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Check valid access token error. ", ex);
            }
            return IsValid;
        }

        public static SecureString ConvertAccessTokenToSecureString(string accessToken)
        {
            SecureString secureString = new SecureString();
            for (int i = 0; i < accessToken.Length; i++)
                secureString.AppendChar(accessToken[i]);

            return secureString;
        }

        public static ClientContext GetClientContextWithAccessToken(string spHostUrl, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(spHostUrl))
                throw new ArgumentException("SharePoint host URL cannot be null or empty.", nameof(spHostUrl));
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));

            try
            {
                var authManager = AuthenticationManager.CreateWithAccessToken(ConvertAccessTokenToSecureString(accessToken));
                var clientContext = authManager.GetContext(spHostUrl);

                if (clientContext == null)
                    throw new InvalidOperationException("Failed to create SharePoint ClientContext.");

                return clientContext;
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                throw new ApplicationException("Error obtaining SharePoint ClientContext.", ex);
            }
        }

        //public ClientContext GetClientContextFromHybridApp(string SPHostUrl)
        //{
        //    ClientContext clientContext = null;

        //    var tenantId = appSettings.TenantId;
        //    var clientId = appSettings.ClientId;
        //    var certificatName = appSettings.CertName;

        //    var certificatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "Certs", certificatName);
        //    var certificatePassword = appSettings.CertPassword;

        //    var certificate2 = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.MachineKeySet);

        //    //Create the authentication manager with cert
        //    var am_1 = PnP.Framework.AuthenticationManager.CreateWithCertificate(clientId, certificate2, tenantId);

        //    //Create Client Contexy with Authentication Manager
        //    clientContext = am_1.GetContext(SPHostUrl);

        //    return clientContext;
        //}
    }
}
