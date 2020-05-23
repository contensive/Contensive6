
using Contensive.Models.Db;
using Contensive.Processor.Controllers;
using System;
using System.Collections.Generic;

namespace Contensive.Processor.Addons.AdminSite.Models {
    /// <summary>
    /// 
    /// </summary>
    public class EditRecordModel {
        public Dictionary<string, EditRecordFieldModel> fieldsLc = new Dictionary<string, EditRecordFieldModel>();
        /// <summary>
        /// ID field of edit record (Record to be edited)
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// ParentID field of edit record (Record to be edited)
        /// </summary>
        public int parentId { get; set; }
        /// <summary>
        /// name field of edit record
        /// </summary>
        public string nameLc { get; set; }
        /// <summary>
        /// active field of the edit record
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// ContentControlID of the edit record
        /// </summary>
        public int contentControlId { get; set; }
        /// <summary>
        /// denormalized name from contentControlId property
        /// </summary>
        public string contentControlId_Name { get; set; }
        /// <summary>
        /// Used for Content Watch Link Label if default
        /// </summary>
        public string menuHeadline { get; set; }
        /// <summary>
        /// Used for control section display
        /// </summary>
        public DateTime modifiedDate { get; set; }
        public PersonModel modifiedBy { get; set; }
        public DateTime dateAdded { get; set; }
        public PersonModel createdBy { get; set; }
        /// <summary>
        /// true/false - set true when the field array values are loaded
        /// </summary>
        public bool loaded { get; set; }
        /// <summary>
        /// true if edit record was saved during this page
        /// </summary>
        public bool saved { get; set; }
        /// <summary>
        /// set if this record can not be edited, for various reasons
        /// </summary>
        public bool userReadOnly { get; set; }
        /// <summary>
        /// true means the edit record has been deleted
        /// </summary>
        public bool isDeleted { get; set; }
        /// <summary>
        /// set if Workflow authoring insert
        /// </summary>
        public bool isInserted { get; set; }
        /// <summary>
        /// record has been modified since last published
        /// </summary>
        public bool isModified { get; set; }
        /// <summary>
        /// member who first edited the record
        /// </summary>
        public string lockModifiedName { get; set; }
        /// <summary>
        /// Date when member modified record
        /// </summary>
        public DateTime lockModifiedDate { get; set; }
        /// <summary>
        /// set if a submit Lock, even if the current user is admin
        /// </summary>
        public bool submitLock { get; set; }
        /// <summary>
        /// member who submitted the record
        /// </summary>
        public string submittedName { get; set; }
        /// <summary>
        /// Date when record was submitted
        /// </summary>
        public DateTime submittedDate { get; set; }
        /// <summary>
        /// set if an approve Lock
        /// </summary>
        public bool approveLock { get; set; }
        /// <summary>
        /// member who approved the record
        /// </summary>
        public string approvedName { get; set; }
        /// <summary>
        /// Date when record was approved
        /// </summary>
        public DateTime approvedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private bool allowUserAdd;
        /// <summary>
        /// This user can add records to this content
        /// </summary>
        public bool getAllowUserAdd() {
            return allowUserAdd;
        }
        /// <summary>
        /// This user can add records to this content
        /// </summary>
        public void setAllowUserAdd(bool value) {
            allowUserAdd = value;
        }
        /// <summary>
        /// This user can save the current record
        /// </summary>
        public bool allowUserSave { get; set; }
        /// <summary>
        /// This user can delete the current record
        /// </summary>
        public bool allowUserDelete { get; set; }
        /// <summary>
        /// set if an edit Lock by anyone else besides the current user
        /// </summary>
        public WorkflowController.editLockClass editLock { get; set; }
    }
}
