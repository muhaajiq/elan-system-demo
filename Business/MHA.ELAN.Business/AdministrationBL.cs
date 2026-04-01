using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using Microsoft.SharePoint.Client;

namespace MHA.ELAN.Business
{
    public class AdministrationBL
    {
        public async Task<ViewModelAdministrationListing> InitAdministrationListing(string spHostUrl, string accessToken)
        {
            ViewModelAdministrationListing vm = new ViewModelAdministrationListing();

            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
                {
                    string[] groups = new[] { ConstantHelper.SPSecurityGroup.ElanAdmin };
                    vm.IsAuthorized = SharePointHelper.IsUserInGroups(clientContext, string.Empty, groups);

                    if (vm.IsAuthorized)
                    {
                        vm.ReportURL = string.Format(ConstantHelper.URLTemplate.ReportURLTemplate, spHostUrl);
                        vm.RunningNumberFormatURL = string.Format(ConstantHelper.URLTemplate.RunningNumberFormatUrlTemplate, spHostUrl);
                        vm.RunningNumberURL = string.Format(ConstantHelper.URLTemplate.RunningNumberUrlTemplate, spHostUrl);
                        vm.CompanyURL = string.Format(ConstantHelper.URLTemplate.CompanyUrlTemplate, spHostUrl);
                        vm.LocationURL = string.Format(ConstantHelper.URLTemplate.LocationUrlTemplate, spHostUrl);
                        vm.GradeURL = string.Format(ConstantHelper.URLTemplate.GradeUrlTemplate, spHostUrl);
                        vm.DepartmentURL = string.Format(ConstantHelper.URLTemplate.DepartmentUrlTemplate, spHostUrl);
                        vm.DesignationURL = string.Format(ConstantHelper.URLTemplate.DesignationUrlTemplate, spHostUrl);
                        vm.ITRequirementsURL = string.Format(ConstantHelper.URLTemplate.ITRequirementsUrlTemplate, spHostUrl);
                        vm.HardwareURL = string.Format(ConstantHelper.URLTemplate.HardwareUrlTemplate, spHostUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return vm;
        }
    }
}
