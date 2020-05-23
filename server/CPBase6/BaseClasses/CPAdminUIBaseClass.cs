
using Contensive.BaseModels;
using System;
using System.Collections.Generic;

namespace Contensive.BaseClasses {
    public abstract class CPAdminUIBaseClass {
        //
        //==========================================================================================
        /// <summary>
        /// Create a new instance of a Tool Form. Tool Forms are simple forms with key elements like buttons and header with a simple body
        /// </summary>
        /// <returns></returns>
        public abstract AdminUI.ToolFormBaseClass NewToolForm();
        //
        //==========================================================================================
        /// <summary>
        /// Create a new instance of a List Report. List reports have a list of data rows with filters on the left
        /// </summary>
        /// <returns></returns>
        public abstract AdminUI.ListReportBaseClass NewListReport();
        //
        //==========================================================================================
        /// <summary>
        /// Create an html row that includes a caption, editor and optional help content
        /// </summary>
        /// <returns></returns>
        public abstract string GetEditRow(string caption, string editor);
        /// <summary>
        /// Create an html row that includes a caption, editor and optional help content
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="editor"></param>
        /// <param name="help"></param>
        /// <returns></returns>
        public abstract string GetEditRow(string caption, string editor, string help);
        /// <summary>
        /// Create an html row that includes a caption, editor and optional help content
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="editor"></param>
        /// <param name="help"></param>
        /// <param name="htmlId"></param>
        /// <returns></returns>
        public abstract string GetEditRow(string caption, string editor, string help, string htmlId);
        /// <summary>
        /// Create an html row that includes a caption, editor and optional help content
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="editor"></param>
        /// <param name="help"></param>
        /// <param name="htmlId"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public abstract string GetEditRow(string caption, string editor, string help, string htmlId, bool required);
        /// <summary>
        /// Create an html row that includes a caption, editor and optional help content
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="editor"></param>
        /// <param name="help"></param>
        /// <param name="htmlId"></param>
        /// <param name="required"></param>
        /// <param name="blockBottomRule"></param>
        /// <returns></returns>
        public abstract string GetEditRow(string caption, string editor, string help, string htmlId, bool required, bool blockBottomRule);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a boolean field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetBooleanEditor(string htmlName, bool htmlValue, string htmlId, bool readOnly, bool required);
        /// <summary>
        /// Create an input for a boolean field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <param name="htmlId"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public abstract string GetBooleanEditor(string htmlName, bool htmlValue, string htmlId, bool readOnly);
        /// <summary>
        /// Create an input for a boolean field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <param name="htmlId"></param>
        /// <returns></returns>
        public abstract string GetBooleanEditor(string htmlName, bool htmlValue, string htmlId);
        /// <summary>
        /// Create an input for a boolean field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <returns></returns>
        public abstract string GetBooleanEditor(string htmlName, bool htmlValue);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a boolean field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetCurrencyEditor(string htmlName, double? htmlValue, string htmlId, bool readOnly, bool required);
        /// <summary>
        /// Create an input for a boolean field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <param name="htmlId"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public abstract string GetCurrencyEditor(string htmlName, double? htmlValue, string htmlId, bool readOnly);
        /// <summary>
        /// Create an input for a boolean field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <param name="htmlId"></param>
        /// <returns></returns>
        public abstract string GetCurrencyEditor(string htmlName, double? htmlValue, string htmlId);
        /// <summary>
        /// Create an input for a boolean field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <returns></returns>
        public abstract string GetCurrencyEditor(string htmlName, double? htmlValue);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a datetime field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetDateTimeEditor(string htmlName, DateTime? htmlValue, string htmlId, bool readOnly, bool required);
        /// <summary>
        /// Create an input for a datetime field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <param name="htmlId"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public abstract string GetDateTimeEditor(string htmlName, DateTime? htmlValue, string htmlId, bool readOnly);
        /// <summary>
        /// Create an input for a datetime field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <param name="htmlId"></param>
        /// <returns></returns>
        public abstract string GetDateTimeEditor(string htmlName, DateTime? htmlValue, string htmlId);
        /// <summary>
        /// Create an input for a datetime field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <returns></returns>
        public abstract string GetDateTimeEditor(string htmlName, DateTime? htmlValue);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a datetime field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetFileEditor(string htmlName, string currentPathFilename, string htmlId, bool readOnly, bool required);
        /// <summary>
        /// Create an input for a datetime field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="currentPathFilename"></param>
        /// <param name="htmlId"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public abstract string GetFileEditor(string htmlName, string currentPathFilename, string htmlId, bool readOnly);
        /// <summary>
        /// Create an input for a datetime field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="currentPathFilename"></param>
        /// <param name="htmlId"></param>
        /// <returns></returns>
        public abstract string GetFileEditor(string htmlName, string currentPathFilename, string htmlId);
        /// <summary>
        /// Create an input for a datetime field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="currentPathFilename"></param>
        /// <returns></returns>
        public abstract string GetFileEditor(string htmlName, string currentPathFilename);
        /// <summary>
        /// Create an input for a datetime field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <returns></returns>
        public abstract string GetFileEditor(string htmlName);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a htmlcode field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetHtmlCodeEditor(string htmlName, string htmlValue, string htmlId, bool readOnly, bool required);
        /// <summary>
        /// Create an input for a htmlcode field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <param name="htmlId"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public abstract string GetHtmlCodeEditor(string htmlName, string htmlValue, string htmlId, bool readOnly);
        /// <summary>
        /// Create an input for a htmlcode field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <param name="htmlId"></param>
        /// <returns></returns>
        public abstract string GetHtmlCodeEditor(string htmlName, string htmlValue, string htmlId);
        /// <summary>
        /// Create an input for a htmlcode field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <returns></returns>
        public abstract string GetHtmlCodeEditor(string htmlName, string htmlValue);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a html field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetHtmlEditor(string htmlName, string htmlValue, string htmlId, bool readOnly, bool required);
        /// <summary>
        /// Create an input for a html field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <param name="htmlId"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public abstract string GetHtmlEditor(string htmlName, string htmlValue, string htmlId, bool readOnly);
        /// <summary>
        /// Create an input for a html field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <param name="htmlId"></param>
        /// <returns></returns>
        public abstract string GetHtmlEditor(string htmlName, string htmlValue, string htmlId);
        /// <summary>
        /// Create an input for a html field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="htmlValue"></param>
        /// <returns></returns>
        public abstract string GetHtmlEditor(string htmlName, string htmlValue);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a html field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetImageEditor(string htmlName, string currentPathFilename, string htmlId, bool readOnly, bool required);
        /// <summary>
        /// Create an input for a html field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="currentPathFilename"></param>
        /// <param name="htmlId"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public abstract string GetImageEditor(string htmlName, string currentPathFilename, string htmlId, bool readOnly);
        /// <summary>
        /// Create an input for a html field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="currentPathFilename"></param>
        /// <param name="htmlId"></param>
        /// <returns></returns>
        public abstract string GetImageEditor(string htmlName, string currentPathFilename, string htmlId);
        /// <summary>
        /// Create an input for a html field type
        /// </summary>
        /// <param name="htmlName"></param>
        /// <param name="currentPathFilename"></param>
        /// <returns></returns>
        public abstract string GetImageEditor(string htmlName, string currentPathFilename);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for an integer field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetIntegerEditor(string htmlName, int? htmlValue, string htmlId, bool readOnly, bool required);
        public abstract string GetIntegerEditor(string htmlName, int? htmlValue, string htmlId, bool readOnly);
        public abstract string GetIntegerEditor(string htmlName, int? htmlValue, string htmlId);
        public abstract string GetIntegerEditor(string htmlName, int? htmlValue);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for an integer field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetLinkEditor(string htmlName, int? htmlValue, string htmlId, bool readOnly, bool required);
        public abstract string GetLinkEditor(string htmlName, int? htmlValue, string htmlId, bool readOnly);
        public abstract string GetLinkEditor(string htmlName, int? htmlValue, string htmlId);
        public abstract string GetLinkEditor(string htmlName, int? htmlValue);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for all text field types
        /// </summary>
        /// <returns></returns>
        public abstract string GetLongTextEditor(string htmlName, string htmlValue, string htmlId, bool readOnly, bool required);
        public abstract string GetLongTextEditor(string htmlName, string htmlValue, string htmlId, bool readOnly);
        public abstract string GetLongTextEditor(string htmlName, string htmlValue, string htmlId);
        public abstract string GetLongTextEditor(string htmlName, string htmlValue);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a lookup content field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetLookupContentEditor(string htmlName, int lookupContentId, int lookupRecordId, string htmlId, bool readOnly, bool required, string sqlFilter);
        public abstract string GetLookupContentEditor(string htmlName, int lookupContentId, int lookupRecordId, string htmlId, bool readOnly, bool required);
        public abstract string GetLookupContentEditor(string htmlName, int lookupContentId, int lookupRecordId, string htmlId, bool readOnly);
        public abstract string GetLookupContentEditor(string htmlName, int lookupContentId, int lookupRecordId, string htmlId);
        public abstract string GetLookupContentEditor(string htmlName, int lookupContentId, int lookupRecordId);
        public abstract string GetLookupContentEditor(string htmlName, int lookupContentId);
        //
        public abstract string GetLookupContentEditor(string htmlName, string lookupContentName, int lookupRecordId, string htmlId, bool readOnly, bool required, string sqlFilter);
        public abstract string GetLookupContentEditor(string htmlName, string lookupContentName, int lookupRecordId, string htmlId, bool readOnly, bool required);
        public abstract string GetLookupContentEditor(string htmlName, string lookupContentName, int lookupRecordId, string htmlId, bool readOnly);
        public abstract string GetLookupContentEditor(string htmlName, string lookupContentName, int lookupRecordId, string htmlId);
        public abstract string GetLookupContentEditor(string htmlName, string lookupContentName, int lookupRecordId);
        public abstract string GetLookupContentEditor(string htmlName, string lookupContentName);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a lookup list content field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetLookupListEditor(string htmlName, List<string> lookupList, int lookupListIndex, string htmlId, bool readOnly, bool required);
        public abstract string GetLookupListEditor(string htmlName, List<string> lookupList, int lookupListIndex, string htmlId, bool readOnly);
        public abstract string GetLookupListEditor(string htmlName, List<string> lookupList, int lookupListIndex, string htmlId);
        public abstract string GetLookupListEditor(string htmlName, List<string> lookupList, int lookupListIndex);
        public abstract string GetLookupListEditor(string htmlName, List<string> lookupList);
        //
        public abstract string GetLookupListEditor(string htmlName, List<string> lookupList, string lookupListName, string htmlId, bool readOnly, bool required);
        public abstract string GetLookupListEditor(string htmlName, List<string> lookupList, string lookupListName, string htmlId, bool readOnly);
        public abstract string GetLookupListEditor(string htmlName, List<string> lookupList, string lookupListName, string htmlId);
        public abstract string GetLookupListEditor(string htmlName, List<string> lookupList, string lookupListName);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a lookup list content field type
        /// </summary>
        /// <returns></returns>
        //public abstract string GetManyToManyCheckList(string htmlName, int lookupListIndex, List<string> lookupList, string htmlId, bool readOnly, bool required);
        //
        //public abstract string GetManyToManySelect(string htmlName, int lookupListIndex, List<string> lookupList, string htmlId, bool readOnly, bool required);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a member select content field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetMemberSelectEditor(string htmlName, int lookupPersonId, string groupGuid, string htmlId, bool readOnly, bool required);
        public abstract string GetMemberSelectEditor(string htmlName, int lookupPersonId, string groupGuid, string htmlId, bool readOnly);
        public abstract string GetMemberSelectEditor(string htmlName, int lookupPersonId, string groupGuid, string htmlId);
        public abstract string GetMemberSelectEditor(string htmlName, int lookupPersonId, string groupGuid);
        //
        public abstract string GetMemberSelectEditor(string htmlName, int lookupPersonId, int groupId, string htmlId, bool readOnly, bool required);
        public abstract string GetMemberSelectEditor(string htmlName, int lookupPersonId, int groupId, string htmlId, bool readOnly);
        public abstract string GetMemberSelectEditor(string htmlName, int lookupPersonId, int groupId, string htmlId);
        public abstract string GetMemberSelectEditor(string htmlName, int lookupPersonId, int groupId);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for a number field type
        /// </summary>
        /// <returns></returns>
        public abstract string GetNumberEditor(string htmlName, double? htmlValue, string htmlId, bool readOnly, bool required);
        public abstract string GetNumberEditor(string htmlName, double? htmlValue, string htmlId, bool readOnly);
        public abstract string GetNumberEditor(string htmlName, double? htmlValue, string htmlId);
        public abstract string GetNumberEditor(string htmlName, double? htmlValue);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for all text field types
        /// </summary>
        /// <returns></returns>
        public abstract string GetPasswordEditor(string htmlName, string htmlValue, string htmlId, bool readOnly, bool required);
        public abstract string GetPasswordEditor(string htmlName, string htmlValue, string htmlId, bool readOnly);
        public abstract string GetPasswordEditor(string htmlName, string htmlValue, string htmlId);
        public abstract string GetPasswordEditor(string htmlName, string htmlValue);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for all text field types
        /// </summary>
        /// <returns></returns>
        public abstract string GetSelectorStringEditor(string htmlName, string htmlValue, string selectorString, string htmlId, bool readOnly, bool required);
        public abstract string GetSelectorStringEditor(string htmlName, string htmlValue, string selectorString, string htmlId, bool readOnly);
        public abstract string GetSelectorStringEditor(string htmlName, string htmlValue, string selectorString, string htmlId);
        public abstract string GetSelectorStringEditor(string htmlName, string htmlValue, string selectorString);
        //
        //==========================================================================================
        /// <summary>
        /// Create an input for all text field types
        /// </summary>
        /// <returns></returns>
        public abstract string GetTextEditor(string htmlName, string htmlValue, string htmlId, bool readOnly, bool required);
        public abstract string GetTextEditor(string htmlName, string htmlValue, string htmlId, bool readOnly);
        public abstract string GetTextEditor(string htmlName, string htmlValue, string htmlId);
        public abstract string GetTextEditor(string htmlName, string htmlValue);
        //
    }
}

