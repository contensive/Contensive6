using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Contensive.BaseClasses;

namespace Contensive.Processor.Controllers {
    public static class ExportDataRecordController {
        // 
        // ====================================================================================================
        // 
        public static string getNodeList(CPBaseClass cp, string DataRecordList, List<string> tempPathFileList, string tempExportPath) {
            try {
                string result = "";
                if (DataRecordList != "") {
                    result += System.Environment.NewLine + "\t" + "<DataRecordList>" + ExportController.EncodeCData( DataRecordList) + "</DataRecordList>";
                    string[] DataRecords = Strings.Split(DataRecordList, System.Environment.NewLine);
                    string RecordNodes = "";
                    for (var Ptr = 0; Ptr <= Information.UBound(DataRecords); Ptr++) {
                        string FieldNodes = "";
                        string DataRecordName = "";
                        string DataRecordGuid = "";
                        string DataRecord = DataRecords[Ptr];
                        if (DataRecord != "") {
                            string[] DataSplit = Strings.Split(DataRecord, ",");
                            if (Information.UBound(DataSplit) >= 0) {
                                string DataContentName = Strings.Trim(DataSplit[0]);
                                int DataContentId = cp.Content.GetID(DataContentName);
                                if (DataContentId <= 0)
                                    RecordNodes = ""
                                        + RecordNodes
                                        + System.Environment.NewLine + "\t" + "<!-- data missing, content not found during export, content=\"" + DataContentName + "\" guid=\"" + DataRecordGuid + "\" name=\"" + DataRecordName + "\" -->";
                                else {
                                    bool supportsGuid = cp.Content.IsField(DataContentName, "ccguid");
                                    string Criteria;
                                    if (Information.UBound(DataSplit) == 0)
                                        Criteria = "";
                                    else {
                                        string TestString = Strings.Trim(DataSplit[1]);
                                        if (TestString == "") {
                                            // 
                                            // blank is a select all
                                            // 
                                            Criteria = "";
                                            DataRecordName = "";
                                            DataRecordGuid = "";
                                        } else if (!supportsGuid) {
                                            // 
                                            // if no guid, this is name
                                            // 
                                            DataRecordName = TestString;
                                            DataRecordGuid = "";
                                            Criteria = "name=" + cp.Db.EncodeSQLText(DataRecordName);
                                        } else if ((Strings.Len(TestString) == 38) & (Strings.Left(TestString, 1) == "{") & (Strings.Right(TestString, 1) == "}")) {
                                            // 
                                            // guid {726ED098-5A9E-49A9-8840-767A74F41D01} format
                                            // 
                                            DataRecordGuid = TestString;
                                            DataRecordName = "";
                                            Criteria = "ccguid=" + cp.Db.EncodeSQLText(DataRecordGuid);
                                        } else if ((Strings.Len(TestString) == 36) & (Strings.Mid(TestString, 9, 1) == "-")) {
                                            // 
                                            // guid 726ED098-5A9E-49A9-8840-767A74F41D01 format
                                            // 
                                            DataRecordGuid = TestString;
                                            DataRecordName = "";
                                            Criteria = "ccguid=" + cp.Db.EncodeSQLText(DataRecordGuid);
                                        } else if ((Strings.Len(TestString) == 32) & (Strings.InStr(1, TestString, " ") == 0)) {
                                            // 
                                            // guid 726ED0985A9E49A98840767A74F41D01 format
                                            // 
                                            DataRecordGuid = TestString;
                                            DataRecordName = "";
                                            Criteria = "ccguid=" + cp.Db.EncodeSQLText(DataRecordGuid);
                                        } else {
                                            // 
                                            // use name
                                            // 
                                            DataRecordName = TestString;
                                            DataRecordGuid = "";
                                            Criteria = "name=" + cp.Db.EncodeSQLText(DataRecordName);
                                        }
                                    }
                                    using (CPCSBaseClass CSData = cp.CSNew()) {
                                        if (!CSData.Open(DataContentName, Criteria, "id"))
                                            RecordNodes = ""
+ RecordNodes
+ System.Environment.NewLine + "\t" + "<!-- data missing, record not found during export, content=\"" + DataContentName + "\" guid=\"" + DataRecordGuid + "\" name=\"" + DataRecordName + "\" -->";
                                        else {
                                            // 
                                            // determine all valid fields
                                            // 
                                            int fieldCnt = 0;
                                            string Sql = "select * from ccFields where contentid=" + DataContentId;

                                            string fieldLookupListValue = "";
                                            string[] fieldNames = Array.Empty<string>();
                                            int[] fieldTypes = Array.Empty<int>();
                                            string[] fieldLookupContent = Array.Empty<string>();
                                            string[] fieldLookupList = Array.Empty<string>();
                                            string FieldLookupContentName;
                                            int FieldTypeNumber;
                                            string FieldName;
                                            using (CPCSBaseClass csFields = cp.CSNew()) {
                                                if (csFields.Open("content fields", "contentid=" + DataContentId)) {
                                                    do {
                                                        FieldName = csFields.GetText("name");
                                                        if (FieldName != "") {
                                                            int FieldLookupContentID = 0;
                                                            FieldLookupContentName = "";
                                                            FieldTypeNumber = csFields.GetInteger("type");
                                                            switch (Strings.LCase(FieldName)) {
                                                                case "ccguid":
                                                                case "name":
                                                                case "id":
                                                                case "dateadded":
                                                                case "createdby":
                                                                case "modifiedby":
                                                                case "modifieddate":
                                                                case "createkey":
                                                                case "contentcontrolid":
                                                                case "editsourceid":
                                                                case "editarchive":
                                                                case "editblank":
                                                                case "contentcategoryid": {
                                                                        break;
                                                                    }

                                                                default: {
                                                                        if (FieldTypeNumber == 7) {
                                                                            FieldLookupContentID = csFields.GetInteger("Lookupcontentid");
                                                                            fieldLookupListValue = csFields.GetText("LookupList");
                                                                            if (FieldLookupContentID != 0)
                                                                                FieldLookupContentName = cp.Content.GetRecordName("content", FieldLookupContentID);
                                                                        }
                                                                        //CPContentBaseClass.FieldTypeIdEnum.File
                                                                        switch (FieldTypeNumber) {
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.File:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.FileImage:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.Lookup:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.Boolean:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.FileCSS:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.FileJavascript:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.FileText:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.FileXML:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.Currency:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.Float:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.Integer:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.Date:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.Link:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.LongText:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.ResourceLink:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.Text:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.HTML:
                                                                            case (int)CPContentBaseClass.FieldTypeIdEnum.FileHTML: {
                                                                                    var oldFieldNames = fieldNames;
                                                                                    fieldNames = new string[fieldCnt + 1];
                                                                                    // 
                                                                                    // this is a keeper
                                                                                    // 
                                                                                    if (oldFieldNames != null)
                                                                                        Array.Copy(oldFieldNames, fieldNames, Math.Min(fieldCnt + 1, oldFieldNames.Length));
                                                                                    var oldFieldTypes = fieldTypes;
                                                                                    fieldTypes = new int[fieldCnt + 1];
                                                                                    if (oldFieldTypes != null)
                                                                                        Array.Copy(oldFieldTypes, fieldTypes, Math.Min(fieldCnt + 1, oldFieldTypes.Length));
                                                                                    var oldFieldLookupContent = fieldLookupContent;
                                                                                    fieldLookupContent = new string[fieldCnt + 1];
                                                                                    if (oldFieldLookupContent != null)
                                                                                        Array.Copy(oldFieldLookupContent, fieldLookupContent, Math.Min(fieldCnt + 1, oldFieldLookupContent.Length));
                                                                                    var oldFieldLookupList = fieldLookupList;
                                                                                    fieldLookupList = new string[fieldCnt + 1];
                                                                                    if (oldFieldLookupList != null)
                                                                                        Array.Copy(oldFieldLookupList, fieldLookupList, Math.Min(fieldCnt + 1, oldFieldLookupList.Length));
                                                                                    // fieldLookupContent
                                                                                    fieldNames[fieldCnt] = FieldName;
                                                                                    fieldTypes[fieldCnt] = FieldTypeNumber;
                                                                                    fieldLookupContent[fieldCnt] = FieldLookupContentName;
                                                                                    fieldLookupList[fieldCnt] = fieldLookupListValue;
                                                                                    fieldCnt = fieldCnt + 1;
                                                                                    break;
                                                                                }
                                                                        }

                                                                        break;
                                                                    }
                                                            }
                                                        }

                                                        csFields.GoNext();
                                                    }
                                                    while (csFields.OK());
                                                }
                                                csFields.Close();
                                            }
                                            // 
                                            // output records
                                            // 
                                            DataRecordGuid = "";
                                            while (CSData.OK()) {
                                                FieldNodes = "";
                                                DataRecordName = CSData.GetText("name");
                                                if (supportsGuid) {
                                                    DataRecordGuid = CSData.GetText("ccguid");
                                                    if (DataRecordGuid == "") {
                                                        DataRecordGuid = cp.Utils.CreateGuid();
                                                        CSData.SetField("ccGuid", DataRecordGuid);
                                                    }
                                                }
                                                int fieldPtr;
                                                for (fieldPtr = 0; fieldPtr <= fieldCnt - 1; fieldPtr++) {
                                                    FieldName = fieldNames[fieldPtr];
                                                    FieldTypeNumber = cp.Utils.EncodeInteger(fieldTypes[fieldPtr]);
                                                    // Dim ContentID As Integer
                                                    string FieldValue;
                                                    switch (FieldTypeNumber) {
                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.File:
                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.FileImage: {
                                                                // 
                                                                // files -- copy pathFilename to tmp folder and save pathFilename to fieldValue
                                                                FieldValue = CSData.GetText(FieldName).ToString();
                                                                if ((!string.IsNullOrWhiteSpace(FieldValue))) {
                                                                    string pathFilename = FieldValue;
                                                                    cp.CdnFiles.Copy(pathFilename, tempExportPath + pathFilename, cp.TempFiles);
                                                                    if (!tempPathFileList.Contains(tempExportPath + pathFilename)) {
                                                                        tempPathFileList.Add(tempExportPath + pathFilename);
                                                                        string path =  FileController.getPath(pathFilename);
                                                                        string filename = FileController.getFilename(pathFilename);
                                                                        result += System.Environment.NewLine + "\t" + "<Resource name=\"" + System.Net.WebUtility.HtmlEncode(filename) + "\" type=\"content\" path=\"" + System.Net.WebUtility.HtmlEncode(path) + "\" />";
                                                                    }
                                                                }

                                                                break;
                                                            }

                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.Boolean: {
                                                                // 
                                                                // true/false
                                                                // 
                                                                FieldValue = CSData.GetBoolean(FieldName).ToString();
                                                                break;
                                                            }

                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.FileCSS:
                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.FileJavascript:
                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.FileText:
                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.FileXML: {
                                                                // 
                                                                // text files
                                                                // 
                                                                FieldValue = CSData.GetText(FieldName);
                                                                FieldValue = ExportController.EncodeCData( FieldValue);
                                                                break;
                                                            }

                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.Integer: {
                                                                // 
                                                                // integer
                                                                // 
                                                                FieldValue = CSData.GetInteger(FieldName).ToString();
                                                                break;
                                                            }

                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.Currency:
                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.Float: {
                                                                // 
                                                                // numbers
                                                                // 
                                                                FieldValue = CSData.GetNumber(FieldName).ToString();
                                                                break;
                                                            }

                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.Date: {
                                                                // 
                                                                // date
                                                                // 
                                                                FieldValue = CSData.GetDate(FieldName).ToString();
                                                                break;
                                                            }

                                                        case (int)CPContentBaseClass.FieldTypeIdEnum.Lookup: {
                                                                // 
                                                                // lookup
                                                                // 
                                                                FieldValue = "";
                                                                int FieldValueInteger = CSData.GetInteger(FieldName);
                                                                if ((FieldValueInteger != 0)) {
                                                                    FieldLookupContentName = fieldLookupContent[fieldPtr];
                                                                    fieldLookupListValue = fieldLookupList[fieldPtr];
                                                                    if ((FieldLookupContentName != "")) {
                                                                        // 
                                                                        // content lookup
                                                                        // 
                                                                        if (cp.Content.IsField(FieldLookupContentName, "ccguid")) {
                                                                            using (CPCSBaseClass CSlookup = cp.CSNew()) {
                                                                                CSlookup.OpenRecord(FieldLookupContentName, FieldValueInteger);
                                                                                if (CSlookup.OK()) {
                                                                                    FieldValue = CSlookup.GetText("ccguid");
                                                                                    if (FieldValue == "") {
                                                                                        FieldValue = cp.Utils.CreateGuid();
                                                                                        CSlookup.SetField("ccGuid", FieldValue);
                                                                                    }
                                                                                }
                                                                                CSlookup.Close();
                                                                            }
                                                                        }
                                                                    } else if (fieldLookupListValue != "")
                                                                        // 
                                                                        // list lookup, ok to save integer
                                                                        // 
                                                                        FieldValue = FieldValueInteger.ToString();
                                                                }

                                                                break;
                                                            }

                                                        default: {
                                                                // 
                                                                // text types
                                                                // 
                                                                FieldValue = CSData.GetText(FieldName);
                                                                FieldValue = ExportController.EncodeCData( FieldValue);
                                                                break;
                                                            }
                                                    }
                                                    FieldNodes = FieldNodes + System.Environment.NewLine + "\t" + "<field name=\"" + System.Net.WebUtility.HtmlEncode(FieldName) + "\">" + FieldValue + "</field>";
                                                }
                                                RecordNodes = ""
                                                        + RecordNodes
                                                        + System.Environment.NewLine + "\t" + "<record content=\"" + System.Net.WebUtility.HtmlEncode(DataContentName) + "\" guid=\"" + DataRecordGuid + "\" name=\"" + System.Net.WebUtility.HtmlEncode(DataRecordName) + "\">"
                                                        + ExportController.tabIndent(cp, FieldNodes)
                                                        + System.Environment.NewLine + "\t" + "</record>";
                                                CSData.GoNext();
                                            }
                                        }
                                        CSData.Close();
                                    }
                                }
                            }
                        }
                    }
                    if (RecordNodes != "")
                        result = ""
+ result
+ System.Environment.NewLine + "\t" + "<data>"
+ ExportController.tabIndent(cp, RecordNodes)
+ System.Environment.NewLine + "\t" + "</data>";
                }
                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return string.Empty;
            }
        }
    }
}
