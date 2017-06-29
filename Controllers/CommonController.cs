using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EntityFramework.Extensions;
using System.Web.Security;
using System.Security.Cryptography;

namespace EnterpriseBackSystem.Controllers
{
    public class CommonController : Controller
    {


        [HttpGet]
        /// <summary>
        /// 验证码测试页
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidCode()
        {
            return View();
        }

        /// <summary>
        /// 验证码测试页提交
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidCodeSubmit(FormCollection fc)
        {
            string[] codeXArry = fc["codeX"].Split(',');
            if (codeXArry.Length<2)
            {
                return Content("验证失败，请点选大图是的汉字<a href=\"/\">点我返回</a>");
            }
            int codeX = int.Parse(codeXArry[0]);
            int codeX1 = int.Parse(codeXArry[1]);
            string[] clickXArry = Session["ClickX"].ToString().Split(',');
            int ClickX = int.Parse(clickXArry[0]);
            int ClickX1 = int.Parse(clickXArry[1]);
            if (codeX >= ClickX && codeX <= (ClickX + 25))
            {
                if (codeX1 >= ClickX1 && codeX1 <= (ClickX1 + 30))
                {
                    return Content("验证成功<a href=\"/\">点我返回</a>");
                }
            }
            return Content("验证失败<a href=\"/\">点我返回</a>");
        }

        /// <summary>
        /// 注册验证码大图
        /// </summary>
        /// <param name="industryId"></param>
        /// <returns></returns>
        public ActionResult VerificationCodeForClick()
        {
            this.CreateCheckCodeImageClick(1, GenerateCheckCodes(6, "CheckCodeRegister"));
            return View();
        }

        /// <summary>
        /// 注册验证码小图（用于显示需浏览者点击的图）
        /// </summary>
        /// <param name="industryId"></param>
        /// <returns></returns>
        public ActionResult VerificationCodeReal()
        {
            if (Session["ClickCode"] != null)
            {
                this.CreateCheckCodeImage(Session["ClickCode"].ToString(), 4);
                Response.Write(Session["ClickX"].ToString());
            }
            return View();
        }

        #region 生成验证码方法
        /// <summary>
        /// 输入汉字的验证码
        /// </summary>
        /// <param name="iCount"></param>
        /// <param name="sessionName"></param>
        /// <returns></returns>
        private string GenerateCheckCodes(int iCount, string sessionName)
        {
            int number;
            string checkCode = String.Empty;
            int iSeed = DateTime.Now.Millisecond;
            System.Random random = new Random(iSeed);
            for (int i = 0; i < iCount; i++)
            {
                number = random.Next(0, CommonModel.TempWords.tempWords.Length);
                checkCode += CommonModel.TempWords.tempWords[number];
            }

            Session[sessionName] = checkCode;
            return checkCode;
        }
        /// <summary>
        /// 生成背景为噪点噪线图的验证码
        /// </summary>
        /// <param name="checkCode"></param>
        /// <param name="noise"></param>
        private void CreateCheckCodeImage(string checkCode, int noise = 400)
        {
            if (checkCode == null || checkCode.Trim() == String.Empty)
                return;
            int iWordWidth = 30;
            int iImageWidth = checkCode.Length * iWordWidth;
            Bitmap image = new Bitmap(iImageWidth, 30);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器 
                Random random = new Random();
                //清空图片背景色 
                g.Clear(Color.White);

                //画图片的背景噪音点
                for (int i = 0; i < 20; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                //画图片的背景噪音线 
                for (int i = 0; i < 20; i++)
                {
                    int x1 = 0;
                    int x2 = image.Width;
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    if (i == 0)
                    {
                        g.DrawLine(new Pen(Color.Gray, 2), x1, y1, x2, y2);
                    }

                }


                for (int i = 0; i < checkCode.Length; i++)
                {

                    string Code = checkCode[i].ToString();
                    int xLeft = iWordWidth * (i);
                    random = new Random(xLeft);
                    int iSeed = DateTime.Now.Millisecond;
                    int iValue = random.Next(iSeed) % 4;
                    if (iValue == 0)
                    {
                        Font font = new Font("Arial", 13, (FontStyle.Bold | System.Drawing.FontStyle.Italic));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Blue, Color.Red, 1.5f, true);
                        g.DrawString(Code, font, brush, xLeft, 2);
                    }
                    else if (iValue == 1)
                    {
                        Font font = new System.Drawing.Font("楷体", 13, (FontStyle.Bold));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Blue, Color.DarkRed, 1.3f, true);
                        g.DrawString(Code, font, brush, xLeft, 2);
                    }
                    else if (iValue == 2)
                    {
                        Font font = new System.Drawing.Font("宋体", 13, (System.Drawing.FontStyle.Bold));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Green, Color.Blue, 1.2f, true);
                        g.DrawString(Code, font, brush, xLeft, 2);
                    }
                    else if (iValue == 3)
                    {
                        Font font = new System.Drawing.Font("微软雅黑", 13, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Bold));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Blue, Color.Green, 1.8f, true);
                        g.DrawString(Code, font, brush, xLeft, 2);
                    }
                }
                ////画图片的前景噪音点 
                for (int i = 0; i < noise; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线 
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                Response.ClearContent();

                Response.ContentType = "image/gif";
                Response.BinaryWrite(ms.ToArray());
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
        /// <summary>
        /// 生成背景为图片的验证码
        /// </summary>
        /// <param name="clickCount"></param>
        /// <param name="checkCode"></param>
        private void CreateCheckCodeImageClick(int clickCount, string checkCode)
        {
            if (checkCode == null || checkCode.Trim() == String.Empty)
                return;
            int iWordWidth = 48;
            int iImageWidth = checkCode.Length * iWordWidth;

            #region 定义点击几次,及点击哪个字
            Random random2 = new Random();
            int clickCountWhitch;
            string clickCode = "";
            string clickX = "";
            clickCountWhitch = random2.Next(0, (checkCode.Length - 1));
            int clickCountWhitch1;
            string clickCode1 = "";
            string clickX1 = "";
            clickCountWhitch1 = random2.Next(0, (checkCode.Length - 1));
            while (clickCountWhitch1 == clickCountWhitch)
            {
                clickCountWhitch1 = random2.Next(0, (checkCode.Length - 1));
            }
            #endregion

            //Bitmap image = new Bitmap(iImageWidth, 90);
            string[] bgImgList = {
                "/Images/background01.png",
                "/Images/background02.png"
            };
            Random random = new Random();
            //生成背景图
            Bitmap image = new Bitmap(Image.FromFile(Server.MapPath(bgImgList[random.Next(0, bgImgList.Length)])), 300, 150);

            Graphics g = Graphics.FromImage(image);
            try
            {
                #region 生成背景干扰
                //生成随机生成器 
                ////清空图片背景色 
                //g.Clear(Color.White);

                ////画图片的背景噪音点
                //for (int i = 0; i < 20; i++)
                //{
                //    int x1 = random.Next(image.Width);
                //    int x2 = random.Next(image.Width);
                //    int y1 = random.Next(image.Height);
                //    int y2 = random.Next(image.Height);
                //    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                //}

                ////画图片的背景噪音线 
                //for (int i = 0; i < 20; i++)
                //{
                //    int x1 = 0;
                //    int x2 = image.Width;
                //    int y1 = random.Next(image.Height);
                //    int y2 = random.Next(image.Height);
                //    if (i == 0)
                //    {
                //        g.DrawLine(new Pen(Color.Gray, 2), x1, y1, x2, y2);
                //    }

                //}
                #endregion

                #region 开始生成汉字
                for (int i = 0; i < checkCode.Length; i++)
                {
                    #region 记录下将要被点击的文字
                    string Code = checkCode[i].ToString();
                    int xLeft = iWordWidth * (i);
                    if (i == clickCountWhitch)
                    {
                        clickCode = Code;
                        clickX = xLeft.ToString();
                    }
                    if (i == clickCountWhitch1)
                    {
                        clickCode1 = Code;
                        clickX1 = xLeft.ToString();
                    }
                    #endregion
                    random = new Random(xLeft);
                    int iSeed = DateTime.Now.Millisecond;
                    int iValue = random.Next(iSeed) % 4;
                    if (iValue == 0)
                    {
                        Font font = new Font("Arial", 18, (FontStyle.Bold | System.Drawing.FontStyle.Italic));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Black, Color.Black, 1.5f, true);
                        Random randomY = new Random(0);
                        g.DrawString(Code, font, brush, xLeft, randomY.Next(1, 110));
                    }
                    else if (iValue == 1)
                    {
                        Font font = new System.Drawing.Font("楷体", 18, (FontStyle.Bold));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Black, Color.Black, 1.3f, true);
                        Random randomY = new Random(1);
                        g.DrawString(Code, font, brush, xLeft, randomY.Next(1, 110));
                    }
                    else if (iValue == 2)
                    {
                        Font font = new System.Drawing.Font("宋体", 18, (System.Drawing.FontStyle.Bold));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Black, Color.Black, 1.2f, true);
                        Random randomY = new Random(2);
                        g.DrawString(Code, font, brush, xLeft, randomY.Next(1, 110));
                    }
                    else if (iValue == 3)
                    {
                        Font font = new System.Drawing.Font("微软雅黑", 18, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Bold));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Black, Color.Black, 1.8f, true);
                        Random randomY = new Random(3);
                        g.DrawString(Code, font, brush, xLeft, randomY.Next(1, 110));
                    }
                }
                //拼接起将被点击的文字和座标
                Session["ClickCode"] = clickCode + clickCode1;
                Session["ClickX"] = clickX + "," + clickX1;
                #endregion

                #region 生成前景干扰
                //////画图片的前景噪音点 
                //for (int i = 0; i < 400; i++)
                //{
                //    int x = random.Next(image.Width);
                //    int y = random.Next(image.Height);
                //    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                //}
                ////画图片的边框线 
                //g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                #endregion

                #region 输出图片
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //此处注意，save的格式必须与图片格式一致
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                Response.ClearContent();
                Response.ContentType = "image/png";
                Response.BinaryWrite(ms.ToArray());
                #endregion
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
        #endregion     
    }
}
