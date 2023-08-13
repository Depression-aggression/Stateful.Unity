// Copyright © 2022-2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using UnityEngine.Events;

namespace Depra.Stateful.Unity.Runtime
{
	[Serializable]
	internal sealed class UnityStateEvent : UnityEvent<StateBehavior> { }
}