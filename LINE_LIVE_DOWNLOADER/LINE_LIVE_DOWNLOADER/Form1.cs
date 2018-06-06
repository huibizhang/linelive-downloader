using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;

namespace LINE_LIVE_DOWNLOADER
{
    public partial class Form1 : Form
    {
        Downloader Downloader1 = new Downloader();
        Thread T1;

        int m_x, m_y;
        Boolean drag;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Downloader1.listBox1 = listBox1;
            Downloader1.pictureBox1 = pictureBox1;
            Downloader1.textBox2 = textBox2;
            Downloader1.comboBox1 = comboBox1;
            Downloader1.progressBar1 = progressBar1;
            Downloader1.button2 = button2;
            Downloader1.button3 = button3;
            Downloader1.textBox1 = textBox1;
            T1 = new Thread(Downloader1.Download);
            T1.Start();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            T1.Abort();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox1.Text != "" && e.KeyChar == System.Convert.ToChar(13))
            {
                Downloader1.Url = textBox1.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                Downloader1.Url = textBox1.Text;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drag = true;
                m_x = e.X;
                m_y = e.Y;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag) {
                this.Left += e.X - m_x;
                this.Top += e.Y - m_y;
                panel1.Cursor = Cursors.SizeAll;
                panel2.Cursor = Cursors.SizeAll;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(Downloader1.R[comboBox1.SelectedIndex].Url);
            if (comboBox1.SelectedIndex != -1)
            {
                if (comboBox1.SelectedIndex == 1)
                {
                    button3.Text = "下載　" + Downloader1.R[comboBox1.SelectedIndex].Name + "　MP3";
                }
                else
                {
                    button3.Text = "下載　" + Downloader1.R[comboBox1.SelectedIndex].Name + "p　MP4";
                }
                button3.Enabled = true;
            }
            else {
                button3.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Downloader1.SelectedIndex = comboBox1.SelectedIndex;
            string Filename_temp;
            if (comboBox1.SelectedIndex == 1)
            {
                saveFileDialog1.FileName = textBox2.Text + ".mp3";
            }
            else
            {
                saveFileDialog1.FileName = textBox2.Text + "_" + Downloader1.R[comboBox1.SelectedIndex].Name + ".mp4";
            }
            Filename_temp = saveFileDialog1.FileName;
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            while (saveFileDialog1.FileName == Filename_temp) {
                saveFileDialog1.ShowDialog();
            }
            //MessageBox.Show(saveFileDialog1.FileName);
            Downloader1.FileName = saveFileDialog1.FileName;
            button3.Enabled = false;
            comboBox1.Enabled = false;
            textBox1.Enabled = false;
            button2.Enabled = false;
            Downloader1.Downloading = true;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drag = false;

                if(this.Left < 0)
                {
                    this.Left = 0;
                }
                else if(this.Left+this.Width > Screen.PrimaryScreen.WorkingArea.Width)
                {
                    this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width ;
                }

                if (this.Top < 0)
                {
                    this.Top = 0;
                }
                else if (this.Top + this.Height > Screen.PrimaryScreen.WorkingArea.Height)
                {
                    this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
                }
                panel1.Cursor = Cursors.Default;
                panel2.Cursor = Cursors.Default;
            }
        }
    }

    public class Resolution {
        private String _Name = "";
        private String _Url = "";

        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public String Url
        {
            get
            {
                return _Url;
            }
            set
            {
                _Url = value;
            }
        }

        public void Setting(String Name, String Url) {
            _Name = Name;
            _Url = Url;
        }
    }

    class Downloader {
        WebClient WebClient1 = new WebClient();
        public String Url = "";
        String[] Splitter = { "lsaPath&quot;:&quot;live/pba/" };
        String[] Splitter2 = { " : " };
        public ListBox listBox1 = new ListBox();
        public TextBox textBox1 = new TextBox();
        public TextBox textBox2 = new TextBox();
        public PictureBox pictureBox1 = new PictureBox();
        public ComboBox comboBox1 = new ComboBox();
        public ProgressBar progressBar1 = new ProgressBar();
        public Button button2 = new Button();
        public Button button3 = new Button();

        public Resolution[] R = new Resolution[6];

        public Boolean Downloading = false;
        public int SelectedIndex;
        public string FileName;

        public void Download() {
            while (true) {
                if (Downloading)
                {
                    string m3u8 = System.Text.Encoding.UTF8.GetString(WebClient1.DownloadData(R[SelectedIndex].Url));
                    string Url_temp = R[SelectedIndex].Url.Replace("m3u8","");
                    m3u8 = m3u8.Split(',')[m3u8.Split(',').Length - 1];
                    int last_count = int.Parse(m3u8.Split('.')[1]);
                    string download_file_name = "";
                    System.IO.BinaryWriter BW = new System.IO.BinaryWriter(System.IO.File.OpenWrite(FileName));
                    for (int i = 0; i <= last_count; i++) {
                        progressBar1.Value = (100* i / last_count) ;
                        textBox2.Text ="Downloading " + (100 * i / last_count) + " % ("+ i + "/" + last_count +")";
                        if (i < 10)
                        {
                            download_file_name = Url_temp + "0000" + i + ".ts";                        }
                        else if (i < 100)
                        {
                            download_file_name = Url_temp + "000" + i + ".ts";
                        }
                        else if (i < 1000)
                        {
                            download_file_name = Url_temp + "00" + i + ".ts";
                        }
                        else if (i < 10000)
                        {
                            download_file_name = Url_temp + "0" + i + ".ts";
                        }
                        else {
                            download_file_name = Url_temp + i + ".ts";
                        }
                        BW.Write(WebClient1.DownloadData(download_file_name));
                        //progressBar1.Value += (i / last_count * 50);
                    }
                    BW.Flush();
                    BW.Close();
                    textBox1.Enabled = true;
                    button2.Enabled = true;
                    MessageBox.Show(FileName + " 下載完成");
                    Downloading = false;
                }
                else {
                    if (Url != "")
                    {
                        pictureBox1.Image = Properties.Resources.loading;
                        listBox1.Items.Clear();
                        comboBox1.Items.Clear();
                        try
                        {
                            //Application.DoEvents();
                            //richTextBox1.Text += "正在下載資料...\n";
                            string temp = System.Text.Encoding.UTF8.GetString(WebClient1.DownloadData(Url));
                            string temp2 = temp.Substring(temp.IndexOf("<title>") + "<title>".Length, temp.IndexOf("</title>") - temp.IndexOf("<title>") - "<title>".Length);
                            textBox2.Text = temp2.Substring(0, temp2.IndexOf(" - "));
                            //richTextBox1.Text += "正在解析資料...\n";
                            temp = temp.Split(Splitter, StringSplitOptions.RemoveEmptyEntries)[temp.Split(Splitter, StringSplitOptions.RemoveEmptyEntries).Length - 1];
                            temp = temp.Substring(0, temp.IndexOf("&quot"));
                            //richTextBox1.Text += "正在下載資料...\n";
                            //richTextBox1.Text += "https://lssapi.line-apps.com/v1/vod/playInfo?contentId=live/pba/" + temp + "&scheme=https&\n";
                            listBox1.Items.AddRange(System.Text.Encoding.UTF8.GetString(WebClient1.DownloadData("https://lssapi.line-apps.com/v1/vod/playInfo?contentId=live/pba/" + temp + "&scheme=https&")).Replace("\"", "").Split(','));
                            temp = listBox1.Items[8].ToString().Split(Splitter2, StringSplitOptions.RemoveEmptyEntries)[1];
                            pictureBox1.Load(temp.Replace(temp.Split('/')[temp.Split('/').Length - 1], "f375x375"));
                            comboBox1.Enabled = true;
                            for (int i = 2; i < 9; i++)
                            {
                                string[] Rtemp = listBox1.Items[i].ToString().Split(Splitter2, StringSplitOptions.RemoveEmptyEntries);
                                R[i - 2] = new Resolution();
                                R[i - 2].Setting(Rtemp[0].Replace(System.Convert.ToChar(10) + "", "").Trim(), Rtemp[1].Substring(0, Rtemp[1].IndexOf("m3u8") + 4));
                                comboBox1.Items.Add(Rtemp[0]);
                            }
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                        Url = "";
                    }
                }
            }
        }
    }
}
