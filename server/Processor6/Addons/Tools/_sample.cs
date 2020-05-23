
using System;
using Contensive.Processor.Controllers;
using Contensive.Processor.Models.Domain;
using static Contensive.Processor.Constants;
using System.Text;
using System.Collections.Generic;
//
namespace Contensive.Processor.Addons.Tools {
    //
    public class SampleToolClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// addon method, deliver complete Html admin site
        /// blank return on OK or cancel button
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cpBase) {
            return sampleTool((CPClass)cpBase);
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        //
        public static string sampleTool(CPClass cp) {
            string result = "";
            CoreController core = cp.core;
            try {
                var resultForm = new StringBuilder();
                resultForm.Append(AdminUIController.getHeaderTitleDescription("Sample Tool", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Erat imperdiet sed euismod nisi. In vitae turpis massa sed elementum tempus egestas sed sed. Tortor consequat id porta nibh. Pulvinar sapien et ligula ullamcorper malesuada. Facilisi nullam vehicula ipsum a. Nibh venenatis cras sed felis eget. Lectus magna fringilla urna porttitor rhoncus dolor. Auctor urna nunc id cursus metus aliquam. Gravida neque convallis a cras semper auctor neque. Faucibus nisl tincidunt eget nullam non nisi est sit amet. In nisl nisi scelerisque eu ultrices vitae auctor eu augue. In egestas erat imperdiet sed euismod nisi. Adipiscing diam donec adipiscing tristique. Ullamcorper eget nulla facilisi etiam dignissim diam quis enim. Sed libero enim sed faucibus turpis in eu. Ultrices neque ornare aenean euismod elementum nisi quis eleifend."));
                //
                // process form
                string button = cp.Doc.GetText("button");
                string PageSize = cp.Doc.GetText("pageSize");
                int countryId = cp.Doc.GetInteger("countryId");
                var buttonList = new List<string> { ButtonCancel };
                if (button == ButtonCancel) {
                    //
                    // Cancel just exits with no content
                    //
                    return string.Empty;
                } else if (!core.session.isAuthenticatedAdmin()) {
                    //
                    // Not Admin Error
                    //
                    resultForm.Append(AdminUIController.getFormBodyAdminOnly());
                } else {
                    //
                    // Process Requests
                    //
                    switch (button) {
                        case ButtonSave:
                        case ButtonOK: {
                                // 
                                // perform action
                                break;
                            }
                        default: {
                                // nothing
                                break;
                            }
                    }
                    if (button.Equals(ButtonOK)) {
                        //
                        // Exit on OK
                        //
                        return string.Empty;
                    }
                }
                //
                // display form
                resultForm.Append(cp.Html5.P("Enter sample data."));
                resultForm.Append(HtmlController.inputTextarea(core, "sampleText", "", 10));
                //
                // Buttons
                //
                buttonList.Add(ButtonSave);
                buttonList.Add(ButtonOK);
                //
                // Close Tables
                //
                resultForm.Append(HtmlController.inputHidden(rnAdminSourceForm, AdminFormSecurityControl));
                bool isEmptyList = false;
                resultForm.Append(AdminUIController.getToolFormInputRow(core, "Caption", AdminUIEditorController.getLookupContentEditor(core, "countryId", countryId, ContentMetadataModel.getContentId(core, "countries"), ref isEmptyList, false, "", "", false, "")));
                resultForm.Append(AdminUIController.getToolFormInputRow(core, "Caption", AdminUIEditorController.getTextEditor(core, "PageSize", PageSize.ToString())));
                //
                // -- assemble form
                result = AdminUIController.getToolForm(core, resultForm.ToString(), String.Join(",", buttonList));
            } catch (Exception ex) {
                LogController.logError(core, ex);
                throw;
            }
            return result;
        }
    }
}

