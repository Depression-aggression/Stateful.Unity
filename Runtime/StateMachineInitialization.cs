// Copyright © 2022-2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using UnityEngine;
using static Depra.Stateful.Unity.Runtime.Constants;

namespace Depra.Stateful.Unity.Runtime
{
	[AddComponentMenu(MODULE_PATH + DISPLAY_NAME)]
	[RequireComponent(typeof(StateMachineBehaviour))]
	internal sealed class StateMachineInitialization : MonoBehaviour
	{
		private const string DISPLAY_NAME = StateMachineBehaviour.DISPLAY_NAME + " Initialization";

		private StateMachineBehaviour _machine;

		private void Awake() => Initialize(GetComponent<StateMachineBehaviour>());

		private void Start() => _machine.StartMachine();

		/// <summary>
		/// Internally used within the framework to auto start the state machine.
		/// </summary>
		private void Initialize(StateMachineBehaviour machine)
		{
			_machine = machine ? machine : throw new ArgumentNullException(nameof(machine));
			for (var index = 0; index < transform.childCount; index++)
			{
				transform.GetChild(index).gameObject.SetActive(false);
			}
		}
	}
}