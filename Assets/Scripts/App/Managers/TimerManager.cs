﻿using UnityEngine;
using System;
using System.Collections.Generic;

namespace TandC.RunIfYouWantToLive
{
    public class TimerManager : IService, ITimerManager
    {
        private int timersCount = 0;

        private List<Timer> _timers;

        private static object _sync = new object();

        public void Dispose()
        {
            _timers.Clear();
        }

        public void Init()
        {
            _timers = new List<Timer>();
        }

        public void StopTimer(Action<object[]> handler)
        {
            for (int i = 0; i < _timers.Count; i++)
                if (_timers[i]._handler == handler)
                    _timers.RemoveAt(i);
        }

        public void StopTimer(int index)
        {
            var timer = _timers.Find((x) => x.index == index);

            if (_timers.Contains(timer))
                _timers.Remove(timer);
        }

        public void AddTimer(Action<object[]> handler, object[] parameters = null, float time = 1, bool loop = false, bool storeTimer = false)
        {
            Timer timer = new Timer(handler, parameters, time, loop);
            timer.index = timersCount++;


            if (storeTimer)
            {
                if (parameters != null)
                {
                    object[] newParams = new object[parameters.Length + 1];

                    for (int i = 0; i < parameters.Length; i++)
                        newParams[i] = parameters[i];


                    newParams[newParams.Length - 1] = timer.index;

                    parameters = newParams;
                }
                else
                {
                    parameters = new object[1];
                    parameters[0] = timer.index;
                }

                timer.parameters = parameters;
            }

            _timers.Add(timer);
        }

        public void Update()
        {
            lock (_sync)
            {
                for (int i = _timers.Count - 1; i > -1; i--)
                {
                    if (_timers.Count <= 0)
                        break;

                    if (i < _timers.Count && i > -1)
                    {
                        if (_timers[i] != null)
                        {
                            if (_timers[i].finished)
                                _timers.RemoveAt(i);
                            else
                                _timers[i].Update();
                        }
                    }
                }
            }
        }
    }

    public class Timer
    {
        private float _time, _currentTime;
        private bool _loop;
        public Action<object[]> _handler;
        public object[] parameters;
        public bool finished;
        public int index;


        public Timer(Action<object[]> handler, object[] parameters, float time, bool loop)
        {
            _handler = handler;
            this.parameters = parameters;
            _time = time;
            _currentTime = time;
            _loop = loop;
            finished = false;
        }

        public void Update()
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime < 0)
            {
                _handler(parameters);
                if (_loop)
                    _currentTime = _time;
                else
                    finished = true;
            }
        }
    }
}