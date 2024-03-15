﻿using DataAccessLibrary;
using MisakaTranslator.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using TextHookLibrary;
using Windows.Win32;

namespace MisakaTranslator.GuidePages.Hook
{
    /// <summary>
    /// ChooseGamePage.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseGamePage : Page
    {
        private readonly Dictionary<string, int> _processList = ProcessHelper.GetProcessList_Name_PID();
        private int _gamePid = -1;
        private List<Process> _sameNameGameProcessList = new();
        static ChooseGameViewModel _viewModel = new();

        private static int _focusingPid;
        static ChooseGamePage()
        {
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChangedHandler);
        }


        public ChooseGamePage()
        {
            InitializeComponent();
            DataContext = _viewModel;
            if (Common.IsAdmin)
            {
                NoAdminPrivilegesTextBlock.Visibility = Visibility.Collapsed;
            }

            GameProcessCombox.ItemsSource = _processList.Keys.OrderBy(p => p);
        }

        private void GameProcessCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _gamePid = _processList[(string)GameProcessCombox.SelectedValue];
            _sameNameGameProcessList = ProcessHelper.FindSameNameProcess(_gamePid);
            AutoHookTag.Text = Application.Current.Resources["ChooseGamePage_AutoHookTag_Begin"].ToString() + _sameNameGameProcessList.Count + Application.Current.Resources["ChooseGamePage_AutoHookTag_End"].ToString();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_gamePid != -1)
            {
                GenerateHookerAndGotoNextStep(_processList[(string)GameProcessCombox.SelectedValue]);
            }
            else
            {
                HandyControl.Controls.Growl.Error(Application.Current.Resources["ChooseGamePage_NextErrorHint"].ToString());
            }
        }

        private void GenerateHookerAndGotoNextStep(int pid)
        {
            if (_sameNameGameProcessList.Count == 1)
            {
                Common.TextHooker = new TextHookHandle(pid);
            }
            else
            {
                Common.TextHooker = new TextHookHandle(_sameNameGameProcessList);
            }
            if (Common.TextHooker.Init(x64GameCheckBox.IsChecked ?? false ? Common.AppSettings.Textractor_Path64 : Common.AppSettings.Textractor_Path32))
            {
                GoToNextStep();
            }
            else
            {
                HandyControl.Controls.MessageBox.Show(Application.Current.Resources["MainWindow_TextractorError_Hint"].ToString());
            }
        }

        private void GoToNextStep()
        {
            Common.GameID = Guid.Empty;
            string filepath = ProcessHelper.FindProcessPath(_gamePid, x64GameCheckBox.IsChecked ?? false);
            if (filepath != "")
            {
                GameInfoBuilder.GameInfo = GameHelper.GetGameByPath(filepath);
                Common.GameID = GameInfoBuilder.GameInfo.GameID;
                GameInfoBuilder.GameInfo.Isx64 = x64GameCheckBox.IsChecked ?? false;

                //使用路由事件机制通知窗口来完成下一步操作
                PageChangeRoutedEventArgs args = new(PageChange.PageChangeRoutedEvent, this)
                {
                    XamlPath = "GuidePages/Hook/ChooseHookFuncPage.xaml",
                };
                this.RaiseEvent(args);
            }
        }

        private void SelectWindowButton_Click(object sender, RoutedEventArgs e)
        {
            _gamePid = _focusingPid;
            _sameNameGameProcessList = ProcessHelper.FindSameNameProcess(_gamePid);
            GenerateHookerAndGotoNextStep(_gamePid);
        }

        unsafe private int GetProcessIdFromFocus()
        {
            uint thisPid;
            PInvoke.GetWindowThreadProcessId(PInvoke.GetForegroundWindow(), &thisPid);
            while (true)
            {
                uint pid;
                if (PInvoke.GetWindowThreadProcessId(PInvoke.GetForegroundWindow(), &pid) != 0 && pid != thisPid)
                {
                    return (int)pid;
                }
            }
        }


        private static void OnFocusChangedHandler(object src, AutomationFocusChangedEventArgs args)
        {
            AutomationElement? element = src as AutomationElement;
            if (element != null)
            {
                if (element.Current.ProcessId != Environment.ProcessId)
                {
                    _focusingPid = element.Current.ProcessId;
                }
                using Process process = Process.GetProcessById(_focusingPid);
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    _viewModel.FocusingProcess = $"目前焦点进程：{process.MainWindowTitle}";
                    _viewModel.EnableSelectFocusButton = true;
                }
                else
                {
                    _viewModel.FocusingProcess = string.Empty;
                    _viewModel.EnableSelectFocusButton = false;
                }
            }
        }
    }
}
