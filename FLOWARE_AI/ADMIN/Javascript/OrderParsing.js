var _valDesc = Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, 'value');
window._lastPicked = '';

function FnHookSelectors() {
    document.querySelectorAll('input[data-onselect]').forEach(function (txt) {
        var h = document.getElementById(txt.getAttribute('data-hdn'));
        if (!h || h._hooked) return;
        h._hooked = true;
        var js = txt.getAttribute('data-onselect');

        Object.defineProperty(h, 'value', {
            get: function () { return _valDesc.get.call(this); },
            set: function (v) {
                var old = _valDesc.get.call(this);
                _valDesc.set.call(this, v);
                if (v !== old && !h._silent && v !== '' && v !== '0') {
                    window._lastPicked = h.id;               // remember which control changed
                    new Function(js)();                       // debounced inside FnUpdateOrderDetails
                }
            }
        });
    });
}

// raw set that does not trigger the hook
function FnSetHdnSilent(id, v) {
    var h = document.getElementById(id);
    if (!h) return;
    h._silent = true; h.value = v; h._silent = false;
}

if (window.Sys && Sys.Application) {
    Sys.Application.add_load(FnHookSelectors);   // covers UpdatePanel
} else {
    document.addEventListener('DOMContentLoaded', FnHookSelectors);
}