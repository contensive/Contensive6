
using System;
using Contensive.Processor.Controllers;
using Contensive.Processor.Models.Domain;
//
namespace Contensive.Processor.Addons.Housekeeping {
    //
    //====================================================================================================
    /// <summary>
    /// housekeep environment, to facilitate argument passing
    /// </summary>
    public class HouseKeepEnvironmentModel {
        public bool forceHousekeep { get; set; }
        public bool runDailyTasks { get; set; }
        public DateTime lastCheckDateTime { get; set; }
        public int serverHousekeepHour { get; set; }
        public DateTime yesterday { get; set; }
        public DateTime aLittleWhileAgo { get; set; }
        public DateTime oldestVisitSummaryWeCareAbout { get; set; }
        public int visitArchiveAgeDays { get; set; }
        public DateTime visitArchiveDate { get; set; }
        public string defaultMemberName { get; set; }
        public int guestArchiveAgeDays { get; set; }
        /// <summary>
        /// how many days the email drop and email log data are kept
        /// </summary>
        public int emailDropArchiveAgeDays { get; set; }
        /// <summary>
        /// How many days the email log stores the email body (large data)
        /// </summary>
        public int emailLogBodyRetainDays { get; set; }
        public bool archiveDeleteNoCookie { get; set; }
        public string sqlDateMidnightTwoDaysAgo { get; set; }
        public DateTime midnightTwoDaysAgo { get; set; }
        public DateTime thirtyDaysAgo { get; set; }
        public bool archiveAlarm { get; set; }
        //
        public HouseKeepEnvironmentModel(CoreController core) {
            try {
                archiveAlarm = false;
                lastCheckDateTime = core.siteProperties.getDate("housekeep, last check", default);
                core.siteProperties.setProperty("housekeep, last check", core.dateTimeNowMockable);
                forceHousekeep = core.docProperties.getBoolean("force");
                serverHousekeepHour = core.siteProperties.getInteger("housekeep, run time hour", 2);
                runDailyTasks = ((core.dateTimeNowMockable.Date > lastCheckDateTime.Date) && (serverHousekeepHour < core.dateTimeNowMockable.Hour));
                yesterday = core.dateTimeNowMockable.AddDays(-1).Date;
                aLittleWhileAgo = core.dateTimeNowMockable.AddDays(-90).Date;
                defaultMemberName = ContentFieldMetadataModel.getDefaultValue(core, "people", "name");
                archiveDeleteNoCookie = core.siteProperties.getBoolean("ArchiveDeleteNoCookie", true);
                sqlDateMidnightTwoDaysAgo = DbController.encodeSQLDate(midnightTwoDaysAgo);
                midnightTwoDaysAgo = core.dateTimeNowMockable.AddDays(-2).Date;
                thirtyDaysAgo = core.dateTimeNowMockable.AddDays(-30).Date;
                //
                // -- Get ArchiveAgeDays - use this as the oldest data they care about
                visitArchiveAgeDays = core.siteProperties.getInteger("ArchiveRecordAgeDays", 2);
                if (visitArchiveAgeDays < 2) {
                    visitArchiveAgeDays = 2;
                    core.siteProperties.setProperty("ArchiveRecordAgeDays", 2);
                }
                visitArchiveDate = core.dateTimeNowMockable.AddDays(-visitArchiveAgeDays).Date;
                oldestVisitSummaryWeCareAbout = core.dateTimeNowMockable.Date.AddDays(-30);
                if (oldestVisitSummaryWeCareAbout < visitArchiveDate) {
                    oldestVisitSummaryWeCareAbout = visitArchiveDate;
                }
                //
                // -- Get GuestArchiveAgeDays
                guestArchiveAgeDays = core.siteProperties.getInteger("ArchivePeopleAgeDays", 2);
                if (guestArchiveAgeDays < 2) {
                    guestArchiveAgeDays = 2;
                    core.siteProperties.setProperty("ArchivePeopleAgeDays", guestArchiveAgeDays);
                }
                //
                // -- Get EmailDropArchiveAgeDays
                emailDropArchiveAgeDays = core.siteProperties.getInteger("ArchiveEmailDropAgeDays", 90);
                if (emailDropArchiveAgeDays < 2) {
                    emailDropArchiveAgeDays = 2;
                    core.siteProperties.setProperty("ArchiveEmailDropAgeDays", emailDropArchiveAgeDays);
                }
                if (emailDropArchiveAgeDays > 365) {
                    emailDropArchiveAgeDays = 365;
                    core.siteProperties.setProperty("ArchiveEmailDropAgeDays", emailDropArchiveAgeDays);
                }
                //
                // -- Get emailLogBodyRetainDays -- 
                emailLogBodyRetainDays = GenericController.encodeInteger(core.siteProperties.getText("EmailLogBodyRetainDays", "7"));
                //
                defaultMemberName = ContentFieldMetadataModel.getDefaultValue(core, "people", "name");
                //
                // Check for site's archive time of day
                //
                string AlarmTimeString = core.siteProperties.getText("ArchiveTimeOfDay", "12:00:00 AM");
                if (string.IsNullOrEmpty(AlarmTimeString)) {
                    AlarmTimeString = "12:00:00 AM";
                    core.siteProperties.setProperty("ArchiveTimeOfDate", AlarmTimeString);
                }
                if (!GenericController.isDate(AlarmTimeString)) {
                    AlarmTimeString = "12:00:00 AM";
                    core.siteProperties.setProperty("ArchiveTimeOfDate", AlarmTimeString);
                }
                double minutesSinceMidnight = core.dateTimeNowMockable.TimeOfDay.TotalMinutes;
                double LastCheckMinutesFromMidnight = lastCheckDateTime.TimeOfDay.TotalMinutes;
                if ((minutesSinceMidnight > LastCheckMinutesFromMidnight) && (LastCheckMinutesFromMidnight < minutesSinceMidnight)) {
                    //
                    // Same Day - Midnight is before last and after current
                    //
                    archiveAlarm = true;
                } else if ((LastCheckMinutesFromMidnight > minutesSinceMidnight) && ((LastCheckMinutesFromMidnight < minutesSinceMidnight))) {
                    //
                    // New Day - Midnight is between Last and Set
                    //
                    archiveAlarm = true;
                }

            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
        }
    }
}