window.JsInteropBPaintComp = {
    alert: function (message) {
        return alert(message);
    },
    log: function (message) {
        return log(message);
    },

    GetElementBoundingClientRect: function (obj)
    {
        // its a JS helper which returns the rect 

        if (document.getElementById(obj["id"]) !== null)
        {
            let rect = document.getElementById(obj["id"]).getBoundingClientRect();

            //let myleft = rect.left.toFixed(2) + window.scrollX;
            //let mytop = rect.top.toFixed(2) + window.scrollY;

            obj["dotnethelper"].invokeMethodAsync('invokeFromjs', obj["id"], rect.left, rect.top, rect.width, rect.height, window.scrollX, window.scrollY);
            return true;
        }
        else
        {
            return false;
        }
    },

    UpdateSVGPosition: function (obj)
    {
        //if (document.getElementById(id) !== null) {

        //    let rect = document.getElementById(id).getBoundingClientRect();

        //    //let left = rect.left + window.scrollX;
        //    //let top = rect.top + window.scrollY;

        //    //let myleft = rect.left.toFixed(2) + window.scrollX;
        //    //let mytop = rect.top.toFixed(2) + window.scrollY;

        //    DotNet.invokeMethodAsync('BlazorPaintComponent', 'invokeFromjs_UpdateSVGPosition', rect.left, rect.top, rect.width, rect.height, window.scrollX, window.scrollY);

        //    return true;
        //}
        //else {
        //    return false;
        //}

        if (document.getElementById(obj["id"]) !== null) {
            let rect = document.getElementById(obj["id"]).getBoundingClientRect();

            obj["dotnethelper"].invokeMethodAsync('invokeFromjs', obj["id"], rect.left, rect.top, rect.width, rect.height, window.scrollX, window.scrollY);
            return true;
        }
        else {
            return false;
        }

    },

    SetCursor: function (cursorStyle) {
        document.body.style.cursor = cursorStyle;
        return true;
    },
};