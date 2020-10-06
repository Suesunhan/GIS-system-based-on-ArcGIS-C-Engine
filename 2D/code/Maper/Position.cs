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
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display; 

namespace Maper
{
    public partial class Position : Form
    {

        public Position(ESRI.ArcGIS.Controls.AxMapControl mapControl)
        {
            InitializeComponent();
            this.m_mapControl = mapControl;
        }

        //指针
        private ESRI.ArcGIS.Controls.AxMapControl m_mapControl;
        public int mLayerIndex;
        public int mQueryModel;

        //初始化图层和方法
        private void Position_Load(object sender, EventArgs e)
        {
            //MapControl没有图层返回 
            //获取图层选项
            if (m_mapControl.LayerCount <= 0)
                return;
            for (int i = 0; i <m_mapControl.LayerCount; ++i)
            {

                comboBoxLayer.Items.Add(m_mapControl.get_Layer(i).Name);

            }

            //设置comboBoxMethod的选择项，并设置默认值为第一项
            comboBoxMethod.Items.Add("矩形查询");
            comboBoxMethod.Items.Add("线查询");
            comboBoxMethod.Items.Add("点查询");
            comboBoxMethod.Items.Add("圆查询");

            comboBoxLayer.SelectedIndex = 0;
            comboBoxMethod.SelectedIndex = 0;
            //初始化ComboBox默认值  
        }

        //选择图层和方法
        private void buttonSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            //判断图层数量  
            if (this.comboBoxLayer.Items.Count <= 0)
            {
                MessageBox.Show("当前MapControl没有添加图层！", "提示");
                return;
            }
            //获取选中的查询方式和图层索引  
            this.mLayerIndex = comboBoxLayer.SelectedIndex;
            this.mQueryModel = comboBoxMethod.SelectedIndex;
        }

        //清除所选内容
        private void buttonClear_Click(object sender, EventArgs e)
        {
            IMap pMap = m_mapControl.Map;
            IActiveView pActiveView = (IActiveView)pMap;
            IGraphicsContainer pGraphicsContainer = (IGraphicsContainer)pActiveView;
            pGraphicsContainer.DeleteAllElements();
            pMap.ClearSelection();
            pActiveView.Refresh();
        }

        //退出
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
