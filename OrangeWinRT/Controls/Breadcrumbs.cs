using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace OrangeWinRT.Controls
{
    [TemplatePart(Name = ITEMS_CONTROL_PARTNAME, Type = typeof(ItemsControl))]
    public sealed class Breadcrumbs : Control
    {
        #region Template Parts
        private ItemsControl _itemsControl;

        const string ITEMS_CONTROL_PARTNAME = "ItemsControl";
        #endregion

        public Breadcrumbs()
        {
            this.DefaultStyleKey = typeof(Breadcrumbs);
        }

        protected override void OnApplyTemplate()
        {
            this._itemsControl = GetTemplateChild(ITEMS_CONTROL_PARTNAME) as ItemsControl;
            base.OnApplyTemplate();
        }

        private void _populateItems()
        {
            var breadcrumbsSource = this.BreadcrumbsSource as IList<object>;
            if (breadcrumbsSource == null ||
                this._itemsControl == null ||
                this.BreadcrumbTemplate == null ||
                this.SeparatorTemplate == null)
            {
                return;
            }
            int cnt = breadcrumbsSource.Count();
            if (cnt == 0)
            {
                return;
            }

            var items = new List<FrameworkElement>();

            for (int i = 0; i < cnt - 1; ++i)
            {
                var breadcrumbElement = this.BreadcrumbTemplate.LoadContent() as FrameworkElement;
                breadcrumbElement.DataContext = breadcrumbsSource[i];
                items.Add(breadcrumbElement);

                var separatorElement = this.SeparatorTemplate.LoadContent() as FrameworkElement;
                separatorElement.DataContext = breadcrumbsSource[i];
                items.Add(separatorElement);
            }

            FrameworkElement terminalBreadcrumbElement;
            if (this.TerminalBreadcrumbTemplate != null)
            {
                terminalBreadcrumbElement = this.TerminalBreadcrumbTemplate.LoadContent() as FrameworkElement;
            }
            else 
            {
                terminalBreadcrumbElement = this.BreadcrumbTemplate.LoadContent() as FrameworkElement;
            }
            terminalBreadcrumbElement.DataContext = breadcrumbsSource[cnt - 1];
            items.Add(terminalBreadcrumbElement);

            this._itemsControl.ItemsSource = items;
        }

        #region BreadcrumbsSource
        public object BreadcrumbsSource
        {
            get { return GetValue(BreadcrumbsSourceProperty); }
            set { SetValue(BreadcrumbsSourceProperty, value); }
        }

        public static readonly DependencyProperty BreadcrumbsSourceProperty = DependencyProperty.Register("BreadcrumbsSource",
            typeof(object),
            typeof(Breadcrumbs),
            new PropertyMetadata(null, OnBreadcrumbsSourcePropertyChanged));

        private static void OnBreadcrumbsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Breadcrumbs)d)._populateItems();
        }
        #endregion

        #region BreadcrumbTemplate
        public DataTemplate BreadcrumbTemplate
        {
            get { return (DataTemplate)GetValue(BreadcrumbTemplateProperty); }
            set { SetValue(BreadcrumbTemplateProperty, value); }
        }

        public static readonly DependencyProperty BreadcrumbTemplateProperty = DependencyProperty.Register("BreadcrumbTemplate",
            typeof(DataTemplate), typeof(Breadcrumbs), null);
        #endregion

        #region TerminalBreadcrumbTemplate
        public DataTemplate TerminalBreadcrumbTemplate
        {
            get { return (DataTemplate)GetValue(TerminalBreadcrumbTemplateProperty); }
            set { SetValue(TerminalBreadcrumbTemplateProperty, value); }
        }

        public static readonly DependencyProperty TerminalBreadcrumbTemplateProperty = DependencyProperty.Register("TerminalBreadcrumbTemplate",
            typeof(DataTemplate), typeof(Breadcrumbs), null);
        #endregion

        #region SeparatorTemplate
        public DataTemplate SeparatorTemplate
        {
            get { return (DataTemplate)GetValue(SeparatorTemplateProperty); }
            set { SetValue(SeparatorTemplateProperty, value); }
        }

        public static readonly DependencyProperty SeparatorTemplateProperty = DependencyProperty.Register("SeparatorTemplate",
            typeof(DataTemplate), typeof(Breadcrumbs), null);
        #endregion
    }
}
