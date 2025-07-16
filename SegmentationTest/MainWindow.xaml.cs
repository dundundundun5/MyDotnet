
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Clipboard = System.Windows.Clipboard;
using ComboBox = System.Windows.Controls.ComboBox;
using Path = System.IO.Path;
using TextBox = System.Windows.Controls.TextBox;
namespace SegmentationTest;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    /// <summary>
    /// 图片的根路径
    /// </summary>
    private const string absolutePath = @"D:\";
    /// <summary>
    /// 切割长图的源路径
    /// </summary>
    private const string longImageFolder = "FTP2";
    /// <summary>
    /// 短图的源路径
    /// </summary>
    private const string shortImageFolder = "Monitor";
    /// <summary>
    /// 车身左汇总的目标路径
    /// </summary>
    private const string gatheredImageFolder = "long";
    /// <summary>
    /// 所有车头汇总的目标路径
    /// </summary>
    private const string onlyLocomotiveFolder = "locomotive";
    /// <summary>
    /// 顿顿头像的文件名
    /// </summary>
    private const string avatarJpgName = "avatar.jpg";
    /// <summary>
    /// 硬件错误、软件错误的长图归纳文件夹
    /// </summary>
    private const string longHardware = "long_hardware", longSoftware = "long_software";
    /// <summary>
    /// 硬件错误、软件错误的短图归纳文件夹
    /// </summary>
    private const string shortHardware = "short_hardware", shortSoftware = "short_software";
    /// <summary>
    /// FTP上传远程路径
    /// </summary>
    private const string ftpRemotePath = "/个人文件夹/张灵顿/segment_test";
    /// <summary>
    /// 头像文件的修改时间的延后分钟数
    /// </summary>
    private const int avatarTimeGap = 2;
    /// <summary>
    /// 将Console输出重定向到文本框实例
    /// </summary>
    private Console2Textbox c2t;
    /// <summary>
    /// 无操作计时器
    /// </summary>
    private DispatcherTimer _inactivityTimer;
    /// <summary>
    /// 写死的站点名列表
    /// </summary>
    public List<string> StationList { get; } = new List<string>() {
        "伍明", "凤台", "包庄", "大许", "宁波", "建国", "新塘边", "杨集", "杭州", "枫泾", "泗安", "淮北北", "湾沚", "炮车", "虞城", "西寺坡", "誓节渡","李庄","姚李庙","杨楼","梓树庄","烔炀河","东孝","白龙桥"
    };
    /// <summary>
    /// 当前站点的中文名
    /// </summary>
    public string presentStation { get; set; } = new string("null");
    /// <summary>
    /// 当前的本地时间
    /// </summary>
    public DateTime CurrentDate { get; private set; }
    /// <summary>
    /// 昨天的本地实际
    /// </summary>
    public DateTime YesterdayDate { get; private set; }
    /// <summary>
    /// 主窗口构造函数
    /// </summary>
    public MainWindow() {

        InitializeComponent();
        c2t = new Console2Textbox(myConsole);
        CheckPresentStation();
        InitializeDates();
        StartInactivityTimer(180);

    }
    /// <summary>
    /// 在线程内输出文本到主窗口的文本框
    /// </summary>
    /// <param name="box">TextBox类实例</param>
    /// <param name="text">要输出的文本</param>
    private void AsyncWrite(TextBox box, string text) {
        void Write(System.Windows.Controls.TextBox box, string text) {
            box.AppendText(text);
            box.ScrollToEnd();
        }
        Action<TextBox, String> updateAction = new Action<TextBox, string>(Write);
        box.Dispatcher.BeginInvoke(updateAction, box, text);
    }
    /// <summary>
    /// 查询是否存在D:\站点名的文件夹，有则锁定选项
    /// </summary>
    private void CheckPresentStation() {
        for (int i = 0; i < StationList.Count(); i++) {
            if (Directory.Exists(Path.Join(absolutePath, StationList[i]))) {
                stations.SelectedIndex = i;
                stations.IsEnabled = false;
                break;
            }
        }
    }
    /// <summary>
    /// 汇总昨日所有车身左的长图到./long
    /// </summary>
    private void GatherLongImages() {

        string resultPath = Path.Join(absolutePath, presentStation, gatheredImageFolder);
        if (Directory.Exists(resultPath))
            Directory.Delete(resultPath, true);
        Directory.CreateDirectory(resultPath);

        string yesterdayPath = Path.Join(absolutePath, longImageFolder, $"{YesterdayDate:yyyy-MM-dd}");
        //string testPath = new string(@"D:\xmind2025");
        int total = 0;
        foreach (var timestampDirectory in Directory.GetDirectories(yesterdayPath)) {
            string cur = "null";
            DateTime curDate = DateTime.MinValue;
            foreach (var trainTypeDirectory in Directory.GetDirectories(timestampDirectory)) {
                if (trainTypeDirectory.Contains("cs-z")) {
                    foreach (var imageFile in Directory.GetFiles(trainTypeDirectory, "*.jpg")) {

                        string newFileName = Dundun.FileName4Train(imageFile);
                        cur = newFileName;
                        curDate = File.GetLastWriteTime(imageFile);
                        string destinationPath = Path.Join(resultPath, newFileName);
                        File.Copy(imageFile, destinationPath, true);

                        //AsyncWrite(c2t.box, $"{newFileName} -> {resultPath} \u2713\n");
                    }
                }
            }
            total += 1;
            if (cur == "null")
                continue;
            string[] t = cur.Split("+");
            string ms = t[^1].Split(".")[0].Split('-')[^1];
            string newMs = (int.Parse(ms) + 1).ToString();

            string newName = $"{t[0]}+{t[1]}+{t[^1][..^7]}{newMs}.jpg";
            string avatarTargetPath = Path.Join(resultPath, newName);
            string avatarSourcePath = Path.Join(absolutePath, presentStation, avatarJpgName);

            File.Copy(avatarSourcePath, avatarTargetPath, true);
            File.SetLastWriteTime(avatarTargetPath, curDate.AddSeconds(avatarTimeGap));
            AsyncWrite(c2t.box, $"{avatarSourcePath} -> {avatarTargetPath} \u2713\n");
        }
        AsyncWrite(c2t.box, $"total={total}，长图收集完成，存储在 {resultPath}\n");
        //AsyncWrite(c2t.box, $"如果站点选择错误，则将{resultPath}的{resultPath.Split('\\')[1]}手动改成实际站点名称\n");
        Process.Start("explorer.exe", resultPath);


    }
    /// <summary>
    /// 汇总所有相机的车头长图到./locomotive
    /// </summary>
    private void GatherLongImageOnlyLocomitive() {
        string resultPath = Path.Join(absolutePath, presentStation, onlyLocomotiveFolder);
        if (Directory.Exists(resultPath))
            Directory.Delete(resultPath, true);
        Directory.CreateDirectory(resultPath);

        string yesterdayPath = Path.Join(absolutePath, longImageFolder, $"{YesterdayDate:yyyy-MM-dd}");
        int total = 0;
        foreach (var timestampDirectory in Directory.GetDirectories(yesterdayPath)) {
            string cur = "null";
            DateTime curDate = DateTime.MinValue;
            foreach (var trainTypeDirectory in Directory.GetDirectories(timestampDirectory)) {
                if (trainTypeDirectory.Contains("-")) {
                    foreach (var imageFile in Directory.GetFiles(trainTypeDirectory, "*.jpg")) {

                        string newFileName = Dundun.FileName4Train(imageFile);
                        cur = newFileName;
                        curDate = File.GetLastWriteTime(imageFile);
                        string destinationPath = Path.Join(resultPath, newFileName);
                        File.Copy(imageFile, destinationPath, true);

                        AsyncWrite(c2t.box, $"{newFileName} -> {resultPath} \u2713\n");
                        break; //每个文件夹一张车头的图
                    }
                }
            }
            total += 1;
            if (cur == "null")
                continue;
            string[] t = cur.Split("+");
            string ms = t[^1].Split(".")[0].Split('-')[^1];
            string newMs = (int.Parse(ms) + 1).ToString();

            string newName = $"{t[0]}+{t[1]}+{t[^1][..^7]}{newMs}.jpg";
            string avatarTargetPath = Path.Join(resultPath, newName);
            string avatarSourcePath = Path.Join(absolutePath, presentStation, avatarJpgName);

            File.Copy(avatarSourcePath, avatarTargetPath, true);
            File.SetLastWriteTime(avatarTargetPath, curDate.AddSeconds(avatarTimeGap));
            AsyncWrite(c2t.box, $"{avatarSourcePath} -> {avatarTargetPath} \u2713\n");
            //break; // TODO 只采集一次车，发布时删除
        }
        AsyncWrite(c2t.box, $"total={total}，长图收集完成，存储在 {resultPath}\n");
        AsyncWrite(c2t.box, $"如果站点选择错误，则将{resultPath}的{resultPath.Split('\\')[1]}手动改成实际站点名称\n");
        Process.Start("explorer.exe", resultPath);
    }
    /// <summary>
    /// 通过选项框选择站点
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StationChanged(object sender, SelectionChangedEventArgs e) {
        var box = (ComboBox)sender;
        var selected = box.SelectedItem;
        if (selected != null) {
            Console.WriteLine($"选择站点： {selected}");
            presentStation = selected.ToString();
            gatherLongButton.IsEnabled = true;
        }
    }
    /// <summary>
    /// 初始化日期
    /// </summary>
    private void InitializeDates() {
        this.CurrentDate = DateTime.Today;
        this.YesterdayDate = CurrentDate.AddDays(-1);
        Console.WriteLine($"当前日期: {CurrentDate:yyyy-MM-dd}");
        Console.WriteLine($"切割测试日期: {YesterdayDate:yyyy-MM-dd}");
    }
    /// <summary>
    /// 根据./long和./locomotive里留下的软件错误的长图找对应短图
    /// </summary>
    /// <returns>csv文件表示当前站点的测试明细</returns>
    private string PickShortImages() {
        string longHardwareImagePath = Path.Join(absolutePath, presentStation, longHardware);
        string longSoftwareImagePath = Path.Join(absolutePath, presentStation, longSoftware);
        string shortSoftwarePath = Path.Join(absolutePath, presentStation, shortSoftware);
        string longImagePath = Path.Join(absolutePath, presentStation, gatheredImageFolder);
        string locomotiveImagePath = Path.Join(absolutePath, presentStation, onlyLocomotiveFolder);
        if (Directory.Exists(longSoftwareImagePath))
            Directory.Delete(longSoftwareImagePath, true);
        if (!Directory.Exists(longHardwareImagePath))
            Directory.CreateDirectory(longHardwareImagePath);
        if (Directory.Exists(shortSoftwarePath))
            Directory.Delete(shortSoftwarePath, true);
        Directory.CreateDirectory(longSoftwareImagePath);
        Directory.CreateDirectory(shortSoftwarePath);
        foreach (string jpg in Directory.GetFiles(longImagePath)) {
            string[] parts = jpg.Split("+");
            string errorDescription = parts[0]; // hxxx or sxx
            if (parts.Length == 3)
                File.Delete(jpg);
        }
        foreach (string jpg in Directory.GetFiles(locomotiveImagePath)) {
            string[] parts = jpg.Split("+");
            string errorDescription = parts[0]; // hxxx or sxx
            if (parts.Length == 3)
                File.Delete(jpg);
            else { 
                string name = jpg.Split("\\")[^1];
                string newJpg = Path.Join(absolutePath, presentStation, gatheredImageFolder, name);
                File.Move(jpg, newJpg, true);
                AsyncWrite(c2t.box, $"{jpg} -> {newJpg}\n");
            }   
        }
        AsyncWrite(c2t.box, $"删除完毕！\n");
        if (Directory.GetFiles(longImagePath).Length == 0) {
            AsyncWrite(c2t.box, $"切割全对！\n");
            return "";
        }
        string res = GenerateCSV(longImagePath);
        string[] files = Directory.GetFiles(longImagePath);
        for (int j = 0; j < files.Length; j++) {
            string jpg = files[j];
            try {
                string[] parts = jpg.Split("\\")[^1].Split("+");
                string errorDescription = parts[0]; // hxxx or sxx
                if (errorDescription.ToLower().Contains("h")) {
                    File.Move(jpg, Path.Join(longHardwareImagePath, jpg.Split("\\")[^1]), true);
                }
                //software error
                else if (errorDescription.ToLower().Contains("s")) {
                    string shortImagePath = Path.Join(absolutePath, shortImageFolder, parts[1], parts[2]);
                    string name = Path.Join(absolutePath, shortImageFolder, parts[1], parts[2], parts[3]);
                    var imgs = Directory.GetFiles(shortImagePath);
                    int idx = -1;
                    for (int i = 0; i < imgs.Length; i++) {
                        /// 大于等
                        if (imgs[i].Split("\\")[^1].CompareTo(parts[3]) >= 0) {
                            idx = i;
                            AsyncWrite(c2t.box, $"找到 {parts[3]} 在 {imgs[i]} index={idx}\n");
                            break;
                        }
                    }
                    if (idx == -1) {
                        AsyncWrite(c2t.box, $"{jpg}的短图不存在或已过期，已跳过\n");
                        File.Move(jpg, Path.Join(longSoftwareImagePath, jpg.Split("\\")[^1]), true);
                        continue;
                    }
                    if (idx >= 0) {
                        int start, end;
                        //TODO 配置文件化
                        start = Math.Max(0, idx - 4);
                        end = Math.Min(idx + 6, imgs.Length - 1);
                        for (int i = start; i <= end; i++) {
                            string temp = imgs[i].Split("\\")[^1];
                            string dest = Path.Join(shortSoftwarePath, temp);
                            File.Copy(imgs[i], dest, true);
                            AsyncWrite(c2t.box, $"copy {imgs[i]} -> {dest}\u2713\n");
                        }
                    }
                    File.Move(jpg, Path.Join(longSoftwareImagePath, jpg.Split("\\")[^1]), true);
                }
            }
            catch (Exception) {
                AsyncWrite(c2t.box, $"{jpg}的短图不存在或已过期，已跳过\n");
                continue;
            }
        }
        return res;
    }
    /// <summary>
    /// 生成测试明细csv
    /// </summary>
    /// <param name="longImagePath">留下长图的绝对路径</param>
    /// <returns>csv字符串</returns>
    private string GenerateCSV(string longImagePath) {
        string res = "";
        string csv = $"{presentStation}CS{Dundun.Two(YesterdayDate.Month)}{Dundun.Two(YesterdayDate.Day)}.csv";
        using (StreamWriter writer = new StreamWriter(Path.Join(absolutePath, presentStation, csv), append: false, encoding: System.Text.Encoding.UTF8)) {
            writer.WriteLine("时间,朝向,错误描述,文件名");
            res += $"时间,朝向,错误描述,文件名\n";
            foreach (string jpg in Directory.GetFiles(longImagePath)) {
                string[] parts = jpg.Split("\\")[^1].Split("+");
                string errorDescription = parts[0]; // hxxx or sxx
                string orientation = parts[1][..2]; // SX-CS-Z -> SX
                string[] errorTimes = parts[^1].Split("-")[3..6];
                string errorTime = $"{errorTimes[0]}{errorTimes[1]}{errorTimes[2]}";
                string filename = parts[^1].Split(".")[0];
                writer.WriteLine($"{errorTime},{orientation},{errorDescription},{filename}");
                res += $"{errorTime},{orientation},{errorDescription},{filename}\n";
            }
        }
        return res;
    }

    private async void GatherLongClick(object sender, RoutedEventArgs e) {
        gatherLongButton.IsEnabled = false;
        stations.IsReadOnly = true;
        stations.IsEnabled = false;
        await Task.Run(() => {
            // 这里执行长时间运行的操作
            //print();
            try {
                // 可能会抛出异常的代码
                GatherLongImages();
            }
            catch (Exception ex) {
                AsyncWrite(c2t.box, $"{ex.ToString()}");
                return;
            }
        });
        gatherLongButton.IsEnabled = false;
        pickShortButton.IsEnabled = true;
    }
    private async void PickShortClick(object sender, RoutedEventArgs e) {
        pickShortButton.IsEnabled = false;
        string res = "";
        await Task.Run(() => {
            // 这里执行长时间运行的操作
            try {
                // 可能会抛出异常的代码
                res = PickShortImages();
            }
            catch (Exception ex) {
                // 显示异常信息
                AsyncWrite(c2t.box, ex.ToString());
                return;
            }
        });
        // TODO 复制到剪贴板 但是Clipboard.SetText(res)会导致卡死
        try {
            Clipboard.SetDataObject(res);
        }
        catch (Exception ex) {
            Console.WriteLine($"复制到剪贴板出错:{ex.ToString()}");
        }
        Console.WriteLine($"信息已复制到剪贴板！");
    }
    private async void GatherLocomotiveClick(object sender, RoutedEventArgs e) {


        await Task.Run(() => {
            // 这里执行长时间运行的操作
            //print();
            try {
                // 可能会抛出异常的代码

                GatherLongImageOnlyLocomitive();
            }
            catch (Exception ex) {
                // 显示异常信息

                AsyncWrite(c2t.box, $"{ex.ToString()}");
                return;
            }
        });
        gatherLocomotiveButton.IsEnabled = false;
    }
    private async void FtpUploadClick(object sender, RoutedEventArgs e) {
        ftpUploadButton.IsEnabled = false;
        await Task.Run(() => {
            // 这里执行长时间运行的操作
            //print();
            try {
                // 可能会抛出异常的代码
                FtpUploadShortSoftwareImage();
            }
            catch (Exception ex) {
                // 显示异常信息
                AsyncWrite(c2t.box, $"{ex.ToString()}");
                return;
            }
        });
    }
    private void FtpUploadShortSoftwareImage() {
        SimpleFtpClient ftp;
        string localPath = Path.Join(absolutePath, presentStation, shortSoftware);
        string testPath = Path.Join(absolutePath, presentStation, avatarJpgName);
        if (!Directory.Exists(localPath) || Directory.GetFiles(localPath).Length == 0) {
            return;
        }
        
        try {
            ftp = Dundun.Ftp();
            string[] jpgs = Directory.GetFiles(localPath);
            AsyncWrite(c2t.box, $"================FTP UPLOADING================\n");
            foreach (var jpg in jpgs) {
                try {
                    string name = jpg.Split("\\")[^1];
                 
                    ftp.UploadFile(jpg, Path.Join(ftpRemotePath, name));
                    AsyncWrite(c2t.box, $"local.{jpg} -> remote.{ftpRemotePath}\n");
                }
                catch (Exception e) {
                    AsyncWrite(c2t.box, $"local.{jpg} upload failed\n");
                    continue;
                }
            }
            AsyncWrite(c2t.box, $"================FTP DONE================\n");
        }
        catch (Exception ex) {
            AsyncWrite(c2t.box, ex.ToString());
            return;
        }

        




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
    /// <summary>
    /// 计算两个数的和
    /// </summary>
    /// <param name="a">第一个数</param>
    /// <param name="b">第二个数</param>
    /// <returns>返回两个数的和</returns>
    private void ResetTimerOnActivity(object sender, EventArgs e) {
        _inactivityTimer.Stop();
        _inactivityTimer.Start();
    }

    // 关闭应用程序
    private void CloseApplication() {
        _inactivityTimer.Stop();
        System.Windows.Application.Current.Shutdown();
    }
}

