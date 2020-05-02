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

        
        public static ValueTask<string> alert(string message)
        {

            return jsRuntime.InvokeAsync<string>(
                "JsInteropBPaintComp.alert",
                message);
        }


        
        public static ValueTask<string> log(string message)
        {

            return jsRuntime.InvokeAsync<string>(
                "JsInteropBPaintComp.log",
                message);
        }

        
        public static ValueTask<bool> GetElementBoundingClientRect(string id, object dotnethelper)
        {
            return jsRuntime.InvokeAsync<bool>(
                    "JsInteropBPaintComp.GetElementBoundingClientRect",
                    new { id, dotnethelper });

        }

        
        public static ValueTask<bool> UpdateSVGPosition(string id, object dotnethelper)
        {
            //Console.WriteLine("hit UpdateSVGPosition");
            return jsRuntime.InvokeAsync<bool>(
                "JsInteropBPaintComp.UpdateSVGPosition", new { id, dotnethelper });
        }


        public static ValueTask<bool> SetCursor(string cursorStyle = "default")
        {

            return jsRuntime.InvokeAsync<bool>(
                "JsInteropBPaintComp.SetCursor",
                cursorStyle);
        }
    }
}
