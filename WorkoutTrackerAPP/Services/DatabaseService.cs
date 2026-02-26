using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WorkoutTrackerAPP.DatabaseEntities;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Services
{
    internal class DatabaseService : IDatabase
    {
        private IDatabaseConnection _databaseConnection;


        public DatabaseService(IDatabaseConnection databaseConnection) 
        {
            _databaseConnection = databaseConnection;

        }

        

        public async Task<List<ExerciseDTO>> GetExercisesAsync()
        {
            var connection = await _databaseConnection.GetConnectionAsync();
            var entities = await connection.Table<ExerciseEntity>().ToListAsync();
            var exercise = new List<ExerciseDTO>(); 

            if(entities == null)
            {
                Debug.WriteLine("Database querry returned empty list of exercise entities");
                throw new InvalidOperationException("Empty database");
            }
            foreach (var entity in entities)
            {
                if (entity.Json == null) continue;
                
                var dto = JsonSerializer.Deserialize<ExerciseDTO>(entity.Json);
                
                if (dto == null) continue; 

                exercise.Add(dto);
            }
            return exercise;


        }

        public async Task<List<WorkoutDTO>> GetWorkoutsAsync()
        {
            var connection = await _databaseConnection.GetConnectionAsync();
            var entities = await connection.Table<WorkoutEntity>().ToListAsync();
            var workouts = new List<WorkoutDTO>();

            if (entities == null || entities.Count == 0)
            {
                Debug.WriteLine("Database querry returned empty list of workout entities");
                return workouts;
            }
            foreach (var entity in entities)
            {
                if (entity.Json == null) continue;
                
                var dto = JsonSerializer.Deserialize<WorkoutDTO>(entity.Json);

                if (dto == null) continue; // also check if deserialization returned null

                dto.Id = entity.Id;

                workouts.Add(dto);
            }
            return workouts;
        }

        public async Task<int> AddWorkoutAsync(WorkoutDTO workout)
        {
            
            var connection = await _databaseConnection.GetConnectionAsync();

            var entity = new WorkoutEntity
            {
                Name = workout.Name,
                Json = JsonSerializer.Serialize(workout)
                
            };
            await connection.InsertAsync(entity);

            return entity.Id;

        }

        public async Task UpdateWorkoutAsync(WorkoutDTO workout)
        {
            var connection = await _databaseConnection.GetConnectionAsync();

            var entity = new WorkoutEntity
            {
                Id = (int)workout.Id,
                Name = workout.Name,
                Json = JsonSerializer.Serialize(workout)
            };

            await connection.UpdateAsync(entity);
        }

        public async Task DeleteWorkoutAsync(int id)
        {
            var connection = await _databaseConnection.GetConnectionAsync();

            await connection.DeleteAsync<WorkoutEntity>(id);
        }

        public async Task<List<SessionDTO>> GetSessionsAsync()
        {
            var connection = await _databaseConnection.GetConnectionAsync();
            var entities = await connection.Table<SessionEntity>().ToListAsync();

            var sessions = new List<SessionDTO>();

            if (entities == null || entities.Count == 0)
            {
                Debug.WriteLine("Database query returned empty list of session entities");
                return sessions;
            }

            foreach (var entity in entities)
            {
                if (string.IsNullOrWhiteSpace(entity.Json)) continue;

                var workoutSnapshot = JsonSerializer.Deserialize<WorkoutDTO>(entity.Json);

                if (workoutSnapshot == null) continue;

                var dto = new SessionDTO
                {
                    Id = entity.Id,

                    Date = DateTimeOffset
                        .FromUnixTimeMilliseconds(entity.Date)
                        .LocalDateTime,

                    StartTime = DateTimeOffset
                        .FromUnixTimeMilliseconds(entity.StartTime)
                        .LocalDateTime,

                    EndTime = DateTimeOffset
                        .FromUnixTimeMilliseconds(entity.EndTime)
                        .LocalDateTime,

                    WorkoutSnapshot = workoutSnapshot
                };

                sessions.Add(dto);
            }

            return sessions;
        }

        public async Task<int> AddSessionAsync(SessionDTO session)
        {
            var connection = await _databaseConnection.GetConnectionAsync();

            var entity = new SessionEntity
            {
                
                Date = new DateTimeOffset(session.Date.ToUniversalTime()).ToUnixTimeMilliseconds(),
                StartTime = new DateTimeOffset(session.StartTime.ToUniversalTime()).ToUnixTimeMilliseconds(),
                EndTime = new DateTimeOffset(session.EndTime.ToUniversalTime()).ToUnixTimeMilliseconds(),
                Json = JsonSerializer.Serialize(session.WorkoutSnapshot)

            };
            await connection.InsertAsync(entity);
            Debug.WriteLine($"{entity.Id}");
            return entity.Id;
        }

    }
}
