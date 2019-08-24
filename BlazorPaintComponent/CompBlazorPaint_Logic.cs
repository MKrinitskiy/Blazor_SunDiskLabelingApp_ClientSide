using BlazorPaintComponent.classes;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using BlazorSvgHelper.Classes.SubClasses;
using Geometry;
using Blazor2PythonWebAPI_interfaces;
using CommonInterfaces;
using Newtonsoft.Json;
using CommonLibs;

namespace BlazorPaintComponent
{
    public class CompBlazorPaint_Logic : ComponentBase
    {
        
        // TODO: Objects list appearance
        // category=Apeearance issue=none estimate=2hr
        // The label "Objects list": align it with the labels of service information like "Current operational mode" etc.
        // Rename "Objects list" to "Labels list" or "Labels"
        // Make the list of labels visually the same as service information: div, outline
        


        bool IsCompLoadedAtLeastOnce = false;


        public OperationalMode CurrOperationalMode = OperationalMode.select;
        public BPaintMode CurrPaintMode = BPaintMode.idle;
        public SelectionMode CurrSelectionMode = SelectionMode.idle;

        

        protected double Curr_Scale_X = 1.0;
        protected double Curr_Scale_Y = 1.0;

        protected CompUsedColors_Logic Curr_CompUsedColors = new CompUsedColors_Logic();
        protected CompMySVG Curr_CompMySVG = new CompMySVG();


        public List<BPaintObject> ObjectsList = new List<BPaintObject>();
        public BPaintRectangle bpSelectionRectangle = null;
        public List<BPaintVertex> VerticesList = new List<BPaintVertex>();
        public List<BPaintVertex> SelectionVerticesList = new List<BPaintVertex>();


        protected string Color1 = "#ffffff";//"#fc3807";

        protected int LineWidth1 = 3;

        protected int StepSize = 5;

        public BPaintVertex bpVertexUnderMousePointer = null;
        public BPaintVertex bpSelectionVertexUnderMousePointer = null;

        protected int FigureCode = 2;

        protected PointD currMouseLocation = PointD.nullPointD();
        protected string strCurrMouseLocation = "";

        protected string strCurrSVGareaLocation =>
            string.Format("x: {0}; y: {1}", LocalData.SVGPosition.X, LocalData.SVGPosition.Y);


        protected string CurrentBackgroundImageURI = "";
        private string guid = (Guid.NewGuid()).ToString().Replace("-", "");
        private readonly HttpClient http = new HttpClient();
        private string base_webAPI_uri = "http://127.0.0.1:2019/";
        protected bool webAPIsessionStarted = false;

        protected string btnSelectMode_CSSclass = "btn btn-primary btn-with-icons";
        protected string btnDrawMode_CSSclass = "btn btn-outline-primary btn-with-icons";
        protected bool btnSelectAllDisabled { get; set; }
        protected bool btnDeleteSelectedDisabled { get; set; }



        protected override Task OnInitializedAsync()
        {
            BlazorWindowHelper.BlazorWindowHelper.Initialize();
            BlazorWindowHelper.BWHJsInterop.SetOnOrOff(true);
            BlazorWindowHelper.BlazorWindowHelper.OnScroll = OnScroll;
            BlazorWindowHelper.BlazorWindowHelper.OnResize = OnScroll;

            CurrentBackgroundImageURI = "";

            CurrOperationalMode = OperationalMode.select;
            OperationalModeHasChanged(); 
            
            return base.OnInitializedAsync();
        }




        protected void OnScroll()
        {
            Console.WriteLine("hit OnScroll");
            BPaintJsInterop.UpdateSVGPosition("PaintArea1", DotNetObjectRef.Create(this));
            StateHasChanged();
        }



        protected override void OnAfterRender()
        {
            if (!IsCompLoadedAtLeastOnce)
            {
                if (Curr_CompUsedColors.ActionColorClicked == null)
                {
                    Console.WriteLine("Curr_CompUsedColors.ActionColorClicked == null: " + (Curr_CompUsedColors.ActionColorClicked == null));
                    Console.WriteLine("setting Curr_CompUsedColors.ActionColorClicked with ColorSelected");
                    Curr_CompUsedColors.ActionColorClicked = ColorSelected;
                    Console.WriteLine("set Curr_CompUsedColors.ActionColorClicked with ColorSelected");
                    Console.WriteLine("Curr_CompUsedColors.ActionColorClicked == null: " + (Curr_CompUsedColors.ActionColorClicked == null));
                }

                GetBoundingClientRect("PaintArea1");

                IsCompLoadedAtLeastOnce = true;
            }
            
            base.OnAfterRender();
        }




        #region operational modes

        protected void btnSelectMode_onClick(UIMouseEventArgs eventArgs)
        {
            OperationalMode prevMode = CurrOperationalMode;
            CurrOperationalMode = OperationalMode.select;
            if (CurrOperationalMode != prevMode)
            {
                OperationalModeHasChanged();
            }
        }


        protected void btnDrawMode_onClick(UIMouseEventArgs eventArgs)
        {
            OperationalMode prevMode = CurrOperationalMode;
            CurrOperationalMode = OperationalMode.draw;
            if (CurrOperationalMode != prevMode)
            {
                OperationalModeHasChanged();
            }
        }


        protected void OperationalModeHasChanged()
        {
            btnDrawMode_CSSclass = (CurrOperationalMode == OperationalMode.draw)?("btn btn-primary btn-with-icons") :("btn btn-outline-primary btn-with-icons");
            btnSelectMode_CSSclass = (CurrOperationalMode == OperationalMode.select) ? ("btn btn-primary btn-with-icons") : ("btn btn-outline-primary btn-with-icons");

            if (CurrOperationalMode == OperationalMode.draw)
            {
                CurrPaintMode = BPaintMode.idle;
                btnDeleteSelectedDisabled = true;
                btnSelectAllDisabled = true;
            }
            else
            {
                CurrSelectionMode = SelectionMode.idle;
                btnDeleteSelectedDisabled = false;
                btnSelectAllDisabled = false;
            }
            
            StateHasChanged();
        }

        #endregion






        #region web API functions
        protected void btnPrevExample_onClick()
        {
            StateHasChanged();
        }


        protected void btnSaveCurrExampleLabels_onClick()
        {
            StateHasChanged();
        }


        protected async void btnStartWebAPIsession_onClick()
        {
            #region starting WebAPI session

            string url = new Uri(base_webAPI_uri).Append("exec?command=start&webapi_client_id="+ guid).AbsoluteUri;
            Console.WriteLine("btnStartWebAPIsession_onClick: HTTP GET");
            Console.WriteLine("URL = " + url);
            HttpClient http = new HttpClient();
            string strRepl = await http.GetStringAsync(url);

            WebAPI_response resp = null;
            try
            {
                resp = JsonConvert.DeserializeObject<WebAPI_response>(strRepl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (resp == null)
            {
                Console.WriteLine("ERROR: failed deserializing WebAPI response");
                return;
            }

            if (resp.ResponseCode == ResponseCodes.Error)
            {
                Console.WriteLine("ERROR : failed starting a WebAPI session:");
                Console.WriteLine("Error code: " + resp.Error.ErrorCode);
                Console.WriteLine("Error message: " + resp.Error.ErrorDescription);
                return;
            }
            else if (resp.ResponseCode == ResponseCodes.OK)
            {
                Console.WriteLine("OK : WebAPI session started successfully!");
                webAPIsessionStarted = true;
            }

            #endregion

            RequestNextExample();
        }


        protected void btnnextExample_onClick()
        {
            RequestNextExample();
        }

        protected async void RequestNextExample()
        {
            #region request for a first image

            string url = new Uri(base_webAPI_uri).Append("images?command=get_next_image&webapi_client_id=" + guid).AbsoluteUri;
            Console.WriteLine("btnStartWebAPIsession_onClick: HTTP GET");
            Console.WriteLine("URL = " + url);
            string strRepl = await http.GetStringAsync(url);

            WebAPI_response resp = null;
            try
            {
                resp = JsonConvert.DeserializeObject<WebAPI_response>(strRepl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (resp == null)
            {
                Console.WriteLine("ERROR: failed deserializing WebAPI response");
                return;
            }

            try
            {
                CurrentBackgroundImageURI = resp.StringAttributes["imageURL"];
                Console.WriteLine("got CurrentBackgroundImageURI value: " + CurrentBackgroundImageURI);
                CurrentBackgroundImageURI = new Uri(base_webAPI_uri).Append(CurrentBackgroundImageURI).AbsoluteUri;
                Console.WriteLine("now CurrentBackgroundImageURI: " + CurrentBackgroundImageURI);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : failed extracting CurrentBackgroundImageURI from JSON response.");
                Console.WriteLine("got JSON response:");
                Console.Write(strRepl);
                Console.WriteLine("converted it to the WebAPI_response instance:");
                Console.Write(resp.ToJSON());
                Console.WriteLine(e);
                return;
            }
            
            StateHasChanged();

            //string SunDiskData_str = resp.StringAttributes["SunDisk_RoundDataWithUnderlyingImgSize"];
            //SunDiskData_str = SunDiskData_str.Replace("\\\"", "\"");
            //string imgSize_str = resp.StringAttributes["ImgSize_"];
            //imgSize_str = imgSize_str.Replace("\\\"", "\"");

            //Console.WriteLine("SunDiskData_str:");
            //Console.WriteLine(SunDiskData_str);
            //Console.WriteLine("imgSize_str:");
            //Console.WriteLine(imgSize_str);

            //Console.WriteLine("");
            //Console.WriteLine("SunDiskData xml:");
            //RoundDataWithUnderlyingImgSize SunDiskData = JsonConvert.DeserializeObject<RoundDataWithUnderlyingImgSize>(SunDiskData_str, new SizeJsonConverter());
            //Console.WriteLine(ServiceTools.XmlSerializeToString(SunDiskData));

            //Console.WriteLine("");
            //Console.WriteLine("imgSize xml:");
            //Geometry.Size imgSize = JsonConvert.DeserializeObject<Geometry.Size>(imgSize_str);
            //Console.WriteLine(ServiceTools.XmlSerializeToString(imgSize));


            //Console.WriteLine("");
            //Console.WriteLine("");
            //Geometry.Size testSize = new Geometry.Size(25,125);
            //Console.WriteLine("testSize JSON:");
            //string testSize_JSON = JsonConvert.SerializeObject(testSize);
            //Console.WriteLine(testSize_JSON);

            //Geometry.Size testSize_deserialized = JsonConvert.DeserializeObject<Geometry.Size>(testSize_JSON, new SizeJsonConverter());
            //Console.WriteLine("");
            //Console.WriteLine("testSize deserialized ToString:");
            //Console.WriteLine(testSize_deserialized.ToString());

            //Console.WriteLine("");
            //Console.WriteLine("testSize deserialized XML:");
            //Console.WriteLine(ServiceTools.XmlSerializeToString(testSize_deserialized));


            //try
            //{
            //    RoundDataWithUnderlyingImgSize SunDiskData = JsonConvert.DeserializeObject<RoundDataWithUnderlyingImgSize>(SunDiskData_str);

            //    Dictionary<Type, List<MethodInfo>> dictAlreadyFoundExtensionsMethods = new Dictionary<Type, List<MethodInfo>>();
            //    dictAlreadyFoundExtensionsMethods = RoundDataWithUnderlyingImgSize.GetExtensionsMethodsForType(typeof(RoundDataWithUnderlyingImgSize), dictAlreadyFoundExtensionsMethods);
            //    string SunDiskData_csv = SunDiskData.ToCSV(ref dictAlreadyFoundExtensionsMethods);

            //    Console.Write(SunDiskData_csv);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("ERROR : failed extracting SunDiskData from JSON response.");
            //    Console.WriteLine(e);
            //    return;
            //}

            #endregion

            //StateHasChanged();
        }

        #endregion


        private void ColorSelected(string a)
        {
            Console.WriteLine("CompBlazorPaint.ColorSelected() hit; string a = " + a);
            Color1 = a;
            
            if (ObjectsList.Any())
            {
                foreach (var item in ObjectsList.Where(x => x.Selected))
                {
                    item.Color = Color1;
                    foreach(BPaintVertex v in item.VerticesList)
                    {
                        v.Color = Color1;
                    }
                }
            }
            
            StateHasChanged();
        }


        #region // cmd_Size_Changed
        //protected void cmd_Size_Changed(UIChangeEventArgs e)
        //{
        //    if (e?.Value != null)
        //    {
        //        LineWidth1 = int.Parse(e.Value as string);

        //        //if (Curr_Mode == BPaintMode.editing)
        //        //{
        //        if (ObjectsList.Any())
        //        {
        //            foreach (var item in ObjectsList.Where(x => x.Selected))
        //            {
        //                item.LineWidth = LineWidth1;
        //            }
        //        }
        //        //}
        //        cmd_RefreshSVG();
        //    }
        //}
        #endregion


        #region // clear & undo
        //public void cmd_clear()
        //{
        //    if (ObjectsList.Any())
        //    {
        //        ObjectsList = new List<BPaintObject>();
        //    }

        //    if (VerticesList.Any())
        //    {
        //        VerticesList = new List<BPaintVertex>();
        //    }

        //    cmd_RefreshSVG();
        //}


        //public void cmd_undo()
        //{
        //    if (ObjectsList.Any())
        //    {
        //        ObjectsList.Remove(ObjectsList.Last());
        //        cmd_RefreshSVG();
        //    }
        //}
        #endregion


        #region // scaling
        //protected void cmd_scale_x(UIChangeEventArgs e)
        //{
        //    if (e?.Value != null)
        //    {
        //        Curr_Scale_X = double.Parse(e.Value as string);




        //        if (ObjectsList.Any())
        //        {

        //            foreach (var item in ObjectsList.Where(x => x.Selected))
        //            {
        //                item.Scale = new PointD(Curr_Scale_X, Curr_Scale_Y);
        //            }



        //            cmd_RefreshSVG();


        //        }
        //    }
        //}


        //protected void cmd_scale_y(UIChangeEventArgs e)
        //{
        //    if (e?.Value != null)
        //    {
        //        Curr_Scale_Y = double.Parse(e.Value as string);

        //        if (ObjectsList.Any())
        //        {
        //            foreach (var item in ObjectsList.Where(x => x.Selected))
        //            {
        //                item.Scale = new PointD(Curr_Scale_X, Curr_Scale_Y);
        //            }
        //            cmd_RefreshSVG();
        //        }
        //    }
        //}
        #endregion // scaling


        //protected void cmd_rotate(UIChangeEventArgs e)
        //{
        //    bool anythingChanged = false;
        //    if (e?.Value != null)
        //    {
        //        Curr_Rotate_rad = double.Parse(e.Value as string);
        //
        //        if (ObjectsList.Any())
        //        {
        //            foreach (BPaintObject paintObject in ObjectsList.Where(x => x.ObjectType == BPaintOpbjectType.Line))
        //            {
        //                BPaintLine currLine = paintObject as BPaintLine;
        //                SVGtransform currLineTransform = new SVGtransform(
        //                    rotateCx: currLine.VerticesList.Average(vertex => vertex.Position.X),
        //                    rotateCy: currLine.VerticesList.Average(vertex => vertex.Position.Y),
        //                    rotateAngleDegrees: Curr_Rotate_deg);
        //                currLine.transform = currLineTransform;
        //                foreach (BPaintVertex vertex in currLine.VerticesList)
        //                {
        //                    vertex.transform = currLineTransform;
        //                }
        //            }
        //            anythingChanged = anythingChanged | true;
        //        }

        //        //if (VerticesList.Any())
        //        //{
        //        //    foreach (BPaintVertex vertex in VerticesList)
        //        //    {
        //        //        vertex.transform = new SVGtransform(rotateAngleDegrees: Curr_Rotate_deg, rotateCx: vertex.Position.X, rotateCy:vertex.Position.Y);
        //        //    }
        //        //    anythingChanged = anythingChanged | true;
        //        //}
        //    }
        //
        //    if (anythingChanged)
        //    {
        //        cmd_RefreshSVG();
        //    }
        //}




        public void cmd_Clear_Selection()
        {
            foreach (var item in ObjectsList.Where(x => x.Selected))
            {
                item.Selected = false;
            }

            foreach (BPaintVertex vertex in VerticesList.Where(v => v.Selected))
            {
                vertex.Selected = false;
            }
        }

        public void cmd_Clear_Editing()
        {
            foreach (var item in ObjectsList.Where(x => x.EditMode))
            {
                item.EditMode = false;
            }
        }



        public string strCurrentlyEditingObject()
        {
            if (!ObjectsList.Any())
            {
                return "none";
            }

            if ((ObjectsList.Any()) & (!ObjectsList.Any(o => o.EditMode)))
            {
                return String.Format("none of {0}", ObjectsList.Count);
            }

            if ((ObjectsList.Any()) & (ObjectsList.Any(o => o.EditMode)))
            {
                return ObjectsList.Single(o => o.EditMode).ToString();
            }

            return "none";
        }



        public void startLine(UIMouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

            BPaintLine new_BPaintLine = new BPaintLine();


            if (ObjectsList.Any())
            {
                new_BPaintLine.ObjectID = ObjectsList.Max(x => x.ObjectID) + 1;
                cmd_Clear_Selection();
                cmd_Clear_Editing();
            }
            else
            {
                new_BPaintLine.ObjectID = 1;
            }

            BPaintVertex startVertex = new BPaintVertex(CurrPosition, Color1);
            VerticesList.Add(startVertex);
            
            new_BPaintLine.Selected = true;
            new_BPaintLine.EditMode = true;
            new_BPaintLine.Color = Color1;
            new_BPaintLine.LineWidth = LineWidth1;
            new_BPaintLine.Position = startVertex;
            BPaintVertex endVertex = new BPaintVertex(CurrPosition, Color1);
            new_BPaintLine.end = endVertex;
            VerticesList.Add(endVertex);
            ObjectsList.Add(new_BPaintLine);

            CurrPaintMode = BPaintMode.movingAnElement;
            bpVertexUnderMousePointer = endVertex;
        }




        public void startCircle(UIMouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

            BPaintCircle new_BPaintCircle = new BPaintCircle();


            if (ObjectsList.Any())
            {
                new_BPaintCircle.ObjectID = ObjectsList.Max(x => x.ObjectID) + 1;
                cmd_Clear_Selection();
                cmd_Clear_Editing();
            }
            else
            {
                new_BPaintCircle.ObjectID = 1;
            }

            BPaintVertex startVertex = new BPaintVertex(CurrPosition, Color1);
            VerticesList.Add(startVertex);

            new_BPaintCircle.Selected = true;
            new_BPaintCircle.EditMode = true;
            new_BPaintCircle.Color = Color1;
            new_BPaintCircle.LineWidth = LineWidth1;
            new_BPaintCircle.Position = startVertex;
            BPaintVertex endVertex = new BPaintVertex(CurrPosition, Color1);
            new_BPaintCircle.end = endVertex;
            VerticesList.Add(endVertex);
            CurrPaintMode = BPaintMode.movingAnElement;
            bpVertexUnderMousePointer = endVertex;
            ObjectsList.Add(new_BPaintCircle);
        }




        public void startEllipse(UIMouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

            BPaintEllipse new_BPaintEllipse = new BPaintEllipse();


            if (ObjectsList.Any())
            {
                new_BPaintEllipse.ObjectID = ObjectsList.Max(x => x.ObjectID) + 1;
                cmd_Clear_Selection();
                cmd_Clear_Editing();
            }
            else
            {
                new_BPaintEllipse.ObjectID = 1;
            }

            BPaintVertex startVertex = new BPaintVertex(CurrPosition, Color1);
            VerticesList.Add(startVertex);

            new_BPaintEllipse.Selected = true;
            new_BPaintEllipse.EditMode = true;
            new_BPaintEllipse.Color = Color1;
            new_BPaintEllipse.LineWidth = LineWidth1;
            new_BPaintEllipse.Position = startVertex;
            //new_BPaintEllipse.VerticesList.Add(startVertex);
            CurrPaintMode = BPaintMode.drawing;
            ObjectsList.Add(new_BPaintEllipse);
        }




        public void continueEllipse(UIMouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

            BPaintEllipse currBPaintEllipse =
                ObjectsList.Single(o => ((o.EditMode) & (o.ObjectType == BPaintOpbjectType.Ellipse))) as BPaintEllipse;

            BPaintVertex currVertex = new BPaintVertex(CurrPosition, Color1);
            VerticesList.Add(currVertex);

            switch (currBPaintEllipse.VerticesList.Count)
            {
                case 1:
                    currBPaintEllipse.pt2 = currVertex;
                    break;
                case 2:
                    currBPaintEllipse.pt3 = currVertex;
                    currBPaintEllipse.EditMode = false;
                    CurrPaintMode = BPaintMode.idle;
                    break;
                default:
                    break;
            }
        }




        public BPaintVertex makeVertex(UIMouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

            BPaintVertex currBPaintVertex = new BPaintVertex();


            if (ObjectsList.Any())
            {
                cmd_Clear_Selection();
            }
            
            currBPaintVertex.Selected = true;
            currBPaintVertex.Color = Color1;
            currBPaintVertex.PtD = new PointD(CurrPosition);
            VerticesList.Add(currBPaintVertex);
            return currBPaintVertex;
        }




        public void onMouseMove(UIMouseEventArgs e)
        {
            PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);
            currMouseLocation = CurrPosition;
            strCurrMouseLocation = "x: " + currMouseLocation.X.ToString("F2") + "; y: " +
                                   currMouseLocation.Y.ToString("F2");

            if (CurrOperationalMode == OperationalMode.select)
            {
                switch (CurrSelectionMode)
                {
                    case SelectionMode.idle:
                        if (SelectionVerticesList.Any(x => (CurrPosition.DistanceTo(x.PtD) <= 10)))
                        {
                            BPaintVertex currVertex =
                                SelectionVerticesList.Single(x => (CurrPosition.DistanceTo(x.PtD) <= 10));

                            bpSelectionVertexUnderMousePointer = currVertex;
                            bpSelectionVertexUnderMousePointer.Selected = true;
                            CurrSelectionMode = SelectionMode.hoveredAVertex;
                        }
                        else
                        {
                            bpSelectionVertexUnderMousePointer = null;
                            CurrSelectionMode = SelectionMode.idle;
                        }

                        break;
                    case SelectionMode.movingAnElement:
                        BPaintVertex vertex2Move = bpSelectionVertexUnderMousePointer as BPaintVertex;
                        vertex2Move.PtD = CurrPosition;
                        break;
                    case SelectionMode.hoveredAVertex:
                        if (SelectionVerticesList.Any(x => (CurrPosition.DistanceTo(x.PtD) <= 10)))
                        {
                            BPaintVertex currVertex =
                                SelectionVerticesList.Single(x => (CurrPosition.DistanceTo(x.PtD) <= 10));

                            bpSelectionVertexUnderMousePointer = currVertex;
                            CurrSelectionMode = SelectionMode.hoveredAVertex;
                        }
                        else
                        {
                            bpSelectionVertexUnderMousePointer = null;

                            foreach (BPaintVertex vertex in SelectionVerticesList)
                            {
                                vertex.Selected = false;
                            }

                            CurrSelectionMode = SelectionMode.idle;
                        }

                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (CurrPaintMode)
                {
                    case BPaintMode.idle:
                        if (VerticesList.Any(x => (CurrPosition.DistanceTo(x.PtD) <= 10)))
                        {
                            BPaintVertex currVertex = VerticesList.Single(x => (CurrPosition.DistanceTo(x.PtD) <= 10));

                            bpVertexUnderMousePointer = currVertex;
                            bpVertexUnderMousePointer.Selected = true;
                            CurrPaintMode = BPaintMode.hoveredAVertex;
                        }
                        else
                        {
                            bpVertexUnderMousePointer = null;
                            CurrPaintMode = BPaintMode.idle;
                        }

                        break;
                    case BPaintMode.drawing:
                        break;
                    case BPaintMode.movingAnElement:
                        BPaintVertex vertex2Move = bpVertexUnderMousePointer as BPaintVertex;
                        vertex2Move.PtD = CurrPosition;
                        break;
                    case BPaintMode.hoveredAVertex:
                        if (VerticesList.Any(x => (CurrPosition.DistanceTo(x.PtD) <= 10)))
                        {
                            BPaintVertex currVertex = VerticesList.Single(x => (CurrPosition.DistanceTo(x.PtD) <= 10));

                            bpVertexUnderMousePointer = currVertex;
                            CurrPaintMode = BPaintMode.hoveredAVertex;
                        }
                        else
                        {
                            bpVertexUnderMousePointer = null;
                            foreach (BPaintObject bpObject in ObjectsList)
                            {
                                //bpObject.Selected = false;
                                bpObject.EditMode = false;
                            }

                            foreach (BPaintVertex vertex in VerticesList)
                            {
                                vertex.Selected = false;
                            }

                            CurrPaintMode = BPaintMode.idle;
                        }

                        break;
                    default:
                        break;
                }
            }

            StateHasChanged();
        }




        public void onMouseDown(UIMouseEventArgs e)
        {
            if (CurrOperationalMode == OperationalMode.select)
            {
                if (CurrSelectionMode == SelectionMode.idle)
                {
                    if (bpSelectionRectangle != null)
                    {
                        foreach (BPaintVertex vertex in bpSelectionRectangle.VerticesList)
                        {
                            SelectionVerticesList.RemoveAll(x => true);
                        }
                        bpSelectionRectangle = null;
                    }
                    
                    PointD CurrPosition = new PointD(e.ClientX - LocalData.SVGPosition.X, e.ClientY - LocalData.SVGPosition.Y);

                    bpSelectionRectangle = new BPaintRectangle()
                    {
                        ObjectID = 999
                    };

                    BPaintVertex startVertex = new BPaintVertex(CurrPosition, "magenta");
                    SelectionVerticesList.Add(startVertex);

                    bpSelectionRectangle.Selected = false;
                    bpSelectionRectangle.EditMode = true;
                    bpSelectionRectangle.Color = "red";
                    bpSelectionRectangle.LineWidth = 2;
                    bpSelectionRectangle.Position = startVertex;
                    BPaintVertex endVertex = new BPaintVertex(CurrPosition, "magenta");
                    bpSelectionRectangle.end = endVertex;
                    SelectionVerticesList.Add(endVertex);
                    CurrSelectionMode = SelectionMode.movingAnElement;
                    bpSelectionVertexUnderMousePointer = endVertex;

                }
                else if (CurrSelectionMode == SelectionMode.hoveredAVertex)
                {
                    if (bpSelectionVertexUnderMousePointer != null)
                    {
                        BPaintVertex currVertex = bpSelectionVertexUnderMousePointer as BPaintVertex;
                        currVertex.Selected = true;
                        CurrSelectionMode = SelectionMode.movingAnElement;
                    }
                }
                StateHasChanged();
            }
            else
            {
                if (CurrPaintMode == BPaintMode.idle)
                {
                    if (!ObjectsList.Any(o => o.EditMode))
                    {
                        switch (FigureCode)
                        {
                            case 2:
                                startLine(e);
                                break;
                            case 3:
                                BPaintVertex newVertex = makeVertex(e);
                                CurrPaintMode = BPaintMode.idle;
                                break;
                            case 4:
                                startCircle(e);
                                break;
                            case 5:
                                //cmd_prepareEllipse(e);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        BPaintObject currBPaintObject = ObjectsList.Single(o => o.EditMode);
                        BPaintVertex newVertex = makeVertex(e);

                        switch (currBPaintObject.ObjectType)
                        {
                            case BPaintOpbjectType.Ellipse:
                                BPaintEllipse currEllipse = currBPaintObject as BPaintEllipse;
                                currEllipse.pt3 = newVertex;
                                currEllipse.EditMode = false;
                                break;
                            default:
                                break;
                        }

                        CurrPaintMode = BPaintMode.idle;
                    }
                }
                else if (CurrPaintMode == BPaintMode.hoveredAVertex)
                {
                    if (bpVertexUnderMousePointer != null)
                    {
                        BPaintVertex currVertex = bpVertexUnderMousePointer as BPaintVertex;
                        currVertex.Selected = true;
                        CurrPaintMode = BPaintMode.movingAnElement;
                    }

                }

                StateHasChanged();
            }
        }





        public void onMouseUp(UIMouseEventArgs e)
        {
            if (CurrOperationalMode == OperationalMode.@select)
            {
                switch (CurrSelectionMode)
                {
                    case SelectionMode.movingAnElement:
                        CurrSelectionMode = SelectionMode.idle;

                        if (!bpSelectionRectangle.IsValid())
                        {
                            bpSelectionRectangle = null;
                            SelectionVerticesList.RemoveAll(x => true);
                        }

                        cmd_RefreshSVG();
                        break;
                    default:
                        break;
                }
                StateHasChanged();
            }
            else
            {
                switch (CurrPaintMode)
                {
                    case BPaintMode.idle:
                        // this can be if the object to create need more than 2 vertices
                        switch (FigureCode)
                        {
                            case 5:
                                startEllipse(e);
                                break;
                            default:
                                break;
                        }

                        break;
                    case BPaintMode.movingAnElement:
                        if (ObjectsList.Any(o => o.EditMode))
                        {
                            BPaintObject currObjectEditing = ObjectsList.Single(o => o.EditMode);
                            if (currObjectEditing.MandatoryVerticesCount > 2)
                            {
                                CurrPaintMode = BPaintMode.idle;
                                cmd_RefreshSVG();
                            }
                            else
                            {
                                CurrPaintMode = BPaintMode.idle;
                                cmd_RefreshSVG();
                            }
                        }
                        else
                        {
                            CurrPaintMode = BPaintMode.hoveredAVertex;
                            cmd_Clear_Editing();
                            cmd_RefreshSVG();
                        }

                        break;
                    case BPaintMode.drawing:
                        switch (FigureCode)
                        {
                            case 5:
                                continueEllipse(e);
                                break;
                            default:
                                break;
                        }

                        break;
                    default:
                        break;
                }

                StateHasChanged();
            }
        }




        protected void btnSelectAll_onClick()
        {
            ChangeObjectsSelection(true);
        }


        protected void ChangeObjectsSelection(bool b)
        {
            if (ObjectsList.Any())
            {
                foreach (var item in ObjectsList)
                {
                    item.Selected = b;
                }
            }
        }


        protected void cmd_delete_object()
        {
            if (ObjectsList.Any())
            {
                if (ObjectsList.Any(x => x.Selected))
                {
                    BPaintObject bpObjectToDelete = ObjectsList.Where(x => x.Selected).First();
                    foreach (BPaintVertex vertex in bpObjectToDelete.VerticesList)
                    {
                        VerticesList.Remove(vertex);
                    }

                    ObjectsList.Remove(bpObjectToDelete);
                    BPaintObject b = ObjectsList.Single(i => i.ObjectID == ObjectsList.Min(x => x.ObjectID));
                    b.Selected = true;

                    cmd_RefreshSVG();
                }

            }
        }




        protected void btnDeleteSelectedObjects_onClick()
        {
            if (ObjectsList.Any())
            {
                if (ObjectsList.Any(x => x.Selected))
                {
                    List<BPaintObject> objectsToDelete = ObjectsList.Where(x => x.Selected).ToList();
                    foreach (BPaintObject bpObjectToDelete in objectsToDelete)
                    {
                        foreach (BPaintVertex vertex in bpObjectToDelete.VerticesList)
                        {
                            VerticesList.Remove(vertex);
                        }
                        ObjectsList.Remove(bpObjectToDelete);
                    }
                    
                    cmd_RefreshSVG();
                }
            }
        }






        public void cmd_RefreshSVG()
        {
            Curr_CompMySVG.Refresh();
            StateHasChanged();
        }



        protected void cmd_ColorChange(UIChangeEventArgs e)
        {
            Console.WriteLine("Hit cmd_ColorChange(), e = " + e.ToString());
            if (e?.Value != null)
            {
                Color1 = e.Value as string;

                if (Curr_CompUsedColors.UsedColors_List.Any(x => x == Color1))
                {
                    Curr_CompUsedColors.UsedColors_List.Remove(Curr_CompUsedColors.UsedColors_List.Single(x => x == Color1));
                }

                if (Curr_CompUsedColors.UsedColors_List.Count > 9)
                {
                    Curr_CompUsedColors.UsedColors_List.RemoveAt(0);
                }

                Curr_CompUsedColors.UsedColors_List.Add(Color1);

                Cmd_RefreshUsedColorsSVG();
            }
        }



        public void Cmd_RefreshUsedColorsSVG()
        {
            Curr_CompUsedColors.Refresh();
            StateHasChanged();
        }



        public void Dispose()
        {

        }






        protected void cmd_Move(BPaintMoveDirection Par_Direction)
        {
            if (ObjectsList.Any())
            {
                foreach (var item in ObjectsList.Where(x => x.Selected))
                {
                    switch (Par_Direction)
                    {
                        case BPaintMoveDirection.left:
                            item.Position.PtD += new SizeD(-StepSize, 0);
                            break;
                        case BPaintMoveDirection.right:
                            item.Position.PtD += new SizeD(StepSize, 0);
                            break;
                        case BPaintMoveDirection.up:
                            item.Position.PtD += new SizeD(0, -StepSize);
                            break;
                        case BPaintMoveDirection.down:
                            item.Position.PtD += new SizeD(0, StepSize);
                            break;
                        default:
                            break;
                    }
                }
                cmd_RefreshSVG();
            }
        }


        public void GetBoundingClientRect(string ElementID)
        {
            BPaintJsInterop.GetElementBoundingClientRect(ElementID, DotNetObjectRef.Create(this));
        }



        [JSInvokable]
        public void invokeFromjs(string id, double rect_left, double rect_top, double rect_width, double rect_height, double window_scrollX, double window_scrollY)
        {
            Console.WriteLine("hit invokeFromjs()");
            Console.WriteLine("id = " + id + "; left = " + rect_left + "; top = " + rect_top + "; width = " + rect_width + "; height = " + rect_height + "; w_scrollX = " + window_scrollX + "; w_scrollY = " + window_scrollY);

            //LocalData.SVGPosition = new PointD(rect_left + window_scrollX, rect_top + window_scrollY);
            LocalData.SVGPosition = new PointD(rect_left, rect_top);
        }
    }
}
