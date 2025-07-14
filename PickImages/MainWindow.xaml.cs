using Microsoft.Win32;
using SegmentationTest;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;
namespace PickImages;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Console2Textbox c2t;
    public MainWindow()
    {
        InitializeComponent();
        c2t = new Console2Textbox(myConsole);
    }
    private void AsyncWrite(TextBox box, string text) {
        void Write(System.Windows.Controls.TextBox box, string text) {
            box.AppendText(text);
            box.ScrollToEnd();
        }
        Action<TextBox, String> updateAction = new Action<TextBox, string>(Write);
        box.Dispatcher.BeginInvoke(updateAction, box, text);
    }

    private async void BrowseFileButton1_Click(object sender, RoutedEventArgs e) {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "文本文件 (*.txt)|*.txt";

        if (openFileDialog.ShowDialog() == true) {
            OCRFile1.Text = openFileDialog.FileName;
            AsyncWrite(c2t.box,$"您选择了{OCRFile1.Text}\n");
            string a = OCRFile1.Text;
            await Task.Run(() => {
                // 这里执行长时间运行的操作
                //print();
                try {
                    // 可能会抛出异常的代码
                    pickShortImage(a);
                }
                catch (Exception ex) {
                    // 显示异常信息

                    AsyncWrite(c2t.box, $"{ex.ToString()}");
                    return;
                }
            });
        }
    }
    private async void BrowseFileButton2_Click(object sender, RoutedEventArgs e) {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "文本文件 (*.txt)|*.txt";

        if (openFileDialog.ShowDialog() == true) {
            OCRFile2.Text = openFileDialog.FileName;
            AsyncWrite(c2t.box,$"您选择了{OCRFile2.Text}\n");
            string a = OCRFile2.Text;
            await Task.Run(() => {
                // 这里执行长时间运行的操作
                //print();
                try {
                    // 可能会抛出异常的代码
                    pickShortImage(a);
                }
                catch (Exception ex) {
                    // 显示异常信息

                    AsyncWrite(c2t.box, $"{ex.ToString()}");
                    return;
                }
            });
            
        }
    }

    private void pickShortImage(string txtFile) {
        string finalFolder = Path.GetDirectoryName(txtFile);
        string name = txtFile.Split("\\")[^1].Split(".")[0];
        string finalPath = Path.Join(finalFolder, name);
        if (!Directory.Exists(finalPath))
            Directory.CreateDirectory(finalPath);
        string[] filenames = File.ReadAllLines(txtFile);
        string[] orientations = ["SX-CS-Z", "XX-CS-Z"];
        string path = @"D:\Monitor";
        foreach (string filename in filenames) {
            int hour = int.Parse(filename.Split("-")[^4]);
            string amOrPm = "AM";
            if (hour >= 13 && hour <= 24)
                amOrPm = "PM";
            //2025-xx-xx
            
            string yearMonthDay = filename[..10];
            string timestamp = $"{yearMonthDay}-{amOrPm}";
            foreach (string orientation in orientations) {
                string targetPath = Path.Join(path, orientation, timestamp);
                AsyncWrite(c2t.box,targetPath);
                string[] imgs;
                try {
                    imgs = Directory.GetFiles(targetPath);
                }
                catch (Exception) {
                    imgs = [];
                }
                int idx = -1;

                for (int i = 0; i < imgs.Length; i++) {
                    if (imgs[i].Split("\\")[^1].CompareTo(filename) >= 0) {
                        idx = i;
                        AsyncWrite(c2t.box, $"找到 {filename} 在 {imgs[i]} index={idx}\n");
                        break;
                    }
                }
                if (idx == -1) {
                    AsyncWrite(c2t.box, $"{filename}的短图不存在或已过期，已跳过\n");
  
                    continue;
                }
                int index = new List<string>(imgs).IndexOf(name);
                if (idx >= 0) {
                    int start, end;
                    start = idx;
                    end = Math.Min(idx + 8, imgs.Length - 1);
                    for (int i = start; i <= end; i++) {
                        string temp = imgs[i].Split("\\")[^1];
                        string dest = Path.Join(finalPath, temp);
                        File.Copy(imgs[i], dest, true);
                        AsyncWrite(c2t.box, $"copy {imgs[i]} -> {dest}\u2713\n");
                    }
                }

            }
        }
        Process.Start("explorer.exe", finalPath);
    }

    
}