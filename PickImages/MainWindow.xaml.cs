using Microsoft.Win32;
using SegmentationTest;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Path = System.IO.Path;
namespace PickImages;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Console2Textbox c2t;
    private const string sourcePath = @"D:\FTP2";
    private const string ocrTxt1 = "OCR识别错.txt", ocrTxt2 = "OCR少框.txt";
    private const string detectCSFolder = "车身误检测", detectZXFolder = "走行误检测";
    private DispatcherTimer _inactivityTimer;
    public MainWindow()
    {
        InitializeComponent();
        c2t = new Console2Textbox(myConsole);
        StartInactivityTimer(seconds:120);
        CheckIfExists();
    }
    private void AsyncWrite(TextBox box, string text) {
        void Write(System.Windows.Controls.TextBox box, string text) {
            box.AppendText(text);
            box.ScrollToEnd();
        }
        Action<TextBox, string> updateAction = new Action<TextBox, string>(Write);
        box.Dispatcher.BeginInvoke(updateAction, box, text);
    }

    private async void OCRButton1_Click(object sender, RoutedEventArgs e) {
        if (OCRFile1.Text.Contains($"选择{ocrTxt1}")) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件 (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true) {
                OCRFile1.Text = openFileDialog.FileName;
                AsyncWrite(c2t.box, $"目标文件 -> {OCRFile1.Text}\n");
            }
        }
        string txtFile = OCRFile1.Text;
        string finalFolder = Path.GetDirectoryName(txtFile);
        string name = txtFile.Split("\\")[^1].Split(".")[0];
        string ocrPath = Path.Join(finalFolder, name);

        if (!Directory.Exists(ocrPath))
            Directory.CreateDirectory(ocrPath);
        if (OCRFile1.Text.Contains($"选择{ocrTxt1}")) {
            Console.WriteLine("您未选择任何文件，必须选择一个文件");
            return; 
        }
        //if (OCRFile1.Text.ToUpper().Contains($"OCR")) {
        //    Console.WriteLine("您必须选择OCRXXX.txt");
        //    return;
        //}
        string[] ocrFiles = File.ReadAllLines(txtFile);
        OCRButton1.IsEnabled = false;
        await Task.Run(() => {
            // 这里执行长时间运行的操作
            //print();
            try {
                // 可能会抛出异常的代码
                PickLongImage(filenames: ocrFiles, finalPath: ocrPath);
            }
            catch (Exception ex) {
                // 显示异常信息

                AsyncWrite(c2t.box, $"{ex.ToString()}");
                return;
            }
        });
    }
    private async void OCRButton2_Click(object sender, RoutedEventArgs e) {

        
        if (OCRFile2.Text.Contains($"选择{ocrTxt2}")) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件 (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true) {
                OCRFile2.Text = openFileDialog.FileName;
                AsyncWrite(c2t.box, $"目标文件 -> {OCRFile2.Text}\n");
            }
        } 
        
        string txtFile = OCRFile2.Text;
        string finalFolder = Path.GetDirectoryName(txtFile);
        string name = txtFile.Split("\\")[^1].Split(".")[0];
        string ocrPath = Path.Join(finalFolder, name);

        if (!Directory.Exists(ocrPath))
            Directory.CreateDirectory(ocrPath);
        if (OCRFile2.Text.Contains($"选择{ocrTxt2}")) {
            Console.WriteLine("您未选择任何文件，必须选择一个文件");
            return;
        }
        //if (OCRFile2.Text.ToUpper().Contains($"OCR")) {
        //    Console.WriteLine("您必须选择OCRXXX.txt");
        //    return;
        //}
        string[] ocrFiles = File.ReadAllLines(txtFile);
        OCRButton2.IsEnabled = false;
        await Task.Run(() => {
            // 这里执行长时间运行的操作
            //print();
            try {
                // 可能会抛出异常的代码
                PickLongImage(filenames: ocrFiles, finalPath: ocrPath);
            }
            catch (Exception ex) {
                // 显示异常信息

                AsyncWrite(c2t.box, $"{ex.ToString()}");
                return;
            }
        });
    }
    private async void DetectCSButton_Click(object sender, RoutedEventArgs e) {
        if (DetectCSFolder.Text.Contains($"选择{detectCSFolder}")) {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            if (openFolderDialog.ShowDialog() == true) {
                DetectCSFolder.Text = openFolderDialog.FolderName;
                AsyncWrite(c2t.box, $"目标文件夹 -> {DetectCSFolder.Text}\n");
            }
        }

        string path = DetectCSFolder.Text; // 获取路径
        string[] filenames = Directory.GetFiles(path, searchPattern: "*.jpg");
        string detectPath = Path.Join(path, "原图");
        if (!Directory.Exists(detectPath)) 
            Directory.CreateDirectory(detectPath);
        DetectCSButton.IsEnabled = false;
        await Task.Run(() => {
            // 这里执行长时间运行的操作
            //print();
            try {
                // 可能会抛出异常的代码
                PickLongImage(filenames: filenames, finalPath: detectPath);
                //PickLongImage(filenames: filenames, finalPath: detectPath, trainType:"cs-y");
            }
            catch (Exception ex) {
                // 显示异常信息

                AsyncWrite(c2t.box, $"{ex.ToString()}");
                return;
            }
        });

    }
    private async void DetectZXButton_Click(object sender, RoutedEventArgs e) {
        if (DetectZXFolder.Text.Contains($"选择{detectZXFolder}")) {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            if (openFolderDialog.ShowDialog() == true) {
                DetectZXFolder.Text = openFolderDialog.FolderName;
                AsyncWrite(c2t.box, $"目标文件夹 -> {DetectZXFolder.Text}\n");
            }
        }

        string path = DetectZXFolder.Text; // 获取路径
        string[] filenames = Directory.GetFiles(path, searchPattern: "*.jpg");
        string detectPath = Path.Join(path, "原图");
        if (!Directory.Exists(detectPath))
            Directory.CreateDirectory(detectPath);
        DetectZXButton.IsEnabled = false;
        await Task.Run(() => {
            // 这里执行长时间运行的操作
            //print();
            try {
                // 可能会抛出异常的代码
                PickLongImage(filenames: filenames, finalPath: detectPath, trainType:"zx-z");
                //PickLongImage(filenames: filenames, finalPath: detectPath, trainType: "zx-y");
            }
            catch (Exception ex) {
                // 显示异常信息

                AsyncWrite(c2t.box, $"{ex.ToString()}");
                return;
            }
        });
    }
    private void CheckIfExists() {
        string destopPath;
        
        try {
            destopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
        catch (Exception ex) {
            string rawUser = "57292";
            destopPath = $@"C:\Users\{rawUser}";
            Console.WriteLine($"用户名更改过，无法获取桌面路径, 修改为{destopPath}");
        }
        foreach (string destopFile in Directory.GetFiles(destopPath, searchPattern: "*.txt")) {
            if (destopFile.Split("\\")[^1].CompareTo(ocrTxt1) == 0) {
                OCRFile1.Text = destopFile;
                OCRButton1.Content = "寻找原图";
            }
            if (destopFile.Split("\\")[^1].CompareTo(ocrTxt2) == 0) {
                OCRFile2.Text = destopFile;
                OCRButton2.Content = "寻找原图";
            }
            
        }
        foreach (string destopFile in Directory.GetDirectories(destopPath)) {
            if (destopFile.Split("\\")[^1].CompareTo(detectCSFolder) == 0) {
                DetectCSFolder.Text = destopFile;
                DetectCSButton.Content = "寻找原图";
            }
            if (destopFile.Split("\\")[^1].CompareTo(detectZXFolder) == 0) {
                DetectZXFolder.Text = destopFile;
                DetectZXButton.Content = "寻找原图";
            }
        }

    }

    private void PickLongImage(string[] filenames, string finalPath, string trainType="cs-z") {
        AsyncWrite(c2t.box, $"===========寻找类型：{trainType.ToUpper()}===========\n");
        foreach (string f in filenames) {
            string filename = f.Split("/")[^1]; // ubuntu路径处理为文件名
            bool flag = false;
            
            string yearMonthDay = filename.Split("\\")[^1][..10];
            string yearMonthDayPath = Path.Join(sourcePath, yearMonthDay);
            string hour = filename.Split("-")[3];
            if (!Directory.Exists(yearMonthDayPath)) {
                AsyncWrite(c2t.box, $"✗ {filename}的原图已过期或不存在\n");
                continue;
            }
            foreach (string timestamp in Directory.GetDirectories(yearMonthDayPath)) {
                // xxxx-hour-minute-second -> hour
                if (timestamp.Split("\\")[^1].Contains(hour)) {
                    foreach (string orientation in Directory.GetDirectories(timestamp)) {
                        if (!orientation.Split("\\")[^1].Contains(trainType))
                            continue;
                        foreach (string jpg in Directory.GetFiles(orientation)) {
                            string jpgName = jpg.Split("\\")[^1];
                    
                            if (jpgName.CompareTo(filename.Split("\\")[^1]) == 0) {
                                File.Copy(jpg, Path.Join(finalPath, jpgName), true);
                                
                                flag = true;
                                break;
                            }
                        }
                    }


                }
            }
            if (flag)
                AsyncWrite(c2t.box, $"✓ {filename}\n");
            else
                AsyncWrite(c2t.box, $"✗ {filename}的原图已过期或不存在\n");


        }
        Process.Start("explorer.exe", finalPath);
    }
    private void StartInactivityTimer(int seconds) {
        _inactivityTimer = new DispatcherTimer {
            Interval = TimeSpan.FromSeconds(seconds) // 15秒无操作后触发
        };
        _inactivityTimer.Tick += (s, e) => CloseApplication();
        _inactivityTimer.Start();

        // 监听所有可能的用户输入事件
        PreviewMouseMove += ResetTimerOnActivity;
        PreviewKeyDown += ResetTimerOnActivity;
        PreviewTouchDown += ResetTimerOnActivity;
    }

    // 用户有操作时重置计时器
    private void ResetTimerOnActivity(object sender, EventArgs e) {
        _inactivityTimer.Stop();
        _inactivityTimer.Start();
    }

    // 关闭应用程序
    private void CloseApplication() {
        _inactivityTimer.Stop();
        Application.Current.Shutdown();
    }


}