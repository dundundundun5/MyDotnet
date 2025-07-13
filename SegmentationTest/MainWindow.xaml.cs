
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using TextBox = System.Windows.Controls.TextBox;
using Clipboard = System.Windows.Clipboard;
namespace SegmentationTest;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private const string absolutePath = @"D:\";
    private const string longImageFolder = "FTP2";
    private const string shortImageFolder = "Monitor";
    private const string gatheredImageFolder = "long";
    private const string avatarJpgName = "avatar.jpg";
    private Console2Textbox c2t;
    public List<string> StationList { get; } = new List<string>() {
        "伍明", "凤台", "包庄", "大许", "宁波", "建国", "新塘边", "杨集", "杭州", "枫泾", "泗安", "淮北北", "湾沚", "炮车", "虞城", "西寺坡", "誓节渡","李庄","姚李庙","杨楼","梓树庄","烔炀河"
    };
    public string presentStation { get; set; } = new string("null");
    public DateTime CurrentDate { get; private set; }
    public DateTime YesterdayDate { get; private set; }

    public MainWindow() {
        
        InitializeComponent();
        c2t = new Console2Textbox(myConsole);
        CheckPresentStation();
        
        
        InitializeDates();

    }
    private void CheckPresentStation() {
        for (int i = 0; i < StationList.Count(); i++) {
            if (Directory.Exists(Path.Join(absolutePath, StationList[i]))) {
                stations.SelectedIndex = i;
                stations.IsEnabled = false;
                break;
            }
        }
    }
    private void AsyncWrite(TextBox box, string text) {
        void Write(System.Windows.Controls.TextBox box, string text) {
            box.AppendText(text);
            box.ScrollToEnd();
        }
        Action<TextBox, String> updateAction = new Action<TextBox, string>(Write);
        box.Dispatcher.BeginInvoke(updateAction, box, text);
    }
    private void GatherLongImages() {
        gatherLongButton.IsEnabled = false;
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
                        
                        AsyncWrite(c2t.box, $"{newFileName} -> {resultPath} \u2713\n");
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
            File.SetLastWriteTime(avatarTargetPath, curDate.AddMilliseconds(3));
            AsyncWrite(c2t.box, $"{avatarSourcePath} -> {avatarTargetPath} \u2713\n");
        }
        AsyncWrite(c2t.box, $"total={total}，长图收集完成，存储在 {resultPath}\n");
        AsyncWrite(c2t.box, $"如果站点选择错误，则将{resultPath}的{resultPath.Split('\\')[1]}手动改成实际站点名称\n");
        Process.Start("explorer.exe", resultPath);


    }
    private void PathCheck() {

        string joinedPath = Path.Join(absolutePath, presentStation);
        if (Directory.Exists(joinedPath)) {
            Console.WriteLine($"路径 {joinedPath} 存在");
        }
        else {
            Console.WriteLine($"路径 {joinedPath} 并不存在，新建文件夹");
            Console.WriteLine($"路径 {joinedPath} 创建成功");
        }

    }

    private void StationChanged(object sender, SelectionChangedEventArgs e) {
        var box = (ComboBox)sender;
        var selected = box.SelectedItem;
        if (selected != null) {
            Console.WriteLine($"选择站点： {selected}");
            presentStation = selected.ToString();
            gatherLongButton.IsEnabled = true;
        }
    }

    private void InitializeDates() {
        this.CurrentDate = DateTime.Today;
        this.YesterdayDate = CurrentDate.AddDays(-1);
        Console.WriteLine($"当前日期: {CurrentDate:yyyy-MM-dd}");
        Console.WriteLine($"切割测试日期: {YesterdayDate:yyyy-MM-dd}");
    }

    // TODO 收集完长图以后才显示在左下角的textblock中，红色字体，大字
    // TODO 简要阐明有问题的长图的标记规则
    public void ShowRules() {

    }

    private string PickShortImages() {
        string longHardware = "long_hardware", longSoftware = "long_software";
        string shortHardware = "short_hardware", shortSoftware = "short_software";
        string longHardwareImagePath = Path.Join(absolutePath, presentStation, longHardware);
        string longSoftwareImagePath = Path.Join(absolutePath, presentStation, longSoftware);
        string shortHardwarePath = Path.Join(absolutePath, presentStation, shortHardware);
        string shortSoftwarePath = Path.Join(absolutePath, presentStation, shortSoftware);
        string longImagePath = Path.Join(absolutePath, presentStation, gatheredImageFolder);
        if (Directory.Exists(longSoftwareImagePath))
            Directory.Delete(longSoftwareImagePath, true);
        if (!Directory.Exists(longHardwareImagePath))
            Directory.CreateDirectory(longHardwareImagePath);
        if (Directory.Exists(shortHardwarePath))
            Directory.Delete(shortHardwarePath, true);
        if (Directory.Exists(shortSoftwarePath))
            Directory.Delete(shortSoftwarePath, true);
        
        Directory.CreateDirectory(longSoftwareImagePath);
        Directory.CreateDirectory(shortHardwarePath);
        Directory.CreateDirectory(shortSoftwarePath);
        foreach (string jpg in Directory.GetFiles(longImagePath)) {
            string[] parts = jpg.Split("+");
            string errorDescription = parts[0]; // hxxx or sxx
            if (parts.Length == 3)
                File.Delete(jpg);
        }
        AsyncWrite(c2t.box, $"删除完毕！\n");
        // TODO GenerateCSV(longImagePath);
        string res = GenerateCSV(longImagePath);
        foreach (string jpg in Directory.GetFiles(longImagePath)) {
            string[] parts = jpg.Split("\\")[^1].Split("+");
            string errorDescription = parts[0]; // hxxx or sxx
            if (errorDescription.ToLower().Contains("h")) {
                string shortImagePath = Path.Join(absolutePath, shortImageFolder, parts[1], parts[2]);
                string name = Path.Join(absolutePath, shortImageFolder, parts[1], parts[2], parts[3]);
                var imgs = Directory.GetFiles(shortImagePath);
                int idx = -1;
                for (int i = 0; i < imgs.Length; i++) {
                    if (imgs[i].Contains(parts[3])) {
                        idx = i;
                        AsyncWrite(c2t.box, $"找到 {parts[3]} 在 {imgs[i]} index={idx}\n");
                        break;
                    }
                }
                //int index = new List<string>(imgs).IndexOf(name);
                if (idx >= 0) {
                    for (int i = Math.Max(idx - 5, 0); i <= Math.Min(idx + 3, imgs.Length -1 ); i++) {
                        string temp = imgs[i].Split("\\")[^1];
                        string dest = Path.Join(shortHardwarePath, temp);
                        File.Copy(imgs[i], dest, true);
                        AsyncWrite(c2t.box, $"copy {imgs[i]} -> {dest}");
                    }
                }
                File.Move(jpg, Path.Join(longHardwareImagePath, jpg.Split("\\")[^1]), true);
            }
            else if (errorDescription.ToLower().Contains("s")) {
                string shortImagePath = Path.Join(absolutePath, shortImageFolder, parts[1], parts[2]);
                string name = Path.Join(absolutePath, shortImageFolder, parts[1], parts[2], parts[3]);
                var imgs = Directory.GetFiles(shortImagePath);
                int idx = -1;
                for (int i = 0; i < imgs.Length; i++) {
                    if (imgs[i].Contains(parts[3])) {
                        idx = i;
                        AsyncWrite(c2t.box, $"找到 {parts[3]} 在 {imgs[i]} index={idx}\n");
                        break;
                    }
                }
                //int index = new List<string>(imgs).IndexOf(name);
                if (idx >= 0) {
                    for (int i = Math.Max(idx , 0); i <= Math.Min(idx + 8, imgs.Length - 1); i++) {
                        string temp = imgs[i].Split("\\")[^1];
                        string dest = Path.Join(shortSoftwarePath, temp);
                        File.Copy(imgs[i], dest, true);
                        AsyncWrite(c2t.box, $"copy {imgs[i]} -> {dest}");
                    }
                }
                File.Move(jpg, Path.Join(longSoftwareImagePath, jpg.Split("\\")[^1]), true);

            }
            
        }
        return res;
    }

    private string GenerateCSV(string longImagePath) {
        string res = "";
        string csv = $"{presentStation}CS{Dundun.Two(YesterdayDate.Month)}{Dundun.Two(YesterdayDate.Day)}.csv";
        using (StreamWriter writer = new StreamWriter(Path.Join(absolutePath, presentStation, csv), append:false, encoding: System.Text.Encoding.UTF8)) {
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
        
        
        AsyncWrite(c2t.box, $"CSV文件已生成：{csv}, 已复制到剪贴板\n");
        return res;
    }

    private async void GatherLongClick(object sender, RoutedEventArgs e) {
        gatherLongButton.IsEnabled = false;
        stations.IsReadOnly = true;
        stations.IsEnabled = false;
        PathCheck();
        await Task.Run(() => {
            // 这里执行长时间运行的操作
            //print();
            try {
                // 可能会抛出异常的代码
                GatherLongImages();
            }
            catch (Exception ex) {
                // 显示异常信息
                MessageBox.Show($"发生错误: {ex.ToString()}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
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
                //MessageBox.Show("短图收集完成！");
            }
            catch (Exception ex) {
                // 显示异常信息
                MessageBox.Show($"发生错误: {ex.ToString()}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        Clipboard.SetText(res);
        //return;
        MessageBox.Show("短图收集完成！");

    }
}

