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
    public partial class Form3 : Form
    {
        public  static Dcb_Station oldInfo =null;
        public Form3()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            this.radioButton2.Checked = true;
        }

        public Form3(string stationId)
        {
            oldInfo = null;
            InitializeComponent();
            
            if (!string.IsNullOrEmpty(stationId))
            {
                var stationInfo = StaionInfo.Station(stationId);
                oldInfo = stationInfo;
                this.radioButton1.Checked = !string.IsNullOrEmpty(stationInfo.Station_Port);
                this.radioButton2.Checked = !string.IsNullOrEmpty(stationInfo.Station_Ip);
                this.textBox1.Text = stationInfo.Station_No;
                this.textBox2.Text = stationInfo.Station_Port ?? "";
                this.textBox3.Text = stationInfo.Station_Ip ?? "";
            }
            else
            {
                this.radioButton2.Checked = true;
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (oldInfo==null)
            {
                //新增
                var addModel = new Dcb_Station
                {
                    Station_Id = ToolUtil.GenerageNumber(),
                    Station_No = textBox1.Text
                };
                if (this.radioButton1.Checked)
                {
                    addModel.Station_Port = this.textBox2.Text;
                }
                else
                {
                    addModel.Station_Ip = this.textBox3.Text;
                }
                StaionInfo.AddStation(addModel);
            }
            else
            {
                //修改
                oldInfo.Station_No = textBox1.Text;
                if (this.radioButton1.Checked)
                {
                    oldInfo.Station_Port = this.textBox2.Text;
                }
                else
                {
                    oldInfo.Station_Ip = this.textBox3.Text;
                }
                StaionInfo.UpdateStation(oldInfo);
            }

            var dr = MessageBox.Show("提交数据成功!");
            if (dr == DialogResult.OK)
            {
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
