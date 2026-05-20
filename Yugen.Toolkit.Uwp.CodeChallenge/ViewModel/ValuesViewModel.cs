using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Yugen.Toolkit.Uwp.CodeChallenge.Interfaces;
using Yugen.Toolkit.Uwp.CodeChallenge.Model;

namespace Yugen.Toolkit.Uwp.CodeChallenge.ViewModel
{
    public class ValuesViewModel : ObservableObject
    {
        private readonly IDummyApiService _apiService;
        private readonly IDataService _dataService;
        private bool _isSynching;
        private ValueModel _selectedValueModel;

        public ValuesViewModel(IDummyApiService apiService, IDataService dataService)
        {
            _apiService = apiService;
            _dataService = dataService;

            SetupValues();
        }

        public bool IsSynching
        {
            get => _isSynching;

            set
            {
                if (SetProperty(ref _isSynching, value))
                {
                    OnPropertyChanged(nameof(CanSync));
                }
            }
        }

        public bool CanSync => _isSynching == false;

        public ObservableCollection<ValueModel> Values { get; set; } = [];

		public ValueModel SelectedValueModel
        {
            get => _selectedValueModel;

            set
            {
                if (SetProperty(ref _selectedValueModel, value))
                {
                    OnPropertyChanged(nameof(HasSelectedModel));
                }
            }
        }

        public bool HasSelectedModel => SelectedValueModel != null;

        public ICommand SyncCommand => new RelayCommand(SyncData);

        private void SetupValues()
        {
            if (_dataService.HasValues)
            {
                foreach (var valueModel in _dataService.Values)
                {
                    Values.Add(valueModel);
                }
            }
        }

        private async void SyncData()
        {
            IsSynching = true;

            Values.Clear();

            var valueModels = await _apiService.GetValueModelsAsync();

            foreach (var ValueModel in valueModels.OrderBy(valueModel => valueModel.Order))
            {
                Values.Add(ValueModel);
            }

            await _dataService.Save(Values.ToList());

            IsSynching = false;
        }

        private void SaveState()
        {
            var currentOrder = 0;

            foreach (var ValueModel in Values)
            {
                ValueModel.Order = currentOrder;
                currentOrder++;
            }

            _dataService.Save(Values.ToList());
        }
    }
}