
using Contensive.BaseClasses;
using Contensive.Processor.Controllers;
using System;

namespace Contensive.Processor.AdminUI {
    public class CPAdminUIListReportClass : BaseClasses.AdminUI.ListReportBaseClass {
        //
        public CPAdminUIListReportClass(CoreController core) {
            this.core = core;
            this.form = new ListReportController();
        }
        //
        private readonly CoreController core;
        //
        private readonly ListReportController form;
        //
        // ====================================================================================================
        //

        public override bool IsOuterContainer {
            get {
                return form.isOuterContainer;
            }
            set {
                form.isOuterContainer = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override int reportRowLimit {
            get {
                return form.reportRowLimit;
            }
            set {
                form.reportRowLimit = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override bool IncludeBodyPadding {
            get {
                return form.includeBodyPadding;
            }
            set {
                form.includeBodyPadding = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override bool IncludeBodyColor {
            get {
                return form.includeBodyColor;
            }
            set {
                form.includeBodyColor = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string HtmlLeftOfTable {
            get {
                return form.htmlLeftOfTable;
            }
            set {
                form.htmlLeftOfTable = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string HtmlBeforeTable {
            get {
                return form.htmlBeforeTable;
            }
            set {
                form.htmlBeforeTable = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string HtmlAfterTable {
            get {
                return form.htmlAfterTable;
            }
            set {
                form.htmlAfterTable = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string FormActionQueryString {
            get {
                return form.formActionQueryString;
            }
            set {
                form.formActionQueryString = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string FormId {
            get {
                return form.formId;
            }
            set {
                form.formId = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string Guid {
            get {
                return form.guid;
            }
            set {
                form.guid = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string Name {
            get {
                return form.name;
            }
            set {
                form.name = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string Title {
            get {
                return form.title;
            }
            set {
                form.title = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string Warning {
            get {
                return form.warning;
            }
            set {
                form.warning = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string Description {
            get {
                return form.description;
            }
            set {
                form.description = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string ColumnName {
            get {
                return form.columnName;
            }
            set {
                form.columnName = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string ColumnCaption {
            get {
                return form.columnCaption;
            }
            set {
                form.columnCaption = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string ColumnCaptionClass {
            get {
                return form.columnCaptionClass;
            }
            set {
                form.columnCaptionClass = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override string ColumnCellClass {
            get {
                return form.columnCellClass;
            }
            set {
                form.columnCellClass = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override bool ColumnSortable {
            get {
                return form.columnSortable;
            }
            set {
                form.columnSortable = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override bool ColumnVisible {
            get {
                return form.columnVisible;
            }
            set {
                form.columnVisible = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override bool ColumnDownloadable {
            get {
                return form.columnDownloadable;
            }
            set {
                form.columnDownloadable = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override bool ExcludeRowFromDownload {
            get {
                return form.excludeRowFromDownload;
            }
            set {
                form.excludeRowFromDownload = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override bool AddCsvDownloadCurrentPage {
            get {
                return form.addCsvDownloadCurrentPage;
            }
            set {
                form.addCsvDownloadCurrentPage = value;
            }
        }
        //
        // ====================================================================================================
        //
        public override void addColumn() {
            form.addColumn();
        }

        //
        // ====================================================================================================
        //
        public override void AddFormButton(string buttonValue) {
            form.addFormButton(buttonValue);
        }

        //
        // ====================================================================================================
        //
        public override void AddFormButton(string buttonValue, string buttonName) {
            form.addFormButton(buttonValue,buttonName);
        }

        //
        // ====================================================================================================
        //
        public override void AddFormButton(string buttonValue, string buttonName, string buttonId) {
            form.addFormButton(buttonValue, buttonName, buttonId);
        }

        //
        // ====================================================================================================
        //
        public override void AddFormButton(string buttonValue, string buttonName, string buttonId, string buttonClass) {
            form.addFormButton(buttonValue, buttonName, buttonId, buttonClass);
        }

        //
        // ====================================================================================================
        //
        public override void AddFormHidden(string name, string value) {
            form.addFormHidden(name, value);
        }

        //
        // ====================================================================================================
        //
        public override void AddFormHidden(string name, int value) {
            form.addFormHidden(name, value);
        }

        //
        // ====================================================================================================
        //
        public override void AddFormHidden(string name, double value) {
            form.addFormHidden(name, value);
        }

        //
        // ====================================================================================================
        //
        public override void AddFormHidden(string name, DateTime value) {
            form.addFormHidden(name, value);
        }

        //
        // ====================================================================================================
        //
        public override void AddFormHidden(string name, bool value) {
            form.addFormHidden(name, value);
        }

        //
        // ====================================================================================================
        //
        public override void AddRow() {
            form.addRow();
        }

        //
        // ====================================================================================================
        //
        public override void AddRowClass(string styleClass) {
            form.addRowClass(styleClass);
        }

        //
        // ====================================================================================================
        //
        public override string GetHtml(CPBaseClass cp) {
            return form.getHtml(cp);
        }

        //
        // ====================================================================================================
        //
        public override void SetCell(string content) {
            form.setCell(content);
        }
        //
        // ====================================================================================================
        //
        public override void SetCell(string content, string downloadContent) {
            form.setCell(content, downloadContent);
        }
        //
        // ====================================================================================================
        //
        public override void SetCell(int content) {
            form.setCell(content);
        }

        //
        // ====================================================================================================
        //
        public override void SetCell(int content, int downloadContent) {
            form.setCell(content, downloadContent);
        }

        //
        // ====================================================================================================
        //
        public override void SetCell(double content) {
            form.setCell(content);
        }

        //
        // ====================================================================================================
        //
        public override void SetCell(double content, double downloadContent) {
            form.setCell(content, downloadContent);
        }

        //
        // ====================================================================================================
        //
        public override void SetCell(bool content) {
            form.setCell(content);
        }

        //
        // ====================================================================================================
        //
        public override void SetCell(bool content, bool downloadContent) {
            form.setCell(content, downloadContent);
        }
        //
        // ====================================================================================================
        //

        public override void SetCell(DateTime content) {
            form.setCell(content);
        }
        //
        // ====================================================================================================
        //

        public override void SetCell(DateTime content, DateTime downloadContent) {
            form.setCell(content, downloadContent);
        }
    }
}