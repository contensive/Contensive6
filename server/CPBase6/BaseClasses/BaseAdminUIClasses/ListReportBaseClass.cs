
using System;
using Contensive.BaseClasses;

namespace Contensive.BaseClasses.AdminUI {
    /// <summary>
    /// A tabular list of data rows with filters on the left.
    /// </summary>
    public abstract class ListReportBaseClass {
        //
        //-------------------------------------------------
        /// <summary>
        /// Set true if this tool is requested directly and not embedded in another AdminUI form
        /// </summary>
        public abstract bool IsOuterContainer { get; set; }
        //
        //-------------------------------------------------
        /// <summary>
        /// Add padding around the body
        /// </summary>
        public abstract int reportRowLimit { get; set; }
        //
        //-------------------------------------------------
        /// <summary>
        /// Add padding around the body
        /// </summary>
        public abstract bool IncludeBodyPadding { get; set; }
        //
        //-------------------------------------------------
        /// <summary>
        /// Add background color to the body
        /// </summary>
        public abstract bool IncludeBodyColor { get; set; }

        // 
        //-------------------------------------------------
        /// <summary>
        /// Method retrieves the rendered html. Call this method after populating all object elements
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public abstract string GetHtml(CPBaseClass cp);
        //
        //-------------------------------------------------
        /// <summary>
        /// use this area for optional filters
        /// </summary>
        public abstract string HtmlLeftOfTable { get; set; }
        //
        //-------------------------------------------------
        /// <summary>
        /// optional html before the table
        /// </summary>
        public abstract string HtmlBeforeTable { get; set; }
        //
        //-------------------------------------------------
        /// <summary>
        /// optional html after the table
        /// </summary>
        public abstract string HtmlAfterTable { get; set; }
        //
        //-------------------------------------------------
        /// <summary>
        /// Add hidden form input
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public abstract void AddFormHidden(string Name, string Value);
        /// <summary>
        /// Add hidden form input
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void AddFormHidden(string name, int value);
        /// <summary>
        /// Add hidden form input
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void AddFormHidden(string name, double value);
        /// <summary>
        /// Add hidden form input
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void AddFormHidden(string name, DateTime value);
        /// <summary>
        /// Add hidden form input
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void AddFormHidden(string name, bool value);
        //
        //-------------------------------------------------
        /// <summary>
        /// Add form button
        /// </summary>
        /// <param name="buttonValue"></param>
        public abstract void AddFormButton(string buttonValue);
        /// <summary>
        /// Add form button
        /// </summary>
        /// <param name="buttonValue"></param>
        /// <param name="buttonName"></param>
        public abstract void AddFormButton(string buttonValue, string buttonName);
        /// <summary>
        /// Add form button
        /// </summary>
        /// <param name="buttonValue"></param>
        /// <param name="buttonName"></param>
        /// <param name="buttonId"></param>
        public abstract void AddFormButton(string buttonValue, string buttonName, string buttonId);
        /// <summary>
        /// Add form button
        /// </summary>
        /// <param name="buttonValue"></param>
        /// <param name="buttonName"></param>
        /// <param name="buttonId"></param>
        /// <param name="buttonClass"></param>
        public abstract void AddFormButton(string buttonValue, string buttonName, string buttonId, string buttonClass);
        //
        //-------------------------------------------------
        /// <summary>
        /// This report will be wrapped in a form tag and the action should send traffic back to the same page. If empty, the form uses cp.Doc.RefreshQueryString
        /// </summary>
        public abstract string FormActionQueryString { get; set; }
        //
        //-------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public abstract string FormId { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract string Guid { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract string Name { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract string Title { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract string Warning { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract string Description { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract string ColumnName { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract string ColumnCaption { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract string ColumnCaptionClass { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract string ColumnCellClass { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract bool ColumnSortable { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract bool ColumnVisible { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract bool ColumnDownloadable { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract void addColumn();
        //
        //-------------------------------------------------
        //
        public abstract void AddRow();
        //
        //-------------------------------------------------
        //
        public abstract bool ExcludeRowFromDownload { get; set; }
        //
        //-------------------------------------------------
        //
        public abstract void AddRowClass(string styleClass);
        //
        //-------------------------------------------------
        //
        public abstract void SetCell(string content);
        public abstract void SetCell(string reportContent, string downloadContent);
        //
        //-------------------------------------------------
        //
        public abstract void SetCell(int content);
        public abstract void SetCell(int content, int downloadContent);
        //
        //-------------------------------------------------
        //
        public abstract void SetCell(double content);
        public abstract void SetCell(double content, double downloadContent);
        //
        //-------------------------------------------------
        //
        public abstract void SetCell(bool content);
        public abstract void SetCell(bool content, bool downloadContent);
        //
        //-------------------------------------------------
        //
        public abstract void SetCell(DateTime content);
        public abstract void SetCell(DateTime content, DateTime downloadContent);
        //
        //-------------------------------------------------
        //
        public abstract bool AddCsvDownloadCurrentPage { get; set; }
    }
}
