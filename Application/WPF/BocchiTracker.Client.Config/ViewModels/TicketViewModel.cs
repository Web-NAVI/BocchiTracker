using BocchiTracker.Client.Config.Controls;
using BocchiTracker.ServiceClientData;
using BocchiTracker.Config.Configs;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BocchiTracker.Config;
using BocchiTracker.ModelEvent;

namespace BocchiTracker.Client.Config.ViewModels
{
    public class ValueMapBase : BindableBase
    {
        public ICommand AddItemCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }
        public ReactiveCollection<ServiceValueMapping> ValueMappings { get; set; } = new ReactiveCollection<ServiceValueMapping>();

        public ValueMapBase()
        {
            AddItemCommand = new DelegateCommand<string>(OnAddItem);
            RemoveItemCommand = new DelegateCommand<string>(OnRemoveItem);
        }

        public void OnAddItem(string inValue)
        {
            if (string.IsNullOrEmpty(inValue))
                return;
            if (Find(inValue) != null)
                return;
            ValueMappings.Add(new ServiceValueMapping(inValue));
        }

        public void OnRemoveItem(string inValue)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var itemToRemove = Find(inValue);
                if (itemToRemove != null)
                    ValueMappings.Remove(itemToRemove);
            });
        }

        public ServiceValueMapping Find(string inValue)
        {
            return ValueMappings.FirstOrDefault(kvp => kvp.Definition.Value == inValue);
        }
    }

    public class TicketViewModel : BindableBase
    {
        public ValueMapBase TicketTypes { get; set; } = new ValueMapBase();
        public ValueMapBase Priorities { get; set; } = new ValueMapBase();
        public ValueMapBase IssueGrades { get; set; } = new ValueMapBase();
        public ValueMapBase QueryFields { get; set; } = new ValueMapBase();

        public TicketViewModel(IEventAggregator inEventAggregator)
        {
            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<ApplicationExitEvent>()
                .Subscribe(OnSaveConfig);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            if (inParam.ProjectConfig != null)
            {
                OnConfigReloadCommon(inParam.ProjectConfig, TicketTypes, Priorities, IssueGrades, QueryFields);
            }
        }

        public static void OnConfigReloadCommon(ProjectConfig projectConfig, ValueMapBase ticketTypes, ValueMapBase priorities, ValueMapBase issueGrades, ValueMapBase queryFields)
        {
            ApplyProjectConfigToValueMappings(
                ticketTypes,
                projectConfig.TicketTypes,
                projectConfig.ServiceConfigs == null ? null : projectConfig.ServiceConfigs.Where(x => x.TicketTypeMappings != null).Select(x => (x.Service, x.TicketTypeMappings)).ToList());

            ApplyProjectConfigToValueMappings(
                priorities,
                projectConfig.Priorities,
                projectConfig.ServiceConfigs == null ? null : projectConfig.ServiceConfigs.Where(x => x.PriorityMappings != null).Select(x => (x.Service, x.PriorityMappings)).ToList());

            ApplyProjectConfigToValueMappings(
                issueGrades,
                projectConfig.IssueGrades,
                projectConfig.ServiceConfigs == null ? null : projectConfig.ServiceConfigs.Where(x => x.IssueGradeMappings != null).Select(x => (x.Service, x.IssueGradeMappings)).ToList());

            ApplyProjectConfigToValueMappings(
                queryFields,
                projectConfig.QueryFields,
                projectConfig.ServiceConfigs == null ? null : projectConfig.ServiceConfigs.Where(x => x.QueryFieldMappings != null).Select(x => (x.Service, x.QueryFieldMappings)).ToList());

        }

        public static void ApplyProjectConfigToValueMappings(ValueMapBase ioValueMappingsContainer, List<string> inDefinitions, List<(ServiceDefinitions, List<ValueMapping>)> inServiceValueMappings)
        {
            foreach (var definition in inDefinitions)
                ioValueMappingsContainer.ValueMappings.Add(new ServiceValueMapping(definition));

            if (inServiceValueMappings == null)
                return;

            foreach (var (service, values) in inServiceValueMappings)
            {
                foreach (var mapping in values)
                {
                    ServiceValueMapping valueMapping = ioValueMappingsContainer.ValueMappings.ToList().Find(vm => vm.Definition.Value == mapping.Definition);
                    if (valueMapping == null)
                        continue;

                    valueMapping.SetServiceName(service, mapping.Name);
                }
            }
        }

        private void OnSaveConfig()
        {
            var projectConfigrepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig = projectConfigrepository.Load();
            OnSaveConfigCommon(projectConfig, TicketTypes, Priorities, IssueGrades, QueryFields);
        }

        public static void OnSaveConfigCommon(ProjectConfig projectConfig, ValueMapBase ticketTypes, ValueMapBase priorities, ValueMapBase issueGrades, ValueMapBase queryFields)
        {
            projectConfig.TicketTypes = ticketTypes.ValueMappings.Select(x => x.Definition.Value).ToList();
            foreach (var serviceConfig in projectConfig.ServiceConfigs)
            {
                serviceConfig.TicketTypeMappings.Clear();
                foreach (var valueMapping in ticketTypes.ValueMappings)
                {
                    var service = serviceConfig.Service;
                    serviceConfig.TicketTypeMappings.Add(new ValueMapping { Definition = valueMapping.Definition.Value, Name = valueMapping.GetServiceName(service) });
                }
            }

            projectConfig.Priorities = priorities.ValueMappings.Select(x => x.Definition.Value).ToList();
            foreach (var serviceConfig in projectConfig.ServiceConfigs)
            {
                serviceConfig.PriorityMappings.Clear();
                foreach (var valueMapping in priorities.ValueMappings)
                {
                    var service = serviceConfig.Service;
                    serviceConfig.PriorityMappings.Add(new ValueMapping { Definition = valueMapping.Definition.Value, Name = valueMapping.GetServiceName(service) });
                }
            }

            projectConfig.IssueGrades = issueGrades.ValueMappings.Select(x => x.Definition.Value).ToList();
            foreach (var serviceConfig in projectConfig.ServiceConfigs)
            {
                serviceConfig.IssueGradeMappings.Clear();
                foreach (var valueMapping in issueGrades.ValueMappings)
                {
                    var service = serviceConfig.Service;
                    serviceConfig.IssueGradeMappings.Add(new ValueMapping { Definition = valueMapping.Definition.Value, Name = valueMapping.GetServiceName(service) });
                }
            }

            projectConfig.QueryFields = queryFields.ValueMappings.Select(x => x.Definition.Value).ToList();
            foreach (var serviceConfig in projectConfig.ServiceConfigs)
            {
                serviceConfig.QueryFieldMappings.Clear();
                foreach (var valueMapping in queryFields.ValueMappings)
                {
                    var service = serviceConfig.Service;
                    serviceConfig.QueryFieldMappings.Add(new ValueMapping { Definition = valueMapping.Definition.Value, Name = valueMapping.GetServiceName(service) });
                }
            }

        }
    }
}
