using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BocchiTracker.MAUIBlazor.Pages
{
    public partial class Index
    {
        [Inject] CachedConfigRepository<ProjectConfig> _configRepo { get; set; }
        [Inject] NavigationManager _nav { get; set; }

        string SelectedValue = "";

        protected override void OnInitialized()
        {
            //    _nav.NavigateTo("config/general");
            OnDialogOpened();
            base.OnInitialized();
        }

        private async Task<IEnumerable<string>> Search(string value)
        {
            if (string.IsNullOrEmpty(value))
                return ItemsSource;
            return ItemsSource.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }


        #region WPF ConfigFilePicker
        public string ProjectConfigDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "Configs", "ProjectConfigs");

        public List<string> ItemsSource { get; set; } = new List<string>();

        void Submit()
        {
            //var filename = r.Parameters.GetValue<string>("Config");
            var result = Path.Combine(ProjectConfigDirectory, SelectedValue + ".yaml");
            _configRepo.SetLoadFilename(result);
            _configRepo.Save(new ProjectConfig());
            _nav.NavigateTo("config");
        }

        public void OnDialogOpened()
        {
            //EnableFileCreation.Value = parameters.GetValue<bool>("EnableFileCreation");
            // HintText.Value = EnableFileCreation.Value
            //   ? "Enter a new config or choose a config to edit"
            //  : "Chose a config to use";

            if (Directory.Exists(ProjectConfigDirectory))
            {
                var configs = Directory.GetFiles(ProjectConfigDirectory, "*.yaml");
                foreach (var config in configs)
                {
                    ItemsSource.Add(Path.GetFileNameWithoutExtension(config));
                }
            }
        }
        #endregion
    }
}
