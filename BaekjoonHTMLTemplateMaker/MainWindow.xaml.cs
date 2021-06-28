using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BaekjoonHTMLTemplateMaker.ThirdParty;
using CsQuery;
using CsQuery.ExtensionMethods.Xml;
using MoreLinq;
using Newtonsoft.Json.Linq;
using Clipboard = System.Windows.Clipboard;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using TextBox = System.Windows.Controls.TextBox;
using TextDataFormat = System.Windows.TextDataFormat;

namespace BaekjoonHTMLTemplateMaker
{
    public class BaekjoonProblem
    {
        public string Title { get; private set; }                                              // 문제 이름
        public int Number { get; private set; }                                                // 문제 번호
        public int TimeLimit { get; private set; }                                             // 시간 제한(초)
        public int MemoryLimit { get; private set; }                                           // 메모리 제한(MB)
        public int SubmitPeopleCount { get; private set; }                                     // 제출자 수
        public int AnswerPeopleCount { get; private set; }                                     // 정답자 수
        public double CorrectAnswerRate { get; private set; }                                  // 정답률
        public List<string> ProblemDescriptionParagraphsArray { get; private set; }            // 문제 설명
        public List<string> ProblemDescriptionTagsArray { get; private set; }                  // 문제 설명 (HTML 형식)
        public List<string> InputDescriptionParagraphsArray { get; private set; }              // 입력 설명
        public List<string> OutputDescriptionParagraphsArray { get; private set; }             // 출력 설명
        public List<Tuple<string, string>> Samples { get; private set; }                       // 입, 출력 예시
        public long StasticsDateTick { get; private set; }                                     // 통계 날짜
        public List<string> HintDescriptionParagraphsArray { get; private set; }               // 힌트

        private BaekjoonProblem()
        {
            // 자체 생성 못하게 막음.
        }

        /**
         * [ 백준에서 추출한 json 프로퍼티 목록 ]
         *
         * _number;                            // 문제번호
         * _title;                             // 문제 이름
         * _timeLimit;                         // 시간 제한(초)
         * _memoryLimit;                       // 메모리 제한(MB)
         * _submitPeopleCount;                 // 제출자 수
         * _answerPeopleCount;                 // 정답자 수
         * _correctAnswerRate;                 // 정답률
         * _problemDescriptionParagraphsArray; // 문제 설명
         * _problemDescriptionTagsArray;       // 문제 설명 (HTML 형식)
         * _inputDescriptionParagraphsArray;   // 입력 설명
         * _outputDescriptionParagraphsArray;  // 출력 설명
         * _samples;                           // 입, 출력 예시
         * _stasticsDateTick;                  // 통계 날짜
         * _hintDescriptionParagraphsArray;    // 힌트
         */

        public static BaekjoonProblem Parse(string jsonContent)
        {
            BaekjoonProblem problem = new BaekjoonProblem();
            JObject root = JObject.Parse(jsonContent);

            int number = int.Parse(root["_number"]?.ToString() ?? "0");
            string title = root["_title"]?.ToString() ?? string.Empty;
            int timeLimit = int.Parse(root["_timeLimit"]?.ToString() ?? "0");
            int memoryLimit = int.Parse(root["_memoryLimit"]?.ToString() ?? "0");
            int submitPeopleCount = int.Parse(root["_submitPeopleCount"]?.ToString() ?? "0");
            int answerPeopleCount = int.Parse(root["_answerPeopleCount"]?.ToString() ?? "0");
            double correctAnswerRate = double.Parse(root["_correctAnswerRate"]?.ToString() ?? "0");
            List<string> problemDescriptionParagraphsArray = new List<string>();
            List<string> problemDescriptionTagsArray = new List<string>();
            List<string> inputDescriptionParagraphsArray = new List<string>();
            List<string> outputDescriptionParagraphsArray = new List<string>();
            List<Tuple<string, string>> samples = new List<Tuple<string, string>>();
            long stasticsDateTick = long.Parse(root["_stasticsDateTick"]?.ToString() ?? "0");
            List<string> hintDescriptionParagraphsArray = new List<string>();

            if (root.ContainsKey("_problemDescriptionParagraphsArray"))
            {
                foreach (JObject obj in root["_problemDescriptionParagraphsArray"] as JArray)
                {
                    problemDescriptionParagraphsArray.Add(obj["paragraph"]?.ToString() ?? string.Empty);
                }
            }

            if (root.ContainsKey("_problemDescriptionTagsArray"))
            {
                foreach (JObject obj in root["_problemDescriptionTagsArray"] as JArray)
                {
                    problemDescriptionTagsArray.Add(obj["tag"]?.ToString() ?? string.Empty);
                }
            }

            if (root.ContainsKey("_inputDescriptionParagraphsArray"))
            {
                foreach (JObject obj in root["_inputDescriptionParagraphsArray"] as JArray)
                {
                    inputDescriptionParagraphsArray.Add(obj["paragraph"]?.ToString() ?? string.Empty);
                }
            }

            if (root.ContainsKey("_outputDescriptionParagraphsArray"))
            {
                foreach (JObject obj in root["_outputDescriptionParagraphsArray"] as JArray)
                {
                    outputDescriptionParagraphsArray.Add(obj["paragraph"]?.ToString() ?? string.Empty);
                }
            }

            if (root.ContainsKey("_samples"))
            {
                foreach (JObject obj in root["_samples"] as JArray)
                {
                    samples.Add(new Tuple<string, string>(
                            obj["inputSample"]?.ToString() ?? string.Empty,
                            obj["outputSample"]?.ToString() ?? string.Empty
                        )
                    );
                }
            }

            if (root.ContainsKey("_hintDescriptionParagraphsArray"))
            {
                foreach (JObject obj in root["_hintDescriptionParagraphsArray"] as JArray)
                {
                    hintDescriptionParagraphsArray.Add(obj["paragraph"]?.ToString() ?? string.Empty);
                }
            }

            problem.Number = number;
            problem.Title = title;
            problem.TimeLimit = timeLimit;
            problem.MemoryLimit = memoryLimit;
            problem.SubmitPeopleCount = submitPeopleCount;
            problem.AnswerPeopleCount = answerPeopleCount;
            problem.CorrectAnswerRate = correctAnswerRate;
            problem.ProblemDescriptionParagraphsArray = problemDescriptionParagraphsArray;
            problem.ProblemDescriptionTagsArray = problemDescriptionTagsArray;
            problem.InputDescriptionParagraphsArray = inputDescriptionParagraphsArray;
            problem.OutputDescriptionParagraphsArray = outputDescriptionParagraphsArray;
            problem.Samples = samples;
            problem.HintDescriptionParagraphsArray = hintDescriptionParagraphsArray;
            problem.StasticsDateTick = stasticsDateTick;

            return problem;
        }
    }

    public partial class MainWindow : Window
    {
        private const string _exportsPath = "exports";
        private const string _backjoonDomCralerFileName = "baekjoon-dom-crawler.exe";
        private const string _cliboardHtmlFormatFile = "clipboard-format/naver-smart-editor.chtml";
        private string _sourceUrl = "";

        public MainWindow()
        {
            InitializeComponent();
            ReloadBaekjoonProblems();
            ListView_Log.Items.Clear();
            TextEditor_Text.TextArea.Margin = new Thickness(5);
        }
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

       

        private ListViewItem CreateListViewLogItem(string content, SolidColorBrush background)
        {
            ListViewItem logItem = new ListViewItem
            {
                Margin = new Thickness(0, 0, 0, 3), 
                Content = content, 
                Background = background
            };
            return logItem;
        }

        private void CheckIsLogFull(int removeCount)
        {
            if (ListView_Log.Items.Count + removeCount >= 200)
            {
                for (int i =0; i < removeCount; i++)
                    ListView_Log.Items.RemoveAt(0);
                ListView_Log.ScrollIntoView(ListView_Log.Items[ListView_Log.Items.Count - 1]);
            }
        }

        private void WriteErrorLog(string log, Exception e, bool save = false)
        {
            this.Dispatcher.BeginInvoke((Action) delegate
            {
                CheckIsLogFull(2);
                ListView_Log.Items.Add(CreateListViewLogItem(log, Brushes.LightCoral));
                ListView_Log.Items.Add(CreateListViewLogItem($"Exception : {e.Message}", Brushes.LightCoral));
                if (save)
                    File.WriteAllText("error.txt", $"{log}\n{e.Message}\n{e.StackTrace}");
                ListView_Log.ScrollIntoView(ListView_Log.Items[ListView_Log.Items.Count - 1]);
            });
            
        }

        private void WriteLog(string log)
        {
            this.Dispatcher.BeginInvoke((Action) delegate
            {
                CheckIsLogFull(1);
                ListView_Log.Items.Add(CreateListViewLogItem(log, Brushes.Thistle));
                ListView_Log.ScrollIntoView(ListView_Log.Items[ListView_Log.Items.Count - 1]);
            });
        }

        private ListViewItem CreateListViewProblemItem(string content, object tag)
        {
            ListViewItem logItem = new ListViewItem
            {
                Margin = new Thickness(0, 0, 0, 3), 
                Content = content, 
                Background = Brushes.Beige,
                Tag = tag
            };

            return logItem;
        }


        private void NumberTextbox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void ReloadBaekjoonProblems()
        {
            try
            {
                if (!Directory.Exists(_exportsPath))
                    Directory.CreateDirectory(_exportsPath);

                ListView_BaekjoonProblems.Items.Clear();

                List<Task> tasks = new List<Task>();

                foreach (string filePath in Directory.GetFiles(_exportsPath))
                {
                    if (string.Equals(Path.GetExtension(filePath), ".json", StringComparison.OrdinalIgnoreCase))
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            BaekjoonProblem tag = BaekjoonProblem.Parse(File.ReadAllText(filePath));
                            string content = Path.GetFileNameWithoutExtension(filePath) + " : " + tag.Title;

                            this.Dispatcher.BeginInvoke((Action) delegate
                            {
                                ListViewItem item = CreateListViewProblemItem(content, tag);
                                ListView_BaekjoonProblems.Items.Add(item);
                            });
                        }));
                    }
                }

                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception e)
            {
                WriteErrorLog("백준 문제를 로딩하는데 실패했습니다.", e, true);
            }
        }

        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(_backjoonDomCralerFileName))
            {
                MessageBox.Show($"{_backjoonDomCralerFileName} 파일이 존재하지 않습니다.");
                return;
            }

            int startNumber =  int.Parse(TextBox_StartNumber.Text);
            int endNumber = int.Parse(TextBox_EndNunber.Text);

            if (endNumber - startNumber < 0)
            {
                MessageBox.Show("마지막 숫자가 시작 숫자보다 크면 안됩니다.");
                return;
            }

            Task.Run(() =>
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo()
                {
                    Arguments = $"{startNumber} {endNumber}",
                    FileName = Path.GetFullPath(_backjoonDomCralerFileName),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };
                p.Start();


                while (!p.StandardOutput.EndOfStream)
                {
                    // 밖에서 해야함. BeginInvoke 내부에서 호출하면 이미 닫힌 상태일 수 있음.
                    string readString = p.StandardOutput.ReadLine();
                    WriteLog(readString);
                }

                p.WaitForExit();
                WriteLog($"{_backjoonDomCralerFileName} 프로세스가 종료되었습니당 ^_^ ({p.ExitCode}) ");
                this.Dispatcher.BeginInvoke((Action)ReloadBaekjoonProblems);
                
            });
        }

        private string CreateSampleTitleTag(string titleName)
        {
            const string sampleTitleStartTag =
                @"<p><span style=""font-family: 나눔바른고딕, NanumBarunGothic, NanumBarunGothicOTF; font-size: 14.6667px;"">";
            const string sampleTitleEndTag =
                @"</span></p>";

            return sampleTitleStartTag + titleName + sampleTitleEndTag;
        }

        private string CreateSampleTag(string sampleContent)
        {
            const string sampleStartTag =
                @"<p><span style=""font-family: 나눔바른고딕, NanumBarunGothic, NanumBarunGothicOTF; font-size: 12.6667px;"">";
            const string sampleEndStartTag =
                @"</span></p>";


            return sampleStartTag + sampleContent + sampleEndStartTag;
        }

        private void ListView_BaekjoonProblems_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ListView_BaekjoonProblems.SelectedItem as ListViewItem;

            if (selectedItem is null)
                return;

            var problem = selectedItem.Tag as BaekjoonProblem;

            if (problem == null)
            {
                MessageBox.Show($"{selectedItem.Content} 데이터를 찾을 수 없습니다.");
                return;
            }

            if (!File.Exists(_cliboardHtmlFormatFile))
            {
                MessageBox.Show($"{_cliboardHtmlFormatFile} 클립보드 포맷 파일을 찾을 수 없습니다.");
                return;
            }

            /**
             *   @ https://blog.naver.com/wjdeh313/222409681295
             *   최대 22글자
             *  _$Title$_
             *  _$CorrectAnswerRate$_
             *  _$SubmitPeopleCount$_
             *  _$AnswerPeopleCount$_
             *  _$MemoryLimit$_
             *  _$TimeLimit$_
             *  _$Number$_
             *  _$StasticsDate$_
             *  _$ProblemDescription$_
             *  _$InputDescription$_
             *  _$OutputDescription$_
             *  _$InputOutputSamples$_
             *  _$HintDescription$_
             */

            
            string chtmlFormatFileContentString = File.ReadAllText(_cliboardHtmlFormatFile);
            StringBuilder problemDescription = new StringBuilder(2000);
            StringBuilder inputDescription = new StringBuilder(2000);
            StringBuilder outputDescription = new StringBuilder(2000);
            StringBuilder hintDescription = new StringBuilder(2000);
            StringBuilder samples = new StringBuilder(2000);

            long TICKS_AT_EPOCH = 621355968000000000L;
            long TICKS_PER_MILLISECOND = 10000;
            DateTime stasticsDateTime = new DateTime(TICKS_AT_EPOCH + problem.StasticsDateTick * TICKS_PER_MILLISECOND);

            foreach (string paragraph in problem.ProblemDescriptionTagsArray)
                problemDescription.Append(paragraph);

            foreach (string paragraph in problem.InputDescriptionParagraphsArray)
                inputDescription.Append(paragraph);

            foreach (string paragraph in problem.OutputDescriptionParagraphsArray)
                outputDescription.Append(paragraph);

            foreach (string paragraph in problem.HintDescriptionParagraphsArray)
                hintDescription.Append(paragraph);

            


            for (int i = 0; i < problem.Samples.Count; i++)
            {
                samples.Append(CreateSampleTitleTag($"< 입력 예시 {i + 1} >"));
                samples.Append(CreateSampleTag($"{problem.Samples[i].Item1}"));
                samples.Append("<br>");
                samples.Append(CreateSampleTitleTag($"< 출력 예시 {i + 1} >"));
                samples.Append(CreateSampleTag($"{problem.Samples[i].Item2}"));
                samples.Append("<br>");
            }

            //string[] sourceUrlSplited =
            //    chtmlFormatFileContentString.Split(new string[] {"SourceURL:"}, StringSplitOptions.None);

            //if (sourceUrlSplited.Length >= 2)
            //{
            //    sourceUrlSplited = sourceUrlSplited[1].Split(new string[] {"<html>"}, StringSplitOptions.None);

            //    if (sourceUrlSplited.Length >= 2)
            //    {
            //        _sourceUrl = sourceUrlSplited[0];
            //    }
            //}


            chtmlFormatFileContentString = chtmlFormatFileContentString
                .Replace("_$Title$_", problem.Title)
                .Replace("_$CorrectAnswerRate$_", string.Format("정답률 : {0:F} %", problem.CorrectAnswerRate))
                .Replace("_$SubmitPeopleCount$_", string.Format("제출자 : {0} 명", problem.SubmitPeopleCount))
                .Replace("_$AnswerPeopleCount$_", string.Format("정답자 : {0} 명", problem.AnswerPeopleCount))
                .Replace("_$MemoryLimit$_", string.Format("메모리 제한 : {0} MB", problem.MemoryLimit))
                .Replace("_$TimeLimit$_", string.Format("시간 제한 : {0} 초", problem.TimeLimit))
                .Replace("_$Number$_", string.Format("{0}", problem.Number))
                .Replace("_$StasticsDate$_", string.Format("통계 날짜 : {0:f} (틱 : {1:D})", stasticsDateTime, problem.StasticsDateTick))
                .Replace("_$ProblemDescription$_", problemDescription.ToString())
                .Replace("_$InputDescription$_", inputDescription.ToString())
                .Replace("_$OutputDescription$_", outputDescription.ToString())
                .Replace("_$InputOutputSamples$_", samples.ToString())
                .Replace("_$HintDescription$_", hintDescription.ToString());

            

            //string htmlFragmentContent = string.Empty;

            //try
            //{
            //    htmlFragmentContent = ClipboardHelper.GetHtmlFragmentContent(chtmlFormatFileContentString);
            //}
            //catch (Exception exception)
            //{
            //    MessageBox.Show($"{exception.Message}\n\n{exception.StackTrace}");
            //    return;
            //}

            ClipboardHelper.CopyToClipboard(chtmlFormatFileContentString, _sourceUrl);
            TextEditor_Text.Text = Clipboard.GetText(TextDataFormat.Html);
            WriteLog("텍스트 박스 내용을 HTML 포맷으로 클립보드에 복사하였습니다.");
        }

        private void Button_SettingHtmlClipboard_OnClick(object sender, RoutedEventArgs e)
        {
            new HtmlClipboardSetting().ShowDialog();
        }

        private void ChangeNodes(IEnumerable<IDomElement> nodeList, Action<IDomElement> nodeActor)
        {
            if (nodeList == null || !nodeList.Any())
                return;

            foreach (var node in nodeList)
            {
                nodeActor(node);
                ChangeNodes(node.ChildElements, nodeActor);
            }
        }

        private void Button_CopyHtml_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextEditor_Text.Text, TextDataFormat.Html);
            string copied = Clipboard.GetText(TextDataFormat.Html);

            if (copied.Equals(TextEditor_Text.Text))
                WriteLog("텍스트 박스 내용을 HTML 포맷으로 클립보드에 복사하였습니다.");
            else
                WriteLog("텍스트 박스 내용을 HTML 포맷으로 클립보드에 복사하는데 실패하였습니다.\n올바른 HTML 포멧인지 확인해주세요");

            //string html = ClipboardHelper.GetHtml(TextEditor_Text.Text);
            //CQ htmlDom = new CQ(html);
            //htmlDom["span"]?.Selection.First(x => x.HasAttribute("data-input-buffer"))?.Remove();
            //string decodedContent = WebUtility.HtmlDecode(htmlDom.Html());
            //ClipboardHelper.CopyToClipboard(decodedContent, _sourceUrl);
            //TextEditor_Text.Text = Clipboard.GetText(TextDataFormat.Html);
            //WriteLog("클립보드 복사 성공");
        }
    }
}
