using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PythonScript
{

    private static string translaterPath = @"";
    //python腳本資料夾
    private static string basePath = @"Assets\Python\";

    // Unity 調用 Python
    // 
    public static IEnumerator Search(
        System.Action<string> _callback, //異步回傳函式
                                         //chatGPT.getApiKey()
                                         //serpapi_Key
        params string[] argvs            //給 python 的其他參數
    )
    {

        string pyScriptPath = basePath + "Search.py";

        // 判斷是否有參數
        if (argvs != null)
        {
            // 添加參數
            foreach (string item in argvs)
            {
                pyScriptPath += " " + item;
            }
        }
        UnityEngine.Debug.Log(pyScriptPath);

        Process process = new Process();

        // ptython 的直譯器位置 python.exe
        process.StartInfo.FileName = translaterPath;

        process.StartInfo.StandardOutputEncoding = System.Text.Encoding.GetEncoding(950); //回傳正確的中文編碼
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.Arguments = pyScriptPath;     // 路徑
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;        // 不顯示執行窗口

        string answer = "";

        // 開始執行，獲取執行輸出，添加結果輸出委託
        process.Start();
        process.BeginOutputReadLine();
        process.OutputDataReceived += new DataReceivedEventHandler(GetData);

        //等待python回傳資料
        while(string.IsNullOrEmpty(answer) == true)
        {
            UnityEngine.Debug.Log("Wait...");
            yield return new WaitForSeconds(1f);//停止1秒
        }

        _callback(answer);

        // 結果輸出委託
        void GetData(object sender, DataReceivedEventArgs e)
        {
            //輸出不為空
            if (string.IsNullOrEmpty(e.Data) == false)
            {
                //UnityEngine.Debug.Log(e.Data);
                answer = e.Data;
            }
        }

        yield return null;
    }
}