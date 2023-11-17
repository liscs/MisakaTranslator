﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TTSHelperLibrary;


namespace MisakaTranslator_WPF.SettingsPages.TTSPages
{
    /// <summary>
    /// LocalTTSSettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class LocalTTSSettingsPage : Page
    {
        LocalTTS tsh;

        public LocalTTSSettingsPage()
        {
            tsh = new LocalTTS();
            InitializeComponent();

            List<string> lst = tsh.GetAllTTSEngine();
            TTSSourceCombox.ItemsSource = lst;

            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i] == Common.appSettings.ttsVoice)
                {
                    TTSSourceCombox.SelectedIndex = i;
                    break;
                }
            }
            VolumeBar.Value = Common.appSettings.ttsVolume;
            RateBar.Value = Common.appSettings.ttsRate;
        }

        private void TTSSourceCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tsh.SetTTSVoice((string)TTSSourceCombox.SelectedValue);
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            tsh.SpeakAsync(TestSrcText.Text);
            Common.appSettings.ttsVoice = (string)TTSSourceCombox.SelectedValue;
            Common.appSettings.ttsVolume = (int)VolumeBar.Value;
            Common.appSettings.ttsRate = (int)RateBar.Value;
        }

        private void VolumeBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tsh.SetVolume((int)VolumeBar.Value);
        }

        private void RateBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tsh.SetRate((int)RateBar.Value);
        }
    }
}