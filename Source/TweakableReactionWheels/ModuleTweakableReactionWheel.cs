// TweakableReactionWheels, a TweakableEverything module
//
// ModuleTweakableReactionWheel.cs
//
// Copyright © 2014, toadicus
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation and/or other
//    materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its contributors may be used
//    to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using KSP;
using System;
using System.Collections.Generic;
using ToadicusTools.DebugTools;
using ToadicusTools.Extensions;
using ToadicusTools.TweakableTools;
using UnityEngine;

namespace TweakableEverything
{
	#if DEBUG && false
	public class ModuleTweakableReactionWheel : DebugPartModule
	#else
	public class ModuleTweakableReactionWheel : PartModule
	#endif
	{
		// Stores the reaction wheel module we're tweaking.
		protected ModuleReactionWheel reactionWheelModule;

		// Stores our tweaked value for roll torque.
		[KSPField(isPersistant = true, guiName = "Roll Torque", guiUnits = "kN-m", guiFormat = "F2",
			guiActiveEditor = true)]
		[UI_FloatRange(scene = UI_Scene.Editor, stepIncrement = 1f)]
		public float RollTorque;

		// Stores our tweaked value for pitch torque.
		[KSPField(isPersistant = true, guiName = "Pitch Torque", guiUnits = "kN-m", guiFormat = "F2",
			guiActiveEditor = true)]
		[UI_FloatRange(scene = UI_Scene.Editor, stepIncrement = 1f)]
		public float PitchTorque;

		// Stores our tweaked value for yaw torque.
		[KSPField(isPersistant = true, guiName = "Yaw Torque", guiUnits = "kN-m", guiFormat = "F2",
			guiActiveEditor = true)]
		[UI_FloatRange(scene = UI_Scene.Editor, stepIncrement = 1f)]
		public float YawTorque;

		// Stores our value for all-axis torque gain
		[KSPField(isPersistant = true, guiName = "Torque Limiter", guiFormat = "P0",
			guiActive = true, guiActiveEditor = false)]
		[UI_FloatRange(minValue = 0f, maxValue = 1f, stepIncrement = .02f)]
		public float TorqueGain;

		// Construct ALL the objects.
		public ModuleTweakableReactionWheel()
		{
			this.TorqueGain = 1f;
		}

		public override void OnLoad(ConfigNode node)
		{
			log.info("OnLoad for {0}", this.part.name);
			base.OnLoad(node);
			this.Init();
		}

		public override void OnStart(StartState state)
		{
			log.info("OnStart for {0}", this.part.name);
			base.OnStart(state);
			this.Setup();
		}

		// Runs late in the update cycle
		public void LateUpdate()
		{
			if (HighLogic.LoadedSceneIsFlight && this.reactionWheelModule != null)
			{
				this.reactionWheelModule.RollTorque = this.RollTorque * this.TorqueGain;
				this.reactionWheelModule.PitchTorque = this.PitchTorque * this.TorqueGain;
				this.reactionWheelModule.YawTorque = this.YawTorque * this.TorqueGain;
			}
		}

		private void Init()
		{
			log.dbg("Init {0}", this.part.name);

			if (!this.part.tryGetFirstModuleOfType<ModuleReactionWheel>(out this.reactionWheelModule))
			{
				log.warn("Failed to locate a ModuleReactionWheel for {0}!", this.part.name);
				return;
			}

			log.info("Found prefab module {0} for partInfo {1} .", this.reactionWheelModule, this.part.name);
			this.RollTorque = this.reactionWheelModule.RollTorque;
			this.PitchTorque = this.reactionWheelModule.PitchTorque;
			this.YawTorque = this.reactionWheelModule.YawTorque;

			{
				BaseField field = this.Fields["TorqueGain"];
				UI_Control torqueGainCtl = field.uiControlCurrent();
				if (torqueGainCtl is UI_FloatRange)
				{
					UI_FloatRange torqueGainSlider = torqueGainCtl as UI_FloatRange;

					torqueGainSlider.maxValue = 1f;
					torqueGainSlider.stepIncrement = 0.025f;
				}
			}
		}

		private void Setup()
		{
			log.dbg("Setup {0}", this.part.name);

			this.TorqueGain = Math.Min(Math.Max(this.TorqueGain, 0f), 1f);

			AvailablePart ap = PartLoader.getPartInfoByName(this.part.name);
			if (null == ap || null == ap.partPrefab)
			{
				log.warn("Got a null on AvailablePart in partPrefab !");
				return;
			}

			ModuleReactionWheel prefabModule;
			if (ap.partPrefab.tryGetFirstModuleOfType<ModuleReactionWheel>(out prefabModule))
			{
				log.info("Found prefab module {0} for partInfo {1} .", prefabModule, this.part.partInfo.name);

				TweakableTools.InitializeTweakable<ModuleTweakableReactionWheel>(
					this.Fields["RollTorque"].uiControlCurrent(),
					ref this.RollTorque,
					ref this.reactionWheelModule.RollTorque,
					prefabModule.RollTorque
				);
				log.info("RollTorque setup");

				TweakableTools.InitializeTweakable<ModuleTweakableReactionWheel>(
					this.Fields["PitchTorque"].uiControlCurrent(),
					ref this.PitchTorque,
					ref this.reactionWheelModule.PitchTorque,
					prefabModule.PitchTorque
				);
				log.info("PitchTorque setup");

				TweakableTools.InitializeTweakable<ModuleTweakableReactionWheel>(
					this.Fields["YawTorque"].uiControlCurrent(),
					ref this.YawTorque,
					ref this.reactionWheelModule.YawTorque,
					prefabModule.YawTorque
				);
				log.info("YawTorque setup");
			}
		}

		private static readonly KSPe.Util.Log.Logger log = KSPe.Util.Log.Logger.CreateForType<ModuleTweakableReactionWheel>("ModuleTweakableReactionWheel");
		static ModuleTweakableReactionWheel()
		{
			log.level = KSPe.Util.Log.Level.DETAIL;
		}
	}
}
