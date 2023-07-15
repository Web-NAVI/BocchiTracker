﻿using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters.Data;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;


//public class TicketProperty
//{
//    [Required(ErrorMessage = "Required")]
//    public ReactiveProperty<string> Summary { get; }

//    [Required(ErrorMessage = "Required")]
//    public ReactiveProperty<string> Description { get; }

//    [Required(ErrorMessage = "Required")]
//    public ReactiveProperty<string> SelectedTicketType { get; }

//    public ReactiveCommand PostIssueCommand { get; private set; }

//    public TicketProperty()
//    {
//        this.Summary = new ReactiveProperty<string>().SetValidateAttribute(() => this.Summary);
//        this.Description = new ReactiveProperty<string>().SetValidateAttribute(() => this.Description);
//        this.SelectedTicketType = new ReactiveProperty<string>().SetValidateAttribute(() => this.SelectedTicketType);

//        // 実行コマンドのアクティブ設定
//        this.PostIssueCommand =
//            new[]
//            {
//                    this.Summary.ObserveHasErrors,
//                    this.Description.ObserveHasErrors,
//                    this.SelectedTicketType.ObserveHasErrors,
//            }
//            .CombineLatestValuesAreAllFalse()   // すべてエラーなしの場合にアクティブ設定
//            .ToReactiveCommand();
//    }
//}

namespace BocchiTracker.Data
{
    public class TicketProperty
    {
        [Required(ErrorMessage = "Required")]
        public ReactiveProperty<string> Summary { get; }

        [Required(ErrorMessage = "Required")]
        public ReactiveProperty<string> TicketType { get; }

        [Required(ErrorMessage = "Required")]
        public ReactiveProperty<string> Description { get; }

        public ReactiveProperty<string> Class { get; }

        public ReactiveProperty<string> Priority { get; }

        public ReactiveProperty<UserData> Assign { get; }

        public ReactiveCollection<UserData> Watchers { get; }

        public ReactiveCollection<string> Labels { get; }

        public AppStatusBundles AppStatusBundles { get; set; }

        public TicketProperty(IssueInfoBundle inIssueInfoBundle, AppStatusBundles inAppStatusBundles)
        {
            AppStatusBundles = inAppStatusBundles;

            Summary = new ReactiveProperty<string>(inIssueInfoBundle.TicketData.Summary);
            Summary.Subscribe(value => inIssueInfoBundle.TicketData.Summary = value);

            TicketType = new ReactiveProperty<string>();
            TicketType.Subscribe(value => inIssueInfoBundle.TicketData.TicketType = value);

            Description = new ReactiveProperty<string>(inIssueInfoBundle.TicketData.Description);
            Description.Subscribe(value => inIssueInfoBundle.TicketData.Description = value);

            Class = new ReactiveProperty<string>(inIssueInfoBundle.TicketData.Class);
            Class.Subscribe(value => inIssueInfoBundle.TicketData.Class = value);

            Priority = new ReactiveProperty<string>(inIssueInfoBundle.TicketData.Priority);
            Priority.Subscribe(value => inIssueInfoBundle.TicketData.Priority = value);

            Assign = new ReactiveProperty<UserData>(inIssueInfoBundle.TicketData.Assign);
            Assign.Subscribe(value => inIssueInfoBundle.TicketData.Assign = value);

            Labels = new ReactiveCollection<string>(/*inIssueInfoBundle.TicketData.Lables*/);
            Labels.CollectionChanged += (_, __) => { inIssueInfoBundle.TicketData.Lables = Labels.ToList(); };

            Watchers = new ReactiveCollection<UserData>(/*inIssueInfoBundle.TicketData.Lables*/);
            Watchers.CollectionChanged += (_, __) => { inIssueInfoBundle.TicketData.Watchers = Watchers.ToList(); };
        }
    }
}