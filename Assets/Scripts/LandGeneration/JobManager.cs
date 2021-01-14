using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Jobs;
using UnityEngine;

namespace LandGeneration
{
    public abstract class JobManager<TJob, TKey, TValue> : MonoBehaviour where TJob : struct, IJob  where TValue:class
    {
        [Min(1)]
        public int completionActionsPerFixedUpdate = 1;
        
        struct JobHolder
        {
            internal TJob job;
            internal JobHandle handle;
            internal float startTime;
            internal Action<TKey, TValue> completionAction;
        }
    
        private readonly Dictionary<TKey, JobHolder> _jobs = new Dictionary<TKey, JobHolder>();
        private readonly Dictionary<TKey, TValue> _results = new Dictionary<TKey, TValue>();

        [CanBeNull]
        private TValue CheckJobCompletion(TKey key, JobHolder job)
        {
            if (job.handle.IsCompleted)
            {
                job.handle.Complete();
                var result = GetResultFromJob(job.job);
                Debug.Log($"Job for key {key} completed in {Time.realtimeSinceStartup - job.startTime} seconds");
                return result;
            }
            return null;
        } 
        
        public void ScheduleJob(TKey key, Action<TKey, TValue> completionAction)
        {
            if (_jobs.ContainsKey(key))
            {
                Debug.Log($"Job already scheduled for key {key}");
                return;
            }
            
            var job = CreateJob(key);
            var jobHandle = job.Schedule();

            var jobHolder = new JobHolder()
            {
                job = job,
                handle = jobHandle,
                startTime = Time.realtimeSinceStartup,
                completionAction = completionAction
            };
            _jobs.Add(key, jobHolder);
        }
        
        public int priority;

        protected abstract TJob CreateJob(TKey position);

        protected abstract TValue GetResultFromJob(TJob job);

        private void FixedUpdate()
        {
            int completionActionsExecuted = 0;
            foreach (var key in _jobs.Keys.ToList())
            {
                var job = _jobs[key];
                var result = CheckJobCompletion(key, job);
                if (result == null)
                {
                    continue;
                }
                _jobs.Remove(key);
                _results[key] = result;
                job.completionAction.Invoke(key, result);
                completionActionsExecuted++;
                if (completionActionsExecuted >= completionActionsPerFixedUpdate)
                {
                    return;
                }
            }
        }
    }
}