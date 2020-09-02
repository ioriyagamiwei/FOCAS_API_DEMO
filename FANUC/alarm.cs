﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace FANUC
{
    public partial class alarm : Form
    {
        public alarm()
        {
            InitializeComponent();
        }
        Fanuc.ODBSYS sys = new Focas1.ODBSYS();
        public void rd_sys_info()//读取cnc系统内的信息，比如轴数dengdeng
        {
            Fanuc.cnc_sysinfo(Fanuc.h, sys);
            toolStripStatusLabel1.Text = "主轴数:  " + sys.axes[0].ToString();
            toolStripStatusLabel2.Text = "被控制的伺服轴数;    " + sys.axes[1].ToString();


        }
        Fanuc.IODBTIMER timer = new Focas1.IODBTIMER();
        public void get_timer()//获取时间
        {
            Fanuc.cnc_gettimer(Fanuc.h, timer);
            string time = timer.date.year + " 年" + timer.date.month + "  月" + timer.date.date + "  日" + timer.time.hour + "  时" + timer.time.minute + "  分" + timer.time.second + "  秒";
            toolStripStatusLabel3.Text = time;


        }
        public void set_timer()//设置时间
        {
            Fanuc.cnc_settimer(Fanuc.h, timer);
        }


        //有报警有关的
        Fanuc.OPMSG opmsg = new Focas1.OPMSG();
        public void get_op_msg()//得到 操作信息的内容
        {
            short ret = Fanuc.cnc_rdopmsg(Fanuc.h, 0, 6 + 256, opmsg);
            string str2 = "msg";

            System.Type type = opmsg.GetType();


            if (ret == 0)
            {
                for (int i = 1; i < 6; i++)
                {
                    str2 = "msg" + i;
                    object obj = type.GetField(str2).GetValue(opmsg);
                    System.Type type2 = obj.GetType();
                    if (Convert.ToInt16(type2.GetField("datano").GetValue(obj)) != -1)
                    {
                        if (Convert.ToInt16(type2.GetField("datano").GetValue(obj)) == 0)
                        {

                            break;
                        }
                        else
                        {
                            listBox3.Items.Add("操作信息： " + type2.GetField("datano").GetValue(obj).ToString() + type2.GetField("data").GetValue(obj).ToString());
                        }
                    }
                    else
                        MessageBox.Show("无操作信息");
                }
            }

            else
            {

                MessageBox.Show(ret + " ");
            }


        }
        Fanuc.ODBST obst = new Focas1.ODBST();
        public void get_state()//获取机器的状态
        {
            Fanuc.cnc_statinfo(Fanuc.h, obst);
            listBox4.Items.Add("运行状态" + obst.run);//0停止，1待机，开动
            listBox4.Items.Add("警报状态" + obst.alarm);//0没有警报，1表示有警报
            listBox4.Items.Add("自动模式下的选择" + obst.aut);
            listBox4.Items.Add("手动模式下的选择" + obst.mstb);

        }
        public void clear_alarm()//清除报警
        {
            short ret = Fanuc.cnc_clralm(Fanuc.h, 0);
            if (ret == 0)
                MessageBox.Show("清除成功");

        }
        public void get_alarm_info()//获取报警信息
        {
            short a = 1;//1：表示有消息；0：无消息
            short b = 5;//b表示警报的类型，比如5：电机温度过热
            short c = 8;
            short ret = Fanuc.cnc_rdalminfo(Fanuc.h, a, b, c, odbalm_1);
            if (ret == 0)
                listBox1.Items.Add("警报：" + odbalm_1.msg1.alm_no);//表示
            else

                MessageBox.Show(ret + " ");

        }
        Fanuc.ODBALM alm = new Focas1.ODBALM();
        public void get_alarm_state()//获取警报的状态
        {

            string[] almmsg = new string[] {
                "P/S 100 ALARM","P/S 000 ALARM",
                "P/S 101 ALARM","P/S ALARM (1-255)",
                "OT ALARM",     "OH ALARM",
                "SERVO ALARM",  "SYSTEM ALARM",
                "APC ALARM",    "SPINDLE ALARM",
                "P/S ALARM (5000-)"};
            int[] a = new int[11];
            short ret = Fanuc.cnc_alarm2(Fanuc.h, out a[0]);
            if (ret == 0)
            {
                for (int i = 0; i < a.Length; i++)
                    if (a[i] == 0)
                        listBox1.Items.Add("NO ALARM");
                    else
                    {

                        listBox1.Items.Add("警报显示：" + almmsg[i]);

                    }
            }
            else
            {

                MessageBox.Show(ret + " ");
            }

        }
        public ushort get_alarm()
        {
            return 1;
        }
        public void get_error()//警报解决方式
        {
            ushort return_numer = get_alarm();
            switch (return_numer)
            {
                case 35: break;
                case 0:
                    MessageBox.Show("no alarm(无警告)", "提示");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:no alarm(无警告)");

                    break;
                case 1:
                    MessageBox.Show("Power off parameter set(关机参数检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:Power off parameter set(关机参数检查)");
                    break;
                case 2:
                    MessageBox.Show(" I/O error (输入输出检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:I/O error (输入输出检查)");
                    break;
                case 3:
                    MessageBox.Show("  Foreground P/S  (前景检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:Foreground P/S  (前景检查)");
                    break;
                case 4:
                    MessageBox.Show("  Overtravel,External data  (外部数据检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:Overtravel,External data  (外部数据检查)");
                    break;
                case 5:
                    MessageBox.Show("  Overheat alarm  (温度过热检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:Overheat alarm  (温度过热检查");
                    break;
                case 6:
                    MessageBox.Show("  Servo alarm   (伺服报警检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:Power off parameter set(关机参数检查)");
                    break;
                case 7:
                    MessageBox.Show("  Data I/O error   (数据i/o检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容: Data I/O error   (数据i/o检查)");
                    break;
                case 8:
                    MessageBox.Show("   Macro alarm    (宏报警,检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:Macro alarm    (宏报警,检查)");
                    break;
                case 9:
                    MessageBox.Show("  Spindle alarm    (主轴报警,检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容: Spindle alarm    (主轴报警,检查)");
                    break;
                case 10:
                    MessageBox.Show("  Other alarm(DS)    (其他报警,检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:Other alarm(DS)    (其他报警,检查)");
                    break;
                case 11:
                    MessageBox.Show("  O Alarm concerning Malfunction prevent functions (有关故障报警预防功能,检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容: O Alarm concerning Malfunction prevent functions (有关故障报警预防功能,检查)");
                    break;
                case 12:
                    MessageBox.Show("   Background P/S  (背景P / S,检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:Background P/S  (背景P / S,检查))");
                    break;
                case 13:
                    MessageBox.Show("   Syncronized error (Syncronized错误,检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:  Background P/S  (背景P / S,检查))");
                    break;
                case 14:
                    MessageBox.Show("   (reserved) (保存,检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容:(reserved) (保存,检查)");
                    break;
                case 15:
                    MessageBox.Show("   External alarm message(外部报警,检查)", "警告");
                    listBox1.Items.Add("时间：  " + DateTime.Now.Date.ToString() + "警告内容: External alarm message(外部报警,检查)");
                    break;

            }

        }
        Fanuc.ODBALMMSG2 msg = new Focas1.ODBALMMSG2();
        public void get_message()//获取报警消息
        {
            short b = 8;
            short ret = Fanuc.cnc_rdalmmsg2(Fanuc.h, -1, ref b, msg);


            string str1 = "msg";

            System.Type type = msg.GetType();


            if (ret == 0)
            {
                for (int i = 1; i < 9; i++)
                {
                    str1 = "msg" + i;
                    object obj = type.GetField(str1).GetValue(msg);
                    System.Type type1 = obj.GetType();
                    listBox1.Items.Add("警报数 " + type1.GetField("alm_no").GetValue(obj).ToString() + type1.GetField("alm_msg").GetValue(obj).ToString());
                }
            }

            else
            {

                MessageBox.Show(ret + " ");
            }


        }
        Fanuc.ODBALM odbalm = new Focas1.ODBALM();
        Fanuc.ALMINFO_1 odbalm_1 = new Focas1.ALMINFO_1();



        //z诊断
        Fanuc.ODBDGN_1 odbd = new Focas1.ODBDGN_1();
        Fanuc.ODBDGN_2 odbd_2 = new Focas1.ODBDGN_2();
        Fanuc.ODBDGN_3 odbd_3 = new Focas1.ODBDGN_3();
        Fanuc.ODBDIAGNUM datrng = new Focas1.ODBDIAGNUM();
        Fanuc.ODBDIAGIF gif = new Focas1.ODBDIAGIF();// cnc_rddiaginfo
        public void get_diagnosr()//读取诊断的数据
        {
            short numer;
            short axes; //1表示x轴，2表示y轴，3表示z轴//attribute主要指哪个轴；
            if (textBox1.Text != null)
            {
                numer = Convert.ToInt16(textBox1.Text);
                for (short i = 1; i < 4; i++)
                {
                    axes = i;
                    short ret = Fanuc.cnc_diagnoss(Fanuc.h, numer, axes, 4 + 4 * 1, odbd_2);
                    if (ret == 0)
                    {
                        listBox2.Items.Add("诊断参数号" + odbd_2.datano + "  " + odbd_2.type + "      :" + odbd_2.rdata.dgn_val * Math.Pow(10, -odbd_2.rdata.dec_val));
                    }
                    else


                        MessageBox.Show(ret + " ");
                }
            }
            else
                MessageBox.Show("您没有输入参数号", "警告");





        }
        public void get_diagnosr_inf()//读取诊断信息
        {
            ushort read_no = 8;
            Fanuc.cnc_rddiaginfo(Fanuc.h, 309, read_no, gif);
            listBox1.Items.Add(gif.info.info1.diag_no);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            // get_error();
            get_alarm_state();//成功读取

        }



        //读取有关维修的
        Fanuc.IODBITEM tem = new Focas1.IODBITEM();
        public void get_mente_special()//读取维修的机床项目
        {
            listBox5.Items.Clear();
            short a = 1;//说明开始读取的序列号
            short b = 11;//表示读取的维修项目的数量
            short ret = Fanuc.cnc_rdpm_mcnitem(Fanuc.h, a, ref b, tem);//ref b，返回的是机器里实际读取的数量
            string str2 = "name";

            System.Type type = tem.GetType();


            if (ret == 0)
            {
                for (int i = 1; i < b; i++)
                {
                    str2 = "name" + i;
                    listBox5.Items.Add(i + "    ：" + type.GetField(str2).GetValue(tem).ToString());
                }
            }
            else
                MessageBox.Show(ret + " ");


        }
        Fanuc.IODBPMAINTE inte = new Focas1.IODBPMAINTE();
        public void get_mente_state()//读取维修的状态项目
        {
            listBox6.Items.Clear();
            short a = 1;//开始
            short b = 10;//数量
            short ret = Fanuc.cnc_rdpm_item(Fanuc.h, a, ref  b, inte);
            string str1 = "data";

            System.Type type = inte.GetType();//返回Type类型的当前对象的类型。


            if (ret == 0)
            {
                for (int i = 1; i < b; i++)
                {
                    str1 = "data" + i;
                    object obj = type.GetField(str1).GetValue(inte);
                    System.Type type1 = obj.GetType();
                    listBox6.Items.Add(i + "   " + type1.GetField("name").GetValue(obj).ToString() + " 寿命  " + type1.GetField("total").GetValue(obj).ToString() + type1.GetField("type").GetValue(obj).ToString());
                    listBox6.Items.Add("状态：   " + type1.GetField("stat").GetValue(obj).ToString());
                }
            }
            else
                MessageBox.Show(ret + " ");


        }
        public void get_peridoc_cnc()//读取维修的cn项目,30i没有
        {
            short a = 1;
            short b = 9;
            short ret = Fanuc.cnc_rdpm_cncitem(Fanuc.h, a, ref b, tem);
            string str2 = "name";

            System.Type type = tem.GetType();


            if (ret == 0)
            {
                for (int i = 1; i < b; i++)
                {
                    str2 = "name" + i;
                    listBox1.Items.Add(i + "  项目:" + type.GetField(str2).GetValue(tem).ToString());
                }
            }
            else
                MessageBox.Show(ret + " ");

        }
        public static short b = 2;
        public void cnc_wrpm_mcnitem()//写入维修的机床项目
        {
            short a = 1;
            if ((b - a) > 10)
            {
                MessageBox.Show("存储已满，你所输入的值将会代替第一个");
                b = 2;
            }
            else
            {

                string str2 = "name";

                var cls = tem.GetType();//读取类名
                str2 = "name" + (b - a);
                FieldInfo fs = cls.GetField(str2);//读取字段
                if (textBox2.Text == null)
                    MessageBox.Show("没有输入");
                else
                {
                    string m = textBox2.Text;
                    fs.SetValue(tem, m);
                    short ret = Fanuc.cnc_wrpm_mcnitem(Fanuc.h, a, b, tem);
                    if (ret == 0)
                        MessageBox.Show("成功");
                }

                b++;
            }
        
          
          
          
            
          
          
            

          
           



        }
        public static short a = 1;
        public void cnc_wrpm_item()//写入维修的状态项目
        {
            
            short b=1;//数量
            short type=1;
            if (a > 10)
            {
                MessageBox.Show("存储已满，你所输入的值将会代替第一个");
                a= 1;
            }
            else
            {
                if (textBox3.Text == null)
                {
                    MessageBox.Show("没有输入项目");
                }
                else
                {
                    string na = textBox3.Text;
                    string str1 = "data" +  a.ToString();
                    var p = inte.GetType();
                    object obj = p.GetField(str1).GetValue(inte);
                    System.Type ob = obj.GetType();
             
                    var data = ob.GetRuntimeField("name");//读取类中的类里面的字段
                    data.SetValue(obj, na);
                    //MessageBox.Show(inte.data1.name);
                    short ret = Fanuc.cnc_wrpm_item(Fanuc.h, a, type, b, inte);
                    if (ret == 0)
                    {
                        MessageBox.Show("输入成功");
                        a++;
                    }
                    
                   

                
                }
            }
           
        }


        //读取有关程序启动的信息
        Fanuc.ODBPRS prs=new Focas1.ODBPRS ();
        public void cnc_rdprstrinfo()//读取有关程序启动的信息
        {
            Fanuc.cnc_rdprstrinfo(Fanuc.h, prs);
            for (int i = 0; i < prs.data_info.Length; i++)
                listBox7.Items.Add(prs.data_info[i]);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            get_message();//成功读取
        }

        private void button3_Click(object sender, EventArgs e)
        {
            get_diagnosr();//成功读取
            //  get_diagnosr_inf();
        }

        private void btnConc_Click_1(object sender, EventArgs e)
        {


        }

        private void alarm_Load(object sender, EventArgs e)
        {
            Fanuc.cnc_rddiagnum(Fanuc.h, datrng);
            label6.Text = " 范围在" + datrng.diag_min + "-" + datrng.diag_max;
            rd_sys_info();
            get_timer();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            get_op_msg();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            get_state();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            clear_alarm();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            get_mente_special();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            get_mente_state();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            get_peridoc_cnc();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            cnc_wrpm_mcnitem();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            cnc_wrpm_item();

        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
