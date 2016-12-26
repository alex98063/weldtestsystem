using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WeldTestSystem
{
    class XYLinesFactory
    {
        #region   画出X轴与Y轴
        /// <summary>
        /// 在任意的panel里画一个坐标，坐标所在的四边形距离panel边50像素
        /// </summary>
        /// <param name="pan"></param>
        public static void DrawXY(Panel pan)
        {
            Graphics g = pan.CreateGraphics();
            //整体内缩move像素
            float move = 30f;
            float newX = pan.Width - move;
            float newY = pan.Height - move;
            

            //绘制X轴,
            PointF px1 = new PointF(move, pan.Height/2);
            PointF px2 = new PointF(newX, pan.Height / 2);
            g.DrawLine(new Pen(Brushes.Black, 2), px1, px2);
            //绘制Y轴
            PointF py1 = new PointF(move, move);
            PointF py2 = new PointF(move, newY);

            g.DrawLine(new Pen(Brushes.Black, 2), py1, py2);
        }
        #endregion

        /// <summary>
        /// 画出Y轴上的分值线，从零开始
        /// </summary>
        /// <param name="pan"></param>
        /// <param name="maxY"></param>
        /// <param name="len"></param>
        #region   画出Y轴上的分值线，从零开始
        public static void DrawYLine(Panel pan, int maxY, int len)
        {
            float move = 30f;
            float LenX = pan.Width - 2 * move;
            float LenY = pan.Height - 2 * move;
            Graphics g = pan.CreateGraphics();
            for (int i = 0; i <= len; i++)    //len等份Y轴
            {
                PointF px1 = new PointF(move, LenY * i / len + move);
                PointF px2 = new PointF(move + 4, LenY * i / len + move);
                string sx = (maxY - maxY * i / len).ToString();
                g.DrawLine(new Pen(Brushes.Black, 2), px1, px2);
                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Far;
                drawFormat.LineAlignment = StringAlignment.Center;
                g.DrawString(sx, new Font("宋体", 8f), Brushes.Black, new PointF(move / 1.2f, LenY * i / len + move * 1.1f), drawFormat);
            }
          //  Pen pen = new Pen(Color.Black, 1);
         //   g.DrawString("Y轴", new Font("宋体 ", 10f), Brushes.Black, new PointF(move / 3, move / 2f));
        }
        #endregion

        /// <summary>
        /// 画出Y轴上的分值线，从任意值开始
        /// </summary>
        /// <param name="pan"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <param name="len"></param>
        #region   画出Y轴上的分值线，从任意值开始
        public static void DrawYLine(Panel pan, int minY, int maxY, int len, String title)
        {
            float move = 30f;
            float LenX = pan.Width - 2 * move;
            float LenY = pan.Height - 2 * move;
            Graphics g = pan.CreateGraphics();
            for (int i = 0; i <= len; i++)    //len等份Y轴
            {
                PointF px1 = new PointF(move, LenY * i / len + move);
                PointF px2 = new PointF(move + 4, LenY * i / len + move);
                string sx = (maxY - (maxY - minY) * i / len).ToString();
                g.DrawLine(new Pen(Brushes.Black, 2), px1, px2);
                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Far;
                drawFormat.LineAlignment = StringAlignment.Center;
                g.DrawString(sx, new Font("宋体", 8f), Brushes.Black, new PointF(move / 1.2f, LenY * i / len + move * 1.1f), drawFormat);
            }
           Pen pen = new Pen(Color.Black, 1);
            g.DrawString(title, new Font("宋体 ", 6f), Brushes.Black, new PointF(move / 3, move / 2f));
        }

        #endregion
        /// <summary>
        /// 画出X轴上的分值线，从零开始
        /// </summary>
        /// <param name="pan"></param>
        /// <param name="maxX"></param>
        /// <param name="len"></param>
        #region   画出X轴上的分值线，从零开始
        public static void DrawXLine(Panel pan, int maxX, int len)
        {
            float move = 30f;
            float LenX = pan.Width - 2 * move;
            float LenY = pan.Height - 2 * move;
            Graphics g = pan.CreateGraphics();
            for (int i = 1; i <= len; i++)
            {
                PointF py1 = new PointF(LenX * i / len + move, pan.Height/2 - 4);
                PointF py2 = new PointF(LenX * i / len + move, pan.Height/2);
                string sy = (maxX * i / len).ToString();
                g.DrawLine(new Pen(Brushes.Black, 2), py1, py2);
                g.DrawString(sy, new Font("宋体", 8f), Brushes.Black, new PointF(LenX * i / len + move, pan.Height/2 + move/6.1f));
            }
           // Pen pen = new Pen(Color.Black, 1);
          //  g.DrawString("X轴", new Font("宋体 ", 10f), Brushes.Black, new PointF(pan.Width - move / 1.5f, pan.Height - move / 1.5f));
        }
        #endregion

        #region   画出X轴上的分值线，从任意值开始
        /// <summary>
        /// 画出X轴上的分值线，从任意值开始
        /// </summary>
        /// <param name="pan"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="len"></param>
        public static void DrawXLine(Panel pan, int minX, int maxX, int len)
        {
            float move = 30f;
            float LenX = pan.Width - 2 * move;
            float LenY = pan.Height - 2 * move;
            Graphics g = pan.CreateGraphics();
            for (int i = 0; i <= len; i++)
            {
                PointF py1 = new PointF(LenX * i / len + move, pan.Height - move - 4);
                PointF py2 = new PointF(LenX * i / len + move, pan.Height - move);
                string sy = ((maxX - minX) * i / len + minX).ToString();
                g.DrawLine(new Pen(Brushes.Black, 2), py1, py2);
                g.DrawString(sy, new Font("宋体", 8f), Brushes.Black, new PointF(LenX * i / len + move, pan.Height - move / 1.1f));
            }
         //   Pen pen = new Pen(Color.Black, 1);
         //   g.DrawString("X轴", new Font("宋体 ", 10f), Brushes.Black, new PointF(pan.Width - move / 1.5f, pan.Height - move / 1.5f));
        }
        #endregion

       

        public static void DrawY(Panel pan, int minY, int maxY, int xlen,List<Double> points)
        {

            try
            {

                float move = 30f;
                float LenX = pan.Width - 2 * move;
                float LenY = pan.Height - 2 * move;
                float sx = maxY - minY;
                    Graphics g = pan.CreateGraphics();
                    List<PointF> llpoints = new List<PointF>();

                    if (points.Count >= 2)
                    {


                        for (int i = 0; i < points.Count - 1; i++)    //len等份Y轴
                        {
                            if (points[i] != 0.0001)
                            {
                                //Point px1 = new Point(LenX * i / xlen + move, (sx / 2 - (int)(points[i])) * LenY / sx + move);
                                //Point px2 = new Point(LenX * (i + 1) / xlen + move, (sx / 2 - (int)(points[i + 1])) * LenY / sx + move);
                                //g.DrawLine(new Pen(Brushes.Red, 2), px1, px2);

                                llpoints.Add(new PointF(LenX * i / xlen + move, (sx / 2 - (float)(points[i])) * LenY / sx + move));
                            }
                        }



                        g.DrawLines(new Pen(Brushes.Red, 2), llpoints.ToArray());


                    }
               
            }
            catch { }

        }

    }
}
