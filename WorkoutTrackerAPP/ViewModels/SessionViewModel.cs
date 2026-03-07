using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Messages;
using WorkoutTrackerAPP.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WorkoutTrackerAPP.ViewModels
{
    [QueryProperty(nameof(SessionId), "SessionId")]
    public partial class SessionViewModel : ObservableObject
    {
        private ISessions _sessions;

        private int _sessionId;
        private WorkoutDTO _workoutSnapshot;
        private SessionDTO _session;


        [ObservableProperty]
        private string workoutName = "";

        [ObservableProperty]
        private DateTime sessionDate;

        [ObservableProperty]
        private TimeSpan sessionDuration;
        public ObservableCollection<WorkoutGroupDTO> Groups { get; } = new();
        public SessionViewModel(ISessions sessions)
        {
            _sessions = sessions;
            Debug.WriteLine($"{WorkoutName}");

        }

        public int SessionId
        {
            set
            {
                if (_sessionId == value)
                    return;

                _sessionId = value;
                if (_workoutSnapshot != null) return;
                var session = _sessions.Sessions.FirstOrDefault(s => s.Id == value);
                if (session == null) return;
                LoadSession(session)    ;
            }
        }

        public void LoadSession(SessionDTO session)
        {
            _workoutSnapshot = session.WorkoutSnapshot;
            if (_workoutSnapshot == null) return;

            LoadWorkout(_workoutSnapshot);
            sessionDate = session.Date;
            sessionDuration = session.Duration;
        }
        public void LoadWorkout(WorkoutDTO workout)
        {
            WorkoutName = workout.Name;
            Debug.WriteLine($"{WorkoutName}");
            Groups.Clear();

            foreach (var group in workout.Groups)
            {
                var groupCopy = new WorkoutGroupDTO
                {
                    Name = group.Name
                };

                foreach (var item in group.Items)
                {
                    var itemCopy = new WorkoutExerciseDTO
                    {
                        ExerciseId = item.ExerciseId,
                        Name = item.Name,
                        Type = item.Type,
                        Reps = item.Reps,
                        MaxDuration = item.MaxDuration,
                        Duration = item.MaxDuration,
                        ParentGroup = groupCopy
                    };
                    groupCopy.Items.Add(itemCopy);
                }
                Groups.Add(groupCopy);
            }
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.Navigation.PopAsync();
        }

    }
}
