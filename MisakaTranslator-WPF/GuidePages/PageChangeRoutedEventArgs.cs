﻿using System;
using System.Windows;

namespace MisakaTranslator.GuidePages
{
    public class PageChangeRoutedEventArgs : RoutedEventArgs
    {
        public PageChangeRoutedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source) { }

        /// <summary>
        /// 下一页的XAML地址
        /// </summary>
        public string XamlPath { get; set; } = string.Empty;
        public bool IsBack { get; internal set; }

        /// <summary>
        /// 部分方法需要用到的额外参数
        /// </summary>
        public object? ExtraArgs;
    }

    public class PageChange
    {
        public string? XamlPath;

        //声明和注册路由事件
        public static readonly RoutedEvent PageChangeRoutedEvent =
            EventManager.RegisterRoutedEvent("PageChange", RoutingStrategy.Bubble, typeof(EventHandler<PageChangeRoutedEventArgs>), typeof(PageChange));

    }


}
