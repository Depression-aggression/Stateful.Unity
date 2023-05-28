// Copyright © 2022-2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using Depra.StateMachines.Abstract;
using Depra.StateMachines.Finite;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using static Depra.StateMachines.Unity.Runtime.Constants;

namespace Depra.StateMachines.Unity.Runtime
{
    [AddComponentMenu(MODULE_PATH + F_STATE_MACHINE)]
    [RequireComponent(typeof(StateMachineInitialization))]
    public sealed class StateMachineBehaviour : MonoBehaviour, IStateMachine
    {
        [SerializeField] private StateBehavior _startingState;
        [SerializeField] private StateBehavior _currentState;
        [SerializeField] private UnityEvent<StateBehavior> _onStateChanged;

        [Tooltip("Can States within this StateMachine be reentered?")] [SerializeField]
        private bool _allowReentry;

        [SerializeField] private bool _verbose;

        private IStateMachine _stateMachine;

        public event Action<IState> StateChanged;

        /// <summary>
        /// Are we at the first state in this state machine.
        /// </summary>
        [UsedImplicitly]
        public bool AtFirst { get; private set; }

        /// <summary>
        /// Are we at the last state in this state machine.
        /// </summary>
        [UsedImplicitly]
        public bool AtLast { get; private set; }

        /// <summary>
        /// Returns the current state.
        /// </summary>
        [UsedImplicitly]
        public StateBehavior CurrentState => _currentState;

        /// <summary>
        /// Returns the current state.
        /// <remarks>For compatibility with <see cref="IStateMachine"/> interface.</remarks>
        /// </summary>
        IState IStateMachine.CurrentState => _currentState;

        private void OnDestroy()
        {
            if (_stateMachine != null)
            {
                _stateMachine.StateChanged += OnStateChanged;
            }
        }

        /// <summary>
        /// Internally used within the framework to auto start the state machine.
        /// </summary>
        public void StartMachine()
        {
            _stateMachine = new StateMachine(_startingState, _allowReentry);
            _stateMachine.StateChanged += OnStateChanged;

            if (Application.isPlaying && _startingState != null)
            {
                ChangeState(_startingState);
            }
        }

        /// <summary>
        /// Change to the next state if possible.
        /// </summary>
        [UsedImplicitly]
        public StateBehavior Next(bool exitIfLast = false)
        {
            if (_currentState == null)
            {
                return ChangeState(0);
            }

            var currentIndex = _currentState.transform.GetSiblingIndex();
            if (currentIndex == transform.childCount - 1)
            {
                if (exitIfLast)
                {
                    Exit();
                    return null;
                }

                return _currentState;
            }

            return ChangeState(++currentIndex);
        }

        /// <summary>
        /// Change to the previous state if possible.
        /// </summary>
        [UsedImplicitly]
        public StateBehavior Previous(bool exitIfFirst = false)
        {
            if (_currentState == null)
            {
                return ChangeState(0);
            }

            var currentIndex = _currentState.transform.GetSiblingIndex();
            if (currentIndex == 0)
            {
                if (exitIfFirst)
                {
                    Exit();
                    return null;
                }

                return _currentState;
            }

            return ChangeState(--currentIndex);
        }

        /// <summary>
        /// Exit the current state.
        /// </summary>
        [UsedImplicitly]
        public void Exit()
        {
            if (_currentState == null)
            {
                return;
            }

            Log($"(-) {name} EXITED state: {_currentState.name}");

            var currentIndex = _currentState.transform.GetSiblingIndex();

            // No longer at first:
            if (currentIndex == 0)
            {
                AtFirst = false;
            }

            // No longer at last:
            if (currentIndex == transform.childCount - 1)
            {
                AtLast = false;
            }

            _currentState.Exit();
            _currentState = null;
        }

        /// <summary>
        /// Changes the state by state instance.
        /// </summary>
        /// <param name="state">New state</param>
        [UsedImplicitly]
        public StateBehavior ChangeState(StateBehavior state)
        {
            if (_currentState)
            {
                if (_allowReentry == false && state == _currentState)
                {
                    Log($"State change ignored. State machine {name} already in {state.name} state.");
                    return null;
                }
            }

            if (state.transform.parent != transform)
            {
                Log($"State {state.name} is not a child of {name} StateMachine state change canceled.");
                return null;
            }

            Enter(state);

            return _currentState;
        }

        /// <summary>
        /// Changes the state by sibling index.
        /// </summary>
        /// <param name="childIndex">Sibling index</param>
        [UsedImplicitly]
        public StateBehavior ChangeState(int childIndex)
        {
            if (childIndex > transform.childCount - 1)
            {
                Log($"Index is greater than the amount of states in the StateMachine {gameObject.name} " +
                    "please verify the index you are trying to change to.");
            }

            return ChangeState(transform.GetChild(childIndex).GetComponent<StateBehavior>());
        }

        /// <summary>
        /// Changes the state by name.
        /// </summary>
        /// <param name="state">State name</param>
        /// <returns></returns>
        [UsedImplicitly]
        public StateBehavior ChangeState(string state)
        {
            var found = transform.Find(state);
            if (found == false)
            {
                Log($"{name} does not contain a state by the name of {state} " +
                    "please verify the name of the state you are trying to reach.");

                return null;
            }

            return ChangeState(found.GetComponent<StateBehavior>());
        }

        private void Enter(StateBehavior state)
        {
            _currentState = state;
            var index = _currentState.transform.GetSiblingIndex();

            // Entering first:
            if (index == 0)
            {
                AtFirst = true;
            }

            // Entering last:
            if (index == transform.childCount - 1)
            {
                AtLast = true;
            }

            _stateMachine.ChangeState(state);

            Log($"(+) {name} ENTERED state {state.name}");
        }

        private void OnStateChanged(IState currentState)
        {
            StateChanged?.Invoke(currentState);
            _onStateChanged.Invoke(_currentState);
        }

        private void Log(string message)
        {
            if (_verbose)
            {
                Debug.Log(message, gameObject);
            }
        }

        /// <summary>
        /// Changes the state by state instance.
        /// </summary>
        /// <remarks>For compatibility with the <see cref="IStateMachine"/> interface.</remarks>
        /// <param name="state">New state</param>
        void IStateMachine.ChangeState(IState state) =>
            ChangeState(state as StateBehavior);
    }
}