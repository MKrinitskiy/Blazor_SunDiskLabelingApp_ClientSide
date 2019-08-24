using BlazorSvgHelper;
using BlazorSvgHelper.Classes.SubClasses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPaintComponent
{
    public class CompUsedColors_Logic : ComponentBase, IDisposable
    {
        [Parameter]
        protected ComponentBase parent { get; set; }

        public List<string> UsedColors_List = new List<string>(){ "#008000", "#FFFFFF","#FF0000", "#0000FF", "#FFFF00", "#808080", "#C0C0C0","#A52A2A", "#FFD700", "#000000"};
        
        public List<CompChildUsedColor> Curr_CompChildUsedColor_List = new List<CompChildUsedColor>();

        public Action<string> ActionColorClicked { get; set; }


        protected override void OnInitialized()
        {
            //for (int i = 0; i < UsedColors_List.Count; i++)
            //{
            //    UsedColors_List[i] = Get_Hex_Code_From_Color_Name(UsedColors_List[i]);
            //}

            base.OnInitialized();
        }


        private string Get_Hex_Code_From_Color_Name(string name)
        {
           
            Color c = Color.FromName(name);

            return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B); ;

        }



        public void ColorSelected(string a)
        {
            Console.WriteLine("CompUsedColors.ColorSelected() hit; string a = " + a);
            Console.WriteLine("CompUsedColors: ActionColorClicked is set: " + (ActionColorClicked != null));
            ActionColorClicked?.Invoke(a);
            //Console.WriteLine("CompUsedColors.ColorSelected() hit; string a = " + a);
            //Console.WriteLine("parent ActionColorClicked is set: " + ((parent as CompBlazorPaint).ActionColorClicked != null));
            //(parent as CompBlazorPaint).ActionColorClicked?.Invoke(a);
        }


        public void Refresh()
        {
            StateHasChanged();
        }


        public void Dispose()
        {

        }
    }
}
