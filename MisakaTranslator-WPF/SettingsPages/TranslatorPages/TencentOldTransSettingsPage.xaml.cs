﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TranslatorLibrary.Translator;

namespace MisakaTranslator_WPF.SettingsPages.TranslatorPages
{
    /// <summary>
    /// TencentOldTransSettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class TencentOldTransSettingsPage : Page
    {
        public TencentOldTransSettingsPage()
        {
            InitializeComponent();
            TransAppIDBox.Text = Common.appSettings.TXOSecretId;
            TransSecretKeyBox.Text = Common.appSettings.TXOSecretKey;
        }

        private async void AuthTestBtn_Click(object sender, RoutedEventArgs e)
        {
            Common.appSettings.TXOSecretId = TransAppIDBox.Text;
            Common.appSettings.TXOSecretKey = TransSecretKeyBox.Text;
            ITranslator Trans = new TencentOldTranslator();
            Trans.TranslatorInit(TransAppIDBox.Text, TransSecretKeyBox.Text);
            if (await Trans.TranslateAsync("apple", "zh", "en") != null)
            {
                HandyControl.Controls.Growl.Success($"腾讯云{Application.Current.Resources["APITest_Success_Hint"]}");
            }
            else
            {
                HandyControl.Controls.Growl.Error($"腾讯云{Application.Current.Resources["APITest_Error_Hint"]}\n{Trans.GetLastError()}");
            }
        }

        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(TencentOldTranslator.GetUrl_allpyAPI()) { UseShellExecute = true });
        }

        private void DocBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(TencentOldTranslator.GetUrl_Doc()) { UseShellExecute = true });
        }

        private void BillBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(TencentOldTranslator.GetUrl_bill()) { UseShellExecute = true });
        }

        private async void TransTestBtn_Click(object sender, RoutedEventArgs e)
        {
            ITranslator Trans = new TencentOldTranslator();
            Trans.TranslatorInit(Common.appSettings.TXOSecretId, Common.appSettings.TXOSecretKey);
            string res = await Trans.TranslateAsync(TestSrcText.Text, TestDstLang.Text, TestSrcLang.Text);
            if (res != null)
            {
                HandyControl.Controls.MessageBox.Show(res, Application.Current.Resources["MessageBox_Result"].ToString());
            }
            else
            {
                HandyControl.Controls.Growl.Error($"腾讯云{Application.Current.Resources["APITest_Error_Hint"]}\n{Trans.GetLastError()}");
            }
        }
    }
}