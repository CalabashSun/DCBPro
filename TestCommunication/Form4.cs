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
    public partial class Form4 : Form
    {
        public static Dcb_Detail oldInfo = null;
        public static List<Dcb_Station> stationData = new List<Dcb_Station>();
        public Form4()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        public Form4(string detailId)
        {
            oldInfo = null;
            InitializeComponent();
            //绑定combobox
            //获取基站数据
            stationData = StaionInfo.Stations();
            var bindDatas = new List<StationList>();
            foreach (var dcbStation in stationData)
            {
                var bindData = new StationList();
                bindData.Id = dcbStation.Station_Id;
                bindData.Name = dcbStation.Station_No + "---" + IsNull(dcbStation.Station_Port) + IsNull(dcbStation.Station_Ip);
                bindDatas.Add(bindData);
            }

            this.comboBox1.DataSource = bindDatas;
            comboBox1.ValueMember = "Id";
            comboBox1.DisplayMember = "Name";
            if (!string.IsNullOrEmpty(detailId))
            {
                var detailInfo = StaionInfo.Detail(detailId);
                this.textBox1.Text = detailInfo.Detail_No;
                this.comboBox1.SelectedItem = bindDatas.First(p => p.Id == detailInfo.Station_Id);
                oldInfo = detailInfo;
            }
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }



        private string IsNull(string data)
        {
            return string.IsNullOrEmpty(data) ? "" : data;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var chooseStation = stationData.First(p => p.Station_Id == comboBox1.SelectedValue.ToString()).Station_No;
            if (oldInfo == null)
            {
                //新增
                var addModel = new Dcb_Detail
                {
                    Detail_Id = ToolUtil.GenerageNumber(),
                    Detail_No = textBox1.Text,
                    Station_Id = comboBox1.SelectedValue.ToString(),
                    Station_No = chooseStation
                };

                StaionInfo.AddDetail(addModel);
            }
            else
            {
                //修改
                oldInfo.Detail_No = textBox1.Text;
                oldInfo.Station_Id = comboBox1.SelectedValue.ToString();
                oldInfo.Station_No = chooseStation;
                StaionInfo.UpdateDetail(oldInfo);
            }

            var dr = MessageBox.Show("提交数据成功!");
            if (dr == DialogResult.OK)
            {
                //更新form2中的数据

                this.Close();
            }
            else
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
