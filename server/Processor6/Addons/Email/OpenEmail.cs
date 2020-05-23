﻿//
using System;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Constants;
using Contensive.Models.Db;
//
namespace Contensive.Processor.Addons.Primitives {
    public class OpenEmailClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// getFieldEditorPreference remote method
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            try {
                CoreController core = ((CPClass)cp).core;
                int emailDropId = core.docProperties.getInteger(rnEmailOpenFlag);
                if (emailDropId != 0) {
                    //
                    // -- Email open detected. Log it and redirect to a 1x1 spacer
                    EmailDropModel emailDrop = DbBaseModel.create<EmailDropModel>(core.cpParent, emailDropId);
                    if (emailDrop != null) {
                        PersonModel recipient = DbBaseModel.create<PersonModel>(core.cpParent, core.docProperties.getInteger(rnEmailMemberId));
                        if (recipient != null) {
                            EmailLogModel log = DbBaseModel.addDefault<EmailLogModel>(core.cpParent);
                            log.name = "User " + recipient.name + " opened email drop " + emailDrop.name + " at " + core.doc.profileStartTime.ToString();
                            log.emailDropId = emailDrop.id;
                            log.emailId = emailDrop.emailId;
                            log.memberId = recipient.id;
                            log.logType = EmailLogTypeOpen;
                            log.visitId = cp.Visit.Id;
                            log.save(cp);
                        }
                    }
                }
                core.webServer.redirect(NonEncodedLink: "" + cdnPrefix + "images/spacer.gif", RedirectReason: "Group Email Open hit, redirecting to a dummy image", IsPageNotFound: false, allowDebugMessage: false);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return "";
        }
    }
}
