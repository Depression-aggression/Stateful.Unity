// Copyright © 2022-2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using Depra.Stateful.Abstract;
using UnityEngine;
using UnityEngine.Events;
using static Depra.Stateful.Unity.Runtime.Constants;

namespace Depra.Stateful.Unity.Runtime
{
	[AddComponentMenu(MODULE_PATH + DISPLAY_NAME)]
	public sealed class StateBehavior : MonoBehaviour, IState
	{
		private const string DISPLAY_NAME = "State";

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