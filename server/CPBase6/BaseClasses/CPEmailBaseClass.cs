
using System;
using System.Collections.Generic;

namespace Contensive.BaseClasses {
    public abstract class CPEmailBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// Returns the site's default email from address
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract string fromAddressDefault { get; }
        //
        //====================================================================================================
        /// <summary>
        /// Sends an email to an email address. Return false if the email could not be sent
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="fromAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="sendImmediately"></param>
        /// <param name="bodyIsHTML"></param>
        /// <param name="userErrorMessage"></param>
        /// <remarks></remarks>
        public abstract void send(string toAddress, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML, ref string userErrorMessage);
        public abstract void send(string toAddress, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML);
        public abstract void send(string toAddress, string fromAddress, string subject, string body, bool sendImmediately);
        public abstract void send(string toAddress, string fromAddress, string subject, string body);
        //
        //====================================================================================================
        /// <summary>
        /// Sends an email that includes all the form elements in the current webpage response.
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="fromAddress"></param>
        /// <param name="subject"></param>
        /// <param name="userErrorMessage"></param>
        /// <remarks></remarks>
        public abstract void sendForm(string toAddress, string fromAddress, string subject, ref string userErrorMessage);
        public abstract void sendForm(string toAddress, string fromAddress, string subject);
        //
        //====================================================================================================
        /// <summary>
        /// Send a list of usernames and passwords to the account(s) that include the given email address. If false, the email could not be sent.
        /// </summary>
        /// <param name="userEmailAddress"></param>
        /// <remarks></remarks>
        public abstract void sendPassword(string userEmailAddress, ref string userErrorMessage);
        public abstract void sendPassword(string userEmailAddress);
        //
        //====================================================================================================
        /// <summary>
        /// Send a system email record. If the EmailIdOrName field contains a number, it is assumed first to be an Id. If false, the email could not be sent
        /// </summary>
        /// <param name="emailName"></param>
        /// <param name="additionalCopy"></param>
        /// <param name="additionalUserID"></param>
        /// <remarks></remarks>
        public abstract void sendSystem(string emailName, string additionalCopy, int additionalUserID, ref string userErrorMessage);
        public abstract void sendSystem(string emailName, string additionalCopy, int additionalUserID);
        public abstract void sendSystem(string emailName, string additionalCopy);
        public abstract void sendSystem(string emailName);
        //
        public abstract void sendSystem(int emailId, string additionalCopy, int additionalUserID, ref string userErrorMessage);
        public abstract void sendSystem(int emailId, string additionalCopy, int additionalUserID);
        public abstract void sendSystem(int emailId, string additionalCopy);
        public abstract void sendSystem(int emailId);
        //
        //====================================================================================================
        /// <summary>
        /// Sends an email to everyone in a group list. The list can be of Group Ids or names. Group names in the list can not contain commas.
        /// </summary>
        /// <param name="groupNameOrIdList"></param>
        /// <param name="fromAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="sendImmediately"></param>
        /// <param name="bodyIsHTML"></param>
        /// <remarks></remarks>
        //
        public abstract void sendGroup(string groupName, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML, ref string userErrorMessage);
        public abstract void sendGroup(string groupName, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML);
        public abstract void sendGroup(string groupName, string fromAddress, string subject, string body, bool sendImmediately);
        public abstract void sendGroup(string groupName, string fromAddress, string subject, string body);
        //
        public abstract void sendGroup(int groupId, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML, ref string userErrorMessage);
        public abstract void sendGroup(int groupId, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML);
        public abstract void sendGroup(int groupId, string fromAddress, string subject, string body, bool sendImmediately);
        public abstract void sendGroup(int groupId, string fromAddress, string subject, string body);
        //
        public abstract void sendGroup(List<string> groupNameList, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML, ref string userErrorMessage);
        public abstract void sendGroup(List<string> groupNameList, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML);
        public abstract void sendGroup(List<string> groupNameList, string fromAddress, string subject, string body, bool sendImmediately);
        public abstract void sendGroup(List<string> groupNameList, string fromAddress, string subject, string body);
        //
        public abstract void sendGroup(List<int> groupIdList, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML, ref string userErrorMessage);
        public abstract void sendGroup(List<int> groupIdList, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML);
        public abstract void sendGroup(List<int> groupIdList, string fromAddress, string subject, string body, bool sendImmediately);
        public abstract void sendGroup(List<int> groupIdList, string fromAddress, string subject, string body);
        //
        //====================================================================================================
        /// <summary>
        /// Send an email using the values in a user record.
        /// </summary>
        /// <param name="toUserId"></param>
        /// <param name="fromAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="sendImmediately"></param>
        /// <param name="bodyIsHTML"></param>
        /// <remarks></remarks>
        public abstract void sendUser(int toUserId, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML, ref string userErrorMessage);
        public abstract void sendUser(int toUserId, string fromAddress, string subject, string body, bool sendImmediately, bool bodyIsHTML);
        public abstract void sendUser(int toUserId, string fromAddress, string subject, string body, bool sendImmediately);
        public abstract void sendUser(int toUserId, string fromAddress, string subject, string body);
        //
        public abstract bool validateEmail(string toAddress);
        //
        public abstract bool validateUserEmail(int toUserId);
        //
        //====================================================================================================
        // deprecated
        //
        //
        [Obsolete("user setUser with argument int toUserId", false)]
        public abstract void sendUser(string ToUserID, string FromAddress, string Subject, string Body, bool SendImmediately, bool BodyIsHTML);
        //
        [Obsolete("user setUser with argument int toUserId", false)]
        public abstract void sendUser(string ToUserID, string FromAddress, string Subject, string Body, bool SendImmediately);
        //
        [Obsolete("user setUser with argument int toUserId", false)]
        public abstract void sendUser(string ToUserID, string FromAddress, string Subject, string Body);
        //[Obsolete("Use uppercase version", false)]
        //public abstract string fromAddressDefault { get; }
        ////
        //[Obsolete("Use uppercase version", false)]
        //public abstract void send(string ToAddress, string FromAddress, string Subject, string Body, bool SendImmediately, bool BodyIsHTML);
        ////
        //[Obsolete("Use uppercase version", false)]
        //public abstract void send(string ToAddress, string FromAddress, string Subject, string Body, bool SendImmediately);
        ////
        //[Obsolete("Use uppercase version", false)]
        //public abstract void send(string ToAddress, string FromAddress, string Subject, string Body);
        ////
        //[Obsolete("Use uppercase version", false)]
        //public abstract void sendForm(string ToAddress, string FromAddress, string Subject);
        //
        //[Obsolete("Use SendToGroup()", false)]
        //public abstract void sendGroup(string GroupNameOrIdList, string FromAddress, string Subject, string Body, bool SendImmediately, bool BodyIsHTML);
        ////
        //[Obsolete("Use SendToGroup()", false)]
        //public abstract void sendGroup(string GroupNameOrIdList, string FromAddress, string Subject, string Body, bool SendImmediately);
        ////
        //[Obsolete("Use SendToGroup()", false)]
        //public abstract void sendGroup(string GroupNameOrIdList, string FromAddress, string Subject, string Body);

        ////
        //[Obsolete("Use uppercase version", false)]
        //public abstract void sendPassword(string UserEmailAddress);
        ////
        //[Obsolete("Use uppercase version", false)]
        //public abstract void sendSystem(string EmailIdOrName, string AdditionalCopy = "", int AdditionalUserID = 0);
    }
}

