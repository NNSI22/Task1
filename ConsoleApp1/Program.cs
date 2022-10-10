using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {            
            ServerCheck();
        }
    
        static void ServerCheck()
        {
            List<string> errorStartTime = new List<string>(); // タイムアウトした時間を保存しておくリスト
            List<string> errorServerAddres = new List<string>();// タイムアウトしたサーバーアドレスを保存しておくリスト            

            while (true)
            {
                string[] s = Console.ReadLine().Trim().Split(',');   // 入力された文字列を受け取る

                if (s == null || s[0] == "") continue;

                string time = s[0];                                  // 時間を保存する
                string[] stringAddress = s[1].Trim().Split('.','/'); // アドレスを保存する
                string milliSecond = s[2];                           // pingの応答時間を保存する

                string w = ""; // アドレスに関する作業を行う

                for(int i = 0; i < stringAddress.Length - 1; i++)
                {
                    int address = int.Parse(stringAddress[i]);
                    string con = Convert.ToString(address, 2).PadLeft(8,'0');                    
                    w += con;
                }

                string serverAddress = w.Substring(0, int.Parse(stringAddress[4]));

                // タイムアウトしたか（末尾に"-"があるか）
                if (milliSecond == "-")
                {
                    
                    errorStartTime.Add(time);
                    errorServerAddres.Add(serverAddress);
                }

                // 入力されたサーバーがタイムアウトしておらず、過去にタイムアウトしたサーバーがあった場合は
                if (milliSecond != "-" && errorServerAddres != null)
                {
                    int index = -1;

                    // タイムアウトしていないサーバーが、過去にタイムアウトしたサーバーと同じ場合は
                    for (int i = 0; i < errorServerAddres.Count; i++)
                    {
                        if (errorServerAddres[i] == serverAddress)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        string startTime = errorStartTime[index];
                        string endTime = time;                        

                        int startYear = int.Parse(startTime.Substring(0,4));
                        int startMonth = int.Parse(startTime.Substring(4,2));
                        int startDay = int.Parse(startTime.Substring(6,2));
                        int startHour = int.Parse(startTime.Substring(8,2));
                        int startMinute = int.Parse(startTime.Substring(10,2));
                        int startSecond = int.Parse(startTime.Substring(12,2));                        
                        
                        int endYear = int.Parse(endTime.Substring(0, 4));
                        int endMonth = int.Parse(endTime.Substring(4, 2));
                        int endDay = int.Parse(endTime.Substring(6, 2));
                        int endHour = int.Parse(endTime.Substring(8, 2));
                        int endMinute = int.Parse(endTime.Substring(10, 2));
                        int endSecond = int.Parse(endTime.Substring(12, 2));

                        int year = Math.Abs(endYear - startYear);
                        int month = Math.Abs(endMonth - startMonth);
                        int day = Math.Abs(endDay - startDay);
                        int hour = Math.Abs(endHour - startHour);
                        int minute = Math.Abs(endMinute - startMinute);
                        int second = Math.Abs(endSecond - startSecond);

                        string removeAddressTarger = errorServerAddres[index];
                        string removeTimeTarget = errorStartTime[index];
                        errorServerAddres.RemoveAll(list => list == removeAddressTarger);
                        errorStartTime.RemoveAll(list => list == removeTimeTarget);

                        Console.WriteLine($"サーバーアドレス{s[1]}のサーバーは{year}年{month}月{day}日{hour}時間{minute}分{second}秒の間故障していました");
                    }
                }
            }
        }
    }
}
