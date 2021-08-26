using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChartTest
{
    public partial class PictureBoxChart : ChartForm
    {
        Color[] chColors;

        public PictureBoxChart(PictureBox pictureBox)
        {
            InitializeComponent();
            pictureBox1 = pictureBox;
            int width = pictureBox1.ClientRectangle.Width;
            int height = pictureBox1.ClientRectangle.Height;
            pictureBox1.Image = new Bitmap(width, height);
            chColors = new Color[MAX_CH];
            for (int i = 0; i < chColors.Length; i++)
            {
                chColors[i] = Color.Black;
            }
            chColors[0] = Color.Red;
            chColors[1] = Color.Purple;
        }

        public override void setDatas(double[][] inputData)
        {
            int i = 0;
            dataX = new double[inputData.Length];
            dataY = new double[inputData.Length];
            foreach (double[] d in inputData)
            {
                dataX[i] = d[0];
                dataY[i++] = d[1];
            }

        }

        public override void setDatas(int ch, double[] inputDataX,double[] inputDataY)
        {
            dataXs[ch] = inputDataX;
            dataYs[ch] = inputDataY;
        }

        public override void PlotData(bool clear = true)
        {
            using (var g = Graphics.FromImage(pictureBox1.Image))
            {
                // イメージをクリア
                if(clear)
                    g.Clear(Color.White);

                Font font = new Font("MS UI Gothic", 10);
                Pen p = new Pen(Color.Black, 1);
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                // 罫線
                for(int i = 0; i <= 10; i++)
                {
                    // 縦線
                    g.DrawLine(p, pictureBox1.Width / 10 * i, 0, pictureBox1.Width / 10 * i, pictureBox1.Height);
                    g.DrawString(((axisX.max-axisX.min)/10*i + axisX.min).ToString(),font, Brushes.Blue, pictureBox1.Width / 10 * i, pictureBox1.Height-20);

                    // 横線
                    g.DrawLine(p, 0, pictureBox1.Height / 10 * i, pictureBox1.Width, pictureBox1.Height / 10 * i);
                    g.DrawString((-((axisY.max - axisY.min) / 10 * i + axisY.min)).ToString(), font, Brushes.Blue, 0, pictureBox1.Height/10*i);
                }

                // 枠線
                p.Width = 2;
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                g.DrawRectangle(p,1,1, pictureBox1.Width-2, pictureBox1.Height-2);

                // 中心線描画
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                g.DrawLine(p, 0, pictureBox1.Height/2, pictureBox1.Width, pictureBox1.Height / 2);
                g.DrawLine(p, pictureBox1.Width/2, 0, pictureBox1.Width / 2, pictureBox1.Height);

                // 線を描画
                bool multi = true;
                if(multi)
                {
                    for(int ch = 0; ch < MAX_CH; ch++)
                    {
                        for (int i = 1; i < dataXs[ch].Length; i++)
                        {
                            int x1 = covertXvalue(dataXs[ch][i - 1]);
                            int y1 = convertYvalue(dataYs[ch][i - 1]);
                            int x2 = covertXvalue(dataXs[ch][i]);
                            int y2 = convertYvalue(dataYs[ch][i]);

                            try
                            {
                                g.DrawLine(new Pen(chColors[ch]), x1, y1, x2, y2);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                        }
                    }
                }
                else
                    for (int i = 1; i < dataX.Length; i ++)
                    {
                        int x1 = covertXvalue(dataX[i - 1]);
                        int y1 = convertYvalue(dataY[i-1]);
                        int x2 = covertXvalue(dataX[i]);
                        int y2 = convertYvalue(dataY[i]);

                        try{
                            g.DrawLine(Pens.Red, x1, y1, x2, y2);
                        }catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    
                    }


            }

            // PictureBoxを再描画
            pictureBox1.Refresh();
        }

        private int convertYvalue(double value)
        {
            int y = 0;
            double valueParPixcel = (double)pictureBox1.Height / (axisY.max - axisY.min);
            y = (int)(pictureBox1.Height - (value- axisY.min) * valueParPixcel);

            return y;
        }

        private int covertXvalue(double value)
        {
            int x = 0;
            double valueParPixcel = (double)pictureBox1.Width / (axisX.max - axisX.min);
            x = (int)((value - axisX.min) * valueParPixcel);

            return x;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            PlotLoop(LOOP_COUNT);
        }

        // Vertical
        private void ScrollVertical_trackBar_ValueChanged(object sender, EventArgs e)
        {

            int v = ScrollVertical_trackBar.Value;
            axisY.max = v*5;
            axisY.min = v * -5;
            PlotData();
        }

        private void ScrollHorizontal_trackBar_Scroll(object sender, EventArgs e)
        {
            int v = ScrollHorizontal_trackBar.Value;
            axisX.max = v * 5;
            axisX.min = v * -5;
            PlotData();
        }
    }
}
