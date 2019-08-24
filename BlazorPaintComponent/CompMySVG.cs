using BlazorPaintComponent.classes;
using BlazorSvgHelper;
using BlazorSvgHelper.Classes.SubClasses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Geometry;

namespace BlazorPaintComponent
{
    public class CompMySVG : ComponentBase, IDisposable
    {

        [Parameter] protected ComponentBase parent { get; set; }

        [Parameter]
        protected double par_width { get; set; }

        [Parameter]
        protected double par_height { get; set; }


        svg _Svg = null;


        SvgHelper SvgHelper1 = new SvgHelper();



        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            Generate_SVG();
            SvgHelper1.Cmd_Render(_Svg, 0, builder);
            base.BuildRenderTree(builder);
        }


        public void Generate_SVG()
        {
            _Svg = new svg
            {
                id = "svgPaint",
                width = par_width,
                height = par_height,
                xmlns = "http://www.w3.org/2000/svg",
                //style = "background-color: rgba(0, 0, 0, 0.0);",
            };

            _Svg.Children.Add(new rect
            {
                width = par_width,
                height = par_height,
                fill = "none",
                stroke = "magenta",
                stroke_width = 1,
            });

            CompBlazorPaint currParent = (parent as CompBlazorPaint);





            #region drawing all the Selection rectangles

            if (currParent.bpSelectionRectangle != null)
            {
                rect bpSelectionRectangleSVGrect = new rect
                {
                    x = Math.Min((double)currParent.bpSelectionRectangle.Position.x, (double)currParent.bpSelectionRectangle.end.x),
                    y = Math.Min((double)currParent.bpSelectionRectangle.Position.y, (double)currParent.bpSelectionRectangle.end.y),
                    width = Math.Abs((double)currParent.bpSelectionRectangle.end.x - currParent.bpSelectionRectangle.Position.x),
                    height = Math.Abs((double)currParent.bpSelectionRectangle.end.y - currParent.bpSelectionRectangle.Position.y),
                    fill = "#FAFAFA",
                    stroke = currParent.bpSelectionRectangle.Color,
                    stroke_width = currParent.bpSelectionRectangle.LineWidth,
                    stroke_dasharray = "4",
                    style = "opacity:0.8",
                };
                _Svg.Children.Add(bpSelectionRectangleSVGrect);
            }
            
            #endregion




            
            #region drawing all the lines
            foreach (BPaintLine currLine in currParent.ObjectsList.Where(x => x.ObjectType == BPaintOpbjectType.Line))
            {
                line l = new line()
                {
                    x1 = currLine.Position.PtD.X,
                    y1 = currLine.Position.PtD.Y,
                    x2 = currLine.end.PtD.X,
                    y2 = currLine.end.PtD.Y,
                    stroke = currLine.Color,
                    stroke_width = currLine.LineWidth,
                };
                _Svg.Children.Add(l);

                if (currLine.Selected)
                {
                    RectD p_rect = BPaintFunctions.Get_Border_Points(currLine as BPaintLine);

                    _Svg.Children.Add(new rect
                    {
                        x = p_rect.x,
                        y = p_rect.y,
                        width = p_rect.width,
                        height = p_rect.height,
                        fill = "none",
                        stroke = "magenta",
                        stroke_width = 1,
                        style = "opacity:0.7",
                    });
                }
            }
            #endregion



            #region drawing all the circles

            foreach (BPaintCircle currCircle in currParent.ObjectsList.Where(x => x.ObjectType == BPaintOpbjectType.Circle))
            {

                circle currSVGCircle = new circle()
                {
                    cx = currCircle.Position.x,
                    cy = currCircle.Position.y,
                    r = currCircle.Position.PtD.DistanceTo(currCircle.end.PtD),
                    fill = "none",
                    stroke = currCircle.Color,
                    stroke_width = 2,
                };
                line circleAuxiliaryLine = new line()
                {
                    x1 = currCircle.Position.x,
                    y1 = currCircle.Position.y,
                    x2 = currCircle.end.x,
                    y2 = currCircle.end.y,
                    stroke = "magenta",
                    stroke_width = 1,
                    style = "opacity:0.7"
                };
                _Svg.Children.Add(currSVGCircle);
                _Svg.Children.Add(circleAuxiliaryLine);

                if (currCircle.Selected)
                {
                    RectD p_rect = BPaintFunctions.Get_Border_Points(currCircle);

                    _Svg.Children.Add(new rect
                    {
                        x = p_rect.x,
                        y = p_rect.y,
                        width = p_rect.width,
                        height = p_rect.height,
                        fill = "none",
                        stroke = "magenta",
                        stroke_width = 1,
                        style = "opacity:0.7",
                    });
                }
            }

            #endregion




            
            #region drawing all the ellipses

            foreach (BPaintEllipse currEllipse in currParent.ObjectsList.Where(x => x.ObjectType == BPaintOpbjectType.Ellipse))
            {
                if (currEllipse.IsValid())
                {
                    ellipse currSVGCircle = currEllipse.SvgEllipseDescription();
                    _Svg.Children.Add(currSVGCircle);

                    if (currEllipse.Selected)
                    {
                        RectD bRect = BPaintFunctions.Get_Border_Points(currEllipse);

                        _Svg.Children.Add(new rect
                        {
                            x = bRect.x,
                            y = bRect.y,
                            width = bRect.width,
                            height = bRect.height,
                            fill = "none",
                            stroke = "magenta",
                            stroke_width = 1,
                            style = "opacity:0.7",
                        });
                    }

                    line ellipseAuxiliaryLine1 = new line()
                    {
                        x1 = currEllipse.Position.x,
                        y1 = currEllipse.Position.y,
                        x2 = currEllipse.pt2.x,
                        y2 = currEllipse.pt2.y,
                        stroke = "magenta",
                        stroke_width = 1,
                        style = "opacity:0.7"
                    };
                    line ellipseAuxiliaryLine2 = new line()
                    {
                        x1 = currEllipse.pt2.x,
                        y1 = currEllipse.pt2.y,
                        x2 = currEllipse.pt3.x,
                        y2 = currEllipse.pt3.y,
                        stroke = "magenta",
                        stroke_width = 1,
                        style = "opacity:0.7"
                    };
                    line ellipseAuxiliaryLine3 = new line()
                    {
                        x1 = currEllipse.Position.x,
                        y1 = currEllipse.Position.y,
                        x2 = currEllipse.pt3.x,
                        y2 = currEllipse.pt3.y,
                        stroke = "magenta",
                        stroke_width = 1,
                        style = "opacity:0.7"
                    };
                    _Svg.Children.Add(ellipseAuxiliaryLine1);
                    _Svg.Children.Add(ellipseAuxiliaryLine2);
                    _Svg.Children.Add(ellipseAuxiliaryLine3);

                }

            }

            #endregion




            #region Drawing all the Selection region vertices
            foreach (BPaintVertex vertex in currParent.SelectionVerticesList)
            {
                circle currVertexCircle = new circle()
                {
                    cx = vertex.PtD.X,
                    cy = vertex.PtD.Y,
                    r = 5,
                    fill = "none",
                    stroke = vertex.Color,
                    stroke_width = 2,
                };
                _Svg.Children.Add(currVertexCircle);

                if (vertex.Selected)
                {
                    RectD p_rect = BPaintFunctions.Get_Border_Points(vertex as BPaintVertex);

                    rect currVertexBoundingRect = new rect
                    {
                        x = p_rect.x,
                        y = p_rect.y,
                        width = p_rect.width,
                        height = p_rect.height,
                        fill = "none",
                        stroke = "magenta",
                        stroke_width = 1,
                        style = "opacity:0.7",
                    };
                    _Svg.Children.Add(currVertexBoundingRect);
                }
            }
            #endregion




            #region Drawing all the vertices
            foreach (BPaintVertex vertex in currParent.VerticesList)
            {
                circle currVertexCircle = new circle()
                {
                    cx = vertex.PtD.X,
                    cy = vertex.PtD.Y,
                    r = 5,
                    fill = "wheat",
                    stroke = vertex.Color,
                    stroke_width = 2,
                };
                _Svg.Children.Add(currVertexCircle);

                if (vertex.Selected)
                {
                    RectD p_rect = BPaintFunctions.Get_Border_Points(vertex as BPaintVertex);

                    rect currVertexBoundingRect = new rect
                    {
                        x = p_rect.x,
                        y = p_rect.y,
                        width = p_rect.width,
                        height = p_rect.height,
                        fill = "none",
                        stroke = "magenta",
                        stroke_width = 1,
                        style = "opacity:0.7",
                    };
                    _Svg.Children.Add(currVertexBoundingRect);
                }
            }
            #endregion



        }



        //private line drawLine(BPaintLine Par_Object)
        //{
        //    line l = new line()
        //    {
        //        x1 = Par_Object.Position.Position.X,
        //        y1 = Par_Object.Position.Position.Y,
        //        x2 = Par_Object.end.Position.X,
        //        y2 = Par_Object.end.Position.Y,
        //        stroke = Par_Object.Color,
        //        stroke_width = Par_Object.LineWidth,
        //    };
        //
        //    //if (Par_Object.Scale.X!=0 || Par_Object.Scale.Y!=0)
        //    //{
        //    //    l.transform = "scale(" + Par_Object.Scale.X + "," + Par_Object.Scale.Y + ")";
        //    //}
        //    if (Par_Object.transform != null)
        //    {
        //        l.transform = Par_Object.transform.ToString();
        //    }
        //
        //    return l;
        //}




        public void Refresh()
        {
            StateHasChanged();
        }


        public void Dispose()
        {
        }

    }
}
