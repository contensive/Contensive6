
using System;
using System.Linq;
using System.Collections.Generic;
using Contensive.Processor.Controllers;
using static Contensive.Processor.Controllers.GenericController;
using Contensive.Processor.Exceptions;
using Contensive.Models.Db;
//
namespace Contensive.Processor.Addons.PageManager {
    public class SaveChildPageListDraggableClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// pageManager addon interface. decode: "sortlist=childPageList_{parentId}_{listName},page{idOfChild},page{idOfChild},etc"
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            string returnHtml = "";
            try {
                CoreController core = ((CPClass)cp).core;
                string pageCommaList = cp.Doc.GetText("sortlist");
                List<string> pageList = new List<string>(pageCommaList.Split(','));
                if (pageList.Count > 1) {
                    string[] ParentPageValues = pageList[0].Split('_');
                    if (ParentPageValues.Count() < 3) {
                        //
                        // -- parent page is not valid
                        cp.Site.ErrorReport(new ArgumentException("pageResort requires first value to identify the parent page"));
                    } else {
                        int parentPageId = encodeInteger(ParentPageValues[1]);
                        if (parentPageId == 0) {
                            //
                            // -- parent page is not valid
                            cp.Site.ErrorReport(new ArgumentException("pageResort requires a parent page id"));
                        } else {
                            //
                            // -- create childPageIdList
                            List<int> childPageIdList = new List<int>();
                            foreach (string PageIDText in pageList) {
                                int pageId = encodeInteger(PageIDText.Replace("page", ""));
                                if (pageId > 0) {
                                    childPageIdList.Add(pageId);
                                }
                            }
                            //
                           PageContentModel parentPage = DbBaseModel.create<PageContentModel>(core.cpParent, parentPageId );
                            if (parentPage == null) {
                                //
                                // -- parent page is not valid
                                cp.Site.ErrorReport(new ArgumentException("pageResort requires a parent page id"));
                            } else {
                                //
                                // -- verify page set to required sort method Id
                                SortMethodModel sortMethod = DbBaseModel.createByUniqueName<SortMethodModel>(core.cpParent, "By Alpha Sort Order Field");
                                if (sortMethod == null) {
                                    sortMethod = DbBaseModel.createByUniqueName<SortMethodModel>(core.cpParent, "Alpha Sort Order Field");
                                }
                                if (sortMethod == null) {
                                    //
                                    // -- create the required sortMethod
                                    sortMethod = DbBaseModel.addDefault<SortMethodModel>(core.cpParent, Processor.Models.Domain.ContentMetadataModel.getDefaultValueDict(core, SortMethodModel.tableMetadata.contentName));
                                    sortMethod.name = "By Alpha Sort Order Field";
                                    sortMethod.orderByClause = "sortOrder";
                                    sortMethod.save(core.cpParent);
                                }
                                if (parentPage.childListSortMethodId != sortMethod.id) {
                                    //
                                    // -- update page if not set correctly
                                    parentPage.childListSortMethodId = sortMethod.id;
                                    parentPage.save(core.cpParent);
                                }
                                int pagePtr = 0;
                                foreach (var childPageId in childPageIdList) {
                                    if (childPageId == 0) {
                                        //
                                        // -- invalid child page
                                        cp.Site.ErrorReport(new GenericException("child page id is invalid from remote request [" + pageCommaList + "]"));
                                    } else {
                                        string SortOrder = (100000 + (pagePtr * 10)).ToString();
                                       PageContentModel childPage = DbBaseModel.create<PageContentModel>(core.cpParent, childPageId);
                                        if (childPage.sortOrder != SortOrder) {
                                            childPage.sortOrder = SortOrder;
                                            childPage.save(core.cpParent);
                                        }
                                    }
                                    pagePtr += 1;
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return returnHtml;
        }
    }
}
