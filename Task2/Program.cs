using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Task2
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string[]> reader = CSVReader();

            ServerCheck(reader);
        }

        static List<string[]> CSVReader()
        {
            // ファイル名
            string fileName = "ServerLog.csv";

            StreamReader sr = new StreamReader(fileName);

            List<string[]> lists = new List<string[]>();
            string[] firstSt = new string[3];

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                firstSt = line.Split(',');
                lists.Add(firstSt);
            }

            return lists;
        }

        static void ServerCheck(List<string[]> sList)
        {
            List<int> timeOutCount = new List<int>(); // タイムアウトしたサーバーがどのサーバーが保存しておく
            List<string> temSaveErrorTime = new List<string>(); // タイムアウトした時間を保存しておく
            List<string> temSaveErrorAddres = new List<string>(); // タイムアウトした時間を保存しておく
            List<string> errorStartTime = new List<string>(); // 故障した時間を保存しておくリスト
            List<string> errorServerAddres = new List<string>();// 故障したサーバーアドレスを保存しておくリスト                                    

            List<string[]> reader = sList;
            int N = int.Parse(reader[0][0]);

            int listIndex = 1;

            while (listIndex < reader.Count)
            {
                string[] s = reader[listIndex];
                listIndex++;
                if (s == null || s[0] == "") continue;

                string time = s[0];                                  // 時間を保存する
                string[] stringAddress = s[1].Trim().Split('.', '/'); // アドレスを保存する
                string milliSecond = s[2];                           // pingの応答時間を保存する

                string w = ""; // アドレスに関する作業を行う

                for (int i = 0; i < stringAddress.Length - 1; i++)
                {
                    int address = int.Parse(stringAddress[i]);
                    string con = Convert.ToString(address, 2).PadLeft(8, '0');
                    w += con;
                }

                string serverAddress = w.Substring(0, int.Parse(stringAddress[4]));

                int findAddress = temSaveErrorAddres.IndexOf(serverAddress);
                // タイムアウトしたか（末尾に"-"があるか）
                if (milliSecond == "-")
                {
                    if (findAddress < 0) // 初めてのタイムアウトか
                    {
                        temSaveErrorTime.Add(time);
                        temSaveErrorAddres.Add(serverAddress);
                        timeOutCount.Add(1);
                    }
                    else // 連続してタイムアウトしているか
                    {
                        timeOutCount[findAddress] += 1;
                        if (timeOutCount[findAddress] >= N)
                        {
                            errorStartTime.Add(temSaveErrorTime[findAddress]);
                            errorServerAddres.Add(temSaveErrorAddres[findAddress]);

                            int index = -1;

                            // 過去にタイムアウトしたサーバーと同じ場合は
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

                                int startYear = int.Parse(startTime.Substring(0, 4));
                                int startMonth = int.Parse(startTime.Substring(4, 2));
                                int startDay = int.Parse(startTime.Substring(6, 2));
                                int startHour = int.Parse(startTime.Substring(8, 2));
                                int startMinute = int.Parse(startTime.Substring(10, 2));
                                int startSecond = int.Parse(startTime.Substring(12, 2));

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

                                string removeTarget = errorServerAddres[index];
                                string removeTarget2 = errorStartTime[index];
                                errorServerAddres.RemoveAll(list => list == removeTarget);
                                errorStartTime.RemoveAll(list => list == removeTarget2);

                                Console.WriteLine($"サーバーアドレス{s[1]}のサーバーは{startYear}年{startMonth}月{startDay}日{startHour}時間{startMinute}分{startSecond}秒から{endYear}年{endMonth}月{endDay}日{endHour}時間{endMinute}分{endSecond}秒までの{year}年{month}月{day}日{hour}時間{minute}分{second}秒の間故障していました");
                            }
                        }
                    }
                }

                if (milliSecond == "-") continue;

                if (findAddress != -1)
                {
                    timeOutCount.RemoveAt(findAddress);
                    string removeTimeTarget = temSaveErrorTime[findAddress];
                    string removeAddressTarger = temSaveErrorAddres[findAddress];
                    temSaveErrorTime.RemoveAll(list => list == removeTimeTarget);
                    temSaveErrorAddres.RemoveAll(list => list == removeAddressTarger);
                }

                // タイムアウトをしておらず、過去にタイムアウトしているサーバーがあるか
                if (timeOutCount.Count > 0)
                {
                    if (findAddress != -1)
                    {
                        string removeTarget = temSaveErrorTime[findAddress];
                        string removeTarget2 = temSaveErrorAddres[findAddress];
                        temSaveErrorTime.RemoveAll(list => list == removeTarget);
                        temSaveErrorAddres.RemoveAll(list => list == removeTarget2);
                        timeOutCount.RemoveAt(findAddress);
                    }
                }
            }
        }
    }
}
