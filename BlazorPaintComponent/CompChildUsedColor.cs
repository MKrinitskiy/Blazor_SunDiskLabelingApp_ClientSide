using BlazorSvgHelper;
using BlazorSvgHelper.Classes.SubClasses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorPaintComponent
{
    public class CompChildUsedColor : ComponentBase
    {

        [Parameter]
        public ComponentBase Parent { get; set; }

        [Parameter]
        public string Color { get; set; }

        public Action<MouseEventArgs> ActionClicked;


        private SvgHelper SvgHelper1 = new SvgHelper();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int par_id = (Parent as CompUsedColors).UsedColors_List.IndexOf(Color);
            circle c = new circle()
            {
                cx = (9 - par_id) * 30 + 15,
                cy = 15,
                r = 10,
                fill = Color,
                stroke = "black",
                stroke_width = 1,
                onclick = "notEmpty",
            };


            SvgHelper1.Cmd_Render(c, 0, builder);

            base.BuildRenderTree(builder);

        }


        protected override void OnAfterRender(bool firstRender)
        {

            SvgHelper1.ActionClicked = ComponentClicked;
            this.ActionClicked = ComponentClicked;

            (Parent as CompUsedColors).Curr_CompChildUsedColor_List.Add(this);

        }



        public void ComponentClicked(MouseEventArgs e)
        {
            Console.WriteLine("hit CompChildUsedColor.ComponentClicked()" + Environment.NewLine +
                              "Color is: " + Color + Environment.NewLine + 
                              "parent: " + Parent.ToString());
            (Parent as CompUsedColors).ColorSelected(Color);
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
