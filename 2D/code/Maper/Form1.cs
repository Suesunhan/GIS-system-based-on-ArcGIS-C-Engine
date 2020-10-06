using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using System.IO;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Geodatabase;
using _sdnMap;
using Maper;

namespace Maper
{
    public partial class Form1 : Form
    {
        //定义指针
        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl = null;
        private ESRI.ArcGIS.Controls.IPageLayoutControl2 m_pageLayoutControl = null;

        private ControlsSynchronizer m_controlsSynchronizer = null;

        private IMapDocument pMapDocument;
        private IMap m_map;

        private string sMapUnits;

        //TOCControl控件变量
        private ITOCControl2 m_tocControl = null;
        //TOCControl中Map菜单
        private IToolbarMenu m_menuMap = null;
        //TOCControl中图层菜单
        private IToolbarMenu m_menuLayer = null;


        public Form1()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            InitializeComponent();

            //在Form1_Load函数进行初始化，即菜单的创建：

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_menuMap = new ToolbarMenuClass();
            m_menuLayer = new ToolbarMenuClass();

            //初始化指针
            m_mapControl = (IMapControl3)this.axMapControl1.Object;
            m_pageLayoutControl = (IPageLayoutControl2)this.axPageLayoutControl1.Object;

            //初始化controls synchronization calss
            m_controlsSynchronizer = new
            ControlsSynchronizer(m_mapControl, m_pageLayoutControl);
            //把MapControl和PageLayoutControl绑定起来(两个都指向同一个Map),然后设置MapControl为活动的Control
            m_controlsSynchronizer.BindControls(true);
            //为了在切换MapControl和PageLayoutControl视图同步，要添加Framework Control
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl1.Object);
            m_controlsSynchronizer.AddFrameworkControl(this.axTOCControl1.Object);
            // 添加打开命令按钮到工具条
            OpenNewMapDocument openMapDoc = new OpenNewMapDocument(m_controlsSynchronizer);
            axToolbarControl1.AddItem(openMapDoc, -1, 0, false, -1, esriCommandStyles.esriCommandStyleIconOnly);

            sMapUnits = "Unknown";

            //添加自定义菜单项到TOCCOntrol的Map菜单中
            //打开文档菜单
            m_menuMap.AddItem(new OpenNewMapDocument(m_controlsSynchronizer), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            //添加数据菜单
            m_menuMap.AddItem(new ControlsAddDataCommandClass(), -1, 1, false, esriCommandStyles.esriCommandStyleIconAndText);
            //打开全部图层菜单
            m_menuMap.AddItem(new LayerVisibility(), 1, 2, false, esriCommandStyles.esriCommandStyleTextOnly);
            //关闭全部图层菜单
            m_menuMap.AddItem(new LayerVisibility(), 2, 3, false, esriCommandStyles.esriCommandStyleTextOnly);
            //以二级菜单的形式添加内置的“选择”菜单
            m_menuMap.AddSubMenu("esriControls.ControlsFeatureSelectionMenu", 4, true);
            //以二级菜单的形式添加内置的“地图浏览”菜单
            m_menuMap.AddSubMenu("esriControls.ControlsMapViewMenu",5, true);
            //添加自定义菜单项到TOCCOntrol的图层菜单中
            m_menuLayer = new ToolbarMenuClass();
            //添加“移除图层”菜单项
            m_menuLayer.AddItem(new RemoveLayer(), -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            //添加“放大到整个图层”菜单项
            m_menuLayer.AddItem(new ZoomToLayer(), -1, 1, true, esriCommandStyles.esriCommandStyleTextOnly);

            m_tocControl = (ITOCControl2)this.axTOCControl1.Object;

            //设置菜单的Hook
            m_menuLayer.SetHook(m_mapControl);
            m_menuMap.SetHook(m_mapControl);
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            //基本功能
            if (e.button == 1)
                this.axMapControl1.Extent = this.axMapControl1.TrackRectangle();
            else if (e.button == 2)
                this.axMapControl1.Extent = this.axMapControl1.FullExtent;
            else if (e.button == 4)
                this.axMapControl1.Pan();

            switch(mTool)
            {
                case "Position":
            //获取当前视图
            ESRI.ArcGIS.Carto.IActiveView pActiveView = this.axMapControl1.ActiveView;
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            //获取鼠标点
            ESRI.ArcGIS.Geometry.IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            IMap pMap = axMapControl1.Map;
            IGeometry pGeometry = null;
            //选择方法
            switch (this.mQueryModel)
            {
                case 0:         //矩形查询
                    pGeometry = this.axMapControl1.TrackRectangle();
                    break;
                case 1:         //线查询
                    pGeometry = this.axMapControl1.TrackLine();
                    break;
                case 2:         //点查询
                    ESRI.ArcGIS.Geometry.ITopologicalOperator pTopo;
                    ESRI.ArcGIS.Geometry.IGeometry pBuffer;
                    pGeometry = pPoint;
                    pTopo = pGeometry as ESRI.ArcGIS.Geometry.ITopologicalOperator;
                    //根据点位创建缓冲区，缓冲半径设为0.005，可自行修改
                    pBuffer = pTopo.Buffer(0.005);
                    pGeometry = pBuffer.Envelope;
                    break;
                case 3:         //圆查询
                    pGeometry = this.axMapControl1.TrackCircle();
                   break;

            }
            ISelectionEnvironment pSelectionEnv = new SelectionEnvironment(); //新建选择环境
            IRgbColor pColor = new RgbColor();
            pColor.Red = 255;
            pSelectionEnv.DefaultColor = pColor;         //设置高亮显示的颜色
            pMap.SelectByShape(pGeometry, pSelectionEnv, false); //选择图形
            axMapControl1.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            break;
                default:
            break;
        }

        }

        //新建地图
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //询问是否保存当前地图
            DialogResult res = MessageBox.Show("是否保存当前地图?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                //如果要保存，调用另存为对话框
                ICommand command = new ControlsSaveAsDocCommandClass();
                //保存地图
                if (m_mapControl != null)
                    command.OnCreate(m_controlsSynchronizer.MapControl.Object);
                //保存图层
                else
                    command.OnCreate(m_controlsSynchronizer.PageLayoutControl.Object);
                command.OnClick();
            }
            //创建新的地图实例
            IMap map = new MapClass();
            map.Name = "Map";
            m_controlsSynchronizer.MapControl.DocumentFilename = string.Empty;
            //更新新建地图实例的共享地图文档
            m_controlsSynchronizer.ReplaceMap(map);
        }

        //打开地图
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.axMapControl1.LayerCount > 0)
            {
                DialogResult result = MessageBox.Show("是否保存当前地图？", "警告",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel) return;
                if (result == DialogResult.Yes) this.saveToolStripMenuItem_Click(null, null);
            }
            OpenNewMapDocument openMapDoc = new OpenNewMapDocument(m_controlsSynchronizer);
            openMapDoc.OnCreate(m_controlsSynchronizer.MapControl.Object);
            openMapDoc.OnClick();
        }

        //保存
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //询问是否保存当前地图
            DialogResult res = MessageBox.Show("是否保存当前地图?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                //如果要保存，调用另存为对话框
                ICommand command = new ControlsSaveAsDocCommandClass();
                if (m_mapControl != null)
                    command.OnCreate(m_controlsSynchronizer.MapControl.Object);
                else
                    command.OnCreate(m_controlsSynchronizer.PageLayoutControl.Object);
                command.OnClick();
            }
        }

        //另存为
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //如果当前视图为MapControl时，提示用户另存为操作将丢失PageLayoutControl中的设置
            if (m_controlsSynchronizer.ActiveControl is IMapControl3)
            {
                if (MessageBox.Show("另存为地图文档将丢失制版视图的设置\r\n您要继续吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
            //调用另存为命令
            ICommand command = new ControlsSaveAsDocCommandClass();
            command.OnCreate(m_controlsSynchronizer.ActiveControl);
            command.OnClick();
        }

        //退出
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //添加数据
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int currentLayerCount = this.axMapControl1.LayerCount;
            ICommand pCommand = new ControlsAddDataCommandClass();
            pCommand.OnCreate(this.axMapControl1.Object);
            pCommand.OnClick();
            IMap pMap = this.axMapControl1.Map;
            this.m_controlsSynchronizer.ReplaceMap(pMap);
        }

        //切换地图和制图
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl2.SelectedIndex == 0)
            {
                //激活MapControl
                m_controlsSynchronizer.ActivateMap();
            }
            else
            {
                //激活PageLayoutControl
                m_controlsSynchronizer.ActivatePageLayout();
            }
        }

        //Message状态栏
        private void axTOCControl1_OnMouseMove(object sender, ITOCControlEvents_OnMouseMoveEvent e)
        {
            // 取得鼠标所在工具的索引号
            int index = axToolbarControl1.HitTest(e.x, e.y, false);
            if (index != -1)
            {
                // 取得鼠标所在工具的 ToolbarItem
                IToolbarItem toolbarItem = axToolbarControl1.GetItem(index);
                // 设置状态栏信息
                MessageLabel.Text = toolbarItem.Command.Message;
            }
            else
            {
                MessageLabel.Text = " 就绪 ";
            }
        }

        //比例尺和坐标系
        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 显示当前比例尺
            ScaleLabel.Text = " 比例尺 1:" + ((long)this.axMapControl1.MapScale).ToString();
            // 显示当前坐标
            CoordinateLabel.Text = " 当前坐标 X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + this.axMapControl1.MapUnits;
        }

        //单位
        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            esriUnits mapUnits = axMapControl1.MapUnits;
            switch (mapUnits)
            {
                case esriUnits.esriCentimeters:
                    sMapUnits = "Centimeters";
                    break;
                case esriUnits.esriDecimalDegrees:
                    sMapUnits = "Decimal Degrees";
                    break;
                case esriUnits.esriDecimeters:
                    sMapUnits = "Decimeters";
                    break;
                case esriUnits.esriFeet:
                    sMapUnits = "Feet";
                    break;
                case esriUnits.esriInches:
                    sMapUnits = "Inches";
                    break;
                case esriUnits.esriKilometers:
                    sMapUnits = "Kilometers";
                    break;
                case esriUnits.esriMeters:
                    sMapUnits = "Meters";
                    break;
                case esriUnits.esriMiles:
                    sMapUnits = "Miles";
                    break;
                case esriUnits.esriMillimeters:
                    sMapUnits = "Millimeters";
                    break;
                case esriUnits.esriNauticalMiles:
                    sMapUnits = "NauticalMiles";
                    break;
                case esriUnits.esriPoints:
                    sMapUnits = "Points";
                    break;
                case esriUnits.esriUnknownUnits:
                    sMapUnits = "Unknown";
                    break;
                case esriUnits.esriYards:
                    sMapUnits = "Yards";
                    break;
            }
        }

        //按属性查询
        private void Attribute_Click(object sender, EventArgs e)
        {
            //判断是否已经打开了地图
            IMap m_Map = axMapControl1.Map;
            if (m_Map.LayerCount < 1)
            {
                MessageBox.Show("请先加载图层！");
                return;
            }
            //创建一个SelectLayer类来控制选择图层查询
            Attribute m_Attribute = new Attribute((IMapControl2)axMapControl1.Object);
            //打开查询对话框
            m_Attribute.ShowDialog();
        }

        #region 空间查询变量
        //查询方式  
        public int mQueryModel;
        //图层索引  
        public int mLayerIndex;
        public string mTool = null;
        #endregion
        //按空间位置查询
        private void Position_Click(object sender, EventArgs e)
        {
            //初始化空间查询窗体  
            Position pPosition = new Position(axMapControl1);
            if (pPosition.ShowDialog() == DialogResult.OK)
            {
                this.mTool = "Position";
                //获取查询方式和图层信息  
                this.mQueryModel = pPosition.mQueryModel;
                this.mLayerIndex = pPosition.mLayerIndex;
                //设置鼠标形状  
                this.axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerCrosshair;

            }  
        }

        //按空间关系查询
        private void Relationship_Click(object sender, EventArgs e)
        {
            Relationship m_Relationship = new Relationship();
            m_Relationship.CurrentMap = axMapControl1.Map;
            m_Relationship.Show();
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            //如果不是右键按下直接返回       
            if (e.button != 2) return;
            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap map = null;
            ILayer layer = null;
            object other = null;
            object index = null;
            //判断所选菜单的类型  
            m_tocControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
            //确定选定的菜单类型，Map 或是图层菜单      
            if (item == esriTOCControlItem.esriTOCControlItemMap)
                m_tocControl.SelectItem(map, null);
            else if (item == esriTOCControlItem.esriTOCControlItemLayer)
                m_tocControl.SelectItem(layer, null);
            else
                return;
            //设置 CustomProperty 为 layer (用于自定义的 Layer 命令)             
            m_mapControl.CustomProperty = layer;
            //弹出右键菜单      
            if (item == esriTOCControlItem.esriTOCControlItemMap)
                m_menuMap.PopupMenu(e.x, e.y, m_tocControl.hWnd);
            if (item == esriTOCControlItem.esriTOCControlItemLayer)
            {
                m_menuLayer.PopupMenu(e.x, e.y, m_tocControl.hWnd);
            }
        }

        private void addPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand pCommand;
            pCommand = new AddNetStopsTool();
            pCommand.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pCommand as ITool;
            pCommand = null;
        }

        private void addNetBarriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand pCommand;
            pCommand = new AddNetBarriesTool();
            pCommand.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pCommand as ITool;
            pCommand = null;
        }

        private void netWorkAnalysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand pCommand;
            pCommand = new ShortPathSolveCommand();
            pCommand.OnCreate(axMapControl1.Object);
            pCommand.OnClick();
            pCommand = null;
        }
        string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        public IFeatureWorkspace pFWorkspace;
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMapControl1.CurrentTool = null;
            try
            {
                string name = NetWorkAnalysClass.getPath(path) + "\\data\\Geodatabase.gdb";
                //打开工作空间
                pFWorkspace = NetWorkAnalysClass.OpenWorkspace(name) as IFeatureWorkspace;
                IGraphicsContainer pGrap = this.axMapControl1.ActiveView as IGraphicsContainer;
                pGrap.DeleteAllElements();//删除所添加的图片要素
                IFeatureClass inputFClass = pFWorkspace.OpenFeatureClass("Stops");
                //删除站点要素
                if (inputFClass.FeatureCount(null) > 0)
                {
                    ITable pTable = inputFClass as ITable;
                    pTable.DeleteSearchedRows(null);
                }
                IFeatureClass barriesFClass = pFWorkspace.OpenFeatureClass("Barries");//删除障碍点要素
                if (barriesFClass.FeatureCount(null) > 0)
                {
                    ITable pTable = barriesFClass as ITable;
                    pTable.DeleteSearchedRows(null);
                }
                for (int i = 0; i < axMapControl1.LayerCount; i++)//删除分析结果
                {
                    ILayer pLayer = axMapControl1.get_Layer(i);
                    if (pLayer.Name == ShortPathSolveCommand.m_NAContext.Solver.DisplayName)
                    {
                        axMapControl1.DeleteLayer(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.axMapControl1.Refresh();
        }


    }
}

