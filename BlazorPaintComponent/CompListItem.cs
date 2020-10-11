using BlazorPaintComponent.classes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using Newtonsoft.Json;


namespace BlazorPaintComponent
{
    public class CompListItem : ComponentBase, IDisposable
    {

        [Parameter]
        public int Par_ID { get; set; } = 0;


        [Parameter]
        public ComponentBase Parent { get; set; }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {

            BPaintObject  curr_object = (Parent as CompBlazorPaint).ObjectsList.Single(x=>x.ObjectID==Par_ID);

            int k = -1;

           

            builder.OpenElement(k++, "div");
            builder.AddAttribute(k++, "id", "listitem" + Par_ID); // + Guid.NewGuid().ToString("d").Substring(1, 4));
            builder.AddAttribute(k++, "style", "width:300px;max-height:26px;position:relative;margin:5px");
            builder.OpenElement(k++, "span");

            if (curr_object.Selected)
            {
                builder.AddAttribute(k++, "style", "width:300px; position:absolute; top:0px; cursor:pointer; background-color:yellow; color:blue; border-style:solid; border-width:1px; border-color:red;");
            }
            else
            {
                builder.AddAttribute(k++, "style", "position:absolute;top:0px;cursor:pointer;");
            }


            builder.AddAttribute(k++, "onclick", new Action(() => Cmd_Item_Select(curr_object.ObjectID)));


            builder.AddContent(k++, curr_object.ObjectType.ToString() + "; guid = " + curr_object.guid);

#if DEBUG
            builder.AddContent(k++, "<br />" + JsonConvert.SerializeObject(curr_object, Formatting.Indented));
#endif

            builder.CloseElement();

            builder.CloseElement();
        

            base.BuildRenderTree(builder);
        }

        private void Cmd_Item_Select(int Par_ID)
        {
            CompBlazorPaint p = Parent as CompBlazorPaint;

            p.cmd_Clear_Selection();

            p.cmd_Clear_Editing();

            p.ObjectsList.Single(x => x.ObjectID == Par_ID).Selected = true;

            p.cmd_RefreshSVG();
        }

        public void Dispose()
        {

        }
    }
}
