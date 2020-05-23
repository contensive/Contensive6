//
namespace Contensive.Processor.Addons.Login {
    public class GetLoginButtonFormClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// Login Button Form
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            if ( cp.User.IsAuthenticated ) {
                return Processor.Properties.Resources.LogoutButtonFormHtml.Replace("{{personName}}", cp.User.Name );
            }
            return Processor.Properties.Resources.LoginButtonFormHtml;
        }
    }
}
