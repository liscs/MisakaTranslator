﻿using DataAccessLibrary;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TextHookLibrary;

namespace MisakaTranslator.GuidePages.Hook
{
    /// <summary>
    /// ChooseGamePage.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseGamePage : Page
    {
        private Dictionary<string, int> lstProcess;
        private int GamePid;
        private List<System.Diagnostics.Process> SameNameGameProcessList = new();

        public ChooseGamePage()
        {
            InitializeComponent();
            GamePid = -1;
            lstProcess = ProcessHelper.GetProcessList_Name_PID();
            GameProcessCombox.ItemsSource = lstProcess.Keys;
        }

        private void GameProcessCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GamePid = lstProcess[(string)GameProcessCombox.SelectedValue];
            SameNameGameProcessList = ProcessHelper.FindSameNameProcess(GamePid);
            AutoHookTag.Text = Application.Current.Resources["ChooseGamePage_AutoHookTag_Begin"].ToString() + SameNameGameProcessList.Count + Application.Current.Resources["ChooseGamePage_AutoHookTag_End"].ToString();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (GamePid != -1)
            {
                if (SameNameGameProcessList.Count == 1)
                {
                    Common.TextHooker = new TextHookHandle(lstProcess[(string)GameProcessCombox.SelectedValue]);
                }
                else
                {
                    Common.TextHooker = new TextHookHandle(SameNameGameProcessList);
                }

                if (!Common.TextHooker.Init(x64GameCheckBox.IsChecked ?? false ? Common.AppSettings.Textractor_Path64 : Common.AppSettings.Textractor_Path32))
                {
                    HandyControl.Controls.MessageBox.Show(Application.Current.Resources["MainWindow_TextractorError_Hint"].ToString());
                    return;
                }

                Common.GameID = Guid.Empty;
                GameInfo targetGame;
                string filepath = ProcessHelper.FindProcessPath(GamePid, x64GameCheckBox.IsChecked ?? false);
                if (filepath != "")
                {
                    targetGame = GameHelper.GetGameByPath(filepath);
                    Common.GameID = targetGame.GameID;
                    targetGame.Isx64 = x64GameCheckBox.IsChecked ?? false;
                    GameHelper.SaveGameInfo(targetGame);
                }

                //使用路由事件机制通知窗口来完成下一步操作
                PageChangeRoutedEventArgs args = new(PageChange.PageChangeRoutedEvent, this)
                {
                    XamlPath = "GuidePages/Hook/ChooseHookFuncPage.xaml"
                };
                this.RaiseEvent(args);
            }
            else
            {
                HandyControl.Controls.Growl.Error(Application.Current.Resources["ChooseGamePage_NextErrorHint"].ToString());
            }

        }
    }
}
