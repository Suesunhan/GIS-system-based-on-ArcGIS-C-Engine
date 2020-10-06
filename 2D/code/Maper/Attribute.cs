using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System.Collections;

namespace Maper
{
    public partial class Attribute : Form
    {
        //指针
        public IMapControl2 pMapControl;
        public IMap pMap;
        public int iLayerIndex;
        public int iFieldIndex;

        //加载地图 初始化图层和方法选项
        public Attribute(IMapControl2 CurrentMapControl)
        {
            InitializeComponent();
            //传入可供查询的地图
            this.pMapControl = CurrentMapControl;
            //将所有图层名添加进comboBoxLayer，并设置默认值为第一项
            for (int i = 0; i < pMapControl.LayerCount; i++)
            {
                comboBoxLayer.Items.Add(pMapControl.get_Layer(i).Name);
            }
            comboBoxLayer.SelectedIndex = 0;
            //设置comboBoxMethod的选择项，并设置默认值为第一项
            comboBoxMethod.Items.Add("新建选择集");
            comboBoxMethod.Items.Add("添加进已有的选择集");
            comboBoxMethod.Items.Add("从当前选择集中清除");
            comboBoxMethod.Items.Add("从当前选择集中再次筛选");
            comboBoxMethod.SelectedIndex = 0;
            //最初时获取唯一值框不可使用
        }

        private void Attribute_Load(object sender, EventArgs e)
        {

        }

        //Button按键
        private void buttonEqual_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " =";
        }

        private void buttonGreater_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " >";
        }

        private void buttonLess_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " <";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " _";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " %";
        }

        private void buttonIs_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " IS";
        }

        private void buttonBracket_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " <>";
        }

        private void buttonGreaterqual_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " >=";
        }

        private void buttonLessequal_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " <=";
        }

        private void buttonRoundbracket_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " ()";
        }

        private void buttonK_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " LIKE";
        }

        private void buttonN_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " AND";
        }

        private void buttonR_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " OR";
        }

        private void buttonT_Click(object sender, EventArgs e)
        {
            textBoxSelect.Text += " NOT";
        }

        //图层选择及图层要素显示
        private void comboBoxLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            //如果没有图层被选择，将不能添加字段
            //获取ListField内容
            if (comboBoxLayer.Text != null)
            {
                //清空listBoxField
                listBoxField.Items.Clear();
                //获取要素层
                IFeatureLayer pFeatureLayer = (IFeatureLayer)pMapControl.Map.get_Layer(comboBoxLayer.SelectedIndex);
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
                        listBoxField.Items.Add(pFeature.Fields.get_Field(i).Name);
                    }
                }
                //设置当前选择字段为第一个
                listBoxField.SelectedIndex = 0;
                //将选择语句的label描述信息修改
                labelSelect.Text = "SELECT * FROM " + comboBoxLayer.Text + " WHERE ：";
            }
        }

        //要素选择
        private void listBoxField_DoubleClick(object sender, EventArgs e)
        {
            IFeatureLayer pFeatureLayer = (IFeatureLayer)pMapControl.Map.get_Layer(comboBoxLayer.SelectedIndex);
            if (pFeatureLayer.DataSourceType == "Shapefile Feature Class")//shapefile文件
            {
                textBoxSelect.Text += " " + listBoxField.Text + " ";
            }
            else
            {
                textBoxSelect.Text += " " + listBoxField.Text + " ";
            }
        }

        //要素选择变化时改变不显示唯一值
        private void listBoxField_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxValue.Visible = false;
            buttonUniqueValue.Enabled = true;
        }

        //获取唯一值
        private void buttonUniqueValue_Click(object sender, EventArgs e)
        {
            //获取要素图层与要素类，将其作为参数传入UniqueValue()函数
            IFeatureLayer pFeatureLayer = (IFeatureLayer)pMapControl.get_Layer(comboBoxLayer.SelectedIndex);
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            //将返回的所有值存入allValue数组中,并进行排序
            string[] allValue = UniqueValue(pFeatureClass, listBoxField.Text);
            Array.Sort(allValue);
            //获取字段对象，用于在将其值添加进listboxValue中时判断字段类型
            IFeatureCursor pFeatureCursor = pFeatureLayer.Search(null, true);
            IFeature pFeature = pFeatureCursor.NextFeature();
            IField pField = new FieldClass();
            for (int j = 0; j < pFeature.Fields.FieldCount; j++)
            {
                if (listBoxField.Text == pFeature.Fields.get_Field(j).Name)
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
            buttonUniqueValue.Enabled = false;
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

        //唯一值选择
        private void listBoxValue_DoubleClick(object sender, EventArgs e)
        {
            textBoxSelect.Text += " " + listBoxValue.Text;
        }

        //清除唯一值
        private void buttonValueClear_Click(object sender, EventArgs e)
        {
            listBoxValue.Items.Clear();
        }

        //应用
        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (textBoxSelect.Text != "")
            {
                //获取图层
                IFeatureLayer lFeatureLayer = (IFeatureLayer)pMapControl.get_Layer(comboBoxLayer.SelectedIndex);
                IFeatureSelection lFeatureSelection = (IFeatureSelection)lFeatureLayer;
                //判断选择的SQL方法的类型
                esriSelectionResultEnum lesriSREnum = esriSelectionResultEnum.esriSelectionResultNew;
                //选择查询方法
                switch (comboBoxMethod.SelectedIndex)
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
                pMapControl.ActiveView.Refresh();



            }
            else
            {
                MessageBox.Show("请选择需要查询的图层");
            }
        }

        //清除选择语句
        private void buttonSelectClear_Click(object sender, EventArgs e)
        {
            textBoxSelect.Clear();
        }

        //清除所选内容
        private void buttonAllClear_Click(object sender, EventArgs e)
        {
            IMap pMap = pMapControl.Map;
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
