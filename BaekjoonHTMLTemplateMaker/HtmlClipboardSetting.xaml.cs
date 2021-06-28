using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BaekjoonHTMLTemplateMaker.ThirdParty;
using CsQuery;
using MoreLinq;
using Clipboard = System.Windows.Clipboard;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using IDataObject = System.Windows.IDataObject;
using MessageBox = System.Windows.MessageBox;
using TextDataFormat = System.Windows.TextDataFormat;

namespace BaekjoonHTMLTemplateMaker
{
    /// <summary>
    /// HtmlClipboardSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HtmlClipboardSetting : Window
    {
        private const string _cliboardHtmlFormatFile = "clipboard-format/naver-smart-editor.chtml";
        private string _clipboardHtmlFormatData = string.Empty;

        public HtmlClipboardSetting()
        {
            InitializeComponent();
            TextEditor_Text.TextArea.Margin = new Thickness(10);
        }

        private void HtmlClipboardSetting_OnClosing(object sender, CancelEventArgs e)
        {
        }

        private void HtmlClipboardSetting_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_PasteHtml_Click(object sender, RoutedEventArgs e)
        {
            TextEditor_Text.Text = Clipboard.GetText(TextDataFormat.Html);
        }

        private void Button_CopyHtml_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextEditor_Text.Text, TextDataFormat.Html);
            string tempClipboardFormatText = Clipboard.GetText(TextDataFormat.Html);

            if (tempClipboardFormatText != TextEditor_Text.Text)
                MessageBox.Show("복사 실패");
            else
            {
                MessageBox.Show("복사 성공");
                _clipboardHtmlFormatData = tempClipboardFormatText;
            }
                
        }

        private void Button_SaveFormat_Click(object sender, RoutedEventArgs e)
        {
            if (_clipboardHtmlFormatData.Equals(String.Empty))
            {
                MessageBox.Show("먼저 클립보드 복사를 해주세요.");
                return;
            }

            File.WriteAllText(_cliboardHtmlFormatFile, _clipboardHtmlFormatData);
            MessageBox.Show("저장완료");
            _clipboardHtmlFormatData = string.Empty;
        }

        

        private void Button_Test_OnClick(object sender, RoutedEventArgs e)
        {
            IDataObject obj = Clipboard.GetDataObject();

            var dataObject = new DataObject();

            string html = obj.GetData(DataFormats.Html) as string;

           

            dataObject.SetData(DataFormats.Html, html);
            //dataObject.SetData(DataFormats.Text, obj.GetData(DataFormats.Text));
            //dataObject.SetData(DataFormats.UnicodeText, obj.GetData(DataFormats.UnicodeText));
            //dataObject.SetData(DataFormats.StringFormat, obj.GetData(DataFormats.StringFormat));
            //dataObject.SetData(DataFormats.Locale, obj.GetData(DataFormats.Locale));
            //dataObject.SetData(DataFormats.OemText, obj.GetData(DataFormats.OemText));


            Clipboard.SetDataObject(dataObject);
        }

        private void Button_Empty_OnClick(object sender, RoutedEventArgs e)
        {
            DataObject obj = new DataObject();
            Clipboard.SetDataObject(obj);
        }

        private void Button_PrintFormats_OnClick(object sender, RoutedEventArgs e)
        {
            IDataObject obj = Clipboard.GetDataObject();
            string info = string.Empty;
            obj.GetFormats().ForEach(x =>
            {
                info += $"{x}\n";
            });

            TextEditor_Test.Text = info;
        }
    }
}
