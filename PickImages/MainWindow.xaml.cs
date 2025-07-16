using Microsoft.Win32;
using SegmentationTest;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.Printing;
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
    private string presentStationPingYing;
    private const string detectCSFolder = "车身误检测", detectZXFolder = "走行误检测";
    private DispatcherTimer _inactivityTimer;
    private string[] stations = [
        "伍明",
        "凤台",
        "包庄",
        "大许",
        "宁波",
        "建国",
        "新塘边",
        "杨集",
        "杭州",
        "枫泾",
        "泗安",
        "淮北北",
        "湾沚",
        "炮车",
        "虞城",
        "西寺坡",
        "誓节渡",
        "李庄",
        "姚李庙",
        "杨楼",
        "梓树庄",
        "烔炀河",
        "东孝",
        "白龙桥"
    ];
    private string[] stationsPingYing = [
        "wm",
        "ft",
        "bz",
        "dx",
        "nb",
        "jg",
        "xtb",
        "yj",
        "hz",
        "fj",
        "sa",
        "hbb",
        "wz",
        "pc",
        "yc",
        "xsp",
        "sjd",
        "lz",
        "ylm",
        "yl",
        "zsz",
        "tyh"
    ];
    public MainWindow()
    {
        InitializeComponent();
        c2t = new Console2Textbox(myConsole);
        StartInactivityTimer(seconds:120);
        CheckIfExists();
    }
    private void CheckIfExists() {
        string destopPath;
        string pingYing = "";
        string yesterday = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
        yesterday = $"{yesterday:yyyy-MM-dd}";
        for (int i = 0; i < stations.Length; i++) {
            string path = @$"D:\{stations[i]}";
            if (Directory.Exists(path)) {
                pingYing = stationsPingYing[i];
                break;
            }
        }

            try {
                destopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            catch (Exception ex) {
                string rawUser = "57292";
                destopPath = $@"C:\Users\{rawUser}";
                Console.WriteLine($"用户名更改过，无法获取桌面路径, 修改为{destopPath}");
            }
        foreach (string destopFile in Directory.GetFiles(destopPath, searchPattern: "*.csv")) {
            if (destopFile.Split("\\")[^1].CompareTo($"{pingYing}_{yesterday}_OCR.csv") == 0) {
                OCRFile1.Text = destopFile;
                OCRButton1.Content = "寻找原图";
                break;
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
    private void AsyncWrite(TextBox box, string text) {
        void Write(System.Windows.Controls.TextBox box, string text) {
            box.AppendText(text);
            box.ScrollToEnd();
        }
        Action<TextBox, string> updateAction = new Action<TextBox, string>(Write);
        box.Dispatcher.BeginInvoke(updateAction, box, text);
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
    private async void OCRButton1_Click(object sender, RoutedEventArgs e) {
        if (OCRFile1.Text.Contains($"选择站点名_日期XXX.csv")) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "逗号分隔文件 (*.csv)|*.csv";
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
        if (OCRFile1.Text.Contains($"选择站点名_日期XXX.csv")) {
            Console.WriteLine("您未选择任何文件，必须选择一个文件");
            return; 
        }
        string[] ocrFiles = File.ReadAllLines(txtFile)[1..];
        OCRButton1.IsEnabled = false;
        await Task.Run(() => {
            // 这里执行长时间运行的操作
            //print();
            try {
                // 可能会抛出异常的代码
                OCRPickLongImage(csvRows: ocrFiles, finalPath: ocrPath);
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

    private void OCRPickLongImage(string[] csvRows, string finalPath, string trainType="cs-z") {
        AsyncWrite(c2t.box, $"===========寻找类型：{trainType.ToUpper()}===========\n");
        foreach (string row in csvRows) {
            string[] cols = row.Replace("\"", "").Split(',');
            string filename = cols[^8].Split("/")[^1];
            string number1 = cols[^3].Replace("-", "").Replace("F", "");
            string number2 = cols[^2];
            string newName = $"{number1}_{number2}.jpg";
            //filename 获取文件名
            //获取车号1-车号2
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
                                File.Copy(jpg, Path.Join(finalPath, newName), true);
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


}