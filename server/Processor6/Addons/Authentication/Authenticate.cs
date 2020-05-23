//
using System;
using System.Collections.Generic;
using Contensive.Processor.Controllers;
using System.Text;
using Contensive.Models.Db;
using System.Globalization;
//
namespace Contensive.Processor.Addons.Primitives {
    public class AuthenticateClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// process a username/password authentication with no success result.
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            string result = "";
            try {
                CoreController core = ((CPClass)cp).core;
                //
                // -- start with a logout if logged in
                if ((core.session.isAuthenticated) || (core.session.isRecognized())) { core.session.logout(); }
                if (core.session.visit.loginAttempts >= core.siteProperties.maxVisitLoginAttempts) {
                    //
                    // -- too many attempts
                    return new AuthenticateResponse {
                        errors = new List<string> { "Authentication failed." },
                        data = new AuthenticateResponseData()
                    };
                }
                //
                // -- count the login attempt
                core.session.visit.loginAttempts++;
                core.session.visit.save(core.cpParent);
                //
                // -- test for username/password authentication
                {
                    string username = core.docProperties.getText("username");
                    string password = core.docProperties.getText("password");
                    if ((!string.IsNullOrWhiteSpace(username)) && (!string.IsNullOrWhiteSpace(password))) {
                        //
                        // -- username and password provided, attempt username login
                        return authenticateUsernamePassword(core, username, password, "Username/Password Login");
                    }
                }
                //
                // -- test for basic username/password authentication
                string basicAuthentication = core.docProperties.getText("authorization");
                if ((!string.IsNullOrWhiteSpace(basicAuthentication)) && (basicAuthentication.Length > 7) && (basicAuthentication.Substring(0, 6).ToLower(CultureInfo.InvariantCulture) == "basic ")) {
                    string usernamePasswordEncoded = basicAuthentication.Substring(6);
                    byte[] usernamePasswordBytes = Convert.FromBase64String(usernamePasswordEncoded);
                    string[] usernamePassword = Encoding.ASCII.GetString(usernamePasswordBytes).Split(':');
                    if (usernamePassword.Length != 2) {
                        cp.Response.SetStatus(WebServerController.httpResponseStatus401_Unauthorized);
                        return new AuthenticateResponse {
                            errors = new List<string> { "Basic Authentication failed." },
                            data = new AuthenticateResponseData()
                        };
                    }
                    string username = usernamePassword[0];
                    string password = usernamePassword[1];
                    return authenticateUsernamePassword(core, username, password, "Basic Authentication");
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
        //
        public static AuthenticateResponse authenticateUsernamePassword(CoreController core, string username, string password, string errorPrefix) {
            int userId = core.session.getUserIdForUsernameCredentials(username, password);
            if (userId == 0) {
                //
                // -- user was not found
                core.webServer.setResponseStatus(WebServerController.httpResponseStatus401_Unauthorized);
                return new AuthenticateResponse {
                    errors = new List<string> { errorPrefix + " failed." },
                    data = new AuthenticateResponseData()
                };
            } else {
                if (!core.session.authenticateById(userId, core.session)) {
                    //
                    // -- username/password login failed
                    core.webServer.setResponseStatus(WebServerController.httpResponseStatus401_Unauthorized);
                    return new AuthenticateResponse {
                        errors = new List<string> { errorPrefix + " failed." },
                        data = new AuthenticateResponseData()
                    };
                } else {
                    var user = DbBaseModel.create<PersonModel>(core.cpParent, core.session.user.id);
                    if (user == null) {
                        core.webServer.setResponseStatus(WebServerController.httpResponseStatus401_Unauthorized);
                        return new AuthenticateResponse {
                            errors = new List<string> { errorPrefix + " user is not valid." },
                            data = new AuthenticateResponseData()
                        };
                    } else {
                        LogController.addSiteActivity(core, errorPrefix + " successful", core.session.user.id, core.session.user.organizationId);
                        return new AuthenticateResponse {
                            errors = new List<string>(),
                            data = new AuthenticateResponseData {
                                firstName = user.firstName,
                                lastName = user.lastName,
                                email = user.email,
                                avatar = (!string.IsNullOrWhiteSpace(user.thumbnailFilename)) ? core.appConfig.cdnFileUrl + user.thumbnailFilename : (!string.IsNullOrWhiteSpace(user.imageFilename)) ? core.appConfig.cdnFileUrl + user.imageFilename : ""
                            }
                        };
                    }
                }
            }

        }
        //
        public class AuthenticateResponse {
            public AuthenticateResponseData data = new AuthenticateResponseData();
            public List<string> errors = new List<string>();

        }
        //
        public class AuthenticateResponseData {
            public string firstName;
            public string lastName;
            public string email;
            public string avatar;
        }
    }
}
