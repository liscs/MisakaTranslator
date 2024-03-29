﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TranslatorLibrary;

namespace MisakaTranslator.SettingsPages
{
    /// <summary>
    /// TranslatorGeneralSettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class TranslatorGeneralSettingsPage : Page
    {
        private List<string> TranslatorList;

        public TranslatorGeneralSettingsPage()
        {
            InitializeComponent();
            TranslatorList = TranslatorCommon.GetTranslatorList();
            FirstTransCombox.ItemsSource = TranslatorList;
            SecondTransCombox.ItemsSource = TranslatorList;

            FirstTransCombox.SelectedIndex = TranslatorCommon.GetTranslatorIndex(Common.AppSettings.FirstTranslator);
            SecondTransCombox.SelectedIndex = TranslatorCommon.GetTranslatorIndex(Common.AppSettings.SecondTranslator);

            EachRowTransCheckBox.IsChecked = Common.AppSettings.EachRowTrans;
            HttpProxyBox.Text = Common.AppSettings.HttpProxy;

            TransLimitBox.Value = Common.AppSettings.TransLimitNums;
            // 给TransLimitBox添加Minimum后，初始化它时就会触发一次ValueChanged，导致Settings被设为1，因此只能从设置中读取数据后再添加事件处理函数
            TransLimitBox.ValueChanged += TransLimitBox_ValueChanged;
        }

        private void FirstTransCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Common.AppSettings.FirstTranslator = TranslatorCommon.TranslatorDict[(string)FirstTransCombox.SelectedValue];
        }

        private void SecondTransCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Common.AppSettings.SecondTranslator = TranslatorCommon.TranslatorDict[(string)SecondTransCombox.SelectedValue];
        }

        private void EachRowTransCheckBox_Click(object sender, RoutedEventArgs e)
        {
            Common.AppSettings.EachRowTrans = EachRowTransCheckBox.IsChecked ?? false;
        }

        private void HttpProxyBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string text = HttpProxyBox.Text.Trim();
            try { _ = new Uri(text); } catch (UriFormatException) { HandyControl.Controls.Growl.Error("Proxy url unsupported."); return; };
            Common.AppSettings.HttpProxy = text;
        }

        private void TransLimitBox_ValueChanged(object? sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            Common.AppSettings.TransLimitNums = (int)TransLimitBox.Value;
        }
    }
}
