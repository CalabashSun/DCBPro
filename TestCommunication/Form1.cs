using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using DCBProject.Database;
using DCBProject.Models;
using DCBProject.Util;

namespace DCBProject
{
    public partial class Form1 : Form
    {
        #region WINAPI
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "FindWindowA", ExactSpelling = false, SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, EntryPoint = "PostMessageA", ExactSpelling = true, SetLastError = true)]
        private static extern int postMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        #endregion

        private IntPtr toHandler;

        #region Initia
        public Form1()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

        }
        #endregion

        #region Close_Load
        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确实要退出服务程序吗？退出本程序后将无法进行无线点单！", "博立服务端"
                , MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result != DialogResult.OK)
            {
                e.Cancel = true;//告诉窗体关闭这个任务取消
            }
            else
            {
                SendMessage(toHandler, (uint)0x10, (IntPtr)0, (IntPtr)0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "WX.exe";//需要启动的程序名  
            p.StartInfo.Arguments = this.Handle.ToString();//启动参数  
            p.Start();//启动  
            this.toHandler = FindWindow(null, "无线点菜机系统");
        }
        #endregion

        #region 接收点菜宝消息

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            //如果是来自点菜宝的消息
            if (m.Msg == 0x500)
            {
                //如果是首次
                if (toHandler == (IntPtr) 0)
                {
                    toHandler = m.LParam;
                }
                else
                {
                    int MessageTag = m.LParam.ToInt32();
                    if (MessageTag >= 1 && MessageTag <= 8)//处理1-8基站的请求，并回复
                    {
                        ProcessBaseStaion(MessageTag);
                        //发回响应通知WX.exe
                        SendMessage(toHandler, (uint)0x500, new IntPtr(0), new IntPtr(MessageTag));
                    }
                    else if (MessageTag == 10)////有登陆请求程序验证密码的正确性，内容在DL.TXT，请回复到DL.TXT。
                    {
                        FileUtil.WriteContent(@".\TXT\DL.TXT", "1");
                        //发回响应通知WX.exe
                        SendMessage(toHandler, (uint)0x500, new IntPtr(0), new IntPtr(10));
                    }else if (MessageTag == 11)
                    {
                        SendMessage(toHandler, (uint)0x500, new IntPtr(0), new IntPtr(11));
                    }
                }
            }
            base.WndProc(ref m);
        }

        #endregion

        #region 处理点菜宝消息
        /// <summary>
        /// 处理基站的请求
        /// 
        /// 所有WX.exe传送的消息，消息文本的第一行皆为固定格式‘请求类型’+‘点菜机编号’
        /// </summary>
        /// <param name="stationNo">基站的编号</param>
        private void ProcessBaseStaion(int stationNo)
        {
            try
            {
                Type type = typeof(DCBFunctionUtil);//针对不同的接口获得响应方法
                DCBFunctionUtil instance = (DCBFunctionUtil)Activator.CreateInstance(type);
                string fileName = "T" + stationNo + ".Txt";
                List<string> list = FileUtil.getContent(Application.StartupPath + @"\TXT\" + fileName);
                if (list != null && list.Count > 0)
                {

                    string[] orderLine = list[0].Split(' ');
                    orderLine = orderLine.Where(p => p != "").ToArray();
                    string requsetType = orderLine[0].Trim();//请求类型
                    string dcbNo = orderLine[1].Trim();//点菜宝编号
                    var method = type.GetMethod(requsetType + "Function");
                    Object[] parametors = new Object[] { stationNo, dcbNo, list };
                    method.Invoke(instance, parametors);
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion

        #region Rubbish
        private void button1_Click(object sender, EventArgs e)
        {
            var form2 = new Form2();
            form2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var postModel = new OrderModel();
            postModel.Taihao = "10";
            postModel.Remarks = "101";
            postModel.Renshu = "2";
            postModel.Details.Add(new OrderDetail
            {
                CpCount = "1",
                CpNumber = "03805",
                CpRemarks = "101"
            });
            postModel.Details.Add(new OrderDetail
            {
                CpCount = "3",
                CpNumber = "03807",
                CpRemarks = ""
            });
            postModel.Details.Add(new OrderDetail
            {
                CpCount = "1",
                CpNumber = "04112",
                CpRemarks = "102"
            });
            decimal cuurentTotalMoney = 0;
            var result = HttpUtil.PostAddOrder("/110302/HanZiDiner/DCB/TestAsp.asp", postModel, out cuurentTotalMoney);
            MessageBox.Show(result);
            LogsHelper.WriteLog(result);
            if (result == "")
            {
                MessageBox.Show("无法连接远程服务器");
            }
            else
            {
                MessageBox.Show(result);
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            var postModel = new OrderModel();
            postModel.Taihao = "01";
            postModel.Remarks = "110104101";
            postModel.Renshu = "2";
            //postModel.Details.Add(new OrderDetail
            //{
            //    CpCount = "1",
            //    CpNumber = "10132",
            //    CpRemarks = "101102",
            //    PackageCode = "03"
            //});
            //postModel.Details.Add(new OrderDetail
            //{
            //    CpCount = "1",
            //    CpNumber = "03913",
            //    CpRemarks = "",
            //    PackageCode = "03"
            //});
            //postModel.Details.Add(new OrderDetail
            //{
            //    CpCount = "1",
            //    CpNumber = "07140",
            //    CpRemarks = "",
            //    PackageCode = "03"
            //});
            postModel.Details.Add(new OrderDetail
            {
                CpCount = "2",
                CpNumber = "04101",
                CpRemarks = "115",
                PackageCode = ""
            });
            postModel.Details.Add(new OrderDetail
            {
                CpCount = "3",
                CpNumber = "01101",
                CpRemarks = "",
                PackageCode = ""
            });
            decimal currentTotalMoeny = 0;
            //获取当前餐桌的状态
            var result = "";
            var currentDesk = DCBOperations.DeskInfo(postModel.Taihao);
            if (currentDesk.EDState == 1 || currentDesk.EDState == null)
            {
                result = "请先开台再进行点菜操作";
            }
            else if (currentDesk.EDState == 2 && !string.IsNullOrEmpty(currentDesk.EDOrderNo))
            {
                postModel.SeatId = currentDesk.CQIDS;
                result = HttpUtil.PostAddOrder("/110302/HanZiDiner/DCB/AddOrder.asp", postModel, out currentTotalMoeny);
            }
            else
            {
                //加菜
                //获取当前订单数据
                var orderInfo = DCBOperations.OrderInfo(currentDesk.EDOrderNo);
                postModel.SeatId = orderInfo.Doc_SeatNumber;
                postModel.TotalCount = orderInfo.Doc_DpCount.ToString();
                postModel.TotalMoney = orderInfo.Doc_DpSum.ToString("0.00");
                postModel.OrderId = orderInfo.Doc_ID;

                result = HttpUtil.PostAddFood("/110302/HanZiDiner/DCB/AddFood.asp", postModel, out currentTotalMoeny);
            }

            MessageBox.Show(result);
            LogsHelper.WriteLog(result);
            if (result == "")
            {
                MessageBox.Show("无法连接远程服务器");
            }
            else
            {
                MessageBox.Show(result);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Type type = typeof(DCBFunctionUtil);//针对不同的接口获得响应方法
            DCBFunctionUtil instance = (DCBFunctionUtil)Activator.CreateInstance(type);
            string fileName = "T1.Txt";
            List<string> list = FileUtil.getContent(Application.StartupPath + @"\TXT\" + fileName);
            if (list != null && list.Count > 0)
            {

                string[] orderLine = list[0].Split(' ');
                orderLine = orderLine.Where(p => p != "").ToArray();
                string requsetType = orderLine[0].Trim();//请求类型
                string dcbNo = orderLine[1].Trim();//点菜宝编号
                var method = type.GetMethod(requsetType + "Function");
                Object[] parametors = new Object[] { 1, dcbNo, list };
                method.Invoke(instance, parametors);

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ExcDCBFile.CreatPackageByView();
            MessageBox.Show("成功了");
        }

        private void ShutExe_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Type type = typeof(DCBFunctionUtil);//针对不同的接口获得响应方法
            DCBFunctionUtil instance = (DCBFunctionUtil)Activator.CreateInstance(type);
            string fileName = "T" + 1 + ".Txt";
            List<string> list = FileUtil.getContent(Application.StartupPath + @"\TXT\" + fileName);
            if (list != null && list.Count > 0)
            {

                string[] orderLine = list[0].Split(' ');
                orderLine = orderLine.Where(p => p != "").ToArray();
                string requsetType = orderLine[0].Trim();//请求类型
                string dcbNo = orderLine[1].Trim();//点菜宝编号
                var method = type.GetMethod(requsetType + "Function");
                Object[] parametors = new Object[] { 1, dcbNo, list };
                method.Invoke(instance, parametors);

            }
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            Type type = typeof(DCBFunctionUtil);//针对不同的接口获得响应方法
            DCBFunctionUtil instance = (DCBFunctionUtil)Activator.CreateInstance(type);
            string fileName = "T" + 1 + ".Txt";
            List<string> list = FileUtil.getContent(Application.StartupPath + @"\TXT\" + fileName);
            if (list != null && list.Count > 0)
            {

                string[] orderLine = list[0].Split(' ');
                orderLine = orderLine.Where(p => p != "").ToArray();
                string requsetType = orderLine[0].Trim();//请求类型
                string dcbNo = orderLine[1].Trim();//点菜宝编号
                var method = type.GetMethod(requsetType + "Function");
                Object[] parametors = new Object[] { 1, dcbNo, list };
                method.Invoke(instance, parametors);

            }
        }

        private void button2_Click_3(object sender, EventArgs e)
        {
            Type type = typeof(DCBFunctionUtil);//针对不同的接口获得响应方法
            DCBFunctionUtil instance = (DCBFunctionUtil)Activator.CreateInstance(type);
            string fileName = "T" + 1 + ".Txt";
            List<string> list = FileUtil.getContent(Application.StartupPath + @"\TXT\" + fileName);
            if (list != null && list.Count > 0)
            {

                string[] orderLine = list[0].Split(' ');
                orderLine = orderLine.Where(p => p != "").ToArray();
                string requsetType = orderLine[0].Trim();//请求类型
                string dcbNo = orderLine[1].Trim();//点菜宝编号
                var method = type.GetMethod(requsetType + "Function");
                Object[] parametors = new Object[] { 1, dcbNo, list };
                method.Invoke(instance, parametors);

            }
        }
        private void button2_Click_4(object sender, EventArgs e)
        {
            Type type = typeof(DCBFunctionUtil);//针对不同的接口获得响应方法
            DCBFunctionUtil instance = (DCBFunctionUtil)Activator.CreateInstance(type);
            string fileName = "T" + 1 + ".Txt";
            List<string> list = FileUtil.getContent(Application.StartupPath + @"\TXT\" + fileName);
            if (list != null && list.Count > 0)
            {

                string[] orderLine = list[0].Split(' ');
                orderLine = orderLine.Where(p => p != "").ToArray();
                string requsetType = orderLine[0].Trim();//请求类型
                string dcbNo = orderLine[1].Trim();//点菜宝编号
                var method = type.GetMethod(requsetType + "Function");
                Object[] parametors = new Object[] { 1, dcbNo, list };
                method.Invoke(instance, parametors);

            }
        }

        private void button2_Click_5(object sender, EventArgs e)
        {
            Type type = typeof(DCBFunctionUtil);//针对不同的接口获得响应方法
            DCBFunctionUtil instance = (DCBFunctionUtil)Activator.CreateInstance(type);
            string fileName = "T" + 1 + ".Txt";
            List<string> list = FileUtil.getContent(Application.StartupPath + @"\TXT\" + fileName);
            if (list != null && list.Count > 0)
            {

                string[] orderLine = list[0].Split(' ');
                orderLine = orderLine.Where(p => p != "").ToArray();
                string requsetType = orderLine[0].Trim();//请求类型
                string dcbNo = orderLine[1].Trim();//点菜宝编号
                var method = type.GetMethod(requsetType + "Function");
                Object[] parametors = new Object[] { 1, dcbNo, list };
                method.Invoke(instance, parametors);

            }
        }
        #endregion


    }
}
