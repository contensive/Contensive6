
using Contensive.BaseClasses;
using Contensive.Processor.Controllers;
using System;

namespace Contensive.Processor.AdminUI {
    public class CPAdminUIToolFormClass : BaseClasses.AdminUI.ToolFormBaseClass {
        //
        public CPAdminUIToolFormClass( CoreController core ) {
            this.core = core;
            this.form = new ToolFormController();
        }
        //
        private readonly CoreController core;
        //
        private readonly ToolFormController form;
        //
        public override bool IncludeBodyPadding {
            get {
                return form.includeBodyPadding;
            }
            set {
                form.includeBodyPadding = value;
            }
        }
        public override bool IncludeBodyColor {
            get {
                return form.includeBodyPadding;
            }
            set {
                form.includeBodyPadding = value;
            }
        }
        public override bool IsOuterContainer {
            get {
                return form.includeBodyPadding;
            }
            set {
                form.includeBodyPadding = value;
            }
        }
        public override string Title {
            get {
                return form.title;
            }
            set {
                form.title = value;
            }
        }
        public override string SuccessMessage {
            get {
                return form.successMessage;
            }
            set {
                form.successMessage = value;
            }
        }
        public override string InfoMessage {
            get {
                return form.infoMessage;
            }
            set {
                form.infoMessage = value;
            }
        }
        public override string WarningMessage {
            get {
                return form.warningMessage;
            }
            set {
                form.warningMessage = value;
            }
        }
        public override string FailMessage {
            get {
                return form.failMessage;
            }
            set {
                form.failMessage = value;
            }
        }
        public override string Description {
            get {
                return form.description;
            }
            set {
                form.description = value;
            }
        }
        public override string FormActionQueryString {
            get {
                return form.formActionQueryString;
            }
            set {
                form.formActionQueryString = value;
            }
        }
        public override string FormId {
            get {
                return form.formId;
            }
            set {
                form.formId = value;
            }
        }
        public override string Body {
            get {
                return form.body;
            }
            set {
                form.body = value;
            }
        }
        public override string Footer {
            get {
                return form.body;
            }
            set {
                form.body = value;
            }
        }

        public override void AddFormButton(string buttonValue) 
            => form.addFormButton(buttonValue);

        public override void AddFormButton(string buttonValue, string buttonName)
            => form.addFormButton(buttonValue,buttonName);

        public override void AddFormButton(string buttonValue, string buttonName, string buttonId)
            => form.addFormButton(buttonValue,buttonValue,buttonId);

        public override void AddFormButton(string buttonValue, string buttonName, string buttonId, string buttonClass)
            => form.addFormButton(buttonValue, buttonValue, buttonId,buttonClass);

        public override void AddFormHidden(string name, string value)
            => form.addFormHidden(name, value);

        public override void AddFormHidden(string name, int value)
            => form.addFormHidden(name, value);

        public override void AddFormHidden(string name, double value)
            => form.addFormHidden(name, value);

        public override void AddFormHidden(string name, DateTime value)
            => form.addFormHidden(name, value);

        public override void AddFormHidden(string name, bool value)
            => form.addFormHidden(name, value);

        public override string GetHtml(CPBaseClass cp)
            => form.getHtml(core.cpParent);
    }
}