using MHA.Framework.Core.SP;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using Microsoft.SharePoint.Client;

namespace MHA.ELAN.Business
{
    public class ReportBL
    {
        public async Task<ViewModelReportListing> InitReportListing(string spHostURL, string accessToken)
        {
            ViewModelReportListing report = new ViewModelReportListing();
            
            try
            {
                TokenHelper.CheckValidAccessToken(accessToken, spHostURL);
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    string[] group = { ConstantHelper.SPSecurityGroup.ElanAdmin };
                    report.IsAuthorized = SharePointHelper.IsUserInGroups(clientContext, string.Empty, group);

                    if (report.IsAuthorized)
                    {
                        string[] viewFields = { ConstantHelper.SPColumn.ReportURL.Title, ConstantHelper.SPColumn.ReportURL.URL };
                        ListItemCollection reportItems = GeneralQueryHelper.GetSPItems(clientContext, ConstantHelper.SPList.ReportUrl, string.Empty, viewFields);
                        if (reportItems != null && reportItems.Count > 0)
                        {
                            foreach (ListItem reportItem in reportItems)
                            {
                                report.ReportLinks.Add(FieldHelper.GetFieldValueAsString(reportItem, ConstantHelper.SPColumn.ReportURL.Title), FieldHelper.GetFieldValueAsString(reportItem, ConstantHelper.SPColumn.ReportURL.URL));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return report;
        }
    }
}
