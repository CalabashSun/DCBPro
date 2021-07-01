using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DCBProject.Database;
using DCBProject.Models;
using DCBProject.Util;

namespace DCBProject
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
            ReloadComboData();
        }

        private string IsNull(string data)
        {
            return string.IsNullOrEmpty(data) ? "" : data;
        }
        /// <summary>
        /// 删除基站
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var messButton = MessageBoxButtons.OKCancel;
            var dr = MessageBox.Show("确定删除该基站吗？删除该基站将把隶属该基站的点菜机也删除，请慎重！","删除基站", messButton);
            if (dr == DialogResult.OK)
            {
                //获取当前选中值并绑定列表数据
                var cuurentChoose = this.comboBox1.SelectedValue.ToString();
                StaionInfo.DeleteStation(cuurentChoose);
                ReloadComboData();
                ReloadDataList();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadDataList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form3 = new Form3();
            form3.ShowDialog();
            if (form3.DialogResult == DialogResult.Cancel)
            {
                ReloadComboData();
                ReloadDataList();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //获取当前选中值并绑定列表数据
            var cuurentChoose = this.comboBox1.SelectedValue.ToString();
            var form3 = new Form3(cuurentChoose);
            form3.ShowDialog();
            if (form3.DialogResult == DialogResult.Cancel)
            {
                ReloadComboData();
                ReloadDataList();
            }
        }
        /// <summary>
        /// 添加点菜宝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            var form4=new Form4("");
            form4.ShowDialog();
            if (form4.DialogResult == DialogResult.Cancel)
            {
                ReloadDataList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            //获取选中行
            var selectModel = this.dataGridView1.SelectedRows;
            if (selectModel.Count > 0)
            {
                var detailId = selectModel[0].Cells["Detail_Id"].Value.ToString();
                var form4 = new Form4(detailId);
                form4.ShowDialog();
                if (form4.DialogResult == DialogResult.Cancel)
                {
                    ReloadDataList();
                }
            }
            else
            {
                MessageBox.Show("请选择一行！");
            }


        }
        //删除点菜机
        private void button6_Click(object sender, EventArgs e)
        {
            var messButton = MessageBoxButtons.OKCancel;
            var dr = MessageBox.Show("确定删除该点菜机吗？请慎重！", "删除基站", messButton);
            if (dr == DialogResult.OK)
            {
                //获取选中行
                var selectModel = this.dataGridView1.SelectedRows;
                var detailId = selectModel[0].Cells["Detail_Id"].Value.ToString();
                StaionInfo.DeleteDetails(detailId);
                ReloadDataList();
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //清除默认选中的单元格
            this.dataGridView1.ClearSelection();
        }

        /// <summary>
        /// 重载combo数据
        /// </summary>
        private void ReloadComboData()
        {
            //获取基站数据
            var stationData = StaionInfo.Stations();
            var bindDatas = new List<StationList>();
            foreach (var dcbStation in stationData)
            {
                var bindData = new StationList();
                bindData.Id = dcbStation.Station_Id;
                bindData.Name = dcbStation.Station_No + "---" + IsNull(dcbStation.Station_Port) + IsNull(dcbStation.Station_Ip);
                bindDatas.Add(bindData);
            }
            //除去首次加载change
            this.comboBox1.SelectedIndexChanged -= new EventHandler(comboBox1_SelectedIndexChanged);
            this.comboBox1.DataSource = bindDatas;
            comboBox1.ValueMember = "Id";
            this.comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
            comboBox1.DisplayMember = "Name";
        }

        private void ReloadDataList()
        {
            //获取当前选中值并绑定列表数据
            var cuurentChoose = "";
            if (this.comboBox1.SelectedValue != null)
            {
                cuurentChoose = this.comboBox1.SelectedValue.ToString();
            }
            var details = StaionInfo.Details(cuurentChoose);
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = details;
        }

        /// <summary>
        /// 生成基站点菜机文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            var messButton = MessageBoxButtons.OKCancel;
            var dr = MessageBox.Show("确定要生成设置文件吗？", "生成文件", messButton);
            if (dr == DialogResult.OK)
            {
                //生成基站
                ParamSettingUtil.GenerateBaseStation();
                ParamSettingUtil.GenerateDcb();
                MessageBox.Show("生成设置文件成功！");
            }
        }
    }
}
