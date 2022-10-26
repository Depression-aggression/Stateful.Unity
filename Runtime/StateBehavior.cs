// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using Depra.StateMachines.Domain;
using UnityEngine;
using UnityEngine.Events;

namespace Depra.StateMachines.Unity.Runtime
{
    public class StateBehavior : MonoBehaviour, IState
    {
        [SerializeField] private UnityEvent _onEnter;
        [SerializeField] private UnityEvent _onExit;

        public void Enter()
        {
            gameObject.SetActive(true);
            _onEnter.Invoke();
        }

        public void Exit()
        {
            gameObject.SetActive(false);
            _onExit.Invoke();
        }
    }
}