
using System;
using static Contensive.Processor.Controllers.GenericController;
using static Contensive.Processor.Constants;
//
// todo -- should not be here
using Contensive.BaseClasses;
using Contensive.Models.Db;
using Contensive.Processor.Models.Domain;
//
namespace Contensive.Processor.Controllers {
    //
    public class LoginController {
        //
        //========================================================================
        /// <summary>
        /// A complete html page with the login form in the middle. If it processes successfully it returns and empty response. (legacy, sorry)
        /// </summary>
        /// <param name="forceDefaultLogin"></param>
        /// <returns></returns>
        public static string getLoginPage(CoreController core, bool forceDefaultLogin) {
            string result = "";
            try {
                if (forceDefaultLogin) {
                    result = getLoginForm_Default(core);
                } else {
                    result = getLoginForm(core);
                }
                if (string.IsNullOrWhiteSpace(result)) return result;
                return "<div class=\"ccCon bg-light pt-2 pb-4\" style=\"width:400px;margin:100px auto 0 auto;border:1px solid #bbb;border-radius:5px;\">" + result + "</div>";
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
        }
        //
        // ====================================================================================================
        /// <summary>
        /// process and return the default login form. If processing is successful, a blank response is returned
        /// </summary>
        /// <param name="core"></param>
        /// <returns></returns>
        public static string getLoginForm_Default(CoreController core) {
            string result = "";
            try {
                //
                bool needLoginForm = true;
                string formType = core.docProperties.getText("type");
                if (formType == FormTypeLogin) {
                    //
                    // -- process a previous login for instance, and return blank if it is successful (legacy workflow)
                    if (processLoginFormDefault(core)) {
                        result = "";
                        needLoginForm = false;
                    }
                } else if (formType == FormTypeSendPassword) {
                    //
                    // -- process send password
                    processSendPasswordForm(core);
                    result += "<p>If this email was found on the system an email was sent with login instructions.</p>";
                }
                if (needLoginForm) {
                    string loginForm;
                    //
                    // -- select the correct html from resources
                    bool allowAutoLogin = core.siteProperties.getBoolean("AllowAutoLogin", false);
                    if (core.siteProperties.getBoolean("allowEmailLogin", false)) {
                        if (allowAutoLogin) {
                            loginForm = Properties.Resources.defaultLogin_email_autoLogin_html;
                        } else {
                            loginForm = Properties.Resources.defaultLogin_email_html;
                        }
                    } else {
                        if (allowAutoLogin) {
                            loginForm = Properties.Resources.defaultLogin_autoLogin_html;
                        } else {
                            loginForm = Properties.Resources.defaultLogin_html;
                        }
                    }
                    //
                    // -- add user errors
                    loginForm = loginForm.Replace("{message}", ErrorController.getUserError(core));
                    if (!core.doc.errorList.Count.Equals(0)) {
                    }
                    //
                    // -- create the action query
                    string QueryString = GenericController.modifyQueryString(core.webServer.requestQueryString, RequestNameHardCodedPage, "", false);
                    QueryString = GenericController.modifyQueryString(QueryString, "requestbinary", "", false);
                    loginForm += HtmlController.inputHidden("Type", FormTypeLogin);
                    loginForm += HtmlController.inputHidden("email", core.session.user.email);
                    result += HtmlController.form(core, loginForm, QueryString);
                    //
                    // ----- Password Form
                    if (core.siteProperties.getBoolean("allowPasswordEmail", true)) {
                        result += getSendPasswordForm(core);
                    }
                    //
                    result = HtmlController.div(result, "ccLoginFormCon");
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
            return result;
        }
        //
        //=============================================================================
        /// <summary>
        /// A login form that can be added to any page. This is just form with no surrounding border, etc. 
        /// </summary>
        /// <returns></returns>
        public static string getLoginForm(CoreController core, bool forceDefaultLoginForm = false) {
            string returnHtml = "";
            try {
                int loginAddonId = 0;
                if (!forceDefaultLoginForm) {
                    loginAddonId = core.siteProperties.getInteger("Login Page AddonID");
                    if (loginAddonId != 0) {
                        //
                        // -- Custom Login
                        AddonModel addon = DbBaseModel.create<AddonModel>(core.cpParent, loginAddonId);
                        CPUtilsBaseClass.addonExecuteContext executeContext = new CPUtilsBaseClass.addonExecuteContext {
                            addonType = CPUtilsBaseClass.addonContext.ContextPage,
                            errorContextMessage = "calling login form addon [" + loginAddonId + "] from internal method"
                        };
                        returnHtml = core.addon.execute(addon, executeContext);
                        if (string.IsNullOrEmpty(returnHtml)) {
                            //
                            // -- login successful, redirect back to this page (without a method)
                            string QS = core.doc.refreshQueryString;
                            QS = GenericController.modifyQueryString(QS, "method", "");
                            QS = GenericController.modifyQueryString(QS, "RequestBinary", "");
                            //
                            return core.webServer.redirect("?" + QS, "Login form success");
                        }
                    }
                }
                if (loginAddonId == 0) {
                    //
                    // ----- When page loads, set focus on login username
                    //
                    returnHtml = getLoginForm_Default(core);
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
            return returnHtml;
        }
        //
        //=============================================================================
        /// <summary>
        /// a simple email password form
        /// </summary>
        /// <returns></returns>
        public static string getSendPasswordForm(CoreController core) {
            string returnResult = "";
            try {
                string QueryString = null;
                //
                if (core.siteProperties.getBoolean("allowPasswordEmail", true)) {
                    returnResult += Properties.Resources.defaultForgetPassword_html;
                    //
                    // write out all of the form input (except state) to hidden fields so they can be read after login
                    returnResult += HtmlController.inputHidden("Type", FormTypeSendPassword);
                    foreach (string formKey in core.docProperties.getKeyList()) {
                        var formValue = core.docProperties.getProperty(formKey);
                        if (formValue.propertyType == DocPropertyModel.DocPropertyTypesEnum.form) {
                            switch (GenericController.toUCase(formValue.name)) {
                                case "S":
                                case "MA":
                                case "MB":
                                case "USERNAME":
                                case "PASSWORD":
                                case "EMAIL":
                                case "TYPE":
                                    break;
                                default:
                                    returnResult = returnResult + HtmlController.inputHidden(formValue.name, formValue.value);
                                    break;
                            }
                        }
                    }
                    QueryString = core.doc.refreshQueryString;
                    QueryString = GenericController.modifyQueryString(QueryString, "S", "");
                    QueryString = GenericController.modifyQueryString(QueryString, "ccIPage", "");
                    returnResult = HtmlController.form(core, returnResult, QueryString);
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
            return returnResult;
        }
        //
        //========================================================================
        /// <summary>
        /// Process the login form username and password
        /// </summary>
        /// <param name="core"></param>
        /// <returns></returns>
        public static bool processLoginFormDefault(CoreController core) {
            bool returnResult = false;
            try {
                //
                if ((core.session.visit.loginAttempts < core.siteProperties.maxVisitLoginAttempts) && core.session.visit.cookieSupport) {
                    int LocalMemberId = core.session.getUserIdForUsernameCredentials(
                        core.docProperties.getText("username"),
                        core.docProperties.getText("password")
                    );
                    if (LocalMemberId == 0) {
                        if ((core.session.isAuthenticated) || (core.session.isRecognized())) { core.session.logout(); }
                        core.session.visit.loginAttempts = core.session.visit.loginAttempts + 1;
                        core.session.visit.save(core.cpParent);
                    } else {
                        returnResult = core.session.authenticateById(LocalMemberId, core.session);
                        if (returnResult) {
                            LogController.addSiteActivity(core, "successful username/password login", core.session.user.id, core.session.user.organizationId);
                        } else {
                            LogController.addSiteActivity(core, "bad username/password login", core.session.user.id, core.session.user.organizationId);
                        }
                    }
                }

            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
            return returnResult;
        }
        //
        //========================================================================
        // ----- Process the send password form
        //
        public static void processSendPasswordForm(CoreController core) {
            try {
                string returnUserMessage = "";
                sendPassword(core, core.docProperties.getText("email"), ref returnUserMessage);
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// Send the Member his username and password
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static bool sendPassword(CoreController core, string Email, ref string returnUserMessage) {
            bool result = false;
            returnUserMessage = "";
            try {
                const string passwordChrs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ012345678999999";
                const int passwordChrsLength = 62;
                //
                string workingEmail = GenericController.encodeText(Email);
                //
                string Message = "";
                string FromAddress = "";
                string subject = "";
                if (string.IsNullOrEmpty(workingEmail)) {
                    ErrorController.addUserError(core, "Please enter your email address before requesting your username and password.");
                } else {
                    int atPtr = GenericController.strInstr(1, workingEmail, "@");
                    if (atPtr < 2) {
                        //
                        // email not valid
                        //
                        ErrorController.addUserError(core, "Please enter a valid email address before requesting your username and password.");
                    } else {
                        string EMailName = strMid(workingEmail, 1, atPtr - 1);
                        //
                        LogController.addSiteActivity(core, "password request for email " + workingEmail, core.session.user.id, core.session.user.organizationId);
                        //
                        bool allowEmailLogin = core.siteProperties.getBoolean("allowEmailLogin", false);
                        int recordCnt = 0;
                        using (var csData = new CsModel(core)) {
                            string sqlCriteria = "(email=" + DbController.encodeSQLText(workingEmail) + ")";
                            sqlCriteria = sqlCriteria + "and((dateExpires is null)or(dateExpires>" + DbController.encodeSQLDate(core.dateTimeNowMockable) + "))";
                            csData.open("People", sqlCriteria, "ID", true, core.session.user.id, "username,password", 1);
                            if (!csData.ok()) {
                                //
                                // valid login account for this email not found
                                //
                                if (encodeText(strMid(workingEmail, atPtr + 1)).ToLowerInvariant() == "contensive.com") {
                                    //
                                    // look for expired account to renew
                                    //
                                    csData.close();
                                    csData.open("People", "((email=" + DbController.encodeSQLText(workingEmail) + "))", "ID");
                                    if (csData.ok()) {
                                        //
                                        // renew this old record
                                        //
                                        csData.set("developer", "1");
                                        csData.set("admin", "1");
                                        if (csData.getDate("dateExpires") > DateTime.MinValue) { csData.set("dateExpires", core.dateTimeNowMockable.AddDays(7).Date.ToString()); }
                                    } else {
                                        //
                                        // inject support record
                                        //
                                        csData.close();
                                        csData.insert("people");
                                        csData.set("name", "Contensive Support");
                                        csData.set("email", workingEmail);
                                        csData.set("developer", "1");
                                        csData.set("admin", "1");
                                        csData.set("dateExpires", core.dateTimeNowMockable.AddDays(7).Date.ToString());
                                    }
                                } else {
                                    ErrorController.addUserError(core, "No current user was found matching this email address. Please try again. ");
                                }
                            }
                            if (csData.ok()) {
                                FromAddress = core.siteProperties.getText("EmailFromAddress", "info@" + core.webServer.requestDomain);
                                subject = "Password Request at " + core.webServer.requestDomain;
                                Message = "";
                                while (csData.ok()) {
                                    bool updateUser = false;
                                    if (string.IsNullOrEmpty(Message)) {
                                        Message = "This email was sent in reply to a request at " + core.webServer.requestDomain + " for the username and password associated with this email address. ";
                                        Message += "If this request was made by you, please return to the login screen and use the following:\r\n";
                                        Message += Environment.NewLine;
                                    } else {
                                        Message += Environment.NewLine;
                                        Message += "Additional user accounts with the same email address: \r\n";
                                    }
                                    //
                                    // username
                                    //
                                    string Username = csData.getText("Username");
                                    bool usernameOK = true;
                                    int Ptr = 0;
                                    if (!allowEmailLogin) {
                                        if (Username != Username.Trim()) {
                                            Username = Username.Trim();
                                            updateUser = true;
                                        }
                                        if (string.IsNullOrEmpty(Username)) {
                                            usernameOK = false;
                                            Ptr = 0;
                                            while (!usernameOK && (Ptr < 100)) {
                                                Username = EMailName + encodeInteger(Math.Floor(encodeNumber(Microsoft.VisualBasic.VBMath.Rnd() * 9999)));
                                                usernameOK = !core.session.isLoginOK(Username, "test");
                                                Ptr = Ptr + 1;
                                            }
                                            if (usernameOK) {
                                                updateUser = true;
                                            }
                                        }
                                        Message += " username: " + Username + Environment.NewLine;
                                    }
                                    if (usernameOK) {
                                        //
                                        // password
                                        //
                                        string Password = csData.getText("Password");
                                        if (Password.Trim() != Password) {
                                            Password = Password.Trim();
                                            updateUser = true;
                                        }
                                        if (string.IsNullOrEmpty(Password)) {
                                            for (Ptr = 0; Ptr <= 8; Ptr++) {
                                                int Index = encodeInteger(Microsoft.VisualBasic.VBMath.Rnd() * passwordChrsLength);
                                                Password = Password + strMid(passwordChrs, Index, 1);
                                            }
                                            updateUser = true;
                                        }
                                        Message += " password: " + Password + Environment.NewLine;
                                        result = true;
                                        if (updateUser) {
                                            csData.set("username", Username);
                                            csData.set("password", Password);
                                        }
                                        recordCnt = recordCnt + 1;
                                    }
                                    csData.goNext();
                                }
                            }
                        }
                    }
                }
                if (result) {
                    string sendStatus = "";
                    EmailController.queueAdHocEmail(core, "Password Email", core.session.user.id, workingEmail, FromAddress, subject, Message, "", "", "", true, false, 0, ref sendStatus);
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
            return result;
        }
        //
        //========================================================================
        //
        public static void processJoinForm(CoreController core) {
            try {
                string ErrorMessage = "";
                string FirstName = null;
                string LastName = null;
                string FullName = null;
                string Email = null;
                int errorCode = 0;
                //
                string loginForm_Username = "";
                string loginForm_Password = "";
                loginForm_Username = core.docProperties.getText("username");
                loginForm_Password = core.docProperties.getText("password");
                //
                if (!GenericController.encodeBoolean(core.siteProperties.getBoolean("AllowMemberJoin", false))) {
                    ErrorController.addUserError(core, "This site does not accept public main_MemberShip.");
                } else {
                    if (!core.session.isNewCredentialOK(loginForm_Username, loginForm_Password, ref ErrorMessage, ref errorCode)) {
                        ErrorController.addUserError(core, ErrorMessage);
                    } else {
                        if (!(!core.doc.userErrorList.Count.Equals(0))) {
                            using (var csData = new CsModel(core)) {
                                csData.open("people", "ID=" + DbController.encodeSQLNumber(core.session.user.id));
                                if (!csData.ok()) {
                                    LogController.logError(core, new Exception("Could not open the current members account to set the username and password."));
                                } else {
                                    if ((csData.getText("username") != "") || (csData.getText("password") != "") || (csData.getBoolean("admin")) || (csData.getBoolean("developer"))) {
                                        //
                                        // if the current account can be logged into, you can not join 'into' it
                                        //
                                        core.session.logout();
                                    }
                                    FirstName = core.docProperties.getText("firstname");
                                    LastName = core.docProperties.getText("lastname");
                                    FullName = FirstName + " " + LastName;
                                    Email = core.docProperties.getText("email");
                                    csData.set("FirstName", FirstName);
                                    csData.set("LastName", LastName);
                                    csData.set("Name", FullName);
                                    csData.set("username", loginForm_Username);
                                    csData.set("password", loginForm_Password);
                                    core.session.authenticateById(core.session.user.id, core.session);
                                }
                                csData.close();
                            }
                        }
                    }
                }
                PersonModel.invalidateCacheOfRecord<PersonModel>(core.cpParent, core.session.user.id);
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
        }
    }
}
