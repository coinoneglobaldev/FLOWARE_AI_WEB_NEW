function ValFailed(ress) {
    alert(ress.get_message());
}
function FnGetCustomerName(PrmCustCode, PrmDestControl) {
    var cCustCode = document.getElementById(PrmCustCode).value;
    Web_GetResultService.FnGetCustomerName(cCustCode, ValCustNameSuccess, ValFailed, PrmDestControl);
}

function ValCustNameSuccess(ress, PrmDestControl)
{
    var strDest = PrmDestControl.split(",");
    document.getElementById(strDest[0]).value = ress[0];
}
var _updTimers = {};

function FnUpdateOrderDetails(PrmToken, PrmId, PrmItemIdCtrl, PrmUomIdCtrl, PrmQtyCtrl, PrmRateCtrl, PrmRemarks, PrmCode, PrmDestControl) {
    clearTimeout(_updTimers[PrmId]);               // cancel earlier pending call for this row
    _updTimers[PrmId] = setTimeout(function () {
        var nItemId = document.getElementById(PrmItemIdCtrl).value;
        var nUomId = document.getElementById(PrmUomIdCtrl).value;
        var nQty = document.getElementById(PrmQtyCtrl).value;
        var nRate = document.getElementById(PrmRateCtrl).value;
        var cRemarks = document.getElementById(PrmRemarks).value;

        console.log('save', PrmId, nItemId, nUomId);   // remove after testing

        Web_GetResultService.FnUpdateOrderDetails(PrmToken, PrmId, nItemId, nUomId, nQty, nRate, cRemarks,
            ValUpdateOrderDetailsSuccess, ValFailed, PrmDestControl);
    }, 400);
}

function ValUpdateOrderDetailsSuccess(ress, PrmDestControl)
{
    document.getElementById(PrmDestControl).value = ress;
}