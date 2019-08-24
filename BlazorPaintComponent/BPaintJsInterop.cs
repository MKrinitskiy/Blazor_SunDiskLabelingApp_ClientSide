using System;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorPaintComponent
{
    public class BPaintJsInterop : ComponentBase
    {
        [Inject]
        IJSRuntime Runtime { get; set; }

        private static IJSRuntime jsRuntime { get; set; }


        protected override void OnInitialized()
        {
            jsRuntime = Runtime;
            base.OnInitialized();
        }

        
        public static Task<string> alert(string message)
        {

            return jsRuntime.InvokeAsync<string>(
                "JsInteropBPaintComp.alert",
                message);
        }


        
        public static Task<string> log(string message)
        {

            return jsRuntime.InvokeAsync<string>(
                "JsInteropBPaintComp.log",
                message);
        }

        
        public static Task<bool> GetElementBoundingClientRect(string id, object dotnethelper)
        {
            return jsRuntime.InvokeAsync<bool>(
                    "JsInteropBPaintComp.GetElementBoundingClientRect",
                    new { id, dotnethelper });

        }

        
        public static Task<bool> UpdateSVGPosition(string id, object dotnethelper)
        {

            return jsRuntime.InvokeAsync<bool>(
                "JsInteropBPaintComp.UpdateSVGPosition", new { id, dotnethelper });
        }


        public static Task<bool> SetCursor(string cursorStyle = "default")
        {

            return jsRuntime.InvokeAsync<bool>(
                "JsInteropBPaintComp.SetCursor",
                cursorStyle);
        }
    }
}
