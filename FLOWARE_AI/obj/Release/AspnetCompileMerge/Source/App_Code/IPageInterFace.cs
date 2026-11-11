using System;
public interface IPageInterFace
{
    void FnAssignProperty();    
    void FnCancel();
    void FnClose();
    void FnClearItems(string PrmFlag);
    void FnFindRecord();
    object FnGetGridRowCount(string PrmFlag);
    void FnGridViewBinding(string PrmFlag);
    void FnInitializeForm();
    void FnPrintRecord();
    void ManiPulateDataEvent_Clicked(object sender, EventArgs e);
}