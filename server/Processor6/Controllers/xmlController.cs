

using System.Xml;
using static Contensive.Processor.Controllers.GenericController;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;
using Contensive.BaseClasses;

namespace Contensive.Processor.Controllers {
    public class XmlController {
        private int iBusy;
        private int iTaskCount;
        const string ApplicationNameLocal = "unknown";
        private CPBaseClass cp;
        // 
        // ----- This should match the Lookup List in the NavIconType field in the Navigator Entry content definition
        // 
        public const int NavTypeIDAddon = 1;
        public const int NavTypeIDReport = 2;
        public const int NavTypeIDSetting = 3;
        public const int NavTypeIDTool = 4;
        // 
        public const string NavIconTypeList = "Custom,Advanced,Content,Folder,Email,User,Report,Setting,Tool,Record,Addon,help";
        public const int NavIconTypeCustom = 1;
        public const int NavIconTypeAdvanced = 2;
        public const int NavIconTypeContent = 3;
        public const int NavIconTypeFolder = 4;
        public const int NavIconTypeEmail = 5;
        public const int NavIconTypeUser = 6;
        public const int NavIconTypeReport = 7;
        public const int NavIconTypeSetting = 8;
        public const int NavIconTypeTool = 9;
        public const int NavIconTypeRecord = 10;
        public const int NavIconTypeAddon = 11;
        public const int NavIconTypeHelp = 12;
        // 
        public const string FieldDescriptorInteger = "Integer";
        public const string FieldDescriptorText = "Text";
        public const string FieldDescriptorLongText = "LongText";
        public const string FieldDescriptorBoolean = "Boolean";
        public const string FieldDescriptorDate = "Date";
        public const string FieldDescriptorFile = "File";
        public const string FieldDescriptorLookup = "Lookup";
        public const string FieldDescriptorRedirect = "Redirect";
        public const string FieldDescriptorCurrency = "Currency";
        public const string FieldDescriptorImage = "Image";
        public const string FieldDescriptorFloat = "Float";
        public const string FieldDescriptorManyToMany = "ManyToMany";
        public const string FieldDescriptorTextFile = "TextFile";
        public const string FieldDescriptorCSSFile = "CSSFile";
        public const string FieldDescriptorXMLFile = "XMLFile";
        public const string FieldDescriptorJavascriptFile = "JavascriptFile";
        public const string FieldDescriptorLink = "Link";
        public const string FieldDescriptorResourceLink = "ResourceLink";
        public const string FieldDescriptorMemberSelect = "MemberSelect";
        public const string FieldDescriptorHTML = "HTML";
        public const string FieldDescriptorHTMLFile = "HTMLFile";
        // 
        public const string FieldDescriptorLcaseInteger = "integer";
        public const string FieldDescriptorLcaseText = "text";
        public const string FieldDescriptorLcaseLongText = "longtext";
        public const string FieldDescriptorLcaseBoolean = "boolean";
        public const string FieldDescriptorLcaseDate = "date";
        public const string FieldDescriptorLcaseFile = "file";
        public const string FieldDescriptorLcaseLookup = "lookup";
        public const string FieldDescriptorLcaseRedirect = "redirect";
        public const string FieldDescriptorLcaseCurrency = "currency";
        public const string FieldDescriptorLcaseImage = "image";
        public const string FieldDescriptorLcaseFloat = "float";
        public const string FieldDescriptorLcaseManyToMany = "manytomany";
        public const string FieldDescriptorLcaseTextFile = "textfile";
        public const string FieldDescriptorLcaseCSSFile = "cssfile";
        public const string FieldDescriptorLcaseXMLFile = "xmlfile";
        public const string FieldDescriptorLcaseJavascriptFile = "javascriptfile";
        public const string FieldDescriptorLcaseLink = "link";
        public const string FieldDescriptorLcaseResourceLink = "resourcelink";
        public const string FieldDescriptorLcaseMemberSelect = "memberselect";
        public const string FieldDescriptorLcaseHTML = "html";
        public const string FieldDescriptorLcaseHTMLFile = "htmlfile";
        // 
        // ====================================================================================================
        /// <summary>
        ///         ''' constructor
        ///         ''' </summary>
        ///         ''' <param name="cp"></param>
        ///         ''' <remarks></remarks>
        public XmlController(CPBaseClass cp) {
            this.cp = cp;
        }
        // 
        // ====================================================================================================
        private class tableClass {
            public string tableName;
            public string dataSourceName;
        }
        // 
        // ====================================================================================================
        public string GetXMLContentDefinition3(string ContentName = "", bool IncludeBaseFields = false) {
            try {
                // 
                const string ContentSelectList = ""
                    + " id,name,active,adminonly,allowadd"
                    + ",allowcalendarevents,allowcontentchildtool,allowcontenttracking,allowdelete,allowmetacontent"
                    + ",allowtopicrules,AllowWorkflowAuthoring,AuthoringTableID"
                    + ",ContentTableID,DefaultSortMethodID,DeveloperOnly,DropDownFieldList"
                    + ",EditorGroupID,ParentID,ccGuid,IsBaseContent"
                    + ",IconLink,IconHeight,IconWidth,IconSprites";

                const string FieldSelectList = ""
                    + "f.ID,f.Name,f.contentid,f.Active,f.AdminOnly,f.Authorable,f.Caption,f.DeveloperOnly,f.EditSortPriority,f.Type,f.HTMLContent"
                    + ",f.IndexColumn,f.IndexSortDirection,f.IndexSortPriority,f.RedirectID,f.RedirectPath,f.Required"
                    + ",f.TextBuffered,f.UniqueName,f.DefaultValue,f.RSSTitleField,f.RSSDescriptionField,f.MemberSelectGroupID"
                    + ",f.EditTab,f.Scramble,f.LookupList,f.NotEditable,f.Password,f.readonly,f.ManyToManyRulePrimaryField"
                    + ",f.ManyToManyRuleSecondaryField,'' as HelpMessageDeprecated,f.ModifiedBy,f.IsBaseField,f.LookupContentID"
                    + ",f.RedirectContentID,f.ManyToManyContentID,f.ManyToManyRuleContentID"
                    + ",h.helpdefault,h.helpcustom,f.IndexWidth";

                // 
                bool IsBaseContent;
                int FieldCnt = 0;
                string FieldName;
                int FieldContentID;
                int LastFieldID;
                int RecordID;
                string RecordName;
                int AuthoringTableID;
                string HelpDefault;
                int HelpCnt;
                int fieldId;
                string fieldType;
                int ContentTableID;
                string TableName;
                string DataSourceName;
                int DefaultSortMethodID;
                string DefaultSortMethod;
                int EditorGroupID;
                string EditorGroupName;
                int ParentID;
                string ParentName;
                int ContentID = 0;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                string iContentName;
                string SQL;
                bool FoundMenuTable = false;
                string appName;
                CPCSBaseClass cs = cp.CSNew();
                // 
                appName = cp.Site.Name;
                iContentName = ContentName;
                if (iContentName != "") {
                    SQL = "select id from cccontent where name=" + cp.Db.EncodeSQLText(iContentName);
                    if (cs.OpenSQL(SQL))
                        ContentID = cs.GetInteger("id");
                    cs.Close();
                }
                if (iContentName != "" & (ContentID == 0)) {
                    return string.Empty;
                } else {
                    // 
                    // Build table lookup
                    // 
                    Dictionary<int, tableClass> tables = new Dictionary<int, tableClass>();
                    SQL = "select T.ID,T.Name as TableName,D.Name as DataSourceName from ccTables T Left Join ccDataSources D on D.ID=T.DataSourceID";
                    if (cs.OpenSQL(SQL)) {
                        do {
                            tableClass table = new tableClass();
                            table.tableName = cs.GetText("TableName");
                            table.dataSourceName = cs.GetText("DataSourceName");
                            tables.Add(cs.GetInteger("id"), table);
                            cs.GoNext();
                        }
                        while (cs.OK());
                    }
                    cs.Close();
                    // 
                    // 
                    // Build SortMethod lookup
                    // 
                    Dictionary<int, string> sorts = new Dictionary<int, string>();
                    SQL = "select ID,Name from ccSortMethods";
                    if (cs.OpenSQL(SQL)) {
                        do {
                            sorts.Add(cs.GetInteger("id"), cs.GetText("name"));
                            cs.GoNext();
                        }
                        while (cs.OK());
                    }
                    cs.Close();
                    // 
                    // Build groups lookup
                    // 
                    Dictionary<int, string> groups = new Dictionary<int, string>();
                    SQL = "select ID,Name from ccGroups";
                    if (cs.OpenSQL(SQL)) {
                        do {
                            groups.Add(cs.GetInteger("id"), cs.GetText("name"));
                            cs.GoNext();
                        }
                        while (cs.OK());
                    }
                    cs.Close();
                    // 
                    // Build Content lookup
                    // 
                    SQL = "select id,name from ccContent";
                    Dictionary<int, string> contents = new Dictionary<int, string>();
                    if (cs.OpenSQL(SQL)) {
                        do {
                            contents.Add(cs.GetInteger("id"), cs.GetText("name"));
                            cs.GoNext();
                        }
                        while (cs.OK());
                    }
                    cs.Close();
                    // '
                    // ' select all the fields
                    // '
                    // If ContentID <> 0 Then
                    // SQL = "select " & FieldSelectList & "" _
                    // & " from ccfields f left join ccfieldhelp h on h.fieldid=f.id" _
                    // & " where (f.Type<>0)and(f.contentid=" & ContentID & ")" _
                    // & ""
                    // Else
                    // SQL = "select " & FieldSelectList & "" _
                    // & " from ccfields f left join ccfieldhelp h on h.fieldid=f.id" _
                    // & " where (f.Type<>0)" _
                    // & ""
                    // End If
                    // If Not IncludeBaseFields Then
                    // SQL = SQL & " and ((f.IsBaseField is null)or(f.IsBaseField=0))"
                    // End If
                    // SQL = SQL & " order by f.contentid,f.id,h.id desc"

                    // RS = cpCore.app.executeSql(SQL)
                    // CFields = convertDataTabletoArray(RS)
                    // CFieldCnt = UBound(CFields, 2) + 1
                    // 
                    // select the content
                    // 
                    if (ContentID != 0)
                        SQL = "select " + ContentSelectList + " from ccContent where (id=" + ContentID + ")and(contenttableid is not null)and(contentcontrolid is not null) order by id";
                    else
                        SQL = "select " + ContentSelectList + " from ccContent where (name<>'')and(name is not null)and(contenttableid is not null)and(contentcontrolid is not null) order by id";
                    CPCSBaseClass csContent = cp.CSNew();
                    if (csContent.OpenSQL(SQL)) {
                        // 
                        // ----- <cdef>
                        // 
                        IsBaseContent = (csContent.GetBoolean("isBaseContent"));
                        iContentName = GetRSXMLAttribute(csContent, "Name");
                        ContentID = (csContent.GetInteger("ID"));
                        sb.Append(System.Environment.NewLine + "\t" + "<CDef");
                        sb.Append(" Name=\"" + iContentName + "\"");
                        if ((!IsBaseContent) | IncludeBaseFields) {
                            sb.Append(" Active=\"" + GetRSXMLAttribute(csContent, "Active") + "\"");
                            sb.Append(" AdminOnly=\"" + GetRSXMLAttribute(csContent, "AdminOnly") + "\"");
                            // sb.Append( " AliasID=""" & GetRSXMLAttribute( appname,RS, "AliasID") & """")
                            // sb.Append( " AliasName=""" & GetRSXMLAttribute( appname,RS, "AliasName") & """")
                            sb.Append(" AllowAdd=\"" + GetRSXMLAttribute(csContent, "AllowAdd") + "\"");
                            sb.Append(" AllowCalendarEvents=\"" + GetRSXMLAttribute(csContent, "AllowCalendarEvents") + "\"");
                            sb.Append(" AllowContentChildTool=\"" + GetRSXMLAttribute(csContent, "AllowContentChildTool") + "\"");
                            sb.Append(" AllowContentTracking=\"" + GetRSXMLAttribute(csContent, "AllowContentTracking") + "\"");
                            sb.Append(" AllowDelete=\"" + GetRSXMLAttribute(csContent, "AllowDelete") + "\"");
                            sb.Append(" AllowMetaContent=\"" + GetRSXMLAttribute(csContent, "AllowMetaContent") + "\"");
                            sb.Append(" AllowTopicRules=\"" + GetRSXMLAttribute(csContent, "AllowTopicRules") + "\"");
                            sb.Append(" AllowWorkflowAuthoring=\"" + GetRSXMLAttribute(csContent, "AllowWorkflowAuthoring") + "\"");
                            // 
                            AuthoringTableID = (csContent.GetInteger("AuthoringTableID"));
                            TableName = "";
                            DataSourceName = "";
                            if ((tables.ContainsKey(AuthoringTableID))) {
                                TableName = tables[AuthoringTableID].tableName;
                                DataSourceName = tables[AuthoringTableID].dataSourceName;
                            }
                            if (DataSourceName == "")
                                DataSourceName = "Default";
                            if (Strings.UCase(TableName) == "CCMENUENTRIES")
                                FoundMenuTable = true;
                            sb.Append(" AuthoringDataSourceName=\"" + EncodeXMLattribute(DataSourceName) + "\"");
                            sb.Append(" AuthoringTableName=\"" + EncodeXMLattribute(TableName) + "\"");
                            // 
                            ContentTableID = (csContent.GetInteger("ContentTableID"));
                            if (ContentTableID != AuthoringTableID) {
                                if (ContentTableID != 0) {
                                    TableName = "";
                                    DataSourceName = "";
                                    if ((tables.ContainsKey(ContentTableID))) {
                                        TableName = tables[ContentTableID].tableName;
                                        DataSourceName = tables[ContentTableID].dataSourceName;
                                        if (DataSourceName == "")
                                            DataSourceName = "Default";
                                    }
                                }
                            }
                            sb.Append(" ContentDataSourceName=\"" + EncodeXMLattribute(DataSourceName) + "\"");
                            sb.Append(" ContentTableName=\"" + EncodeXMLattribute(TableName) + "\"");
                            // 
                            DefaultSortMethod = "";
                            DefaultSortMethodID = (csContent.GetInteger("DefaultSortMethodID"));
                            if ((sorts.ContainsKey(DefaultSortMethodID)))
                                DefaultSortMethod = sorts[DefaultSortMethodID];
                            sb.Append(" DefaultSortMethod=\"" + EncodeXMLattribute(DefaultSortMethod) + "\"");
                            // 
                            sb.Append(" DeveloperOnly=\"" + GetRSXMLAttribute(csContent, "DeveloperOnly") + "\"");
                            sb.Append(" DropDownFieldList=\"" + GetRSXMLAttribute(csContent, "DropDownFieldList") + "\"");
                            // 
                            EditorGroupName = "";
                            EditorGroupID = (csContent.GetInteger("EditorGroupID"));
                            if ((groups.ContainsKey(EditorGroupID)))
                                EditorGroupName = groups[EditorGroupID];
                            sb.Append(" EditorGroupName=\"" + EncodeXMLattribute(EditorGroupName) + "\"");
                            // 
                            ParentName = "";
                            ParentID = (csContent.GetInteger("ParentID"));
                            if ((contents.ContainsKey(ParentID)))
                                ParentName = contents[ParentID];
                            sb.Append(" Parent=\"" + EncodeXMLattribute(ParentName) + "\"");
                            // 
                            sb.Append(" IconLink=\"" + GetRSXMLAttribute(csContent, "IconLink") + "\"");
                            sb.Append(" IconHeight=\"" + GetRSXMLAttribute(csContent, "IconHeight") + "\"");
                            sb.Append(" IconWidth=\"" + GetRSXMLAttribute(csContent, "IconWidth") + "\"");
                            sb.Append(" IconSprites=\"" + GetRSXMLAttribute(csContent, "IconSprites") + "\"");
                            // 
                            // 
                            if (true)
                                // 
                                // Add IsBaseContent
                                // 
                                sb.Append(" isbasecontent=\"" + GetRSXMLAttribute(csContent, "IsBaseContent") + "\"");
                        }
                        // 
                        if (true)
                            // 
                            // Add guid
                            // 
                            sb.Append(" guid=\"" + GetRSXMLAttribute(csContent, "ccGuid") + "\"");
                        sb.Append(" >");
                        // 
                        // create output
                        // 
                        if (ContentID != 0)
                            SQL = "select " + FieldSelectList + ""
                                + " from ccfields f left join ccfieldhelp h on h.fieldid=f.id"
                                + " where (f.Type<>0)and(f.contentid=" + ContentID + ")"
                                + "";
                        else
                            SQL = "select " + FieldSelectList + ""
                            + " from ccfields f left join ccfieldhelp h on h.fieldid=f.id"
                            + " where (f.Type<>0)"
                            + "";
                        if (!IncludeBaseFields)
                            SQL = SQL + " and ((f.IsBaseField is null)or(f.IsBaseField=0))";
                        SQL = SQL + " order by f.contentid,f.editTab,f.editSortPriority,f.id";
                        CPCSBaseClass CFields = cp.CSNew();
                        if ((CFields.OpenSQL(SQL))) {
                            fieldId = 0;
                            do {
                                LastFieldID = fieldId;
                                fieldId = CFields.GetInteger("ID");
                                FieldName = CFields.GetText("Name");
                                FieldContentID = CFields.GetInteger("contentid");
                                if (FieldContentID > ContentID)
                                    break;
                                else if ((FieldContentID == ContentID) & (fieldId != LastFieldID)) {
                                    if (IncludeBaseFields | (Strings.InStr(1, ",id,ContentCategoryID,dateadded,createdby,modifiedby,EditBlank,EditArchive,EditSourceID,ContentControlID,CreateKey,ModifiedDate,ccguid,", "," + FieldName + ",", CompareMethod.Text) == 0)) {
                                        int memberSelectGroupId = CFields.GetInteger("MemberSelectGroupID");
                                        string memberSelectGroup = memberSelectGroupId <= 0 ? "" : cp.Group.GetName(memberSelectGroupId.ToString());
                                        sb.Append(System.Environment.NewLine + "\t" + "\t" + "<Field");
                                        fieldType = csv_GetFieldDescriptorByType(CFields.GetInteger("Type"));
                                        sb.Append(" Name=\"" + FieldName + "\"");
                                        sb.Append(" Caption=\"" + CFields.GetText("Caption") + "\"");
                                        sb.Append(" EditTab=\"" + CFields.GetText("EditTab") + "\"");
                                        sb.Append(" FieldType=\"" + fieldType + "\"");
                                        sb.Append(" Authorable=\"" + CFields.GetBoolean("Authorable") + "\"");
                                        sb.Append(" EditSortPriority=\"" + CFields.GetText("EditSortPriority") + "\"");
                                        sb.Append(" DefaultValue=\"" + CFields.GetText("DefaultValue") + "\"");
                                        sb.Append(" active=\"" + CFields.GetBoolean("Active") + "\"");
                                        sb.Append(" AdminOnly=\"" + CFields.GetBoolean("AdminOnly") + "\"");
                                        sb.Append(" DeveloperOnly=\"" + CFields.GetBoolean("DeveloperOnly") + "\"");
                                        sb.Append(" HTMLContent=\"" + CFields.GetBoolean("HTMLContent") + "\"");
                                        sb.Append(" Required=\"" + CFields.GetBoolean("Required") + "\"");
                                        sb.Append(" TextBuffered=\"" + CFields.GetBoolean("TextBuffered") + "\"");
                                        sb.Append(" UniqueName=\"" + CFields.GetBoolean("UniqueName") + "\"");
                                        sb.Append(" RSSTitle=\"" + CFields.GetBoolean("RSSTitleField") + "\"");
                                        sb.Append(" RSSDescription=\"" + CFields.GetBoolean("RSSDescriptionField") + "\"");
                                        sb.Append(" MemberSelectGroup=\"" + memberSelectGroup + "\"");
                                        sb.Append(" Scramble=\"" + CFields.GetBoolean("Scramble") + "\"");
                                        sb.Append(" LookupList=\"" + CFields.GetText("LookupList") + "\"");
                                        sb.Append(" NotEditable=\"" + CFields.GetBoolean("NotEditable") + "\"");
                                        sb.Append(" Password=\"" + CFields.GetBoolean("Password") + "\"");
                                        sb.Append(" ReadOnly=\"" + CFields.GetBoolean("ReadOnly") + "\"");
                                        sb.Append(" ManyToManyRulePrimaryField=\"" + CFields.GetText("ManyToManyRulePrimaryField") + "\"");
                                        sb.Append(" ManyToManyRuleSecondaryField=\"" + CFields.GetText("ManyToManyRuleSecondaryField") + "\"");
                                        sb.Append(" IsModified=\"" + (CFields.GetInteger("ModifiedBy") != 0) + "\"");
                                        sb.Append(" IndexColumn=\"" + CFields.GetText("IndexColumn") + "\"");
                                        sb.Append(" IndexSortDirection=\"" + CFields.GetText("IndexSortDirection") + "\"");
                                        sb.Append(" IndexSortOrder=\"" + CFields.GetText("IndexSortPriority") + "\"");
                                        sb.Append(" IndexWidth=\"" + CFields.GetText("IndexWidth") + "\"");
                                        sb.Append(" RedirectID=\"" + CFields.GetText("RedirectID") + "\"");
                                        sb.Append(" RedirectPath=\"" + CFields.GetText("RedirectPath") + "\"");
                                        if (true)
                                            sb.Append(" IsBaseField=\"" + CFields.GetBoolean("IsBaseField") + "\"");
                                        // 
                                        RecordName = "";
                                        RecordID = CFields.GetInteger("LookupContentID");
                                        if ((contents.ContainsKey(RecordID)))
                                            RecordName = contents[RecordID];
                                        sb.Append(" LookupContent=\"" + System.Net.WebUtility.HtmlEncode(RecordName) + "\"");
                                        // 
                                        RecordName = "";
                                        RecordID = CFields.GetInteger("RedirectContentID");
                                        if ((contents.ContainsKey(RecordID)))
                                            RecordName = contents[RecordID];
                                        sb.Append(" RedirectContent=\"" + System.Net.WebUtility.HtmlEncode(RecordName) + "\"");
                                        // 
                                        RecordName = "";
                                        RecordID = CFields.GetInteger("ManyToManyContentID");
                                        if ((contents.ContainsKey(RecordID)))
                                            RecordName = contents[RecordID];
                                        sb.Append(" ManyToManyContent=\"" + System.Net.WebUtility.HtmlEncode(RecordName) + "\"");
                                        // 
                                        RecordName = "";
                                        RecordID = CFields.GetInteger("ManyToManyRuleContentID");
                                        if ((contents.ContainsKey(RecordID)))
                                            RecordName = contents[RecordID];
                                        sb.Append(" ManyToManyRuleContent=\"" + System.Net.WebUtility.HtmlEncode(RecordName) + "\"");
                                        // 
                                        sb.Append(" >");
                                        // 
                                        HelpCnt = 0;
                                        HelpDefault = CFields.GetText("helpcustom");
                                        if (HelpDefault == "")
                                            HelpDefault = CFields.GetText("helpdefault");
                                        if (HelpDefault != "") {
                                            sb.Append(System.Environment.NewLine + "\t" + "\t" + "\t" + "<HelpDefault>" + EncodeCData(HelpDefault) + "</HelpDefault>");
                                            HelpCnt = HelpCnt + 1;
                                        }
                                        // HelpCustom = cfields.getText("helpcustom")
                                        // If HelpCustom <> "" Then
                                        // sb.Append( vbCrLf & vbTab & vbTab & vbTab & "<HelpCustom>" & HelpCustom & "</HelpCustom>")
                                        // HelpCnt = HelpCnt + 1
                                        // End If
                                        if (HelpCnt > 0)
                                            sb.Append(System.Environment.NewLine + "\t" + "\t");
                                        sb.Append("</Field>");
                                    }

                                    FieldCnt = FieldCnt + 1;
                                }

                                CFields.GoNext();
                            }
                            while (CFields.OK());
                        }
                        CFields.Close();
                        // 
                        if (FieldCnt > 0)
                            sb.Append(System.Environment.NewLine + "\t");
                        sb.Append("</CDef>");
                    }
                    csContent.Close();
                    if (ContentName == "") {
                        // 
                        // Add other areas of the CDef file
                        // 
                        sb.Append(GetXMLContentDefinition_SQLIndexes());
                        if (FoundMenuTable)
                            sb.Append(GetXMLContentDefinition_AdminMenus());
                    }
                    const string ApplicationCollectionGuid = "{C58A76E2-248B-4DE8-BF9C-849A960F79C6}";
                    const string CollectionFileRootNode = "collection";
                    return "<" + CollectionFileRootNode + " name=\"Application\" guid=\"" + ApplicationCollectionGuid + "\">" + sb.ToString() + System.Environment.NewLine + "</" + CollectionFileRootNode + ">";
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetXMLContentDefinition3");
                return string.Empty;
            }
        }
        // 
        // ========================================================================
        // ----- Save the admin menus to CDef AdminMenu tags
        // ========================================================================
        // 
        private string GetXMLContentDefinition_SQLIndexes() {
            try {
                string DataSourceName;
                string TableName;
                // 
                string IndexFields = "";
                string IndexList = "";
                string IndexName;
                string[] ListRows;
                string[] ListRowSplit;
                string SQL;
                CPCSBaseClass cs = cp.CSNew();
                int Ptr;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                // 
                SQL = "select D.name as DataSourceName,T.name as TableName"
                    + " from cctables T left join ccDataSources d on D.ID=T.DataSourceID"
                    + " where t.active<>0";
                if (cs.OpenSQL(SQL)) {
                    do {
                        DataSourceName = cs.GetText("DataSourceName");
                        TableName = cs.GetText("TableName");
                        // 
                        // need a solution for this
                        // 
                        // IndexList = cpCore.app.csv_GetSQLIndexList(DataSourceName, TableName)
                        // 
                        if (IndexList != "") {
                            ListRows = Strings.Split(IndexList, System.Environment.NewLine);
                            IndexName = "";
                            for (Ptr = 0; Ptr <= Information.UBound(ListRows) + 1; Ptr++) {
                                if (Ptr <= Information.UBound(ListRows))
                                    // 
                                    // ListRowSplit has the indexname and field for this index
                                    // 
                                    ListRowSplit = Strings.Split(ListRows[Ptr], ",");
                                else
                                    // 
                                    // one past the last row, ListRowSplit gets a dummy entry to force the output of the last line
                                    // 
                                    ListRowSplit = Strings.Split("-,-", ",");
                                if (Information.UBound(ListRowSplit) > 0) {
                                    if (ListRowSplit[0] != "") {
                                        if (IndexName == "") {
                                            // 
                                            // first line of the first index description
                                            // 
                                            IndexName = ListRowSplit[0];
                                            IndexFields = ListRowSplit[1];
                                        } else if (IndexName == ListRowSplit[0])
                                            // 
                                            // next line of the index description
                                            // 
                                            IndexFields = IndexFields + "," + ListRowSplit[1];
                                        else {
                                            // 
                                            // first line of a new index description
                                            // save previous line
                                            // 
                                            if (IndexName != "" & IndexFields != "") {
                                                sb.Append("<SQLIndex");
                                                sb.Append(" Indexname=\"" + EncodeXMLattribute(IndexName) + "\"");
                                                sb.Append(" DataSourceName=\"" + EncodeXMLattribute(DataSourceName) + "\"");
                                                sb.Append(" TableName=\"" + EncodeXMLattribute(TableName) + "\"");
                                                sb.Append(" FieldNameList=\"" + EncodeXMLattribute(IndexFields) + "\"");
                                                sb.Append("></SQLIndex>" + System.Environment.NewLine);
                                            }
                                            // 
                                            IndexName = ListRowSplit[0];
                                            IndexFields = ListRowSplit[1];
                                        }
                                    }
                                }
                            }
                        }

                        cs.GoNext();
                    }
                    while (cs.OK());
                }
                cs.Close();
                return sb.ToString();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetXMLContentDefinition3");
                return string.Empty;
            }
        }
        // 
        // ========================================================================
        // ----- Save the admin menus to CDef AdminMenu tags
        // ========================================================================
        // 
        private string GetXMLContentDefinition_AdminMenus() {
            return GetXMLContentDefinition_AdminMenus_MenuEntries()
                + GetXMLContentDefinition_AdminMenus_NavigatorEntries();
        }
        // 
        // ========================================================================
        // ----- Save the admin menus to CDef AdminMenu tags
        // ========================================================================
        // 
        private string GetXMLContentDefinition_AdminMenus_NavigatorEntries() {
            try {
                int NavIconType;
                string NavIconTitle;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                CPCSBaseClass dt = cp.CSNew();
                string menuNameSpace;
                string RecordName;
                int ParentID;
                int MenuContentID;
                string[] SplitArray;
                int SplitIndex;
                // 
                // ****************************** if cdef not loaded, this fails
                // 
                MenuContentID = cp.Content.GetRecordID("Content", "Navigator Entries");
                if (dt.OpenSQL("select * from ccMenuEntries where (contentcontrolid=" + MenuContentID + ")and(name<>'')")) {
                    NavIconType = 0;
                    NavIconTitle = "";
                    do {
                        RecordName = (dt.GetText("Name"));
                        ParentID = (dt.GetInteger("ParentID"));
                        menuNameSpace = getMenuNameSpace(ParentID, "");
                        sb.Append("<NavigatorEntry Name=\"" + EncodeXMLattribute(RecordName) + "\"");
                        sb.Append(" NameSpace=\"" + menuNameSpace + "\"");
                        sb.Append(" LinkPage=\"" + GetRSXMLAttribute(dt, "LinkPage") + "\"");
                        sb.Append(" ContentName=\"" + GetRSXMLLookupAttribute(dt, "ContentID", "ccContent") + "\"");
                        sb.Append(" AdminOnly=\"" + GetRSXMLAttribute(dt, "AdminOnly") + "\"");
                        sb.Append(" DeveloperOnly=\"" + GetRSXMLAttribute(dt, "DeveloperOnly") + "\"");
                        sb.Append(" NewWindow=\"" + GetRSXMLAttribute(dt, "NewWindow") + "\"");
                        sb.Append(" Active=\"" + GetRSXMLAttribute(dt, "Active") + "\"");
                        sb.Append(" AddonName=\"" + GetRSXMLLookupAttribute(dt, "AddonID", "ccAggregateFunctions") + "\"");
                        sb.Append(" SortOrder=\"" + GetRSXMLAttribute(dt, "SortOrder") + "\"");
                        NavIconType = cp.Utils.EncodeInteger(GetRSXMLAttribute(dt, "NavIconType"));
                        NavIconTitle = GetRSXMLAttribute(dt, "NavIconTitle");
                        sb.Append(" NavIconTitle=\"" + NavIconTitle + "\"");
                        SplitArray = Strings.Split(NavIconTypeList + ",help", ",");
                        SplitIndex = NavIconType - 1;
                        if ((SplitIndex >= 0) & (SplitIndex <= Information.UBound(SplitArray))) { sb.Append(" NavIconType=\"" + SplitArray[SplitIndex] + "\""); }
                        sb.Append(" guid=\"" + GetRSXMLAttribute(dt, "ccGuid") + "\"");
                        // 
                        sb.Append("></NavigatorEntry>" + System.Environment.NewLine);
                        dt.GoNext();
                    }
                    while (dt.OK());
                }
                return sb.ToString();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetXMLContentDefinition3");
                return string.Empty;
            }
        }
        // 
        // ========================================================================
        // ----- Save the admin menus to CDef AdminMenu tags
        // ========================================================================
        // 
        private string GetXMLContentDefinition_AdminMenus_MenuEntries() {
            try {
                StringBuilder sb = new StringBuilder();
                CPCSBaseClass dr = cp.CSNew();
                string RecordName;
                int MenuContentID;
                // 
                MenuContentID = cp.Content.GetRecordID("Content", "Menu Entries");
                if ((dr.OpenSQL("select * from ccMenuEntries where (contentcontrolid=" + MenuContentID + ")and(name<>'')"))) {
                    do {
                        RecordName = (dr.GetText("Name"));
                        sb.Append("<MenuEntry Name=\"" + EncodeXMLattribute(RecordName) + "\"");
                        sb.Append(" ParentName=\"" + GetRSXMLLookupAttribute(dr, "ParentID", "ccMenuEntries") + "\"");
                        sb.Append(" LinkPage=\"" + GetRSXMLAttribute(dr, "LinkPage") + "\"");
                        sb.Append(" ContentName=\"" + GetRSXMLLookupAttribute(dr, "ContentID", "ccContent") + "\"");
                        sb.Append(" AdminOnly=\"" + GetRSXMLAttribute(dr, "AdminOnly") + "\"");
                        sb.Append(" DeveloperOnly=\"" + GetRSXMLAttribute(dr, "DeveloperOnly") + "\"");
                        sb.Append(" NewWindow=\"" + GetRSXMLAttribute(dr, "NewWindow") + "\"");
                        sb.Append(" Active=\"" + GetRSXMLAttribute(dr, "Active") + "\"");
                        if (true)
                            sb.Append(" AddonName=\"" + GetRSXMLLookupAttribute(dr, "AddonID", "ccAggregateFunctions") + "\"");
                        sb.Append("/>" + System.Environment.NewLine);
                        dr.GoNext();
                    }
                    while (dr.OK());
                }
                dr.Close();
                return sb.ToString();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetXMLContentDefinition3");
                return string.Empty;

            }
        }
        // 
        // ========================================================================
        // 
        // ========================================================================
        // 
        private string GetXMLContentDefinition_AggregateFunctions() {
            try {
                CPCSBaseClass rs = cp.CSNew();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                // 
                if ((rs.OpenSQL("select * from ccAggregateFunctions"))) {
                    do {
                        sb.Append("<Addon Name=\"" + GetRSXMLAttribute(rs, "Name") + "\"");
                        sb.Append(" Link=\"" + GetRSXMLAttribute(rs, "Link") + "\"");
                        sb.Append(" ObjectProgramID=\"" + GetRSXMLAttribute(rs, "ObjectProgramID") + "\"");
                        sb.Append(" ArgumentList=\"" + GetRSXMLAttribute(rs, "ArgumentList") + "\"");
                        sb.Append(" SortOrder=\"" + GetRSXMLAttribute(rs, "SortOrder") + "\"");
                        sb.Append(" >");
                        sb.Append(GetRSXMLAttribute(rs, "Copy"));
                        sb.Append("</Addon>" + System.Environment.NewLine);
                        rs.GoNext();
                    }
                    while (rs.OK());
                }
                return sb.ToString();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetXMLContentDefinition3");
                return string.Empty;

            }
        }
        // 
        // 
        // 
        private string EncodeXMLattribute(string Source) {
            string result = System.Net.WebUtility.HtmlEncode(Source);
            result = Strings.Replace(result, System.Environment.NewLine, " ");
            result = Strings.Replace(result, "\n", "");
            result = Strings.Replace(result, "\r", "");
            return result;
        }
        // 
        // 
        // 
        private string GetTableRecordName(string TableName, int RecordID) {
            try {
                using (CPCSBaseClass dt = cp.CSNew()) {
                    if (RecordID != 0 & TableName != "") {
                        if (dt.OpenSQL("select Name from " + TableName + " where ID=" + RecordID))
                            return dt.GetText("name");
                        dt.Close();
                    }
                }
                return string.Empty;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetXMLContentDefinition3");
                return string.Empty;
            }
        }
        // 
        // 
        // 
        private string GetRSXMLAttribute(CPCSBaseClass dr, string FieldName) {
            return EncodeXMLattribute((dr.GetText(FieldName)));
        }
        // 
        // 
        // 
        private string GetRSXMLLookupAttribute(CPCSBaseClass dr, string FieldName, string TableName) {
            return EncodeXMLattribute(GetTableRecordName(TableName, (dr.GetInteger(FieldName))));
        }
        // 
        // 
        // 
        private string getMenuNameSpace(int RecordID, string UsedIDString) {
            string result = "";
            try {
                CPCSBaseClass rs = cp.CSNew();
                int ParentID;
                string RecordName = "";
                string ParentSpace = "";
                // 
                if (RecordID != 0) {
                    if (Strings.InStr(1, "," + UsedIDString + ",", "," + RecordID + ",", CompareMethod.Text) != 0) {
                        cp.Site.ErrorReport("Circular reference found in UsedIDString [" + UsedIDString + "] getting ccMenuEntries namespace for recordid [" + RecordID + "]");
                        result = "";
                    } else {
                        UsedIDString = UsedIDString + "," + RecordID;
                        ParentID = 0;
                        if (RecordID != 0) {
                            if ((rs.OpenSQL("select Name,ParentID from ccMenuEntries where ID=" + RecordID))) {
                                ParentID = rs.GetInteger("parentid");
                                RecordName = rs.GetText("name");
                            }
                            rs.Close();
                        }
                        if (RecordName != "") {
                            if (ParentID == RecordID) {
                                // 
                                // circular reference
                                // 
                                cp.Site.ErrorReport("Circular reference found (ParentID=RecordID) getting ccMenuEntries namespace for recordid [" + RecordID + "]");
                                result = "";
                            } else {
                                if (ParentID != 0)
                                    // 
                                    // get next parent
                                    // 
                                    ParentSpace = getMenuNameSpace(ParentID, UsedIDString);
                                if (ParentSpace != "")
                                    result = ParentSpace + "." + RecordName;
                                else
                                    result = RecordName;
                            }
                        } else
                            result = "";
                    }
                }
                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetXMLContentDefinition3");
                return string.Empty;
            }
        }
        // 
        // ========================================================================
        // ----- Get FieldDescritor from FieldType
        // ========================================================================
        // 
        public string csv_GetFieldDescriptorByType(int fieldType) {
            string result = "";
            result = "";
            try {
                switch (fieldType) {
                    case (int)CPContentBaseClass.FieldTypeIdEnum.Boolean: {
                            result = FieldDescriptorBoolean;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.Currency: {
                            result = FieldDescriptorCurrency;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.Date: {
                            result = FieldDescriptorDate;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.File: {
                            result = FieldDescriptorFile;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.Float: {
                            result = FieldDescriptorFloat;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.FileImage: {
                            result = FieldDescriptorImage;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.Link: {
                            result = FieldDescriptorLink;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.ResourceLink: {
                            result = FieldDescriptorResourceLink;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.Integer: {
                            result = FieldDescriptorInteger;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.LongText: {
                            result = FieldDescriptorLongText;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.Lookup: {
                            result = FieldDescriptorLookup;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.MemberSelect: {
                            result = FieldDescriptorMemberSelect;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.Redirect: {
                            result = FieldDescriptorRedirect;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.ManyToMany: {
                            result = FieldDescriptorManyToMany;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.FileText: {
                            result = FieldDescriptorTextFile;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.FileCSS: {
                            result = FieldDescriptorCSSFile;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.FileXML: {
                            result = FieldDescriptorXMLFile;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.FileJavascript: {
                            result = FieldDescriptorJavascriptFile;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.Text: {
                            result = FieldDescriptorText;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.HTML: {
                            result = FieldDescriptorHTML;
                            break;
                        }

                    case (int)CPContentBaseClass.FieldTypeIdEnum.FileHTML: {
                            result = FieldDescriptorHTMLFile;
                            break;
                        }

                    default: {
                            if (fieldType == (int)CPContentBaseClass.FieldTypeIdEnum.AutoIdIncrement)
                                result = "AutoIncrement";
                            else if (fieldType == (int)CPContentBaseClass.FieldTypeIdEnum.MemberSelect)
                                result = "MemberSelect";
                            else
                                // 
                                // If field type is ignored, call it a text field
                                // 
                                result = FieldDescriptorText;
                            break;
                        }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetXMLContentDefinition3");
            }
            return result;
        }
        // 
        // ====================================================================================================
        private string EncodeCData(string Source) {
            if (string.IsNullOrWhiteSpace(Source)) { return string.Empty; }
            return "<![CDATA[" + Strings.Replace(Source, "]]>", "]]]]><![CDATA[>") + "]]>";
        }
        //
        //====================================================================================================
        /// <summary>
        ///  Get an XML nodes attribute based on its name
        /// </summary>
        public static string getXMLAttribute(CoreController core, ref bool Found, XmlNode Node, string Name, string DefaultIfNotFound) {
            string returnAttr = "";
            try {
                XmlNode ResultNode = null;
                string UcaseName = null;
                //
                Found = false;
                ResultNode = Node.Attributes.GetNamedItem(Name);
                if (ResultNode == null) {
                    UcaseName = GenericController.toUCase(Name);
                    foreach (XmlAttribute NodeAttribute in Node.Attributes) {
                        if (GenericController.toUCase(NodeAttribute.Name) == UcaseName) {
                            returnAttr = NodeAttribute.Value;
                            Found = true;
                            break;
                        }
                    }
                    if (!Found) {
                        returnAttr = DefaultIfNotFound;
                    }
                } else {
                    returnAttr = ResultNode.Value;
                    Found = true;
                }
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
            return returnAttr;
        }
        //
        //====================================================================================================
        //
        public static double getXMLAttributeNumber(CoreController core, ref bool Found, XmlNode Node, string Name, string DefaultIfNotFound) {
            return encodeNumber(getXMLAttribute(core, ref Found, Node, Name, DefaultIfNotFound));
        }
        //
        //====================================================================================================
        //
        public static bool getXMLAttributeBoolean(CoreController core, ref bool Found, XmlNode Node, string Name, bool DefaultIfNotFound) {
            return GenericController.encodeBoolean(getXMLAttribute(core, ref Found, Node, Name, encodeText(DefaultIfNotFound)));
        }
        //
        //====================================================================================================
        //
        public static int getXMLAttributeInteger(CoreController core, ref bool Found, XmlNode Node, string Name, int DefaultIfNotFound) {
            return GenericController.encodeInteger(getXMLAttribute(core, ref Found, Node, Name, DefaultIfNotFound.ToString()));
        }


    }
}
