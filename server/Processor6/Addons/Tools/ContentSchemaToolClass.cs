﻿
using System;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Controllers.GenericController;
using static Contensive.Processor.Constants;
//
namespace Contensive.Processor.Addons.Tools {
    //
    public class ContentSchemaToolClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// addon method, deliver complete Html admin site
        /// blank return on OK or cancel button
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cpBase) {
            return get(((CPClass)cpBase).core);
        }
        //=============================================================================
        //   Print the manual query form
        //=============================================================================
        //
        public static string get(CoreController core ) {
            string result = null;
            try {
                //
                int TableColSpan = 0;
                bool TableEvenRow = false;
                string SQL = null;
                string TableName = null;
                string ButtonList;
                //
                ButtonList = ButtonCancel;
                result = AdminUIController.getHeaderTitleDescription("Get Content Database Schema", "This tool displays all tables and fields required for the current Content Defintions.");
                //
                TableColSpan = 3;
                result += HtmlController.tableStart(2, 0, 0);
                SQL = "SELECT DISTINCT ccTables.Name as TableName, ccFields.Name as FieldName, ccFieldTypes.Name as FieldType"
                        + " FROM ((ccContent LEFT JOIN ccTables ON ccContent.ContentTableId = ccTables.ID) LEFT JOIN ccFields ON ccContent.Id = ccFields.ContentID) LEFT JOIN ccFieldTypes ON ccFields.Type = ccFieldTypes.ID"
                        + " ORDER BY ccTables.Name, ccFields.Name;";
                using (var csData = new CsModel(core)) {
                    csData.openSql(SQL);
                    TableName = "";
                    while (csData.ok()) {
                        if (TableName != csData.getText("TableName")) {
                            TableName = csData.getText("TableName");
                            result += HtmlController.tableRow("<B>" + TableName + "</b>", TableColSpan, TableEvenRow);
                        }
                        result += HtmlController.tableRowStart();
                        result += HtmlController.td("&nbsp;", "", 0, TableEvenRow);
                        result += HtmlController.td(csData.getText("FieldName"), "", 0, TableEvenRow);
                        result += HtmlController.td(csData.getText("FieldType"), "", 0, TableEvenRow);
                        result += kmaEndTableRow;
                        TableEvenRow = !TableEvenRow;
                        csData.goNext();
                    }
                }
                //
                // Field Type Definitions
                //
                result += HtmlController.tableRow("<br><br><B>Field Type Definitions</b>", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("Boolean - Boolean values 0 and 1 are stored in a database long integer field type", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("Lookup - References to related records stored as database long integer field type", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("Integer - database long integer field type", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("Float - database floating point value", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("Date - database DateTime field type.", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("AutoIncrement - database long integer field type. Field automatically increments when a record is added.", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("Text - database character field up to 255 characters.", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("LongText - database character field up to 64K characters.", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("TextFile - references a filename in the Content Files folder. Database character field up to 255 characters. ", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("File - references a filename in the Content Files folder. Database character field up to 255 characters. ", TableColSpan, TableEvenRow);
                result += HtmlController.tableRow("Redirect - This field has no database equivelent. No Database field is required.", TableColSpan, TableEvenRow);
                //
                // Spacers
                //
                result += HtmlController.tableRowStart();
                result += HtmlController.td(nop2(20, 1), "20");
                result += HtmlController.td(nop2(300, 1), "300");
                result += HtmlController.td("&nbsp;", "100%");
                result += kmaEndTableRow;
                result += kmaEndTable;
                //
                result = AdminUIController.getToolForm(core, result, ButtonList);
                //
                // ----- Error Trap
                //
            } catch (Exception ex) {
                LogController.logError(core, ex);
            }
            return result;
        }
        //
    }
}

