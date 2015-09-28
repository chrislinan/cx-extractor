using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace cx_extractor
{
    class TextExtract
    {
        private static string[] lines;
        private static int blocksWidth = 3;
        /* 当待抽取的网页正文中遇到成块的新闻标题未剔除时，只要增大此阈值即可。*/
        /* 阈值增大，准确率提升，召回率下降；值变小，噪声会大，但可以保证抽到只有一句话的正文 */
        private static int threshold = 86;
        private static String html;
        private static Boolean flag = false;
        private static int start;
        private static int end;
        private static StringBuilder text = new StringBuilder();
        private static List<int> indexDistribution = new List<int>();
  

       

        /// <summary>
        /// 抽取网页正文，不判断该网页是否是目录型。即已知传入的肯定是可以抽取正文的主题类网页。
        /// </summary>
        /// <param name="_html">网页HTML字符串</param>
        /// <returns>网页正文string</returns>
        public static String parse(String _html)
        {
            return parse(_html, false);
        }
        /// <summary>
        /// 判断传入HTML，若是主题类网页，则抽取正文；否则输出"unkown"。
        /// </summary>
        /// <param name="_html">网页HTML字符串</param>
        /// <param name="_flag">true进行主题类判断, 省略此参数则默认为false</param>
        /// <returns>网页正文string</returns>
        public static String parse(String _html, Boolean _flag)
        {
            flag = _flag;
            html = _html;
            preProcess();
            return getText();
        }

        private static void preProcess()
        {
            html = Regex.Replace(html,"(?is)<!DOCTYPE.*?>","");
            html = Regex.Replace(html,"(?is)<!--.*?-->", "");				// remove html comment
            html = Regex.Replace(html,"(?is)<script.*?>.*?</script>", ""); // remove javascript
            html = Regex.Replace(html,"(?is)<style.*?>.*?</style>", "");   // remove css
            html = Regex.Replace(html,"&.{2,5};|&#.{2,5};", "");			// remove special char
            html = Regex.Replace(html, "(?is)<.*?>", "");
            html = html.Replace("\r", "");
            html = html.Replace("\t", "");
            html = html.Replace(" ","");
        }

        private static String getText()
        {
            lines = html.Split('\n');
            indexDistribution.Clear();

            for (int i = 0; i < lines.Length - blocksWidth; i++)
            {
                int wordsNum = 0;
                for (int j = i; j < i + blocksWidth; j++)
                {
                    lines[j] = lines[j].Replace("\\s","");
                    wordsNum += lines[j].Length;
                }
                indexDistribution.Add(wordsNum);
            }

            start = -1; end = -1;
            Boolean boolstart = false, boolend = false;
            text.Length = 0;

            for (int i = 0; i < indexDistribution.Count - 1; i++)
            {
                if (indexDistribution[i] > threshold && !boolstart)
                {
                    if (indexDistribution[i + 1] != 0
                        || indexDistribution[i + 2] != 0
                        || indexDistribution[i + 3] != 0)
                    {
                        boolstart = true;
                        start = i;
                        continue;
                    }
                }
                if (boolstart)
                {
                    if (indexDistribution[i] == 0
                        || indexDistribution[i + 1] == 0)
                    {
                        end = i;
                        boolend = true;
                    }
                }
                StringBuilder tmp = new StringBuilder();
                if (boolend)
                {
                    for (int ii = start; ii <= end; ii++)
                    {
                        if (lines[ii].Length < 5)
                            continue;
                        tmp.Append(lines[ii] + "\n");
                    }
                    String str = tmp.ToString();
                    if (str.Contains("Copyright") || str.Contains("版权所有")) continue;
                    text.Append(str);
                    boolstart = boolend = false;
                }
            }
            return text.ToString();
        }


        public static void setthreshold(int value)
        {
            threshold = value;
        }
       
        
    }
}
