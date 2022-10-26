// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using Depra.StateMachines.Application;
using Depra.StateMachines.Domain;
using UnityEngine;

namespace Depra.StateMachines.Unity.Runtime
{
    [RequireComponent(typeof(StateMachineInitialization))]
    public class StateMachineBehaviour : MonoBehaviour
    {
        [SerializeField] private StateBehavior _startingState;
        [SerializeField] private StateBehavior _currentState;

        [Tooltip("Can States within this StateMachine be reentered?")] [SerializeField]
        private bool _allowReentry;

        [SerializeField] private bool _verbose;

        private IStateMachine _stateMachine;

        public StateBehavior CurrentState => _currentState;

        /// <summary>
        /// Are we at the first state in this state machine.
        /// </summary>
        public bool AtFirst { get; private set; }

        /// <summary>
        /// Are we at the last state in this state machine.
        /// </summary>
        public bool AtLast { get; private set; }

        private IStateMachine StateMachine => _stateMachine ??= new StateMachine(_startingState);

        private void Update()
        {
            Tick();
        }

        /// <summary>
        /// Internally used within the framework to auto start the state machine.
        /// </summary>
        public void StartMachine()
        {
            // Start the machine:
            if (UnityEngine.Application.isPlaying && _startingState != null)
            {
                ChangeState(_startingState);
            }
        }

        /// <summary>
        /// Change to the next state if possible.
        /// </summary>
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
        /// Changes the state by sibling index.
        /// </summary>
        /// <param name="state">New state</param>
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
        public StateBehavior ChangeState(string state)
        {
            var found = transform.Find(state);
            if (found == false)
            {
                Log($"{name} does not contain a state by the name of {state} " +
                    $"please verify the name of the state you are trying to reach.");

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

            StateMachine.ChangeState(state);

            Log($"(+) {name} ENTERED state {state.name}");
        }

        private void Tick()
        {
            if (StateMachine.NeedTransition(out var nextState))
            {
                Enter(nextState as StateBehavior);
            }
        }

        private void Log(string message)
        {
            if (_verbose)
            {
                Debug.Log(message, gameObject);
            }
        }
    }
}