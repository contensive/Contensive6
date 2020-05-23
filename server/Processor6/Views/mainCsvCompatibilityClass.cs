
using System;
using Contensive.Processor.Controllers;
 
namespace Contensive.Processor {
    public class MainCsvScriptCompatibilityClass {
        private readonly CoreController core;
        public MainCsvScriptCompatibilityClass(CoreController core) {
            this.core = core;
        }
        //
        public void SetViewingProperty( string propertyName , string propertyValue ) {
            core.siteProperties.setProperty(propertyName, propertyValue);
        }
        //
        public string EncodeContent9(string Source, int personalizationPeopleId , string ContextContentName, int ContextRecordID, int ContextContactPeopleID, bool PlainText, bool AddLinkEID, bool EncodeActiveFormatting , bool EncodeActiveImages , bool EncodeActiveEditIcons , bool EncodeActivePersonalization, string AddAnchorQuery , string ProtocolHostString , bool IsEmailContent ,  int DefaultWrapperID , String ignore_TemplateCaseOnly_Content, int addonContext ) {
            return ActiveContentController.encode(core, Source, personalizationPeopleId, ContextContentName, ContextRecordID, ContextContactPeopleID, PlainText, AddLinkEID, EncodeActiveFormatting, EncodeActiveImages, EncodeActiveEditIcons, EncodeActivePersonalization, AddAnchorQuery, ProtocolHostString, IsEmailContent, DefaultWrapperID, ignore_TemplateCaseOnly_Content, (CPUtilsClass.addonContext)addonContext, core.session.isAuthenticated, null, false);
        }
    }
}