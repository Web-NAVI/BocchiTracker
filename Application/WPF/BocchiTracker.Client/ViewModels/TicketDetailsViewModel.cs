﻿using BocchiTracker.Data;
using BocchiTracker.Client.Share.Events;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters.Data;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Prism.Mvvm;
using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ServiceClientData;
using BocchiTracker.Client.Controls;

namespace BocchiTracker.Client.ViewModels
{
    public class TicketLabels : MultipleChoicesControl
    {
        public TicketProperty _ticketProperty { get; set; }

        public TicketLabels(TicketProperty ticketProperty) : base("Labels") { _ticketProperty = ticketProperty; }

        public override void OnUpdateRegisteredItems()
        {
            _ticketProperty.Labels.Clear();
            foreach (var item in RegisteredItems)
                _ticketProperty.Labels.Add(item as string);
        }
    }

    public class TicketWatchers : MultipleChoicesControl
    {
        public TicketProperty _ticketProperty { get; set; }

        public TicketWatchers(TicketProperty ticketProperty) : base("Watchers") { _ticketProperty = ticketProperty; }

        public override void OnUpdateRegisteredItems()
        {
            _ticketProperty.Watchers.Clear();
            foreach (var item in RegisteredItems)
                _ticketProperty.Watchers.Add(item as UserData);
        }
    }

    public class ConnectTo : OneChoiceControl
    {
        public TicketProperty _ticketProperty { get; set; }

        public ConnectTo(TicketProperty ticketProperty) : base("Connected To...") 
        { 
            _ticketProperty = ticketProperty;
            _ticketProperty.AppStatusBundles.AppConnected       = this.Connected;
            _ticketProperty.AppStatusBundles.AppDisconnected    = this.Disconnected;
        }

        public override void OnSelected(object inItem)
        {
            _ticketProperty.AppStatusBundles.TrackerApplication = inItem as AppStatusBundle;
        }

        public void Connected(AppStatusBundle inAppStatusBundle)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var foundItems = Items.Where(x => x == inAppStatusBundle).ToList();
                if (foundItems.Count() == 0)
                {
                    base.AddItem(inAppStatusBundle);
                }
            });
        }

        public void Disconnected(AppStatusBundle inAppStatusBundle)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var foundItems = Items.Where(x => x == inAppStatusBundle).ToList();
                foreach (var removeItem in foundItems)
                {
                    base.RemoveItem(removeItem);
                }


                if((Selected.Value as AppStatusBundle) == inAppStatusBundle)
                {
                    Selected.Value = null;
                }
            });
        }
    }

    public class TicketAssign : OneChoiceControl
    {
        public TicketProperty _ticketProperty { get; set; }

        public TicketAssign(TicketProperty ticketProperty) : base("Assign") { _ticketProperty = ticketProperty; }

        public override void OnSelected(object inItem) { _ticketProperty.Assign.Value = inItem as UserData; }
    }

    public class TicketClass : OneChoiceControl
    {
        public TicketProperty _ticketProperty { get; set; }

        public TicketClass(TicketProperty ticketProperty) : base("Class") { _ticketProperty = ticketProperty; }

        public override void OnSelected(object inItem) { _ticketProperty.Class.Value = inItem?.ToString(); }
    }

    public class TicketPriority : OneChoiceControl
    {
        public TicketProperty _ticketProperty { get; set; }

        public TicketPriority(TicketProperty ticketProperty) : base("Priority") { _ticketProperty = ticketProperty; }

        public override void OnSelected(object inItem) { _ticketProperty.Priority.Value = inItem?.ToString(); }
    }

    public class TicketDetailsViewModel : BindableBase
    {
        [Dependency]
        public TicketProperty TicketProperty { get; set; }

        public TicketClass TicketClass { get; set; }

        public TicketPriority TicketPriority { get; set; }

        public TicketAssign TicketAssign { get; set; }

        public TicketLabels TicketLabels { get; set; }

        public TicketWatchers TicketWatchers { get; set; }

        public ConnectTo ConnectTo { get; set; }

        public TicketDetailsViewModel(IEventAggregator inEventAggregator, TicketProperty inTicketProperty) 
        {
            TicketClass = new TicketClass(inTicketProperty);
            TicketPriority = new TicketPriority(inTicketProperty);
            TicketAssign = new TicketAssign(inTicketProperty);
            TicketLabels = new TicketLabels(inTicketProperty);
            TicketWatchers = new TicketWatchers(inTicketProperty);
            ConnectTo = new ConnectTo(inTicketProperty);

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<PopulateCbValuesEvent>()
                .Subscribe(OnPopulateCbValuesEvent, ThreadOption.UIThread);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach (var item in inParam.ProjectConfig.IssueGrades)
                TicketClass.Items.Add(item);

            foreach (var item in inParam.ProjectConfig.Priorities)
                TicketPriority.Items.Add(item);

            var issue_info_bundle = (Application.Current as PrismApplication).Container.Resolve<IssueInfoBundle>();
            foreach (var item in issue_info_bundle.LabelListService.GetUnifiedData())
                TicketLabels.Items.Add(item.Name);

            foreach (var item in issue_info_bundle.UserListService.GetUnifiedData())
            {
                TicketAssign.Items.Add(item);
                TicketWatchers.Items.Add(item);
            }
        }

        private void OnPopulateCbValuesEvent()
        {
            TicketClass.Selected.Value      = TicketProperty.Class.Value;
            TicketClass.EditText.Value      = TicketProperty.Class.Value;
            TicketPriority.Selected.Value   = TicketProperty.Priority.Value;
            TicketPriority.EditText.Value   = TicketProperty.Priority.Value;
            TicketAssign.Selected.Value     = TicketProperty.Assign.Value;
            TicketAssign.EditText.Value     = TicketProperty.Assign.Value.Name;

            {
                var temporaryList = new List<string>(TicketProperty.Labels);
                foreach (var item in temporaryList)
                    TicketLabels.RegisteredItems.Add(item);
            }

            {
                var temporaryList = new List<UserData>(TicketProperty.Watchers);
                foreach (var item in temporaryList)
                    TicketWatchers.RegisteredItems.Add(item.Name);
            }
        }
    }
}
