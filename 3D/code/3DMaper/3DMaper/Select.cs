using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using System.IO;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using _3DMaper;

namespace _3DMaper
{
    public partial class Select : Form
    {
        //指针
        public ISceneControl pSceneControl;
        public IScene pScene;
        public ISceneGraph2 pSceneGraph;
        public int iLayerIndex;
        public int iFieldIndex;

        public Select(ISceneControl CurrentSceneControl)
        {
            InitializeComponent();
            this.pSceneControl = CurrentSceneControl;
            //设置comboBoxMethod的选择项，并设置默认值为第一项
            comBoxMethod.Items.Add("新建选择集");
            comBoxMethod.Items.Add("添加进已有的选择集");
            comBoxMethod.Items.Add("从当前选择集中清除");
            comBoxMethod.Items.Add("从当前选择集中再次筛选");
            comBoxMethod.SelectedIndex = 0;
            //最初时获取唯一值框不可使用
        }

        //获取字段值
        private void Select_Load(object sender, EventArgs e)
        {
            checkListField.Items.Clear();
            //获取要素层
            IFeatureLayer pFeatureLayer = (IFeatureLayer)pSceneControl.Scene.get_Layer(0);
            //索引
            IFeatureCursor pFeatureCursor = pFeatureLayer.Search(null, true);
            //获取要素
            IFeature pFeature = pFeatureCursor.NextFeature();
            //循环添加所属图层的字段名进入listBoxField中
            //对于esriFieldTypeGeometry类型的自动则不予以添加
            for (int i = 0; i < pFeature.Fields.FieldCount; i++)
            {
                if (pFeature.Fields.get_Field(i).Type != esriFieldType.esriFieldTypeGeometry)
                {
                    checkListField.Items.Add(pFeature.Fields.get_Field(i).Name);
                }
            }
            //设置当前选择字段为第一个
            checkListField.SelectedIndex = 0;
        }

        //获取唯一值
        private void buttonValue_Click(object sender, EventArgs e)
        {
            //获取要素图层与要素类，将其作为参数传入UniqueValue()函数
            IFeatureLayer pFeatureLayer = (IFeatureLayer)pSceneControl.Scene.get_Layer(0);
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            //将返回的所有值存入allValue数组中,并进行排序
            string[] allValue = UniqueValue(pFeatureClass, checkListField.Text);
            Array.Sort(allValue);
            //获取字段对象，用于在将其值添加进listboxValue中时判断字段类型
            IFeatureCursor pFeatureCursor = pFeatureLayer.Search(null, true);
            IFeature pFeature = pFeatureCursor.NextFeature();
            IField pField = new FieldClass();
            for (int j = 0; j < pFeature.Fields.FieldCount; j++)
            {
                if (checkListField.Text == pFeature.Fields.get_Field(j).Name)
                {
                    pField = pFeature.Fields.get_Field(j);
                }
            }
            //将之前listBox_Value中的值清空，然后添加此次选中字段的所有数据
            listBoxValue.Items.Clear();
            for (int i = 0; i < allValue.Length; i++)
            {
                if (pField.Type == esriFieldType.esriFieldTypeString)
                {
                    allValue[i] = "\'" + allValue[i] + "\'";
                    listBoxValue.Items.Add(allValue[i]);
                }
                else
                {
                    listBoxValue.Items.Add(allValue[i]);
                }
            }
            listBoxValue.Visible = true;
            buttonValue.Enabled = false;
        }

        //获取唯一值函数
        public string[] UniqueValue(IFeatureClass pFeatureClass, string strFld)
        {
            //得到IFeatureCursor游标
            IFeatureCursor pCursor = pFeatureClass.Search(null, false);
            //IDataStatistics对象实例生成
            IDataStatistics pData = new DataStatisticsClass();
            pData.Field = strFld;
            pData.Cursor = pCursor as ICursor;
            //枚举唯一值
            IEnumerator pEnumVar = pData.UniqueValues;
            //记录总数
            int RecordCount = pData.UniqueValueCount;
            //字符数组
            string[] strValue = new string[RecordCount];
            pEnumVar.Reset();

            int i = 0;
            while (pEnumVar.MoveNext())
            {
                strValue[i++] = pEnumVar.Current.ToString();
            }
            return strValue;
        }

        //实施
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (textBoxSelect.Text != "")
            {
                //获取图层
                IFeatureLayer lFeatureLayer = (IFeatureLayer)pSceneControl.Scene.get_Layer(0);
                IFeatureSelection lFeatureSelection = (IFeatureSelection)lFeatureLayer;
                //判断选择的SQL方法的类型
                esriSelectionResultEnum lesriSREnum = esriSelectionResultEnum.esriSelectionResultNew;
                //选择查询方法
                switch (comBoxMethod.SelectedIndex)
                {
                    case 0:
                        lesriSREnum = esriSelectionResultEnum.esriSelectionResultNew;
                        break;
                    case 1:
                        lesriSREnum = esriSelectionResultEnum.esriSelectionResultAdd;
                        break;
                    case 2:
                        lesriSREnum = esriSelectionResultEnum.esriSelectionResultSubtract;
                        break;
                    case 3:
                        lesriSREnum = esriSelectionResultEnum.esriSelectionResultAnd;
                        break;
                    default:
                        MessageBox.Show("请选择一种查询方法");
                        break;
                }
                //创建查询的条件
                IQueryFilter2 lQueryFilter = new QueryFilterClass();
                lQueryFilter.WhereClause = textBoxSelect.Text;
                //根据查询添加进行选择，并刷新屏幕
                lFeatureSelection.SelectFeatures(lQueryFilter, lesriSREnum, false);
                pSceneControl.Scene.SceneGraph.RefreshViewers();
            }
        }

        //texyBox内容输入
        private void checkListField_DoubleClick_1(object sender, EventArgs e)
        {
            textBoxSelect.Text += " " + checkListField.Text + " ";
        }

        private void listBoxValue_DoubleClick(object sender, EventArgs e)
        {
            textBoxSelect.Text += " " + listBoxValue.Text + " ";
        }

        private void buttonEqual_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " =";
        }

        //清除唯一值
        private void buttonClearValue_Click(object sender, EventArgs e)
        {
            listBoxValue.Items.Clear();
            buttonValue.Enabled = true;
        }

        //清除选择条件
        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxSelect.Clear();
        }

        private void buttonClearFeature_Click(object sender, EventArgs e)
        {
            pSceneControl.Scene.ClearSelection();
            pSceneControl.Scene.SceneGraph.RefreshViewers();
        }





    }
}
