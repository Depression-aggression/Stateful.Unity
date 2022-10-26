// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using UnityEngine;

namespace Depra.StateMachines.Unity.Runtime
{
    public class StateMachineInitialization : MonoBehaviour
    {
        private StateMachineBehaviour _stateMachine;

        private void Awake()
        {
            _stateMachine = GetComponent<StateMachineBehaviour>();
            
            Initialize();
        }

        private void Start()
        {
            _stateMachine.StartMachine();
        }

        /// <summary>
        /// Internally used within the framework to auto start the state machine.
        /// </summary>
        public void Initialize()
        {
            //turn off all states:
            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}