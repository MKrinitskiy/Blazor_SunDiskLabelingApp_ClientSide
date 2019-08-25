using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorWindowHelper
{
    public class BWHJsInterop
    {
        [Inject]
        static IJSRuntime JSRuntime { get; set; }

        //public static async Task<string> Prompt(string message)
        //{
           
        //    return await JSRuntime.InvokeAsync<string>(
        //        "BWHJsFunctions.showPrompt",
        //        message);
        //}

        //public static async Task<bool> Alert(string message)
        //{
        //    return await JSRuntime.InvokeAsync<bool>(
        //        "BWHJsFunctions.alert",message);
            
        //}

        //public static async Task<bool> Log(string message)
        //{
        //    return await JSRuntime.InvokeAsync<bool>(
        //        "BWHJsFunctions.log", message);
        //
        //}

        //public static async Task<bool> LogWithTime(string message)
        //{
        //    return await JSRuntime.InvokeAsync<bool>(
        //        "BWHJsFunctions.logWithTime", message);
        //
        //}

        public static async Task<bool> SetOnOrOff(bool OnOrOff)
        {
            return await JSRuntime.InvokeAsync<bool>(
                "BWHJsFunctions.setOnOrOff", OnOrOff);

        }
        
    }
}
