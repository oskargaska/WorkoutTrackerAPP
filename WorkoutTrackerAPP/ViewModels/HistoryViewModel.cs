using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;
using WorkoutTrackerAPP.Views;


namespace WorkoutTrackerAPP.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        private ISessions _sessions;

        public ObservableCollection<SessionDTO> Sessions => _sessions.Sessions;
        public ObservableCollection<SessionDTO> SelectedDaySessions { get; } = new();


        public HistoryViewModel(ISessions sessions)
        {
            _sessions = sessions;
            FilterSessions(selectedDate);
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.Navigation.PopAsync();
        }


        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        partial void OnSelectedDateChanged(DateTime value)
        {
            FilterSessions(value);
        }

        private void FilterSessions(DateTime date)
        {
            SelectedDaySessions.Clear();

            var sessionsForDay = Sessions
                .Where(s => s.Date.Date == date.Date)
                .OrderBy(s => s.StartTime);

            foreach (var session in sessionsForDay)
            {
                SelectedDaySessions.Add(session);
            }
        }

        [RelayCommand]
        async Task SessionSelected(SessionDTO session)
        {
            var page = App.Current.Handler.MauiContext.Services.GetRequiredService<SessionView>();
            var vm = (SessionViewModel)page.BindingContext;
            vm.SessionId = session.Id;
            await Shell.Current.Navigation.PushAsync(page);
        }

    }
}
